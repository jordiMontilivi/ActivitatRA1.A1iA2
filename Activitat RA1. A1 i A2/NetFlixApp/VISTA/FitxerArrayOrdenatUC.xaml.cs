using Microsoft.Win32;
using NetFlixApp.DADES;
using NetFlixApp.MODEL;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace NetFlixApp.VISTA
{
    /// <summary>
    /// Lógica de interacción para FitxerArrayOrdenatUC.xaml
    /// </summary>
    public partial class FitxerArrayOrdenatUC : UserControl
    {
        IDAO dao;
        public FitxerArrayOrdenatUC(IDAO dao)
        {
            InitializeComponent();
            this.dao = dao;
        }

        private void BtnGuardarFitxer_Click(object sender, RoutedEventArgs e)
        {
            int index, largada;
            string nomFitxer = TxtFitxerSortida.Text.Trim();

            if (!int.TryParse(TxtIndex.Text, out index) || !int.TryParse(TxtLargada.Text, out largada) || index < 0 || largada < 0 || string.IsNullOrWhiteSpace(nomFitxer))
            {
                MessageBox.Show("L'índex i la llargada han de ser números vàlids i has de especificar un nom de fitxer.",
                "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                TxtIndex.Clear();
                TxtLargada.Clear();
                TxtFitxerSortida.Clear();
            }
            else
            {
                RawTitle[] pelicules = dao.ReadTitles(index, largada);
                if (pelicules.Length == 0)
                {
                    MessageBox.Show("No s'han trobat pel·lícules amb aquest índex i llargada.",
                        "Error",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                    TxtIndex.Clear();
                    TxtLargada.Clear();
                }
                else
                {
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
                        FileName = $"{nomFitxer}.csv"
                    };

                    if (dialog.ShowDialog() == true)
                    {
                        string outputFile = dialog.FileName;

                        dao.PreMerge(pelicules, outputFile);

                        if (RbEditor.IsChecked == true)
                        {
                            // Obrim un fitxer csv amb el etitor de text per defecte
                            Process.Start(new ProcessStartInfo
                            {
                                FileName = outputFile,
                                UseShellExecute = true
                            });
                        }
                        else if (RbNotepad.IsChecked == true)
                        {                        
                            // Obrim el fitxer CSV amb Notepad
                            Process.Start(new ProcessStartInfo
                            {
                                FileName = "notepad.exe",
                                Arguments = $"\"{outputFile}\"",
                                UseShellExecute = true
                            });
                        }
                        else if (RbCsv.IsChecked == true)
                        {
                            // Obrim la finestra que mostra el CSV
                            CsvWindow finestra = new CsvWindow(outputFile);

                            finestra.Owner = Window.GetWindow(this);

                            finestra.Show();
                        }


                    }
                }

            }
        }
    }
}
