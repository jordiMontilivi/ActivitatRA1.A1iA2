using Microsoft.Win32;
using NetFlixApp.DADES;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace NetFlixApp.VISTA
{
    /// <summary>
    /// Lógica de interacción para MergeFilesUC.xaml
    /// </summary>
    public partial class MergeFilesUC : UserControl
    {
        IDAO dao;
        public MergeFilesUC(IDAO dao)
        {
            InitializeComponent();
            this.dao = dao;
        }

        private void BtnFitxer1_Click(object sender, RoutedEventArgs e)
        {

            OpenFileDialog dialog = new OpenFileDialog();

            dialog.Filter = "Fitxers CSV (*.csv)|*.csv|Tots els fitxers (*.*)|*.*";

            if (dialog.ShowDialog() == true)
            {
                TxtFitxer1.Text = dialog.FileName;
            }
        }

        private void BtnFitxer2_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();

            dialog.Filter = "Fitxers CSV (*.csv)|*.csv|Tots els fitxers (*.*)|*.*";

            if (dialog.ShowDialog() == true)
            {
                TxtFitxer2.Text = dialog.FileName;
            }

        }

        private void BtnFitxerSortida_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();

            dialog.Filter = "Fitxers CSV (*.csv)|*.csv|Tots els fitxers (*.*)|*.*";
            dialog.DefaultExt = ".csv";

            if (dialog.ShowDialog() == true)
            {
                TxtFitxerSortida.Text = dialog.FileName;
            }
        }

        private void BtnFusionar_Click(object sender, RoutedEventArgs e)
        {
            // Comprovem que s'han seleccionat els fitxers

            if (string.IsNullOrWhiteSpace(TxtFitxer1.Text))
            {
                MessageBox.Show("Selecciona el primer fitxer.",
                                "Avís",
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning);
            }

            else if (string.IsNullOrWhiteSpace(TxtFitxer2.Text))
            {
                MessageBox.Show("Selecciona el segon fitxer.",
                                "Avís",
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning);
            }

            else if (string.IsNullOrWhiteSpace(TxtFitxerSortida.Text))
            {
                MessageBox.Show("Selecciona el fitxer de sortida.",
                                "Avís",
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning);
            }

            else
            {
                // Cridem el mètode del DAO

                int registres = dao.Merge(
                    TxtFitxer1.Text,
                    TxtFitxer2.Text,
                    TxtFitxerSortida.Text
                );



                if (RbEditor.IsChecked == true)
                {
                    // Obrim un fitxer csv amb el etitor de text per defecte
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = TxtFitxerSortida.Text,
                        UseShellExecute = true
                    });
                }
                else if (RbNotepad.IsChecked == true)
                {
                    // Obrim el fitxer CSV amb Notepad
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = "notepad.exe",
                        Arguments = $"\"{TxtFitxerSortida.Text}\"",
                        UseShellExecute = true
                    });
                }
                else if (RbCsv.IsChecked == true)
                {
                    // Obrim la finestra que mostra el CSV
                    CsvWindow finestra = new CsvWindow(TxtFitxerSortida.Text);

                    finestra.Owner = Window.GetWindow(this);

                    finestra.Show();
                }


                // Mostrem el resultat

                TxtResultat.Text =
                    $"S'han fusionat correctament {registres} registres.";
            }
        }

    }
}
   
