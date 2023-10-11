using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace VistasSorrySliders
{
    /// <summary>
    /// Lógica de interacción para MenuPrincipalPagina.xaml
    /// </summary>
    public partial class MenuPrincipalPagina : Page
    {
        public MenuPrincipalPagina(string correoUsuario)
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ClicMostrarAjustes(object sender, RoutedEventArgs e)
        {
            AjustesVentana ajustes = new AjustesVentana();
            ajustes.Show();
        }

        private void ClicSalirMenuPrincipal(object sender, RoutedEventArgs e)
        {
            this.NavigationService.GoBack();
        }
    }
}
