using NetFlixApp.DADES;
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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly IDAO dao;

        public MainWindow()
        {
            InitializeComponent();
            dao = DAOFactory.CreateDAO();
        }

        private void BtnGenerarFitxerGenere_Click(object sender, RoutedEventArgs e)
        {
            RegioPrincipal.Content = new FitxerGenere(dao);
        }
    }
}