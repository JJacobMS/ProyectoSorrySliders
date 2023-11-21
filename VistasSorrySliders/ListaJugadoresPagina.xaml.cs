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
        private List<CuentaSet> _lista;
        private List<NotificacionSet> _notificaciones;
        private NotificarJugadoresClient _proxyNotificar;
        private CuentaSet _cuentaUsuario;
        private ListaAmigosClient _proxyAmigos;
        private List<TipoNotificacion> _tiposNotificacion;

        public ListaJugadoresPagina(CuentaSet cuenta)
        {
            InitializeComponent();
            _cuentaUsuario = cuenta;
            //_lista = new List<CuentaSet>();
            _lista = new List<CuentaSet> {new CuentaSet { Nickname = "Jacobobo", CorreoElectronico= "Jacobobo@gmail.com"},
                new CuentaSet { Nickname = "Jacobo", CorreoElectronico= "jacob123@gmail.com"},
                new CuentaSet { Nickname = "melus", CorreoElectronico = "waos@gmail.com"}
            };
            CrearProxyAmigos();
            GuardarContexto();
            RecuperarTiposNotificacion();
            RecuperarBaneados();
            RecuperarPendientes();
            CargarAmigos();
            CargarNotificaciones();
            MostrarJugadores();
        }

        private void CrearProxyAmigos()
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
                log.LogError("Error de Comunicación con el Servidor", ex);
            }
            catch (TimeoutException ex)
            {
                resultado = Constantes.ERROR_TIEMPO_ESPERA_SERVIDOR;
                log.LogWarn("Se agoto el tiempo de espera del servidor", ex);
            }
            catch (Exception ex)
            {
                resultado = Constantes.ERROR_CONEXION_SERVIDOR;
                log.LogFatal("Ha ocurrido un error inesperado", ex);
            }
            switch (resultado)
            {
                case Constantes.ERROR_CONEXION_SERVIDOR:
                    MessageBox.Show(Properties.Resources.msgErrorConexion);
                    break;
                case Constantes.ERROR_TIEMPO_ESPERA_SERVIDOR:
                    MessageBox.Show(Properties.Resources.msgErrorTiempoEsperaServidor);
                    break;
            }
        }

        private void GuardarContexto()
        {
            Logger log = new Logger(this.GetType());
            Constantes resultado;
            try
            {
                InstanceContext contexto = new InstanceContext(this);
                _proxyNotificar = new NotificarJugadoresClient(contexto);
                resultado = _proxyNotificar.AgregarProxy(_cuentaUsuario.CorreoElectronico);
            }
            catch (CommunicationException ex)
            {
                resultado = Constantes.ERROR_CONEXION_SERVIDOR;
                log.LogError("Error de Comunicación con el Servidor", ex);
            }
            catch (TimeoutException ex)
            {
                resultado = Constantes.ERROR_TIEMPO_ESPERA_SERVIDOR;
                log.LogWarn("Se agoto el tiempo de espera del servidor", ex);
            }
            catch (Exception ex)
            {
                resultado = Constantes.ERROR_CONEXION_SERVIDOR;
                log.LogFatal("Ha ocurrido un error inesperado", ex);
            }
            switch (resultado)
            {
                case Constantes.ERROR_CONEXION_SERVIDOR:
                    MessageBox.Show(Properties.Resources.msgErrorConexion);
                    break;
                case Constantes.ERROR_TIEMPO_ESPERA_SERVIDOR:
                    MessageBox.Show(Properties.Resources.msgErrorTiempoEsperaServidor);
                    break;
            }
        }


        private void RecuperarTiposNotificacion()
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
                }
            }
            catch (CommunicationException ex)
            {
                resultado = Constantes.ERROR_CONEXION_SERVIDOR;
                log.LogError("Error de Comunicación con el Servidor", ex);
            }
            catch (TimeoutException ex)
            {
                resultado = Constantes.ERROR_TIEMPO_ESPERA_SERVIDOR;
                log.LogWarn("Se agoto el tiempo de espera del servidor", ex);
            }
            catch (Exception ex)
            {
                resultado = Constantes.ERROR_CONEXION_SERVIDOR;
                log.LogFatal("Ha ocurrido un error inesperado", ex);
            }
            switch (resultado)
            {
                case Constantes.ERROR_CONEXION_BD:
                    MessageBox.Show(Properties.Resources.msgErrorBaseDatos);
                    break;
                case Constantes.ERROR_CONSULTA:
                    MessageBox.Show(Properties.Resources.msgErrorBaseDatos);
                    break;
                case Constantes.ERROR_CONEXION_SERVIDOR:
                    MessageBox.Show(Properties.Resources.msgErrorConexion);
                    break;
                case Constantes.OPERACION_EXITOSA_VACIA:
                    MessageBox.Show(Properties.Resources.msgTiposNotificacionVacios);
                    break;
                case Constantes.ERROR_TIEMPO_ESPERA_SERVIDOR:
                    MessageBox.Show(Properties.Resources.msgErrorTiempoEsperaServidor);
                    break;
            }
        }
        private void RecuperarJugadores()
        {
            /*
            Constantes resultado;
            Logger log = new Logger(this.GetType());
            CuentaSet[] listaJugadores;
            try
            {
                (resultado, listaJugadores) = _proxyAmigos.RecuperarTodosLosJugadores();
                if (resultado == Constantes.OPERACION_EXITOSA)
                {
                   _lista = listaJugadores.ToList();
                }
            }
            catch (CommunicationException ex)
            {
                resultado = Constantes.ERROR_CONEXION_SERVIDOR;
                log.LogError("Error de Comunicación con el Servidor", ex);
            }
            catch (TimeoutException ex)
            {
                resultado = Constantes.ERROR_TIEMPO_ESPERA_SERVIDOR;
                log.LogWarn("Se agoto el tiempo de espera del servidor", ex);
            }
            catch (Exception ex)
            {
                resultado = Constantes.ERROR_CONEXION_SERVIDOR;
                log.LogFatal("Ha ocurrido un error inesperado", ex);
            }
            switch (resultado)
            {
                case Constantes.OPERACION_EXITOSA:
                    Console.WriteLine("EXITOSO");
                    break;
                case Constantes.OPERACION_EXITOSA_VACIA:
                    Console.WriteLine("VACIA");
                    break;
                case Constantes.ERROR_CONEXION_BD:
                    MessageBox.Show(Properties.Resources.msgErrorBaseDatos);
                    break;
                case Constantes.ERROR_CONSULTA:
                    MessageBox.Show(Properties.Resources.msgErrorConsulta);
                    break;
                case Constantes.ERROR_CONEXION_SERVIDOR:
                    MessageBox.Show(Properties.Resources.msgErrorConexion);
                    break;
                case Constantes.ERROR_TIEMPO_ESPERA_SERVIDOR:
                    MessageBox.Show(Properties.Resources.msgErrorTiempoEsperaServidor);
                    break;
            }
            */
        }

        private void RecuperarBaneados()
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
                }
            }
            catch (CommunicationException ex)
            {
                resultado = Constantes.ERROR_CONEXION_SERVIDOR;
                log.LogError("Error de Comunicación con el Servidor", ex);
            }
            catch (TimeoutException ex)
            {
                resultado = Constantes.ERROR_TIEMPO_ESPERA_SERVIDOR;
                log.LogWarn("Se agoto el tiempo de espera del servidor", ex);
            }
            catch (Exception ex)
            {
                resultado = Constantes.ERROR_CONEXION_SERVIDOR;
                log.LogFatal("Ha ocurrido un error inesperado", ex);
            }
            switch (resultado)
            {
                case Constantes.OPERACION_EXITOSA:
                    //HAY BANEADOS
                    Console.WriteLine("EXITOSO");
                    break;
                case Constantes.OPERACION_EXITOSA_VACIA:
                    //NO HAY BANEADOS
                    Console.WriteLine("VACIA");
                    break;
                case Constantes.ERROR_CONEXION_BD:
                    MessageBox.Show(Properties.Resources.msgErrorBaseDatos);
                    break;
                case Constantes.ERROR_CONSULTA:
                    MessageBox.Show(Properties.Resources.msgErrorConsulta);
                    break;
                case Constantes.ERROR_CONEXION_SERVIDOR:
                    MessageBox.Show(Properties.Resources.msgErrorConexion);
                    break;
                case Constantes.ERROR_TIEMPO_ESPERA_SERVIDOR:
                    MessageBox.Show(Properties.Resources.msgErrorTiempoEsperaServidor);
                    break;
            }
        }

        private void RecuperarPendientes()
        {
            Constantes resultado;
            Logger log = new Logger(this.GetType());
            CuentaSet[] listaPendientes;
            try
            {
                (resultado, listaPendientes) = _proxyAmigos.RecuperarSolicitudesAmistad(_cuentaUsuario.CorreoElectronico);

                if (resultado == Constantes.OPERACION_EXITOSA)
                {
                    _cuentasSolicitudesPendientes = listaPendientes.ToList();

                }
            }
            catch (CommunicationException ex)
            {
                resultado = Constantes.ERROR_CONEXION_SERVIDOR;
                log.LogError("Error de Comunicación con el Servidor", ex);
            }
            catch (TimeoutException ex)
            {
                resultado = Constantes.ERROR_TIEMPO_ESPERA_SERVIDOR;
                log.LogWarn("Se agoto el tiempo de espera del servidor", ex);
            }
            catch (Exception ex)
            {
                resultado = Constantes.ERROR_CONEXION_SERVIDOR;
                log.LogFatal("Ha ocurrido un error inesperado", ex);
            }
            switch (resultado)
            {
                case Constantes.OPERACION_EXITOSA:
                    //HAY BANEADOS
                    Console.WriteLine("EXITOSO");
                    break;
                case Constantes.OPERACION_EXITOSA_VACIA:
                    //NO HAY BANEADOS
                    Console.WriteLine("VACIA");
                    break;
                case Constantes.ERROR_CONEXION_BD:
                    MessageBox.Show(Properties.Resources.msgErrorBaseDatos);
                    break;
                case Constantes.ERROR_CONSULTA:
                    MessageBox.Show(Properties.Resources.msgErrorConsulta);
                    break;
                case Constantes.ERROR_CONEXION_SERVIDOR:
                    MessageBox.Show(Properties.Resources.msgErrorConexion);
                    break;
                case Constantes.ERROR_TIEMPO_ESPERA_SERVIDOR:
                    MessageBox.Show(Properties.Resources.msgErrorTiempoEsperaServidor);
                    break;
            }
        }


        private void MostrarJugadores()
        {
            lstBoxJugadores.Items.Clear();
            if (_baneados != null && _baneados.Count > 0)
            {
                _lista.AddRange(_baneados);
            }
            if (_cuentasSolicitudesPendientes != null && _cuentasSolicitudesPendientes.Count > 0)
            {
                _lista.AddRange(_cuentasSolicitudesPendientes);
            }
            if (_amigos != null && _amigos.Count > 0)
            {
                _lista.AddRange(_amigos);
            }
            EliminarDuplicados();
            if (_lista != null && _lista.Count() > 0)
            {
                foreach (CuentaSet cuenta in _lista)
                {
                    ListBoxItem lstBoxItemCuenta = new ListBoxItem
                    {
                        DataContext = cuenta
                    };
                    Console.WriteLine("es cuenta " + cuenta.CorreoElectronico);

                    if (EsCuentaAmigo(cuenta))
                    {
                        lstBoxItemCuenta.Style = (Style)FindResource("estiloLstBoxItemAmigo");
                        Console.WriteLine("es amigo " + cuenta.CorreoElectronico);
                    }
                    if (EsPendienteRecibida(cuenta))
                    {
                        lstBoxItemCuenta.Style = (Style)FindResource("estiloLstBoxItemJugadorPendiente");
                        Console.WriteLine("es pendiente " + cuenta.CorreoElectronico);
                    }
                    if (EsCuentaBaneada(cuenta))
                    {
                        lstBoxItemCuenta.Style = (Style)FindResource("estiloLstBoxItemJugadorBaneado");
                        Console.WriteLine("es baneado " + cuenta.CorreoElectronico);
                    }
                    Console.WriteLine("es añadido " + cuenta.CorreoElectronico);
                    lstBoxJugadores.Items.Add(lstBoxItemCuenta);
                }
            }
        }

        private void CargarAmigos()
        {
            Constantes resultado;
            Logger log = new Logger(this.GetType());
            CuentaSet[] listaAmigos;
            try
            {
                (resultado, listaAmigos) = _proxyAmigos.RecuperarAmigos(_cuentaUsuario.CorreoElectronico);
                if (resultado == Constantes.OPERACION_EXITOSA)
                {
                    _amigos = listaAmigos.ToList();
                }
            }
            catch (CommunicationException ex)
            {
                resultado = Constantes.ERROR_CONEXION_SERVIDOR;
                log.LogError("Error de Comunicación con el Servidor", ex);
            }
            catch (TimeoutException ex)
            {
                resultado = Constantes.ERROR_TIEMPO_ESPERA_SERVIDOR;
                log.LogWarn("Se agoto el tiempo de espera del servidor", ex);
            }
            catch (Exception ex)
            {
                resultado = Constantes.ERROR_CONEXION_SERVIDOR;
                log.LogFatal("Ha ocurrido un error inesperado", ex);
            }
            switch (resultado)
            {
                case Constantes.OPERACION_EXITOSA:
                    lstBoxAmigos.ItemsSource = null;
                    lstBoxAmigos.Items.Clear();
                    lstBoxAmigos.ItemsSource = _amigos;
                    break;
                case Constantes.OPERACION_EXITOSA_VACIA:
                    lstBoxAmigos.ItemsSource = null;
                    lstBoxAmigos.Items.Clear();
                    break;
                case Constantes.ERROR_CONEXION_BD:
                    MessageBox.Show(Properties.Resources.msgErrorBaseDatos);
                    break;
                case Constantes.ERROR_CONSULTA:
                    MessageBox.Show(Properties.Resources.msgErrorConsulta);
                    break;
                case Constantes.ERROR_CONEXION_SERVIDOR:
                    MessageBox.Show(Properties.Resources.msgErrorConexion);
                    break;
                case Constantes.ERROR_TIEMPO_ESPERA_SERVIDOR:
                    MessageBox.Show(Properties.Resources.msgErrorTiempoEsperaServidor);
                    break;
            }
        }


        private void CargarNotificaciones()
        {
            lstBoxNotificaciones.Items.Clear();
            Constantes resultado;
            Logger log = new Logger(this.GetType());
            try
            {
                NotificacionSet[] listaNotificacionesResultado;
                (resultado, listaNotificacionesResultado) = _proxyAmigos.RecuperarNotificaciones(_cuentaUsuario.CorreoElectronico);
                if (resultado == Constantes.OPERACION_EXITOSA)
                {
                    _notificaciones = listaNotificacionesResultado.ToList();
                    MostrarNotificaciones();
                    return;
                }
            }
            catch (CommunicationException ex)
            {
                resultado = Constantes.ERROR_CONEXION_SERVIDOR;
                log.LogError("Error de Comunicación con el Servidor", ex);
            }
            catch (TimeoutException ex)
            {
                resultado = Constantes.ERROR_TIEMPO_ESPERA_SERVIDOR;
                log.LogWarn("Se agoto el tiempo de espera del servidor", ex);
            }
            catch (Exception ex)
            {
                resultado = Constantes.ERROR_CONEXION_SERVIDOR;
                log.LogFatal("Ha ocurrido un error inesperado", ex);
            }
            switch (resultado)
            {
                case Constantes.OPERACION_EXITOSA:

                    break;
                case Constantes.OPERACION_EXITOSA_VACIA:

                    MessageBox.Show(Properties.Resources.msgNotificacionRecuperar);
                    break;
                case Constantes.ERROR_CONEXION_BD:
                    MessageBox.Show(Properties.Resources.msgErrorBaseDatos);
                    break;
                case Constantes.ERROR_CONSULTA:
                    MessageBox.Show(Properties.Resources.msgErrorConsulta);
                    break;
                case Constantes.ERROR_CONEXION_SERVIDOR:
                    MessageBox.Show(Properties.Resources.msgErrorConexion);
                    break;
                case Constantes.ERROR_TIEMPO_ESPERA_SERVIDOR:
                    MessageBox.Show(Properties.Resources.msgErrorTiempoEsperaServidor);
                    break;
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

            foreach (var cuenta in _lista)
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
            _lista = listaSinDuplicados;

        }

        private bool EsCuentaBaneada(CuentaSet cuentaComparar)
        {
            if (_baneados != null)
            {
                foreach (CuentaSet cuenta in _baneados)
                {
                    Console.WriteLine("Baneado añadido");
                    if (cuenta.CorreoElectronico.Equals(cuentaComparar.CorreoElectronico))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private bool EsNotificacionCuentaBaneada(NotificacionSet notificacion)
        {
            if (_baneados != null)
            {
                foreach (CuentaSet cuenta in _baneados)
                {
                    Console.WriteLine("Baneado añadido");
                    if (cuenta.CorreoElectronico.Equals(notificacion.CorreoElectronicoRemitente))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private bool EsCuentaAmigo(CuentaSet cuentaComparar)
        {
            if (_amigos != null)
            {
                foreach (CuentaSet cuenta in _amigos)
                {
                    if (cuenta.CorreoElectronico.Equals(cuentaComparar.CorreoElectronico))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private bool EsPendienteRecibida(CuentaSet cuentaComparar)
        {
            if (_cuentasSolicitudesPendientes != null)
            {
                foreach (CuentaSet cuenta in _cuentasSolicitudesPendientes)
                {
                    if (cuenta.CorreoElectronico.Equals(cuentaComparar.CorreoElectronico))
                    {
                        return true;
                    }
                }
            }
            return false;
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
            catch (CommunicationException ex)
            {
                log.LogError("Error de Comunicación con el Servidor", ex);
            }
            catch (TimeoutException ex)
            {
                log.LogWarn("Se agoto el tiempo de espera del servidor", ex);
            }
            catch (Exception ex)
            {
                log.LogFatal("Ha ocurrido un error inesperado", ex);
            }
            this.NavigationService.GoBack();
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
            Button btnEliminarNotificacion = sender as Button;
            NotificacionSet notificacion = btnEliminarNotificacion.CommandParameter as NotificacionSet;
            EliminarNotificacion(notificacion);
        }

        private void PreviewMouseDownEliminarNotificacionPartida(object sender, MouseButtonEventArgs e)
        {
            Button btnEliminarNotificacion = sender as Button;
            NotificacionSet notificacion = btnEliminarNotificacion.CommandParameter as NotificacionSet;
            EliminarNotificacion(notificacion);
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
            catch (CommunicationException ex)
            {
                resultado = Constantes.ERROR_CONEXION_SERVIDOR;
                log.LogError("Error de Comunicación con el Servidor", ex);
            }
            catch (TimeoutException ex)
            {
                resultado = Constantes.ERROR_TIEMPO_ESPERA_SERVIDOR;
                log.LogWarn("Se agoto el tiempo de espera del servidor", ex);
            }
            catch (Exception ex)
            {
                resultado = Constantes.ERROR_CONEXION_SERVIDOR;
                log.LogFatal("Ha ocurrido un error inesperado", ex);
            }
            switch (resultado)
            {
                case Constantes.OPERACION_EXITOSA:
                    NotificarUsuario(cuentaJugador.CorreoElectronico);
                    RecargarPantalla();
                    break;
                case Constantes.ERROR_CONEXION_BD:
                    MessageBox.Show(Properties.Resources.msgErrorBaseDatos);
                    break;
                case Constantes.ERROR_CONSULTA:
                    MessageBox.Show(Properties.Resources.msgErrorConsulta);
                    break;
                case Constantes.ERROR_CONEXION_SERVIDOR:
                    MessageBox.Show(Properties.Resources.msgErrorConexion);
                    break;
                case Constantes.OPERACION_EXITOSA_VACIA:
                    MessageBox.Show(Properties.Resources.msgNotificacionGuardarError);
                    break;
                case Constantes.ERROR_TIEMPO_ESPERA_SERVIDOR:
                    MessageBox.Show(Properties.Resources.msgErrorTiempoEsperaServidor);
                    break;
            }
        }

        private void BanearJugador(CuentaSet cuentaJugador) 
        {
            Logger log = new Logger(this.GetType());
            Constantes resultado;
            try
            {
                resultado = _proxyAmigos.BanearJugador(_cuentaUsuario.CorreoElectronico ,cuentaJugador.CorreoElectronico);
            }
            catch (CommunicationException ex)
            {
                resultado = Constantes.ERROR_CONEXION_SERVIDOR;
                log.LogError("Error de Comunicación con el Servidor", ex);
            }
            catch (TimeoutException ex)
            {
                resultado = Constantes.ERROR_TIEMPO_ESPERA_SERVIDOR;
                log.LogWarn("Se agoto el tiempo de espera del servidor", ex);
            }
            catch (Exception ex)
            {
                resultado = Constantes.ERROR_CONEXION_SERVIDOR;
                log.LogFatal("Ha ocurrido un error inesperado", ex);
            }
            switch (resultado)
            {
                case Constantes.OPERACION_EXITOSA:
                    RecargarPantalla();
                    break;
                case Constantes.ERROR_CONEXION_BD:
                    MessageBox.Show(Properties.Resources.msgErrorBaseDatos);
                    break;
                case Constantes.ERROR_CONSULTA:
                    MessageBox.Show(Properties.Resources.msgErrorConsulta);
                    break;
                case Constantes.ERROR_CONEXION_SERVIDOR:
                    MessageBox.Show(Properties.Resources.msgErrorConexion);
                    break;
                case Constantes.OPERACION_EXITOSA_VACIA:
                    //NO SE PUEDE BANEAR
                    break;
                case Constantes.ERROR_TIEMPO_ESPERA_SERVIDOR:
                    MessageBox.Show(Properties.Resources.msgErrorTiempoEsperaServidor);
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
            catch (CommunicationException ex)
            {
                resultado = Constantes.ERROR_CONEXION_SERVIDOR;
                log.LogError("Error de Comunicación con el Servidor", ex);
            }
            catch (TimeoutException ex)
            {
                resultado = Constantes.ERROR_TIEMPO_ESPERA_SERVIDOR;
                log.LogWarn("Se agoto el tiempo de espera del servidor", ex);
            }
            catch (Exception ex)
            {
                resultado = Constantes.ERROR_CONEXION_SERVIDOR;
                log.LogFatal("Ha ocurrido un error inesperado", ex);
            }
            switch (resultado)
            {
                case Constantes.OPERACION_EXITOSA:
                    RecargarPantalla();
                    break;
                case Constantes.ERROR_CONEXION_BD:
                    MessageBox.Show(Properties.Resources.msgErrorBaseDatos);
                    break;
                case Constantes.ERROR_CONSULTA:
                    MessageBox.Show(Properties.Resources.msgErrorConsulta);
                    break;
                case Constantes.ERROR_CONEXION_SERVIDOR:
                    MessageBox.Show(Properties.Resources.msgErrorConexion);
                    break;
                case Constantes.OPERACION_EXITOSA_VACIA:
                    //NO SE PUEDE ELIMINAR EL BANEAO
                    break;
                case Constantes.ERROR_TIEMPO_ESPERA_SERVIDOR:
                    MessageBox.Show(Properties.Resources.msgErrorTiempoEsperaServidor);
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
            catch (CommunicationException ex) 
            {
                resultado = Constantes.ERROR_CONEXION_SERVIDOR;
                log.LogError("Error de Comunicación con el Servidor", ex);
            } 
            catch (TimeoutException ex) 
            {
                resultado = Constantes.ERROR_TIEMPO_ESPERA_SERVIDOR;
                log.LogWarn("Se agoto el tiempo de espera del servidor", ex);
            } 
            catch (Exception ex) 
            {
                resultado = Constantes.ERROR_CONEXION_SERVIDOR;
                log.LogFatal("Ha ocurrido un error inesperado", ex);
            }
            switch (resultado)
            {
                case Constantes.OPERACION_EXITOSA:
                    EliminarNotificacion(notificacion);
                    break;
                case Constantes.ERROR_CONEXION_BD:
                    MessageBox.Show(Properties.Resources.msgErrorBaseDatos);
                    break;
                case Constantes.ERROR_CONSULTA:
                    MessageBox.Show(Properties.Resources.msgErrorConsulta);
                    break;
                case Constantes.ERROR_CONEXION_SERVIDOR:
                    MessageBox.Show(Properties.Resources.msgErrorConexion);
                    break;
                case Constantes.OPERACION_EXITOSA_VACIA:


                    break;
                case Constantes.ERROR_TIEMPO_ESPERA_SERVIDOR:
                    MessageBox.Show(Properties.Resources.msgErrorTiempoEsperaServidor);
                    break;
            }
            
        }

        private void EliminarNotificacion(NotificacionSet notificacion) 
        {
            Logger log = new Logger(this.GetType());
            Constantes resultado;
            try
            {
                resultado = _proxyAmigos.EliminarNotificacionJugador(_cuentaUsuario.CorreoElectronico, notificacion.IdNotificacion);
            }
            catch (CommunicationException ex)
            {
                resultado = Constantes.ERROR_CONEXION_SERVIDOR;
                log.LogError("Error de Comunicación con el Servidor", ex);
            }
            catch (TimeoutException ex)
            {
                resultado = Constantes.ERROR_TIEMPO_ESPERA_SERVIDOR;
                log.LogWarn("Se agoto el tiempo de espera del servidor", ex);
            }
            catch (Exception ex)
            {
                resultado = Constantes.ERROR_CONEXION_SERVIDOR;
                log.LogFatal("Ha ocurrido un error inesperado", ex);
            }
            switch (resultado)
            {
                case Constantes.OPERACION_EXITOSA:
                    NotificarUsuario(notificacion.CorreoElectronicoRemitente);
                    RecargarPantalla();
                    break;
                case Constantes.OPERACION_EXITOSA_VACIA:
                    MessageBox.Show(Properties.Resources.msgAmistadEliminarError);
                    break;
                case Constantes.ERROR_CONEXION_BD:
                    MessageBox.Show(Properties.Resources.msgErrorBaseDatos);
                    break;
                case Constantes.ERROR_CONSULTA:
                    MessageBox.Show(Properties.Resources.msgErrorConsulta);
                    break;
                case Constantes.ERROR_CONEXION_SERVIDOR:
                    MessageBox.Show(Properties.Resources.msgErrorConexion);
                    break;
                case Constantes.ERROR_TIEMPO_ESPERA_SERVIDOR:
                    MessageBox.Show(Properties.Resources.msgErrorTiempoEsperaServidor);
                    break;
            }
        }

        private void EliminarAmistad(CuentaSet cuentaAmigo) 
        {
            Logger log = new Logger(this.GetType());
            Constantes resultado;
            try
            {
                resultado = _proxyAmigos.EliminarAmistad(_cuentaUsuario.CorreoElectronico, cuentaAmigo.CorreoElectronico);
            }
            catch (CommunicationException ex)
            {
                resultado = Constantes.ERROR_CONEXION_SERVIDOR;
                log.LogError("Error de Comunicación con el Servidor", ex);
            }
            catch (TimeoutException ex)
            {
                resultado = Constantes.ERROR_TIEMPO_ESPERA_SERVIDOR;
                log.LogWarn("Se agoto el tiempo de espera del servidor", ex);
            }
            catch (Exception ex)
            {
                resultado = Constantes.ERROR_CONEXION_SERVIDOR;
                log.LogFatal("Ha ocurrido un error inesperado", ex);
            }
            switch (resultado)
            {
                case Constantes.OPERACION_EXITOSA:
                    //NOTIFICAR AL OTRO JUGADOR
                    NotificarUsuario(cuentaAmigo.CorreoElectronico);
                    RecargarPantalla();
                    break;
                case Constantes.OPERACION_EXITOSA_VACIA:
                    MessageBox.Show(Properties.Resources.msgAmistadEliminarError);
                    break;
                case Constantes.ERROR_CONEXION_BD:
                    MessageBox.Show(Properties.Resources.msgErrorBaseDatos);
                    break;
                case Constantes.ERROR_CONSULTA:
                    MessageBox.Show(Properties.Resources.msgErrorConsulta);
                    break;
                case Constantes.ERROR_CONEXION_SERVIDOR:
                    MessageBox.Show(Properties.Resources.msgErrorConexion);
                    break;
                case Constantes.ERROR_TIEMPO_ESPERA_SERVIDOR:
                    MessageBox.Show(Properties.Resources.msgErrorTiempoEsperaServidor);
                    break;
            }
        }

        private void NotificarUsuario(string correoJugadorNotificado)
        {
            Logger log = new Logger(this.GetType());
            try
            {
                _proxyAmigos.NotificarUsuario(correoJugadorNotificado);
            }
            catch (CommunicationException ex)
            {
                log.LogError("Error de Comunicación con el Servidor", ex);
            }
            catch (TimeoutException ex)
            {
                log.LogWarn("Se agoto el tiempo de espera del servidor", ex);
            }
            catch (Exception ex)
            {
                log.LogFatal("Ha ocurrido un error inesperado", ex);
            }
        }

        private void RecargarPantalla() 
        {
            NavigationService.Refresh();
            _baneados = null;
            _notificaciones = null;
            _cuentasSolicitudesPendientes = null;
            _amigos = null;
            _lista = null;
            _lista = new List<CuentaSet>();

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
            RecuperarPendientes();
            CargarAmigos();
            CargarNotificaciones();
            MostrarJugadores();
        }


        public void RecuperarNotificacion()
        {
           
            RecargarPantalla();
        }
    }
}
