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
        private const int LIMITE_FILAS = 50;
        private Dictionary<string, List<Ellipse>> _diccionarioElementos = new Dictionary<string, List<Ellipse>>();
        private List<JugadorLanzamiento> _jugadoresLanzamiento;
        private List<Ellipse> _listaEllipseRoja;
        private List<Ellipse> _listaEllipseAmarilla;
        private List<Ellipse> _listaEllipseVerde;
        private List<Ellipse> _listaEllipseAzul;
        private Button btnAnterior;
        private Button btnSeleccionado;

        //VALIDAR QUE YA LLEGO A SU LIMITE LA FICHA
        //SOLO VOLVER ENABLE SI ES EL JUGADOR -- SI ES SU TURNO
        //VALIDAR QUE LOS 4 YA LLEGARON
        //COMUNICAR POSICIONES A LOS DEMAS


        public JuegoPuntuacionesPagina(List<JugadorLanzamiento> jugadores, string correo)
        {
            InitializeComponent();
            _listaEllipseRoja = new List<Ellipse> { llpPeonRojo1, llpPeonRojo2, llpPeonRojo3, llpPeonRojo4 };
            _listaEllipseAmarilla = new List<Ellipse> { llpPeonAmarillo1, llpPeonAmarillo2, llpPeonAmarillo3, llpPeonAmarillo4 };
            _listaEllipseVerde = new List<Ellipse> { llpPeonVerde1, llpPeonVerde2, llpPeonVerde3, llpPeonVerde4 };
            _listaEllipseAzul = new List<Ellipse> { llpPeonAzul1, llpPeonAzul2, llpPeonAzul3, llpPeonAzul4 };
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
                _diccionarioElementos.Add(jugador.CorreElectronico, _listaEllipseRoja);
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
            MoverPeonRojoAsync(sender);
        }
        private async void MoverPeonRojoAsync(object sender) 
        {
            if (btnSeleccionado != null)
            {
                Ellipse llpSeleccionada = sender as Ellipse;
                int puntosObtenidos = ObtenerFilas();
                if (puntosObtenidos >= 0)
                {
                    btnSeleccionado.IsEnabled = false;
                    btnSeleccionado = null;
                    for (int i = 0; i < puntosObtenidos; i++)
                    {
                        double posicion = Canvas.GetTop(llpSeleccionada);
                        if (posicion < LIMITE_FILAS)
                        {
                            Canvas.SetTop(llpSeleccionada, posicion + AVANZAR_UNA_FILA);
                            await Task.Delay(1000);
                        }
                        else
                        {
                            Canvas.SetTop(llpSeleccionada, POSICION_FINAL_HOME);
                            llpSeleccionada.IsEnabled = false;
                            ComprobarGanador();
                        }
                    }
                }
            }
        }

        private void MouseLeftButtonDownPeonAzul(object sender, MouseButtonEventArgs e)
        {
            MoverPeonAzulAsync(sender);    
        }
        private async void MoverPeonAzulAsync(object sender)
        {
            if (btnSeleccionado != null)
            {
                Ellipse llpSeleccionada = sender as Ellipse;
                double posicion = Canvas.GetBottom(llpSeleccionada);
                int puntosObtenidos = ObtenerFilas();

                if (puntosObtenidos >= 0)
                {
                    btnSeleccionado.IsEnabled = false;
                    btnSeleccionado = null;
                    for (int i = 0; i < puntosObtenidos; i++)
                    {
                        Console.WriteLine("Puntos obtenidos " + puntosObtenidos);
                        if (posicion < LIMITE_FILAS)
                        {
                            Canvas.SetBottom(llpSeleccionada, posicion + AVANZAR_UNA_FILA);
                            await Task.Delay(1000);
                        }
                        else
                        {
                            Canvas.SetBottom(llpSeleccionada, POSICION_FINAL_HOME);
                            llpSeleccionada.IsEnabled = false;
                            ComprobarGanador();
                        }
                    }
                }
            }
        }

        private void MouseLeftButtonDownPeonVerde(object sender, MouseButtonEventArgs e)
        {
            MoverPeonVerdeAsync(sender);
        }
        private async void MoverPeonVerdeAsync(object sender)
        {
            if (btnSeleccionado != null)
            {
                Ellipse llpSeleccionada = sender as Ellipse;
                int puntosObtenidos = ObtenerFilas();
                if (puntosObtenidos >= 0)
                {
                    btnSeleccionado.IsEnabled = false;
                    btnSeleccionado = null;
                    for (int i = 0; i < puntosObtenidos; i++)
                    {
                        double posicion = Canvas.GetRight(llpSeleccionada);
                        if (posicion < LIMITE_FILAS)
                        {
                            Canvas.SetRight(llpSeleccionada, posicion + AVANZAR_UNA_FILA);
                            await Task.Delay(1000);
                        }
                        else
                        {
                            Canvas.SetRight(llpSeleccionada, POSICION_FINAL_HOME);
                            llpSeleccionada.IsEnabled = false;
                            ComprobarGanador();
                        }
                    }
                }
            }
        }

        private void MouseLeftButtonDownPeonAmarillo(object sender, MouseButtonEventArgs e)
        {
            MoverPeonAmarilloAsync(sender);
        }

        private async void MoverPeonAmarilloAsync(object sender)
        {
            if (btnSeleccionado != null)
            {
                Ellipse llpSeleccionada = sender as Ellipse;
                int puntosObtenidos = ObtenerFilas();
                if (puntosObtenidos >= 0)
                {
                    Console.WriteLine("Puntos obtenidos " + puntosObtenidos);
                    btnSeleccionado.IsEnabled = false;
                    btnSeleccionado = null;
                    for (int i = 0; i < puntosObtenidos; i++)
                    {
                        double posicion = Canvas.GetLeft(llpSeleccionada);
                        if (posicion < LIMITE_FILAS)
                        {
                            Canvas.SetLeft(llpSeleccionada, posicion + AVANZAR_UNA_FILA);
                            await Task.Delay(1000);
                        }
                        else
                        {
                            Canvas.SetLeft(llpSeleccionada, POSICION_FINAL_HOME);
                            llpSeleccionada.IsEnabled = false;
                            //NotificarJugador
                            ComprobarGanador();
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

        private void ComprobarGanador()
        {
            int contadorRojo = 0;
            int contadorAzul = 0;
            int contadorVerde = 0;
            int contadorAmarillo = 0;
            int tamañoListas = 4;
            int fichasEnHome = 4;
            for (int i = 0; i < tamañoListas; i++)
            {
                if (Canvas.GetTop(_listaEllipseRoja[i]) == POSICION_FINAL_HOME)
                {
                    contadorRojo++;
                }
                if (Canvas.GetBottom(_listaEllipseAzul[i]) == POSICION_FINAL_HOME)
                {
                    contadorAzul++;
                }
                if (Canvas.GetRight(_listaEllipseVerde[i]) == POSICION_FINAL_HOME)
                {
                    contadorVerde++;
                }
                if (Canvas.GetLeft(_listaEllipseAmarilla[i]) == POSICION_FINAL_HOME)
                {
                    contadorAmarillo++;
                }
            }
            if (contadorRojo == fichasEnHome || contadorAzul == fichasEnHome || contadorVerde == fichasEnHome || contadorAmarillo == fichasEnHome) 
            {
                Console.WriteLine("Contador Rojo " + contadorRojo);
                Console.WriteLine("Contador Rojo " + contadorAzul);
                Console.WriteLine("Contador Rojo " + contadorVerde);
                Console.WriteLine("Contador Rojo " + contadorAmarillo);

                //
                //Guardar datos de cada jugador en la bd
                //
            }
        }

        //Metodo mover ficha compañero, que ocupe el findname y lo mueva segun el int
    }
}
