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
using System.Data.Entity.Core;
using System.Security.Authentication;
using System.Globalization;
using System.Text.RegularExpressions;

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

        public Constantes InicializarPagina()
        {
            try
            {
                RecuperarAmigos();
                RecuperarTiposNotificacion();
            }
            catch (CommunicationException ex)
            {
                Logger log = new Logger(this.GetType());
                log.LogWarn("Error de Comunicación con el Servidor", ex);
                return Constantes.ERROR_CONEXION_SERVIDOR;
            }
            catch (EntitySqlException ex)
            {
                Logger log = new Logger(this.GetType());
                log.LogError("Error Con Base de Datos", ex);
                return Constantes.ERROR_CONEXION_BD;
            }
            return Constantes.OPERACION_EXITOSA;
        }

        private void RecuperarAmigos()
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
            catch (CommunicationObjectFaultedException ex)
            {
                resultado = Constantes.ERROR_CONEXION_DEFECTUOSA;
                log.LogError("Se ha perdido la conexión previa", ex);
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
            Utilidades.MostrarMensajesError(resultado);
            switch (resultado)
            {
                case Constantes.OPERACION_EXITOSA:
                    lstBoxAmigos.Style = (Style)FindResource("estiloLstBoxAmigos");
                    lstBoxAmigos.ItemsSource = amigosLista;
                    return;
                case Constantes.OPERACION_EXITOSA_VACIA:
                    lstBoxAmigos.Style = (Style)FindResource("estiloLstBoxAmigosVacia");
                    return;
                case Constantes.ERROR_TIEMPO_ESPERA_SERVIDOR:
                case Constantes.ERROR_CONEXION_SERVIDOR:
                case Constantes.ERROR_CONEXION_DEFECTUOSA:
                    throw new CommunicationException();
            }
            throw new EntitySqlException();
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

        private void EnviarCorreo(string correoDestinatario)
        {
            Logger log = new Logger(this.GetType());
            string cuerpoCorreo = string.Format(Properties.Resources.msgCorreoInvitacion, _cuenta.CorreoElectronico, _codigoPartida);
            string asuntoCorreo = Properties.Resources.msgTituloCorreoInvitacion;
            Constantes resultado;
            try
            {
                resultado = _proxyAmigos.EnviarCorreo(correoDestinatario, asuntoCorreo, cuerpoCorreo);
            }
            catch (CommunicationObjectFaultedException ex)
            {
                resultado = Constantes.ERROR_CONEXION_DEFECTUOSA;
                log.LogError("Se ha perdido la conexión previa", ex);
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
                    Utilidades.MostrarMensajeInformacion(Properties.Resources.msgInvitacionExitosa);
                    break;
                case Constantes.OPERACION_EXITOSA_VACIA:
                case Constantes.ERROR_CONSULTA:
                    Utilidades.MostrarUnMensajeError(Properties.Resources.msgErroEnviarCorreo);
                    break;
                case Constantes.ERROR_CONEXION_SERVIDOR:
                case Constantes.ERROR_TIEMPO_ESPERA_SERVIDOR:
                case Constantes.ERROR_CONEXION_DEFECTUOSA:
                    Utilidades.MostrarMensajesError(resultado);
                    break;
            }
        }

        private bool ValidarCorreo(string correo)
        {
            Logger log = new Logger(this.GetType());
            if (string.IsNullOrWhiteSpace(correo))
            {
                return false;
            }
            try
            {
                correo = Regex.Replace(correo, @"(@)(.+)$", DomainMapper, RegexOptions.None, TimeSpan.FromMilliseconds(200));
                string DomainMapper(Match match)
                {
                    var idn = new IdnMapping();
                    string domainName = idn.GetAscii(match.Groups[2].Value);
                    return match.Groups[1].Value + domainName;
                }
                bool correoValido = Regex.IsMatch(correo, @"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
                if (correoValido)
                {
                    return true;
                }
                else
                {
                    Utilidades.MostrarUnMensajeError(Properties.Resources.msgCorreoInvalido, Properties.Resources.msgTituloCorreoInvalido);
                    return false;
                }
            }
            catch (RegexMatchTimeoutException ex)
            {
                log.LogError("El tiempo de espera para la expresión se ha agotado", ex);
                return false;
            }
            catch (ArgumentException ex)
            {
                log.LogError("Se ha proporcionado un argumento invalido", ex);
                return false;
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
            Constantes resultadoBaneados = Constantes.OPERACION_EXITOSA;
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
                    (resultadoBaneados, cuentasBaneadas) = _proxyAmigos.RecuperarBaneados(_cuenta.CorreoElectronico);
                    if (resultadoBaneados == Constantes.OPERACION_EXITOSA)
                    {
                        jugadoresBaneados = cuentasBaneadas.ToList();
                    }
                }

            }
            catch (CommunicationObjectFaultedException ex)
            {
                resultado = Constantes.ERROR_CONEXION_DEFECTUOSA;
                log.LogError("Se ha perdido la conexión previa", ex);
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
            Utilidades.MostrarMensajesError(resultado);
            Utilidades.MostrarMensajesError(resultadoBaneados);
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

                if (jugadoresBaneados.Exists(jugadorBaneado => jugadorBaneado.CorreoElectronico.Equals(cuenta.CorreoElectronico)))
                {
                    lstBoxItemCuenta.IsEnabled = false;
                }
                
                lstBoxJugadores.Items.Add(lstBoxItemCuenta);
            }
        }

        private void ClickEnviarCodigoJugadorSinCuenta(object sender, RoutedEventArgs e)
        {
            if (txtBoxCorreoInvitacion.Text.Length > 0 && ValidarCorreo(txtBoxCorreoInvitacion.Text))
            {
                EnviarCorreo(txtBoxCorreoInvitacion.Text);
                txtBoxCorreoInvitacion.Text = "";
            }
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
            catch (CommunicationObjectFaultedException ex)
            {
                resultado = Constantes.ERROR_CONEXION_DEFECTUOSA;
                log.LogError("Se ha perdido la conexión previa", ex);
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
            catch (CommunicationObjectFaultedException ex)
            {
                log.LogError("Se ha perdido la conexión previa", ex);
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
                    return;
                }
            }
            catch (CommunicationObjectFaultedException ex)
            {
                resultado = Constantes.ERROR_CONEXION_DEFECTUOSA;
                log.LogError("Se ha perdido la conexión previa", ex);
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
            Utilidades.MostrarMensajesError(resultado);
            switch (resultado)
            {
                case Constantes.OPERACION_EXITOSA_VACIA:
                    Utilidades.MostrarUnMensajeError(Properties.Resources.msgTiposNotificacionVacios);
                    break;
                case Constantes.ERROR_TIEMPO_ESPERA_SERVIDOR:
                case Constantes.ERROR_CONEXION_SERVIDOR:
                case Constantes.ERROR_CONEXION_DEFECTUOSA:
                    throw new CommunicationException();
            }
            throw new EntitySqlException();
        }

        private void TextChangedTamañoCorreoElectronicoInvitacion(object sender, TextChangedEventArgs e)
        {
            int tamañoMaximoCorreo = 100;
            if (txtBoxCorreoInvitacion.Text.Length > tamañoMaximoCorreo)
            {
                txtBoxCorreoInvitacion.Text = txtBoxCorreoInvitacion.Text.Substring(0, tamañoMaximoCorreo);
                txtBoxCorreoInvitacion.SelectionStart = txtBoxCorreoInvitacion.Text.Length;
            }
        }

        private void TextChangedTamanoBuscador(object sender, TextChangedEventArgs e)
        {
            int tamañoMaximoCorreo = 100;
            if (txtBoxBuscadorJugadores.Text.Length > tamañoMaximoCorreo)
            {
                txtBoxBuscadorJugadores.Text = txtBoxBuscadorJugadores.Text.Substring(0, tamañoMaximoCorreo);
                txtBoxBuscadorJugadores.SelectionStart = txtBoxBuscadorJugadores.Text.Length;
            }
        }
    }
}
