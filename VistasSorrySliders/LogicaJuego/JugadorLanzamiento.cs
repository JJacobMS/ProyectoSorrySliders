using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;
using VistasSorrySliders.ServicioSorrySliders;
using System.Windows;
using System.Windows.Media;

namespace VistasSorrySliders.LogicaJuego
{
    public class JugadorLanzamiento
    {
        public string Nickname { get; set; }
        public string CorreElectronico { get; set; }
        public bool EstaConectado { get; set; }
        public Direccion DireccionJugador { get; set; }
        public LineaMovimiento LineaMovimiento { get; set; }
        public List<PeonLanzamiento> PeonesLanzamiento { get; set; }
        public int PeonTurnoActual { get; set; }
        public List<DadoPotencia> DadosJugador { get; set; }
        public int NumeroDadosLanzados { get; set; }
        public List<int> Puntuaciones { get; set; }
        private readonly Tablero _tablero;
        public JugadorLanzamiento(Direccion direccionJugador, Tablero tablero, CuentaSet cuentaJugador)
        {
            _tablero = tablero;
            Nickname = cuentaJugador.Nickname;
            CorreElectronico = cuentaJugador.CorreoElectronico;
            DireccionJugador = direccionJugador;
            NumeroDadosLanzados = 0;
            EstaConectado = true;

            GenerarPeonesLanzamiento();
            GenerarLineaMovimiento();
            GenerarDados();
        }
        public int RegresarValorDado(int dado)
        {
            return DadosJugador[dado].NumeroDado;
        }
        private void GenerarPeonesLanzamiento()
        {
            Random rnd = new Random(Guid.NewGuid().GetHashCode());
            PeonesLanzamiento = new List<PeonLanzamiento>();
            for (int i = 0; i < Tablero.NUMERO_PEONES_POR_JUGADOR; i++)
            {
                int posicionY = rnd.Next(Tablero.ESPACIO_COLOCAR_FICHAS) + Convert.ToInt32(_tablero.PosicionInicioJugadores[DireccionJugador].Y);
                int posicionX = rnd.Next(Tablero.ESPACIO_COLOCAR_FICHAS) + Convert.ToInt32(_tablero.PosicionInicioJugadores[DireccionJugador].X);
                Ellipse elipse = new Ellipse
                {
                    Width = Tablero.TAMANO_PEON,
                    Height = Tablero.TAMANO_PEON,
                    Fill = _tablero.ColorPorJugador[DireccionJugador]
                };
                PeonesLanzamiento.Add(new PeonLanzamiento(elipse, new Point(posicionX, posicionY)));
            }
            PeonTurnoActual = 0;
        }
        private void GenerarLineaMovimiento()
        {
            Line linea = new Line
            {
                Stroke = new SolidColorBrush(Colors.Black),
                StrokeThickness = 3
            };

            int margenLinea = 5;
            Point puntoCanvas = new Point(_tablero.PosicionLanzamientoInicial[DireccionJugador].X + margenLinea, _tablero.PosicionLanzamientoInicial[DireccionJugador].Y + margenLinea);
            switch (DireccionJugador)
            {
                case Direccion.Abajo:
                    LineaMovimiento = new LineaMovimientoAbajo(linea, puntoCanvas, Tablero.LONGITUD_LINEA, Tablero.NO_INTERVALOS);
                    break;
                case Direccion.Derecha:
                    LineaMovimiento = new LineaMovimientoDerecha(linea, puntoCanvas, Tablero.LONGITUD_LINEA, Tablero.NO_INTERVALOS);
                    break;
                case Direccion.Arriba:
                    LineaMovimiento = new LineaMovimientoArriba(linea, puntoCanvas, Tablero.LONGITUD_LINEA, Tablero.NO_INTERVALOS);
                    break;
                case Direccion.Izquierda:
                    LineaMovimiento = new LineaMovimientoIzquierda(linea, puntoCanvas, Tablero.LONGITUD_LINEA, Tablero.NO_INTERVALOS);
                    break;
            }
        }
        private void GenerarDados()
        {
            (Point puntoDado1, Point puntoDado2) = _tablero.PosicionDados[DireccionJugador];
            DadosJugador = new List<DadoPotencia>
            {
                new DadoPotencia(1, puntoDado1, Tablero.TAMANO_DADO), new DadoPotencia(1, puntoDado2, Tablero.TAMANO_DADO)
            };
        }
        public void IniciarMovimientoPeon()
        {
            PeonLanzamiento peonActual = PeonesLanzamiento[PeonTurnoActual];
            int distanciaVertical, distanciaHorizontal;
            switch (DireccionJugador)
            {
                case Direccion.Ninguna:
                    break;

                case Direccion.Izquierda:
                    peonActual.CambiarDireccionHorizontal(Direccion.Derecha);
                    break;

                case Direccion.Derecha:
                    peonActual.CambiarDireccionHorizontal(Direccion.Izquierda);
                    break;

                case Direccion.Arriba:
                    peonActual.CambiarDireccionVertical(Direccion.Abajo);
                    break;

                case Direccion.Abajo:
                    peonActual.CambiarDireccionVertical(Direccion.Arriba);
                    break;
            }

            if (DireccionJugador == Direccion.Izquierda || DireccionJugador == Direccion.Derecha)
            {
                (distanciaVertical, distanciaHorizontal) = CalcularDistanciasConBaseMultiplicador();
                if (distanciaVertical > 0)
                {
                    peonActual.CambiarDireccionVertical(LineaMovimiento.RecuperarLadoVertical());
                }
                peonActual.VelocidadVertical = distanciaVertical;
                peonActual.VelocidadHorizontal = distanciaHorizontal;
            }

            if (DireccionJugador == Direccion.Abajo || DireccionJugador == Direccion.Arriba)
            {
                (distanciaVertical, distanciaHorizontal) = CalcularDistanciasConBaseMultiplicador();
                if (distanciaHorizontal > 0)
                {
                    peonActual.CambiarDireccionHorizontal(LineaMovimiento.RecuperarLadoHorizontal());
                }
                peonActual.VelocidadVertical = distanciaVertical;
                peonActual.VelocidadHorizontal = distanciaHorizontal;
            }
        }

        public void CambiarDados()
        {
            if (NumeroDadosLanzados == 0)
            {
                DadosJugador[0].CambiarNumeroDado();
                DadosJugador[1].CambiarNumeroDado();
                return;
            }
            if (NumeroDadosLanzados == 1)
            {
                DadosJugador[1].CambiarNumeroDado();
            }
        }
        public void AumentarDadosLanzados()
        {
            NumeroDadosLanzados++;
        }
        public (int, int) RegresarPotenciaDados()
        {
            int potenciaDados = 0;
            int potenciaAgregada = 0;
            if (DadosJugador[0].NumeroDado == DadosJugador[1].NumeroDado)
            {
                potenciaAgregada = 1;
            }

            foreach (DadoPotencia dado in DadosJugador)
            {
                potenciaDados += dado.NumeroDado;
            }
            return (potenciaDados, potenciaAgregada);
        }
        public void TerminarTurno()
        {
            GenerarLineaMovimiento();
            if (PeonTurnoActual < 4)
            {
                PeonTurnoActual++;
            }
            NumeroDadosLanzados = 0;
        }
        private (int, int) CalcularDistanciasConBaseMultiplicador()
        {
            double anguloValorAbsoluto = Math.Abs(LineaMovimiento.RegresarAnguloFormado());
            (int potenciaDados, int potenciaAumentada) = RegresarPotenciaDados();
            int sumaPotencia = (potenciaDados + potenciaAumentada);

            switch (anguloValorAbsoluto)
            {
                case 90:
                    return (sumaPotencia, 0);
                case 0:
                    return (0, sumaPotencia);
                default:
                    break;
            }

            int distanciaVertical = Math.Abs(Convert.ToInt32(sumaPotencia * Math.Sin(anguloValorAbsoluto)));
            int distanciaHorizontal = Math.Abs(Convert.ToInt32(sumaPotencia * Math.Cos(anguloValorAbsoluto)));
            return (distanciaVertical, distanciaHorizontal);
        }

        public void CalcularPuntajePeonesTablero(Dictionary<Ellipse, int> circulosPuntuacion, List<PeonLanzamiento> peonesTablero)
        {
            Puntuaciones = new List<int>();
            foreach (PeonLanzamiento peonJugador in PeonesLanzamiento)
            {
                if (peonesTablero.Contains(peonJugador))
                {
                    Puntuaciones.Add(peonJugador.CalcularPuntuacion(circulosPuntuacion));
                }
                else
                {
                    Puntuaciones.Add(0);
                }
                
            }
        }
    }
}
