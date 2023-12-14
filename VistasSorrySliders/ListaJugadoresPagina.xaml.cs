using log4net;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
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
using VistasSorrySliders.ServicioSorrySliders;

namespace VistasSorrySliders
{
    /// <summary>
    /// Lógica de interacción para ListaJugadoresPagina.xaml
    /// </summary>
    public partial class ListaJugadoresPagina : Page, INotificarJugadoresCallback
    {
        private List<CuentaSet> _baneados;
        private List<CuentaSet> _amigos;
        private List<CuentaSet> _cuentasSolicitudesPendientes;
        private List<CuentaSet> _listaTodasCuentasBuscadas;
        private List<NotificacionSet> _notificaciones;
        private NotificarJugadoresClient _proxyNotificar;
        private CuentaSet _cuentaUsuario;
        private ListaAmigosClient _proxyAmigos;
        private List<TipoNotificacion> _tiposNotificacion;

        public ListaJugadoresPagina(CuentaSet cuenta)
        {
            InitializeComponent();
            _cuentaUsuario = cuenta;
            _listaTodasCuentasBuscadas = new List<CuentaSet>();
        }

        public bool InicializarListaJugadores()
        {
            return CrearProxyAmigos() && GuardarContexto() && RecuperarTiposNotificacion() && RecuperarBaneados() 
                && RecuperarCuentasConSolicituPendiente() && CargarAmigos() && CargarNotificaciones();
        }

        private bool CrearProxyAmigos()
        {
            Logger log = new Logger(this.GetType());
            Constantes resultado;
            try
            {
                _proxyAmigos = new ListaAmigosClient();
                resultado = Constantes.OPERACION_EXITOSA;
            }
            catch (CommunicationException ex)
            {
                resultado = Constantes.ERROR_CONEXION_SERVIDOR;
                log.LogWarn("Error de Comunicación con el Servidor", ex);
            }
            catch (TimeoutException ex)
            {
                resultado = Constantes.ERROR_TIEMPO_ESPERA_SERVIDOR;
                log.LogWarn("Se agoto el tiempo de espera del servidor", ex);
            }
            switch (resultado)
            {
                case Constantes.OPERACION_EXITOSA:
                    return true;
                default:
                    Utilidades.MostrarMensajesError(resultado);
                    return false;
            }
        }

        private bool GuardarContexto()
        {
            Logger log = new Logger(this.GetType());
            Constantes resultado;
            try
            {
                InstanceContext contexto = new InstanceContext(this);
                _proxyNotificar = new NotificarJugadoresClient(contexto);
                resultado = _proxyNotificar.AgregarProxy(_cuentaUsuario.CorreoElectronico);
                resultado = Constantes.OPERACION_EXITOSA;
            }
            catch (CommunicationObjectFaultedException ex)
            {
                resultado = Constantes.ERROR_CONEXION_DEFECTUOSA;
                log.LogWarn("Se ha perdido la conexión previa", ex);
            }
            catch (CommunicationException ex)
            {
                resultado = Constantes.ERROR_CONEXION_SERVIDOR;
                log.LogWarn("Error de Comunicación con el Servidor", ex);
            }
            catch (TimeoutException ex)
            {
                resultado = Constantes.ERROR_TIEMPO_ESPERA_SERVIDOR;
                log.LogWarn("Se agoto el tiempo de espera del servidor", ex);
            }
            switch (resultado)
            {
                case Constantes.OPERACION_EXITOSA:
                    return true;
                default:
                    Utilidades.MostrarMensajesError(resultado);
                    return false;
            }
        }


        private bool RecuperarTiposNotificacion()
        {
            Logger log = new Logger(this.GetType());
            Constantes resultado;
            TipoNotificacion[] listaTiposNotificaciones;
            try
            {
                (resultado, listaTiposNotificaciones) = _proxyAmigos.RecuperarTipoNotificacion();
                if (resultado == Constantes.OPERACION_EXITOSA)
                {
                    _tiposNotificacion = listaTiposNotificaciones.ToList();
                    return true;
                }
            }
            catch (CommunicationObjectFaultedException ex)
            {
                resultado = Constantes.ERROR_CONEXION_DEFECTUOSA;
                log.LogWarn("Se ha perdido la conexión previa", ex);
            }
            catch (CommunicationException ex)
            {
                resultado = Constantes.ERROR_CONEXION_SERVIDOR;
                log.LogWarn("Error de Comunicación con el Servidor", ex);
            }
            catch (TimeoutException ex)
            {
                resultado = Constantes.ERROR_TIEMPO_ESPERA_SERVIDOR;
                log.LogWarn("Se agoto el tiempo de espera del servidor", ex);
            }
            switch (resultado)
            {
                case Constantes.OPERACION_EXITOSA_VACIA:
                    Utilidades.MostrarUnMensajeError(Properties.Resources.msgTiposNotificacionVacios);
                    break;
                default:
                    Utilidades.MostrarMensajesError(resultado);
                    break;
            }
            return false;
        }
        private void RecuperarJugadoresEspecificos(string informacionJugador)
        {
            Constantes resultado;
            Logger log = new Logger(this.GetType());
            try
            {
                CuentaSet[] listaJugadores;
                (resultado, listaJugadores) = _proxyAmigos.RecuperarJugadoresCuenta(informacionJugador, _cuentaUsuario.CorreoElectronico);
                if (resultado == Constantes.OPERACION_EXITOSA)
                {
                   _listaTodasCuentasBuscadas = listaJugadores.ToList();
                    MostrarJugadores();
                    return;
                }
            }
            catch (CommunicationObjectFaultedException ex)
            {
                resultado = Constantes.ERROR_CONEXION_DEFECTUOSA;
                log.LogWarn("Se ha perdido la conexión previa", ex);
            }
            catch (CommunicationException ex)
            {
                resultado = Constantes.ERROR_CONEXION_SERVIDOR;
                log.LogWarn("Error de Comunicación con el Servidor", ex);
            }
            catch (TimeoutException ex)
            {
                resultado = Constantes.ERROR_TIEMPO_ESPERA_SERVIDOR;
                log.LogWarn("Se agoto el tiempo de espera del servidor", ex);
            }
            switch (resultado)
            {
                case Constantes.OPERACION_EXITOSA_VACIA:
                    lstBoxJugadores.Style = (Style)FindResource("estiloLstBoxJugadoresVacia");
                    Utilidades.MostrarMensajeInformacion(Properties.Resources.msgJugadoresRecuperacion);
                    return;
                default:
                    Utilidades.MostrarMensajesError(resultado);
                    SalirAlMenu();
                    break;
            }
        }

        private bool RecuperarBaneados()
        {
            Constantes resultado;
            Logger log = new Logger(this.GetType());
            CuentaSet[] listaBaneados;
            try
            {
                (resultado, listaBaneados) = _proxyAmigos.RecuperarBaneados(_cuentaUsuario.CorreoElectronico);
                if (resultado == Constantes.OPERACION_EXITOSA)
                {
                    _baneados = listaBaneados.ToList();
                    return true;
                }
            }
            catch (CommunicationObjectFaultedException ex)
            {
                resultado = Constantes.ERROR_CONEXION_DEFECTUOSA;
                log.LogWarn("Se ha perdido la conexión previa", ex);
            }
            catch (CommunicationException ex)
            {
                resultado = Constantes.ERROR_CONEXION_SERVIDOR;
                log.LogWarn("Error de Comunicación con el Servidor", ex);
            }
            catch (TimeoutException ex)
            {
                resultado = Constantes.ERROR_TIEMPO_ESPERA_SERVIDOR;
                log.LogWarn("Se agoto el tiempo de espera del servidor", ex);
            }
            switch (resultado)
            {
                case Constantes.OPERACION_EXITOSA_VACIA:
                    return true;
                default:
                    Utilidades.MostrarMensajesError(resultado);
                    return false;
            }
        }

        private bool RecuperarCuentasConSolicituPendiente()
        {
            Constantes resultado;
            Logger log = new Logger(this.GetType());
            try
            {
                CuentaSet[] listaPendientes;
                (resultado, listaPendientes) = _proxyAmigos.RecuperarSolicitudesAmistad(_cuentaUsuario.CorreoElectronico);

                if (resultado == Constantes.OPERACION_EXITOSA)
                {
                    _cuentasSolicitudesPendientes = listaPendientes.ToList();
                    return true;
                }
            }
            catch (CommunicationObjectFaultedException ex)
            {
                resultado = Constantes.ERROR_CONEXION_DEFECTUOSA;
                log.LogWarn("Se ha perdido la conexión previa", ex);
            }
            catch (CommunicationException ex)
            {
                resultado = Constantes.ERROR_CONEXION_SERVIDOR;
                log.LogWarn("Error de Comunicación con el Servidor", ex);
            }
            catch (TimeoutException ex)
            {
                resultado = Constantes.ERROR_TIEMPO_ESPERA_SERVIDOR;
                log.LogWarn("Se agoto el tiempo de espera del servidor", ex);
            }
            switch (resultado)
            {
                case Constantes.OPERACION_EXITOSA_VACIA:
                    return true;
                default:
                    Utilidades.MostrarMensajesError(resultado);
                    return false;
            }
        }

        private void MostrarJugadores()
        {
            lstBoxJugadores.Style = (Style)FindResource("estiloLstBoxAmigos");
            lstBoxJugadores.Items.Clear();

            EliminarDuplicados();

            if (_listaTodasCuentasBuscadas != null && _listaTodasCuentasBuscadas.Any())
            {
                foreach (CuentaSet cuenta in _listaTodasCuentasBuscadas)
                {
                    ListBoxItem lstBoxItemCuenta = new ListBoxItem
                    {
                        DataContext = cuenta
                    };

                    if (EsCuentaAmigo(cuenta))
                    {
                        lstBoxItemCuenta.Style = (Style)FindResource("estiloLstBoxItemAmigo");
                    }
                    if (EsPendienteRecibida(cuenta))
                    {
                        lstBoxItemCuenta.Style = (Style)FindResource("estiloLstBoxItemJugadorPendiente");
                    }
                    if (EsCuentaBaneada(cuenta))
                    {
                        lstBoxItemCuenta.Style = (Style)FindResource("estiloLstBoxItemJugadorBaneado");
                    }
                    lstBoxJugadores.Items.Add(lstBoxItemCuenta);
                }
            }
        }

        private bool CargarAmigos()
        {
            Constantes resultado;
            Logger log = new Logger(this.GetType());
            try
            {
                CuentaSet[] listaAmigos;
                (resultado, listaAmigos) = _proxyAmigos.RecuperarAmigos(_cuentaUsuario.CorreoElectronico);
                if (resultado == Constantes.OPERACION_EXITOSA)
                {
                    _amigos = listaAmigos.ToList();
                    lstBoxAmigos.Style = (Style)FindResource("estiloLstBoxAmigos");
                    lstBoxAmigos.ItemsSource = null;
                    lstBoxAmigos.Items.Clear();
                    lstBoxAmigos.ItemsSource = _amigos;
                    return true;
                }
            }
            catch (CommunicationObjectFaultedException ex)
            {
                resultado = Constantes.ERROR_CONEXION_DEFECTUOSA;
                log.LogWarn("Se ha perdido la conexión previa", ex);
            }
            catch (CommunicationException ex)
            {
                resultado = Constantes.ERROR_CONEXION_SERVIDOR;
                log.LogWarn("Error de Comunicación con el Servidor", ex);
            }
            catch (TimeoutException ex)
            {
                resultado = Constantes.ERROR_TIEMPO_ESPERA_SERVIDOR;
                log.LogWarn("Se agoto el tiempo de espera del servidor", ex);
            }
            switch (resultado)
            {
                case Constantes.OPERACION_EXITOSA_VACIA:
                    lstBoxAmigos.Style = (Style)FindResource("estiloLstBoxAmigosVacia");
                    lstBoxAmigos.ItemsSource = null;
                    lstBoxAmigos.Items.Clear();
                    return true;
                default:
                    Utilidades.MostrarMensajesError(resultado);
                    return false;
            }
        }

        private bool CargarNotificaciones()
        {
            Constantes resultado;
            Logger log = new Logger(this.GetType());
            try
            {
                NotificacionSet[] listaNotificacionesResultado;
                (resultado, listaNotificacionesResultado) = _proxyAmigos.RecuperarNotificaciones(_cuentaUsuario.CorreoElectronico);
                if (resultado == Constantes.OPERACION_EXITOSA)
                {
                    lstBoxNotificaciones.Style = (Style)FindResource("estiloLstBoxAmigos");
                    lstBoxNotificaciones.Items.Clear();
                    _notificaciones = listaNotificacionesResultado.ToList();
                    MostrarNotificaciones();
                    return true;
                }
            }
            catch (CommunicationObjectFaultedException ex)
            {
                resultado = Constantes.ERROR_CONEXION_DEFECTUOSA;
                log.LogWarn("Se ha perdido la conexión previa", ex);
            }
            catch (CommunicationException ex)
            {
                resultado = Constantes.ERROR_CONEXION_SERVIDOR;
                log.LogWarn("Error de Comunicación con el Servidor", ex);
            }
            catch (TimeoutException ex)
            {
                resultado = Constantes.ERROR_TIEMPO_ESPERA_SERVIDOR;
                log.LogWarn("Se agoto el tiempo de espera del servidor", ex);
            }
            switch (resultado)
            {
                case Constantes.OPERACION_EXITOSA_VACIA:
                    lstBoxNotificaciones.Items.Clear();
                    lstBoxNotificaciones.Style = (Style)FindResource("estiloLstBoxNotificacionesVacia");
                    return true;
                default:
                    Utilidades.MostrarMensajesError(resultado); 
                    return false;
            }
        }

        private void MostrarNotificaciones()
        {
            if (_notificaciones != null) {
                foreach (NotificacionSet notificacion in _notificaciones)
                {
                    ListBoxItem lstBoxItemCuenta = new ListBoxItem
                    {
                        DataContext = notificacion
                    };
                    if (!EsNotificacionCuentaBaneada(notificacion)) {
                        if (notificacion.IdTipoNotificacion == _tiposNotificacion[1].IdTipoNotificacion)
                        {
                            lstBoxItemCuenta.Style = (Style)FindResource("estiloLstBoxItemNotificacionAmigo");
                        }
                        if (notificacion.IdTipoNotificacion == _tiposNotificacion[0].IdTipoNotificacion)
                        {
                            lstBoxItemCuenta.Style = (Style)FindResource("estiloLstBoxItemNotificacionPartida");
                        }
                        lstBoxNotificaciones.Items.Add(lstBoxItemCuenta);
                    }
                }
            }
        }

        private void EliminarDuplicados()
        {
            List<CuentaSet> listaSinDuplicados = new List<CuentaSet>();

            foreach (var cuenta in _listaTodasCuentasBuscadas)
            {
                bool estaDuplicada = false;
                foreach (var cuentaDiferente in listaSinDuplicados)
                {
                    if (cuentaDiferente.CorreoElectronico == cuenta.CorreoElectronico)
                    {
                        estaDuplicada = true;
                        break;
                    }
                }
                if (!estaDuplicada)
                {
                    listaSinDuplicados.Add(cuenta);
                }
            }
            _listaTodasCuentasBuscadas = listaSinDuplicados;

        }

        private bool EsCuentaBaneada(CuentaSet cuentaComparar)
        {
            return _baneados?.Exists(cuenta => cuenta.CorreoElectronico.Equals(cuentaComparar.CorreoElectronico)) ?? false;
        }

        private bool EsNotificacionCuentaBaneada(NotificacionSet notificacion)
        {
            return _baneados?.Exists(cuenta => cuenta.CorreoElectronico.Equals(notificacion.CorreoElectronicoRemitente)) ?? false;
        }

        private bool EsCuentaAmigo(CuentaSet cuentaComparar)
        {
            return _amigos?.Exists(cuenta => cuenta.CorreoElectronico.Equals(cuentaComparar.CorreoElectronico)) ?? false;
        }

        private bool EsPendienteRecibida(CuentaSet cuentaComparar)
        {
            return _cuentasSolicitudesPendientes?.Exists(cuenta => cuenta.CorreoElectronico.Equals(cuentaComparar.CorreoElectronico)) ?? false;
        }

        private void ClickRegresarMenu(object sender, RoutedEventArgs e)
        {
            SalirAlMenu();
        }
        public void SalirAlMenu()
        {
            Logger log = new Logger(this.GetType());
            try
            {
                _proxyNotificar.EliminarProxy(_cuentaUsuario.CorreoElectronico);
            }
            catch (CommunicationObjectFaultedException ex)
            {
                log.LogWarn("Se ha perdido la conexión previa", ex);
            }
            catch (CommunicationException ex)
            {
                log.LogWarn("Error de Comunicación con el Servidor", ex);
            }
            catch (TimeoutException ex)
            {
                log.LogWarn("Se agoto el tiempo de espera del servidor", ex);
            }
            MenuPrincipalPagina menu = new MenuPrincipalPagina(_cuentaUsuario);
            this.NavigationService.Navigate(menu);
        }

        private void PreviewMouseDownSolicitudAmistad(object sender, MouseButtonEventArgs e)
        {
            Button boton = sender as Button;
            CuentaSet cuenta = boton.CommandParameter as CuentaSet;
            EnviarSolicitudAmistad(cuenta);
        }

        private void PreviewMouseDownBanearJugador(object sender, MouseButtonEventArgs e)
        {
            Button boton = sender as Button;
            CuentaSet cuenta = boton.CommandParameter as CuentaSet;
            BanearJugador(cuenta);
        }

        private void PreviewMouseDownEliminarBaneo(object sender, MouseButtonEventArgs e)
        {
            Button boton = sender as Button;
            CuentaSet cuenta = boton.CommandParameter as CuentaSet;
            EliminarBaneo(cuenta);
        }


        private void PreviewMouseDownEliminarAmigo(object sender, MouseButtonEventArgs e)
        {
            Button boton = sender as Button;
            CuentaSet cuentaAmigo = boton.CommandParameter as CuentaSet;
            EliminarAmistad(cuentaAmigo);
         }

        private void PreviewMouseDownAceptarNotificacion(object sender, MouseButtonEventArgs e)
        {
            Button btnEliminarNotificacion = sender as Button;
            NotificacionSet notificacion = btnEliminarNotificacion.CommandParameter as NotificacionSet;
            CrearAmistad(notificacion);
            
        }

        private void PreviewMouseDownRechazarNotificacion(object sender, MouseButtonEventArgs e)
        {
            EliminarNotificacionDelJugador(sender);
        }

        private void PreviewMouseDownEliminarNotificacionPartida(object sender, MouseButtonEventArgs e)
        {
            EliminarNotificacionDelJugador(sender);
        }

        private void EliminarNotificacionDelJugador(object sender)
        {
            Button btnEliminarNotificacion = sender as Button;
            NotificacionSet notificacion = btnEliminarNotificacion.CommandParameter as NotificacionSet;
            EliminarNotificacion(notificacion, false);
        }

        private void EnviarSolicitudAmistad(CuentaSet cuentaJugador) 
        {
            Logger log = new Logger(this.GetType());
            Constantes resultado;
            try
            {
                NotificacionSet notificacionNueva = new NotificacionSet
                {
                    CorreoElectronicoRemitente = _cuentaUsuario.CorreoElectronico,
                    CorreoElectronicoDestinatario = cuentaJugador.CorreoElectronico,
                    IdTipoNotificacion = _tiposNotificacion[1].IdTipoNotificacion,
                    Mensaje = ""
                };
                resultado = _proxyAmigos.GuardarNotificacion(notificacionNueva);
            }
            catch (CommunicationObjectFaultedException ex)
            {
                resultado = Constantes.ERROR_CONEXION_DEFECTUOSA;
                log.LogWarn("Se ha perdido la conexión previa", ex);
            }
            catch (CommunicationException ex)
            {
                resultado = Constantes.ERROR_CONEXION_SERVIDOR;
                log.LogWarn("Error de Comunicación con el Servidor", ex);
            }
            catch (TimeoutException ex)
            {
                resultado = Constantes.ERROR_TIEMPO_ESPERA_SERVIDOR;
                log.LogWarn("Se agoto el tiempo de espera del servidor", ex);
            }
            switch (resultado)
            {
                case Constantes.OPERACION_EXITOSA:
                    _ = MostrarMensaje(Properties.Resources.msgSolicitudAmistadEnviada);
                    NotificarUsuario(cuentaJugador.CorreoElectronico);
                    RecargarPantalla();
                    return;
                case Constantes.OPERACION_EXITOSA_VACIA:
                    Utilidades.MostrarUnMensajeError(Properties.Resources.msgNotificacionGuardarError);
                    break;
                default:
                    Utilidades.MostrarMensajesError(resultado);
                    break;
            }
            SalirAlMenu();
        }

        private void BanearJugador(CuentaSet cuentaJugador) 
        {
            Logger log = new Logger(this.GetType());
            Constantes resultado;
            try
            {
                resultado = _proxyAmigos.BanearJugador(_cuentaUsuario.CorreoElectronico ,cuentaJugador.CorreoElectronico);
            }
            catch (CommunicationObjectFaultedException ex)
            {
                resultado = Constantes.ERROR_CONEXION_DEFECTUOSA;
                log.LogWarn("Se ha perdido la conexión previa", ex);
            }
            catch (CommunicationException ex)
            {
                resultado = Constantes.ERROR_CONEXION_SERVIDOR;
                log.LogWarn("Error de Comunicación con el Servidor", ex);
            }
            catch (TimeoutException ex)
            {
                resultado = Constantes.ERROR_TIEMPO_ESPERA_SERVIDOR;
                log.LogWarn("Se agoto el tiempo de espera del servidor", ex);
            }
            switch (resultado)
            {
                case Constantes.OPERACION_EXITOSA:
                    _ = MostrarMensaje(Properties.Resources.msgBanearJugador);
                    RecargarPantalla();
                    return;
                default:
                    Utilidades.MostrarMensajesError(resultado);
                    SalirAlMenu();
                    break;
            }
        }
        private void EliminarBaneo(CuentaSet cuentaJugador) 
        {
            Logger log = new Logger(this.GetType());
            Constantes resultado;
            try
            {
                resultado = _proxyAmigos.EliminarBaneo(_cuentaUsuario.CorreoElectronico, cuentaJugador.CorreoElectronico);
            }
            catch (CommunicationObjectFaultedException ex)
            {
                resultado = Constantes.ERROR_CONEXION_DEFECTUOSA;
                log.LogWarn("Se ha perdido la conexión previa", ex);
            }
            catch (CommunicationException ex)
            {
                resultado = Constantes.ERROR_CONEXION_SERVIDOR;
                log.LogWarn("Error de Comunicación con el Servidor", ex);
            }
            catch (TimeoutException ex)
            {
                resultado = Constantes.ERROR_TIEMPO_ESPERA_SERVIDOR;
                log.LogWarn("Se agoto el tiempo de espera del servidor", ex);
            }
            switch (resultado)
            {
                case Constantes.OPERACION_EXITOSA:
                    _ = MostrarMensaje(Properties.Resources.msgEliminarBaneoJugador);
                    RecargarPantalla();
                    break;
                default:
                    Utilidades.MostrarMensajesError(resultado);
                    SalirAlMenu();
                    break;
            }
        }

        private void CrearAmistad(NotificacionSet notificacion) 
        {
            Logger log = new Logger(this.GetType());
            Constantes resultado;
            try 
            {
                resultado = _proxyAmigos.GuardarAmistad(notificacion.CorreoElectronicoDestinatario, notificacion.CorreoElectronicoRemitente);
            }
            catch (CommunicationObjectFaultedException ex)
            {
                resultado = Constantes.ERROR_CONEXION_DEFECTUOSA;
                log.LogWarn("Se ha perdido la conexión previa", ex);
            }
            catch (CommunicationException ex) 
            {
                resultado = Constantes.ERROR_CONEXION_SERVIDOR;
                log.LogWarn("Error de Comunicación con el Servidor", ex);
            } 
            catch (TimeoutException ex) 
            {
                resultado = Constantes.ERROR_TIEMPO_ESPERA_SERVIDOR;
                log.LogWarn("Se agoto el tiempo de espera del servidor", ex);
            }
            switch (resultado)
            {
                case Constantes.OPERACION_EXITOSA:
                    _ = MostrarMensaje(Properties.Resources.msgAceptarNotificacion);
                    EliminarNotificacion(notificacion, true);
                    return;
                case Constantes.OPERACION_EXITOSA_VACIA:
                    _ = MostrarMensaje(Properties.Resources.msgNoExisteNotificacion);
                    EliminarNotificacion(notificacion, true);
                    return;
                default:
                    Utilidades.MostrarMensajesError(resultado);
                    SalirAlMenu();
                    break;
            }
            
        }

        private void EliminarNotificacion(NotificacionSet notificacion, bool esEliminarDesdeAmistad) 
        {
            Logger log = new Logger(this.GetType());
            Constantes resultado;
            try
            {
                resultado = _proxyAmigos.EliminarNotificacionJugador(_cuentaUsuario.CorreoElectronico, notificacion.IdNotificacion);
            }
            catch (CommunicationObjectFaultedException ex)
            {
                resultado = Constantes.ERROR_CONEXION_DEFECTUOSA;
                log.LogWarn("Se ha perdido la conexión previa", ex);
            }
            catch (CommunicationException ex)
            {
                resultado = Constantes.ERROR_CONEXION_SERVIDOR;
                log.LogWarn("Error de Comunicación con el Servidor", ex);
            }
            catch (TimeoutException ex)
            {
                resultado = Constantes.ERROR_TIEMPO_ESPERA_SERVIDOR;
                log.LogWarn("Se agoto el tiempo de espera del servidor", ex);
            }
            switch (resultado)
            {
                case Constantes.OPERACION_EXITOSA:
                    if (!esEliminarDesdeAmistad)
                    {
                        _ = MostrarMensaje(Properties.Resources.msgEliminarNotificacion);
                    }
                    NotificarUsuario(notificacion.CorreoElectronicoRemitente);
                    RecargarPantalla();
                    return;
                case Constantes.OPERACION_EXITOSA_VACIA:
                    Utilidades.MostrarUnMensajeError(Properties.Resources.msgErrorEliminarNotificacionEliminada);
                    break;
                default:
                    Utilidades.MostrarMensajesError(resultado);
                    break;
            }
            SalirAlMenu();
        }

        private void EliminarAmistad(CuentaSet cuentaAmigo) 
        {
            Logger log = new Logger(this.GetType());
            Constantes resultado;
            try
            {
                resultado = _proxyAmigos.EliminarAmistad(_cuentaUsuario.CorreoElectronico, cuentaAmigo.CorreoElectronico);
            }
            catch (CommunicationObjectFaultedException ex)
            {
                resultado = Constantes.ERROR_CONEXION_DEFECTUOSA;
                log.LogWarn("Se ha perdido la conexión previa", ex);
            }
            catch (CommunicationException ex)
            {
                resultado = Constantes.ERROR_CONEXION_SERVIDOR;
                log.LogWarn("Error de Comunicación con el Servidor", ex);
            }
            catch (TimeoutException ex)
            {
                resultado = Constantes.ERROR_TIEMPO_ESPERA_SERVIDOR;
                log.LogWarn("Se agoto el tiempo de espera del servidor", ex);
            }
            switch (resultado)
            {
                case Constantes.OPERACION_EXITOSA:
                    _ = MostrarMensaje(Properties.Resources.msgEliminarAmigo);
                    NotificarUsuario(cuentaAmigo.CorreoElectronico);
                    RecargarPantalla();
                    return;
                case Constantes.OPERACION_EXITOSA_VACIA:
                    Utilidades.MostrarUnMensajeError(Properties.Resources.msgAmistadEliminarError);
                    break;
                default:
                    Utilidades.MostrarMensajesError(resultado);
                    break;
            }
            SalirAlMenu();
        }

        private void NotificarUsuario(string correoJugadorNotificado)
        {
            Logger log = new Logger(this.GetType());
            try
            {
                _proxyAmigos.NotificarUsuario(correoJugadorNotificado);
            }
            catch (CommunicationObjectFaultedException ex)
            {
                log.LogWarn("Se ha perdido la conexión previa", ex);
            }
            catch (CommunicationException ex)
            {
                log.LogWarn("Error de Comunicación con el Servidor", ex);
            }
            catch (TimeoutException ex)
            {
                log.LogWarn("Se agoto el tiempo de espera del servidor", ex);
            }
        }

        private void RecargarPantalla() 
        {
            NavigationService.Refresh();
            _baneados = null;
            _notificaciones = null;
            _cuentasSolicitudesPendientes = null;
            _amigos = null;
            _listaTodasCuentasBuscadas = null;
            _listaTodasCuentasBuscadas = new List<CuentaSet>();

            for (int i = lstBoxJugadores.Items.Count - 1; i > -1; i--) 
            {
                lstBoxJugadores.Items.RemoveAt(i);
            }
            lstBoxAmigos.ItemsSource = null;
            for (int i = lstBoxAmigos.Items.Count - 1; i > -1; i--)
            {
                lstBoxAmigos.Items.RemoveAt(i);
            }

            for (int i = lstBoxNotificaciones.Items.Count - 1; i > -1; i--)
            {
                lstBoxNotificaciones.Items.RemoveAt(i);
            }
            RecuperarBaneados();
            RecuperarCuentasConSolicituPendiente();
            CargarAmigos();
            CargarNotificaciones();
            BuscarJugadores();
        }

        public void RecuperarNotificacion()
        {
            Console.WriteLine("Notificaciones");
            RecargarPantalla();
        }

        private void ClickBuscarJugadores(object sender, RoutedEventArgs e)
        {
            BuscarJugadores();
        }

        private void BuscarJugadores()
        {
            string informacionJugador = txtBoxBuscador.Text;
            if (!string.IsNullOrWhiteSpace(informacionJugador))
            {
                RecuperarJugadoresEspecificos(informacionJugador);
            }
        }

        private async Task MostrarMensaje(string mensaje)
        {
            txtBlockMensaje.Text = mensaje;
            brdMensaje.Visibility = Visibility.Visible;
            await Task.Delay(2500);
            brdMensaje.Visibility = Visibility.Hidden;
        }

        private void TextChangedBuscadorJugadores(object sender, TextChangedEventArgs e)
        {
            int tamañoMaximoCorreo = 100;
            if (txtBoxBuscador.Text.Length > tamañoMaximoCorreo)
            {
                txtBoxBuscador.Text = txtBoxBuscador.Text.Substring(0, tamañoMaximoCorreo);
                txtBoxBuscador.SelectionStart = txtBoxBuscador.Text.Length;
            }
        }
    }
}
