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
using VistasSorrySliders.ServicioInicioSesion;

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
                string correoVerificado = "";
                int resultado = 0;
                try
                {
                    ServicioInicioSesion.InicioSesionClient proxyInicioSesion = new ServicioInicioSesion.InicioSesionClient();
                    (correoVerificado, resultado) = proxyInicioSesion.VerificarExistenciaCorreoCuenta(correoIngresado);
                    proxyInicioSesion.Close();
                }
                catch (CommunicationException excepcion) 
                {
                    MessageBox.Show(Properties.Resources.msgErrorConexion);
                    Console.WriteLine(excepcion);
                }

                switch (resultado)
                {
                    //case 0:
                }

                
                if (correoVerificado == null)
                {
                    MessageBox.Show(Properties.Resources.msgErrorBaseDatos);
                    return;
                }
                if (correoVerificado == "")
                {
                    txtBlockCorreoInvalido.Visibility = Visibility.Visible;
                }
                else
                {
                    CuentaSet cuentaVerificada = new CuentaSet { CorreoElectronico = correoVerificado, Contraseña = contrasena };
                    VerificarContrasena(cuentaVerificada);
                }
            }

        }

        private void VerificarContrasena(CuentaSet cuentaPorVerificar)
        {
            CuentaSet cuentaVerificada = new CuentaSet();
            int resultado;
            try
            {
                ServicioInicioSesion.InicioSesionClient proxyInicioSesion = new ServicioInicioSesion.InicioSesionClient();
                (cuentaVerificada, resultado) = proxyInicioSesion.VerificarContrasenaDeCuenta(cuentaPorVerificar);
                proxyInicioSesion.Close();
            }
            catch (CommunicationException excepcion)    
            {
                MessageBox.Show(Properties.Resources.msgErrorConexion);
                Console.WriteLine(excepcion);
            }

            if (cuentaVerificada == null)
            {
                MessageBox.Show(Properties.Resources.msgErrorBaseDatos);
                return;
            }
            if (cuentaVerificada.CorreoElectronico == null)
            {
                
                txtBlockContrasenaInvalida.Visibility = Visibility.Visible;
            }
            else
            {
                MessageBox.Show("ENTRAR AL SISTEMA");
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
