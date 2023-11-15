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
        private JuegoYLobbyVentana _juegoYLobbyVentana;
        private InstanceContext _contextoCallback;

        public LobbyPagina(CuentaSet cuentaUsuario, string codigoPartida, bool esInvitado, JuegoYLobbyVentana ventana)
        {
            InitializeComponent();
            _juegoYLobbyVentana = ventana;
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
            Logger log = new Logger(this.GetType());
            try
            {
                UnirsePartidaClient proxyRecuperarJugadores = new UnirsePartidaClient();
                (respuesta, _cuentas) = proxyRecuperarJugadores.RecuperarJugadoresLobby(codigoPartida);
                if (_cuentas.Count() == _partidaActual.CantidadJugadores && _cuentaUsuario.CorreoElectronico == _cuentas[0].CorreoElectronico)
                {
                    btnIniciarPartida.IsEnabled = true;
                }
                else
                {
                    btnIniciarPartida.IsEnabled = true;
                }
            }
            catch (CommunicationException ex)
            {
                respuesta = Constantes.ERROR_CONEXION_SERVIDOR;
                Console.WriteLine(ex);
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
                    IrMenuUsuario();
                    break;
                case Constantes.ERROR_CONSULTA:
                    MessageBox.Show(Properties.Resources.msgErrorBaseDatos);
                    IrMenuUsuario();
                    break;
                case Constantes.ERROR_CONEXION_SERVIDOR:
                    MessageBox.Show(Properties.Resources.msgErrorConexion);
                    IrMenuUsuario();
                    break;
                case Constantes.OPERACION_EXITOSA:
                    grdJugadores.Children.Clear();
                    CrearBorders(_cuentas);
                    CrearEllipses(_cuentas);
                    CrearLabels(_cuentas);
                    txtBoxHost.Text = _cuentas[0].Nickname;
                    break;
                case Constantes.OPERACION_EXITOSA_VACIA:
                    MessageBox.Show(Properties.Resources.msgErrorConexion);
                    IrMenuUsuario();
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
                Rectangle rctNuevo = XamlReader.Parse(XamlWriter.Save(rctJugador)) as Rectangle;
                rctNuevo.Name = "rctJugador" + (contador + 1);
                Grid.SetRow(rctNuevo, contador);
                grdJugadores.Children.Add(rctNuevo);
                contador++;
            }
        }
        private void CrearEllipses(CuentaSet[] cuentas)
        {
            int contador = 0;
            foreach (var cuenta in cuentas)
            {
                Ellipse llpNuevaAvatar = XamlReader.Parse(XamlWriter.Save(llpAvatar)) as Ellipse;
                llpNuevaAvatar.Name = "llpAvatarJugador" + (contador + 1);
                Grid.SetRow(llpNuevaAvatar, contador);
                grdJugadores.Children.Add(llpNuevaAvatar);
                

                Ellipse llpNuevaFondo = XamlReader.Parse(XamlWriter.Save(llpFondo)) as Ellipse;
                llpNuevaFondo.Name = "llpFondoJugador" + (contador + 1);
                llpNuevaFondo.Fill = Utilidades.ConvertirBytesAImageBrush(cuenta.Avatar);
                Grid.SetRow(llpNuevaFondo, contador);
                grdJugadores.Children.Add(llpNuevaFondo);

                Ellipse llpNuevaDecoracion = XamlReader.Parse(XamlWriter.Save(llpDecoracion)) as Ellipse;
                llpNuevaDecoracion.Name = "llpDecoracion" + (contador + 1);
                Grid.SetRow(llpNuevaDecoracion, contador);
                grdJugadores.Children.Add(llpNuevaDecoracion);

                contador++;
            }
        }
        private void CrearLabels(CuentaSet[] cuentas) 
        {
            int contador = 0;
            foreach (var cuenta in cuentas)
            {
                Label lblNuevo = XamlReader.Parse(XamlWriter.Save(lblJugador)) as Label;
                lblNuevo.Name = "lblJugador" + (contador + 1);
                lblNuevo.Content = cuenta.Nickname;
                Grid.SetRow(lblNuevo, contador);
                grdJugadores.Children.Add(lblNuevo);
                contador++;
            }
        }

        private void InicializarDatosPartida(string codigoPartida) 
        {
            Constantes respuesta;
            Logger log = new Logger(this.GetType());
            try
            {
                UnirsePartidaClient proxyUnirsePartida = new UnirsePartidaClient();
                (respuesta, _partidaActual) = proxyUnirsePartida.RecuperarPartida(codigoPartida);
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
                    IrMenuUsuario();
                    break;
                case Constantes.ERROR_CONSULTA:
                    MessageBox.Show(Properties.Resources.msgErrorBaseDatos);
                    IrMenuUsuario();
                    break;
                case Constantes.ERROR_CONEXION_SERVIDOR:
                    MessageBox.Show(Properties.Resources.msgErrorConexion);
                    IrMenuUsuario();
                    break;
                case Constantes.OPERACION_EXITOSA:
                    txtBoxCodigoPartida.Text = _partidaActual.CodigoPartida.ToString();
                    txtBoxJugadores.Text = ""+ _partidaActual.CantidadJugadores;
                    switch (_partidaActual.CantidadJugadores)
                    {
                        case 2:
                            lblCantidadJugadoresPartida.Content = Properties.Resources.lblDosJugadores;
                            Uri urlRelativa1 = new Uri("/Recursos/TableroDosConFondo.png", UriKind.Relative);
                            mgTablero.Source = new BitmapImage(urlRelativa1);
                            break;
                        case 3:
                            lblCantidadJugadoresPartida.Content = Properties.Resources.lblTresJugadores;
                            Uri urlRelativa2 = new Uri("/Recursos/TableroTresConFondo.png", UriKind.Relative);
                            mgTablero.Source = new BitmapImage(urlRelativa2);
                            break;
                        case 4:
                            lblCantidadJugadoresPartida.Content = Properties.Resources.lblCuatroJugadores;
                            Uri urlRelativa3 = new Uri("/Recursos/TableroCuatroConFondo.png", UriKind.Relative);
                            mgTablero.Source = new BitmapImage(urlRelativa3);
                            break;
                        default:
                            break;
                    }
                    
                    break;
                case Constantes.OPERACION_EXITOSA_VACIA:
                    MessageBox.Show(Properties.Resources.msgErrorBaseDatos);
                    IrMenuUsuario();
                    break;
                default:
                    break;
            }
            
        }

        private void ClickSalirLobbyJugadores(object sender, RoutedEventArgs e)
        {
            SalirPartida();
            IrMenuUsuario();
        }
        public void IrMenuUsuario() 
        {
            var ventanaPrincipal = new MainWindow();

            if (_esInvitado)
            {
                EliminarCuentaProvisionalInvitado(_cuentaUsuario.CorreoElectronico);
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
            Logger log = new Logger(this.GetType());
            try
            {
                InstanceContext contextoCallback = new InstanceContext(this);
                _contextoCallback = contextoCallback;
                _proxyLobby = new LobbyClient(contextoCallback);
                _proxyLobby.EntrarPartida(_codigoPartida, _cuentaUsuario.CorreoElectronico);
            }
            catch (CommunicationException ex)
            {
                Console.WriteLine(ex);
                log.LogError("Error de Comunicación con el Servidor", ex);
            }
            catch (TimeoutException ex)
            {
                Console.WriteLine(ex);
                log.LogWarn("Se agoto el tiempo de espera del servidor", ex);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                log.LogFatal("Ha ocurrido un error inesperado", ex);
            }
        }


        public void SalirPartida() 
        {
            Logger log = new Logger(this.GetType());
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
                log.LogError("Error de Comunicación con el Servidor", ex);
            }
            catch (TimeoutException ex)
            {
                Console.WriteLine(ex);
                System.Windows.Forms.MessageBox.Show(Properties.Resources.msgErrorConexion);
                log.LogWarn("Se agoto el tiempo de espera del servidor", ex);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                System.Windows.Forms.MessageBox.Show(Properties.Resources.msgErrorConexion);
                log.LogFatal("Ha ocurrido un error inesperado", ex);
            }
        }

        public void JugadorSalioPartida()
        {
            RecuperarDatosPartida(_codigoPartida);
        }

        private void ClickIniciarPartida(object sender, RoutedEventArgs e)
        {
            if (_cuentas.Count() == _partidaActual.CantidadJugadores && txtBoxHost.Text == _cuentaUsuario.Nickname)
            {
                CambiarPaginas();
            }
        }

        private void CambiarPaginas() 
        {
            Logger log = new Logger(this.GetType());
            try
            {
                _proxyLobby.IniciarPartida(_codigoPartida);
            }
            catch (CommunicationException ex)
            {
                Console.WriteLine(ex);
                System.Windows.Forms.MessageBox.Show(Properties.Resources.msgErrorConexion);
                log.LogError("Error de Comunicación con el Servidor", ex);
            }
            catch (TimeoutException ex)
            {
                Console.WriteLine(ex);
                System.Windows.Forms.MessageBox.Show(Properties.Resources.msgErrorConexion);
                log.LogWarn("Se agoto el tiempo de espera del servidor", ex);
            }
        }

        private void EliminarCuentaProvisionalInvitado(string correoProvisional)
        {
            try
            {
                UnirsePartidaClient proxyRecuperarJugadores = new UnirsePartidaClient();
                proxyRecuperarJugadores.EliminarCuentaProvisional(correoProvisional);
            }
            catch (CommunicationException ex)
            {
                Console.WriteLine(ex);
            }
            catch (TimeoutException ex)
            {
                Console.WriteLine(ex);
            }
        }

        public void HostInicioPartida()
        {
            List<CuentaSet> cuentasJugadores = _cuentas.ToList();
            Page paginaJuego = new JuegoLanzamientoPagina(cuentasJugadores, _partidaActual.CantidadJugadores, _proxyLobby);
            Page paginaChat = new JugadoresChatPagina(_cuentas, _cuentaUsuario, _partidaActual);
            _juegoYLobbyVentana.CambiarFrameLobby(paginaJuego);
            _juegoYLobbyVentana.CambiarFrameListaAmigos(paginaChat);
        }
    }
}
