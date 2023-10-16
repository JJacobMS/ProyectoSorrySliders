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

        public ConfiguracionLobby(CuentaSet cuentaUsuario)
        {
            InitializeComponent();
            _cuentaUsuario = new CuentaSet();
            _cuentaUsuario.CorreoElectronico = cuentaUsuario.CorreoElectronico;
            _cuentaUsuario.Nickname = cuentaUsuario.Nickname;
            _cuentaUsuario.Avatar = cuentaUsuario.Avatar;
            txtBlockNickname.Text = cuentaUsuario.Nickname;
            Utilidades.IngresarImagen(_cuentaUsuario.Avatar, this.mgBrushAvatar);
        }

        private Border selectedBorder;

        private void MouseLeftButtonDownSeleccionarTablero(object sender, MouseButtonEventArgs e)
        {
            Border clickedBorder = sender as Border;

            if (selectedBorder != null)
            {
                selectedBorder.BorderBrush = Brushes.Black;
                selectedBorder.BorderThickness = new Thickness(0);
            }
            clickedBorder.BorderThickness = new Thickness(4);
            clickedBorder.BorderBrush = Brushes.Red; 
            selectedBorder = clickedBorder;

            
            btnCrearLobby.IsEnabled = (selectedBorder != null);
            e.Handled = false;
        }

        private void ClickCrearLobby(object sender, RoutedEventArgs e)
        {
            if (selectedBorder != null)
            {
                int numeroJugadoresInt = 0;
                string numeroJugadoresString = "";
                if (selectedBorder.Name == "brdTablero4Jugadores")
                {
                    numeroJugadoresInt = 4;
                    numeroJugadoresString = "4";
                }
                else if (selectedBorder.Name == "brdTablero3Jugadores")
                {
                    numeroJugadoresInt = 3;
                    numeroJugadoresString = "3";

                }
                else if (selectedBorder.Name == "brdTablero2Jugadores")
                {
                    numeroJugadoresInt = 2;
                    numeroJugadoresString = "2";
                }
                Constantes respuesta = Constantes.OPERACION_EXITOSA_VACIA;
                string codigoPartida = "";
                try
                {
                    CrearLobbyClient proxyCrearLobby = new CrearLobbyClient();
                    (respuesta, codigoPartida) = proxyCrearLobby.CrearPartida(_cuentaUsuario.CorreoElectronico, numeroJugadoresInt);
                }
                catch (CommunicationException excepcion)
                {
                    respuesta = Constantes.ERROR_CONEXION_SERVIDOR;
                    System.Windows.Forms.MessageBox.Show(Properties.Resources.msgErrorConexion);
                }
                catch (TimeoutException excepcion)
                {
                    respuesta = Constantes.ERROR_CONEXION_SERVIDOR;
                    System.Windows.Forms.MessageBox.Show(Properties.Resources.msgErrorConexion);

                }
                switch (respuesta)
                    {
                        case Constantes.ERROR_CONEXION_BD:
                            MessageBox.Show(Properties.Resources.msgErrorBaseDatos);
                            break;
                        case Constantes.ERROR_CONSULTA:
                            MessageBox.Show(Properties.Resources.msgErrorBaseDatos);
                            break;
                        case Constantes.ERROR_CONEXION_SERVIDOR:
                            MessageBox.Show(Properties.Resources.msgErrorConexion);
                            break;
                        case Constantes.OPERACION_EXITOSA:
                            CrearVentanaLobby(numeroJugadoresString, _cuentaUsuario, codigoPartida);
                            Window.GetWindow(this).Close();
                        break;
                        case Constantes.OPERACION_EXITOSA_VACIA:
                            break;
                        default:
                            break;
                    }
                    
                
            }
        }

        private void CrearVentanaLobby(string numeroJugadoresString, CuentaSet _cuentaUsuario, string codigoPartida) 
        {
            NavigationService navigationService = NavigationService.GetNavigationService(this);
            LobbyPagina lobby = new LobbyPagina();
            lobby.RecuperarDatosPartida(codigoPartida);
            JuegoYLobbyVentana lobbyUnirse = new JuegoYLobbyVentana(lobby);
            lobbyUnirse.Show();

        }

        private void ClickSalirConfigurarLobby(object sender, RoutedEventArgs e)
        {
            MenuPrincipalPagina menu = new MenuPrincipalPagina(_cuentaUsuario.CorreoElectronico);
            this.NavigationService.Navigate(menu);
        }
    }
}
