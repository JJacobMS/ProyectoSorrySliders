using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows.Shapes;
using VistasSorrySliders.ServicioSorrySliders;
using System.Windows;

namespace VistasSorrySliders.LogicaJuego
{
    public class TableroTresJugadores : Tablero
    {
        public TableroTresJugadores(List<CuentaSet> listaJugadores, List<Rectangle> obstaculos, List<Rectangle> noValidos) : base()
        {
            NumeroJugadores = 3;
            TurnoActual = 0;
            ListaObstaculos = obstaculos;
            ListaLugaresNoValidos = noValidos;

            IniciarColoresJugadores();
            IniciarPosicionesInicioPeones();
            IniciarPosicionLanzamiento();
            IniciarPosicionDados();
            AsignarLugaresJugadores(listaJugadores);

        }
        private void IniciarColoresJugadores()
        {
            ImageBrush pintarImagenAzul = new ImageBrush
            {
                ImageSource = new BitmapImage(new Uri("pack://application:,,,/Recursos/peonMorado.png"))
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
                { Direccion.Derecha, pintarImagenVerde }, //Negro
                { Direccion.Izquierda, pintarImagenAmarillo }, //Gris
            };
        }
        private void IniciarPosicionesInicioPeones()
        {
            PosicionInicioJugadores = new Dictionary<Direccion, Point>
            {
                { Direccion.Abajo,  new Point(36, 384)},
                { Direccion.Derecha,  new Point(511, 384)},
                { Direccion.Izquierda,  new Point(36, 128)}
            };
        }
        private void IniciarPosicionLanzamiento()
        {
            PosicionLanzamientoInicial = new Dictionary<Direccion, Point>
            {
                { Direccion.Abajo,  new Point(300, 535)},
                { Direccion.Derecha,  new Point(576, 279)},
                { Direccion.Izquierda,  new Point(28, 279)}
            };
        }
        private void IniciarPosicionDados()
        {
            PosicionDados = new Dictionary<Direccion, (Point, Point)>
            {
                { Direccion.Abajo, (new Point(33,465), new Point(144,465)) },
                { Direccion.Derecha, (new Point(402,465), new Point(506,465)) },
                { Direccion.Izquierda, (new Point(34,32), new Point(144,32)) }
            };
        }

        private void AsignarLugaresJugadores(List<CuentaSet> listaJugadores)
        {
            ListaJugadores = new List<JugadorLanzamiento>
            {
                new JugadorLanzamiento(Direccion.Abajo, this, listaJugadores[0]),
                new JugadorLanzamiento(Direccion.Derecha, this, listaJugadores[1]),
                new JugadorLanzamiento(Direccion.Izquierda, this, listaJugadores[2])
            };
        }
    }
}
