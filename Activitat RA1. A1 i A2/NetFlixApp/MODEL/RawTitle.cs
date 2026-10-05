using System.Text;

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
        /// Compara dos objectes RawTitle segons el seu Index.
        /// </summary>
        public int CompareTo(RawTitle? other)
        {
            int resultat;
            if (other == null)
                resultat = 1;

            else
                resultat = Index.CompareTo(other.Index);

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
        /// Dos RawTitle es consideren iguals si tenen el mateix Title.
        /// </summary>
        public bool Equals(RawTitle? other)
        {
            bool resultat;
            if (other == null)
                resultat = false;
            else
                resultat = string.Equals(Title, other.Title);

            return resultat;
        }

        /// <summary>
        /// Necessari perquè Equals i GetHashCode siguin coherents.
        /// </summary>
        public override int GetHashCode()
        {
            return Title?.GetHashCode(StringComparison.Ordinal) ?? 0;
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
            //string resultat = string.Join(",",
            //        Index,
            //        Id,
            //        Title,
            //        Type,
            //        ReleaseYear,
            //        AgeCertification,
            //        Runtime,
            //        LlistaToString(Genres),
            //        LlistaToString(ProductionCountries),
            //        Seasons,
            //        ImdbId,
            //        ImdbScore,
            //        ImdbVotes
            //        );
            //return resultat;

            var sb = new StringBuilder();

            sb.Append(Index); sb.Append(',');
            sb.Append(Id); sb.Append(',');
            sb.Append(Title); sb.Append(',');
            sb.Append(Type); sb.Append(',');
            sb.Append(ReleaseYear); sb.Append(',');
            sb.Append(AgeCertification); sb.Append(',');
            sb.Append(Runtime); sb.Append(',');
            sb.Append(LlistaToString(Genres)); sb.Append(',');
            sb.Append(LlistaToString(ProductionCountries)); sb.Append(',');
            sb.Append(Seasons); sb.Append(',');
            sb.Append(ImdbId); sb.Append(',');
            sb.Append(ImdbScore); sb.Append(',');
            sb.Append(ImdbVotes);

            return sb.ToString();
        }

        /// <summary>
        /// Converteix una List<string> al format:
        /// ssi els valors són null o la llista està buida -> ""
        /// si tenim un element -> ['element'] 
        /// si tenim diversos elements -> "['element1','element2','elementN']"
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

    }
}
