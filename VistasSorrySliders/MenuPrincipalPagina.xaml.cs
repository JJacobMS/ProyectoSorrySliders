using System;
using System.Collections.Generic;
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

        public CuentaSet CuentaUsuario { get => _cuentaUsuario; set => _cuentaUsuario = value; }

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
            Constantes resultado = Constantes.OPERACION_EXITOSA_VACIA;
            string nickname;
            byte[] avatar;

            try
            {

                MenuPrincipalClient proxyRegistrarUsuario = new MenuPrincipalClient();
                (resultado, nickname, avatar) = proxyRegistrarUsuario.RecuperarDatosUsuario(correoUsuario);
                proxyRegistrarUsuario.Close();
                switch (resultado)
                {
                    case Constantes.OPERACION_EXITOSA:
                        txtBlockNickname.Text = nickname;
                        txtBlockCorreoElectronico.Text = correoUsuario;
                        IngresarImagen(avatar);

                        CuentaUsuario.CorreoElectronico = correoUsuario;
                        CuentaUsuario.Nickname = nickname;
                        CuentaUsuario.Avatar = avatar;
                        break;
                    case Constantes.OPERACION_EXITOSA_VACIA:
                        this.NavigationService.GoBack();
                        break;
                    case Constantes.ERROR_CONEXION_BD:
                        System.Windows.Forms.MessageBox.Show(Properties.Resources.msgErrorBaseDatos);
                        this.NavigationService.GoBack();
                        break;
                    case Constantes.ERROR_CONSULTA:
                        System.Windows.Forms.MessageBox.Show(Properties.Resources.msgErrorBaseDatos);
                        this.NavigationService.GoBack();
                        break;
                    case Constantes.ERROR_CONEXION_SERVIDOR:
                        System.Windows.Forms.MessageBox.Show(Properties.Resources.msgErrorConexion);
                        this.NavigationService.GoBack();
                        break;
                }
            }
            catch (CommunicationException excepcion)
            {
                resultado = Constantes.ERROR_CONEXION_SERVIDOR;
                System.Windows.Forms.MessageBox.Show(Properties.Resources.msgErrorConexion);
            }
            catch (TimeoutException excepcion)
            {
                resultado = Constantes.ERROR_CONEXION_SERVIDOR;
                System.Windows.Forms.MessageBox.Show(Properties.Resources.msgErrorConexion);

            }
                
        }

        public void IngresarImagen(byte[] avatar) 
        {
            try
            {
                BitmapImage bitmapImage = new BitmapImage();
                using (MemoryStream stream = new MemoryStream(avatar))
                {
                    BitmapDecoder decoder = BitmapDecoder.Create(stream, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
                    bitmapImage.BeginInit();
                    bitmapImage.StreamSource = stream;
                    bitmapImage.EndInit();
                }
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine("Argumento no válido al cargar la imagen");
            }
            catch (OutOfMemoryException ex)
            {
                Console.WriteLine("Error de memoria al cargar la imagen");
            }
            catch (System.IO.IOException ex)
            {
                Console.WriteLine("Error de lectura en el MemoryStream");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error de lectura al cargar la imagen"+ex.Message);
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
            ConfiguracionLobby configuracionLobby = new ConfiguracionLobby(CuentaUsuario);
            this.NavigationService.Navigate(configuracionLobby);
        }
    }
}
