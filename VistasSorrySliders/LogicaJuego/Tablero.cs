using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows;

namespace VistasSorrySliders.LogicaJuego
{
    public abstract class Tablero
    {
        public int NumeroJugadores;
        public int TurnoActual;

        public const int ESPACIO_COLOCAR_FICHAS = 40;
        public const int NUMERO_PEONES_POR_JUGADOR = 4;
        public const int TAMANO_PEON = 12;
        public const int LONGITUD_LINEA = 40;
        public const int NO_INTERVALOS = 5;
        public const int TAMANO_DADO = 73;
        public const int DADOS_POR_JUGADOR = 2;

        public List<JugadorLanzamiento> ListaJugadores;
        public List<PeonLanzamiento> ListaPeonesTablero;
        public List<Rectangle> ListaObstaculos;
        public Dictionary<Direccion, ImageBrush> ColorPorJugador;
        public Dictionary<Direccion, Point> PosicionInicioJugadores;
        public Dictionary<Direccion, Point> PosicionLanzamientoInicial;
        public Dictionary<Direccion, (Point, Point)> PosicionDados;

        protected DispatcherTimer _temporizadorDado;
        protected DispatcherTimer _temporizadorLinea;
        protected DispatcherTimer _temporizadorPeonesMovimiento;

        public event Action IniciarJuego;
        public event Action<int, int> MostrarPotenciaLanzamiento;
        public event Action TerminarTurno;


        public Tablero()
        {
            ListaPeonesTablero = new List<PeonLanzamiento>();

            _temporizadorDado = new DispatcherTimer();
            _temporizadorDado.Tick += IniciarMovimientoDados;
            _temporizadorDado.Interval = TimeSpan.FromMilliseconds(100);

            _temporizadorLinea = new DispatcherTimer();
            _temporizadorLinea.Tick += IniciarMovimientoLinea;
            _temporizadorLinea.Interval = TimeSpan.FromMilliseconds(100);

            _temporizadorPeonesMovimiento = new DispatcherTimer();
            _temporizadorPeonesMovimiento.Tick += IniciarMovimientoPeones;
            _temporizadorPeonesMovimiento.Interval = TimeSpan.FromMilliseconds(20);
        }

        private void IniciarMovimientoDados(object sender, EventArgs e)
        {
            ListaJugadores[TurnoActual].CambiarDados();
        }
        private void IniciarMovimientoLinea(object sender, EventArgs e)
        {
            ListaJugadores[TurnoActual].LineaMovimiento.SiguientePosicion();
        }
        private void IniciarMovimientoPeones(object sender, EventArgs e)
        {
            int piezasEnMovimiento = 0;
            foreach (PeonLanzamiento peonTablero in ListaPeonesTablero)
            {
                if (peonTablero.VelocidadHorizontal > 0 || peonTablero.VelocidadVertical > 0)
                {
                    piezasEnMovimiento++;
                    peonTablero.RealizarMovimiento(ListaObstaculos, ListaPeonesTablero);
                }
            }
            if (piezasEnMovimiento == 0)
            {
                SiguienteTurno();
            }
        }
        private void SiguienteTurno()
        {
            _temporizadorPeonesMovimiento.Stop();

            TerminarTurno?.Invoke();
            ListaJugadores[TurnoActual].TerminarTurno();

            TurnoActual = (TurnoActual + 1 >= NumeroJugadores) ? 0 : TurnoActual + 1;

            if (ListaJugadores[TurnoActual].PeonTurnoActual < NUMERO_PEONES_POR_JUGADOR)
            {
                IniciarTurno();
            }
            else
            {
                //ACABAR RONDA PARTE DE JACOB
            }
        }

        public void IniciarTurno()
        {
            JugadorLanzamiento jugadorTurnoActual = ListaJugadores[TurnoActual];
            PeonLanzamiento peonTurnoActual = jugadorTurnoActual.PeonesLanzamiento[jugadorTurnoActual.PeonTurnoActual];
            int posicionX = Convert.ToInt32(PosicionLanzamientoInicial[jugadorTurnoActual.DireccionJugador].X);
            int posicionY = Convert.ToInt32(PosicionLanzamientoInicial[jugadorTurnoActual.DireccionJugador].Y);

            peonTurnoActual.CambiarPosicionPeon(posicionX, posicionY);
            ListaPeonesTablero.Add(peonTurnoActual);

            IniciarJuego?.Invoke();
            _temporizadorDado.Start();

        }
        public void DetenerDado()
        {
            JugadorLanzamiento jugadorTurno = ListaJugadores[TurnoActual];
            if (jugadorTurno.NumeroDadosLanzados < DADOS_POR_JUGADOR)
            {
                jugadorTurno.AumentarDadosLanzados();
            }

            if (jugadorTurno.NumeroDadosLanzados >= DADOS_POR_JUGADOR)
            {
                (int potencia, int potenciaAgregada) = jugadorTurno.RegresarPotenciaDados();
                MostrarPotenciaLanzamiento?.Invoke(potencia, potenciaAgregada);
                _temporizadorDado.Stop();
                _temporizadorLinea.Start();
            }
        }
        public void LanzarPeonActual()
        {
            _temporizadorLinea.Stop();

            ListaJugadores[TurnoActual].IniciarMovimientoPeon();

            _temporizadorPeonesMovimiento.Start();
        }

    }
}
