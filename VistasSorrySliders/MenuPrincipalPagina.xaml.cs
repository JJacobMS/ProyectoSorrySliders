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
    /// Lógica de interacción para MenuPrincipalPagina.xaml
    /// </summary>
    public partial class MenuPrincipalPagina : Page
    {
        public MenuPrincipalPagina(string correoUsuario)
        {
            InitializeComponent();
            //Metodo BuscarUsuarioPorCorreo Puedo reutilizar la consulta de buscar correo, debe regresar Nickname y Avatar
            //Metodo settear datos usuario txtBlockCorreo, txtBlockNickname y imgBrushAvatar
        }



        private void ClickMostrarAjustes(object sender, RoutedEventArgs e)
        {
            AjustesVentana ajustes = new AjustesVentana();
            ajustes.Show();
        }

        private void ClickSalirMenuPrincipal(object sender, RoutedEventArgs e)
        {
            InicioSesionPagina inicio = new InicioSesionPagina();
            this.NavigationService.Navigate(inicio);
        }
    }
}
