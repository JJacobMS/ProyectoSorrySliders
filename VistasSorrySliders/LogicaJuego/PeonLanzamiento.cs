using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;
using System.Windows;
using System.Windows.Controls;

namespace VistasSorrySliders.LogicaJuego
{
    public class PeonLanzamiento
    {
        public Ellipse Figura { get; set; }
        public int VelocidadHorizontal { get; set;  }
        public int VelocidadVertical { get; set; }
        public bool EnMovimientoDerecha { get; set; }
        public bool EnMovimientoIzquierda { get; set; }
        public bool EnMovimientoArriba { get; set; }
        public bool EnMovimientoAbajo { get; set; }
        public Direccion DireccionHorizontal { get; set; }
        public Direccion DireccionVertical { get; set; }
        public Point PosicionPeon { get; set; }
        private int _numeroDisminuirVelocidad;
        private int _disminuyeVelocidad;

        public PeonLanzamiento(Ellipse figura, Point posicion)
        {
            _disminuyeVelocidad = 15;
            _numeroDisminuirVelocidad = 1;

            PosicionPeon = posicion;
            Figura = figura;
            VelocidadHorizontal = 0;
            VelocidadVertical = 0;
            EnMovimientoAbajo = false;
            EnMovimientoArriba = false;
            EnMovimientoDerecha = false;
            EnMovimientoIzquierda = false;
        }
        public void CambiarPosicionPeon(int posicionX, int posicionY)
        {
            PosicionPeon = new Point(posicionX, posicionY);
        }

        public void CambiarDireccionHorizontal(Direccion direccionNueva)
        {
            DireccionHorizontal = direccionNueva;
            switch (direccionNueva)
            {
                case Direccion.Izquierda:
                    EnMovimientoIzquierda = true;
                    EnMovimientoDerecha = false;
                    break;
                case Direccion.Derecha:
                    EnMovimientoIzquierda = false;
                    EnMovimientoDerecha = true;
                    break;
                default:
                    break;
            }
        }

        public void CambiarDireccionVertical(Direccion direccionNueva)
        {
            DireccionVertical = direccionNueva;
            switch (direccionNueva)
            {
                case Direccion.Abajo:
                    EnMovimientoAbajo = true;
                    EnMovimientoArriba = false;
                    break;
                case Direccion.Arriba:
                    EnMovimientoAbajo = false;
                    EnMovimientoArriba = true;
                    break;
                default:
                    break;
            }
        }

        public void DisminuirVelocidad()
        {
            if (VelocidadHorizontal >= 7)
            {
                VelocidadHorizontal = 5;
            }
            if (VelocidadVertical >= 7)
            {
                VelocidadVertical = 5;
            }

            VelocidadHorizontal = (VelocidadHorizontal - 1 <= 0) ? 0 : VelocidadHorizontal - 1;
            VelocidadVertical = (VelocidadVertical - 1 <= 0) ? 0 : VelocidadVertical - 1;
            if (VelocidadHorizontal <= 0)
            {
                EnMovimientoDerecha = false;
                EnMovimientoIzquierda = false;
            }

            if (VelocidadVertical <= 0)
            {
                EnMovimientoAbajo = false;
                EnMovimientoArriba = false;
            }
        }

        public bool EstaLugarValido(List<Rectangle> LugaresNoValidos)
        {
            foreach (Rectangle lugarNoValido in LugaresNoValidos)
            {
                if (IntersectaConRectangulo(lugarNoValido))
                {
                    return false;
                }
            }
            return true;
        }

        public void RealizarMovimiento(List<Rectangle> paredes, List<PeonLanzamiento> peonesTablero)
        {
            if (VelocidadHorizontal <= 0 && VelocidadVertical <= 0)
            {
                return;
            }

            if (EnMovimientoDerecha)
            {
                HacerMovimientoDerecha();
            }

            if (EnMovimientoIzquierda)
            {
                HacerMovimientoIzquierda();
            }

            if (EnMovimientoArriba)
            {
                HacerMovimientoArriba();
            }

            if (EnMovimientoAbajo)
            {
                HacerMovimientoAbajo();
            }


            foreach (Rectangle rectangulo in paredes)
            {
                if (IntersectaConRectangulo(rectangulo))
                {
                    CambiarDireccionPared(rectangulo);
                    RealizarMovimiento(paredes, peonesTablero);
                }
            }

            foreach (PeonLanzamiento peon in peonesTablero)
            {
                if (peon != this)
                {
                    if (IntersectaConOtroCirculo(peon))
                    {
                        CambiarDireccionPeones(peon);
                        peon.RealizarMovimiento(paredes, peonesTablero);
                    }
                }
            }

            if (_numeroDisminuirVelocidad >= _disminuyeVelocidad)
            {
                DisminuirVelocidad();
                _numeroDisminuirVelocidad = 1;
            }
            else
            {
                _numeroDisminuirVelocidad++;
            }
        }
        private bool IntersectaConOtroCirculo(PeonLanzamiento peonEnemigo)
        {
            //Si la distancia entre los centros es menor que la suma de los radios, los círculos se intersectan; de lo contrario, no se intersectan.
            double centroXPeonActual = Canvas.GetLeft(Figura) + Figura.Width / 2;
            double centroYPeonActual = Canvas.GetTop(Figura) + Figura.Height / 2;

            double centroXPeonEnemigo = Canvas.GetLeft(peonEnemigo.Figura) + peonEnemigo.Figura.Width / 2;
            double centroYPeonEnemigo = Canvas.GetTop(peonEnemigo.Figura) + peonEnemigo.Figura.Height / 2;

            double distancia = Math.Sqrt(Math.Pow(centroXPeonEnemigo - centroXPeonActual, 2) + Math.Pow(centroYPeonEnemigo - centroYPeonActual, 2));

            double radioFiguraActual = Figura.Width / 2;
            double radioFiguraEnemiga = peonEnemigo.Figura.Width / 2;

            return distancia < (radioFiguraActual + radioFiguraEnemiga);
        }

        private void CambiarDireccionPeones(PeonLanzamiento peonConElQueChocaron)
        {
            if (DireccionHorizontal != Direccion.Ninguna)
            {
                if (DireccionHorizontal == Direccion.Derecha)
                {
                    CambiarDireccionHorizontal(Direccion.Izquierda);
                    peonConElQueChocaron.CambiarDireccionHorizontal(Direccion.Derecha);
                }
                else
                {
                    CambiarDireccionHorizontal(Direccion.Derecha);
                    peonConElQueChocaron.CambiarDireccionHorizontal(Direccion.Izquierda);
                }
                peonConElQueChocaron.VelocidadHorizontal += VelocidadHorizontal;
                VelocidadHorizontal --;
            }

            if (DireccionVertical != Direccion.Ninguna)
            {
                if (DireccionVertical == Direccion.Arriba)
                {
                    CambiarDireccionVertical(Direccion.Abajo);
                    peonConElQueChocaron.CambiarDireccionVertical(Direccion.Arriba);
                }
                else
                {
                    CambiarDireccionHorizontal(Direccion.Arriba);
                    peonConElQueChocaron.CambiarDireccionVertical(Direccion.Abajo);
                }
                peonConElQueChocaron.VelocidadVertical += VelocidadVertical;
                VelocidadVertical --;
            }
        }

        private void CambiarDireccionPared(Rectangle rectangulo)
        {
            if (DireccionVertical != Direccion.Ninguna)
            {
                switch (DireccionVertical)
                {
                    case Direccion.Abajo:
                        if ((Canvas.GetTop(Figura) + Figura.Height) >= Canvas.GetTop(rectangulo) && Canvas.GetTop(Figura) <= Canvas.GetTop(rectangulo))
                        {
                            CambiarDireccionVertical(Direccion.Arriba);
                        }
                        break;
                    case Direccion.Arriba:
                        if ((Canvas.GetTop(Figura) + Figura.Height) >= (Canvas.GetTop(rectangulo) + rectangulo.Height) && Canvas.GetTop(Figura) <= (Canvas.GetTop(rectangulo) + rectangulo.Height))
                        {
                            CambiarDireccionVertical(Direccion.Abajo);
                        }
                        break;
                }
                if (VelocidadVertical > 1)
                {
                    VelocidadVertical--;
                }
            }

            if (DireccionHorizontal != Direccion.Ninguna)
            {
                switch (DireccionHorizontal)
                {
                    case Direccion.Derecha:
                        if ((Canvas.GetLeft(Figura) + Figura.Width) >= Canvas.GetLeft(rectangulo) && Canvas.GetLeft(Figura) <= Canvas.GetLeft(rectangulo))
                        {
                            CambiarDireccionHorizontal(Direccion.Izquierda);
                        }
                        break;
                    case Direccion.Izquierda:
                        if ((Canvas.GetLeft(Figura) + Figura.Width) >= (Canvas.GetLeft(rectangulo) + rectangulo.Width) && Canvas.GetLeft(Figura) <= (Canvas.GetLeft(rectangulo) + rectangulo.Width))
                        {
                            CambiarDireccionHorizontal(Direccion.Derecha);
                        }
                        break;
                }
                if (VelocidadHorizontal > 1)
                {
                    VelocidadHorizontal--;
                }
            }

        }

        private bool IntersectaConRectangulo(Rectangle rectangulo)
        {
            double radioPeon = Figura.Width / 2;
            double centroXPeonActual = Canvas.GetLeft(Figura) + (Figura.Width / 2);
            double centroYPeonActual = Canvas.GetTop(Figura) + (Figura.Height / 2);

            double centroXRectangulo = Canvas.GetLeft(rectangulo) + (rectangulo.Width / 2);
            double centroYRectangulo = Canvas.GetTop(rectangulo) + (rectangulo.Height / 2);

            double distanciaXCirculoRectangulo = Math.Abs(centroXPeonActual - centroXRectangulo);
            double distanciaYCirculoRectangulo = Math.Abs(centroYPeonActual - centroYRectangulo);

            if (distanciaXCirculoRectangulo > (rectangulo.Width / 2 + radioPeon))
            {
                return false;
            }
            if (distanciaYCirculoRectangulo > (rectangulo.Height / 2 + radioPeon))
            {
                return false;
            }
            if (distanciaXCirculoRectangulo <= rectangulo.Width / 2)
            {
                return true;
            }
            if (distanciaYCirculoRectangulo <= rectangulo.Height / 2)
            {
                return true;
            }

            double distanciaEsquinaRCentroC = Math.Pow(distanciaXCirculoRectangulo - rectangulo.Width / 2, 2) + Math.Pow(distanciaYCirculoRectangulo - rectangulo.Height / 2, 2);
            return distanciaEsquinaRCentroC <= Math.Pow(radioPeon, 2);
        }

        private void HacerMovimientoDerecha()
        {
            
            if ((Canvas.GetLeft(Figura) + Figura.Width * 2) < Tablero.ANCHO_TABLERO)
            {
                Canvas.SetLeft(Figura, Canvas.GetLeft(Figura) + VelocidadHorizontal);
            }
            else
            {
                VelocidadHorizontal--;
                if (VelocidadHorizontal > 0)
                {
                    CambiarDireccionHorizontal(Direccion.Izquierda);
                }
                else
                {
                    EnMovimientoDerecha = false;
                }
            }
            
        }

        private void HacerMovimientoAbajo()
        {
            if ((Canvas.GetTop(Figura) + Figura.Height * 2) < Tablero.ALTO_TABLERO)
            {
                Canvas.SetTop(Figura, Canvas.GetTop(Figura) + VelocidadVertical);
            }
            else
            {
                VelocidadVertical--;
                if (VelocidadVertical > 0)
                {
                    CambiarDireccionVertical(Direccion.Arriba);
                }
                else
                {
                    EnMovimientoAbajo = false;
                }
            }
        }

        private void HacerMovimientoArriba()
        {
            if (Canvas.GetTop(Figura) > 0)
            {
                Canvas.SetTop(Figura, Canvas.GetTop(Figura) - VelocidadVertical);
            }
            else
            {
                VelocidadVertical--;
                if (VelocidadVertical > 0)
                {
                    CambiarDireccionVertical(Direccion.Abajo);
                }
                else
                {
                    EnMovimientoArriba = false;
                }
            }
        }

        private void HacerMovimientoIzquierda()
        {
            if (Canvas.GetLeft(Figura) > 0)
            {
                Canvas.SetLeft(Figura, Canvas.GetLeft(Figura) - VelocidadHorizontal);
            }
            else
            {
                VelocidadHorizontal--;
                if (VelocidadHorizontal > 0)
                {
                    CambiarDireccionHorizontal(Direccion.Derecha);
                }
                else
                {
                    EnMovimientoIzquierda = false;
                }
            }
        }
    }
}
