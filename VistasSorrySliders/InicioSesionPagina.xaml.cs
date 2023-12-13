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
    /// Lógica de interacción para InicioSesionPagina.xaml
    /// </summary>
    public partial class InicioSesionPagina : Page
    {
        private InicioSesionClient _proxyInicioSesion;
        public InicioSesionPagina()
        {
            InitializeComponent();
        }

        private void ClickContinuar(object sender, RoutedEventArgs e)
        {
            VerificarCuenta();
        }

        private void ClickCancelar(object sender, RoutedEventArgs e)
        {
            InicioPagina inicio = new InicioPagina();
            this.NavigationService.Navigate(inicio);
        }

        private void ClickRegistrarCuenta(object sender, RoutedEventArgs e)
        {
            RegistroUsuariosPagina paginaRegistroUsuarios = new RegistroUsuariosPagina();
            this.NavigationService.Navigate(paginaRegistroUsuarios);
        }

        private void VerificarCuenta()
        {
            ReiniciarPantalla();
            bool datosCompletos = true;
            string correoIngresado = txtBoxCorreo.Text;
            string contrasena = pssBoxContrasena.Password;

            if (string.IsNullOrWhiteSpace(correoIngresado))
            {
                datosCompletos = false;
                txtBoxCorreo.Style = (Style)FindResource("estiloTxtBoxDatosRojo");
            }

            if (string.IsNullOrWhiteSpace(contrasena))
            {
                datosCompletos = false;
                pssBoxContrasena.Style = (Style)FindResource("estiloPssBoxContrasenaRojo");
            }

            if (datosCompletos)
            {
                Constantes resultado;
                Logger log = new Logger(this.GetType());
                try
                {
                    InicioSesionClient proxyInicioSesion = new InicioSesionClient();
                    resultado = proxyInicioSesion.VerificarExistenciaCorreoCuenta(correoIngresado);
                    proxyInicioSesion.Close();
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
                        CuentaSet cuentaVerificada = new CuentaSet { CorreoElectronico = correoIngresado, Contraseña = contrasena };
                        VerificarContrasena(cuentaVerificada);
                        break;
                    case Constantes.OPERACION_EXITOSA_VACIA:
                        txtBlockCorreoInvalido.Visibility = Visibility.Visible;
                        break;
                    default:
                        Utilidades.MostrarMensajesError(resultado);
                        break;
                }
            }

        }

        private void VerificarContrasena(CuentaSet cuentaPorVerificar)
        {
            Constantes resultado;
            Logger log = new Logger(this.GetType());
            try
            {
                InicioSesionClient proxyInicioSesion = new InicioSesionClient();
                resultado = proxyInicioSesion.VerificarContrasenaDeCuenta(cuentaPorVerificar);
                proxyInicioSesion.Close();
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
                    VerificarUsuarioUnicoSistema(cuentaPorVerificar.CorreoElectronico);
                    break;
                case Constantes.OPERACION_EXITOSA_VACIA:
                    txtBlockContrasenaInvalida.Visibility = Visibility.Visible;
                    break;
                default:
                    Utilidades.MostrarMensajesError(resultado);
                    break;
            }
        }

        private void ReiniciarPantalla()
        {
            txtBlockContrasenaInvalida.Visibility = Visibility.Hidden;
            txtBlockCorreoInvalido.Visibility = Visibility.Hidden;
            txtBoxCorreo.Style = (Style)FindResource("estiloTxtBoxDatosAzul");
            pssBoxContrasena.Style = (Style)FindResource("estiloPssBoxContrasenaAzul");
        }

        private void TextChagedCambiarTextoDeCorreo(object sender, TextChangedEventArgs e)
        {
            if (txtBoxCorreo.Text.Length > 100)
            {
                txtBoxCorreo.Text = txtBoxCorreo.Text.Substring(0, 100);
                txtBoxCorreo.SelectionStart = txtBoxCorreo.Text.Length;
            }
        }

        private void ClickEntrarComoInvitado(object sender, RoutedEventArgs e)
        {
            UnirsePartidaPagina unirsePaginaInvitado = new UnirsePartidaPagina();
            this.NavigationService.Navigate(unirsePaginaInvitado);
        }

        private void VerificarUsuarioUnicoSistema(string correoJugador)
        {
            Logger log = new Logger(this.GetType());
            try
            {
                Constantes puedoPasar;
                _proxyInicioSesion = new InicioSesionClient();
                puedoPasar = _proxyInicioSesion.JugadorEstaEnLinea(correoJugador);
                switch (puedoPasar)
                {
                    case Constantes.OPERACION_EXITOSA_VACIA:
                        CambiarPantallaMenuPrincipal(correoJugador);
                        break;
                    case Constantes.OPERACION_EXITOSA:
                        Utilidades.MostrarUnMensajeError(Properties.Resources.txtBlockCuentaYaEnLobby);
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

        private void CambiarPantallaMenuPrincipal(string correoVerificado)
        {
            VentanaPrincipal ventanaPrincipal = Window.GetWindow(this) as VentanaPrincipal;
            ventanaPrincipal.IndicarCorreoCuenta(correoVerificado);

            if (ventanaPrincipal.EntrarSistemaEnLineaMenu())
            {
                MenuPrincipalPagina menuPrincipal = new MenuPrincipalPagina();
                menuPrincipal.CambiarPaginaMenu += CambiarPaginaMenu;
                menuPrincipal.LlamarRecuperarDatosUsuario(correoVerificado);
            }
        }

        private void CambiarPaginaMenu(MenuPrincipalPagina pagina)
        {
            this.NavigationService.Navigate(pagina);
        }

    }
}
