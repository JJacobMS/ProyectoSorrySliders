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
        private bool _esInvitado;
        public UnirsePartidaPagina(CuentaSet cuentaActual)
        {
            InitializeComponent();
            _cuentaActual = cuentaActual;
            _esInvitado = false;
        }

        public UnirsePartidaPagina()
        {
            InitializeComponent();
            _esInvitado = true;
            grdNickname.Visibility = Visibility.Visible;
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
                txtBlockErrorPartida.Visibility = Visibility.Visible;
                txtBlockErrorPartida.Text = Properties.Resources.txtBlockCodigoNoValido;
                esValido = false;
            }

            if (esValido)
            {
                if (_esInvitado)
                {
                    ValidarInvitado(codigoPartida);
                    return;
                }
                UnirsePartida(codigoPartida);
            }
        }
        private void ValidarInvitado(string codigo)
        {
            bool esValido = true;
            string nickname = txtBoxNickname.Text;
            if (string.IsNullOrWhiteSpace(nickname))
            {
                txtBoxNickname.Style = (Style)FindResource("estiloTxtBoxDatosRojo");
                txtBlockNicknameNoValido.Visibility = Visibility.Visible;
                esValido = false;
            }

            if (esValido)
            {
                CrearCuentaProvisionalInvitado(codigo);
            }
        }

        private void UnirsePartida(string codigo)
        {
            Logger log = new Logger(this.GetType());
            Constantes resultado;
            int numeroMaximoJugadores = 0;
            try
            {
                UnirsePartidaClient proxyUnirsePartida = new UnirsePartidaClient();
                (resultado, numeroMaximoJugadores) = proxyUnirsePartida.UnirseAlLobby(codigo, _cuentaActual.CorreoElectronico);
            }
            catch (CommunicationException ex)
            {
                resultado = Constantes.ERROR_CONEXION_SERVIDOR;
                log.LogError("Error de Comunicación con el Servidor", ex);
            }
            catch (TimeoutException ex)
            {
                resultado = Constantes.ERROR_TIEMPO_ESPERA_SERVIDOR;
                log.LogWarn("Se agoto el tiempo de espera del servidor", ex);
            }

            switch (resultado)
            {
                case Constantes.OPERACION_EXITOSA:
                    EntrarLobby(codigo);
                    break;
                case Constantes.OPERACION_EXITOSA_VACIA:
                    MostrarErrorJugadores(numeroMaximoJugadores);
                    break;
                default:
                    Utilidades.MostrarMensajesError(resultado);
                    break;
            }
        }

        private void CrearCuentaProvisionalInvitado(string codigo)
        {
            byte[] avatar = Utilidades.GenerarImagenDefectoBytes();

            if (avatar == null || avatar.Length == 0)
            {
                SalirDeUnirse();
                return;
            }

            _cuentaActual = new CuentaSet
            {
                CorreoElectronico = Guid.NewGuid().ToString(),
                Avatar = avatar,
                Nickname = txtBoxNickname.Text
            };

            Constantes resultado;
            Logger log = new Logger(this.GetType());
            try
            {
                UnirsePartidaClient proxyUnirsePartida = new UnirsePartidaClient();
                resultado = proxyUnirsePartida.CrearCuentaProvisionalInvitado(_cuentaActual);
                proxyUnirsePartida.Close();
            }
            catch (CommunicationException ex)
            {
                resultado = Constantes.ERROR_CONEXION_SERVIDOR;
                log.LogError("Error de Comunicación con el Servidor", ex);
            }
            catch (TimeoutException ex)
            {
                resultado = Constantes.ERROR_CONEXION_SERVIDOR;
                log.LogWarn("Se agoto el tiempo de espera del servidor", ex);
            }
            switch (resultado)
            {
                case Constantes.OPERACION_EXITOSA:
                    UnirsePartida(codigo);
                    break;
                default:
                    Utilidades.MostrarMensajesError(resultado);
                    SalirDeUnirse();
                    break;
            }
        }

        private void EntrarLobby(string codigo)
        {
            JuegoYLobbyVentana lobbyUnirse = new JuegoYLobbyVentana(_cuentaActual, codigo, _esInvitado);
            if (lobbyUnirse.EntrarSistemaEnLinea())
            {
                lobbyUnirse.Show();
                MainWindow ventanaPrincipal = (MainWindow) Window.GetWindow(this);
                ventanaPrincipal.EliminarProxyLineaAnterior();
                Window.GetWindow(this).Close();
            }
        }

        private void MostrarErrorJugadores(int numeroMaximoJugadores)
        {
            txtBlockErrorPartida.Visibility = Visibility.Visible;
            switch (numeroMaximoJugadores)
            {
                case -3:
                    txtBlockErrorPartida.Text = Properties.Resources.txtBlockJuegoNoValido;
                    break;
                case -2:
                    txtBlockErrorPartida.Text = Properties.Resources.txtBlockJugadorBaneado;
                    break;
                case -1:
                    txtBlockErrorPartida.Text = Properties.Resources.txtBlockCuentaYaEnLobby;
                    break;
                case 0:
                    txtBlockErrorPartida.Text = Properties.Resources.txtBlockCodigoNoValido;
                    break;
            }

            if (numeroMaximoJugadores > 0)
            {
                txtBlockErrorPartida.Text = Properties.Resources.txtBlockJugadoresMaximosExcedidos;
            }
        }

        private void ReiniciarEstilosPantalla()
        {
            txtBlockErrorPartida.Visibility = Visibility.Hidden;
            txtBlockNicknameNoValido.Visibility = Visibility.Hidden;
            txtBoxNickname.Style = (Style)FindResource("estiloTxtBoxDatosAzul");
            txtBoxCodigo.Style = (Style)FindResource("estiloTxtBoxDatosAzul");
        }

        private bool EsUniqueIdentifierValido(string uid)
        {
            Logger log = new Logger(this.GetType());
            try
            {
                int segundos = 3;
                string patron = @"^[0-9A-Fa-f]{8}-[0-9A-Fa-f]{4}-[0-9A-Fa-f]{4}-[0-9A-Fa-f]{4}-[0-9A-Fa-f]{12}$";
                TimeSpan tiempoAgotadoPatron = TimeSpan.FromSeconds(segundos);
                return Regex.IsMatch(uid, patron, RegexOptions.None, tiempoAgotadoPatron);
            }
            catch (RegexMatchTimeoutException ex)
            {
                log.LogWarn("El tiempo de espera para la expresión se ha agotado", ex);
                return false;
            }
        }

        private void ClickSalir(object sender, RoutedEventArgs e)
        {
            SalirDeUnirse();
        }

        private void SalirDeUnirse()
        {
            if (_esInvitado)
            {
                InicioSesionPagina inicioPagina = new InicioSesionPagina();
                this.NavigationService.Navigate(inicioPagina);
                return;
            }
            MenuPrincipalPagina menu = new MenuPrincipalPagina(_cuentaActual);
            this.NavigationService.Navigate(menu);
        }
    }
}
