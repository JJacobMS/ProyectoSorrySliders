using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.ServiceModel;
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
        private CuentaSet _cuentaUsuario;
        private Border bordeAnterior;

        public ConfiguracionLobby(CuentaSet cuentaUsuario)
        {
            InitializeComponent();
            _cuentaUsuario = new CuentaSet();
            _cuentaUsuario = cuentaUsuario;
            txtBlockNickname.Text = _cuentaUsuario.Nickname;
            Utilidades.IngresarImagen(_cuentaUsuario.Avatar, this.mgBrushAvatar);
        }

        private void MouseLeftButtonDownSeleccionarTablero(object sender, MouseButtonEventArgs e)
        {
            Border bordeSeleccionado = sender as Border;

            if (bordeAnterior != null)
            {
                bordeAnterior.BorderBrush = Brushes.Black;
                bordeAnterior.BorderThickness = new Thickness(0);
            }
            bordeSeleccionado.BorderThickness = new Thickness(4);
            bordeSeleccionado.BorderBrush = Brushes.Red;
            bordeAnterior = bordeSeleccionado;

            btnCrearLobby.IsEnabled = (bordeAnterior != null);
        }

        private void ClickCrearLobby(object sender, RoutedEventArgs e)
        {
            if (bordeAnterior != null)
            {
                int numeroJugadoresInt = 0;
                if (bordeAnterior.Name == "brdTablero4Jugadores")
                {
                    numeroJugadoresInt = 4;
                }
                else if (bordeAnterior.Name == "brdTablero3Jugadores")
                {
                    numeroJugadoresInt = 3;
                }
                else if (bordeAnterior.Name == "brdTablero2Jugadores")
                {
                    numeroJugadoresInt = 2;
                }

                if (numeroJugadoresInt >= 2)
                {
                    CrearLobby(numeroJugadoresInt);
                }
            }
        }

        private void CrearLobby(int numeroJugadoresInt)
        {
            Constantes respuesta;
            string codigoPartida = "";
            Logger log = new Logger(this.GetType());
            try
            {
                CrearLobbyClient proxyCrearLobby = new CrearLobbyClient();
                (respuesta, codigoPartida) = proxyCrearLobby.CrearPartida(_cuentaUsuario.CorreoElectronico, numeroJugadoresInt);
            }
            catch (CommunicationException ex)
            {
                respuesta = Constantes.ERROR_CONEXION_SERVIDOR;
                log.LogWarn("Error de Comunicación con el Servidor", ex);
            }
            catch (TimeoutException ex)
            {
                respuesta = Constantes.ERROR_TIEMPO_ESPERA_SERVIDOR;
                log.LogWarn("Se agoto el tiempo de espera del servidor", ex);
            }

            Utilidades.MostrarMensajesError(respuesta);
        }

        private void CrearVentanaLobby(CuentaSet _cuentaUsuario, string codigoPartida) 
        {
            JuegoYLobbyVentana lobbyUnirse = new JuegoYLobbyVentana(_cuentaUsuario, codigoPartida, false);
            Constantes respuesta = lobbyUnirse.InicializarPaginas();
            switch (respuesta)
            {
                case Constantes.OPERACION_EXITOSA:
                    MostrarVentanaLobby(lobbyUnirse);
                    break;
                case Constantes.ERROR_CONEXION_SERVIDOR:
                    Utilidades.SalirInicioSesionDesdeVentanaPrincipal(this);
                    break;
            }
        }

        private void MostrarVentanaLobby(JuegoYLobbyVentana lobbyUnirse)
        {
            Window.GetWindow(this).Close();
            lobbyUnirse.Show();
        }

        private void ClickSalirConfigurarLobby(object sender, RoutedEventArgs e)
        {
            MenuPrincipalPagina menu = new MenuPrincipalPagina(_cuentaUsuario);
            this.NavigationService.Navigate(menu);
        }
    }
}
