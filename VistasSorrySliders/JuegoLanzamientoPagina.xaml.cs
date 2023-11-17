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
        private Dictionary<Direccion, TextBlock> _etiquetasJugadoresLanzamientoPotencia;
        private Dictionary<Direccion, Button> _botonesLanzarPeonJugador;
        private Dictionary<Direccion, Button> _botonesTirarDadoJugador;
        private Tablero _tablero;
        private int _numeroJugadores;
        private IJuegoLanzamiento _proxyLanzamiento;
        private string _codigoPartida;
        private string _correoJugadorActual;
        public JuegoLanzamientoPagina(List<CuentaSet> listaCuentas, int numeroJugadores, string codigoPartida, string correoElectronicoActual)
        {
            _correoJugadorActual = correoElectronicoActual;
            _codigoPartida = codigoPartida;
            _numeroJugadores = numeroJugadores;
            InitializeComponent();
            ColocarNombres(listaCuentas);
            MostrarTableroElementosCorrespondientes();
            InicializarProxyJuegoLanzamiento();

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

            ColocarPiezasJugadores();
            _tablero.IniciarTurno();
        }

        private void InicializarProxyJuegoLanzamiento()
        {
            Logger log = new Logger(this.GetType());
            try
            {
                InstanceContext contexto = new InstanceContext(this);
                _proxyLanzamiento = new JuegoLanzamientoClient(contexto);
                _proxyLanzamiento.AgregarJugadorJuegoLanzamiento(_codigoPartida, _correoJugadorActual);
            }
            catch (CommunicationException ex)
            {
                Console.WriteLine(ex);
                System.Windows.Forms.MessageBox.Show(Properties.Resources.msgErrorConexion);
                log.LogError("Error de Comunicación con el Servidor", ex);
                RegresarMenuPrincipal();
            }
            catch (TimeoutException ex)
            {
                Console.WriteLine(ex);
                System.Windows.Forms.MessageBox.Show(Properties.Resources.msgErrorConexion);
                log.LogWarn("Se agoto el tiempo de espera del servidor", ex);
                RegresarMenuPrincipal();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                System.Windows.Forms.MessageBox.Show(Properties.Resources.msgErrorConexion);
                log.LogFatal("Ha ocurrido un error inesperado", ex);
                RegresarMenuPrincipal();
            }
        }

        private void MostrarTableroElementosCorrespondientes()
        {
            ImageBrush fondo = new ImageBrush();
            switch(_numeroJugadores)
            {
                case 2:
                    fondo.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Recursos/tableroDos.png"));
                    brdFondoTablero.Background = fondo;
                    cnvEspacioAmarillo.Visibility = Visibility.Hidden;
                    cnvEspacioVerde.Visibility = Visibility.Hidden;
                    break;
                case 3:
                    fondo.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Recursos/tableroTres.png"));
                    brdFondoTablero.Background = fondo;
                    cnvEspacioRojo.Visibility = Visibility.Hidden;
                    break;
                case 4:
                    fondo.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Recursos/tableroCuatro.png"));
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
                    lblJugadorVerde.Content = jugadores[2].Nickname;
                    lblJugadorAmarillo.Content = jugadores[1].Nickname;
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
                        rctPistaAzul, rctPistaRojo
                    };
                    break;
                case 3:
                    listaNoValidos = new List<Rectangle>
                    {
                       rctPistaAmarillo, rctPistaAzul, rctPistaVerde
                    };
                    break;
                case 4:
                    listaNoValidos = new List<Rectangle>
                    {
                        rctPistaAmarillo, rctPistaAzul, rctPistaRojo, rctPistaVerde
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
            JugadorLanzamiento jugadorTurnoActual = _tablero.ListaJugadores[_tablero.TurnoActual];
            PeonLanzamiento peonTurnoActual = jugadorTurnoActual.PeonesLanzamiento[jugadorTurnoActual.PeonTurnoActual];
            Canvas.SetTop(peonTurnoActual.Figura, peonTurnoActual.PosicionPeon.Y);
            Canvas.SetLeft(peonTurnoActual.Figura, peonTurnoActual.PosicionPeon.X);

            LineaMovimiento linea = jugadorTurnoActual.LineaMovimiento;

            cnvEspacioJuego.Children.Add(linea.FiguraLinea);
            Canvas.SetTop(linea.FiguraLinea, linea.PosicionCanva.Y);
            Canvas.SetLeft(linea.FiguraLinea, linea.PosicionCanva.X);

            if (_correoJugadorActual.Equals(jugadorTurnoActual.CorreElectronico))
            {
                _botonesTirarDadoJugador[jugadorTurnoActual.DireccionJugador].IsEnabled = true;
            }

            lblTurnoJugador.Content = "TURNO DEL JUGADOR: " + jugadorTurnoActual.Nickname;
        }
        private void MostrarPotenciaLanzamiento(int potencia, int potenciaAgregada)
        {
            JugadorLanzamiento jugadorTurno = _tablero.ListaJugadores[_tablero.TurnoActual];
            string potenciaLanzamiento = "Potencia de Lanzamiento: " + potencia + " + " + potenciaAgregada;
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
            _etiquetasJugadoresLanzamientoPotencia[jugadorTurnoActual.DireccionJugador].Text = "Potencia de Lanzamiento: -";
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
            switch (_numeroJugadores)
            {
                case 2:
                    _tablero = new TableroDosJugadores(listaCuentas, obstaculos, noValidos);
                    break;
                case 3:
                    _tablero = new TableroTresJugadores(listaCuentas, obstaculos, noValidos);
                    break;
                case 4:
                    _tablero = new TableroCuatroJugadores(listaCuentas, obstaculos, noValidos);
                    break;
                default:
                    _tablero = null;
                    break;
            }
        }

        private void NotificarTurnoAcabado()
        {
            Logger log = new Logger(this.GetType());
            try
            {
                _proxyLanzamiento.NotificarFinalizarLanzamiento(_codigoPartida, _correoJugadorActual);
            }
            catch (CommunicationException ex)
            {
                log.LogError("Error de Comunicación con el Servidor", ex);
            }
            catch (TimeoutException ex)
            {
                log.LogWarn("Se agoto el tiempo de espera del servidor", ex);
            }
        }

        private void RevivirProxy()
        {
            Logger log = new Logger(this.GetType());
            try
            {
                InstanceContext contexto = new InstanceContext(this);
                //_proxyLobby = new LobbyClient(contexto);
            }
            catch (CommunicationException ex)
            {
                log.LogError("Error de Comunicación con el Servidor", ex);
            }
            catch (TimeoutException ex)
            {
                log.LogWarn("Se agoto el tiempo de espera del servidor", ex);
            }
        }

        public void JugadorTiroDado(int numeroDado)
        {
            _tablero.DetenerDadoPosicion(numeroDado);
        }

        public void JugadorSalioJuegoLanzamiento(string correoElectronicoSalido)
        {
            //Por implementar
        }

        private void RegresarMenuPrincipal()
        {
            Console.WriteLine("Regresando a menú principal por un error....");
        }

        private void MandarDadoJugadores(int numeroDado)
        {
            Logger log = new Logger(this.GetType());
            try
            {
                _proxyLanzamiento.NotificarLanzamientoDado(_codigoPartida, _correoJugadorActual, numeroDado);
            }
            catch (CommunicationException ex)
            {
                log.LogError("Error de Comunicación con el Servidor", ex);
            }
            catch (TimeoutException ex)
            {
                log.LogWarn("Se agoto el tiempo de espera del servidor", ex);
            }
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
            }
            catch (CommunicationException ex)
            {
                log.LogError("Error de Comunicación con el Servidor", ex);
            }
            catch (TimeoutException ex)
            {
                log.LogWarn("Se agoto el tiempo de espera del servidor", ex);
            }
        }

        public void JugadoresListosParaSiguienteTurno()
        {
            _tablero.CambiarTurnoSiguiente();
        }
    }
}
