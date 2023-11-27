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
using System.Windows.Media.Imaging;
using VistasSorrySliders.ServicioSorrySliders;

namespace VistasSorrySliders.LogicaJuego
{
    public class Tablero
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
        public Dictionary<Ellipse, int> ListaPuntuaciones;
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
        public event Action AcabarJuegoFaltaJugadores;
        public event Action<List<JugadorLanzamiento>> PasarPuntuacionesJuego;

        public Tablero(List<CuentaSet> listaJugadores, List<Rectangle> obstaculos,
            List<Rectangle> noValidos, Dictionary<Ellipse, int> listaPuntuaciones)
        {
            NumeroJugadores = listaJugadores.Count;
            TurnoActual = 0;
            ListaObstaculos = obstaculos;
            ListaLugaresNoValidos = noValidos;
            ListaPuntuaciones = listaPuntuaciones;
            
            IniciarColoresJugadores();
            IniciarPosicionesInicioPeones();
            IniciarPosicionLanzamiento();
            IniciarPosicionDados();

            AsignarLugaresJugadores(listaJugadores);

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
        private void AsignarLugaresJugadores(List<CuentaSet> listaJugadores)
        {
            switch (NumeroJugadores)
            {
                case 2:
                    ListaJugadores = new List<JugadorLanzamiento>
                    {
                        new JugadorLanzamiento(Direccion.Abajo, this, listaJugadores[0]),
                        new JugadorLanzamiento(Direccion.Arriba, this, listaJugadores[1])
                    };
                    break;
                case 3:
                    ListaJugadores = new List<JugadorLanzamiento>
                    {
                        new JugadorLanzamiento(Direccion.Abajo, this, listaJugadores[0]),
                        new JugadorLanzamiento(Direccion.Derecha, this, listaJugadores[1]),
                        new JugadorLanzamiento(Direccion.Izquierda, this, listaJugadores[2])
                    };
                    break;
                case 4:
                    ListaJugadores = new List<JugadorLanzamiento>
                    {
                        new JugadorLanzamiento(Direccion.Abajo, this, listaJugadores[0]),
                        new JugadorLanzamiento(Direccion.Derecha, this, listaJugadores[1]),
                        new JugadorLanzamiento(Direccion.Arriba, this, listaJugadores[2]),
                        new JugadorLanzamiento(Direccion.Izquierda, this, listaJugadores[3])
                    };
                    break;
            }
            
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
            Task.Delay(2500).Wait();
            FinalizarMovimientoPeones?.Invoke();            
        }

        public void DesconectarJugador(string correoElectronico)
        {
            int posicionJugadorDesconectado = ListaJugadores.FindIndex(jugadorLista => jugadorLista.CorreElectronico.Equals(correoElectronico));

            if (posicionJugadorDesconectado != -1)
            {
                ListaJugadores[posicionJugadorDesconectado].EstaConectado = false;

                if (posicionJugadorDesconectado == TurnoActual)
                {
                    PararTurnoJugador();
                }
            }
        }

        private void PararTurnoJugador()
        {
            if (_temporizadorDado.IsEnabled || _temporizadorLinea.IsEnabled)
            {
                _temporizadorDado.Stop();
                _temporizadorLinea.Stop();
                CambiarTurnoSiguiente();
            }
        }

        public void CambiarTurnoSiguiente()
        {
            ComprobarLugarValidoFinal();
            TerminarTurno?.Invoke();
            ListaJugadores[TurnoActual].TerminarTurno();

            AumentarTurnoActual(NumeroJugadores);

            if (ListaJugadores[TurnoActual].PeonTurnoActual < NUMERO_PEONES_POR_JUGADOR)
            {
                IniciarTurno();
            }
            else
            {
                RecolectarPuntuacionesTablero();
            }
        }

        private void AumentarTurnoActual(int numeroJugadoresTotales)
        {
            if (numeroJugadoresTotales <= 1)
            {
                AcabarJuegoFaltaJugadores?.Invoke();
            }
            TurnoActual = (TurnoActual + 1 >= NumeroJugadores) ? 0 : TurnoActual + 1;
            if (!ListaJugadores[TurnoActual].EstaConectado)
            {
                AumentarTurnoActual(numeroJugadoresTotales - 1);
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

        public void RecolectarPuntuacionesTablero()
        {
            foreach (JugadorLanzamiento jugador in ListaJugadores)
            {
                Logger log = new Logger(this.GetType());
                jugador.CalcularPuntajePeonesTablero(ListaPuntuaciones, ListaPeonesTablero);
            }
            PasarPuntuacionesJuego?.Invoke(ListaJugadores);
        }

        private void IniciarColoresJugadores()
        {
            ImageBrush pintarImagenAzul = new ImageBrush
            {
                ImageSource = new BitmapImage(new Uri(Properties.Resources.uriPeonMorado))
            };
            ImageBrush pintarImagenRojo = new ImageBrush
            {
                ImageSource = new BitmapImage(new Uri(Properties.Resources.uriPeonRosa))
            };
            ImageBrush pintarImagenVerde = new ImageBrush
            {
                ImageSource = new BitmapImage(new Uri(Properties.Resources.uriPeonNegro))
            };
            ImageBrush pintarImagenAmarillo = new ImageBrush
            {
                ImageSource = new BitmapImage(new Uri(Properties.Resources.uriPeonGris))
            };

            ColorPorJugador = new Dictionary<Direccion, ImageBrush>
            {
                { Direccion.Abajo, pintarImagenAzul }, 
                { Direccion.Arriba, pintarImagenRojo }, 
                { Direccion.Derecha, pintarImagenVerde }, 
                { Direccion.Izquierda, pintarImagenAmarillo }, 
            };
        }
        private void IniciarPosicionesInicioPeones()
        {
            PosicionInicioJugadores = new Dictionary<Direccion, Point>
            {
                { Direccion.Abajo,  new Point(36, 384)},
                { Direccion.Arriba,  new Point(511, 128)},
                { Direccion.Derecha,  new Point(511, 384)},
                { Direccion.Izquierda,  new Point(36, 128)}
            };
        }
        private void IniciarPosicionLanzamiento()
        {
            PosicionLanzamientoInicial = new Dictionary<Direccion, Point>
            {
                { Direccion.Abajo,  new Point(300, 535)},
                { Direccion.Arriba,  new Point(300, 25)},
                { Direccion.Derecha,  new Point(576, 279)},
                { Direccion.Izquierda,  new Point(28, 279)}
            };
        }
        private void IniciarPosicionDados()
        {
            PosicionDados = new Dictionary<Direccion, (Point, Point)>
            {
                { Direccion.Abajo, (new Point(33,465), new Point(144,465)) },
                { Direccion.Arriba, (new Point(402,31), new Point(509,31)) },
                { Direccion.Derecha, (new Point(402,465), new Point(509,465)) },
                { Direccion.Izquierda, (new Point(33,33), new Point(144,33)) }
            };
        }
    }
}
