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
using VistasSorrySliders.LogicaJuego;
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

        public bool EsInvitado { get => _esInvitado; }

        public event Action EliminarContexto;

        public JuegoYLobbyVentana(CuentaSet cuenta, string codigoPartida, bool esInvitado, UsuariosEnLineaClient proxyLinea)
        {
            _proxyLinea = proxyLinea;
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
            try
            {
                EliminarContexto?.Invoke();
                EliminarDiccionariosRestantes();
                SalirCuentaRegistroPartidaBD();
                IrMenuUsuario();
            }
            catch (CommunicationException ex)
            {
                Logger log = new Logger(this.GetType());
                log.LogError("Error de Comunicación con el Servidor", ex);
                Utilidades.MostrarInicioSesion();
            }
        }
        private void EliminarDiccionariosRestantes()
        {
            Logger log = new Logger(this.GetType());
            try
            {
                UnirsePartidaClient proxy = new UnirsePartidaClient();
                proxy.SalirJuegoCompleto(_codigoPartida, _cuenta.CorreoElectronico);
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

        public void CambiarVentanaGanadores(List<JugadorGanador> listaPuntuaciones)
        {
            DesuscribirseDeCerrarVentana();

            if (_esInvitado)
            {
                try
                {
                    SalirCuentaRegistroPartidaBD();
                    EliminarCuentaProvisionalInvitado(_cuenta.CorreoElectronico);
                }
                catch (CommunicationException ex)
                {
                    Logger log = new Logger(this.GetType());
                    log.LogError("Error de Comunicación con el Servidor", ex);
                    Utilidades.MostrarInicioSesion(this);
                }
            }
            
            IrVentanaGanadores(listaPuntuaciones);
        }
        private void IrVentanaGanadores(List<JugadorGanador> listaPuntuaciones)
        {
            VentanaPrincipal ventanaPrincipal = new VentanaPrincipal(_proxyLinea, _cuenta.CorreoElectronico);
            TableroGanadoresPartidaPagina paginaGanadores = new TableroGanadoresPartidaPagina(_cuenta, listaPuntuaciones, _esInvitado);
            ventanaPrincipal.Content = paginaGanadores;
            ventanaPrincipal.Show();
            Close();
        }

        public void DesuscribirseDeCerrarVentana()
        {
            Closing -= CerrarVentana;
        }
        public void CambiarFrameLobby(Page paginaNueva)
        {
            frameLobby.Content = paginaNueva;
        }
        public void CambiarFrameListaAmigos(Page paginaNueva)
        {
            frameListaAmigos.Content = paginaNueva;
        }
        private void SalirCuentaRegistroPartidaBD()
        {
            Logger log = new Logger(this.GetType());
            try
            {
                UnirsePartidaClient proxyUnirse = new UnirsePartidaClient();
                proxyUnirse.SalirDelLobby(_cuenta.CorreoElectronico, _codigoPartida);
                return;
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
            throw new CommunicationException();
        }

        public void IrMenuUsuario()
        {
            VentanaPrincipal ventanaPrincipal = new VentanaPrincipal(_proxyLinea, _cuenta.CorreoElectronico);
            if (!_esInvitado)
            {
                MenuPrincipalPagina menu = new MenuPrincipalPagina(_cuenta);
                ventanaPrincipal.Content = menu;
            }
            else
            {
                EliminarCuentaProvisionalInvitado(_cuenta.CorreoElectronico);
                InicioSesionPagina inicio = new InicioSesionPagina();
                ventanaPrincipal.Content = inicio;
            }
            ventanaPrincipal.Show();
        }
        
        private void EliminarCuentaProvisionalInvitado(string correoProvisional)
        {
            Logger log = new Logger(this.GetType());
            try
            {
                UnirsePartidaClient proxyRecuperarJugadores = new UnirsePartidaClient();
                proxyRecuperarJugadores.EliminarCuentaProvisional(correoProvisional);
                return;
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
            throw new CommunicationException();
        }

        public void ComprobarJugador()
        {
            Logger log = new Logger(this.GetType());
            log.LogInfo("Jugador en línea");
        }

    }
}
