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
    /// Lógica de interacción para JuegoLanzamientoPagina.xaml
    /// </summary>
    public partial class JuegoLanzamientoPagina : Page
    {
        private Dictionary<Direccion, TextBlock> _etiquetasJugadoresLanzamientoPotencia;
        private Dictionary<Direccion, Button> _botonesLanzarPeonJugador;
        private Dictionary<Direccion, Button> _botonesTirarDadoJugador;
        private TableroCuatroJugadores _tablero;
        public JuegoLanzamientoPagina(List<CuentaSet> listaCuentas)
        {
            InitializeComponent();
            ColocarNombres(listaCuentas);
            
            _etiquetasJugadoresLanzamientoPotencia = new Dictionary<Direccion, TextBlock>
            {
                {Direccion.Arriba, txtBlockPotenciaRojo },
                {Direccion.Abajo, txtBlockPotenciaAzul },
                {Direccion.Izquierda, txtBlockPotenciaAmarillo } ,
                {Direccion.Derecha, txtBlockPotenciaVerde }
            };
            _botonesLanzarPeonJugador = new Dictionary<Direccion, Button>
            {
                { Direccion.Abajo, btnLanzarPeonAzul },
                { Direccion.Arriba, btnLanzarPeonRojo },
                { Direccion.Izquierda, btnLanzarPeonAmarillo },
                { Direccion.Derecha, btnLanzarPeonVerde }
            };
            _botonesTirarDadoJugador = new Dictionary<Direccion, Button>
            {
                { Direccion.Abajo, btnTirarDadoAzul },
                { Direccion.Arriba, btnTirarDadoRojo },
                { Direccion.Izquierda, btnTirarDadoAmarillo },
                { Direccion.Derecha, btnTirarDadoVerde }
            };

            _tablero = new TableroCuatroJugadores(listaCuentas, RegresarObstaculos());
            _tablero.IniciarJuego += IniciarTurno;
            _tablero.MostrarPotenciaLanzamiento += MostrarPotenciaLanzamiento;
            _tablero.TerminarTurno += TerminarTurno;

            ColocarPiezasJugadores();
            _tablero.IniciarTurno();
        }
        private void ColocarNombres(List<CuentaSet> jugadores)
        {
            lblJugadorAzul.Content = jugadores[0].Nickname;
            lblJugadorVerde.Content = jugadores[1].Nickname;
            lblJugadorRojo.Content = jugadores[2].Nickname;
            lblJugadorAmarillo.Content = jugadores[3].Nickname;
        }
        private List<Rectangle> RegresarObstaculos()
        {
            return new List<Rectangle>
            {
                rctParedAmarillaAbajo, rctParedAmarilloArriba, rctParedAzulDerecha, rctParedAzulIzquierda,
                rctParedRojaDerecha, rctParedRojaIzquierda, rctParedVerdeAbajo, rctParedVerdeArriba
            };
        }
        private void ClickDetenerDado(object sender, RoutedEventArgs e)
        {
            _tablero.DetenerDado();
        }
        private void ClickLanzarPeon(object sender, RoutedEventArgs e)
        {
            _tablero.LanzarPeonActual();
            _botonesLanzarPeonJugador[_tablero.ListaJugadores[_tablero.TurnoActual].DireccionJugador].IsEnabled = false;
        }
        private void IniciarTurno()
        {
            JugadorLanzamiento jugadorTurnoActual = _tablero.ListaJugadores[_tablero.TurnoActual];
            PeonLanzamiento peonTurnoActual = jugadorTurnoActual.PeonesLanzamiento[jugadorTurnoActual.PeonTurnoActual];
            Canvas.SetTop(peonTurnoActual.Figura, peonTurnoActual.PosicionPeon.Y);
            Canvas.SetLeft(peonTurnoActual.Figura, peonTurnoActual.PosicionPeon.X);

            LineaMovimiento linea = jugadorTurnoActual.LineaMovimiento;

            cnvEspacioJuego.Children.Add(linea.FiguraLinea);
            Canvas.SetTop(linea.FiguraLinea, linea.PosicionCanva.Y);
            Canvas.SetLeft(linea.FiguraLinea, linea.PosicionCanva.X);

            _botonesTirarDadoJugador[jugadorTurnoActual.DireccionJugador].IsEnabled = true;

            lblTurnoJugador.Content = "TURNO DEL JUGADOR: " + jugadorTurnoActual.Nickname;
        }
        private void MostrarPotenciaLanzamiento(int potencia, int potenciaAgregada)
        {
            JugadorLanzamiento jugadorTurno = _tablero.ListaJugadores[_tablero.TurnoActual];
            string potenciaLanzamiento = "Potencia de Lanzamiento: " + potencia + " + " + potenciaAgregada;
            _etiquetasJugadoresLanzamientoPotencia[jugadorTurno.DireccionJugador].Text = potenciaLanzamiento;
            _botonesTirarDadoJugador[jugadorTurno.DireccionJugador].IsEnabled = false;
            _botonesLanzarPeonJugador[jugadorTurno.DireccionJugador].IsEnabled = true;
        }
        private void TerminarTurno()
        {
            JugadorLanzamiento jugadorTurnoActual = _tablero.ListaJugadores[_tablero.TurnoActual];
            Line lineaJugador = jugadorTurnoActual.LineaMovimiento.FiguraLinea;
            if (cnvEspacioJuego.Children.Contains(lineaJugador))
            {
                cnvEspacioJuego.Children.Remove(lineaJugador);
            }
            _etiquetasJugadoresLanzamientoPotencia[jugadorTurnoActual.DireccionJugador].Text = "Potencia de Lanzamiento: -";
        }
        private void ColocarPiezasJugadores()
        {
            foreach (JugadorLanzamiento jugador in _tablero.ListaJugadores)
            {
                foreach (PeonLanzamiento peonJugador in jugador.PeonesLanzamiento)
                {
                    cnvEspacioJuego.Children.Add(peonJugador.Figura);
                    Canvas.SetTop(peonJugador.Figura, peonJugador.PosicionPeon.Y);
                    Canvas.SetLeft(peonJugador.Figura, peonJugador.PosicionPeon.X);
                }
                foreach (DadoPotencia dadoJugador in jugador.DadosJugador)
                {
                    cnvEspacioJuego.Children.Add(dadoJugador.ImagenDado);
                    Canvas.SetTop(dadoJugador.ImagenDado, dadoJugador.PosicionCanva.Y);
                    Canvas.SetLeft(dadoJugador.ImagenDado, dadoJugador.PosicionCanva.X);
                }
            }
        }
    }
}
