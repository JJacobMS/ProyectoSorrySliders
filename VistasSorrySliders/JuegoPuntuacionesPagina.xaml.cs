using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.ServiceModel;
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
    public partial class JuegoPuntuacionesPagina : Page, IJuegoPuntuacionCallback
    {
        private string _correoUsuario;
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
        private JuegoPuntuacionClient _proxyJuegoPuntuacion;
        private string _codigoPartida;
        private List<JugadorTurno> _listaTurnos;
        private List<JugadorGanador> _listaPuntuaciones;
        private List<(string, string, int)> _listaDesordenada;
        private CuentaSet _cuentaUsuario;
        private List<CuentaSet> _listaCuentas;
        private JuegoYLobbyVentana _juegoYLobbyVentana;
        private List<JugadorGanador> _listaGanadores;
        private int _contadorAvisarTurno;
        public JuegoPuntuacionesPagina(List<JugadorLanzamiento> jugadores, CuentaSet cuentaUsuario, string codigoPartida, List<CuentaSet> listaCuentas, JuegoYLobbyVentana juegoYLobbyVentana)
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
            _listaCuentas = listaCuentas;
            _juegoYLobbyVentana = juegoYLobbyVentana;
            _juegoYLobbyVentana.EliminarContexto += NotificarEliminarJugadorServidor;
            _listaTurnos = new List<JugadorTurno>();
            _codigoPartida = codigoPartida;
            _cuentaUsuario = cuentaUsuario;
            _correoUsuario = cuentaUsuario.CorreoElectronico;
            _jugadoresLanzamiento = jugadores;
            _contadorAvisarTurno = 0;
            InicializarTablero();
            _turnoActualJuego = 0;
            EmpezarTurno(_turnoActualJuego);
        }
        public Constantes AgregarProxy() 
        {
            Logger log = new Logger(this.GetType());
            try
            {
                InstanceContext contexto = new InstanceContext(this);
                _proxyJuegoPuntuacion = new JuegoPuntuacionClient(contexto);
                _proxyJuegoPuntuacion.AgregarJugador(_codigoPartida, _correoUsuario);
                return Constantes.OPERACION_EXITOSA;
            }
            catch (CommunicationException ex)
            {
                Utilidades.MostrarUnMensajeError(Properties.Resources.msgErrorConexion);
                log.LogError("Error de Comunicación con el Servidor", ex);
            }
            catch (TimeoutException ex)
            {
                Utilidades.MostrarUnMensajeError(Properties.Resources.msgErrorTiempoEsperaServidor);
                log.LogWarn("Se agoto el tiempo de espera del servidor", ex);
            }
            return Constantes.ERROR_CONEXION_SERVIDOR;
        }
        private void AsignarDados()
        {
            switch (_jugadoresLanzamiento.Count)
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
            switch (_jugadoresLanzamiento.Count)
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
            switch (_jugadoresLanzamiento.Count)
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
            string puntuacionUltima = Properties.Resources.lblPuntuacionUltimaJugada;
            switch (_jugadoresLanzamiento.Count)
            {
                case 2:
                    lblJugadorAzul.Content = _jugadoresLanzamiento[0].Nickname;
                    lblPuntuacionJugadorAzul.Content = puntuacionUltima + " " + CalcularPuntuacion(_jugadoresLanzamiento[0].Puntuaciones);
                    lblJugadorRojo.Content = _jugadoresLanzamiento[1].Nickname;
                    lblPuntuacionJugadorRojo.Content = puntuacionUltima+ " " + CalcularPuntuacion(_jugadoresLanzamiento[1].Puntuaciones);
                    break;
                case 3:
                    lblJugadorAzul.Content = _jugadoresLanzamiento[0].Nickname;
                    lblPuntuacionJugadorAzul.Content = puntuacionUltima+ " " + CalcularPuntuacion(_jugadoresLanzamiento[0].Puntuaciones);
                    lblJugadorVerde.Content = _jugadoresLanzamiento[1].Nickname;
                    lblPuntuacionJugadorVerde.Content = puntuacionUltima+ " " +CalcularPuntuacion(_jugadoresLanzamiento[1].Puntuaciones);
                    lblJugadorAmarillo.Content = _jugadoresLanzamiento[2].Nickname;
                    lblPuntuacionJugadorAmarillo.Content = puntuacionUltima+ " " + CalcularPuntuacion(_jugadoresLanzamiento[2].Puntuaciones);
                    break;
                case 4:
                    lblJugadorAzul.Content = _jugadoresLanzamiento[0].Nickname;
                    lblPuntuacionJugadorAzul.Content = puntuacionUltima + " " + CalcularPuntuacion(_jugadoresLanzamiento[0].Puntuaciones);
                    lblJugadorVerde.Content = _jugadoresLanzamiento[1].Nickname;
                    lblPuntuacionJugadorVerde.Content = puntuacionUltima + " " + CalcularPuntuacion(_jugadoresLanzamiento[1].Puntuaciones);
                    lblJugadorRojo.Content = _jugadoresLanzamiento[2].Nickname;
                    lblPuntuacionJugadorRojo.Content = puntuacionUltima + " " + CalcularPuntuacion(_jugadoresLanzamiento[2].Puntuaciones);
                    lblJugadorAmarillo.Content = _jugadoresLanzamiento[3].Nickname;
                    lblPuntuacionJugadorAmarillo.Content = puntuacionUltima + " " + CalcularPuntuacion(_jugadoresLanzamiento[3].Puntuaciones);
                    break;
                default:
                    break;
            }
        }
        private void MostrarComponentesJuego()
        {
            switch (_jugadoresLanzamiento.Count)
            {
                case 2:
                    MostrarComponentesAzul();
                    MostrarComponentesRojo();
                    break;
                case 3:
                    MostrarComponentesAzul();
                    MostrarComponentesVerde();
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
        private void AsignarTurno()
        {
            for (int i = 0; i < _jugadoresLanzamiento.Count; i++)
            {
                JugadorTurno jugadorTurno = new JugadorTurno()
                {
                    CorreoJugador = _jugadoresLanzamiento[i].CorreElectronico,
                    TurnoJugador = i,
                    EstaConectado = _jugadoresLanzamiento[i].EstaConectado,
                    Nickname = _jugadoresLanzamiento[i].Nickname
                };
                _listaTurnos.Add(jugadorTurno);
                if (jugadorTurno.CorreoJugador == _correoUsuario) 
                {
                    _turnoJugador = jugadorTurno.TurnoJugador;
                }
            }
        }
        private int CalcularPuntuacion(List<int> puntuaciones)
        {
            int puntuacion = 0;
            for (int i = 0; i < puntuaciones.Count; i++)
            {
                puntuacion = puntuacion + puntuaciones[i];
            }
            return puntuacion;
        }
        private void InicializarDados()
        {
            switch (_jugadoresLanzamiento.Count)
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
            
            for (int i = 0; i < puntuaciones.Count; i++)
            {
                _listaBotonAzul[i].Content = puntuaciones[i];
            }
        }

        private void IngresarPuntuacionesRojo(List<int> puntuaciones)
        {
            for (int i = 0; i < puntuaciones.Count; i++)
            {
                _listaBotonRojo[i].Content = puntuaciones[i];
            }
        }
        private void IngresarPuntuacionesVerde(List<int> puntuaciones)
        {
            for (int i = 0; i < puntuaciones.Count; i++)
            {
                _listaBotonVerde[i].Content = puntuaciones[i];
            }
        }
        private void IngresarPuntuacionesAmarillo(List<int> puntuaciones)
        {
            for (int i = 0; i < puntuaciones.Count; i++)
            {
                _listaBotonAmarillo[i].Content = puntuaciones[i];
            }
        }
        private void MouseLeftButtonDownMoverPeon(object sender, MouseButtonEventArgs e)
        {
            RealizarJugada(sender);
        }
        private async void RealizarJugada(object sender) 
        {
            await MoverPeonAsync(sender);
            if (!ComprobarDadosActuales() && _contadorAvisarTurno==0)
            {
                NotificarCambiarTurno();
                _contadorAvisarTurno = _contadorAvisarTurno + 1;
            }
        }
        private async Task MoverPeonAsync(object sender)
        {
            if (_btnSeleccionado != null)
            {
                Ellipse llpSeleccionada = sender as Ellipse;
                int puntosObtenidos = ObtenerValorBoton(_btnSeleccionado);
                if (puntosObtenidos >= 0)
                {
                    //Settear enable false todos los demas botones
                    _btnSeleccionado.IsEnabled = false;
                    _btnSeleccionado.BorderBrush = Brushes.Black;
                    _btnSeleccionado = null;
                    for (int i = 0; i < puntosObtenidos; i++)
                    {
                        llpSeleccionada.IsEnabled = false;
                        double posicion = ObtenerPosicionFicha(llpSeleccionada.Name);
                        if (posicion < LIMITE_FILAS)
                        {
                            SettearPosicionFicha(llpSeleccionada.Name, posicion + AVANZAR_UNA_FILA);
                            await Task.Delay(1000);
                            llpSeleccionada.IsEnabled = true;
                        }
                        else
                        {
                            SettearPosicionFicha(llpSeleccionada.Name, POSICION_FINAL_HOME);
                            llpSeleccionada.IsEnabled = false;
                            ComprobarGanador();
                            i = puntosObtenidos;
                        }
                    }
                    await NotificarJugadores(llpSeleccionada, puntosObtenidos);
                    //Des settear
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

        private void ClickObtenerBoton(object sender, RoutedEventArgs e)
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
                OrdenarPuntuaciones(contadorRojo, contadorAzul, contadorVerde, contadorAmarillo);
            }
        }

        private void OrdenarPuntuaciones(int contadorRojo, int contadorAzul, int contadorVerde, int contadorAmarillo) 
        {
            _listaDesordenada = new List<(string, string, int)>();
            switch (_jugadoresLanzamiento.Count)
            {
                case 2:
                    _listaDesordenada.Add((_jugadoresLanzamiento[0].CorreElectronico, _jugadoresLanzamiento[0].Nickname, contadorAzul));
                    _listaDesordenada.Add((_jugadoresLanzamiento[1].CorreElectronico, _jugadoresLanzamiento[1].Nickname, contadorRojo));
                    break;
                case 3:
                    _listaDesordenada.Add((_jugadoresLanzamiento[0].CorreElectronico, _jugadoresLanzamiento[0].Nickname, contadorAzul));
                    _listaDesordenada.Add((_jugadoresLanzamiento[1].CorreElectronico, _jugadoresLanzamiento[1].Nickname, contadorVerde));
                    _listaDesordenada.Add((_jugadoresLanzamiento[2].CorreElectronico, _jugadoresLanzamiento[2].Nickname, contadorAmarillo));
                    break;
                case 4:
                    _listaDesordenada.Add((_jugadoresLanzamiento[0].CorreElectronico, _jugadoresLanzamiento[0].Nickname, contadorAzul));
                    _listaDesordenada.Add((_jugadoresLanzamiento[1].CorreElectronico, _jugadoresLanzamiento[1].Nickname, contadorVerde));
                    _listaDesordenada.Add((_jugadoresLanzamiento[2].CorreElectronico, _jugadoresLanzamiento[2].Nickname, contadorRojo));
                    _listaDesordenada.Add((_jugadoresLanzamiento[3].CorreElectronico, _jugadoresLanzamiento[3].Nickname, contadorAmarillo));
                    break;
                default:
                    break;
            }
            _listaDesordenada = _listaDesordenada.OrderByDescending(contador => contador.Item3).ToList();
            IngresarRelacionPuntuacion(_listaDesordenada);
        }

        private void IngresarRelacionPuntuacion(List<(string, string, int)> listaOrdenada) 
        {
            Dictionary<int, (string, string, int)[]> diccionarioPuntuaciones = new Dictionary<int, (string, string, int)[]>();
            _listaPuntuaciones = new List<JugadorGanador>();
            for (int i = 0; i < listaOrdenada.Count; i++)
            {
                JugadorGanador jugadorGanador = new JugadorGanador()
                {
                    CorreoElectronico = listaOrdenada[i].Item1,
                    Nickname = listaOrdenada[i].Item2,
                    Posicion = i + 1
                };
                _listaPuntuaciones.Add(jugadorGanador);
                if (i > 0 && listaOrdenada[i].Item3 == listaOrdenada[i - 1].Item3)
                {
                    jugadorGanador.Posicion = _listaPuntuaciones[i - 1].Posicion;
                    _listaPuntuaciones[i] = jugadorGanador;
                }
            }
            if (_listaPuntuaciones[0].CorreoElectronico == _correoUsuario) 
            {
                if (!_juegoYLobbyVentana.EsInvitado) 
                {
                    GuardarGanador();
                }
                NotificarCambiarPantalla(_listaPuntuaciones);
            }
            
        }

        private void GuardarGanador() 
        {
            Logger log = new Logger(this.GetType());
            Constantes respuesta;
            try
            {
                _proxyJuegoPuntuacion.ActualizarGanador(_codigoPartida, _listaPuntuaciones[0].CorreoElectronico, _listaPuntuaciones[0].Posicion);
                respuesta = Constantes.OPERACION_EXITOSA;
            }
            catch (CommunicationException ex)
            {
                log.LogError("Error de Comunicación con el Servidor", ex);
                respuesta = Constantes.ERROR_CONEXION_SERVIDOR;
            }
            catch (TimeoutException ex)
            {
                log.LogWarn("Se agoto el tiempo de espera del servidor", ex);
                respuesta = Constantes.ERROR_TIEMPO_ESPERA_SERVIDOR;
            }
            switch (respuesta)
            {
                case Constantes.ERROR_CONEXION_SERVIDOR:
                    Utilidades.SalirHastaInicioSesionDesdeJuegoYLobbyVentana(this);
                    break;
                case Constantes.OPERACION_EXITOSA:
                    break;
                case Constantes.ERROR_TIEMPO_ESPERA_SERVIDOR:
                    Utilidades.SalirHastaInicioSesionDesdeJuegoYLobbyVentana(this);
                    break;
                default:
                    Utilidades.MostrarMensajesError(respuesta);
                    SalirMenuPrincipal();
                    break;
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
                _contadorAvisarTurno = 0;
                ActivarDados();
                if (!ComprobarDadosActuales())
                {
                    NotificarCambiarTurno();
                }
                else
                {
                    ActivarEllipses();
                }
            }
            else
            {
                foreach (JugadorTurno jugador in _listaTurnos)
                {
                    if (jugador.TurnoJugador == turnoActual && !jugador.EstaConectado) 
                    {
                        CambiarTurno();
                    }
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
            bool desactivarEllipse = false;
            foreach (Ellipse ellipse in _diccionarioEllipses[_correoUsuario])
            {
                if (ObtenerPosicionFicha(ellipse.Name) < POSICION_FINAL_HOME)
                {
                    ellipse.IsEnabled = activarEllipse;
                }
                else 
                {
                    ellipse.IsEnabled = desactivarEllipse;
                }
            }
        }

        
        public void EmpezarJuegoPuntuacionNuevo(List<JugadorLanzamiento> jugadores) 
        {
            _contadorAvisarTurno = 0;
            _turnoActualJuego = 0;
            _jugadoresLanzamiento = jugadores;
            InicializarLabels();
            InicializarDados();
            ActualizarEstadoJugadores(jugadores);
            EmpezarTurno(_turnoActualJuego);
        }

        private void ActualizarEstadoJugadores(List<JugadorLanzamiento> jugadores)
        {
            foreach (JugadorLanzamiento jugadorLanzamiento in jugadores)
            {
                foreach (JugadorTurno jugadorTurno in _listaTurnos)
                {
                    if (jugadorTurno.CorreoJugador.Equals(jugadorLanzamiento.CorreElectronico))
                    {
                        jugadorTurno.EstaConectado = jugadorLanzamiento.EstaConectado;
                    }
                }
            }
        }

        public void NotificarJugadorMovimiento(string nombrePeon, int puntosObtenidos)
        {
            MoverPeonJugador(nombrePeon, puntosObtenidos);
        }

        private async void NotificarCambiarTurno() 
        {
            await LogicaNotificarCambiarTurno();
        }

        private async Task LogicaNotificarCambiarTurno() 
        {
            await Task.Delay(3000);
            Logger log = new Logger(this.GetType());
            try
            {
                _proxyJuegoPuntuacion.NotificarCambioTurno(_codigoPartida);
                return;
            }
            catch (CommunicationException ex)
            {
                Utilidades.MostrarUnMensajeError(Properties.Resources.msgErrorConexion);
                log.LogError("Error de Comunicación con el Servidor", ex);
            }
            catch (TimeoutException ex)
            {
                Utilidades.MostrarUnMensajeError(Properties.Resources.msgErrorTiempoEsperaServidor);
                log.LogWarn("Se agoto el tiempo de espera del servidor", ex);
            }
            Utilidades.SalirHastaInicioSesionDesdeJuegoYLobbyVentana(this);
        }

        private void MoverPeonJugador(string nombrePeon, int puntosObtenidos) 
        {
            if (puntosObtenidos >= 0)
            {
                for (int i = 0; i < puntosObtenidos; i++)
                {
                    double posicion = ObtenerPosicionFicha(nombrePeon);
                    if (posicion < LIMITE_FILAS)
                    {
                        SettearPosicionFicha(nombrePeon, posicion + AVANZAR_UNA_FILA);
                    }
                    else
                    {
                        SettearPosicionFicha(nombrePeon, POSICION_FINAL_HOME);
                    }
                }
            }
        }

        private double ObtenerPosicionFicha(string nombreEllipse) 
        {
            foreach (Ellipse llpFicha in _listaEllipseRojo)
            {
                if (llpFicha.Name == nombreEllipse) 
                {
                    return Canvas.GetTop(llpFicha);
                }
            }
            foreach (Ellipse llpFicha in _listaEllipseAzul)
            {
                if (llpFicha.Name == nombreEllipse)
                {
                    return Canvas.GetBottom(llpFicha);
                }
            }
            foreach (Ellipse llpFicha in _listaEllipseVerde)
            {
                if (llpFicha.Name == nombreEllipse)
                {
                    return Canvas.GetRight(llpFicha);
                }
            }
            foreach (Ellipse llpFicha in _listaEllipseAmarillo)
            {
                if (llpFicha.Name == nombreEllipse)
                {
                    return Canvas.GetLeft(llpFicha);
                }
            }
            double posicionErronea = 0.0;
            return posicionErronea;
        }

        private void SettearPosicionFicha(string nombreEllipse, double posicion)
        {
            foreach (Ellipse llpFicha in _listaEllipseRojo)
            {
                if (llpFicha.Name == nombreEllipse)
                {
                    Canvas.SetTop(llpFicha, posicion);
                }
            }
            foreach (Ellipse llpFicha in _listaEllipseAzul)
            {
                if (llpFicha.Name == nombreEllipse)
                {
                    Canvas.SetBottom(llpFicha, posicion);
                }
            }
            foreach (Ellipse llpFicha in _listaEllipseVerde)
            {
                if (llpFicha.Name == nombreEllipse)
                {
                    Canvas.SetRight(llpFicha, posicion);
                }
            }
            foreach (Ellipse llpFicha in _listaEllipseAmarillo)
            {
                if (llpFicha.Name == nombreEllipse)
                {
                    Canvas.SetLeft(llpFicha, posicion);
                }
            }
        }
        private async Task NotificarJugadores(Ellipse llpSeleccionada, int puntosObtenidos)
        {
            await Task.Delay(1500);
            Logger log = new Logger(this.GetType());
            try
            {
                _proxyJuegoPuntuacion.NotificarJugadores(_codigoPartida,_correoUsuario, llpSeleccionada.Name, puntosObtenidos);
                return;
            }
            catch (CommunicationException ex)
            {
                Utilidades.MostrarUnMensajeError(Properties.Resources.msgErrorConexion);
                log.LogError("Error de Comunicación con el Servidor", ex);
            }
            catch (TimeoutException ex)
            {
                Utilidades.MostrarUnMensajeError(Properties.Resources.msgErrorTiempoEsperaServidor);
                log.LogWarn("Se agoto el tiempo de espera del servidor", ex);
            } 
            Utilidades.SalirHastaInicioSesionDesdeJuegoYLobbyVentana(this);
        }

        public void CambiarTurno()
        {
            LogicaCambiarTurno();
        }

        private async void LogicaCambiarTurno() 
        {
            _turnoActualJuego = _turnoActualJuego + 1;
            if (_turnoActualJuego > _listaTurnos.Count - 1)
            {
                _turnoActualJuego = 0;
                await CambiarPaginaLanzamiento();
            }
            else
            {
                EmpezarTurno(_turnoActualJuego);
            }
            InicializarLabelTurno();
        }
        private async Task CambiarPaginaLanzamiento()
        {
            JuegoLanzamientoPagina paginaJuego = new JuegoLanzamientoPagina(_listaCuentas, _listaCuentas.Count, _codigoPartida, _cuentaUsuario, this, _juegoYLobbyVentana, _listaTurnos);
            Constantes respuestaInicio = paginaJuego.InicializarConexionYJuego();
            brdConteoPuntuaciones.Visibility = Visibility.Visible;
            await Task.Delay(5000);
            brdConteoPuntuaciones.Visibility = Visibility.Hidden;
            switch (respuestaInicio)
            {
                case Constantes.OPERACION_EXITOSA:
                    _juegoYLobbyVentana.CambiarFrameLobby(paginaJuego);
                    break;
                case Constantes.ERROR_CONEXION_SERVIDOR:
                    Utilidades.SalirHastaInicioSesionDesdeJuegoYLobbyVentana(this);
                    break;
            }
        }
        private void InicializarLabelTurno() 
        {
            foreach (var jugadorTurno in _listaTurnos)
            {
                if (jugadorTurno.TurnoJugador == _turnoActualJuego) 
                {
                    lblTurnoJugador.Content = Properties.Resources.lblTurnoJugador + " "+ jugadorTurno.Nickname;

                }
            }
        }
        public void EliminarTurnoJugador(string correoElectronico)
        {
            bool estaConectado = false;
            foreach (JugadorTurno jugadorTurno in _listaTurnos)
            {
                if (jugadorTurno.CorreoJugador == correoElectronico)
                {
                    jugadorTurno.EstaConectado = estaConectado;
                }
            }
            if (VerificarNumeroJugadoresValido())
            {
                VerificarTurnoSalido();
            }
        }
        private bool VerificarNumeroJugadoresValido() 
        {
            int contadorConectados = 0;
            foreach (JugadorTurno jugadorTurno in _listaTurnos)
            {
                if (jugadorTurno.EstaConectado)
                {
                    contadorConectados = contadorConectados + 1;
                }
            }
            if (contadorConectados == 1) 
            {
                SalirMenuPrincipal();
                return false;
            }
            return true;
        }
        private void VerificarTurnoSalido() 
        {
            foreach (JugadorTurno jugadorTurno in _listaTurnos)
            {
                if (!jugadorTurno.EstaConectado && jugadorTurno.TurnoJugador == _turnoActualJuego)
                {
                    CambiarTurno();
                }
            }
        }
        private void SalirMenuPrincipal() 
        {
            if (Window.GetWindow(this) != null)
            {
                Window.GetWindow(this).Close();
                Utilidades.MostrarUnMensajeError(Properties.Resources.msgFaltaJugadores);
            }
        }
        private void NotificarCambiarPantalla(List<JugadorGanador> listaPuntuaciones)
        {
            int[] arrayPosiciones = new int[listaPuntuaciones.Count];
            string[] arrayNickname = new string[listaPuntuaciones.Count];
            for (int i = 0; i < listaPuntuaciones.Count; i++)
            {
                arrayNickname[i] = listaPuntuaciones[i].Nickname;
                arrayPosiciones[i] = listaPuntuaciones[i].Posicion;
            }
            Logger log = new Logger(this.GetType());
            try
            {
                _proxyJuegoPuntuacion.NotificarCambiarPagina(_codigoPartida, arrayPosiciones, arrayNickname);
                return;
            }
            catch (CommunicationException ex)
            {
                Utilidades.MostrarUnMensajeError(Properties.Resources.msgErrorConexion);
                log.LogError("Error de Comunicación con el Servidor", ex);
            }
            catch (TimeoutException ex)
            {
                Utilidades.MostrarUnMensajeError(Properties.Resources.msgErrorTiempoEsperaServidor);
                log.LogWarn("Se agoto el tiempo de espera del servidor", ex);
            }
            Utilidades.SalirHastaInicioSesionDesdeJuegoYLobbyVentana(this);
        }
        private bool EliminarDiccionarios() 
        {
            Logger log = new Logger(this.GetType());
            try
            {
                _proxyJuegoPuntuacion.EliminarDiccionariosJuego(_codigoPartida);
                return true;
            }
            catch (CommunicationException ex)
            {
                Utilidades.MostrarUnMensajeError(Properties.Resources.msgErrorConexion);
                log.LogError("Error de Comunicación con el Servidor", ex);
            }
            catch (TimeoutException ex)
            {
                Utilidades.MostrarUnMensajeError(Properties.Resources.msgErrorTiempoEsperaServidor);
                log.LogWarn("Se agoto el tiempo de espera del servidor", ex);
            }
            Utilidades.SalirHastaInicioSesionDesdeJuegoYLobbyVentana(this);
            return false;
        }
        private void NotificarEliminarJugadorServidor()
        {
            Logger log = new Logger(this.GetType());
            try
            {
                _proxyJuegoPuntuacion.EliminarJugador(_codigoPartida, _correoUsuario);
                return;
            }
            catch (CommunicationException ex)
            {
                Utilidades.MostrarUnMensajeError(Properties.Resources.msgErrorConexion);
                log.LogError("Error de Comunicación con el Servidor", ex);
            }
            catch (TimeoutException ex)
            {
                Utilidades.MostrarUnMensajeError(Properties.Resources.msgErrorTiempoEsperaServidor);
                log.LogWarn("Se agoto el tiempo de espera del servidor", ex);
            }
            throw new CommunicationException();
        }

        public void CambiarPagina(int[] arrayPosiciones, string[] arrayNickname)
        {
            AcomodarPosiciones(arrayPosiciones, arrayNickname);
            if (EliminarDiccionarios())
            {
                _juegoYLobbyVentana.CambiarVentanaGanadores(_listaGanadores);
            }
        }
        private void AcomodarPosiciones(int[] arrayPosiciones, string[] arrayNickname) 
        {
            _listaGanadores = new List<JugadorGanador>();

            for (int i = 0; i < arrayPosiciones.Count(); i++)
            {
                JugadorGanador jugador = new JugadorGanador()
                {
                    Nickname = arrayNickname[i],
                    Posicion = arrayPosiciones[i]
                };
                _listaGanadores.Add(jugador);
            }
        }
    }
}
