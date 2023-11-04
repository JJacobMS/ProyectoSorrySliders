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

    }
}
