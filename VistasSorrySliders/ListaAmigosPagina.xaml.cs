using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
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
using System.Net.NetworkInformation;
using VistasSorrySliders.LogicaJuego;

namespace VistasSorrySliders
{
    /// <summary>
    /// Lógica de interacción para ListaAmigosPagina.xaml
    /// </summary>
    public partial class ListaAmigosPagina : Page
    {
        private CuentaSet _cuenta;
        private string _codigoPartida;
        private ListaAmigosClient _proxyAmigos;
        private List<TipoNotificacion> _tiposNotificacion;

        public ListaAmigosPagina(CuentaSet cuenta, string codigoPartida)
        {
            InitializeComponent();
            _cuenta = cuenta;
            _codigoPartida = codigoPartida;
            
        }

        public bool InicializarPagina()
        {
            return RecuperarAmigos() && RecuperarTiposNotificacion();
        }

        private bool RecuperarAmigos()
        {
            Constantes resultado;
            Logger log = new Logger(this.GetType());
            List<CuentaSet> amigosLista = new List<CuentaSet>();
            try
            {
                CuentaSet[] cuentas;
                _proxyAmigos = new ListaAmigosClient();
                (resultado, cuentas) = _proxyAmigos.RecuperarAmigosCuenta(_cuenta.CorreoElectronico);
                if (cuentas != null)
                {
                    amigosLista = cuentas.ToList();
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

            switch (resultado)
            {
                case Constantes.OPERACION_EXITOSA:
                    lstBoxAmigos.Style = (Style)FindResource("estiloLstBoxAmigos");
                    lstBoxAmigos.ItemsSource = amigosLista;
                    return true;
                case Constantes.OPERACION_EXITOSA_VACIA:
                    lstBoxAmigos.Style = (Style)FindResource("estiloLstBoxAmigosVacia");
                    return true;
                default:
                    Utilidades.MostrarMensajesError(resultado); 
                    return false;
            }
        }
        private void ClickEnviarCodigo(object sender, RoutedEventArgs e)
        {
            CuentaSet cuentaJugadorClickeado = RecuperarCuentaListItem((ListBoxItem)sender);
            if (_tiposNotificacion != null)
            {
                EnviarInvitacionJugador(cuentaJugadorClickeado);                
            }
        }

        private CuentaSet RecuperarCuentaListItem(ListBoxItem itemJugador)
        {
            var bordeEstilo = VisualTreeHelper.GetChild(itemJugador, 0);
            var gridEstilo = VisualTreeHelper.GetChild(bordeEstilo, 0);
            var botonDelItem = VisualTreeHelper.GetChild(gridEstilo, 3);

            Button botonJugador = (Button)botonDelItem;

            return (CuentaSet)botonJugador.CommandParameter;
        }
        //Cambiar Enviar Correo a Servidor
        private void EnviarCorreo()
        {
            Logger log = new Logger(this.GetType());
            try
            {
                if (NetworkInterface.GetIsNetworkAvailable())
                {

                    MailMessage correo = new MailMessage();
                    string correoJuego = "TheSorrySliders@gmail.com";
                    string contraseñaAplicacion = "nsnd wsuu kqeb qayk";
                    correo.From = new MailAddress(correoJuego);
                    correo.To.Add("montielsalasjesus@gmail.com");
                    correo.Subject = "Invitación a Sorry Sliders";

                    correo.Body = $"<p>El jugador {_cuenta.CorreoElectronico} te ha invitado a jugar Sorry Sliders \n " +
                        $"Únete como invitado con el siguiente código de partida: {_codigoPartida} \n " +
                        $"</p><img src='VistasSorrySliders/Recursos/logoSliders.png' alt='Sorry Sliders'>";
                    correo.IsBodyHtml = true;

                    SmtpClient clienteSmtp = new SmtpClient("smtp.gmail.com");
                    clienteSmtp.Port = 587;
                    clienteSmtp.Credentials = new NetworkCredential(correoJuego, contraseñaAplicacion);
                    clienteSmtp.EnableSsl = true;
                    clienteSmtp.Send(correo);
                }
                else
                {
                    Console.WriteLine("No hay conexión a Internet");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al enviar el correo: " + ex.StackTrace);
                log.LogFatal("Ha ocurrido un error inesperado", ex);
            }
        }

        private void ClickBuscarJugador(object sender, RoutedEventArgs e)
        {
            lstBoxJugadores.Style = (Style)FindResource("estiloLstBoxAmigos");
            lstBoxJugadores.Items.Clear();
            lstBoxJugadores.ItemsSource = null;
            string informacionJugador = txtBoxBuscadorJugadores.Text;

            if (!string.IsNullOrWhiteSpace(informacionJugador))
            {
                CargarJugadores(informacionJugador);
            }
        }
        private void CargarJugadores(string informacionJugador)
        {
            Constantes resultado;
            List<CuentaSet> jugadoresLista = new List<CuentaSet>();
            List<CuentaSet> jugadoresBaneados = new List<CuentaSet>();
            Logger log = new Logger(this.GetType());
            try
            {
                CuentaSet[] cuentasBusqueda;
                (resultado, cuentasBusqueda) = _proxyAmigos.RecuperarJugadoresCuenta(informacionJugador, _cuenta.CorreoElectronico);
                
                if (resultado == Constantes.OPERACION_EXITOSA)
                {
                    jugadoresLista = cuentasBusqueda.ToList();
                    CuentaSet[] cuentasBaneadas;
                    (resultado, cuentasBaneadas) = _proxyAmigos.RecuperarBaneados(_cuenta.CorreoElectronico);
                    if (cuentasBaneadas != null)
                    {
                        jugadoresBaneados = cuentasBaneadas.ToList();
                    }
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

            switch (resultado)
            {
                case Constantes.OPERACION_EXITOSA:
                    MostrarJugadoresBuscados(jugadoresLista, jugadoresBaneados);
                    break;
                case Constantes.OPERACION_EXITOSA_VACIA:
                    lstBoxJugadores.Style = (Style)FindResource("estiloLstBoxJugadoresVacia");
                    break;
                default:
                    Window.GetWindow(this).Close();
                    Utilidades.MostrarMensajesError(resultado);
                    break;
            }
        }

        private void MostrarJugadoresBuscados(List<CuentaSet> jugadoresLista, List<CuentaSet> jugadoresBaneados)
        {
            lstBoxJugadores.Style = (Style)FindResource("estiloLstBoxAmigos");
            foreach (CuentaSet cuenta in jugadoresLista)
            {
                ListBoxItem lstBoxItemCuenta = new ListBoxItem
                {
                    DataContext = cuenta
                };
                foreach (CuentaSet jugadorBaneado in jugadoresBaneados)
                {
                    if (jugadorBaneado.CorreoElectronico.Equals(cuenta.CorreoElectronico))
                    {
                        lstBoxItemCuenta.IsEnabled = false;
                    }
                }
                
                lstBoxJugadores.Items.Add(lstBoxItemCuenta);
            }
        }

        private void ClickEnviarCodigoJugadorSinCuenta(object sender, RoutedEventArgs e)
        {
            //EnviarCorreo();
        }

        private void EnviarInvitacionJugador(CuentaSet cuentaJugadorClickeado) 
        {
            Logger log = new Logger(this.GetType());
            Constantes resultado;
            try
            {
                NotificacionSet notificacionNueva = new NotificacionSet
                {
                    CorreoElectronicoRemitente = _cuenta.CorreoElectronico,
                    CorreoElectronicoDestinatario = cuentaJugadorClickeado.CorreoElectronico,
                    IdTipoNotificacion = _tiposNotificacion[0].IdTipoNotificacion,
                    Mensaje = _codigoPartida
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
            switch (resultado)
            {
                case Constantes.OPERACION_EXITOSA:
                    NotificarUsuarioInvitado(cuentaJugadorClickeado);
                    return;
                case Constantes.OPERACION_EXITOSA_VACIA:
                    Utilidades.MostrarUnMensajeError(Properties.Resources.msgNotificacionGuardarError);
                    break;
                default:
                    Utilidades.MostrarMensajesError(resultado);
                    break;
            }
            Window.GetWindow(this).Close();
        }

        private void NotificarUsuarioInvitado(CuentaSet cuentaJugadorClickeado)
        {
            Logger log = new Logger(this.GetType());
            try
            {
                _proxyAmigos.NotificarUsuario(cuentaJugadorClickeado.CorreoElectronico);
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
    }
}
