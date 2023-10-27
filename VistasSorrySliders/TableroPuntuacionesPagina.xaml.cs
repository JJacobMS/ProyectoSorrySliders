using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Lógica de interacción para TableroPuntuacionesPagina.xaml
    /// </summary>
    public partial class TableroPuntuacionesPagina : Page
    {
        private CuentaSet _cuenta;
        public ObservableCollection<Puntuacion> ListaPuntuaciones { get; set; }

        public TableroPuntuacionesPagina(CuentaSet cuenta)
        {
            InitializeComponent();
            RecuperarPuntuaciones();
            _cuenta = cuenta;
        }

        private void RecuperarPuntuaciones() 
        {
            Constantes resultado = new Constantes();
            Puntuacion[] puntuaciones = new Puntuacion[] { };
            try
            {
                MenuPrincipalClient proxyRecuperarPuntuaciones = new MenuPrincipalClient();
                (resultado, puntuaciones) = proxyRecuperarPuntuaciones.RecuperarPuntuaciones();
                proxyRecuperarPuntuaciones.Close();
                switch (resultado)
                {
                    case Constantes.OPERACION_EXITOSA:
                        ListaPuntuaciones = new ObservableCollection<Puntuacion>();
                        foreach (var puntuacion in puntuaciones)
                        {
                            ListaPuntuaciones.Add(puntuacion);
                        }
                        this.DataContext = this;
                        break;
                    case Constantes.OPERACION_EXITOSA_VACIA:
                        System.Windows.Forms.MessageBox.Show(Properties.Resources.msgTablaVacia);
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
                    default:
                        break;
                }
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
        }

        private void ClickSalirMenuPrincipal(object sender, RoutedEventArgs e)
        {
            MenuPrincipalPagina menu = new MenuPrincipalPagina(_cuenta);
            this.NavigationService.Navigate(menu);
        }
    }
}
