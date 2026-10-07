using NetFlixApp.MODEL;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;

namespace NetFlixApp.DADES
{
    public class DAOCsvImpl : IDAO
    {
        #region Atributs
        private const string FILENAME = "raw_titles.csv";

        //private const string PATH = @"..\..\..\DADES\DATA";
        //Fa el mateix que hem posat a dalt, però amb Path.Combine i Path.GetFullPath per a que sigui més portable.
        private static readonly string PATH = Path.GetFullPath(
            Path.Combine(
                AppContext.BaseDirectory, "..", "..", "..", "DADES", "DATA")
            );

        private const string PATTERN = @",(?=(?:[^""]*""[^""]*"")*[^""]*$)";
        private const string LIST_ITEM_PATTERN = @"'([^']*)'";


        #endregion

        #region Constructor
        public DAOCsvImpl()
        {
        }
        #endregion

        #region Methods
        // No cal posar el summary en aquests mètodes ja que ja estan a la interfície IDAO i per tant ja tenim la documentació de cada mètode a la interfície.

        //Algorisme U-NK -> genre no és clau única i no està ordenat [ RECORREGUT ]

        //Exercici 1: Genera un fitxer de text (outputFile) que conté només registres del gènere especificat.
        //El fitxer ha de ser un CSV amb un registre per linia amb els següents camps: Retorna el nombre de registres trobats.
        //index;id;title;genres 

        //Utilitzem els mètodes privats per a obtenir els camps i la llista de gèneres.
        public int SelectByGenre(string genre, string outputFile)
        {
            genre = genre.ToLower().Trim();
            int count = 0;

            string pathFileName = Path.Combine(PATH, FILENAME);

            // Procés - Llegim fitxer
            using (FileStream fsRawTitles = new FileStream(pathFileName, FileMode.Open, FileAccess.Read)) // com que aquest using solament té un using per a srRawTitles, no cal posar les claus {} per a fsRawTitles, ja que es tancarà automàticament quan surti del using de srRawTitles.
            using (StreamReader srRawTitles = new StreamReader(fsRawTitles))
            {
                StreamWriter? swRawTitles = null;

                string? linia;
                srRawTitles.ReadLine(); // Saltem la primera linia que és l'encapçalament del fitxer CSV.
                while ((linia = srRawTitles.ReadLine()) != null)
                {
                    var rawTitle = CrearRawTitle(linia); // Creem un objecte RawTitle a partir de la linia del fitxer CSV. (mètode privat)
                    #region Comentari fer-ho amb un bucle while
                    //Podem fer-ho amb un bucle while, però també podem fer-ho amb una sentència LinQ, que és més neta i llegible. La sentència LinQ és la que està comentada a sota.
                    //if (rawTitle != null)
                    //{
                    //    int i = 0; bool trobat = false;
                    //    while (i < rawTitle?.Genres?.Count && !trobat)
                    //    {
                    //        if (rawTitle?.Genres[i]?.ToLower().Trim() == genre)
                    //        {
                    //            trobat = true;
                    //            //Posar aqui el tractament del registre trobat, per exemple escriure'l al fitxer de sortida.
                    //        }
                    //        i++;
                    //    } 
                    //}
                    #endregion

                    if (rawTitle != null && rawTitle.Genres != null && rawTitle.Genres.Any(g => g.ToLower().Trim() == genre)) // Comprovem amb un LinQ si el gènere especificat està a la llista de gèneres de l'objecte RawTitle.
                    {
                        //el podeu crear inicialment i fer-ho amb un using que és més net, però he volgut obrir-lo només quan trobem el primer registre que compleix la condició, per a no crear un fitxer buit si no hi ha cap registre que compleixi la condició.
                        if (swRawTitles == null)
                        {
                            swRawTitles = new StreamWriter(Path.Combine(PATH, outputFile));
                            swRawTitles.WriteLine("index;id;title;genres"); // Escrivim l'encapçalament del fitxer CSV de sortida sols 1 vegada.
                        }
                        //swRawTitles.WriteLine($"{rawTitle.Index};{rawTitle.Id};{RawTitle.EscaparCsv(rawTitle.Title)};{LlistaToString(rawTitle.Genres)}");
                        swRawTitles.WriteLine($"{rawTitle.Index};{rawTitle.Id};{rawTitle.Title};{LlistaToString(rawTitle.Genres)}");
                        count++;
                    }
                }
                swRawTitles?.Close();
            }

            return count;
        }

        //Algorisme S-K -> index és CLAU i està ORDENADA -> [ CERCA ordenada amb control de sobrepassar l'index ]
        public RawTitle SelectByIndex(int index)
        {
            // Com que l'index és un valor positiu, si és negatiu llancem una excepció.
            if (index < 0)
                throw new ArgumentException("L'índex ha de ser un valor positiu.");

            bool trobat = false;
            bool passat = false;
            RawTitle? rawTitle = null;

            using (FileStream fsRawTitles = new FileStream(Path.Combine(PATH, FILENAME), FileMode.Open, FileAccess.Read))
            using (StreamReader srRawTitles = new StreamReader(fsRawTitles))
            {
                srRawTitles.ReadLine(); // Saltem la primera linia que és l'encapçalament del fitxer CSV.
                string? linia;
                linia = srRawTitles.ReadLine();

                while (linia != null && !trobat && !passat)
                {
                    var camps = ObtenirCamps(linia);
                    if (camps.Count > 0) // Ens assegurem de no trobar-nos una linia buida o mal formada, que podria donar un error al intentar convertir camps[0] a int.
                    {
                        int indexLinia = Convert.ToInt32(camps[0]);
                        if (indexLinia == index)
                        {
                            trobat = true;
                        }
                        else if (indexLinia > index) // Com que l'index està ordenat, si el rawTitle.Index és més gran que l'index que busquem, ja no cal seguir llegint el fitxer.
                            passat = true;
                        else
                            linia = srRawTitles.ReadLine();
                    }
                    else
                    {
                        linia = srRawTitles.ReadLine(); // Si la linia està buida o mal formada, simplement passem a la següent linia.
                    }
                }
                if (trobat)
                    rawTitle = CrearRawTitle(linia);
            }

            return rawTitle;
        }
        //Algorisme U-K -> ID és clau ÚNICA però no està ORDENADA -> [CERCA segons id]
        public RawTitle SelectById(string id)
        {
            bool trobat = false;
            RawTitle? rawTitle = null;

            using (FileStream fsRawTitles = new FileStream(Path.Combine(PATH, FILENAME), FileMode.Open, FileAccess.Read))
            using (StreamReader srRawTitles = new StreamReader(fsRawTitles))
            {
                srRawTitles.ReadLine(); // Saltem la primera linia que és l'encapçalament del fitxer CSV.
                string? linia;
                linia = srRawTitles.ReadLine();

                while (linia != null && !trobat)
                {
                    var camps = ObtenirCamps(linia);
                    if (camps.Count > 1) // Ens assegurem de no trobar-nos una linia buida o mal formada, que podria donar un error al intentar convertir camps[0] a int.
                    {
                        string idLinia = camps[1] ?? "";
                        if (idLinia == id)
                            trobat = true;
                        else
                            linia = srRawTitles.ReadLine();
                    }
                    else
                    {
                        linia = srRawTitles.ReadLine(); // Si la linia està buida o mal formada, simplement passem a la següent linia.
                    }
                }
                if (trobat)
                    rawTitle = CrearRawTitle(linia);
            }

            return rawTitle;
        }

        public RawTitle[] ReadTitles(int index, int length)
        {
            if (index < 0 || length < 0)
                throw new ArgumentException("L'índex i la longitud han de ser valors positius.");

            // Vaig a suposar que si l'index no es troba agafarem els següents length registres a partir de l'index que es troba més proper, ja que no s'ha especificat què fer si l'index no es troba.
            var pelicules = new List<RawTitle>();
            bool passat = false;
            bool trobat = false;

            using (FileStream fsRawTitles = new FileStream(Path.Combine(PATH, FILENAME), FileMode.Open, FileAccess.Read))
            using (StreamReader srRawTitles = new StreamReader(fsRawTitles))
            {
                srRawTitles.ReadLine(); // Saltem la primera linia que és l'encapçalament del fitxer CSV.
                string? linia;
                linia = srRawTitles.ReadLine();

                while (linia != null && !trobat && !passat)
                {
                    var camps = ObtenirCamps(linia);
                    if (camps.Count > 0) // Ens assegurem de no trobar-nos una linia buida o mal formada, que podria donar un error al intentar convertir camps[0] a int.
                    {
                        int indexLinia = string.IsNullOrEmpty(camps[0]) ? -1 : Convert.ToInt32(camps[0]);
                        if (indexLinia == index)
                        {
                            trobat = true;
                        }
                        else if (indexLinia > index) // Com que l'index està ordenat, si el rawTitle.Index és més gran que l'index que busquem, ja no cal seguir llegint el fitxer.
                            passat = true;
                        else
                            linia = srRawTitles.ReadLine();
                    }
                }
                if (trobat || passat)
                {
                    int contador = 0;
                    // Si hem trobat l'index o l'hem passat, afegim els següents length registres a partir de la linia actual.
                    while (linia != null && contador < length)
                    {
                        var rawTitle = CrearRawTitle(linia);
                        if (rawTitle != null)
                        {
                            pelicules.Add(rawTitle);
                            contador++;
                        }
                        linia = srRawTitles.ReadLine();
                        
                    }
                }
            }
            return pelicules.ToArray(); // Retornem un array de RawTitle amb els registres trobats.
        }

        public void PreMerge(RawTitle[] titles, string outputFileName)
        {
            Array.Sort(titles);

            using (FileStream fsRawTitles = new FileStream(Path.Combine(PATH, outputFileName), FileMode.Create, FileAccess.Write))
            using (StreamWriter swRawTitles = new StreamWriter(fsRawTitles))
            {
                swRawTitles.WriteLine("index;id;title;type;release_year;age_certification;runtime;genres;production_countries;seasons;imdb_id;imdb_score;imdb_votes"); // Escrivim l'encapçalament del fitxer CSV de sortida sols 1 vegada.
                foreach (var title in titles)
                {
                    swRawTitles.WriteLine(title);
                }
            }
        }

        public int Merge(string inputFileName1, string inputFilename2, string outputFileName)
        {
            int contador = 0;
            using (FileStream fsRawTitles = new FileStream(Path.Combine(PATH, inputFileName1), FileMode.Open, FileAccess.Read))
            using (FileStream fsRawTitles2 = new FileStream(Path.Combine(PATH, inputFilename2), FileMode.Open, FileAccess.Read))
            using (FileStream fsOutput = new FileStream(Path.Combine(PATH, outputFileName), FileMode.Create, FileAccess.Write))
            using (StreamReader srRawTitles = new StreamReader(fsRawTitles))
            using (StreamReader srRawTitles2 = new StreamReader(fsRawTitles2))
            using (StreamWriter swOutput = new StreamWriter(fsOutput))
            {
                RawTitle rt1, rt2;
                string linia1, linia2;
                srRawTitles.ReadLine(); // Saltem la primera linia que és l'encapçalament del fitxer CSV.
                srRawTitles2.ReadLine(); // Saltem la primera linia que és l'encapçalament del fitxer CSV.
                linia1 = srRawTitles.ReadLine();
                linia2 = srRawTitles2.ReadLine();

                if (linia1 != null || linia2 != null)
                {
                    // Si almenys un dels fitxers té dades, escrivim l'encapçalament al fitxer de sortida.
                    swOutput.WriteLine("index;id;title;type;release_year;age_certification;runtime;genres;production_countries;seasons;imdb_id;imdb_score;imdb_votes");
                }
                while (linia1 != null && linia2 != null)
                {
                    // Revisar si rt1.ImdbScore i rt2.ImdbScore són nulls abans de comparar-los, ja que poden ser nulls segons la definició de la classe RawTitle.
                    rt1 = CrearRawTitle(linia1);
                    rt2 = CrearRawTitle(linia2);
                    if(rt1.ImdbScore < rt2.ImdbScore)
                    {
                        swOutput.WriteLine(rt1);
                        linia1 = srRawTitles.ReadLine();
                    }
                    else if (rt1.ImdbScore.Equals(rt2.ImdbScore))// Si rt1.ImdbScore i rt2.ImdbScore són iguals hem de comprovar si son iguals per a no escriure duplicats al fitxer de sortida.
                    {
                        if (rt1.Equals(rt2)) // Si són iguals, només escrivim un dels dos i avancem amb les dues línies.
                            swOutput.WriteLine(rt1);
                        else
                        {
                            swOutput.WriteLine(rt1);
                            swOutput.WriteLine(rt2);
                            contador++;
                        }

                        linia1 = srRawTitles.ReadLine();
                        linia2 = srRawTitles2.ReadLine();
                    }
                    else
                    {
                        swOutput.WriteLine(rt2);
                        linia2 = srRawTitles2.ReadLine();
                    }
                    contador++;
                }
                while(linia1 != null)
                {
                    swOutput.WriteLine(linia1);
                    linia1 = srRawTitles.ReadLine();
                    contador++;
                }
                while(linia2 != null)
                {
                    swOutput.WriteLine(linia2);
                    linia2 = srRawTitles2.ReadLine();
                    contador++;
                }
            }

            return contador;// Retornem el nombre de registres escrits al fitxer de sortida.
        } 
        #endregion

        #region Private Methods -> Regular Expressions 
        //per obtenir un objecte RawTitle a partir d'una linia del fitxer CSV

        /// <summary>
        /// Obté els camps d'una linia del fitxer CSV. Els camps estan separats per comes, però les comes dins de cometes no es consideren separadors.
        /// </summary>
        /// <param name="linia">La linia del fitxer CSV</param>
        /// <returns>La llista de camps, sense processar les subllistes de gèneres o països de producció</returns>
        private static List<string?> ObtenirCamps(string? linia)
        {
            var camps = Regex.Split(linia ?? "", PATTERN).ToList();
            return camps;
        }

        /// <summary>
        /// Obté la llista de gèneres a partir d'una linia del fitxer CSV. La llista de gèneres està representada com una cadena (string) de text amb el format:
        /// "['Gènere1','Gènere2',...]"  
        /// </summary>
        /// <param name="generesString">la linia de generes o paisos de producció</param>
        /// <returns>la llista </returns>
        //private static List<string>? ObtenirLlista(string? generesString)
        //{
        //    var llista = Regex.Split(generesString ?? "", LIST_ITEM_PATTERN).ToList();
        //    return llista;
        //}
        private static List<string> ObtenirLlista(string? generesString)
        {
            var llista = new List<string>();

            if (string.IsNullOrWhiteSpace(generesString) || generesString == "[]")
                return llista;

            MatchCollection coincidencies = Regex.Matches(generesString, LIST_ITEM_PATTERN);

            foreach (Match coincidencia in coincidencies)
            {
                llista.Add(coincidencia.Groups[1].Value);
            }

            return llista;
        }



        /// <summary>
        /// Crea un objecte RawTitle a partir d'una linia del fitxer CSV. Utilitzant els mètodes privats ObtenirCamps i ObtenirLlista per a obtenir els camps i la llista de gèneres.
        /// </summary>
        /// <param name="linia">string que representa una linia del fitxer CSV</param>
        /// <returns>Objecte RawTitle creat a partir de la linia</returns>
        private static RawTitle? CrearRawTitle(string linia)
        {
            var camps = ObtenirCamps(linia);
            RawTitle rawTitle = null;
            try
            {
                if (camps.Count == 13)
                {
                    rawTitle = new RawTitle()
                    {
                        Index = Convert.ToInt32(camps[0] ?? "0"),
                        Id = camps[1],
                        Title = camps[2],
                        Type = camps[3],
                        ReleaseYear = string.IsNullOrEmpty(camps[4]) ? null : Convert.ToInt32(camps[4]),
                        AgeCertification = camps[5],
                        Runtime = string.IsNullOrEmpty(camps[6]) ? null : Convert.ToInt32(camps[6]),
                        Genres = ObtenirLlista(camps[7]),
                        ProductionCountries = ObtenirLlista(camps[8]),
                        Seasons = string.IsNullOrEmpty(camps[9]) ? null : Convert.ToDouble(camps[9], CultureInfo.InvariantCulture),
                        ImdbId = camps[10],
                        ImdbScore = string.IsNullOrEmpty(camps[11]) ? null : Convert.ToDouble(camps[11], CultureInfo.InvariantCulture),
                        ImdbVotes = string.IsNullOrEmpty(camps[12]) ? null : Convert.ToDouble(camps[12], CultureInfo.InvariantCulture)
                    };
                }
                else
                    throw new Exception($"La linia no té prou camps: {linia}");

            }
            catch (Exception ex)
            {
                rawTitle = null;
            }
            return rawTitle;
        }

        /// <summary>
        /// Converteix una List<string> al format:
        /// ssi els valors són null o la llista està buida -> ""
        /// si tenim un element -> ['element'] 
        /// si tenim diversos elements -> "['element1','element2', ..., 'elementN']"
        /// </summary>
        /// <param name="values">La llista de strings a convertir.</param>
        /// <returns>La representació en format CSV de la llista.</returns>
        private string LlistaToString(List<string>? values)
        {
            string resultat;
            if (values == null || values.Count == 0)
                resultat = string.Empty;

            else if (values.Count == 1)
                resultat = $"['{values[0]}']";

            else
                resultat = $"\"['{string.Join("','", values)}']\""; ;

            return resultat;
        }


        #endregion

    }
}
