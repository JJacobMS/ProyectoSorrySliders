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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;

namespace VistasSorrySliders
{
    /// <summary>
    /// Lógica de interacción para ListaAmigosPagina.xaml
    /// </summary>
    public partial class ListaAmigosPagina : Page
    {
        private CuentaSet _cuenta;
        public ListaAmigosPagina(CuentaSet cuenta)
        {
            InitializeComponent();
            _cuenta = cuenta;
            RecuperarAmigos();
        }


        private void RecuperarAmigos()
        {
            Constantes resultado;
            List<CuentaSet> amigosLista = new List<CuentaSet>();
            try
            {
                CuentaSet[] cuentas;
                ListaAmigosClient proxyAmigos = new ListaAmigosClient();
                (resultado, cuentas) = proxyAmigos.RecuperarAmigosCuenta(_cuenta.CorreoElectronico);
                if (cuentas != null)
                {
                    amigosLista = cuentas.ToList();
                }
            }
            catch (CommunicationException excepcion)
            {
                resultado = Constantes.ERROR_CONEXION_SERVIDOR;
                Console.WriteLine(excepcion);
            }

            switch (resultado)
            {
                case Constantes.OPERACION_EXITOSA:
                    lstBoxAmigos.ItemsSource = amigosLista;
                    break;
                case Constantes.OPERACION_EXITOSA_VACIA:
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
        private void ClickEnviarCodigo(object sender, RoutedEventArgs e)
        {
            RecuperarCuentaListItem((ListBoxItem)sender);
        }

        private CuentaSet RecuperarCuentaListItem(ListBoxItem itemJugador)
        {
            var bordeEstilo = VisualTreeHelper.GetChild(itemJugador, 0);
            var gridEstilo = VisualTreeHelper.GetChild(bordeEstilo, 0);
            var botonDelItem = VisualTreeHelper.GetChild(gridEstilo, 3);

            Button botonJugador = (Button)botonDelItem;

            return (CuentaSet)botonJugador.CommandParameter;

        }

        private void EnviarInvitacion()
        {

        }

        private void ClicBuscarJugador(object sender, RoutedEventArgs e)
        {
            lstBoxJugadores.ItemsSource = null;
            string informacionJugador = txtBoxBuscadorJugadores.Text;

            if (!string.IsNullOrWhiteSpace(informacionJugador))
            {
                CargarJugadores(informacionJugador);
            }
        }


        private void CargarJugadores(string informacionJugador)
        {
            Console.WriteLine(informacionJugador);
            Constantes resultado;
            List<CuentaSet> jugadoresLista = new List<CuentaSet>();
            try
            {
                CuentaSet[] cuentas;
                ListaAmigosClient proxyAmigos = new ListaAmigosClient();
                (resultado, cuentas) = proxyAmigos.RecuperarJugadoresCuenta(informacionJugador);
                if (cuentas != null)
                {
                    jugadoresLista = cuentas.ToList();
                }
            }
            catch (CommunicationException excepcion)
            {
                resultado = Constantes.ERROR_CONEXION_SERVIDOR;
                Console.WriteLine(excepcion);
            }

            switch (resultado)
            {
                case Constantes.OPERACION_EXITOSA:
                    lstBoxJugadores.ItemsSource = jugadoresLista;
                    break;
                case Constantes.OPERACION_EXITOSA_VACIA:
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
}
