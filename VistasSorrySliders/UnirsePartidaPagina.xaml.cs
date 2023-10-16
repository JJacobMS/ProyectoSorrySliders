using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Lógica de interacción para UnirsePartidaPagina.xaml
    /// </summary>
    public partial class UnirsePartidaPagina : Page
    {
        private CuentaSet _cuentaActual;
        public UnirsePartidaPagina(CuentaSet cuentaActual)
        {
            InitializeComponent();
            _cuentaActual = cuentaActual;
        }

        private void ClickUnirsePartida(object sender, RoutedEventArgs e)
        {
            ValidarCodigo();
        }

        private void ValidarCodigo()
        {
            ReiniciarEstilosPantalla();
            bool esValido = true;
            string codigoPartida = txtBoxCodigo.Text;
            if (string.IsNullOrWhiteSpace(codigoPartida))
            {
                txtBoxCodigo.Style = (Style)FindResource("estiloTxtBoxDatosRojo");
                esValido = false;
            }

            if(esValido && !EsUniqueIdentifierValido(codigoPartida))
            {
                txtBoxCodigo.Style = (Style)FindResource("estiloTxtBoxDatosRojo");
                lblCodigoNoValido.Visibility = Visibility.Visible;
                esValido = false;
            }

            if (esValido)
            {
                UnirsePartida(codigoPartida);
            }
        }

        private void UnirsePartida(string codigo)
        {
            Constantes resultado;
            int numeroMaximoJugadores = 0;
            try
            {
                UnirsePartidaClient proxyUnirsePartida = new UnirsePartidaClient();
                (resultado, numeroMaximoJugadores) = proxyUnirsePartida.UnirseAlLobby(codigo, _cuentaActual.CorreoElectronico);
            }
            catch (CommunicationException excepcion)
            {
                resultado = Constantes.ERROR_CONEXION_SERVIDOR;
                Console.WriteLine(excepcion);
            }

            switch (resultado)
            {
                case Constantes.OPERACION_EXITOSA:
                    EntrarLobby(numeroMaximoJugadores, codigo);
                    break;
                case Constantes.OPERACION_EXITOSA_VACIA:
                    MostrarErrorJugadores(numeroMaximoJugadores);
                    break;
                case Constantes.ERROR_CONEXION_BD:
                case Constantes.ERROR_CONSULTA:
                    MessageBox.Show(Properties.Resources.msgErrorBaseDatos);
                    break;
                case Constantes.ERROR_CONEXION_SERVIDOR:
                    MessageBox.Show(Properties.Resources.msgErrorConexion);
                    break;
            }
        }

        private void EntrarLobby(int numeroMaximoJugadores, string codigo)
        {
            NavigationService navigationService = NavigationService.GetNavigationService(this);
            LobbyPagina lobby = new LobbyPagina();
            lobby.RecuperarDatosPartida(codigo);
            JuegoYLobbyVentana lobbyUnirse = new JuegoYLobbyVentana(lobby);
            lobbyUnirse.Show();
        }

        private void MostrarErrorJugadores(int numeroMaximoJugadores)
        {
            switch (numeroMaximoJugadores)
            {
                case -1:
                    lblCuentaYaEnJuego.Visibility = Visibility.Visible; 
                    break;
                case 0:
                    lblCodigoNoValido.Visibility = Visibility.Visible;
                    break;
            }

            if (numeroMaximoJugadores > 0)
            {
                lblMaximoJugadores.Visibility = Visibility.Visible;
            }
        }

        private void ReiniciarEstilosPantalla()
        {
            lblCodigoNoValido.Visibility = Visibility.Hidden;
            lblMaximoJugadores.Visibility = Visibility.Hidden;
            txtBoxCodigo.Style = (Style)FindResource("estiloTxtBoxDatosAzul");
        }

        private bool EsUniqueIdentifierValido(string uid)
        {
            string pattern = @"^[0-9A-Fa-f]{8}-[0-9A-Fa-f]{4}-[0-9A-Fa-f]{4}-[0-9A-Fa-f]{4}-[0-9A-Fa-f]{12}$";
            return Regex.IsMatch(uid, pattern);
        }

        private void ClickSalirMenuPrincipal(object sender, RoutedEventArgs e)
        {
            MenuPrincipalPagina menu = new MenuPrincipalPagina(_cuentaActual.CorreoElectronico);
            this.NavigationService.Navigate(menu);
        }
    }
}
