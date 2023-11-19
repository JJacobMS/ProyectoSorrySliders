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
            CrearProxyAmigos();
            GuardarContexto();
            
            _lista = new List<CuentaSet> {new CuentaSet { Nickname = "jazmin", CorreoElectronico= "jazmin123@gmail.com"}, 
                new CuentaSet {Nickname= "sulem", CorreoElectronico = "sulem477@gmail.com"},
                new CuentaSet { Nickname = "Jacobo", CorreoElectronico= "jacob123@gmail.com"}, 
                new CuentaSet { Nickname = "melus", CorreoElectronico = "waos@gmail.com"}
            };
            

            _baneados = new List<CuentaSet> { 
                new CuentaSet { Nickname = "sulem", CorreoElectronico= "sulem477@gmail.com"},
                new CuentaSet { Nickname = "Jacobo", CorreoElectronico = "jacob123@gmail.com"}
            };

            RecuperarTiposNotificacion();
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
                Console.WriteLine(ex);
                resultado = Constantes.ERROR_CONEXION_SERVIDOR;
                log.LogError("Error de Comunicación con el Servidor", ex);
            }
            catch (TimeoutException ex)
            {
                Console.WriteLine(ex);
                resultado = Constantes.ERROR_CONEXION_SERVIDOR;
                log.LogWarn("Se agoto el tiempo de espera del servidor", ex);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                resultado = Constantes.ERROR_CONEXION_SERVIDOR;
                log.LogFatal("Ha ocurrido un error inesperado", ex);
            }
            switch (resultado)
            {
                case Constantes.OPERACION_EXITOSA:
                    break;
                case Constantes.OPERACION_EXITOSA_VACIA:
                    break;
                case Constantes.ERROR_CONEXION_BD:
                    MessageBox.Show(Properties.Resources.msgErrorBaseDatos);
                    break;
                case Constantes.ERROR_CONSULTA:
                    MessageBox.Show(Properties.Resources.msgErrorBaseDatos);
                    break;
                case Constantes.ERROR_CONEXION_SERVIDOR:
                    MessageBox.Show(Properties.Resources.msgErrorConexion);
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
                Console.WriteLine(ex);
                resultado = Constantes.ERROR_CONEXION_SERVIDOR;
                log.LogError("Error de Comunicación con el Servidor", ex);
            }
            catch (TimeoutException ex)
            {
                Console.WriteLine(ex);
                resultado = Constantes.ERROR_CONEXION_SERVIDOR;
                log.LogWarn("Se agoto el tiempo de espera del servidor", ex);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                resultado = Constantes.ERROR_CONEXION_SERVIDOR;
                log.LogFatal("Ha ocurrido un error inesperado", ex);
            }
            switch (resultado)
            {
                case Constantes.OPERACION_EXITOSA:
                    break;
                case Constantes.OPERACION_EXITOSA_VACIA:
                    break;
                case Constantes.ERROR_CONEXION_BD:
                    MessageBox.Show(Properties.Resources.msgErrorBaseDatos);
                    break;
                case Constantes.ERROR_CONSULTA:
                    MessageBox.Show(Properties.Resources.msgErrorBaseDatos);
                    break;
                case Constantes.ERROR_CONEXION_SERVIDOR:
                    MessageBox.Show(Properties.Resources.msgErrorConexion);
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
                Console.WriteLine(ex);
                resultado = Constantes.ERROR_CONEXION_SERVIDOR;
                log.LogError("Error de Comunicación con el Servidor", ex);
            }
            catch (TimeoutException ex)
            {
                Console.WriteLine(ex);
                resultado = Constantes.ERROR_CONEXION_SERVIDOR;
                log.LogWarn("Se agoto el tiempo de espera del servidor", ex);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
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
                case Constantes.OPERACION_EXITOSA:
                    break;
                case Constantes.OPERACION_EXITOSA_VACIA:
                    MessageBox.Show(Properties.Resources.msgErrorBaseDatos);
                    break;
                default:
                    break;
            }
        }

        private void MostrarJugadores()
        {
            lstBoxJugadores.Items.Clear();
            if (_lista != null)
            {
                foreach (CuentaSet cuenta in _lista)
                {
                    ListBoxItem lstBoxItemCuenta = new ListBoxItem
                    {
                        DataContext = cuenta
                    };
                    if (EsCuentaBaneada(cuenta))
                    {
                        lstBoxItemCuenta.Style = (Style)FindResource("estiloLstBoxItemJugadorBaneado");
                    }
                    if (EsCuentaAmigo(cuenta))
                    {
                        lstBoxItemCuenta.Style = (Style)FindResource("estiloLstBoxItemAmigo");
                    }
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
                Console.WriteLine(ex);
                resultado = Constantes.ERROR_CONEXION_SERVIDOR;
                log.LogError("Error de Comunicación con el Servidor", ex);
            }
            catch (TimeoutException ex)
            {
                Console.WriteLine(ex);
                resultado = Constantes.ERROR_CONEXION_SERVIDOR;
                log.LogWarn("Se agoto el tiempo de espera del servidor", ex);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                resultado = Constantes.ERROR_CONEXION_SERVIDOR;
                log.LogFatal("Ha ocurrido un error inesperado", ex);
            }
            switch (resultado)
            {
                case Constantes.OPERACION_EXITOSA:
                    Console.WriteLine("OPERACION_EXITOSA");
                    lstBoxAmigos.ItemsSource = null;
                    lstBoxAmigos.Items.Clear();
                    lstBoxAmigos.ItemsSource = _amigos;
                    //NOTIFICAR AL OTRO USUARIO QUE RECARGUE LA PANTALLA
                    
                    break;
                case Constantes.OPERACION_EXITOSA_VACIA:
                    Console.WriteLine("OPERACION_EXITOSA_VACIA");
                    lstBoxAmigos.ItemsSource = null;
                    lstBoxAmigos.Items.Clear();
                    //lstBoxAmigos.ItemsSource = _amigos;

                    break;
                case Constantes.ERROR_CONEXION_BD:
                    MessageBox.Show(Properties.Resources.msgErrorBaseDatos);
                    break;
                case Constantes.ERROR_CONSULTA:
                    MessageBox.Show(Properties.Resources.msgErrorBaseDatos);
                    break;
                case Constantes.ERROR_CONEXION_SERVIDOR:
                    MessageBox.Show(Properties.Resources.msgErrorConexion);
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
                }
            }
            catch (CommunicationException ex)
            {
                Console.WriteLine(ex);
                resultado = Constantes.ERROR_CONEXION_SERVIDOR;
                log.LogError("Error de Comunicación con el Servidor", ex);
            }
            catch (TimeoutException ex)
            {
                Console.WriteLine(ex);
                resultado = Constantes.ERROR_CONEXION_SERVIDOR;
                log.LogWarn("Se agoto el tiempo de espera del servidor", ex);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                resultado = Constantes.ERROR_CONEXION_SERVIDOR;
                log.LogFatal("Ha ocurrido un error inesperado", ex);
            }
            switch (resultado)
            {
                case Constantes.OPERACION_EXITOSA:
                    MostrarNotificaciones();
                    break;
                case Constantes.OPERACION_EXITOSA_VACIA:
                    break;
                case Constantes.ERROR_CONEXION_BD:
                    MessageBox.Show(Properties.Resources.msgErrorBaseDatos);
                    break;
                case Constantes.ERROR_CONSULTA:
                    MessageBox.Show(Properties.Resources.msgErrorBaseDatos);
                    break;
                case Constantes.ERROR_CONEXION_SERVIDOR:
                    MessageBox.Show(Properties.Resources.msgErrorConexion);
                    break;
            }
        }

        private void MostrarNotificaciones()
        {
            if(_notificaciones != null) {
                foreach (NotificacionSet notificacion in _notificaciones)
                {
                    ListBoxItem lstBoxItemCuenta = new ListBoxItem
                    {
                        DataContext = notificacion
                    };
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


       

        private bool EsCuentaBaneada(CuentaSet cuentaComparar)
        {
            if (_baneados != null)
            {
                foreach (CuentaSet cuenta in _baneados)
                {
                    if (cuenta.CorreoElectronico.Equals(cuentaComparar.CorreoElectronico))
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


        private void ClickRegresarMenu(object sender, RoutedEventArgs e)
        {
            Logger log = new Logger(this.GetType());
            try
            {
                _proxyNotificar.EliminarProxy(_cuentaUsuario.CorreoElectronico);
            }
            catch(CommunicationException ex)
            {
                Console.WriteLine(ex);
                log.LogError("Error de Comunicación con el Servidor", ex);
            }
            catch (TimeoutException ex)
            {
                Console.WriteLine(ex);
                log.LogWarn("Se agoto el tiempo de espera del servidor", ex);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                log.LogFatal("Ha ocurrido un error inesperado", ex);
            }
            this.NavigationService.GoBack();
        }

        private void PreviewMouseDownSolicitudAmistad(object sender, MouseButtonEventArgs e)
        {
            Button boton = sender as Button;
            CuentaSet cuenta = boton.CommandParameter as CuentaSet;
            Console.WriteLine(cuenta.Nickname);
            Console.WriteLine("Solicitud Amistad");

        }

        private void PreviewMouseDownBanearJugador(object sender, MouseButtonEventArgs e)
        {
            Button boton = sender as Button;
            CuentaSet cuenta = boton.CommandParameter as CuentaSet;
            Console.WriteLine(cuenta.Nickname);
            Console.WriteLine("Banear Jug");
        }

        private void PreviewMouseDownEliminarBaneo(object sender, MouseButtonEventArgs e)
        {
            Button boton = sender as Button;
            CuentaSet cuenta = boton.CommandParameter as CuentaSet;
            Console.WriteLine(cuenta.Nickname);
            Console.WriteLine("Eliminar Banear");
        }


        private void PreviewMouseDownEliminarAmigo(object sender, MouseButtonEventArgs e)
        {
            Button boton = sender as Button;
            CuentaSet cuentaAmigo = boton.CommandParameter as CuentaSet;
            EliminarAmistad(cuentaAmigo);
        }

        private void PreviewMouseDownAceptarNotificacion(object sender, MouseButtonEventArgs e)
        {
            Console.WriteLine("Aceptar");
            Button btnEliminarNotificacion = sender as Button;
            NotificacionSet notificacion = btnEliminarNotificacion.CommandParameter as NotificacionSet;
            if (CrearAmistad(notificacion)) 
            {
                EliminarNotificacion(notificacion);
            }
        }

        private void PreviewMouseDownRechazarNotificacion(object sender, MouseButtonEventArgs e)
        {
            Console.WriteLine("Rechazar");
            Button btnEliminarNotificacion = sender as Button;
            NotificacionSet notificacion = btnEliminarNotificacion.CommandParameter as NotificacionSet;
            EliminarNotificacion(notificacion);
        }

        private void PreviewMouseDownEliminarNotificacionPartida(object sender, MouseButtonEventArgs e)
        {
            Console.WriteLine("Eliminar Notificacion");
            Button btnEliminarNotificacion = sender as Button;
            NotificacionSet notificacion = btnEliminarNotificacion.CommandParameter as NotificacionSet;
            EliminarNotificacion(notificacion);
        }

        private bool CrearAmistad(NotificacionSet notificacion) 
        {
            Logger log = new Logger(this.GetType());
            Constantes resultado;
            try 
            {
                resultado = _proxyAmigos.GuardarAmistad(notificacion.CorreoElectronicoDestinatario, notificacion.CorreoElectronicoRemitente);
            } 
            catch (CommunicationException ex) 
            {
                Console.WriteLine(ex);
                resultado = Constantes.ERROR_CONEXION_SERVIDOR;
                log.LogError("Error de Comunicación con el Servidor", ex);
            } 
            catch (TimeoutException ex) 
            {
                Console.WriteLine(ex);
                resultado = Constantes.ERROR_CONEXION_SERVIDOR;
                log.LogWarn("Se agoto el tiempo de espera del servidor", ex);
            } 
            catch (Exception ex) 
            {
                Console.WriteLine(ex);
                resultado = Constantes.ERROR_CONEXION_SERVIDOR;
                log.LogFatal("Ha ocurrido un error inesperado", ex);
            }
            switch (resultado)
            {
                case Constantes.ERROR_CONEXION_BD:
                    MessageBox.Show(Properties.Resources.msgErrorBaseDatos);
                    return false;
                case Constantes.ERROR_CONSULTA:
                    MessageBox.Show(Properties.Resources.msgErrorBaseDatos);
                    return false;
                case Constantes.ERROR_CONEXION_SERVIDOR:
                    MessageBox.Show(Properties.Resources.msgErrorConexion);
                    return false;
                case Constantes.OPERACION_EXITOSA:
                    return true;
                case Constantes.OPERACION_EXITOSA_VACIA:
                    MessageBox.Show(Properties.Resources.msgErrorConexion);
                    return false;
                default:
                    return false;        
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
                Console.WriteLine(ex);
                resultado = Constantes.ERROR_CONEXION_SERVIDOR;
                log.LogError("Error de Comunicación con el Servidor", ex);
            }
            catch (TimeoutException ex)
            {
                Console.WriteLine(ex);
                resultado = Constantes.ERROR_CONEXION_SERVIDOR;
                log.LogWarn("Se agoto el tiempo de espera del servidor", ex);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                resultado = Constantes.ERROR_CONEXION_SERVIDOR;
                log.LogFatal("Ha ocurrido un error inesperado", ex);
            }
            switch (resultado)
            {
                case Constantes.OPERACION_EXITOSA:
                    CargarNotificaciones();
                    break;
                case Constantes.OPERACION_EXITOSA_VACIA:
                    break;
                case Constantes.ERROR_CONEXION_BD:
                    MessageBox.Show(Properties.Resources.msgErrorBaseDatos);
                    break;
                case Constantes.ERROR_CONSULTA:
                    MessageBox.Show(Properties.Resources.msgErrorBaseDatos);
                    break;
                case Constantes.ERROR_CONEXION_SERVIDOR:
                    MessageBox.Show(Properties.Resources.msgErrorConexion);
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
                Console.WriteLine(ex);
                resultado = Constantes.ERROR_CONEXION_SERVIDOR;
                log.LogError("Error de Comunicación con el Servidor", ex);
            }
            catch (TimeoutException ex)
            {
                Console.WriteLine(ex);
                resultado = Constantes.ERROR_CONEXION_SERVIDOR;
                log.LogWarn("Se agoto el tiempo de espera del servidor", ex);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                resultado = Constantes.ERROR_CONEXION_SERVIDOR;
                log.LogFatal("Ha ocurrido un error inesperado", ex);
            }
            switch (resultado)
            {
                case Constantes.OPERACION_EXITOSA:
                    NotificarUsuario(cuentaAmigo);
                    CargarAmigos();
                    break;
                case Constantes.OPERACION_EXITOSA_VACIA:
                    break;
                case Constantes.ERROR_CONEXION_BD:
                    MessageBox.Show(Properties.Resources.msgErrorBaseDatos);
                    break;
                case Constantes.ERROR_CONSULTA:
                    MessageBox.Show(Properties.Resources.msgErrorBaseDatos);
                    break;
                case Constantes.ERROR_CONEXION_SERVIDOR:
                    MessageBox.Show(Properties.Resources.msgErrorConexion);
                    break;
            }
        }

        private void NotificarUsuario(CuentaSet cuentaJugador)
        {
            Logger log = new Logger(this.GetType());
            try
            {
                _proxyAmigos.NotificarUsuario(cuentaJugador.CorreoElectronico);
            }
            catch (CommunicationException ex)
            {
                Console.WriteLine(ex);
                log.LogError("Error de Comunicación con el Servidor", ex);
            }
            catch (TimeoutException ex)
            {
                Console.WriteLine(ex);
                log.LogWarn("Se agoto el tiempo de espera del servidor", ex);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                log.LogFatal("Ha ocurrido un error inesperado", ex);
            }
        }

        public void RecuperarNotificacion()
        {
            CargarAmigos();
            CargarNotificaciones();
            MostrarJugadores();
        }
    }
}
