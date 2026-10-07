using Microsoft.Win32;
using NetFlixApp.DADES;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace NetFlixApp.VISTA
{
    /// <summary>
    /// Lógica de interacción para FitxerGenere.xaml
    /// </summary>
    public partial class FitxerGenere : UserControl
    {
        private readonly IDAO dao;
        public FitxerGenere(IDAO dao)
        {
            this.dao = dao;
            InitializeComponent();
        }

        private void BtnGenerarCsvGenere_Click(object sender, RoutedEventArgs e)
        {
            string genere = TxtGenere.Text.Trim();
            string nomFitxer = TxtFitxerSortida.Text.Trim();

            if (string.IsNullOrWhiteSpace(genere) || string.IsNullOrWhiteSpace(nomFitxer))
            {
                MessageBox.Show(
                    "Has d'introduir un gènere i un nom de fitxer.",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            // Carpeta on volem que s'obri inicialment el SaveFileDialog
            //string outputDirectory = System.IO.Path.Combine(
            //    AppDomain.CurrentDomain.BaseDirectory,
            //    "Fitxers");
            string outputDirectory = System.IO.Path.GetFullPath(
                System.IO.Path.Combine(
                    AppContext.BaseDirectory,
                    "..", "..", "..",
                    "DADES",
                    "DATA"
                )
            );


            // Si no existeix, la creem
            Directory.CreateDirectory(outputDirectory);

            SaveFileDialog dialog = new SaveFileDialog
            {
                Title = "Guardar fitxer CSV",
                Filter = "Fitxers CSV (*.csv)|*.csv",
                DefaultExt = ".csv",
                AddExtension = true,
                FileName = $"{nomFitxer}_{genere}.csv"
            };

            if (dialog.ShowDialog() != true)
                return;

            string outputFile = dialog.FileName;

            int numPelicules = dao.SelectByGenre(
            genere,
            outputFile);


            if (numPelicules == 0)
            {
                TxtResultat.Text =
                    $"No s'ha trobat cap pel·lícula del gènere '{genere}' i no s'ha generat cap fitxer.";
            }
            else
            {
                TxtResultat.Text =
                    $"S'han trobat {numPelicules} pel·lícules del gènere '{genere}' i al fitxer '{outputFile}'.";

                Process.Start(new ProcessStartInfo
                {
                    FileName = outputFile,
                    UseShellExecute = true
                });

                Process.Start(new ProcessStartInfo
                {
                    FileName = "notepad.exe",
                    Arguments = $"\"{outputFile}\"",
                    UseShellExecute = true
                });


                // Obrim la finestra que mostra el CSV
                CsvWindow finestra = new CsvWindow(outputFile);

                finestra.Owner = Window.GetWindow(this);

                finestra.Show();
            }

        }
    }
}
