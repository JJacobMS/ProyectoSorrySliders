using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
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
    /// Lógica de interacción para MenuPrincipalPagina.xaml
    /// </summary>
    public partial class MenuPrincipalPagina : Page
    {
        private CuentaSet _cuentaUsuario;

        public MenuPrincipalPagina(string correoUsuario)
        {
            InitializeComponent();
            RecuperarDatosUsuario(correoUsuario);

        }
        public MenuPrincipalPagina(CuentaSet cuentaActual)
        {
            InitializeComponent();
            _cuentaUsuario = cuentaActual;
            InicializarDatosMenu();
        }

        private void InicializarDatosMenu()
        {
            txtBlockNickname.Text = _cuentaUsuario.Nickname;
            txtBlockCorreoElectronico.Text = _cuentaUsuario.CorreoElectronico;
            Utilidades.IngresarImagen(_cuentaUsuario.Avatar, this.mgBrushAvatar);
        }

        private void RecuperarDatosUsuario(string correoUsuario)
        {
            Constantes resultado;
            Logger log = new Logger(this.GetType());
            try
            {
                MenuPrincipalClient proxyRegistrarUsuario = new MenuPrincipalClient();
                string nickname;
                string contraseña;
                byte[] avatar;
                (resultado, nickname, avatar) = proxyRegistrarUsuario.RecuperarDatosUsuario(correoUsuario);
                if (resultado == Constantes.OPERACION_EXITOSA)
                {
                    _cuentaUsuario = new CuentaSet
                    {
                        Nickname = nickname,
                        Avatar = avatar,
                        CorreoElectronico = correoUsuario
                    };
                    InicializarDatosMenu();
                }
                proxyRegistrarUsuario.Close();
            }
            catch (CommunicationException ex)
            {
                Console.WriteLine(ex);
                resultado = Constantes.ERROR_CONEXION_SERVIDOR;
                log.LogError("Error de Comunicación con el Servidor", ex);
            }
            catch (TimeoutException ex)
            {
                Console.WriteLine(ex);
                resultado = Constantes.ERROR_TIEMPO_ESPERA_SERVIDOR;
                log.LogWarn("Se agoto el tiempo de espera del servidor", ex);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                resultado = Constantes.ERROR_CONEXION_SERVIDOR;
                log.LogFatal("Ha ocurrido un error inesperado", ex);
            }
            switch (resultado)
            {
                case Constantes.OPERACION_EXITOSA:
                    return;
                case Constantes.OPERACION_EXITOSA_VACIA:
                    MessageBox.Show(Properties.Resources.msgDatosCuentaVacia);
                    break;
                default:
                    Utilidades.MostrarMensajesError(resultado);
                    break;
            }
            IrInicioSesion();
        }

        private void ClickMostrarAjustes(object sender, RoutedEventArgs e)
        {
            AjustesVentana ajustes = new AjustesVentana();
            ajustes.IdiomaCambiado += ActualizarIdiomaMenuPrincipal;
            ajustes.ShowDialog();
        }

        private void ActualizarIdiomaMenuPrincipal()
        {
            txtBlockAjustes.Text = Properties.Resources.txtBlockAjustes;
            txtBlockUnirsePartida.Text = Properties.Resources.txtBlockUnirsePartida;
            txtBlockCrearLobby.Text = Properties.Resources.txtBlockCrearLobby;
            txtBlockJugadoresAmigos.Text = Properties.Resources.txtBlockJugadoresAmigos;
            txtBlockUnirsePartida.Text = Properties.Resources.txtBlockUnirsePartida;
            txtBlockSalir.Text = Properties.Resources.btnSalir;
            txtBlockTablaPuntuaciones.Text = Properties.Resources.txtBlockTablaPuntuaciones;
        }

        private void ClickSalirMenuPrincipal(object sender, RoutedEventArgs e)
        {
            IrInicioSesion();
        }
        private void IrInicioSesion()
        {
            MainWindow ventana = Window.GetWindow(this) as MainWindow;
            ventana.SalirSistema();
            InicioSesionPagina inicio = new InicioSesionPagina();
            this.NavigationService.Navigate(inicio);
        }
        private void ClickMostrarConfiguracionLobby(object sender, RoutedEventArgs e)
        {
            ConfiguracionLobby configuracionLobby = new ConfiguracionLobby(_cuentaUsuario);

            this.NavigationService.Navigate(configuracionLobby);
        }

        private void ClickMostrarUnirsePartida(object sender, RoutedEventArgs e)
        {
            UnirsePartidaPagina unirsePartida = new UnirsePartidaPagina(_cuentaUsuario);
            this.NavigationService.Navigate(unirsePartida);
        }


        private void ClickMostrarPuntuaciones(object sender, RoutedEventArgs e)
        {
            TableroPuntuacionesPagina tablero = new TableroPuntuacionesPagina(_cuentaUsuario);
            this.NavigationService.Navigate(tablero);
            
        }

        private void MouseLeftButtonDownMostrarDetallesCuenta(object sender, MouseButtonEventArgs e)
        {
            CuentaDetallesVentana detalles = new CuentaDetallesVentana(_cuentaUsuario);
            detalles.ModificarUsuarioCuenta += ActualizarPaginaMenuPrincipal;
            detalles.ModificarContrasena += CambiarPaginaModificarContrasena;
            detalles.AbrirVentana += AbrirVentanaDetalles;
            detalles.MostrarVentana();
        }

        private void AbrirVentanaDetalles(Window ventanaAbrir)
        {
            ventanaAbrir.ShowDialog();
        }

        private void ActualizarPaginaMenuPrincipal(UsuarioSet usuario)
        {
            RegistroUsuariosPagina registro = new RegistroUsuariosPagina(_cuentaUsuario, usuario);
            this.NavigationService.Navigate(registro);
        }

        private void CambiarPaginaModificarContrasena()
        {
            CambiarContrasenaPagina modificar = new CambiarContrasenaPagina(_cuentaUsuario);
            this.NavigationService.Navigate(modificar);
        }

        private void ClickIrJugadoresAmigos(object sender, RoutedEventArgs e)
        {
            ListaJugadoresPagina jugadores = new ListaJugadoresPagina(_cuentaUsuario);
            this.NavigationService.Navigate(jugadores);
        }
    }
}
