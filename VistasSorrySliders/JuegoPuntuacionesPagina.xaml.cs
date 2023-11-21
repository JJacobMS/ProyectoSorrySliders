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
using VistasSorrySliders.LogicaJuego;
using VistasSorrySliders.ServicioSorrySliders;

namespace VistasSorrySliders
{
    /// <summary>
    /// Lógica de interacción para JuegoPuntuacionesPagina.xaml
    /// </summary>
    public partial class JuegoPuntuacionesPagina : Page
    {
        public JuegoPuntuacionesPagina(List<JugadorLanzamiento> jugadores)
        {
            InitializeComponent();
            Mostrar(jugadores);
        }

        private void Mostrar(List<JugadorLanzamiento> jugadores)
        {
            foreach (JugadorLanzamiento jugador in jugadores)
            {
                Console.WriteLine(jugador + ": ");
                foreach (int puntuacionPeon in jugador.Puntuaciones)
                {
                    Console.Write(puntuacionPeon + " - ");
                }
            }
        }
    }
}
