﻿using System;
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
            _cuentaUsuario = cuentaUsuario;
            Utilidades.IngresarImagen(_cuentaUsuario.Avatar, this.mgBrushAvatar);
            txtBlockNickname.Text = _cuentaUsuario.Nickname; 
        }

        private Border bordeAnterior;

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
                    Console.WriteLine(ex);
                    respuesta = Constantes.ERROR_CONEXION_SERVIDOR;
                    log.LogError("Error de Comunicación con el Servidor", ex);
                }
                catch (TimeoutException ex)
                {
                    Console.WriteLine(ex);
                    respuesta = Constantes.ERROR_CONEXION_SERVIDOR;
                    log.LogWarn("Se agoto el tiempo de espera del servidor", ex);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    respuesta = Constantes.ERROR_CONEXION_SERVIDOR;
                    log.LogFatal("Ha ocurrido un error inesperado", ex);
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
                        CrearVentanaLobby(numeroJugadoresInt.ToString(), _cuentaUsuario, codigoPartida);
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
            JuegoYLobbyVentana lobbyUnirse = new JuegoYLobbyVentana(_cuentaUsuario, codigoPartida, false);
            lobbyUnirse.Show();

        }

        private void ClickSalirConfigurarLobby(object sender, RoutedEventArgs e)
        {
            MenuPrincipalPagina menu = new MenuPrincipalPagina(_cuentaUsuario);
            this.NavigationService.Navigate(menu);
        }
    }
}
