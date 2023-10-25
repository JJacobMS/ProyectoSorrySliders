using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
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
    public partial class LobbyPagina : Page, ILobbyCallback
    {
        private CuentaSet _cuentaUsuario;
        private string _codigoPartida;
        private LobbyClient _proxyLobby;
        private CuentaSet[] _cuentas;
        private PartidaSet _partidaActual;
        private bool _esInvitado;

        public LobbyPagina(CuentaSet cuentaUsuario, string codigoPartida, bool esInvitado)
        {
            InitializeComponent();
            _esInvitado = esInvitado;
            _cuentaUsuario = cuentaUsuario;
            _codigoPartida = codigoPartida;
            EntrarPartida();
            InicializarDatosPartida(codigoPartida);
            RecuperarDatosPartida(codigoPartida);
        }

        public void RecuperarDatosPartida(string codigoPartida) 
        {
            Constantes respuesta;
            try
            {
                UnirsePartidaClient proxyRecuperarJugadores = new UnirsePartidaClient();
                (respuesta, _cuentas) = proxyRecuperarJugadores.RecuperarJugadoresLobby(codigoPartida);
                if (_cuentas.Count() == _partidaActual.CantidadJugadores)
                {
                    btnIniciarPartida.IsEnabled = true;
                }
                else
                {
                    btnIniciarPartida.IsEnabled = false;
                }
            }
            catch (CommunicationException ex)
            {
                respuesta = Constantes.ERROR_CONEXION_SERVIDOR;
                Console.WriteLine(ex);
                System.Windows.Forms.MessageBox.Show(Properties.Resources.msgErrorConexion);
            }
            catch (TimeoutException ex)
            {
                Console.WriteLine(ex);
                respuesta = Constantes.ERROR_CONEXION_SERVIDOR;
                System.Windows.Forms.MessageBox.Show(Properties.Resources.msgErrorConexion);
            }
            switch (respuesta)
            {
                case Constantes.ERROR_CONEXION_BD:
                    Console.WriteLine("ERROR_CONEXION_BD");
                    break;
                case Constantes.ERROR_CONSULTA:
                    Console.WriteLine("ERROR_CONSULTA");
                    break;
                case Constantes.ERROR_CONEXION_SERVIDOR:
                    Console.WriteLine("ERROR_CONEXION_SERVIDOR1");
                    break;
                case Constantes.OPERACION_EXITOSA:
                    grdJugadores.Children.Clear();
                    CrearBorders(_cuentas);
                    CrearEllipses(_cuentas);
                    CrearLabels(_cuentas);
                    txtBoxHost.Text = _cuentas[0].CorreoElectronico;
                    break;
                case Constantes.OPERACION_EXITOSA_VACIA:
                    Console.WriteLine("OPERACION_EXITOSA_VACIA1");
                    break;
                default:
                    break;
            }
        }

        private void CrearBorders(CuentaSet[] cuentas) 
        {
            int contador = 0;
            foreach(var cuenta in cuentas) 
            {
                Rectangle rectanguloNuevo = XamlReader.Parse(XamlWriter.Save(rctJugador)) as Rectangle;
                rectanguloNuevo.Name = "rctJugador" + (contador + 1);
                Grid.SetRow(rectanguloNuevo, contador);
                grdJugadores.Children.Add(rectanguloNuevo);
                contador++;
            }
        }
        private void CrearEllipses(CuentaSet[] cuentas)
        {
            int contador = 0;
            foreach (var cuenta in cuentas)
            {
                Ellipse nuevaEllipseAvatar = XamlReader.Parse(XamlWriter.Save(llpAvatar)) as Ellipse;
                nuevaEllipseAvatar.Name = "llpAvatarJugador" + (contador + 1);
                Grid.SetRow(nuevaEllipseAvatar, contador);
                grdJugadores.Children.Add(nuevaEllipseAvatar);
                

                Ellipse nuevaEllipseFondo = XamlReader.Parse(XamlWriter.Save(llpFondo)) as Ellipse;
                nuevaEllipseFondo.Name = "llpFondoJugador" + (contador + 1);
                nuevaEllipseFondo.Fill = Utilidades.ConvertirBytesAImageBrush(cuenta.Avatar);
                Grid.SetRow(nuevaEllipseFondo, contador);
                grdJugadores.Children.Add(nuevaEllipseFondo);

                Ellipse nuevaEllipseDecoracion = XamlReader.Parse(XamlWriter.Save(llpDecoracion)) as Ellipse;
                nuevaEllipseDecoracion.Name = "llpDecoracion" + (contador + 1);
                Grid.SetRow(nuevaEllipseDecoracion, contador);
                grdJugadores.Children.Add(nuevaEllipseDecoracion);

                contador++;
            }
        }
        private void CrearLabels(CuentaSet[] cuentas) 
        {
            int contador = 0;
            foreach (var cuenta in cuentas)
            {
                Label nuevoLabel = XamlReader.Parse(XamlWriter.Save(lblJugador)) as Label;
                nuevoLabel.Name = "lblJugador" + (contador + 1);
                nuevoLabel.Content = cuenta.Nickname;
                Grid.SetRow(nuevoLabel, contador);
                grdJugadores.Children.Add(nuevoLabel);
                contador++;
            }
        }

        private void InicializarDatosPartida(string codigoPartida) 
        {
            Constantes respuesta;
            try
            {
                UnirsePartidaClient proxyUnirsePartida = new UnirsePartidaClient();
                (respuesta, _partidaActual) = proxyUnirsePartida.RecuperarPartida(codigoPartida);
                
            }
            catch (CommunicationException ex)
            {
                Console.WriteLine(ex);
                respuesta = Constantes.ERROR_CONEXION_SERVIDOR;
                System.Windows.Forms.MessageBox.Show(Properties.Resources.msgErrorConexion);
            }
            catch (TimeoutException ex)
            {
                Console.WriteLine(ex);
                respuesta = Constantes.ERROR_CONEXION_SERVIDOR;
                System.Windows.Forms.MessageBox.Show(Properties.Resources.msgErrorConexion);
            }
            switch (respuesta)
            {
                case Constantes.ERROR_CONEXION_BD:
                    Console.WriteLine("ERROR_CONEXION_BD");
                    break;
                case Constantes.ERROR_CONSULTA:
                    Console.WriteLine("ERROR_CONSULTA");

                    break;
                case Constantes.ERROR_CONEXION_SERVIDOR:
                    Console.WriteLine("ERROR_CONEXION_SERVIDOR2");

                    break;
                case Constantes.OPERACION_EXITOSA:
                    txtBoxCodigoPartida.Text = _partidaActual.CodigoPartida.ToString();
                    txtBoxHost.Text = _partidaActual.CorreoElectronico;
                    txtBoxJugadores.Text = ""+ _partidaActual.CantidadJugadores;
                    switch (_partidaActual.CantidadJugadores)
                    {
                        case 2:
                            lblCantidadJugadoresPartida.Content = Properties.Resources.lblDosJugadores;
                            //mgTablero.Source = "/Recursos/TableroCuatroConFondo.png";
                            break;
                        case 3:
                            lblCantidadJugadoresPartida.Content = Properties.Resources.lblTresJugadores;
                            //
                            break;
                        case 4:
                            lblCantidadJugadoresPartida.Content = Properties.Resources.lblCuatroJugadores;
                            //
                            break;
                        default:
                            break;
                    }
                    
                    break;
                case Constantes.OPERACION_EXITOSA_VACIA:
                    Console.WriteLine("OPERACION_EXITOSA_VACIA2");
                    break;
                default:
                    break;
            }
            
        }

        private void ClickSalirLobbyJugadores(object sender, RoutedEventArgs e)
        {
            SalirPartida();
            var ventanaPrincipal = new MainWindow();

            if (_esInvitado)
            {
                InicioSesionPagina inicio = new InicioSesionPagina();
                ventanaPrincipal.Content = inicio;
            }
            else
            {
                MenuPrincipalPagina menu = new MenuPrincipalPagina(_cuentaUsuario);
                ventanaPrincipal.Content = menu;
            }
            ventanaPrincipal.Show();
            Window.GetWindow(this).Close();
        }

        public void JugadorEntroPartida()
        {
            RecuperarDatosPartida(_codigoPartida);
        }

        public void EntrarPartida()
        {
            try
            {
                InstanceContext contextoCallback = new InstanceContext(this);
                _proxyLobby = new LobbyClient(contextoCallback);
                _proxyLobby.EntrarPartida(_codigoPartida);
            }
            catch (CommunicationException ex)
            {
                MessageBox.Show("Error servidor :c");
                Console.WriteLine(ex);
            }
        }


        public void SalirPartida() 
        {
            try
            {
                UnirsePartidaClient proxyUnirse = new UnirsePartidaClient();
                proxyUnirse.SalirDelLobby(_cuentaUsuario.CorreoElectronico, _codigoPartida);
                _proxyLobby.SalirPartida(_codigoPartida);
                   
            }
            catch (CommunicationException ex)
            {
                Console.WriteLine(ex);
                System.Windows.Forms.MessageBox.Show(Properties.Resources.msgErrorConexion);
            }
            catch (TimeoutException ex)
            {
                Console.WriteLine(ex);
                System.Windows.Forms.MessageBox.Show(Properties.Resources.msgErrorConexion);
            }
        }
        public void JugadorSalioPartida()
        {
            RecuperarDatosPartida(_codigoPartida);
        }

        private void ClickIniciarPartida(object sender, RoutedEventArgs e)
        {
            if (_cuentas.Count() == _partidaActual.CantidadJugadores)
            {
                
            }
        }
    }
}
