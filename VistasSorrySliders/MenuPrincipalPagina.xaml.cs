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
            Constantes resultado = Constantes.OPERACION_EXITOSA;
            try
            {
                /*
                RecuperarDatosUsuarioClient proxyRegistrarUsuario = new RecuperarDatosUsuarioClient();
                resultado = proxyRegistrarUsuario.AgregarUsuario(correoUsuario);
                proxyRegistrarUsuario.Close();
                switch (resultado)
                {
                    case Constantes.OPERACION_EXITOSA:
                            
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
                */
        }

        private void ClickMostrarAjustes(object sender, RoutedEventArgs e)
        {
            AjustesVentana ajustes = new AjustesVentana();
            ajustes.Show();
        }

        private void ClickSalirMenuPrincipal(object sender, RoutedEventArgs e)
        {
            this.NavigationService.GoBack();
        }
    }
}
