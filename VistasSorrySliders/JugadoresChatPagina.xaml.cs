using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using VistasSorrySliders.LogicaJuego;
using VistasSorrySliders.ServicioSorrySliders;

namespace VistasSorrySliders
{
    /// <summary>
    /// Lógica de interacción para JugadoresChatPagina.xaml
    /// </summary>
    public partial class JugadoresChatPagina : Page, IChatCallback
    {
        private CuentaSet _cuentaUsuario;
        private CuentaSet[] _cuentas;
        private PartidaSet _partida;
        private ChatClient _proxyChat;
        private ObservableCollection<JugadorLista> _jugadoresListas;
        private JuegoYLobbyVentana _juegoYLobbyVentana;

        public JugadoresChatPagina(CuentaSet[] cuentas, CuentaSet cuentaUsuario, PartidaSet partida, JuegoYLobbyVentana ventana )
        {
            InitializeComponent();
            _cuentas = cuentas;
            _cuentaUsuario = cuentaUsuario;
            _partida = partida;

            _juegoYLobbyVentana = ventana;
            _juegoYLobbyVentana.EliminarContexto += RemoverCallbacks;

        }

        public Constantes InicializarConexionServidorYChat()
        {
            Logger log = new Logger(this.GetType());
            try
            {
                IngresarCallbacks();
            }
            catch (CommunicationException ex)
            {
                log.LogWarn("Error de Comunicación con el Servidor", ex);
                return Constantes.ERROR_CONEXION_SERVIDOR;
            }
            CargarJugadores();
            return Constantes.OPERACION_EXITOSA;
        }

        private void TextChangedTamañoChat(object sender, TextChangedEventArgs e)
        {
            int tamañoMaximoMensaje = 20;
            if (txtBoxMensajeChat.Text.Length > tamañoMaximoMensaje) 
            {
                txtBoxMensajeChat.Text = txtBoxMensajeChat.Text.Substring(0, tamañoMaximoMensaje);
                txtBoxMensajeChat.SelectionStart = txtBoxMensajeChat.Text.Length;
            }
        }
        private void RemoverCallbacks() 
        {
            Logger log = new Logger(this.GetType());
            try
            {
                _proxyChat.SalirChatListaJugadores(_partida.CodigoPartida.ToString(), _cuentaUsuario.CorreoElectronico);
                _juegoYLobbyVentana.EliminarContexto -= RemoverCallbacks;
                return;
            }
            catch (CommunicationObjectFaultedException ex)
            {
                Utilidades.MostrarUnMensajeError(Properties.Resources.msgEstadoDefectuoso);
                log.LogWarn("Se ha perdido la conexión previa", ex);
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

        private void IngresarCallbacks() 
        {
            Logger log = new Logger(this.GetType());
            string codigoPartida = _partida.CodigoPartida + "";
            try
            {
                InstanceContext contextoCallback = new InstanceContext(this);
                ChatClient proxyChat = new ChatClient(contextoCallback);
                _proxyChat = proxyChat;
                _proxyChat.IngresarAlChat(codigoPartida, _cuentaUsuario.CorreoElectronico);
                return;
            }
            catch (CommunicationObjectFaultedException ex)
            {
                Utilidades.MostrarUnMensajeError(Properties.Resources.msgEstadoDefectuoso);
                log.LogWarn("Se ha perdido la conexión previa", ex);
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

        private void ClickEnviarMensaje(object sender, RoutedEventArgs e)
        {
            if(txtBoxMensajeChat.Text.Length > 0) 
            {
                Logger log = new Logger(this.GetType());
                try
                {
                    string codigoPartida = _partida.CodigoPartida + "";
                    _proxyChat.ChatJuego(codigoPartida, _cuentaUsuario.Nickname, txtBoxMensajeChat.Text);

                }
                catch (CommunicationObjectFaultedException ex)
                {
                    Utilidades.MostrarUnMensajeError(Properties.Resources.msgEstadoDefectuoso);
                    log.LogWarn("Se ha perdido la conexión previa", ex);
                }
                catch (CommunicationException ex)
                {
                    log.LogWarn("Error de Comunicación con el Servidor", ex);
                    Utilidades.MostrarUnMensajeError(Properties.Resources.msgErrorConexion);
                }
                catch (TimeoutException ex)
                {
                    Utilidades.MostrarUnMensajeError(Properties.Resources.msgErrorTiempoEsperaServidor);
                    log.LogWarn("Se agoto el tiempo de espera del servidor", ex);
                }
                txtBoxMensajeChat.Text = "";
            }
            
        }

        private void ClickSalirPartida(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(this).Close();
        }

        public void DevolverMensaje(string nickname, string mensaje)
        {
            AgregarMensaje(nickname, mensaje);
        }

        private void AgregarMensaje(string nickname, string mensaje) 
        {
            Label mensajeChat = new Label();
            mensajeChat.Style = (Style) FindResource("estiloLblMensajeChat");
            mensajeChat.Content = nickname+": "+mensaje;
            stcPanelMensaje.Children.Add(mensajeChat);
        }

        private void CargarJugadores()
        {
            _jugadoresListas = new ObservableCollection<JugadorLista>();
            for (int i = 0; i < _cuentas.Count(); i++)
            {
                JugadorLista jugador = new JugadorLista
                {
                    Nickname = _cuentas[i].Nickname,
                    CorreoElectronico = _cuentas[i].CorreoElectronico,
                    EstaExpulsado = false,
                    EstaEnLinea = true,
                    SePuedeExpulsar = true
                };
                if (_cuentas[i].CorreoElectronico.Equals(_cuentaUsuario.CorreoElectronico))
                {
                    jugador.SePuedeExpulsar = false;
                }
                _jugadoresListas.Add(jugador);
            }
            dtGridJugadores.ItemsSource = _jugadoresListas;

        }

        private void PreviewMouseLeftButtonDownExpulsarJugadorJuego(object sender, MouseButtonEventArgs e)
        {
            Button botonExpulsar = (Button)sender;
            JugadorLista jugador = (JugadorLista) botonExpulsar.CommandParameter;
            botonExpulsar.IsEnabled = false;
            EnviarJugadorExpulsado(jugador);
        }

        private void EnviarJugadorExpulsado(JugadorLista jugador)
        {
            Logger log = new Logger(this.GetType());
            try
            {
                _proxyChat.ExpulsarJugadorPartida(_partida.CodigoPartida.ToString(), jugador.CorreoElectronico);
            }
            catch (CommunicationObjectFaultedException ex)
            {
                Utilidades.MostrarUnMensajeError(Properties.Resources.msgEstadoDefectuoso);
                log.LogWarn("Se ha perdido la conexión previa", ex);
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
        }

        public void ExpulsadoDeJugador(string correoElectronico)
        {
            if (correoElectronico.Equals(_cuentaUsuario.CorreoElectronico))
            {
                _juegoYLobbyVentana.DesuscribirseDeCerrarVentana();
                _juegoYLobbyVentana.IrMenuUsuario();
                Window.GetWindow(this).Close();
                Utilidades.MostrarMensajeInformacion(Properties.Resources.msgExpulsarJugador, Properties.Resources.msgExpulsadoTitulo);
            }
            else
            {
                NotificarExpulsionJuego(correoElectronico);
                _ = MostrarAvisoJugadorSalioJuego(correoElectronico, true);
            }
        }

        public void JugadorSalioListaJugadores(string correoElectronico)
        {
            NotificarExpulsionJuego(correoElectronico);
            _ = MostrarAvisoJugadorSalioJuego(correoElectronico, false);
        }

        private void NotificarExpulsionJuego(string correoElectronico)
        {
            for (int i = 0; i < _jugadoresListas.Count; i++)
            {
                if (_jugadoresListas[i].CorreoElectronico.Equals(correoElectronico))
                {
                    _jugadoresListas[i].EstaExpulsado = true;
                    _jugadoresListas[i].EstaEnLinea = false;
                    _jugadoresListas[i].SePuedeExpulsar = false;

                    dtGridJugadores.Items.Refresh();
                }
            }
        }

        private async Task MostrarAvisoJugadorSalioJuego(string correoJugador, bool esBaneo)
        {
            string nicknameJugador = DevolverNicknameDelCorreo(correoJugador);
            string mensaje;
            if (esBaneo)
            {
                mensaje = string.Format(Properties.Resources.msgJugadorHaSidoExpulsado, nicknameJugador);
            }
            else
            {
                mensaje = string.Format(Properties.Resources.msgJugadorHaSalidoJuego, nicknameJugador);
            }
            txtBlockJugadorDesconectado.Text = mensaje;
            brdJugadorDesconectado.Visibility = Visibility.Visible;
            await Task.Delay(2500);
            brdJugadorDesconectado.Visibility = Visibility.Hidden;
        }

        private string DevolverNicknameDelCorreo(string correo)
        {
            JugadorLista jugadorLista = _jugadoresListas.FirstOrDefault(jugador => jugador.CorreoElectronico.Equals(correo));
            if (jugadorLista == null) { return ""; }
            return jugadorLista.Nickname;
        }
    }
}
