using NetFlixApp.DADES;
using NetFlixApp.MODEL;
using System.Windows;
using System.Windows.Controls;

namespace NetFlixApp.VISTA
{
    /// <summary>
    /// Lógica de interacción para BuscarPeliculaIdUC.xaml
    /// </summary>
    public partial class BuscarPeliculaIdUC : UserControl
    {
        IDAO dao;
        public BuscarPeliculaIdUC(IDAO dao)
        {
            this.dao = dao;
            InitializeComponent();
        }

        private void BtnBuscar_Click(object sender, RoutedEventArgs e)
        {
            RawTitle? pelicula = dao.SelectById(TxtId.Text);
            if (pelicula == null)
            {
                MessageBox.Show("No s'ha trobat cap pel·lícula amb aquest id.");
                TxtId.Clear();
                NetejarCamps();
            }

            else
                MostrarPelicula(pelicula);
        }
        private void MostrarPelicula(RawTitle pelicula)
        {
            TxtResultIndex.Text = pelicula.Index.ToString();
            TxtResultId.Text = pelicula.Id;
            TxtResultTitle.Text = pelicula.Title;
            TxtResultType.Text = pelicula.Type;
            TxtResultYear.Text = pelicula.ReleaseYear?.ToString();
            TxtResultCertification.Text = pelicula.AgeCertification;
            TxtResultRuntime.Text = pelicula.Runtime?.ToString();

            TxtResultGenres.Text =
                string.Join(", ", pelicula.Genres ?? new List<string>());

            TxtResultCountries.Text =
                string.Join(", ", pelicula.ProductionCountries ?? new List<string>());

            TxtResultSeasons.Text = pelicula.Seasons?.ToString();

            TxtResultImdbId.Text = pelicula.ImdbId;
            TxtResultImdbScore.Text = pelicula.ImdbScore?.ToString();
            TxtResultImdbVotes.Text = pelicula.ImdbVotes?.ToString();
        }
        private void NetejarCamps()
        {
            TxtId.Clear();
            TxtResultIndex.Text = string.Empty;
            TxtResultId.Text = string.Empty;
            TxtResultTitle.Text = string.Empty;
            TxtResultType.Text = string.Empty;
            TxtResultYear.Text = string.Empty;
            TxtResultCertification.Text = string.Empty;
            TxtResultRuntime.Text = string.Empty;
            TxtResultGenres.Text = string.Empty;
            TxtResultCountries.Text = string.Empty;
            TxtResultSeasons.Text = string.Empty;
            TxtResultImdbId.Text = string.Empty;
            TxtResultImdbScore.Text = string.Empty;
            TxtResultImdbVotes.Text = string.Empty;
        }
    }
}
