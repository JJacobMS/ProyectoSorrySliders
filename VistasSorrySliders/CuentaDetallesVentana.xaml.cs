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
using System.Windows.Shapes;
using VistasSorrySliders.ServicioSorrySliders;

namespace VistasSorrySliders
{
    /// <summary>
    /// Lógica de interacción para CuentaDetallesVentana.xaml
    /// </summary>
    public partial class CuentaDetallesVentana : Window
    {
        private CuentaSet _cuenta;
        private UsuarioSet _usuario;
        public CuentaDetallesVentana(CuentaSet cuenta)
        {
            InitializeComponent();
            _cuenta = cuenta;
            RecuperarDatos();
            ColocarDatos();

        }

        private void ColocarDatos()
        {
            txtBoxNombre.Text = _usuario.Nombre;
            txtBoxApellidos.Text = _usuario.Apellido;
            txtBoxCorreo.Text = _cuenta.CorreoElectronico;
            txtBoxNickname.Text = _cuenta.Nickname;
            llpAvatar.Fill = Utilidades.ConvertirBytesAImageBrush(_cuenta.Avatar);
        }
        
        private void RecuperarDatos()
        {
            Constantes resultado;
            try
            {
                DetallesCuentaUsuarioClient proxyUsuario = new DetallesCuentaUsuarioClient();
                (resultado, _usuario) = proxyUsuario.RecuperarDatosUsuarioDeCuenta(_cuenta.CorreoElectronico);
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
                    ColocarDatos();
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

        private void ClickSalir(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
