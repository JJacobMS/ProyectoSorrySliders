using BibliotecaClasesSorrySliders;
using System;
using System.Collections.Generic;
using System.Linq;
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
            string correo = txtBoxCorreo.Text;
            string contrasena = pssBoxContrasena.Password;

            if (string.IsNullOrWhiteSpace(correo))
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
                Cuenta cuentaPorVerificar = new Cuenta { CorreoElectronico = correo };
                CuentaDAO cuentaDAO = new CuentaDAO();
                List<Cuenta> cuentaVerificada = cuentaDAO.VerificarExistenciaCorreoCuenta(cuentaPorVerificar);
                
                if (cuentaVerificada == null)
                {
                    MessageBox.Show("ERROR BD 1");
                    return;
                }
                if (cuentaVerificada.Count < 0)
                {
                    txtBlockCorreoInvalido.Visibility = Visibility.Visible;
                }
                else
                {
                    cuentaVerificada[0].Contrasena = contrasena;
                    VerificarContrasena(cuentaVerificada[0]);
                }
            }

        }

        private void VerificarContrasena(Cuenta cuentaPorVerificar)
        {
            CuentaDAO cuentaDAO = new CuentaDAO();
            List<string> cuentaVerificada = cuentaDAO.VerificarContrasenaDeCuenta(cuentaPorVerificar);

            if (cuentaVerificada == null)
            {
                MessageBox.Show("ERROR BD 2");
                return;
            }
            if (cuentaVerificada.Count < 0)
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
