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
        private List<JugadorLista> _jugadoresListas;

        public JugadoresChatPagina(CuentaSet[] cuentas, CuentaSet cuentaUsuario, PartidaSet partida )
        {
            InitializeComponent();
            _cuentas = cuentas;
            _cuentaUsuario = cuentaUsuario;
            _partida = partida;
            IngresarCallbacks();
            RemoverCallbacks();
            CargarJugadores();
        }

        private void TextChangedTamañoChat(object sender, TextChangedEventArgs e)
        {
            int tamañoMaximoMensaje = 20;
            if (txtBoxMensajeChat.Text.Length == tamañoMaximoMensaje) 
            {
                txtBoxMensajeChat.Text = txtBoxMensajeChat.Text.Substring(0, tamañoMaximoMensaje);
                txtBoxMensajeChat.SelectionStart = txtBoxMensajeChat.Text.Length;
            }
        }
        private void RemoverCallbacks() 
        {
            
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
            }
            catch (CommunicationException ex)
            {
                log.LogError("Error de Comunicación con el Servidor", ex);
            }
            catch (TimeoutException ex)
            {
                log.LogWarn("Se agoto el tiempo de espera del servidor", ex);
            }

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
                catch (CommunicationException ex)
                {
                    log.LogError("Error de Comunicación con el Servidor", ex);
                }
                catch (TimeoutException ex)
                {
                    log.LogWarn("Se agoto el tiempo de espera del servidor", ex);
                }
                catch (Exception ex)
                {
                    log.LogFatal("Ha ocurrido un error inesperado", ex);
                }
                txtBoxMensajeChat.Text = "";
            }
            
        }

        private void ClickSalirPartida(object sender, RoutedEventArgs e)
        {

        }

        public void DevolverMensaje(string nickname, string mensaje)
        {
            txtBlockMensajeChat.Text = nickname + ": " + mensaje;
        }

        private void CargarJugadores()
        {
            _jugadoresListas = new List<JugadorLista>();
            for (int i = 0; i < _cuentas.Count(); i++)
            {
                JugadorLista jugador = new JugadorLista
                {
                    Nickname = _cuentas[i].Nickname,
                    CorreoElectronico = _cuentas[i].CorreoElectronico,
                    EstaExpulsado = false
                };
                if (i == 0)
                {
                    jugador.EsHost = true;
                }
                _jugadoresListas.Add(jugador);
            }
            dtGridJugadores.ItemsSource = _jugadoresListas;
        }
    }
}
