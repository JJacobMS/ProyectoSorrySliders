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
            frameLobby.Content = null;
            frameListaAmigos.Content = null;
        }

        public Constantes InicializarPaginas()
        {
            _frameLobby = new LobbyPagina(_cuenta, _codigoPartida, this);
            Constantes respuestaLobby = _frameLobby.InicializarPagina();

            switch (respuestaLobby)
            {
                case Constantes.ERROR_CONEXION_BD:
                case Constantes.ERROR_CONEXION_SERVIDOR:
                    return respuestaLobby;
            }

            frameLobby.Content = _frameLobby;

            if (!_esInvitado)
            {
                ListaAmigosPagina amigos = new ListaAmigosPagina(_cuenta, _codigoPartida);
                Constantes resultadoAmigos = amigos.InicializarPagina();
                switch (resultadoAmigos)
                {
                    case Constantes.ERROR_CONEXION_BD:
                    case Constantes.ERROR_CONEXION_SERVIDOR:
                        return resultadoAmigos;
                }
                frameListaAmigos.Content = amigos;
            }

            return Constantes.OPERACION_EXITOSA;
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
        }
        public void DesucribirseDeCerrarVentana()
        {
            Closing -= CerrarVentana;
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
                Utilidades.MostrarUnMensajeError(Properties.Resources.msgErrorConexion);
                log.LogError("Error de Comunicación con el Servidor", ex);
            }
            catch (TimeoutException ex)
            {
                Utilidades.MostrarUnMensajeError(Properties.Resources.msgErrorConexion);
                log.LogWarn("Se agoto el tiempo de espera del servidor", ex);
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
            Logger log = new Logger(this.GetType());
            log.LogInfo("Jugador en línea");
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
                Utilidades.MostrarUnMensajeError(Properties.Resources.msgErrorConexion);
                log.LogError("Error de Comunicación con el Servidor", ex);
            }
            catch (TimeoutException ex)
            {
                Utilidades.MostrarUnMensajeError(Properties.Resources.msgErrorTiempoEsperaServidor);
                log.LogWarn("Se agoto el tiempo de espera del servidor", ex);
            }
            return false;
        }

    }
}
