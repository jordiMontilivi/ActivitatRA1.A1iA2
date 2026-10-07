using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace NetFlixApp.VISTA
{
    /// <summary>
    /// Lógica de interacción para CsvWindow.xaml
    /// </summary>
    public partial class CsvWindow : Window
    {
        public CsvWindow(string filePath)
        {
            InitializeComponent();

            TxtCsv.Text = File.ReadAllText(filePath);
        }
    }
}
