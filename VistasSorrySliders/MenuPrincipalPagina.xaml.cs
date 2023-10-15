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
            //Metodo BuscarUsuarioPorCorreo Puedo reutilizar la consulta de buscar correo, debe regresar Nickname y Avatar SERVIDOR
            //Metodo settear datos usuario txtBlockCorreo, txtBlockNickname y imgBrushAvatar
            //Metodo calcular partidas todos los jugadores SERVIDOR
            //
            RecuperarDatosUsuario(correoUsuario);
        }

        private void RecuperarDatosUsuario(string correoUsuario) 
        {
            Constantes resultado;
            string nickname = "";
            byte[] avatar = null;

            try
            {
                MenuPrincipalClient proxyRegistrarUsuario = new MenuPrincipalClient();
                (resultado, nickname, avatar) = proxyRegistrarUsuario.RecuperarDatosUsuario(correoUsuario);
                _cuentaUsuario = new CuentaSet
                {
                    Nickname = nickname, Avatar = avatar, CorreoElectronico = correoUsuario
                };
                proxyRegistrarUsuario.Close();
            }
            catch (CommunicationException excepcion)
            {
                resultado = Constantes.ERROR_CONEXION_SERVIDOR;
                Console.WriteLine(excepcion);
            }
            catch (TimeoutException excepcion)
            {
                resultado = Constantes.ERROR_CONEXION_SERVIDOR;
                Console.WriteLine(excepcion);
            }

            switch (resultado)
            {
                case Constantes.OPERACION_EXITOSA:
                    txtBlockNickname.Text = _cuentaUsuario.Nickname;
                    txtBlockCorreoElectronico.Text = _cuentaUsuario.CorreoElectronico;
                    Utilidades.IngresarImagen(_cuentaUsuario.Avatar, this.mgBrushAvatar);
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
            ajustes.Show();
        }

        private void ActualizarVentanaMenuPrincipal()
        {
            txtBlockAjustes.Text = Properties.Resources.txtBlockAjustes;
            txtBlockUnirsePartida.Text = Properties.Resources.txtBlockUnirsePartida;
            txtBlockAjustes.Text = Properties.Resources.txtBlockAjustes;
            txtBlockAjustes.Text = Properties.Resources.txtBlockAjustes;
            txtBlockAjustes.Text = Properties.Resources.txtBlockAjustes;

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
    }
}
