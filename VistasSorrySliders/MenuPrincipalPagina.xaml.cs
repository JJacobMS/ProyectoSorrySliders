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

        public void RecuperarDatosUsuario(String correoUsuario) 
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
                    txtBlockNickname.Text = nickname;
                    txtBlockCorreoElectronico.Text = correoUsuario;
                    IngresarImagen(avatar);
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

        public void IngresarImagen(byte[] avatar) 
        {
            try
            {
                BitmapImage bitmapImage = new BitmapImage();
                using (MemoryStream stream = new MemoryStream(avatar))
                {

                    bitmapImage.BeginInit();
                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImage.StreamSource = stream;
                    bitmapImage.DecodePixelWidth = 100;
                    bitmapImage.EndInit();
                    mgBrushAvatar.ImageSource = bitmapImage;
                }
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine(ex);
                Console.WriteLine("Argumento no válido al cargar la imagen");
            }
            catch (OutOfMemoryException ex)
            {
                Console.WriteLine(ex);
                Console.WriteLine("Error de memoria al cargar la imagen");
            }
            catch (System.IO.IOException ex)
            {
                Console.WriteLine(ex);
                Console.WriteLine("Error de lectura en el MemoryStream");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Console.WriteLine("Error de lectura al cargar la imagen");
            }
        }


        private void ClickMostrarAjustes(object sender, RoutedEventArgs e)
        {
            AjustesVentana ajustes = new AjustesVentana();
            ajustes.Show();
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
