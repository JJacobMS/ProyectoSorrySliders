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

        public JugadoresChatPagina(CuentaSet[] cuentas, CuentaSet cuentaUsuario, PartidaSet partida )
        {
            InitializeComponent();
            _cuentas = cuentas;
            _cuentaUsuario = cuentaUsuario;
            _partida = partida;
            IngresarCallbacks();
            RemoverCallbacks();
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
            Console.WriteLine("Ingresando al callback");
            InstanceContext contextoCallback = new InstanceContext(this);
            ChatClient proxyChat = new ChatClient(contextoCallback);
            _proxyChat = proxyChat;
            string codigoPartida = _partida.CodigoPartida + "";
            _proxyChat.IngresarAlChat(codigoPartida);
        }

        private void ClickEnviarMensaje(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Click enviar mensaje");
            if(txtBoxMensajeChat.Text.Length > 0) 
            {
                Logger log = new Logger(this.GetType());
                try
                {
                    Console.WriteLine("Click enviando mensaje a servidor");
                    string codigoPartida = _partida.CodigoPartida + "";
                    _proxyChat.ChatJuego(codigoPartida, _cuentaUsuario.Nickname, txtBoxMensajeChat.Text);

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
            
        }

        private void ClickSalirPartida(object sender, RoutedEventArgs e)
        {

        }

        public void DevolverMensaje(string nickname, string mensaje)
        {
            Console.WriteLine(nickname + ": " + mensaje);
            txtBlockMensajeChat.Text = nickname + ": " + mensaje;

        }
    }
}
