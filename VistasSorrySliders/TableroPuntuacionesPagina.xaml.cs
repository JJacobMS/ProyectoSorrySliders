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
            Constantes resultado;
            Puntuacion[] puntuaciones = new Puntuacion[] { };
            Logger log = new Logger(this.GetType());
            try
            {
                PuntuacionClient proxyRecuperarPuntuaciones = new PuntuacionClient();
                (resultado, puntuaciones) = proxyRecuperarPuntuaciones.RecuperarPuntuaciones();
                proxyRecuperarPuntuaciones.Close();
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
                    ListaPuntuaciones = new ObservableCollection<Puntuacion>();
                    foreach (var puntuacion in puntuaciones)
                    {
                        ListaPuntuaciones.Add(puntuacion);
                    }
                    this.DataContext = this;
                    return;
                case Constantes.OPERACION_EXITOSA_VACIA:
                    MessageBox.Show(Properties.Resources.msgTablaVacia);
                    break;
                case Constantes.ERROR_CONEXION_BD:
                    MessageBox.Show(Properties.Resources.msgErrorBaseDatos);
                    break;
                case Constantes.ERROR_CONSULTA:
                    MessageBox.Show(Properties.Resources.msgErrorConsulta);
                    break;
                case Constantes.ERROR_CONEXION_SERVIDOR:
                    MessageBox.Show(Properties.Resources.msgErrorConexion);
                    break;
                case Constantes.ERROR_TIEMPO_ESPERA_SERVIDOR:
                    MessageBox.Show(Properties.Resources.msgErrorTiempoEsperaServidor);
                    break;
            }
            this.NavigationService.GoBack();
        }

        private void ClickSalirMenuPrincipal(object sender, RoutedEventArgs e)
        {
            MenuPrincipalPagina menu = new MenuPrincipalPagina(_cuenta);
            this.NavigationService.Navigate(menu);
        }
    }
}
