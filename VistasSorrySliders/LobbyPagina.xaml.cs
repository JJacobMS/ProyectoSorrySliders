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
    /// Lógica de interacción para LobbyPagina.xaml
    /// </summary>
    public partial class LobbyPagina : Page
    {
        private CuentaSet _cuentaUsuario;

        public LobbyPagina()
        {
            InitializeComponent();
        }

        public void InicializarDatos(string numeroJugadores, CuentaSet cuentaHost, string codigoPartida) 
        {
            txtBoxCodigoPartida.Text = codigoPartida;
            txtBoxHost.Text = cuentaHost.Nickname;
            txtBoxJugadores.Text = numeroJugadores;
            _cuentaUsuario = new CuentaSet(); 
            _cuentaUsuario = cuentaHost;

        }

        private void ClickSalirLobbyJugadores(object sender, RoutedEventArgs e)
        {
            var ventanaPrincipal = new Window();
            MenuPrincipalPagina menu = new MenuPrincipalPagina(_cuentaUsuario.CorreoElectronico);
            ventanaPrincipal.Content = menu;
            ventanaPrincipal.Show();
            Window.GetWindow(this).Close();
        }
    }
}
