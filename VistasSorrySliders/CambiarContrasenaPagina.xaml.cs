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
    /// Lógica de interacción para CambiarContrasenaPagina.xaml
    /// </summary>
    public partial class CambiarContrasenaPagina : Page
    {
        private CuentaSet _cuentaModificar;
        public CambiarContrasenaPagina(CuentaSet cuenta)
        {
            InitializeComponent();
            _cuentaModificar = cuenta;
        }

        private void ClickGuardarContrasena(object sender, RoutedEventArgs e)
        {
            ChecarContrasenaActual();
        }

        private void ClickCancelar(object sender, RoutedEventArgs e)
        {
            this.NavigationService.GoBack();
        }

        private void ChecarContrasenaActual()
        {
            ReiniciarElementos();
            string contrasenaActual = pssBoxContrasenaAnterior.Password;

            if (!string.IsNullOrWhiteSpace(contrasenaActual))
            {
                CuentaSet cuentaPorVerificar = new CuentaSet
                {
                    CorreoElectronico = _cuentaModificar.CorreoElectronico,
                    Contraseña = contrasenaActual
                };
                VerificarContrasenaAnterior(cuentaPorVerificar);
            }
            else
            {
                pssBoxContrasenaAnterior.Style = (Style)FindResource("estiloPssBoxContrasenaRojo");
            }
        }

        private void VerificarContrasenaAnterior(CuentaSet cuentaPorVerificar)
        {
            Constantes resultado;
            try
            {
                DetallesCuentaUsuarioClient proxyUsuario = new DetallesCuentaUsuarioClient();
                resultado = proxyUsuario.VerificarContrasenaActual(cuentaPorVerificar);
            }
            catch (CommunicationException ex)
            {
                Console.WriteLine(ex);
                resultado = Constantes.ERROR_CONEXION_SERVIDOR;
            }
            catch (TimeoutException ex)
            {
                Console.WriteLine(ex);
                resultado = Constantes.ERROR_CONEXION_SERVIDOR;
            }

            switch (resultado)
            {
                case Constantes.OPERACION_EXITOSA:
                    VerificarContrasenaNueva();
                    break;
                case Constantes.OPERACION_EXITOSA_VACIA:
                    txtBlockContrasenaDiferenteActual.Visibility = Visibility.Visible;
                    break;
                case Constantes.ERROR_CONEXION_BD:
                    MessageBox.Show(Properties.Resources.msgErrorBaseDatos);
                    break;
                case Constantes.ERROR_CONSULTA:
                    MessageBox.Show(Properties.Resources.msgErrorBaseDatos);
                    break;
                case Constantes.ERROR_CONEXION_SERVIDOR:
                    MessageBox.Show(Properties.Resources.msgErrorConexion);
                    break;
            }
        }

        private void VerificarContrasenaNueva()
        {
            bool esValido = true;
            string contrasenaNueva = pssBoxContrasenaNueva.Password;
            string constrasenaRepetida = pssBoxContrasenaRepetida.Password;

            if (string.IsNullOrWhiteSpace(contrasenaNueva))
            {
                esValido = false;
                pssBoxContrasenaNueva.Style = (Style)FindResource("estiloPssBoxContrasenaRojo");
            }

            if (string.IsNullOrWhiteSpace(constrasenaRepetida))
            {
                esValido = false;
                pssBoxContrasenaRepetida.Style = (Style)FindResource("estiloPssBoxContrasenaRojo");
            }

            if (esValido)
            {
                if (contrasenaNueva.Equals(pssBoxContrasenaAnterior.Password))
                {
                    pssBoxContrasenaNueva.Style = (Style)FindResource("estiloPssBoxContrasenaRojo");
                    MessageBox.Show("La contraseña no puede ser la misma que la anterior");
                    return;
                }

                if (!Utilidades.ValidarContraseña(contrasenaNueva))
                {
                    pssBoxContrasenaNueva.Style = (Style)FindResource("estiloPssBoxContrasenaRojo");
                    txtBlockContrasenaNoCumpleNormas.Visibility = Visibility.Visible;
                    return;
                }

                if (!contrasenaNueva.Equals(constrasenaRepetida))
                {
                    pssBoxContrasenaRepetida.Style = (Style)FindResource("estiloPssBoxContrasenaRojo");
                    txtBlockContrasenaDiferenteNueva.Visibility = Visibility.Visible;
                    return;
                }

                CuentaSet cuenta = new CuentaSet
                {
                    Contraseña = pssBoxContrasenaNueva.Password,
                    CorreoElectronico = _cuentaModificar.CorreoElectronico
                };
                CambiarContrasena(cuenta);

            }
        }

        private void CambiarContrasena(CuentaSet cuentaCambiarContrasena)
        {
            Constantes resultado;
            try
            {
                DetallesCuentaUsuarioClient proxyUsuario = new DetallesCuentaUsuarioClient();
                resultado = proxyUsuario.CambiarContrasena(cuentaCambiarContrasena);
            }
            catch (CommunicationException ex)
            {
                Console.WriteLine(ex);
                resultado = Constantes.ERROR_CONEXION_SERVIDOR;
            }
            catch (TimeoutException ex)
            {
                Console.WriteLine(ex);
                resultado = Constantes.ERROR_CONEXION_SERVIDOR;
            }

            switch (resultado)
            {
                case Constantes.OPERACION_EXITOSA:
                    MessageBox.Show("Cambio Contraseña Exitoso");
                    this.NavigationService.GoBack();
                    break;
                case Constantes.OPERACION_EXITOSA_VACIA:
                    txtBlockContrasenaDiferenteActual.Visibility = Visibility.Visible;
                    break;
                case Constantes.ERROR_CONEXION_BD:
                    MessageBox.Show(Properties.Resources.msgErrorBaseDatos);
                    break;
                case Constantes.ERROR_CONSULTA:
                    MessageBox.Show(Properties.Resources.msgErrorBaseDatos);
                    break;
                case Constantes.ERROR_CONEXION_SERVIDOR:
                    MessageBox.Show(Properties.Resources.msgErrorConexion);
                    break;
            }
        }

        private void ReiniciarElementos()
        {
            txtBlockContrasenaDiferenteActual.Visibility = Visibility.Collapsed;
            txtBlockContrasenaDiferenteNueva.Visibility = Visibility.Collapsed;
            txtBlockContrasenaNoCumpleNormas.Visibility = Visibility.Collapsed;
            pssBoxContrasenaAnterior.Style = (Style)FindResource("estiloPssBoxContrasenaAzul");
            pssBoxContrasenaNueva.Style = (Style)FindResource("estiloPssBoxContrasenaAzul");
            pssBoxContrasenaRepetida.Style = (Style)FindResource("estiloPssBoxContrasenaAzul");
        }
    }
}
