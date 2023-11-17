using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;
using System.Windows;

namespace VistasSorrySliders.LogicaJuego
{
    public abstract class LineaMovimiento
    {
        public Dictionary<int, Point> CalculoPosiciones { get; set; }
        public int NumeroPosicion { get; set; }
        public int SentidoRecorrido { get; set; }
        public Point PosicionCanva { get; set; }
        public int CoordenadaX { get; set; }
        public int CoordenadaY { get; set; }
        public Line FiguraLinea { get; set; }
        public double Longitud { get; set; }
        public int IntervalosMovimientos { get; set; }

        public abstract void CalcularPosiciones();
        public void SiguientePosicion()
        {
            Point puntoSiguiente = CalculoPosiciones[NumeroPosicion];
            FiguraLinea.X2 = puntoSiguiente.X;
            FiguraLinea.Y2 = puntoSiguiente.Y;

            if (SentidoRecorrido > 0 && (NumeroPosicion + 1) >= CalculoPosiciones.Count)
            {
                SentidoRecorrido = -1;
            }
            if (SentidoRecorrido < 0 && (NumeroPosicion - 1) < 0)
            {
                SentidoRecorrido = 1;
            }
            NumeroPosicion += SentidoRecorrido;
        }

        public (double, double) RegresarCoordenadasLinea()
        {
            return (FiguraLinea.X2, FiguraLinea.Y2);
        }

        public double RegresarAnguloFormado()
        {
            double valorAbsolutoCatetoOpuesto = Math.Abs(FiguraLinea.Y2);
            return Math.Asin(valorAbsolutoCatetoOpuesto / Longitud);
        }

        public Direccion RecuperarLadoVertical()
        {
            if (FiguraLinea.Y2 > 0)
            {
                return Direccion.Abajo;
            }
            return Direccion.Arriba;
        }

        public Direccion RecuperarLadoHorizontal()
        {
            if (FiguraLinea.X2 > 0)
            {
                return Direccion.Derecha;
            }
            return Direccion.Izquierda;
        }

    }

    public class LineaMovimientoAbajo : LineaMovimiento
    {
        public LineaMovimientoAbajo(Line linea, Point puntoCanvas, int longitud, int noIntervalos)
        {
            PosicionCanva = puntoCanvas;
            IntervalosMovimientos = noIntervalos;
            Longitud = longitud;
            CoordenadaY = 0;
            CoordenadaX = -longitud;
            NumeroPosicion = 0;
            SentidoRecorrido = 1;

            FiguraLinea = linea;
            FiguraLinea.X1 = 0;
            FiguraLinea.Y1 = 0;
            FiguraLinea.X2 = -longitud;
            FiguraLinea.Y2 = 0;

            CalcularPosiciones();
        }
        public override void CalcularPosiciones()
        {
            CalculoPosiciones = new Dictionary<int, Point>();
            int numeroPosiciones = Convert.ToInt32(Longitud / IntervalosMovimientos);
            double distanciaLineaAlCuadrado = Math.Pow(Longitud, 2);

            int ladoX = -1;
            int ladoY = -1;
            //Primera mitad de la línea
            for (int i = 0; i <= numeroPosiciones; i++)
            {
                double coordenadaY = ladoY * (i * IntervalosMovimientos);
                double coordenadaYAlCuadrado = Math.Pow(coordenadaY, 2);
                double valorAbsolutoResta = Math.Abs(distanciaLineaAlCuadrado - coordenadaYAlCuadrado);
                double coordenadaX = ladoX * Math.Sqrt(valorAbsolutoResta);
                CalculoPosiciones.Add(i, new Point(coordenadaX, coordenadaY));
            }
            //Segunda mitad de la línea
            ladoX = 1;
            for (int i = numeroPosiciones + 1, llaveY = numeroPosiciones - 1; i < numeroPosiciones * 2 + 1; i++, llaveY--)
            {
                double coordenadaY = ladoY * (llaveY * IntervalosMovimientos);
                double coordenadaYAlCuadrado = Math.Pow(coordenadaY, 2);
                double valorAbsolutoResta = Math.Abs(distanciaLineaAlCuadrado - coordenadaYAlCuadrado);
                double coordenadaX = ladoX * Math.Sqrt(valorAbsolutoResta);

                CalculoPosiciones.Add(i, new Point(coordenadaX, coordenadaY));
            }
        }
    }

    public class LineaMovimientoDerecha : LineaMovimiento
    {
        public LineaMovimientoDerecha(Line linea, Point puntoCanvas, int longitud, int noIntervalos)
        {
            PosicionCanva = puntoCanvas;
            IntervalosMovimientos = noIntervalos;
            Longitud = longitud;
            CoordenadaY = -longitud;
            CoordenadaX = 0;
            NumeroPosicion = 0;
            SentidoRecorrido = 1;

            FiguraLinea = linea;
            FiguraLinea.X1 = 0;
            FiguraLinea.Y1 = 0;
            FiguraLinea.X2 = 0;
            FiguraLinea.Y2 = -longitud;

            CalcularPosiciones();
        }
        public override void CalcularPosiciones()
        {
            CalculoPosiciones = new Dictionary<int, Point>();
            int numeroPosiciones = Convert.ToInt32(Longitud / IntervalosMovimientos);
            double distanciaLineaAlCuadrado = Math.Pow(Longitud, 2);

            int ladoX = -1;
            int ladoY = -1;
            //Primera mitad de la línea, Arriba a Abajo
            for (int i = 0; i <= numeroPosiciones; i++)
            {
                double coordenadaX = ladoX * (i * IntervalosMovimientos);
                double coordenadaXAlCuadrado = Math.Pow(coordenadaX, 2);
                double valorAbsolutoResta = Math.Abs(distanciaLineaAlCuadrado - coordenadaXAlCuadrado);
                double coordenadaY = ladoY * Math.Sqrt(valorAbsolutoResta);
                CalculoPosiciones.Add(i, new Point(coordenadaX, coordenadaY));
            }
            //Segunda mitad de la línea

            ladoY = 1;
            for (int i = numeroPosiciones + 1, llaveX = numeroPosiciones - 1; i < numeroPosiciones * 2 + 1; i++, llaveX--)
            {
                double coordenadaX = ladoX * (llaveX * IntervalosMovimientos);
                double coordenadaXAlCuadrado = Math.Pow(coordenadaX, 2);
                double valorAbsolutoResta = Math.Abs(distanciaLineaAlCuadrado - coordenadaXAlCuadrado);
                double coordenadaY = ladoY * Math.Sqrt(valorAbsolutoResta);

                CalculoPosiciones.Add(i, new Point(coordenadaX, coordenadaY));
            }
        }
    }

    public class LineaMovimientoArriba : LineaMovimiento
    {
        public LineaMovimientoArriba(Line linea, Point puntoCanvas, int longitud, int noIntervalos)
        {
            PosicionCanva = puntoCanvas;
            IntervalosMovimientos = noIntervalos;
            Longitud = longitud;
            CoordenadaY = 0;
            CoordenadaX = -longitud;
            NumeroPosicion = 0;
            SentidoRecorrido = 1;

            FiguraLinea = linea;
            FiguraLinea.X1 = 0;
            FiguraLinea.Y1 = 0;
            FiguraLinea.X2 = -longitud;
            FiguraLinea.Y2 = 0;

            CalcularPosiciones();
        }
        public override void CalcularPosiciones()
        {
            CalculoPosiciones = new Dictionary<int, Point>();
            int numeroPosiciones = Convert.ToInt32(Longitud / IntervalosMovimientos);
            double distanciaLineaAlCuadrado = Math.Pow(Longitud, 2);

            int ladoX = -1;
            int ladoY = 1;
            //Primera mitad de la línea
            for (int i = 0; i <= numeroPosiciones; i++)
            {
                double coordenadaY = ladoY * (i * IntervalosMovimientos);
                double coordenadaYAlCuadrado = Math.Pow(coordenadaY, 2);
                double valorAbsolutoResta = Math.Abs(distanciaLineaAlCuadrado - coordenadaYAlCuadrado);
                double coordenadaX = ladoX * Math.Sqrt(valorAbsolutoResta);
                CalculoPosiciones.Add(i, new Point(coordenadaX, coordenadaY));
            }
            //Segunda mitad de la línea
            ladoX = 1;
            for (int i = numeroPosiciones + 1, llaveY = numeroPosiciones - 1; i < numeroPosiciones * 2 + 1; i++, llaveY--)
            {
                double coordenadaY = ladoY * (llaveY * IntervalosMovimientos);
                double coordenadaYAlCuadrado = Math.Pow(coordenadaY, 2);
                double valorAbsolutoResta = Math.Abs(distanciaLineaAlCuadrado - coordenadaYAlCuadrado);
                double coordenadaX = ladoX * Math.Sqrt(valorAbsolutoResta);

                CalculoPosiciones.Add(i, new Point(coordenadaX, coordenadaY));
            }
        }
    }

    public class LineaMovimientoIzquierda : LineaMovimiento
    {
        public LineaMovimientoIzquierda(Line linea, Point puntoCanvas, int longitud, int noIntervalos)
        {
            PosicionCanva = puntoCanvas;
            IntervalosMovimientos = noIntervalos;
            Longitud = longitud;
            CoordenadaY = -longitud;
            CoordenadaX = 0;
            NumeroPosicion = 0;
            SentidoRecorrido = 1;

            FiguraLinea = linea;
            FiguraLinea.X1 = 0;
            FiguraLinea.Y1 = 0;
            FiguraLinea.X2 = 0;
            FiguraLinea.Y2 = -longitud;

            CalcularPosiciones();
        }
        public override void CalcularPosiciones()
        {
            CalculoPosiciones = new Dictionary<int, Point>();
            int numeroPosiciones = Convert.ToInt32(Longitud / IntervalosMovimientos);
            double distanciaLineaAlCuadrado = Math.Pow(Longitud, 2);

            int ladoX = 1;
            int ladoY = -1;
            //Primera mitad de la línea, Arriba a Abajo
            for (int i = 0; i <= numeroPosiciones; i++)
            {
                double coordenadaX = ladoX * (i * IntervalosMovimientos);
                double coordenadaXAlCuadrado = Math.Pow(coordenadaX, 2);
                double valorAbsolutoResta = Math.Abs(distanciaLineaAlCuadrado - coordenadaXAlCuadrado);
                double coordenadaY = ladoY * Math.Sqrt(valorAbsolutoResta);
                CalculoPosiciones.Add(i, new Point(coordenadaX, coordenadaY));
            }
            //Segunda mitad de la línea
            ladoY = 1;
            for (int i = numeroPosiciones + 1, llaveX = numeroPosiciones - 1; i < numeroPosiciones * 2 + 1; i++, llaveX--)
            {
                double coordenadaX = ladoX * (llaveX * IntervalosMovimientos);
                double coordenadaXAlCuadrado = Math.Pow(coordenadaX, 2);
                double valorAbsolutoResta = Math.Abs(distanciaLineaAlCuadrado - coordenadaXAlCuadrado);
                double coordenadaY = ladoY * Math.Sqrt(valorAbsolutoResta);

                CalculoPosiciones.Add(i, new Point(coordenadaX, coordenadaY));
            }
        }
    }
}
