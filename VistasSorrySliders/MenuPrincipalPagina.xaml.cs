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
using System.Windows.Forms;
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
            
            //Metodo calcular partidas todos los jugadores SERVIDOR

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
            try
            {
                MenuPrincipalClient proxyRegistrarUsuario = new MenuPrincipalClient();
                string nickname;
                byte[] avatar;
                (resultado, nickname, avatar) = proxyRegistrarUsuario.RecuperarDatosUsuario(correoUsuario);
                _cuentaUsuario = new CuentaSet
                {
                    Nickname = nickname,
                    Avatar = avatar,
                    CorreoElectronico = correoUsuario
                };
                proxyRegistrarUsuario.Close();
            }
            catch (CommunicationException ex)
            {
                Console.WriteLine(ex);
                resultado = Constantes.ERROR_CONEXION_SERVIDOR;
                System.Windows.Forms.MessageBox.Show(Properties.Resources.msgErrorConexion);
            }
            catch (TimeoutException ex)
            {
                Console.WriteLine(ex);
                resultado = Constantes.ERROR_CONEXION_SERVIDOR;
                System.Windows.Forms.MessageBox.Show(Properties.Resources.msgErrorConexion);
            }

            switch (resultado)
            {
                case Constantes.OPERACION_EXITOSA:
                    InicializarDatosMenu();
                    break;
                case Constantes.OPERACION_EXITOSA_VACIA:
                    break;
                case Constantes.ERROR_CONEXION_BD:
                    System.Windows.Forms.MessageBox.Show(Properties.Resources.msgErrorBaseDatos);
                    break;
                case Constantes.ERROR_CONSULTA:
                    System.Windows.Forms.MessageBox.Show(Properties.Resources.msgErrorBaseDatos);
                    break;
                case Constantes.ERROR_CONEXION_SERVIDOR:
                    System.Windows.Forms.MessageBox.Show(Properties.Resources.msgErrorConexion);
                    break;
            }

        }

        private void ClickMostrarAjustes(object sender, RoutedEventArgs e)
        {
            AjustesVentana ajustes = new AjustesVentana();
            ajustes.IdiomaCambiado += ActualizarVentanaMenuPrincipal;
            ajustes.ShowDialog();
        }

        private void ActualizarVentanaMenuPrincipal()
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

        private void MouseLeftButtonDownMostrarDetallesCuenta(object sender, MouseButtonEventArgs e)
        {
            CuentaDetallesVentana detalles = new CuentaDetallesVentana(_cuentaUsuario);
            detalles.ShowDialog();
        }
    }
}
