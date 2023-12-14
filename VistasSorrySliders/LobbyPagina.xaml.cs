using System;
using System.Collections.Generic;
using System.Data.Entity.Core;
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
        private readonly CuentaSet _cuentaUsuario;
        private readonly string _codigoPartida;
        private LobbyClient _proxyLobby;
        private CuentaSet[] _cuentas;
        private PartidaSet _partidaActual;
        private readonly JuegoYLobbyVentana _juegoYLobbyVentana;
        private int _juegoIniciado;

        public LobbyPagina(CuentaSet cuentaUsuario, string codigoPartida, JuegoYLobbyVentana ventana)
        {
            InitializeComponent();
            _juegoYLobbyVentana = ventana;
            _juegoYLobbyVentana.EliminarContexto += SalirLobbyServidor;

            _cuentaUsuario = cuentaUsuario;
            _codigoPartida = codigoPartida;
            _juegoIniciado = 0;
            
        }

        public Constantes InicializarPagina()
        {
            try
            {
                EntrarPartida();
                InicializarDatosPartida(_codigoPartida);
                _= RecuperarDatosPartidaAsync(_codigoPartida);
            }
            catch (CommunicationException ex)
            {
                Logger log = new Logger(this.GetType());
                log.LogWarn("Error de Comunicación con el Servidor", ex);
                return Constantes.ERROR_CONEXION_SERVIDOR;
            }
            catch (EntitySqlException ex)
            {
                Logger log = new Logger(this.GetType());
                log.LogError("Error Con Base de Datos", ex);
                return Constantes.ERROR_CONEXION_BD;
            }
            return Constantes.OPERACION_EXITOSA;
        }

        public async Task RecuperarDatosPartidaAsync(string codigoPartida) 
        {
            brdComenzarPartida.Visibility = Visibility.Hidden;
            brdActualizarJugadores.Visibility = Visibility.Visible;
            if (_partidaActual == null)
            {
                return;
            }
            Constantes respuesta;
            Logger log = new Logger(this.GetType());
            try
            {
                await Task.Delay(2000);
                UnirsePartidaClient proxyRecuperarJugadores = new UnirsePartidaClient();
                (respuesta, _cuentas) = proxyRecuperarJugadores.RecuperarJugadoresLobby(codigoPartida);
                if (_cuentas.Count() == _partidaActual.CantidadJugadores && _cuentaUsuario.CorreoElectronico == _cuentas[0].CorreoElectronico)
                {
                    btnIniciarPartida.IsEnabled = (_juegoIniciado == 0);                    
                }
                else
                {
                    btnIniciarPartida.IsEnabled = false;
                    _juegoIniciado = _juegoIniciado >= 1 ? 0 : _juegoIniciado;
                }
            }
            catch (CommunicationObjectFaultedException ex)
            {
                respuesta = Constantes.ERROR_CONEXION_DEFECTUOSA;
                log.LogError("Se ha perdido la conexión previa", ex);
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
            brdActualizarJugadores.Visibility = Visibility.Hidden;
            Utilidades.MostrarMensajesError(respuesta);
            switch (respuesta)
            {
                case Constantes.OPERACION_EXITOSA:
                    grdJugadores.Children.Clear();
                    CrearBorders(_cuentas);
                    CrearEllipses(_cuentas);
                    CrearLabels(_cuentas);
                    txtBoxHost.Text = _cuentas[0].Nickname;
                    return;
                case Constantes.OPERACION_EXITOSA_VACIA:
                    Utilidades.MostrarUnMensajeError(Properties.Resources.msgJugadoresLobbyRecuperar);
                    break;
                case Constantes.ERROR_TIEMPO_ESPERA_SERVIDOR:
                case Constantes.ERROR_CONEXION_SERVIDOR:
                case Constantes.ERROR_CONEXION_DEFECTUOSA:
                    throw new CommunicationException();
            }
            throw new EntitySqlException();
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
            catch (CommunicationObjectFaultedException ex)
            {
                respuesta = Constantes.ERROR_CONEXION_DEFECTUOSA;
                log.LogError("Se ha perdido la conexión previa", ex);
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
            switch (respuesta)
            {
                case Constantes.OPERACION_EXITOSA:
                    CargarDatosPartida();
                    return;
                case Constantes.OPERACION_EXITOSA_VACIA:
                    Utilidades.MostrarUnMensajeError(Properties.Resources.msgPartidaRecuperarVacia);
                    break;
                case Constantes.ERROR_TIEMPO_ESPERA_SERVIDOR:
                case Constantes.ERROR_CONEXION_SERVIDOR:
                case Constantes.ERROR_CONEXION_DEFECTUOSA:
                    throw new CommunicationException();
            }
            throw new EntitySqlException();
        }

        private void CargarDatosPartida()
        {
            txtBoxCodigoPartida.Text = _partidaActual.CodigoPartida.ToString();
            txtBoxJugadores.Text = "" + _partidaActual.CantidadJugadores;
            switch (_partidaActual.CantidadJugadores)
            {
                case 2:
                    lblCantidadJugadoresPartida.Content = Properties.Resources.lblDosJugadores;
                    Uri urlRelativa1 = new Uri(Properties.Resources.uriTableroDosJugadoresConFondo, UriKind.Relative);
                    mgTablero.Source = new BitmapImage(urlRelativa1);
                    break;
                case 3:
                    lblCantidadJugadoresPartida.Content = Properties.Resources.lblTresJugadores;
                    Uri urlRelativa2 = new Uri(Properties.Resources.uriTableroTresJugadoresConFondo, UriKind.Relative);
                    mgTablero.Source = new BitmapImage(urlRelativa2);
                    break;
                case 4:
                    lblCantidadJugadoresPartida.Content = Properties.Resources.lblCuatroJugadores;
                    Uri urlRelativa3 = new Uri(Properties.Resources.uriTableroCuatroJugadoresConFondo, UriKind.Relative);
                    mgTablero.Source = new BitmapImage(urlRelativa3);
                    break;
                default:
                    break;
            }

        }

        private void ClickSalirLobbyJugadores(object sender, RoutedEventArgs e)
        {
            IrMenuPrincipal();
        }
        
        private void IrMenuPrincipal()
        {
            try
            {
                SalirLobbyServidor();
                Window.GetWindow(this).Close();
                return;
            }
            catch (CommunicationException ex)
            {
                Logger log = new Logger(this.GetType());
                log.LogWarn("Error de Comunicación con el Servidor", ex);
            }
            Utilidades.SalirHastaInicioSesionDesdeJuegoYLobbyVentana(this);
        }

        public void JugadorEntroPartida()
        {
            try
            {
                _ = RecuperarDatosPartidaAsync(_codigoPartida);
            }
            catch (CommunicationException ex)
            {
                Logger log = new Logger(this.GetType());
                log.LogWarn("Error de Comunicación con el Servidor", ex);
                Window.GetWindow(this).Close();
            }
            catch (EntitySqlException ex)
            {
                Logger log = new Logger(this.GetType());
                log.LogError("Error Con Base de Datos", ex);
                Window.GetWindow(this).Close();
            }
        }

        public void EntrarPartida()
        {
            Logger log = new Logger(this.GetType());
            try
            {
                InstanceContext contextoCallback = new InstanceContext(this);
                _proxyLobby = new LobbyClient(contextoCallback);
                _proxyLobby.EntrarPartida(_codigoPartida, _cuentaUsuario.CorreoElectronico);
                return;
            }
            catch (CommunicationObjectFaultedException ex)
            {
                log.LogError("Se ha perdido la conexión previa", ex);
            }
            catch (CommunicationException ex)
            {
                log.LogWarn("Error de Comunicación con el Servidor", ex);
            }
            catch (TimeoutException ex)
            {
                log.LogWarn("Se agoto el tiempo de espera del servidor", ex);
            }
            throw new CommunicationException();
        }

        public void JugadorSalioPartida()
        {
            try
            {
                _ = RecuperarDatosPartidaAsync(_codigoPartida);
            }
            catch (CommunicationException ex)
            {
                Logger log = new Logger(this.GetType());
                log.LogWarn("Error de Comunicación con el Servidor", ex);
                Window.GetWindow(this).Close();
            }
            catch (EntitySqlException ex)
            {
                Logger log = new Logger(this.GetType());
                log.LogError("Error Con Base de Datos", ex);
                Window.GetWindow(this).Close();
            }
        }

        private void ClickIniciarPartida(object sender, RoutedEventArgs e)
        {
            _juegoIniciado++;
            btnIniciarPartida.IsEnabled = false;
            if (_cuentas.Length == _partidaActual.CantidadJugadores && txtBoxHost.Text == _cuentaUsuario.Nickname && _juegoIniciado == 1)
            {
                _ = InicializarPartidaParaTodos();
            }
        }

        private async Task InicializarPartidaParaTodos()
        {
            brdComenzarPartida.Visibility = Visibility.Visible;
            if (!ComprobarJugadores())
            {
                return;
            }
            await Task.Delay(3500);
            JugadorSalioPartida();
            btnIniciarPartida.IsEnabled = false;
            if (_cuentas.Length == _partidaActual.CantidadJugadores && txtBoxHost.Text == _cuentaUsuario.Nickname)
            {
                CambiarPaginas();
            }
            else
            {
                brdComenzarPartida.Visibility = Visibility.Hidden;
            }
        }
        private bool ComprobarJugadores()
        {
            Logger log = new Logger(this.GetType());
            try
            {
                _proxyLobby.ComprobarJugadoresExistentes(_codigoPartida);
                return true;
            }
            catch (CommunicationObjectFaultedException ex)
            {
                Utilidades.MostrarUnMensajeError(Properties.Resources.msgErrorIdentificarJugadores);
                log.LogError("Se ha perdido la conexión previa", ex);
            }
            catch (CommunicationException ex)
            {
                Utilidades.MostrarUnMensajeError(Properties.Resources.msgErrorConexion);
                log.LogWarn("Error de Comunicación con el Servidor", ex);
            }
            catch (TimeoutException ex)
            {
                Utilidades.MostrarUnMensajeError(Properties.Resources.msgErrorTiempoEsperaServidor);
                log.LogWarn("Se agoto el tiempo de espera del servidor", ex);
            }

            Utilidades.SalirHastaInicioSesionDesdeJuegoYLobbyVentana(this);
            return false;
        }

        private void CambiarPaginas() 
        {
            Logger log = new Logger(this.GetType());
            try
            {
                _proxyLobby.IniciarPartida(_codigoPartida);
                return;
            }
            catch (CommunicationObjectFaultedException ex)
            {
                Utilidades.MostrarUnMensajeError(Properties.Resources.msgEstadoDefectuoso);
                log.LogError("Se ha perdido la conexión previa", ex);
            }
            catch (CommunicationException ex)
            {
                Utilidades.MostrarUnMensajeError(Properties.Resources.msgErrorConexion);
                log.LogWarn("Error de Comunicación con el Servidor", ex);
            }
            catch (TimeoutException ex)
            {
                Utilidades.MostrarUnMensajeError(Properties.Resources.msgErrorTiempoEsperaServidor);
                log.LogWarn("Se agoto el tiempo de espera del servidor", ex);
            }
            IrMenuPrincipal();
        }

        public void HostInicioPartida()
        {
            List<CuentaSet> cuentasJugadores = _cuentas.ToList();
            JuegoLanzamientoPagina paginaJuego = new JuegoLanzamientoPagina(cuentasJugadores, _partidaActual.CantidadJugadores, _partidaActual.CodigoPartida.ToString(), _cuentaUsuario, _juegoYLobbyVentana);
            JugadoresChatPagina paginaChat = new JugadoresChatPagina(_cuentas, _cuentaUsuario, _partidaActual, _juegoYLobbyVentana);

            Constantes respuestaInicio = (paginaJuego.InicializarConexionYJuego() == Constantes.ERROR_CONEXION_SERVIDOR || 
                paginaChat.InicializarConexionServidorYChat() == Constantes.ERROR_CONEXION_SERVIDOR) ? Constantes.ERROR_CONEXION_SERVIDOR : Constantes.OPERACION_EXITOSA;

            switch (respuestaInicio)
            {
                case Constantes.OPERACION_EXITOSA:
                    CambiarPaginasAJuego(paginaJuego, paginaChat);
                    break;
                case Constantes.ERROR_CONEXION_SERVIDOR:
                    Utilidades.SalirHastaInicioSesionDesdeJuegoYLobbyVentana(this);
                    break;
            }
        }

        private void CambiarPaginasAJuego(Page paginaJuego, Page paginaChat)
        {
            try
            {
                SalirLobbyServidor();
                _juegoYLobbyVentana.CambiarFrameLobby(paginaJuego);
                _juegoYLobbyVentana.CambiarFrameListaAmigos(paginaChat);
                return;
            }
            catch (CommunicationException ex)
            {
                Logger log = new Logger(this.GetType());
                log.LogWarn("Error de Comunicación con el Servidor", ex);
            }
            Utilidades.SalirHastaInicioSesionDesdeJuegoYLobbyVentana(this);
        }

        private void SalirLobbyServidor()
        {
            Logger log = new Logger(this.GetType());
            try
            {
                _proxyLobby.SalirPartida(_codigoPartida);
                _juegoYLobbyVentana.EliminarContexto -= SalirLobbyServidor;
                return;
            }
            catch (CommunicationObjectFaultedException ex)
            {
                Utilidades.MostrarUnMensajeError(Properties.Resources.msgEstadoDefectuoso);
                log.LogError("Se ha perdido la conexión previa", ex);
            }
            catch (CommunicationException ex)
            {
                Utilidades.MostrarUnMensajeError(Properties.Resources.msgErrorConexion);
                log.LogWarn("Error de Comunicación con el Servidor", ex);
            }
            catch (TimeoutException ex)
            {
                Utilidades.MostrarUnMensajeError(Properties.Resources.msgErrorTiempoEsperaServidor);
                log.LogWarn("Se agoto el tiempo de espera del servidor", ex);
            }
            throw new CommunicationException();
        }
    }
}
