using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using VistasSorrySliders.LogicaJuego;
using VistasSorrySliders.ServicioSorrySliders;

namespace VistasSorrySliders
{
    /// <summary>
    /// Lógica de interacción para TableroGanadoresPartidaPagina.xaml
    /// </summary>
    public partial class TableroGanadoresPartidaPagina : Page
    {
        private CuentaSet _cuenta;
        public ObservableCollection<JugadorGanador> ListaGanadores { get; set; }
        public TableroGanadoresPartidaPagina(CuentaSet cuenta, List<JugadorGanador> listaGanadores)
        {
            _cuenta = cuenta;
            InitializeComponent();
            AgregarGanadores(listaGanadores);
        }
        private void AgregarGanadores(List<JugadorGanador> listaGanadores) 
        {
            ListaGanadores = new ObservableCollection<JugadorGanador>();
            foreach (var ganador in listaGanadores)
            {
                ListaGanadores.Add(ganador);
            }
            this.DataContext = this;
        }

        private void ClickSalirMenuPrincipal(object sender, RoutedEventArgs e)
        {
            MenuPrincipalPagina menu = new MenuPrincipalPagina(_cuenta);
            this.NavigationService.Navigate(menu);
        }
    }
}
