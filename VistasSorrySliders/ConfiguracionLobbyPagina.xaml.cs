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
using VistasSorrySliders.ServicioSorrySliders;

namespace VistasSorrySliders
{
    /// <summary>
    /// Lógica de interacción para ConfiguracionLobby.xaml
    /// </summary>
    public partial class ConfiguracionLobby : Page
    {
        public ConfiguracionLobby(CuentaSet cuentaUsuario)
        {
            InitializeComponent();
        }

        private void ClickCrearLobby(object sender, RoutedEventArgs e)
        {

        }

        private void ClickSalirConfigurarLobby(object sender, RoutedEventArgs e)
        {
            MenuPrincipalPagina menu = new MenuPrincipalPagina("");
            this.NavigationService.Navigate(menu);
        }
    }
}
