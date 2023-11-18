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
    /// Lógica de interacción para InicioPagina.xaml
    /// </summary>
    public partial class InicioPagina : Page
    {
        public InicioPagina()
        {
            InitializeComponent();
            Logger log = new Logger(this.GetType());
        }

        private void ClickIniciarJuego(object sender, RoutedEventArgs e)
        {
            InicioSesionPagina inicioSesion = new InicioSesionPagina();
            this.NavigationService.Navigate(inicioSesion);
            
        }
    }
}
