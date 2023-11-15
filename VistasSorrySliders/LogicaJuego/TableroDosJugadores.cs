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
    public class TableroDosJugadores: Tablero
    {
        public TableroDosJugadores(List<CuentaSet> listaJugadores, List<Rectangle> obstaculos, List<Rectangle> noValidos) : base()
        {
            NumeroJugadores = 2;
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
            ImageBrush pintarImagenRojo = new ImageBrush
            {
                ImageSource = new BitmapImage(new Uri("pack://application:,,,/Recursos/peonRosa.png"))
            };

            ColorPorJugador = new Dictionary<Direccion, ImageBrush>
            {
                { Direccion.Abajo, pintarImagenAzul }, //Morado
                { Direccion.Arriba, pintarImagenRojo }, //Rosa
            };
        }
        private void IniciarPosicionesInicioPeones()
        {
            PosicionInicioJugadores = new Dictionary<Direccion, Point>
            {
                { Direccion.Abajo,  new Point(36, 384)},
                { Direccion.Arriba,  new Point(511, 128)}
            };
        }
        private void IniciarPosicionLanzamiento()
        {
            PosicionLanzamientoInicial = new Dictionary<Direccion, Point>
            {
                { Direccion.Abajo,  new Point(300, 535)},
                { Direccion.Arriba,  new Point(300, 25)}
            };
        }
        private void IniciarPosicionDados()
        {
            PosicionDados = new Dictionary<Direccion, (Point, Point)>
            {
                { Direccion.Abajo, (new Point(40,466), new Point(146,466)) },
                { Direccion.Arriba, (new Point(389,31), new Point(493,31)) }
            };
        }

        private void AsignarLugaresJugadores(List<CuentaSet> listaJugadores)
        {
            ListaJugadores = new List<JugadorLanzamiento>
            {
                new JugadorLanzamiento(Direccion.Abajo, this, listaJugadores[0]),
                new JugadorLanzamiento(Direccion.Arriba, this, listaJugadores[1])
            };
        }
    }
}
