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
        }

        private void ClickIniciarJuego(object sender, RoutedEventArgs e)
        {
            /*MenuPrincipalPagina pagina = new MenuPrincipalPagina("trescuatro1234@gmail.com");
            this.NavigationService.Navigate(pagina);*/
            InicioSesionPagina inicioSesion = new InicioSesionPagina();
            this.NavigationService.Navigate(inicioSesion);
            /*JuegoYLobbyVentana juego = new JuegoYLobbyVentana();
            juego.Show();*/
        }
    }
}
