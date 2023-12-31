﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
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
    /// Lógica de interacción para JuegoLanzamientoPagina.xaml
    /// </summary>
    public partial class JuegoLanzamientoPagina : Page, IJuegoLanzamientoCallback
    {
        private JuegoYLobbyVentana _juegoYLobbyVentana;
        private Dictionary<Direccion, TextBlock> _etiquetasJugadoresLanzamientoPotencia;
        private Dictionary<Direccion, Button> _botonesLanzarPeonJugador;
        private Dictionary<Direccion, Button> _botonesTirarDadoJugador;
        private Tablero _tablero;
        private int _numeroJugadores;
        private IJuegoLanzamiento _proxyLanzamiento;
        private string _codigoPartida;
        private string _correoJugadorActual;
        private CuentaSet _cuentaUsuario;
        private List<CuentaSet> _listaCuentas;
        private JuegoPuntuacionesPagina _juegoPuntuacionPagina;
        public JuegoLanzamientoPagina(List<CuentaSet> listaCuentas, int numeroJugadores, string codigoPartida,CuentaSet cuentaUsuario, JuegoYLobbyVentana ventana)
        {
            InicializarJuegoLanzamiento(listaCuentas,  numeroJugadores,  codigoPartida, cuentaUsuario, ventana);
        }

        public JuegoLanzamientoPagina(List<CuentaSet> listaCuentas, int numeroJugadores, string codigoPartida, CuentaSet cuentaUsuario, JuegoPuntuacionesPagina juegoPuntuacionPagina, JuegoYLobbyVentana ventana, List<JugadorTurno> jugadoresPuntuaciones)
        {
            _juegoPuntuacionPagina = juegoPuntuacionPagina;
            InicializarJuegoLanzamiento(listaCuentas, numeroJugadores, codigoPartida, cuentaUsuario, ventana);
            _tablero.CambiarEstadosJugadores(jugadoresPuntuaciones);
        }

        private void InicializarJuegoLanzamiento(List<CuentaSet> listaCuentas, int numeroJugadores, string codigoPartida, CuentaSet cuentaUsuario, JuegoYLobbyVentana ventana) 
        {
            _listaCuentas = listaCuentas;
            _juegoYLobbyVentana = ventana;
            _juegoYLobbyVentana.EliminarContexto += EliminarContextoJuegoLanzamiento;
            _cuentaUsuario = cuentaUsuario;
            _correoJugadorActual = cuentaUsuario.CorreoElectronico;
            _codigoPartida = codigoPartida;
            _numeroJugadores = numeroJugadores;
            InitializeComponent();
            ColocarNombres(listaCuentas);
            MostrarTableroElementosCorrespondientes();

            _etiquetasJugadoresLanzamientoPotencia = new Dictionary<Direccion, TextBlock>
            {
                {Direccion.Arriba, txtBlockPotenciaRojo },
                {Direccion.Abajo, txtBlockPotenciaAzul },
                {Direccion.Izquierda, txtBlockPotenciaAmarillo } ,
                {Direccion.Derecha, txtBlockPotenciaVerde }
            };

            _botonesLanzarPeonJugador = new Dictionary<Direccion, Button>
            {
                { Direccion.Abajo, btnLanzarPeonAzul },
                { Direccion.Arriba, btnLanzarPeonRojo },
                { Direccion.Izquierda, btnLanzarPeonAmarillo },
                { Direccion.Derecha, btnLanzarPeonVerde }
            };
            _botonesTirarDadoJugador = new Dictionary<Direccion, Button>
            {
                { Direccion.Abajo, btnTirarDadoAzul },
                { Direccion.Arriba, btnTirarDadoRojo },
                { Direccion.Izquierda, btnTirarDadoAmarillo },
                { Direccion.Derecha, btnTirarDadoVerde }
            };

            InicializarTablero(listaCuentas);

            _tablero.IniciarJuego += IniciarTurno;
            _tablero.MostrarPotenciaLanzamiento += MostrarPotenciaLanzamiento;
            _tablero.TerminarTurno += EliminarLineaMovimientoJugador;
            _tablero.EliminarPeonTablero += EliminarPeonCanvas;
            _tablero.FinalizarMovimientoPeones += NotificarTurnoAcabado;
            _tablero.AcabarJuegoFaltaJugadores += TerminarJuegoFaltaJugadores;
            _tablero.PasarPuntuacionesJuego += PasarPuntuaciones;

            ColocarPiezasJugadores();
        }

        public Constantes InicializarConexionYJuego()
        {
            Logger log = new Logger(this.GetType());
            try
            {
                InicializarProxyJuegoLanzamiento();
            }
            catch (CommunicationException ex)
            {
                log.LogWarn("Error de Comunicación con el Servidor", ex);
                return Constantes.ERROR_CONEXION_SERVIDOR;
            }

            _tablero.IniciarTurno();
            return Constantes.OPERACION_EXITOSA;

        }

        private void InicializarProxyJuegoLanzamiento()
        {
            Logger log = new Logger(this.GetType());
            try
            {
                InstanceContext contexto = new InstanceContext(this);
                _proxyLanzamiento = new JuegoLanzamientoClient(contexto);
                _proxyLanzamiento.AgregarJugadorJuegoLanzamiento(_codigoPartida, _correoJugadorActual);
                return;
            }
            catch (CommunicationObjectFaultedException ex)
            {
                log.LogWarn("Se ha perdido la conexión previa", ex);
                Utilidades.MostrarUnMensajeError(Properties.Resources.msgErrorComunicacionDefectuosaJuego);
            }
            catch (CommunicationException ex)
            {
                Utilidades.MostrarUnMensajeError(Properties.Resources.msgErrorConexion);
                log.LogWarn("Error de Comunicación con el Servidor", ex);
            }
            catch (TimeoutException ex)
            {
                Utilidades.MostrarUnMensajeError(Properties.Resources.msgErrorTiempoEsperaServidor);
                log.LogWarn("Se agoto el tiempo de espera del servidor", ex);
            }

            throw new CommunicationException();
        }

        
        private void MostrarTableroElementosCorrespondientes()
        {
            ImageBrush fondo = new ImageBrush();
            switch(_numeroJugadores)
            {
                case 2:
                    fondo.ImageSource = new BitmapImage(new Uri(Properties.Resources.uriTableroLanzamientoDosJugadores));
                    brdFondoTablero.Background = fondo;
                    cnvEspacioAmarillo.Visibility = Visibility.Hidden;
                    cnvEspacioVerde.Visibility = Visibility.Hidden;
                    break;
                case 3:
                    fondo.ImageSource = new BitmapImage(new Uri(Properties.Resources.uriTableroLanzamientoTresJugadores));
                    brdFondoTablero.Background = fondo;
                    cnvEspacioRojo.Visibility = Visibility.Hidden;
                    break;
                case 4:
                    fondo.ImageSource = new BitmapImage(new Uri(Properties.Resources.uriTableroLanzamientoCuatroJugadores));
                    brdFondoTablero.Background = fondo;
                    break;
            }
        }

        private void ColocarNombres(List<CuentaSet> jugadores)
        {
            switch (_numeroJugadores)
            {
                case 2:
                    lblJugadorAzul.Content = jugadores[0].Nickname;
                    lblJugadorRojo.Content = jugadores[1].Nickname;
                    break;
                case 3:
                    lblJugadorAzul.Content = jugadores[0].Nickname;
                    lblJugadorVerde.Content = jugadores[1].Nickname;
                    lblJugadorAmarillo.Content = jugadores[2].Nickname;
                    break;
                case 4:
                    lblJugadorAzul.Content = jugadores[0].Nickname;
                    lblJugadorVerde.Content = jugadores[1].Nickname;
                    lblJugadorRojo.Content = jugadores[2].Nickname;
                    lblJugadorAmarillo.Content = jugadores[3].Nickname;
                    break;
                default:
                    break;
            }
            
        }
        private Dictionary<Ellipse, int> RegresarCirculosPuntuaciones()
        {
            return new Dictionary<Ellipse, int>
            {
                { llpPuntuacion5, 5}, { llpPuntuacion4, 4}, { llpPuntuacion3, 3},
                { llpPuntuacion2, 2 }, { llpPuntuacion1, 1 }
            };
        }
        private List<Rectangle> RegresarObstaculos()
        {
            List<Rectangle> listaObstaculos;
            switch (_numeroJugadores)
            {
                case 2:
                    listaObstaculos = new List<Rectangle>
                    {
                        rctParedAzulIzquierdaDos, rctParedAzulDerecha, rctParedAzulIzquierda,
                        rctParedRojaDerecha, rctParedRojaIzquierda, rctParedRojaDerechaDos
                    };
                    break;
                case 3:
                    listaObstaculos = new List<Rectangle>
                    {
                        rctParedAzulArribaTres, rctParedAmarilloArriba, rctParedAmarillaAbajo, rctParedAzulDerecha, rctParedAzulIzquierda,
                        rctParedVerdeAbajo, rctParedVerdeArriba
                    };
                    break;
                case 4:
                    listaObstaculos = new List<Rectangle>
                    {
                        rctParedAmarillaAbajo, rctParedAmarilloArriba, rctParedAzulDerecha, rctParedAzulIzquierda,
                        rctParedRojaDerecha, rctParedRojaIzquierda, rctParedVerdeAbajo, rctParedVerdeArriba
                    };
                    break;
                default:
                    listaObstaculos = null;
                    break;
            }
            return listaObstaculos;
        }
        private List<Rectangle> RegresarLugaresNoValidos()
        {
            List<Rectangle> listaNoValidos;
            switch (_numeroJugadores)
            {
                case 2:
                    listaNoValidos = new List<Rectangle>
                    {
                        rctPistaAzul, rctPistaRojo,
                        rctIzquierdaNoValido, rctAbajoNoValido, rctDerechaNoValido, rctArribaNoValido
                    };
                    break;
                case 3:
                    listaNoValidos = new List<Rectangle>
                    {
                       rctPistaAmarillo, rctPistaAzul, rctPistaVerde,
                       rctIzquierdaNoValido, rctAbajoNoValido, rctDerechaNoValido, rctArribaNoValido
                    };
                    break;
                case 4:
                    listaNoValidos = new List<Rectangle>
                    {
                        rctPistaAmarillo, rctPistaAzul, rctPistaRojo, rctPistaVerde, 
                        rctIzquierdaNoValido, rctAbajoNoValido, rctDerechaNoValido, rctArribaNoValido
                    };
                    break;
                default:
                    listaNoValidos = null;
                    break;
            }
            return listaNoValidos;
        }
        private void ClickDetenerDado(object sender, RoutedEventArgs e)
        {
            int numeroDadoTirado = _tablero.RetornarDado();
            _tablero.DetenerDadoPosicion(numeroDadoTirado);
            MandarDadoJugadores(numeroDadoTirado);
        }
        private void ClickLanzarPeon(object sender, RoutedEventArgs e)
        {
            _tablero.LanzarPeonActual();
            (double posicionX, double posicionY) = _tablero.RecuperarPosicionLineaJugadorActual();
            DetenerLineaJugadorActual(posicionX, posicionY);
            _botonesLanzarPeonJugador[_tablero.ListaJugadores[_tablero.TurnoActual].DireccionJugador].IsEnabled = false;
        }
        private void IniciarTurno()
        {
            CambiarBotonesADeshabilitado();
            JugadorLanzamiento jugadorTurnoActual = _tablero.ListaJugadores[_tablero.TurnoActual];
            PeonLanzamiento peonTurnoActual = jugadorTurnoActual.PeonesLanzamiento[jugadorTurnoActual.PeonTurnoActual];
            Canvas.SetTop(peonTurnoActual.Figura, peonTurnoActual.PosicionPeon.Y);
            Canvas.SetLeft(peonTurnoActual.Figura, peonTurnoActual.PosicionPeon.X);
            Canvas.SetZIndex(peonTurnoActual.Figura, 1);

            LineaMovimiento linea = jugadorTurnoActual.LineaMovimiento;

            cnvEspacioJuego.Children.Add(linea.FiguraLinea);
            Canvas.SetTop(linea.FiguraLinea, linea.PosicionCanva.Y);
            Canvas.SetLeft(linea.FiguraLinea, linea.PosicionCanva.X);

            if (_correoJugadorActual.Equals(jugadorTurnoActual.CorreElectronico))
            {
                _botonesTirarDadoJugador[jugadorTurnoActual.DireccionJugador].IsEnabled = true;
            }

            lblTurnoJugador.Content = Properties.Resources.lblTurnoJugador + " " + jugadorTurnoActual.Nickname;
        }
        private void MostrarPotenciaLanzamiento(int potencia, int potenciaAgregada)
        {
            JugadorLanzamiento jugadorTurno = _tablero.ListaJugadores[_tablero.TurnoActual];
            string potenciaLanzamiento = Properties.Resources.txtBlockPotenciaLanzamiento + potencia + " + " + potenciaAgregada;
            _etiquetasJugadoresLanzamientoPotencia[jugadorTurno.DireccionJugador].Text = potenciaLanzamiento;
            if (_correoJugadorActual.Equals(jugadorTurno.CorreElectronico))
            {
                _botonesTirarDadoJugador[jugadorTurno.DireccionJugador].IsEnabled = false;
                _botonesLanzarPeonJugador[jugadorTurno.DireccionJugador].IsEnabled = true;
            }
        }
        private void EliminarLineaMovimientoJugador()
        {
            JugadorLanzamiento jugadorTurnoActual = _tablero.ListaJugadores[_tablero.TurnoActual];
            Line lineaJugador = jugadorTurnoActual.LineaMovimiento.FiguraLinea;
            if (cnvEspacioJuego.Children.Contains(lineaJugador))
            {
                cnvEspacioJuego.Children.Remove(lineaJugador);
            }
            _etiquetasJugadoresLanzamientoPotencia[jugadorTurnoActual.DireccionJugador].Text = Properties.Resources.txtBlockPotenciaLanzamiento + " -";
        }
        private void ColocarPiezasJugadores()
        {
            foreach (JugadorLanzamiento jugador in _tablero.ListaJugadores)
            {
                foreach (PeonLanzamiento peonJugador in jugador.PeonesLanzamiento)
                {
                    cnvEspacioJuego.Children.Add(peonJugador.Figura);
                    Canvas.SetTop(peonJugador.Figura, peonJugador.PosicionPeon.Y);
                    Canvas.SetLeft(peonJugador.Figura, peonJugador.PosicionPeon.X);
                }
                foreach (DadoPotencia dadoJugador in jugador.DadosJugador)
                {
                    cnvEspacioJuego.Children.Add(dadoJugador.ImagenDado);
                    Canvas.SetTop(dadoJugador.ImagenDado, dadoJugador.PosicionCanva.Y);
                    Canvas.SetLeft(dadoJugador.ImagenDado, dadoJugador.PosicionCanva.X);
                }
            }
        }

        private void EliminarPeonCanvas(PeonLanzamiento peon)
        {
            cnvEspacioJuego.Children.Remove(peon.Figura);
        }
        private void InicializarTablero(List<CuentaSet> listaCuentas)
        {
            List<Rectangle> obstaculos = RegresarObstaculos();
            List<Rectangle> noValidos = RegresarLugaresNoValidos();
            Dictionary<Ellipse, int> circulosPuntuaciones = RegresarCirculosPuntuaciones();

            _tablero = new Tablero(listaCuentas, obstaculos, noValidos, circulosPuntuaciones);
        }

        private void NotificarTurnoAcabado()
        {
            Logger log = new Logger(this.GetType());
            try
            {
                _proxyLanzamiento.NotificarFinalizarLanzamiento(_codigoPartida, _correoJugadorActual);
                return;
            }
            catch (CommunicationObjectFaultedException ex)
            {
                Utilidades.MostrarUnMensajeError(Properties.Resources.msgErrorComunicacionDefectuosaJuego);
                log.LogWarn("Se ha perdido la conexión previa", ex);
            }
            catch (CommunicationException ex)
            {
                Utilidades.MostrarUnMensajeError(Properties.Resources.msgErrorConexion);
                log.LogWarn("Error de Comunicación con el Servidor", ex);
            }
            catch (TimeoutException ex)
            {
                Utilidades.MostrarUnMensajeError(Properties.Resources.msgErrorTiempoEsperaServidor);
                log.LogWarn("Se agoto el tiempo de espera del servidor", ex);
            }
            Utilidades.SalirHastaInicioSesionDesdeJuegoYLobbyVentana(this);
        }

        public void JugadorTiroDado(int numeroDado)
        {
            _tablero.DetenerDadoPosicion(numeroDado);
        }

        private void MandarDadoJugadores(int numeroDado)
        {
            Logger log = new Logger(this.GetType());
            try
            {
                _proxyLanzamiento.NotificarLanzamientoDado(_codigoPartida, _correoJugadorActual, numeroDado);
                return;
            }
            catch (CommunicationObjectFaultedException ex)
            {
                Utilidades.MostrarUnMensajeError(Properties.Resources.msgErrorComunicacionDefectuosaJuego);
                log.LogWarn("Se ha perdido la conexión previa", ex);
            }
            catch (CommunicationException ex)
            {
                Utilidades.MostrarUnMensajeError(Properties.Resources.msgErrorConexion);
                log.LogWarn("Error de Comunicación con el Servidor", ex);
            }
            catch (TimeoutException ex)
            {
                Utilidades.MostrarUnMensajeError(Properties.Resources.msgErrorTiempoEsperaServidor);
                log.LogWarn("Se agoto el tiempo de espera del servidor", ex);
            }
            Utilidades.SalirHastaInicioSesionDesdeJuegoYLobbyVentana(this);
        }

        public void JugadorDetuvoLinea(double posicionX, double posicionY)
        {
            _tablero.DetenerLineaMovimiento(posicionX, posicionY);
        }

        private void DetenerLineaJugadorActual(double posicionX, double posicionY)
        {
            Logger log = new Logger(this.GetType());
            try
            {
                _proxyLanzamiento.NotificarLanzamientoLinea(_codigoPartida, _correoJugadorActual, posicionX, posicionY);
                return;
            }
            catch (CommunicationObjectFaultedException ex)
            {
                Utilidades.MostrarUnMensajeError(Properties.Resources.msgErrorComunicacionDefectuosaJuego);
                log.LogWarn("Se ha perdido la conexión previa", ex);
            }
            catch (CommunicationException ex)
            {
                Utilidades.MostrarUnMensajeError(Properties.Resources.msgErrorConexion);
                log.LogWarn("Error de Comunicación con el Servidor", ex);
            }
            catch (TimeoutException ex)
            {
                Utilidades.MostrarUnMensajeError(Properties.Resources.msgErrorTiempoEsperaServidor);
                log.LogWarn("Se agoto el tiempo de espera del servidor", ex);
            }
            Utilidades.SalirHastaInicioSesionDesdeJuegoYLobbyVentana(this);
        }

        public void JugadoresListosParaSiguienteTurno()
        {
            JugadorLanzamiento jugadorActual = _tablero.ListaJugadores[_tablero.TurnoActual];
            if (jugadorActual.CorreElectronico.Equals(_correoJugadorActual))
            {
                EnviarTableroPeones(_tablero.ObtenerPosicionPeonesActuales());
            }
        }

        private void EnviarTableroPeones(PeonesTablero peones)
        {
            Logger log = new Logger(this.GetType());
            try
            {
                _proxyLanzamiento.NotificarPosicionFichasFinales(_codigoPartida, _correoJugadorActual, peones);
                return;
            }
            catch (CommunicationObjectFaultedException ex)
            {
                log.LogWarn("Se ha perdido la conexión previa", ex);
                Utilidades.MostrarUnMensajeError(Properties.Resources.msgErrorComunicacionDefectuosaJuego);
            }
            catch (CommunicationException ex)
            {
                Utilidades.MostrarUnMensajeError(Properties.Resources.msgErrorConexion);
                log.LogWarn("Error de Comunicación con el Servidor", ex);
            }
            catch (TimeoutException ex)
            {
                Utilidades.MostrarUnMensajeError(Properties.Resources.msgErrorConexion);
                log.LogWarn("Se agoto el tiempo de espera del servidor", ex);
            }
            Utilidades.SalirHastaInicioSesionDesdeJuegoYLobbyVentana(this);
        }
        public void CambiarPosicionPeonesTableroYContinuar(PeonesTablero peones)
        {
            _tablero.ModificarPosicionPeones(peones);
            _tablero.CambiarTurnoSiguiente();
        }

        public void JugadorSalioJuegoLanzamiento(string correoElectronicoSalido)
        {
            _tablero.DesconectarJugador(correoElectronicoSalido);
            _tablero.ComprobarJugadoresRestantes();
        }

        private void TerminarJuegoFaltaJugadores()
        {
            if (Window.GetWindow(this) != null)
            {
                Window.GetWindow(this).Close();
                Utilidades.MostrarUnMensajeError(Properties.Resources.msgFaltaJugadores);
            }
        }

        private void EliminarContextoJuegoLanzamiento()
        {
            Logger log = new Logger(this.GetType());
            try
            {
                _proxyLanzamiento.EliminarJugadorJuegoLanzamiento(_codigoPartida);
                _juegoYLobbyVentana.EliminarContexto -= EliminarContextoJuegoLanzamiento;
                return;
            }
            catch (CommunicationObjectFaultedException ex)
            {
                Utilidades.MostrarUnMensajeError(Properties.Resources.msgErrorComunicacionDefectuosaSalida);
                log.LogWarn("Se ha perdido la conexión previa", ex);
            }
            catch (CommunicationException ex)
            {
                Utilidades.MostrarUnMensajeError(Properties.Resources.msgErrorConexion);
                log.LogWarn("Error de Comunicación con el Servidor", ex);
            }
            catch (TimeoutException ex)
            {
                Utilidades.MostrarUnMensajeError(Properties.Resources.msgErrorConexion);
                log.LogWarn("Se agoto el tiempo de espera del servidor", ex);
            }
            throw new CommunicationException();
        }

        private async void PasarPuntuaciones(List<JugadorLanzamiento> jugadoresConPuntuaciones)
        {
            brdConteoPuntuaciones.Visibility = Visibility.Visible;
            await Task.Delay(3500);
            brdConteoPuntuaciones.Visibility = Visibility.Hidden;
            if (_juegoPuntuacionPagina == null)
            {
                CrearJuegoPuntuaciones(jugadoresConPuntuaciones);
            }
            else 
            {
                _juegoPuntuacionPagina.EmpezarJuegoPuntuacionNuevo(jugadoresConPuntuaciones);
                _juegoYLobbyVentana.CambiarFrameLobby(_juegoPuntuacionPagina);
            }
            _juegoYLobbyVentana.EliminarContexto -= EliminarContextoJuegoLanzamiento;
        }

        private void CrearJuegoPuntuaciones(List<JugadorLanzamiento> jugadoresConPuntuaciones)
        {
            JuegoPuntuacionesPagina puntuaciones = new JuegoPuntuacionesPagina(jugadoresConPuntuaciones, _cuentaUsuario, _codigoPartida, _listaCuentas, _juegoYLobbyVentana);
            Constantes respuesta = puntuaciones.AgregarProxy();
            switch (respuesta)
            {
                case Constantes.OPERACION_EXITOSA:
                    _juegoYLobbyVentana.CambiarFrameLobby(puntuaciones);
                    break;
                case Constantes.ERROR_CONEXION_SERVIDOR:
                    Utilidades.SalirHastaInicioSesionDesdeJuegoYLobbyVentana(this);
                    break;
            }
            
        }

        private void CambiarBotonesADeshabilitado()
        {
            foreach (var botonLanzamientoDirreccion in _botonesLanzarPeonJugador)
            {
                botonLanzamientoDirreccion.Value.IsEnabled = false;
            }

            foreach (var botonesDadoDireccion in _botonesTirarDadoJugador)
            {
                botonesDadoDireccion.Value.IsEnabled = false;
            }
        }
    }
}
