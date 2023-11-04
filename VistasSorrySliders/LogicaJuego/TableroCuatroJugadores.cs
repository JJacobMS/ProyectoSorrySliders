using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Windows.Shapes;
using VistasSorrySliders.ServicioSorrySliders;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows;

namespace VistasSorrySliders.LogicaJuego
{
    public class TableroCuatroJugadores : Tablero
    {
        public event Action IniciarJuego;
        public event Action<int, int> MostrarPotenciaLanzamiento;
        public event Action TerminarTurno;
        public TableroCuatroJugadores(List<CuentaSet> listaJugadores, List<Rectangle> obstaculos)
        {
            NumeroJugadores = 4;
            TurnoActual = 0;
            ListaObstaculos = obstaculos;

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
            _temporizadorPeonesMovimiento.Interval = TimeSpan.FromMilliseconds(20);

            /*_temporizadorDisminuarVelocidad = new DispatcherTimer();
            _temporizadorDisminuarVelocidad.Tick += DisminuirVelocidadPeones;
            _temporizadorDisminuarVelocidad.Interval = TimeSpan.FromMilliseconds(500);*/

        }

        private void IniciarColoresJugadores()
        {
            ImageBrush pintarImagenAzul = new ImageBrush
            {
                ImageSource = new BitmapImage(new Uri("pack://application:,,,/Recursos/peonMorado.png"))
            };
            ImageBrush pintarImagenRojo = new ImageBrush
            {
                ImageSource = new BitmapImage(new Uri("pack://application:,,,/Recursos/peonRosa.png"))
            };
            ImageBrush pintarImagenVerde = new ImageBrush
            {
                ImageSource = new BitmapImage(new Uri("pack://application:,,,/Recursos/peonNegro.png"))
            };
            ImageBrush pintarImagenAmarillo = new ImageBrush
            {
                ImageSource = new BitmapImage(new Uri("pack://application:,,,/Recursos/peonGris.png"))
            };

            ColorPorJugador = new Dictionary<Direccion, ImageBrush>
            {
                { Direccion.Abajo, pintarImagenAzul }, //Morado
                { Direccion.Arriba, pintarImagenRojo }, //Rosa
                { Direccion.Derecha, pintarImagenVerde }, //Negro
                { Direccion.Izquierda, pintarImagenAmarillo }, //Gris
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
        public void IniciarPosicionDados()
        {
            PosicionDados = new Dictionary<Direccion, (Point, Point)>
            {
                { Direccion.Abajo, (new Point(33,465), new Point(144,465)) },
                { Direccion.Arriba, (new Point(398,32), new Point(506,32)) },
                { Direccion.Derecha, (new Point(402,465), new Point(506,465)) },
                { Direccion.Izquierda, (new Point(33,32), new Point(144,32)) }
            };
        }

        public void AsignarLugaresJugadores(List<CuentaSet> listaJugadores)
        {
            ListaJugadores = new List<JugadorLanzamiento>
            {
                new JugadorLanzamiento(Direccion.Abajo, this, listaJugadores[0]),
                new JugadorLanzamiento(Direccion.Derecha, this, listaJugadores[1]),
                new JugadorLanzamiento(Direccion.Arriba, this, listaJugadores[2]),
                new JugadorLanzamiento(Direccion.Izquierda, this, listaJugadores[3])
            };
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

            TurnoActual = (TurnoActual + 1 >= 4) ? 0 : TurnoActual + 1;

            if (ListaJugadores[TurnoActual].PeonTurnoActual < 4)
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
