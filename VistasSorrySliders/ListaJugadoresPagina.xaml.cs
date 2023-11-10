using System;
using System.Collections;
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
using VistasSorrySliders.ServicioSorrySliders;

namespace VistasSorrySliders
{
    /// <summary>
    /// Lógica de interacción para ListaJugadoresPagina.xaml
    /// </summary>
    public partial class ListaJugadoresPagina : Page
    {
        private List<CuentaSet> _baneados;
        private List<CuentaSet> _amigos;
        private List<CuentaSet> _lista;
        public ListaJugadoresPagina()
        {
            InitializeComponent();
            _lista = new List<CuentaSet> {new CuentaSet { Nickname = "jazmin", CorreoElectronico= "jazmin123@gmail.com"}, 
                new CuentaSet {Nickname= "sulem", CorreoElectronico = "sulem477@gmail.com"},
                new CuentaSet { Nickname = "Jacobo", CorreoElectronico= "jacob123@gmail.com"}, 
                new CuentaSet { Nickname = "melus", CorreoElectronico = "waos@gmail.com"}
            };

            _baneados = new List<CuentaSet> { 
                new CuentaSet { Nickname = "sulem", CorreoElectronico= "sulem477@gmail.com"},
                new CuentaSet { Nickname = "Jacobo", CorreoElectronico = "jacob123@gmail.com"}
            };

            _amigos = new List<CuentaSet> {
                new CuentaSet { Nickname = "melus", CorreoElectronico = "waos@gmail.com"}
            };

            CargarJugadores();
            CargarAmigos();
            CargarNotificaciones();
            
        }

        private void CargarJugadores()
        {
            foreach (CuentaSet cuenta in _lista)
            {
                ListBoxItem lstBoxItemCuenta = new ListBoxItem
                {
                    DataContext = cuenta
                };
                if (EsCuentaBaneada(cuenta))
                {
                    lstBoxItemCuenta.Style = (Style)FindResource("estiloLstBoxItemJugadorBaneado");
                }
                if (EsCuentaAmigo(cuenta))
                {
                    lstBoxItemCuenta.Style = (Style)FindResource("estiloLstBoxItemJugadorAmigo");
                }
                lstBoxJugadores.Items.Add(lstBoxItemCuenta);
            }
        }

        private void CargarNotificaciones()
        {

        }

        private void CargarAmigos()
        {
            lstBoxAmigos.Items.Clear();
            lstBoxAmigos.ItemsSource = _amigos;
        }

        private bool EsCuentaBaneada(CuentaSet cuentaComparar)
        {
            foreach (CuentaSet cuenta in _baneados)
            {
                if (cuenta.CorreoElectronico.Equals(cuentaComparar.CorreoElectronico))
                {
                    return true;
                }
            }
            return false;
        }

        private bool EsCuentaAmigo(CuentaSet cuentaComparar)
        {
            foreach (CuentaSet cuenta in _amigos)
            {
                if (cuenta.CorreoElectronico.Equals(cuentaComparar.CorreoElectronico))
                {
                    return true;
                }
            }
            return false;
        }


        private void ClickRegresarMenu(object sender, RoutedEventArgs e)
        {
            this.NavigationService.GoBack();
        }

        private void PreviewMouseDownSolicitudAmistad(object sender, MouseButtonEventArgs e)
        {
            Button boton = sender as Button;
            CuentaSet cuenta = boton.CommandParameter as CuentaSet;
            Console.WriteLine(cuenta.Nickname);
            Console.WriteLine("Solicitud Amistad");

        }

        private void PreviewMouseDownBanearJugador(object sender, MouseButtonEventArgs e)
        {
            Button boton = sender as Button;
            CuentaSet cuenta = boton.CommandParameter as CuentaSet;
            Console.WriteLine(cuenta.Nickname);
            Console.WriteLine("Banear Jug");
        }

        private void PreviewMouseDownEliminarBaneo(object sender, MouseButtonEventArgs e)
        {
            Button boton = sender as Button;
            CuentaSet cuenta = boton.CommandParameter as CuentaSet;
            Console.WriteLine(cuenta.Nickname);
            Console.WriteLine("Eliminar Banear");
        }

        private void PreviewMouseDownBanearAmigo(object sender, MouseButtonEventArgs e)
        {
            Button boton = sender as Button;
            CuentaSet cuenta = boton.CommandParameter as CuentaSet;
            Console.WriteLine(cuenta.Nickname);
            Console.WriteLine("Banear Amigo");
        }

        private void PreviewMouseDownEliminarAmigo(object sender, MouseButtonEventArgs e)
        {
            Button boton = sender as Button;
            CuentaSet cuenta = boton.CommandParameter as CuentaSet;
            Console.WriteLine(cuenta.Nickname);
            Console.WriteLine("Eliminar Amigo");
        }

        private void PreviewMouseDownAceptarNotificacion(object sender, MouseButtonEventArgs e)
        {
            Console.WriteLine("Aceptar");
        }

        private void PreviewMouseDownRechazarNotificacion(object sender, MouseButtonEventArgs e)
        {
            Console.WriteLine("Rechazar");
        }

        private void PreviewMouseDownEliminarNotificacionPartida(object sender, MouseButtonEventArgs e)
        {
            Console.WriteLine("Notificacion");
        }
    }
}
