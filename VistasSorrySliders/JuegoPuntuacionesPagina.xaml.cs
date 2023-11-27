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
        private Dictionary<string, List<Ellipse>> _diccionarioEllipses = new Dictionary<string, List<Ellipse>>();
        private Dictionary<string, List<Button>> _diccionarioBotones = new Dictionary<string, List<Button>>();
        private List<JugadorLanzamiento> _jugadoresLanzamiento;
        private List<Ellipse> _listaEllipseRojo;
        private List<Ellipse> _listaEllipseAmarillo;
        private List<Ellipse> _listaEllipseVerde;
        private List<Ellipse> _listaEllipseAzul;
        private List<Button> _listaBotonRojo;
        private List<Button> _listaBotonAmarillo;
        private List<Button> _listaBotonVerde;
        private List<Button> _listaBotonAzul;
        private Button _btnAnterior;
        private Button _btnSeleccionado;
        private int _turnoActualJuego;
        private int _turnoJugador;

        /*SOLO VOLVER ENABLE SI ES EL JUGADOR -- SI ES SU TURNO
        SI una puntuacion es 0 entonces volver enabled ese boton
        VALIDAR QUE LAS 4 FICHAS YA LLEGARON
        COMUNICAR POSICIONES A LOS DEMAS CADA QUE MUEVA UNA FICHA
        GUARDAR DATOS EN BD
        asignar lista ellipse Y HACERLO ENABLE TRUE
        Hacer ellipses FALSE EN ENABLED
         */


        public JuegoPuntuacionesPagina(List<JugadorLanzamiento> jugadores, string correo)
        {
            InitializeComponent();

            _listaEllipseRojo = new List<Ellipse> { llpPeonRojo1, llpPeonRojo2, llpPeonRojo3, llpPeonRojo4 };
            _listaEllipseAmarillo = new List<Ellipse> { llpPeonAmarillo1, llpPeonAmarillo2, llpPeonAmarillo3, llpPeonAmarillo4 };
            _listaEllipseVerde = new List<Ellipse> { llpPeonVerde1, llpPeonVerde2, llpPeonVerde3, llpPeonVerde4 };
            _listaEllipseAzul = new List<Ellipse> { llpPeonAzul1, llpPeonAzul2, llpPeonAzul3, llpPeonAzul4 };

            _listaBotonRojo = new List<Button> { btnPuntuacionRojo1, btnPuntuacionRojo2, btnPuntuacionRojo3, btnPuntuacionRojo4 };
            _listaBotonAmarillo = new List<Button> { btnPuntuacionAmarillo1, btnPuntuacionAmarillo2, btnPuntuacionAmarillo3, btnPuntuacionAmarillo4 };
            _listaBotonVerde = new List<Button> { btnPuntuacionVerde1, btnPuntuacionVerde2, btnPuntuacionVerde3, btnPuntuacionVerde4 };
            _listaBotonAzul = new List<Button> { btnPuntuacionAzul1, btnPuntuacionAzul2, btnPuntuacionAzul3, btnPuntuacionAzul4 };

            _correoUsuario = correo;
            _jugadoresLanzamiento = jugadores;
            Mostrar(jugadores);
            InicializarTablero();
            _turnoActualJuego = 0;
            //ActivarComponentesAzul();
            //ActivarComponentesRojo();
            //ActivarComponentesVerde();
            //ActivarComponentesAmarillo();

            EmpezarTurno(_turnoActualJuego);

            // (si es el primer jugador con el turno 0 y todos son enabled, que pase al siguiente, ese metodo va a servvir
            // para el notificacion, porque debe comprobar que el siguiente puede tirar)
        }
        private void AsignarDados()
        {
            switch (_jugadoresLanzamiento.Count())
            {
                case 2:
                    _diccionarioBotones.Add(_jugadoresLanzamiento[0].CorreElectronico, _listaBotonAzul);
                    _diccionarioBotones.Add(_jugadoresLanzamiento[1].CorreElectronico, _listaBotonRojo);
                    break;
                case 3:
                    _diccionarioBotones.Add(_jugadoresLanzamiento[0].CorreElectronico, _listaBotonAzul);
                    _diccionarioBotones.Add(_jugadoresLanzamiento[1].CorreElectronico, _listaBotonVerde);
                    _diccionarioBotones.Add(_jugadoresLanzamiento[2].CorreElectronico, _listaBotonAmarillo);
                    break;
                case 4:
                    _diccionarioBotones.Add(_jugadoresLanzamiento[0].CorreElectronico, _listaBotonAzul);
                    _diccionarioBotones.Add(_jugadoresLanzamiento[1].CorreElectronico, _listaBotonVerde);
                    _diccionarioBotones.Add(_jugadoresLanzamiento[2].CorreElectronico, _listaBotonRojo);
                    _diccionarioBotones.Add(_jugadoresLanzamiento[3].CorreElectronico, _listaBotonAmarillo);
                    break;
                default:
                    break;
            }
        }

        private void AsignarPeones()
        {
            switch (_jugadoresLanzamiento.Count())
            {
                case 2:
                    _diccionarioEllipses.Add(_jugadoresLanzamiento[0].CorreElectronico, _listaEllipseAzul);
                    _diccionarioEllipses.Add(_jugadoresLanzamiento[1].CorreElectronico, _listaEllipseRojo);
                    break;
                case 3:
                    _diccionarioEllipses.Add(_jugadoresLanzamiento[0].CorreElectronico, _listaEllipseAzul);
                    _diccionarioEllipses.Add(_jugadoresLanzamiento[1].CorreElectronico, _listaEllipseVerde);
                    _diccionarioEllipses.Add(_jugadoresLanzamiento[2].CorreElectronico, _listaEllipseAmarillo);
                    break;
                case 4:
                    _diccionarioEllipses.Add(_jugadoresLanzamiento[0].CorreElectronico, _listaEllipseAzul);
                    _diccionarioEllipses.Add(_jugadoresLanzamiento[1].CorreElectronico, _listaEllipseVerde);
                    _diccionarioEllipses.Add(_jugadoresLanzamiento[2].CorreElectronico, _listaEllipseRojo);
                    _diccionarioEllipses.Add(_jugadoresLanzamiento[3].CorreElectronico, _listaEllipseAmarillo);
                    break;
                default:
                    break;
            }
        }

        private void Mostrar(List<JugadorLanzamiento> jugadores)
        {
        }
        private void InicializarTablero()
        {
            InicializarImagen();
            InicializarLabels();
            MostrarComponentesJuego();
            AsignarTurno();
            AsignarDados();
            AsignarPeones();
            InicializarDados();
        }

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

                    lblJugadorVerde.Content = _jugadoresLanzamiento[1].Nickname;
                    lblPuntuacionJugadorVerde.Content = "Puntuación Última Jugada: " + CalcularPuntuacion(_jugadoresLanzamiento[1].Puntuaciones);

                    lblJugadorAmarillo.Content = _jugadoresLanzamiento[2].Nickname;
                    lblPuntuacionJugadorAmarillo.Content = "Puntuación Última Jugada: " + CalcularPuntuacion(_jugadoresLanzamiento[2].Puntuaciones);

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
        private void MostrarComponentesJuego()
        {
            switch (_jugadoresLanzamiento.Count())
            {
                case 2:
                    //OTRO METODO PARA ACTIVAR, ahí debo poner un metodo para _diccionarioEllipses y _diccionarioBotones
                    //Activar si es el 0 y es el turno de 0
                    MostrarComponentesAzul();
                    //Activar si es el 1 y es el turno de 1
                    MostrarComponentesRojo();
                    break;
                case 3:
                    //Activar si es el 0...
                    MostrarComponentesAzul();
                    //Activar si es el 2...
                    MostrarComponentesVerde();
                    //Activar si es el 1...
                    MostrarComponentesAmarillo();
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

        private void AsignarTurno()
        {
            for (int i = 0; i < _jugadoresLanzamiento.Count(); i++)
            {
                if (_jugadoresLanzamiento[i].CorreElectronico == _correoUsuario)
                {
                    _turnoJugador = i;
                }
            }
        }

        private int CalcularPuntuacion(List<int> puntuaciones)
        {
            int puntuacion = 0;
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
                    IngresarPuntuacionesVerde(_jugadoresLanzamiento[1].Puntuaciones);
                    IngresarPuntuacionesAmarillo(_jugadoresLanzamiento[2].Puntuaciones);
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
            /*
            for (int i = 0; i < puntuaciones.Count(); i++)
            {
                _listaBotonAzul[i].Content = puntuaciones[i];
            }
            */
            btnPuntuacionAzul1.Content = 1;//puntuaciones[0];
            btnPuntuacionAzul2.Content = 0;// puntuaciones[1];
            btnPuntuacionAzul3.Content = 1;// puntuaciones[2];
            btnPuntuacionAzul4.Content = 0;// puntuaciones[3];
        }

        private void IngresarPuntuacionesRojo(List<int> puntuaciones)
        {
            /*
            for (int i = 0; i < puntuaciones.Count(); i++)
            {
                _listaBotonRojo[i].Content = puntuaciones[i];
            }
            */
            btnPuntuacionRojo1.Content = 1;// puntuaciones[0];
            btnPuntuacionRojo2.Content = 0;// puntuaciones[1];
            btnPuntuacionRojo3.Content = 1;// puntuaciones[2];
            btnPuntuacionRojo4.Content = 0;// puntuaciones[3];
        }
        private void IngresarPuntuacionesVerde(List<int> puntuaciones)
        {
            /*
            for (int i = 0; i < puntuaciones.Count(); i++)
            {
                _listaBotonVerde[i].Content = puntuaciones[i];
            }
            */
            btnPuntuacionVerde1.Content = 1;//puntuaciones[0];
            btnPuntuacionVerde2.Content = 0;//puntuaciones[1];
            btnPuntuacionVerde3.Content = 1;//puntuaciones[2];
            btnPuntuacionVerde4.Content = 0;//puntuaciones[3];
        }
        private void IngresarPuntuacionesAmarillo(List<int> puntuaciones)
        {
            /*
            for (int i = 0; i < puntuaciones.Count(); i++)
            {
                _listaBotonAmarillo[i].Content = puntuaciones[i];
            }
            */
            btnPuntuacionAmarillo1.Content = 1;//puntuaciones[0];
            btnPuntuacionAmarillo2.Content = 0;//puntuaciones[1];
            btnPuntuacionAmarillo3.Content = 1;//puntuaciones[2];
            btnPuntuacionAmarillo4.Content = 0;//puntuaciones[3];
        }

        private void MouseLeftButtonDownPeonRojo(object sender, MouseButtonEventArgs e)
        {
            MoverPeonRojoAsync(sender);
        }
        private async void MoverPeonRojoAsync(object sender)
        {
            if (_btnSeleccionado != null)
            {
                Ellipse llpSeleccionada = sender as Ellipse;
                int puntosObtenidos = ObtenerValorBoton(_btnSeleccionado);
                if (puntosObtenidos >= 0)
                {
                    _btnSeleccionado.IsEnabled = false;
                    _btnSeleccionado = null;
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
            if (_btnSeleccionado != null)
            {
                Ellipse llpSeleccionada = sender as Ellipse;
                int puntosObtenidos = ObtenerValorBoton(_btnSeleccionado);
                if (puntosObtenidos >= 0)
                {
                    _btnSeleccionado.IsEnabled = false;
                    _btnSeleccionado = null;
                    for (int i = 0; i < puntosObtenidos; i++)
                    {
                        double posicion = Canvas.GetBottom(llpSeleccionada);

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
            if (_btnSeleccionado != null)
            {
                Ellipse llpSeleccionada = sender as Ellipse;
                int puntosObtenidos = ObtenerValorBoton(_btnSeleccionado);
                if (puntosObtenidos >= 0)
                {
                    _btnSeleccionado.IsEnabled = false;
                    _btnSeleccionado = null;
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
            if (_btnSeleccionado != null)
            {
                Ellipse llpSeleccionada = sender as Ellipse;
                int puntosObtenidos = ObtenerValorBoton(_btnSeleccionado);
                if (puntosObtenidos >= 0)
                {
                    Console.WriteLine("Puntos obtenidos " + puntosObtenidos);
                    _btnSeleccionado.IsEnabled = false;
                    _btnSeleccionado = null;
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

                            ComprobarGanador();
                        }
                        //ComprobarCambiarTurno Si todos los dados estan desabilitados CambiarTurno()
                        //NotificarJugador
                    }
                }
            }
        }

        private int ObtenerValorBoton(Button boton)
        {
            Logger log = new Logger(this.GetType());
            try
            {
                string puntosTextoBoton = boton.Content.ToString();
                int puntos = int.Parse(puntosTextoBoton);
                return puntos;
            }
            catch (FormatException ex)
            {
                log.LogError("Error al Parsear String a Int", ex);
                return -1;
            }
        }

        private void ClickBtnAmarillo(object sender, RoutedEventArgs e)
        {
            _btnSeleccionado = CambiarColorSeleccion(sender);
        }

        private void ClickBtnRojo(object sender, RoutedEventArgs e)
        {
            _btnSeleccionado = CambiarColorSeleccion(sender);
        }

        private void ClickBtnAzul(object sender, RoutedEventArgs e)
        {
            _btnSeleccionado = CambiarColorSeleccion(sender);
        }

        private void ClickBtnVerde(object sender, RoutedEventArgs e)
        {
            _btnSeleccionado = CambiarColorSeleccion(sender);
        }

        private Button CambiarColorSeleccion(object sender)
        {
            if (_btnAnterior != null)
            {
                _btnAnterior.BorderBrush = Brushes.Black;
            }
            Button btnSeleccionado = sender as Button;
            btnSeleccionado.BorderBrush = Brushes.Red;
            _btnAnterior = btnSeleccionado;
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
                if (Canvas.GetTop(_listaEllipseRojo[i]) == POSICION_FINAL_HOME)
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
                if (Canvas.GetLeft(_listaEllipseAmarillo[i]) == POSICION_FINAL_HOME)
                {
                    contadorAmarillo++;
                }
            }
            if (contadorRojo == fichasEnHome || contadorAzul == fichasEnHome || contadorVerde == fichasEnHome || contadorAmarillo == fichasEnHome)
            {
                Console.WriteLine("Contador Rojo " + contadorRojo);
                Console.WriteLine("Contador Azul " + contadorAzul);
                Console.WriteLine("Contador Verde " + contadorVerde);
                Console.WriteLine("Contador Amarillo " + contadorAmarillo);

                //
                //Guardar datos de cada jugador en la bd y pasar a la siguiente pantalla
                //
            }
        }

        private bool ComprobarDadosActuales()
        {
            int contadorBotones = 0;
            int totalBotones = 4;
            foreach (Button boton in _diccionarioBotones[_correoUsuario])
            {
                if (!boton.IsEnabled)
                {
                    contadorBotones = contadorBotones + 1;
                }
            }
            if (contadorBotones == totalBotones)
            {
                return false;
            }
            return true;

        }
        private void EmpezarTurno(int turnoActual)
        {
            if (turnoActual == _turnoJugador)
            {
                ActivarDados();
                if (!ComprobarDadosActuales())
                {
                    Console.WriteLine("CAMBIAR DE TURNO");
                }
                else
                {
                    ActivarEllipses();
                    Console.WriteLine("CONTINUA EL TURNO");
                }
            }
        }

        private void ActivarDados()
        {
            bool activarBoton = true;
            bool desactivarBoton = false;
            foreach (Button boton in _diccionarioBotones[_correoUsuario])
            {
                if (ObtenerValorBoton(boton) <= 0)
                {
                    boton.IsEnabled = desactivarBoton;
                    Console.WriteLine("Desabilitar Boton");
                }
                else
                {
                    boton.IsEnabled = activarBoton;
                }
            }
        }

        private void ActivarEllipses()
        {
            bool activarEllipse = true;
            foreach (Ellipse ellipse in _diccionarioEllipses[_correoUsuario])
            {
                Console.WriteLine("Activar ellipse "+ellipse.Name);
                ellipse.IsEnabled = activarEllipse;
            }
        }
        //Metodo mover ficha compañero, que ocupe el correo, int de ficha y lo mueva segun el int de movimiento

    }
}
