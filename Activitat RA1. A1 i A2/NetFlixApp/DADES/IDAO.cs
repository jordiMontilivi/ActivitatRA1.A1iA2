using System;
using System.Collections.Generic;
using System.Text;
using NetFlixApp.MODEL;

namespace NetFlixApp.DADES
{
    public interface IDAO
    {
        /// <summary>
        /// Selecciona els registres que compleixen amb el filtre de gènere i els escriu en un fitxer de sortida.
        /// el fitxer solament tindrà els registres -> index;id;title;genres
        /// </summary>
        /// <param name="genre"> Genere especificat per al filtre</param>
        /// <param name="outputFile">nom del fitxer de sortida</param>
        /// <returns>nombre de registres trobats </returns>
        int SelectByGenre(string genre, string outputFile);

        /// <summary>
        /// Selecciona el registre que coincideix amb l'index del fitxer de dades i el retorna com a objecte RawTitle
        /// index es Sorted Key (SK)
        /// </summary>
        /// <param name="index">index del registre a seleccionar</param>
        /// <returns>Objecte RawTitle del registre seleccionat, null si no es troba</returns>
        RawTitle SelectByIndex(int index);

        /// <summary>
        /// Selecciona el registre que coincideix amb l'id del fitxer de dades i el retorna com a objecte RawTitle.
        /// </summary>
        /// <param name="id">id del registre a seleccionar</param>
        /// <returns>Objecte RawTitle del registre seleccionat, null si no es troba</returns>
        RawTitle SelectById(string id);

        /// <summary>
        /// Selecciona els registres que coincideixen amb l'index i la longitud especificats del fitxer de dades i els retorna com a array d'objectes RawTitle.
        /// </summary>
        /// <param name="index">ide del registre a seleccionar</param>
        /// <param name="length">longitud dels registres a seleccionar</param>
        /// <returns>Array d'objectes RawTitle dels registres seleccionats. Si no hi ha prou registres com diu length retorna solament els que hi ha i reajusta l'array, null si no es troben</returns>
        RawTitle[] ReadTitles(int index, int length);

        /// <summary>
        /// Escriu en el fitxer outputFileName tots els RawTitles de l’array ordenats per imdb_score.
        /// </summary>
        /// <param name="titles">Array de RawTitles a escriure</param>
        /// <param name="outputFileName">Nom del fitxer de sortida</param>
        void PreMerge(RawTitle[] titles, string outputFileName);

        /// <summary>
        /// Fusionar els dos fitxers d'entrada generant un fitxer sortida amb tots els registres, conservant l’ordre per imdb_score
        /// </summary>
        /// <param name="inputFileName1">fitxer d'entrada 1 ordenat per imdb_score</param>
        /// <param name="inputFilename2">fitxer d'entrada 2 ordenat per imdb_score</param>
        /// <param name="outputFileName">fitxer de sortida ordenat per imdb_score</param>
        /// <returns></returns>
        int Merge(string inputFileName1, string inputFilename2, string outputFileName);
    }
}
