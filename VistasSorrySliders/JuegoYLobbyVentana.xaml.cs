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
    /// Lógica de interacción para JuegoYLobbyVentana.xaml
    /// </summary>
    public partial class JuegoYLobbyVentana : Window, IUsuariosEnLineaCallback
    {
        private LobbyPagina _frameLobby;
        private bool _esInvitado;
        private CuentaSet _cuenta;
        private string _codigoPartida;
        private UsuariosEnLineaClient _proxyLinea;

        public event Action EliminarContexto;
        public event Action<string> ExpulsarJugador;
        public JuegoYLobbyVentana()
        {
            InitializeComponent();
        }

        public JuegoYLobbyVentana(CuentaSet cuenta, string codigoPartida, bool esInvitado)
        {
            _cuenta = cuenta;
            _codigoPartida = codigoPartida;
            _esInvitado = esInvitado;
            InitializeComponent();
            _frameLobby = new LobbyPagina(cuenta, codigoPartida, this);
            frameLobby.Content = _frameLobby;
            
            if (!esInvitado)
            {
                frameListaAmigos.Content = new ListaAmigosPagina(cuenta, codigoPartida);
            }
        }

        public void CerrarVentana(object sender, CancelEventArgs e) 
        {
            CerrarVentanaActual();
        }
        public void CerrarVentanaActual()
        {
            EliminarContexto?.Invoke();
            SalirCuentaRegistroPartidaBD();
            IrMenuUsuario();
            if (_proxyLinea != null)
            {
                SalirSistemaEnLinea();
            }
        }
        public void CambiarFrameLobby(Page paginaNueva)
        {
            frameLobby.Content = paginaNueva;
        }
        public void CambiarFrameListaAmigos(Page paginaNueva)
        {
            //_frame = pagina nueva;, ponerle a pagina nueva salirPartida();, y en ese metodo poner el RecargarListaJugadores
            frameListaAmigos.Content = paginaNueva;
        }
        public void ExpulsarJugadorJuego(string correoElectronico)
        {
            
            ExpulsarJugador?.Invoke(correoElectronico);
        }

        private void SalirCuentaRegistroPartidaBD()
        {
            Logger log = new Logger(this.GetType());
            try
            {
                UnirsePartidaClient proxyUnirse = new UnirsePartidaClient();
                proxyUnirse.SalirDelLobby(_cuenta.CorreoElectronico, _codigoPartida);
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

        public void IrMenuUsuario()
        {
            var ventanaPrincipal = new MainWindow(_cuenta.CorreoElectronico);

            if (!_esInvitado && ventanaPrincipal.EntrarSistemaEnLineaMenu())
            {
                MenuPrincipalPagina menu = new MenuPrincipalPagina(_cuenta);
                ventanaPrincipal.Content = menu;
            }
            else
            {
                InicioSesionPagina inicio = new InicioSesionPagina();
                ventanaPrincipal.Content = inicio;
            }

            if (_esInvitado)
            {
                EliminarCuentaProvisionalInvitado(_cuenta.CorreoElectronico);
            }
            ventanaPrincipal.Show();
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

        public void ComprobarJugador()
        {
            throw new NotImplementedException();
        }

        public bool EntrarSistemaEnLinea()
        {
            if (_esInvitado)
            {
                return true;
            }
            Logger log = new Logger(this.GetType());
            try
            {
                InstanceContext contexto = new InstanceContext(this);
                _proxyLinea = new UsuariosEnLineaClient(contexto);
                _proxyLinea.EntrarConCuenta(_cuenta.CorreoElectronico);
                return true;
            }
            catch (CommunicationException ex)
            {
                MessageBox.Show(Properties.Resources.msgErrorConexion);
                log.LogError("Error de Comunicación con el Servidor", ex);
            }
            catch (TimeoutException ex)
            {
                MessageBox.Show(Properties.Resources.msgErrorTiempoEsperaServidor);
                log.LogWarn("Se agoto el tiempo de espera del servidor", ex);
            }
            catch (Exception ex)
            {
                MessageBox.Show(Properties.Resources.msgErrorConexion);
                log.LogFatal("Ha ocurrido un error inesperado", ex);
            }
            return false;
        }

        private void SalirSistemaEnLinea()
        {
            Logger log = new Logger(this.GetType());
            try
            {
                _proxyLinea.SalirDelSistema(_cuenta.CorreoElectronico);
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
        }
    }
}
