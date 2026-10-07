using NetFlixApp.DADES;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using NetFlixApp.MODEL;

namespace NetFlixApp.VISTA
{
    /// <summary>
    /// Lógica de interacción para LlistaPeliculesUC.xaml
    /// </summary>
    public partial class LlistaPeliculesUC : UserControl
    {
        IDAO dao;
        public LlistaPeliculesUC(IDAO dao)
        {
            InitializeComponent();
            this.dao = dao;
        }

        private void BtnIndexLlargada_Click(object sender, RoutedEventArgs e)
        {
            int index, largada;

            if (!int.TryParse(TxtIndex.Text, out index) || !int.TryParse(TxtLargada.Text, out largada) || index < 0 || largada < 0)
            {
                MessageBox.Show("L'índex i la llargada han de ser números vàlids.");
                TxtIndex.Clear();
                TxtLargada.Clear();
                DgPelicules.ItemsSource = null;
            }
            else
            {
                RawTitle[] pelicules = dao.ReadTitles(index, largada);
                if (pelicules.Length == 0)
                {
                    MessageBox.Show("No s'han trobat pel·lícules amb aquest índex i llargada.");
                    TxtIndex.Clear();
                    TxtLargada.Clear();
                    DgPelicules.ItemsSource = null;
                }
                else
                {
                    var rawTitlesAnonim = pelicules.Select(p => new
                    {
                        p.Index,
                        p.Id,
                        p.Title,
                        p.Type,
                        p.ReleaseYear,
                        p.AgeCertification,
                        p.Runtime,
                        Genres = p.LlistaToString(p.Genres),
                        ProductionCountries = p.LlistaToString(p.ProductionCountries),
                        p.Seasons,
                        p.ImdbId,
                        p.ImdbScore,
                        p.ImdbVotes
                    }).ToList();

                    DgPelicules.ItemsSource = rawTitlesAnonim;
                }
            }
        }
    }
}
