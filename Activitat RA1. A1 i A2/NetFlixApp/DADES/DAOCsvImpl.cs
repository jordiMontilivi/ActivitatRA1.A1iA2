using NetFlixApp.MODEL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace NetFlixApp.DADES
{
    public class DAOCsvImpl : IDAO
    {
        #region Atributs
        private const string FILENAME = "raw_titles.csv";
        private const string PATH = @"..\..\..\DADES\DATA";
        private const string PATTERN = @",(?=(?:[^""]*""[^""]*"")*[^""]*$)";
        private const string PATTERN2 = @"^(?:\['([^']*)'\]|""\['([^']*)'(?:\s*,\s*'([^']*)')*\]"")$";

        private FileStream fsRawTitles = null;
        private StreamReader srRawTitles = null;
        #endregion

        #region Constructor
        public DAOCsvImpl()
        {
            string pathFileName = Path.Combine(PATH, FILENAME);
            fsRawTitles = new FileStream(pathFileName, FileMode.Open, FileAccess.Read);
            srRawTitles = new StreamReader(fsRawTitles);
        }
        #endregion

        #region Methods

        // U-NK
        public int SelectByGenre(string genre, string outputFile)
        {
            throw new NotImplementedException();
        }

        public RawTitle SelectByIndex(int index)
        {
            throw new NotImplementedException();
        }

        public RawTitle SelectById(string id)
        {
            throw new NotImplementedException();
        }

        public RawTitle[] ReadTitles(int index, int length)
        {
            throw new NotImplementedException();
        }

        public void PreMerge(RawTitle[] titles, string outputFileName)
        {
            throw new NotImplementedException();
        }

        public int Merge(string inputFileName1, string inputFilename2, string outputFileName)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Private Methods -> Regular Expressions
        private static List<string?> ObtenirCamps(string? linia)
        {
            var camps = Regex.Split(linia ?? "", PATTERN).ToList();
            return camps;
        }

        private static List<string>? ObtenirLlista(string? linia)
        {
            var llista = Regex.Split(linia ?? "", PATTERN2).ToList();
            return llista;
        }

        private static RawTitle? CrearRawTitle(string linia)
        {
            var camps = ObtenirCamps(linia);

            RawTitle rawTitle = new RawTitle()
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
                Seasons = string.IsNullOrEmpty(camps[9]) ? null : Convert.ToDouble(camps[9]),
                ImdbId = camps[10],
                ImdbScore = string.IsNullOrEmpty(camps[11]) ? null : Convert.ToDouble(camps[11]),
                ImdbVotes = string.IsNullOrEmpty(camps[12]) ? null : Convert.ToDouble(camps[12])
            };

            return rawTitle;
        }


        #endregion

    }
}
