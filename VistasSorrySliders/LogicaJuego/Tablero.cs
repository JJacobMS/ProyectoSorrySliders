using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows;
using System.Windows.Controls;

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
        public const int ANCHO_TABLERO = 615;
        public const int ALTO_TABLERO = 615;

        public List<JugadorLanzamiento> ListaJugadores;
        public List<PeonLanzamiento> ListaPeonesTablero;
        public List<Rectangle> ListaObstaculos;
        public List<Rectangle> ListaLugaresNoValidos;
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
        public event Action<PeonLanzamiento> EliminarPeonTablero;
        public event Action FinalizarMovimientoPeones;

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
            _temporizadorPeonesMovimiento.Interval = TimeSpan.FromMilliseconds(50);
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
                FinalizarTurno();
            }
        }
        private void ComprobarLugarValidoFinal()
        {
            List<PeonLanzamiento> listaPeonesAEliminar = new List<PeonLanzamiento>();
            foreach (PeonLanzamiento peon in ListaPeonesTablero)
            {
                if (!peon.EstaLugarValido(ListaLugaresNoValidos))
                {
                    listaPeonesAEliminar.Add(peon);
                }
            }
            foreach (PeonLanzamiento peonAEliminar in listaPeonesAEliminar)
            {
                ListaPeonesTablero.Remove(peonAEliminar);
                EliminarPeonTablero?.Invoke(peonAEliminar);
            }
        }
        private void FinalizarTurno()
        {
            _temporizadorPeonesMovimiento.Stop();
            Task.Delay(500).Wait();
            FinalizarMovimientoPeones?.Invoke();            
        }

        public void CambiarTurnoSiguiente()
        {
            ComprobarLugarValidoFinal();
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
        public int RetornarDado()
        {
            JugadorLanzamiento jugadorTurno = ListaJugadores[TurnoActual];
            if (jugadorTurno.NumeroDadosLanzados < DADOS_POR_JUGADOR)
            {
                return jugadorTurno.DadosJugador[jugadorTurno.NumeroDadosLanzados].NumeroDado;
            }
            return -1;
        }
        public void DetenerDadoPosicion(int numeroDado)
        {
            JugadorLanzamiento jugadorTurno = ListaJugadores[TurnoActual];
            switch (jugadorTurno.NumeroDadosLanzados)
            {
                case 0:
                    jugadorTurno.AumentarDadosLanzados();
                    jugadorTurno.DadosJugador[0].AsignarPosicionDado(numeroDado);
                    break;
                case 1:
                    jugadorTurno.AumentarDadosLanzados();
                    jugadorTurno.DadosJugador[1].AsignarPosicionDado(numeroDado);

                    (int potencia, int potenciaAgregada) = jugadorTurno.RegresarPotenciaDados();
                    MostrarPotenciaLanzamiento?.Invoke(potencia, potenciaAgregada);
                    _temporizadorDado.Stop();
                    _temporizadorLinea.Start();
                    break;
            }
        }

        public void LanzarPeonActual()
        {
            _temporizadorLinea.Stop();
            ListaJugadores[TurnoActual].IniciarMovimientoPeon();
            _temporizadorPeonesMovimiento.Start();
        }

        public (double, double) RecuperarPosicionLineaJugadorActual()
        {
            JugadorLanzamiento jugadorTurno = ListaJugadores[TurnoActual];
            if (jugadorTurno.LineaMovimiento != null)
            {
                LineaMovimiento linea = jugadorTurno.LineaMovimiento;
                return (linea.FiguraLinea.X2, linea.FiguraLinea.Y2);
            }
            return (-1, -1);
        }
        public void DetenerLineaMovimiento(double posicionX, double posicionY)
        {
            JugadorLanzamiento jugadorTurno = ListaJugadores[TurnoActual];
            jugadorTurno.LineaMovimiento.FiguraLinea.X2 = posicionX;
            jugadorTurno.LineaMovimiento.FiguraLinea.Y2 = posicionY;
            LanzarPeonActual();
        }

    }
}
