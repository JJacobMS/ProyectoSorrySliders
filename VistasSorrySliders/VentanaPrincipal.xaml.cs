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
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class VentanaPrincipal : NavigationWindow, IUsuariosEnLineaCallback
    {
        private UsuariosEnLineaClient _proxyLinea;
        private string _correo;
        private InicioSesionClient _proxyInicio;

        public UsuariosEnLineaClient ProxyLinea { get => _proxyLinea; }

        public VentanaPrincipal()
        {
            InitializeComponent();
        }
        public VentanaPrincipal(UsuariosEnLineaClient proxyEnLinea, string correo)
        {
            InitializeComponent();
            _proxyLinea = proxyEnLinea;
            _correo = correo;
        }

        public void DesuscribirseCerrarVentana()
        {
            Closing -= CerrarVentana;
        }

        private void CerrarVentana(object sender, CancelEventArgs e)
        {
            if (_proxyLinea != null)
            {
                SalirSistema();
            }
        }

        public void IndicarCorreoCuenta(string correo)
        {
            _correo = correo;
        }

        public bool EntrarSistemaEnLineaMenu()
        {
            Logger log = new Logger(this.GetType());
            try
            {
                InstanceContext contexto = new InstanceContext(this);
                _proxyLinea = new UsuariosEnLineaClient(contexto);
                _proxyLinea.EntrarConCuenta(_correo);
                return true;
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
            return false;
        }

        public void SalirSistema()
        {
            Logger log = new Logger(this.GetType());
            try
            {
                _proxyLinea.SalirDelSistema(_correo);
            }
            catch (CommunicationException ex)
            {
                log.LogWarn("Error de Comunicación con el Servidor", ex);
            }
            catch (TimeoutException ex)
            {
                log.LogWarn("Se agoto el tiempo de espera del servidor", ex);
            }
        }

        public void ComprobarJugador()
        {
            Logger log = new Logger(this.GetType());
            log.LogInfo("Jugador en línea");
        }

        public void ComprobarConexionUsuario (string correoElectronico)
        {
            Logger log = new Logger(this.GetType());
            try
            {
                Constantes puedoPasar;
                _proxyInicio = new InicioSesionClient();
                puedoPasar = _proxyInicio.JugadorEstaEnLinea(correoElectronico);
                switch (puedoPasar)
                {
                    case Constantes.OPERACION_EXITOSA_VACIA:
                        Utilidades.MostrarInicioSesion();
                        break;
                    case Constantes.OPERACION_EXITOSA:
                        break;
                }
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
        }
    }
}
