using System.Globalization;

namespace NetFlixApp.MODEL
{
    public class RawTitle : IComparable<RawTitle>, IEquatable<RawTitle>
    {

        public int Index { get; set; }

        public string? Id { get; set; }

        public string? Title { get; set; }

        public string? Type { get; set; }

        public int? ReleaseYear { get; set; }

        public string? AgeCertification { get; set; }

        public int? Runtime { get; set; }

        public List<string>? Genres { get; set; }

        public List<string>? ProductionCountries { get; set; }

        public double? Seasons { get; set; }

        public string? ImdbId { get; set; }

        public double? ImdbScore { get; set; }

        public double? ImdbVotes { get; set; }

        /// <summary>
        /// Compara dos objectes RawTitle segons el seu ImdbScore
        /// </summary>
        public int CompareTo(RawTitle? other)
        {
            int resultat;
            if (other == null)
                resultat = 1;

            else
                resultat = Nullable.Compare(ImdbScore, other.ImdbScore);

            return resultat;
        }

        /// <summary>
        /// Sobreescrivim Equals perquè funcioni també amb object.
        /// </summary>
        public override bool Equals(object? obj)
        {
            return Equals(obj as RawTitle);
        }

        /// <summary>
        /// Dos RawTitle es consideren iguals si tenen el mateix Id.
        /// </summary>
        public bool Equals(RawTitle? other)
        {
            bool resultat;
            if (other == null)
                resultat = false;
            else
                resultat = string.Equals(Id, other.Id);

            return resultat;
        }

        /// <summary>
        /// Necessari perquè Equals i GetHashCode siguin coherents.
        /// </summary>
        public override int GetHashCode()
        {
            // Utilitzem l'id per generar el hash code, ja que és el camp que utilitzem per comparar la igualtat.
            return StringComparer.Ordinal.GetHashCode(Id ?? string.Empty);
        }

        /// <summary>
        /// Converteix l'objecte al format CSV original.
        ///
        /// Les llistes es representen com:
        /// "['element1','element2']"
        ///
        /// Els valors null es representen com un camp buit.
        /// </summary>
        public override string ToString()
        {
            //string rawTitleString = $"{Index},{Id},{Title},{Type},{ReleaseYear},{AgeCertification},{Runtime},{LlistaToString(Genres)},{LlistaToString(ProductionCountries)},{Seasons},{ImdbId},{ImdbScore},{ImdbVotes}";
            string resultat = string.Join(",",
                Index,
                EscaparCsv(Id),
                EscaparCsv(Title),
                EscaparCsv(Type),
                ReleaseYear?.ToString(CultureInfo.InvariantCulture),
                EscaparCsv(AgeCertification),
                Runtime?.ToString(CultureInfo.InvariantCulture),
                LlistaToString(Genres),
                LlistaToString(ProductionCountries),
                Seasons?.ToString(CultureInfo.InvariantCulture),
                EscaparCsv(ImdbId),
                ImdbScore?.ToString(CultureInfo.InvariantCulture),
                ImdbVotes?.ToString(CultureInfo.InvariantCulture)
            );
            return resultat;

            //He estat revisant el guany per utilitzar StringBuilder en lloc de string.Join, i en aquest cas, per a un nombre limitat de camps, la diferència de rendiment és mínima.
            //En cas de fer un bucle amb concatenacions si seria plenament recomanable utilitzar StringBuilder, però en aquest cas, amb un nombre fix de camps, string.Join és més net i llegible.

            //var sb = new StringBuilder();

            //sb.Append(Index); sb.Append(',');
            //sb.Append(Id); sb.Append(',');
            //sb.Append(Title); sb.Append(',');
            //sb.Append(Type); sb.Append(',');
            //sb.Append(ReleaseYear); sb.Append(',');
            //sb.Append(AgeCertification); sb.Append(',');
            //sb.Append(Runtime); sb.Append(',');
            //sb.Append(LlistaToString(Genres)); sb.Append(',');
            //sb.Append(LlistaToString(ProductionCountries)); sb.Append(',');
            //sb.Append(Seasons); sb.Append(',');
            //sb.Append(ImdbId); sb.Append(',');
            //sb.Append(ImdbScore); sb.Append(',');
            //sb.Append(ImdbVotes);

            //return sb.ToString();
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
        /// <summary>
        /// Escapa un valor perquè sigui segur per a ser utilitzat en un fitxer CSV, ens permet posar " dintre d'un camp que te algun caracter especial com , o " o salts de línia.
        /// </summary>
        /// <param name="valor">El valor a escapar.</param>
        /// <returns>El valor escapat.</returns>
        public static string EscaparCsv(string? valor)
        {
            if (string.IsNullOrEmpty(valor))
                return string.Empty;
            //Detecta si trobem algun caracter , o " dintre d'un camp per poder escapar-lo correctament. També escapa els salts de línia.
            if (valor.Contains(',') || valor.Contains('"') || valor.Contains('\n'))
            {
                valor = valor.Replace("\"", "\"\"");
                return $"\"{valor}\"";
            }

            return valor;
        }


    }
}
