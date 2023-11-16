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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;
using System.Drawing;
using System.Net.NetworkInformation;

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
        private TipoNotificacion[] _tiposNotificacion;

        public ListaAmigosPagina(CuentaSet cuenta, string codigoPartida)
        {
            InitializeComponent();
            _cuenta = cuenta;
            _codigoPartida = codigoPartida;
            RecuperarAmigos();
            RecuperarTiposNotificacion();
        }


        private void RecuperarAmigos()
        {
            Constantes resultado;
            Logger log = new Logger(this.GetType());
            List<CuentaSet> amigosLista = new List<CuentaSet>();
            try
            {
                CuentaSet[] cuentas;
                InstanceContext contexto = new InstanceContext(this);
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
                Console.WriteLine(ex);
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
                    lstBoxAmigos.ItemsSource = amigosLista;
                    break;
                case Constantes.OPERACION_EXITOSA_VACIA:
                    break;
                case Constantes.ERROR_CONEXION_BD:
                case Constantes.ERROR_CONSULTA:
                    MessageBox.Show(Properties.Resources.msgErrorBaseDatos);
                    break;
                case Constantes.ERROR_CONEXION_SERVIDOR:
                    MessageBox.Show(Properties.Resources.msgErrorConexion);
                    break;
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

        private void ClicBuscarJugador(object sender, RoutedEventArgs e)
        {
            lstBoxJugadores.ItemsSource = null;
            string informacionJugador = txtBoxBuscadorJugadores.Text;

            if (!string.IsNullOrWhiteSpace(informacionJugador))
            {
                CargarJugadores(informacionJugador);
            }
        }


        private void CargarJugadores(string informacionJugador)
        {
            Console.WriteLine(informacionJugador);
            Constantes resultado;
            List<CuentaSet> jugadoresLista = new List<CuentaSet>();
            Logger log = new Logger(this.GetType());
            try
            {
                CuentaSet[] cuentas;
                (resultado, cuentas) = _proxyAmigos.RecuperarJugadoresCuenta(informacionJugador);
                if (cuentas != null)
                {
                    jugadoresLista = cuentas.ToList();
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
                    lstBoxJugadores.ItemsSource = jugadoresLista;
                    break;
                case Constantes.OPERACION_EXITOSA_VACIA:
                    break;
                case Constantes.ERROR_CONEXION_BD:
                case Constantes.ERROR_CONSULTA:
                    MessageBox.Show(Properties.Resources.msgErrorBaseDatos);
                    break;
                case Constantes.ERROR_CONEXION_SERVIDOR:
                    MessageBox.Show(Properties.Resources.msgErrorConexion);
                    break;
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
                _proxyAmigos.GuardarNotificacion(notificacionNueva);
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

        private void RecuperarTiposNotificacion() 
        {
            Logger log = new Logger(this.GetType());
            Constantes resultado = new Constantes();
            try
            {
                InstanceContext contexto = new InstanceContext(this);
                (resultado, _tiposNotificacion) = _proxyAmigos.RecuperarTipoNotificacion();
                
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

    }
}
