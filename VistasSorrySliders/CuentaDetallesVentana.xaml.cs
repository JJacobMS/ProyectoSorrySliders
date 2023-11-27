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
    /// Lógica de interacción para CuentaDetallesVentana.xaml
    /// </summary>
    public partial class CuentaDetallesVentana : Window
    {
        public event Action<UsuarioSet> ModificarUsuarioCuenta; 
        public event Action ModificarContrasena;
        public event Action<Window> AbrirVentana;
        private CuentaSet _cuenta;
        private UsuarioSet _usuario;
        public CuentaDetallesVentana(CuentaSet cuenta)
        {
            InitializeComponent();
            _cuenta = cuenta;
            RecuperarDatos();
        }

        public void MostrarVentana()
        {
            RecuperarDatos();
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
            Logger log = new Logger(this.GetType());
            try
            {
                DetallesCuentaUsuarioClient proxyUsuario = new DetallesCuentaUsuarioClient();
                (resultado, _usuario) = proxyUsuario.RecuperarDatosUsuarioDeCuenta(_cuenta.CorreoElectronico);
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
            catch (Exception ex)
            {
                resultado = Constantes.ERROR_CONEXION_SERVIDOR;
                log.LogFatal("Ha ocurrido un error inesperado", ex);
            }
            switch (resultado)
            {
                case Constantes.OPERACION_EXITOSA:
                    ColocarDatos();
                    AbrirVentana?.Invoke(this);
                    break;
                case Constantes.OPERACION_EXITOSA_VACIA:
                    MessageBox.Show(Properties.Resources.mgsCuentaDetalleErrorRecuperar);
                    break;
                default:
                    Utilidades.MostrarMensajesError(resultado);
                    break;
            }
        }

        private void ClickSalir(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ClickIrRegistroUsuarios(object sender, RoutedEventArgs e)
        {
            ModificarUsuarioCuenta?.Invoke(_usuario);
            this.Close();
        }

        private void ClickModificarContrasena(object sender, RoutedEventArgs e)
        {
            ModificarContrasena?.Invoke();
            this.Close();
        }
    }
}
