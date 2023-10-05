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
using VistasSorrySliders.DAO;
using VistasSorrySliders.ServicioSorrySliders;

namespace VistasSorrySliders
{
    /// <summary>
    /// Lógica de interacción para InicioSesionPagina.xaml
    /// </summary>
    public partial class InicioSesionPagina : Page
    {
        public InicioSesionPagina()
        {
            InitializeComponent();
        }

        private void ClicContinuar(object sender, RoutedEventArgs e)
        {
            VerificarCuenta();
        }

        private void ClicCancelar(object sender, RoutedEventArgs e)
        {
            this.NavigationService.GoBack();
        }

        private void ClicRegistrarCuenta(object sender, RoutedEventArgs e)
        {
            RegistroUsuariosPagina paginaRegistroUsuarios = new RegistroUsuariosPagina();
            this.NavigationService.Navigate(paginaRegistroUsuarios);
        }

        private void VerificarCuenta()
        {
            ReiniciarPantalla();
            Boolean datosCompletos = true;
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
                Constantes resultado = Constantes.OPERACION_EXITOSA;
                try
                {
                    InicioSesionClient proxyInicioSesion = new InicioSesionClient();
                    resultado = proxyInicioSesion.VerificarExistenciaCorreoCuenta(correoIngresado);
                    proxyInicioSesion.Close();
                }
                catch (CommunicationException excepcion) 
                {
                    resultado = Constantes.ERROR_CONEXION_SERVIDOR;
                    Console.WriteLine(excepcion);
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
                    case Constantes.ERROR_CONEXION_BD:
                    case Constantes.ERROR_CONSULTA:
                        MessageBox.Show(Properties.Resources.msgErrorBaseDatos);
                        break;
                    case Constantes.ERROR_CONEXION_SERVIDOR:
                        MessageBox.Show(Properties.Resources.msgErrorConexion);
                        break;
                }
            }

        }

        private void VerificarContrasena(CuentaSet cuentaPorVerificar)
        {
            Constantes resultado = Constantes.OPERACION_EXITOSA;
            try
            {
                InicioSesionClient proxyInicioSesion = new InicioSesionClient();
                resultado = proxyInicioSesion.VerificarContrasenaDeCuenta(cuentaPorVerificar);
                proxyInicioSesion.Close();
            }
            catch (CommunicationException excepcion)    
            {
                resultado = Constantes.ERROR_CONEXION_SERVIDOR;
                Console.WriteLine(excepcion);
            }

            switch (resultado)
            {
                case Constantes.OPERACION_EXITOSA:
                    MessageBox.Show("ENTRAR AL SISTEMA");
                    break;
                case Constantes.OPERACION_EXITOSA_VACIA:
                    txtBlockContrasenaInvalida.Visibility = Visibility.Visible;
                    break;
                case Constantes.ERROR_CONEXION_BD:
                case Constantes.ERROR_CONSULTA:
                    MessageBox.Show(Properties.Resources.msgErrorBaseDatos);
                    break;
                case Constantes.ERROR_CONEXION_SERVIDOR:
                    MessageBox.Show(Properties.Resources.msgErrorConexion);
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
    }
}
