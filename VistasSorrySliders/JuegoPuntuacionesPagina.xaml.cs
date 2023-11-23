using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using VistasSorrySliders.LogicaJuego;
using VistasSorrySliders.ServicioSorrySliders;

namespace VistasSorrySliders
{
    /// <summary>
    /// Lógica de interacción para JuegoPuntuacionesPagina.xaml
    /// </summary>
    public partial class JuegoPuntuacionesPagina : Page
    {
        private string _correoUsuario;
        private int _numeroJugadores;
        private const int AVANZAR_UNA_FILA = 9;
        private const int POSICION_FINAL_HOME = 79;
        private const int LIMITE_FILAS = 60;
        private List<JugadorLanzamiento> _jugadoresLanzamiento;
        Button btnAnterior;
        Button btnSeleccionado;

        //VALIDAR QUE YA LLEGO A SU LIMITE LA FICHA
        //SOLO VOLVER ENABLE SI ES EL JUGADOR -- SI ES SU TURNO
        //VALIDAR QUE LOS 4 YA LLEGARON
        //COMUNICAR POSICIONES A LOS DEMAS


        public JuegoPuntuacionesPagina(List<JugadorLanzamiento> jugadores, string correo)
        {
            InitializeComponent();
            _correoUsuario = correo;
            _jugadoresLanzamiento = jugadores;
            Mostrar(jugadores);
            InicializarTablero();
            ActivarComponentesAzul();
            ActivarComponentesRojo();
            ActivarComponentesVerde();
            ActivarComponentesAmarillo();

        }

        private void Mostrar(List<JugadorLanzamiento> jugadores)
        {
            foreach (JugadorLanzamiento jugador in jugadores)
            {
                Console.WriteLine(jugador.CorreElectronico + ": ");
                foreach (int puntuacionPeon in jugador.Puntuaciones)
                {
                    Console.Write(puntuacionPeon + " - ");
                }
            }
        }
        private void InicializarTablero()
        {
            InicializarImagen();
            InicializarLabels();
            InicializarComponentesJuego();
            InicializarDados();
        }

        //Metodo mover ficha --En 9
        //Metodo cliquear dado
        //Metodo pasar datos

        private void InicializarImagen() 
        {
            switch (_jugadoresLanzamiento.Count())
            {
                case 2:
                    Uri urlRelativa1 = new Uri("Recursos/tableroPuntuacionDos.png", UriKind.Relative);
                    mgTablero.Source = new BitmapImage(urlRelativa1);
                    break;
                case 3:
                    Uri urlRelativa2 = new Uri("Recursos/tableroPuntuacionTres.png", UriKind.Relative);
                    mgTablero.Source = new BitmapImage(urlRelativa2);
                    break;
                case 4:
                    Uri urlRelativa3 = new Uri("Recursos/tableroPuntuacionCuatro.png", UriKind.Relative);
                    mgTablero.Source = new BitmapImage(urlRelativa3);
                    break;
                default:
                    break;
            }
        }
        private void InicializarLabels()
        {
            switch (_jugadoresLanzamiento.Count())
            {
                case 2:
                    lblJugadorAzul.Content = _jugadoresLanzamiento[0].Nickname;
                    lblPuntuacionJugadorAzul.Content = "Puntuación Última Jugada: " + CalcularPuntuacion(_jugadoresLanzamiento[0].Puntuaciones);
                    lblJugadorRojo.Content = _jugadoresLanzamiento[1].Nickname;
                    lblPuntuacionJugadorRojo.Content = "Puntuación Última Jugada: " + CalcularPuntuacion(_jugadoresLanzamiento[1].Puntuaciones);
                    break;
                case 3:
                    lblJugadorAzul.Content = _jugadoresLanzamiento[0].Nickname;
                    lblPuntuacionJugadorAzul.Content = "Puntuación Última Jugada: " + CalcularPuntuacion(_jugadoresLanzamiento[0].Puntuaciones);

                    lblJugadorAmarillo.Content = _jugadoresLanzamiento[1].Nickname;
                    lblPuntuacionJugadorAmarillo.Content = "Puntuación Última Jugada: " + CalcularPuntuacion(_jugadoresLanzamiento[1].Puntuaciones);

                    lblJugadorVerde.Content = _jugadoresLanzamiento[2].Nickname;
                    lblPuntuacionJugadorVerde.Content = "Puntuación Última Jugada: " + CalcularPuntuacion(_jugadoresLanzamiento[2].Puntuaciones);

                    break;
                case 4:
                    lblJugadorAzul.Content = _jugadoresLanzamiento[0].Nickname;
                    lblPuntuacionJugadorAzul.Content = "Puntuación Última Jugada: " + CalcularPuntuacion(_jugadoresLanzamiento[0].Puntuaciones);

                    lblJugadorVerde.Content = _jugadoresLanzamiento[1].Nickname;
                    lblPuntuacionJugadorVerde.Content = "Puntuación Última Jugada: " + CalcularPuntuacion(_jugadoresLanzamiento[1].Puntuaciones);

                    lblJugadorRojo.Content = _jugadoresLanzamiento[2].Nickname;
                    lblPuntuacionJugadorRojo.Content = "Puntuación Última Jugada: " + CalcularPuntuacion(_jugadoresLanzamiento[2].Puntuaciones);

                    lblJugadorAmarillo.Content = _jugadoresLanzamiento[3].Nickname;
                    lblPuntuacionJugadorAmarillo.Content = "Puntuación Última Jugada: " + CalcularPuntuacion(_jugadoresLanzamiento[3].Puntuaciones);

                    break;
                default:
                    break;
            }
        }

        private void InicializarComponentesJuego() 
        {
            switch (_jugadoresLanzamiento.Count())
            {
                case 2:
                    MostrarComponentesAzul();
                    MostrarComponentesRojo();
                    break;
                case 3:
                    MostrarComponentesAzul();
                    MostrarComponentesAmarillo();
                    MostrarComponentesVerde();
                    break;
                case 4:
                    MostrarComponentesAzul();
                    MostrarComponentesVerde();
                    MostrarComponentesRojo();
                    MostrarComponentesAmarillo();
                    break;
                default:
                    break;
            }
        }
        private void MostrarComponentesAzul() 
        {
            cnvAzul.Visibility = Visibility.Visible;
            grdAzul.Visibility = Visibility.Visible;
        }
        
        private void MostrarComponentesRojo()
        {
            cnvRojo.Visibility = Visibility.Visible;
            grdRojo.Visibility = Visibility.Visible;
        }
        private void MostrarComponentesAmarillo()
        {
            cnvAmarillo.Visibility = Visibility.Visible;
            grdAmarillo.Visibility = Visibility.Visible;
        }
        private void MostrarComponentesVerde()
        {
            cnvVerde.Visibility = Visibility.Visible;
            grdVerde.Visibility = Visibility.Visible;
        }
        private void ActivarComponentesAzul()
        {
            cnvAzul.IsEnabled = true;
            grdAzul.IsEnabled = true;
        }
        private void ActivarComponentesRojo()
        {
            cnvRojo.IsEnabled = true;
            grdRojo.IsEnabled = true;
        }
        private void ActivarComponentesAmarillo()
        {
            cnvAmarillo.IsEnabled = true;
            grdAmarillo.IsEnabled = true;
        }
        private void ActivarComponentesVerde()
        {
            cnvVerde.IsEnabled = true;
            grdVerde.IsEnabled = true;
        }
        

        private int CalcularPuntuacion(List<int> puntuaciones) 
        {
            int puntuacion= 0;
            for (int i = 0; i < puntuaciones.Count(); i++)
            {
                puntuacion = puntuacion + puntuaciones[i];
            }
            return puntuacion;
        }
        private void InicializarDados() 
        {
            switch (_jugadoresLanzamiento.Count())
            {
                case 2:
                    IngresarPuntuacionesAzul(_jugadoresLanzamiento[0].Puntuaciones);
                    IngresarPuntuacionesRojo(_jugadoresLanzamiento[1].Puntuaciones);
                    break;
                case 3:
                    IngresarPuntuacionesAzul(_jugadoresLanzamiento[0].Puntuaciones);
                    IngresarPuntuacionesAmarillo(_jugadoresLanzamiento[1].Puntuaciones);
                    IngresarPuntuacionesVerde(_jugadoresLanzamiento[2].Puntuaciones);

                    break;
                case 4:
                    IngresarPuntuacionesAzul(_jugadoresLanzamiento[0].Puntuaciones);
                    IngresarPuntuacionesVerde(_jugadoresLanzamiento[1].Puntuaciones);
                    IngresarPuntuacionesRojo(_jugadoresLanzamiento[2].Puntuaciones);
                    IngresarPuntuacionesAmarillo(_jugadoresLanzamiento[3].Puntuaciones);

                    break;
                default:
                    break;
            }
        }
        private void IngresarPuntuacionesAzul(List<int> puntuaciones) 
        {
            btnPuntuacionAzul1.Content = 1;//puntuaciones[0];
            btnPuntuacionAzul2.Content = 2;// puntuaciones[1];
            btnPuntuacionAzul3.Content = 3;// puntuaciones[2];
            btnPuntuacionAzul4.Content = 4;// puntuaciones[3];
        }

        private void IngresarPuntuacionesRojo(List<int> puntuaciones)
        {
            btnPuntuacionRojo1.Content = 1;// puntuaciones[0];
            btnPuntuacionRojo2.Content = 2;// puntuaciones[1];
            btnPuntuacionRojo3.Content = 3;// puntuaciones[2];
            btnPuntuacionRojo4.Content = 4;// puntuaciones[3];
        }
        private void IngresarPuntuacionesVerde(List<int> puntuaciones)
        {
            btnPuntuacionVerde1.Content = 1;//puntuaciones[0];
            btnPuntuacionVerde2.Content = 2;//puntuaciones[1];
            btnPuntuacionVerde3.Content = 3;//puntuaciones[2];
            btnPuntuacionVerde4.Content = 4;//puntuaciones[3];
        }
        private void IngresarPuntuacionesAmarillo(List<int> puntuaciones)
        {
            btnPuntuacionAmarillo1.Content = 1;//puntuaciones[0];
            btnPuntuacionAmarillo2.Content = 2;//puntuaciones[1];
            btnPuntuacionAmarillo3.Content = 3;//puntuaciones[2];
            btnPuntuacionAmarillo4.Content = 4;//puntuaciones[3];
        }

        private void MouseLeftButtonDownPeonRojo(object sender, MouseButtonEventArgs e)
        {
            Console.WriteLine("Elipse roja");
            if (btnSeleccionado != null)
            {
                Ellipse llpSeleccionada = sender as Ellipse;
                double posicion = Canvas.GetTop(llpSeleccionada);
                int puntosObtenidos = ObtenerFilas();
                if (puntosObtenidos >= 0)
                {
                    btnSeleccionado.IsEnabled = false;
                    for (int i = 0; i < puntosObtenidos; i++)
                    {
                        Console.WriteLine("Puntos obtenidos " + puntosObtenidos);
                        if (posicion < LIMITE_FILAS)
                        {
                            Console.WriteLine("Avanza una fila");
                            Canvas.SetTop(llpSeleccionada, posicion + (AVANZAR_UNA_FILA * puntosObtenidos));
                            Thread.Sleep(750);
                        }
                        else 
                        {
                            Canvas.SetTop(llpSeleccionada, POSICION_FINAL_HOME);
                            llpSeleccionada.IsEnabled = false;
                        }
                    }
                }
            }
        }

        private void MouseLeftButtonDownPeonAzul(object sender, MouseButtonEventArgs e)
        {
            Console.WriteLine("Elipse azul");
            if (btnSeleccionado != null)
            {
                Ellipse llpSeleccionada = sender as Ellipse;
                double posicion = Canvas.GetBottom(llpSeleccionada);
                int puntosObtenidos = ObtenerFilas();

                if (puntosObtenidos >= 0)
                {
                    btnSeleccionado.IsEnabled = false;
                    for (int i = 0; i < puntosObtenidos; i++)
                    {
                        Console.WriteLine("Puntos obtenidos " + puntosObtenidos);
                        if (posicion < LIMITE_FILAS)
                        {
                            Console.WriteLine("Avanza una fila");
                            Canvas.SetBottom(llpSeleccionada, posicion + (AVANZAR_UNA_FILA * puntosObtenidos));
                            Thread.Sleep(750);
                        }
                        else 
                        {
                            Canvas.SetBottom(llpSeleccionada, POSICION_FINAL_HOME);
                            llpSeleccionada.IsEnabled = false;
                        }
                    }
                }
            }
        }

        private void MouseLeftButtonDownPeonVerde(object sender, MouseButtonEventArgs e)
        {
            Console.WriteLine("Elipse verde");
            if (btnSeleccionado != null)
            {
                Ellipse llpSeleccionada = sender as Ellipse;
                double posicion = Canvas.GetRight(llpSeleccionada);
                int puntosObtenidos = ObtenerFilas();
                if (puntosObtenidos >= 0)
                {
                    btnSeleccionado.IsEnabled = false;
                    for (int i = 0; i < puntosObtenidos; i++)
                    {
                        Console.WriteLine("Puntos obtenidos " + puntosObtenidos);
                        if (posicion < LIMITE_FILAS)
                        {
                            Console.WriteLine("Avanza una fila");
                            Canvas.SetRight(llpSeleccionada, posicion + AVANZAR_UNA_FILA);
                            Thread.Sleep(750);
                        }
                        else 
                        {
                            Canvas.SetRight(llpSeleccionada, POSICION_FINAL_HOME);
                            llpSeleccionada.IsEnabled = false;
                        }
                    }
                }
            }
        }

        private void MouseLeftButtonDownPeonAmarillo(object sender, MouseButtonEventArgs e)
        {
            Console.WriteLine("Elipse amarillo");
            if (btnSeleccionado != null)
            {
                Ellipse llpSeleccionada = sender as Ellipse;
                double posicion = Canvas.GetLeft(llpSeleccionada);
                int puntosObtenidos = ObtenerFilas();
                if (puntosObtenidos >= 0)
                {
                    Console.WriteLine("Puntos obtenidos " + puntosObtenidos);
                    btnSeleccionado.IsEnabled = false;
                    for (int i = 0; i < puntosObtenidos; i++)
                    {
                        if (posicion < LIMITE_FILAS)
                        {
                            Console.WriteLine("Avanza una fila");
                            Canvas.SetLeft(llpSeleccionada, posicion + (AVANZAR_UNA_FILA * puntosObtenidos));
                            Thread.Sleep(750);
                        }
                        else 
                        {
                            Canvas.SetLeft(llpSeleccionada, POSICION_FINAL_HOME);
                            llpSeleccionada.IsEnabled = false;
                        }
                    }
                }
            }
        }

        private int ObtenerFilas() 
        {
            Logger log = new Logger(this.GetType());
            try
            {
                string puntosTextoBoton = btnSeleccionado.Content.ToString();
                int puntos = int.Parse(puntosTextoBoton);
                Console.WriteLine("Puntos obtenidos "+puntos);
                return puntos;
            }
            catch (FormatException ex)
            {
                log.LogError("Error al Parsear String a Int",ex);
                return -1;
            }
        }

        private void ClickBtnAmarillo(object sender, RoutedEventArgs e)
        {
            btnSeleccionado = CambiarColorSeleccion(sender);
        }

        private void ClickBtnRojo(object sender, RoutedEventArgs e)
        {
            btnSeleccionado = CambiarColorSeleccion(sender);
        }

        private void ClickBtnAzul(object sender, RoutedEventArgs e)
        {
            btnSeleccionado = CambiarColorSeleccion(sender);
        }

        private void ClickBtnVerde(object sender, RoutedEventArgs e)
        {
            btnSeleccionado = CambiarColorSeleccion(sender);
        }

        private Button CambiarColorSeleccion(object sender)
        {
            if (btnAnterior != null)
            {
                btnAnterior.BorderBrush = Brushes.Black;
            }
            Button btnSeleccionado = sender as Button;
            btnSeleccionado.BorderBrush = Brushes.Red;
            btnAnterior = btnSeleccionado;
            return btnSeleccionado;
        }
    }
}
