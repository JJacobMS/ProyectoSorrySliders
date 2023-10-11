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

        private void ClicIniciarJuego(object sender, RoutedEventArgs e)
        {
            //AjustesVentana ajustes = new AjustesVentana();
            //ajustes.Show();
            RegistroUsuariosPagina usu = new RegistroUsuariosPagina();
            MenuPrincipalPagina menuPrincipal = new MenuPrincipalPagina("correo");
            this.NavigationService.Navigate(usu);
        }
    }
}
