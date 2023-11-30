using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure.Interception;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.ServiceModel;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using VistasSorrySliders.ServicioSorrySliders;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace VistasSorrySliders
{
    /// <summary>
    /// Lógica de interacción para RegistroUsuariosPagina.xaml
    /// </summary>
    public partial class RegistroUsuariosPagina : Page
    {
        private CuentaSet _cuenta;
        private UsuarioSet _usuario;
        private bool _esModificacion;
        private string _rutaImagen;
        private byte[] _avatarByte = null;

        public RegistroUsuariosPagina()
        {
            InitializeComponent();
            InicializarDatosCrearUsuario();
            EstablecerImagenPorDefecto();
            _esModificacion = false;
        }
        public RegistroUsuariosPagina(CuentaSet cuentaUsuario, UsuarioSet usuario)
        {
            InitializeComponent();
            _esModificacion = true;
            _cuenta = cuentaUsuario;
            _usuario = usuario;
            InicializarDatosModificarUsuario();
        }
        
        private void EstablecerImagenPorDefecto()
        {
            Logger log = new Logger(this.GetType());
            _rutaImagen = Properties.Resources.uriAvatarPredefinido;
            try
            {
                imgBrushAvatar.ImageSource = new BitmapImage(new Uri(_rutaImagen));
                BitmapImage mapaBits = new BitmapImage(new Uri(_rutaImagen));
                JpegBitmapEncoder codificador = new JpegBitmapEncoder();
                codificador.Frames.Add(BitmapFrame.Create(mapaBits));
                using (MemoryStream flujoMemoria = new MemoryStream())
                {
                    codificador.Save(flujoMemoria);
                    _avatarByte = flujoMemoria.ToArray();
                    if (_avatarByte == null) 
                    {
                        IrInicioSesion();
                    }
                }
            }
            catch (ArgumentException ex)
            {
                System.Windows.Forms.MessageBox.Show(Properties.Resources.msgErrorImagen, Properties.Resources.msgTituloErrorImagen, MessageBoxButtons.OK, MessageBoxIcon.Information);
                log.LogWarn("Se ha proporcionado un argumento invalido", ex);
            }
            catch (OutOfMemoryException ex)
            {
                System.Windows.Forms.MessageBox.Show(Properties.Resources.msgErrorImagen, Properties.Resources.msgTituloErrorImagen, MessageBoxButtons.OK, MessageBoxIcon.Information);
                log.LogWarn("Se ha agotado la memoria", ex);
            }
            catch (System.IO.IOException ex)
            {
                System.Windows.Forms.MessageBox.Show(Properties.Resources.msgErrorImagen, Properties.Resources.msgTituloErrorImagen, MessageBoxButtons.OK, MessageBoxIcon.Information);
                log.LogWarn("Error al acceder a la imagen", ex);
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(Properties.Resources.msgErrorAvatarDefault, Properties.Resources.msgTituloErrorAvatarDefault, MessageBoxButtons.OK, MessageBoxIcon.Error);
                log.LogFatal("Ha ocurrido un error inesperado", ex);
            }
        }

        private void InicializarDatosCrearUsuario()
        {
            lblTitulo.Content = Properties.Resources.lblBienvenido;
            btnContinuar.Content = Properties.Resources.btnCrearCuenta;
            lblContraseña.Visibility = Visibility.Visible;
        }

        private void InicializarDatosModificarUsuario()
        {
            txtBoxCorreoElectronico.Text = _cuenta.CorreoElectronico;
            pssBoxContrasena.Password = _cuenta.Contraseña;
            txtBoxNickname.Text = _cuenta.Nickname;
            txtBoxApellidos.Text = _usuario.Apellido;
            txtBoxNombre.Text = _usuario.Nombre;

            txtBoxCorreoElectronico.IsReadOnly = true;
            txtBoxCorreoElectronico.BorderBrush = Brushes.LightGray;
            pssBoxContrasena.Visibility = Visibility.Hidden;
            lblContraseña.Visibility = Visibility.Hidden;

            lblTitulo.Content = Properties.Resources.lblBienvenidoNuevo;
            btnContinuar.Content = Properties.Resources.btnGuardarCambios;
            _avatarByte = _cuenta.Avatar;
            Utilidades.IngresarImagen(_cuenta.Avatar, imgBrushAvatar);
        }

        private bool ValidarCampos()
        {
            bool validacionCampos = true;
            Style estiloTxtBoxRojo = (Style)System.Windows.Application.Current.FindResource("estiloTxtBoxRojo");
            Style estiloTxtBoxAzul = (Style)System.Windows.Application.Current.FindResource("estiloTxtBoxAzul");
            Style estiloPssBoxRojo = (Style)System.Windows.Application.Current.FindResource("estiloPssBoxRojo");
            Style estiloPssBoxAzul = (Style)System.Windows.Application.Current.FindResource("estiloPssBoxAzul");

            if (!string.IsNullOrWhiteSpace(txtBoxNombre.Text))
            {
                txtBoxNombre.Style = estiloTxtBoxAzul;
            }
            else
            {
                txtBoxNombre.Style = estiloTxtBoxRojo;
                validacionCampos = false;
            }

            if (!string.IsNullOrWhiteSpace(txtBoxApellidos.Text))
            {
                txtBoxApellidos.Style = estiloTxtBoxAzul;
            }
            else
            {
                txtBoxApellidos.Style = estiloTxtBoxRojo;
                validacionCampos = false;
            }

            if (!_esModificacion)
            {
                if (!string.IsNullOrWhiteSpace(txtBoxCorreoElectronico.Text) && ValidarCorreo(txtBoxCorreoElectronico.Text) && ValidarExistenciaCorreo())
                {
                    txtBoxCorreoElectronico.Style = estiloTxtBoxAzul;
                }
                else
                {
                    txtBoxCorreoElectronico.Style = estiloTxtBoxRojo;
                    validacionCampos = false;
                }

                if (!string.IsNullOrWhiteSpace(pssBoxContrasena.Password) && Utilidades.ValidarContraseña(pssBoxContrasena.Password))
                {
                    pssBoxContrasena.Style = estiloPssBoxAzul;
                }
                else
                {
                    pssBoxContrasena.Style = estiloPssBoxRojo;
                    validacionCampos = false;
                }
            }


            if (!string.IsNullOrWhiteSpace(txtBoxNickname.Text))
            {
                txtBoxNickname.Style = estiloTxtBoxAzul;
            }
            else
            {
                txtBoxNickname.Style = estiloTxtBoxRojo;
                validacionCampos = false;
            }
            return validacionCampos;
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
                    System.Windows.Forms.MessageBox.Show(Properties.Resources.msgCorreoInvalido, Properties.Resources.msgTituloCorreoInvalido, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }
            catch (RegexMatchTimeoutException ex)
            {
                log.LogWarn("El tiempo de espera para la expresión se ha agotado", ex);
                return false;
            }
            catch (ArgumentException ex)
            {
                log.LogWarn("Se ha proporcionado un argumento invalido", ex);
                return false;
            }
            catch (Exception ex)
            {
                log.LogFatal("Ha ocurrido un error inesperado", ex);
                return false;
            }
        }

        private void AñadirCuenta()
        {
            string nombre = txtBoxNombre.Text;
            string apellidos = txtBoxApellidos.Text;
            string correoElectronico = txtBoxCorreoElectronico.Text;
            string contraseña = pssBoxContrasena.Password;

            string nickname = txtBoxNickname.Text;
            var nuevaCuenta = new CuentaSet
            {
                CorreoElectronico = correoElectronico,
                Avatar = _avatarByte, 
                Nickname = nickname,
                Contraseña = contraseña
            };
            var usuarioNuevo = new UsuarioSet
            {
                Nombre = nombre,
                Apellido = apellidos
            };
            Constantes resultado;
            Logger log = new Logger(this.GetType());
            try
            {
                RegistroUsuarioClient proxyRegistrarUsuario = new RegistroUsuarioClient();
                resultado = proxyRegistrarUsuario.AgregarUsuario(usuarioNuevo,nuevaCuenta);
                proxyRegistrarUsuario.Close();
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
                    System.Windows.Forms.MessageBox.Show(Properties.Resources.msgCuentaCreada, Properties.Resources.msgTituloCuentaCreada, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    break;
                case Constantes.OPERACION_EXITOSA_VACIA:
                    System.Windows.Forms.MessageBox.Show(Properties.Resources.msgGuardarCuentaError);
                    break;
                default:
                    Utilidades.MostrarMensajesError(resultado);
                    break;
            }
        }

        private void ActualizarCuenta()
        {
            string nombre = txtBoxNombre.Text;
            string apellidos = txtBoxApellidos.Text;
            string nickname = txtBoxNickname.Text;
            _cuenta.Nickname = nickname;
            _cuenta.Avatar = _avatarByte;
            var cuentaActualizada = new CuentaSet
            {
                CorreoElectronico=_cuenta.CorreoElectronico,
                Nickname = _cuenta.Nickname,
                Avatar = _cuenta.Avatar
            };

            var usuarioActualizado = new UsuarioSet
            {
                Nombre = nombre,
                Apellido = apellidos
            };
            Constantes resultado;
            Logger log = new Logger(this.GetType());
            try
            {
                RegistroUsuarioClient proxyRegistrarUsuario = new RegistroUsuarioClient();
                resultado = proxyRegistrarUsuario.ActualizarUsuario(usuarioActualizado, cuentaActualizada);
                proxyRegistrarUsuario.Close();
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
                    System.Windows.Forms.MessageBox.Show(Properties.Resources.msgActualizarCuentaExito);
                    break;
                case Constantes.OPERACION_EXITOSA_VACIA:
                    System.Windows.Forms.MessageBox.Show(Properties.Resources.msgActualizarCuentaError);
                    break;
                default:
                    Utilidades.MostrarMensajesError(resultado);
                    break;
            }
        }

        private bool ValidarExistenciaCorreo() 
        {
            string correoIngresado = txtBoxCorreoElectronico.Text;
            Constantes resultado;
            Logger log = new Logger(this.GetType());
            try
            {
                InicioSesionClient proxyInicioSesion = new InicioSesionClient();
                resultado = proxyInicioSesion.VerificarExistenciaCorreoCuenta(correoIngresado);
                proxyInicioSesion.Close();
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
                case Constantes.OPERACION_EXITOSA_VACIA:
                    return true;
                case Constantes.OPERACION_EXITOSA:
                    System.Windows.Forms.MessageBox.Show(Properties.Resources.msgCorreoEncontrado, Properties.Resources.msgTituloCorreoEncontrado, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                default:
                    Utilidades.MostrarMensajesError(resultado);
                    break;
            }
            return false;
        }
        private void ClickCrearCuenta(object sender, RoutedEventArgs e)
        {
            Logger log = new Logger(this.GetType());
            try
            {
                if (_esModificacion)
                {
                    if (ValidarCampos()) 
                    {
                        ActualizarCuenta();
                        MenuPrincipalPagina menu = new MenuPrincipalPagina(_cuenta);
                        this.NavigationService.Navigate(menu);
                    }
                }
                else
                {
                    if (ValidarCampos())
                    {
                        AñadirCuenta();
                        IrInicioSesion();
                    }
                }
            }
            catch (CommunicationException ex)
            {
                System.Windows.Forms.MessageBox.Show(Properties.Resources.msgErrorConexion);
                log.LogError("Error de Comunicación con el Servidor", ex);
            }
            catch (TimeoutException ex)
            {
                System.Windows.Forms.MessageBox.Show(Properties.Resources.msgErrorConexion);
                log.LogWarn("Se agoto el tiempo de espera del servidor", ex);
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(Properties.Resources.msgErrorConexion);
                log.LogFatal("Ha ocurrido un error inesperado", ex);
            }

        }
        private void IrInicioSesion() 
        {
            this.NavigationService.GoBack();
        }
        private void ClickCancelar(object sender, RoutedEventArgs e)
        {
            IrInicioSesion();
        }

        private void SeleccionarImagen(object sender, MouseButtonEventArgs e)
        {
            Logger log = new Logger(this.GetType());
            try
            {
                OpenFileDialog abrirBiblioteca = new OpenFileDialog();
                abrirBiblioteca.Filter = $"{Properties.Resources.strArchivosImagen}|*.jpg;*.jpeg";
                if (abrirBiblioteca.ShowDialog() == DialogResult.OK)
                {
                    _rutaImagen = abrirBiblioteca.FileName;
                    string[] formatosSoportados = { ".jpg", ".jpeg" };
                    if (formatosSoportados.Any(ext => _rutaImagen.EndsWith(ext, StringComparison.OrdinalIgnoreCase)))
                    {
                        BitmapImage mapaBits = new BitmapImage(new Uri(_rutaImagen));
                        if(ValidarTamañoImagen(_rutaImagen)) 
                        {
                            imgBrushAvatar.ImageSource = mapaBits;
                            _avatarByte = File.ReadAllBytes(_rutaImagen);
                        }   
                    }
                    else
                    {
                        System.Windows.Forms.MessageBox.Show(Properties.Resources.msgImagenInvalida, Properties.Resources.msgTituloErrorImagen, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (ArgumentException ex)
            {
                System.Windows.Forms.MessageBox.Show(Properties.Resources.msgErrorImagen, Properties.Resources.msgTituloErrorImagen, MessageBoxButtons.OK, MessageBoxIcon.Information);
                log.LogWarn("Se ha proporcionado un argumento invalido", ex);
            }
            catch (OutOfMemoryException ex)
            {
                System.Windows.Forms.MessageBox.Show(Properties.Resources.msgErrorImagen, Properties.Resources.msgTituloErrorImagen, MessageBoxButtons.OK, MessageBoxIcon.Information);
                log.LogWarn("Se ha agotado la memoria", ex);
            }
            catch (UnauthorizedAccessException ex)
            {
                log.LogWarn("Error al acceder a la imagen", ex);
                System.Windows.Forms.MessageBox.Show(Properties.Resources.msgErrorImagenPermisos, Properties.Resources.msgErrorTituloImagenPermisos);
            }
            catch (System.IO.IOException ex)
            {
                System.Windows.Forms.MessageBox.Show(Properties.Resources.msgErrorImagen, Properties.Resources.msgTituloErrorImagen, MessageBoxButtons.OK, MessageBoxIcon.Information);
                log.LogWarn("Error al acceder a la imagen", ex);
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(Properties.Resources.msgErrorImagen, Properties.Resources.msgTituloErrorImagen, MessageBoxButtons.OK, MessageBoxIcon.Error);
                log.LogFatal("Ha ocurrido un error inesperado", ex);
            }
        }

        private void ClickRemoverImagen(object sender, RoutedEventArgs e)
        {
            EstablecerImagenPorDefecto();
        }

        private void TextChangedTamañoNombre(object sender, TextChangedEventArgs e)
        {
            int tamañoMaximoNombre = 60;
            if (txtBoxNombre.Text.Length > tamañoMaximoNombre)
            {
                txtBoxNombre.Text = txtBoxNombre.Text.Substring(0, tamañoMaximoNombre);
                txtBoxNombre.SelectionStart = txtBoxNombre.Text.Length;
            }
        }

        private void TextChangedTamañoNickname(object sender, TextChangedEventArgs e)
        {
            int tamañoMaximoNickname = 30;
            if (txtBoxNickname.Text.Length > tamañoMaximoNickname)
            {
                txtBoxNickname.Text = txtBoxNickname.Text.Substring(0, tamañoMaximoNickname);
                txtBoxNickname.SelectionStart = txtBoxNickname.Text.Length;
            }
        }

        private void TextChangedTamañoApellidos(object sender, TextChangedEventArgs e)
        {
            int tamañoMaximoApellidos = 30;
            if (txtBoxApellidos.Text.Length > tamañoMaximoApellidos)
            {
                txtBoxApellidos.Text = txtBoxApellidos.Text.Substring(0, tamañoMaximoApellidos);
                txtBoxApellidos.SelectionStart = txtBoxApellidos.Text.Length;
            }
        }

        private void TextChangedTamañoCorreoElectronico(object sender, TextChangedEventArgs e)
        {
            int tamañoMaximoCorreoElectronico = 100;
            if (txtBoxCorreoElectronico.Text.Length > tamañoMaximoCorreoElectronico)
            {
                txtBoxCorreoElectronico.Text = txtBoxCorreoElectronico.Text.Substring(0, tamañoMaximoCorreoElectronico);
                txtBoxCorreoElectronico.SelectionStart = txtBoxCorreoElectronico.Text.Length;
            }
        }

        static bool ValidarTamañoImagen(string ruta)
        {
            Logger log = new Logger(typeof(Utilidades));
            try
            {
                if (ruta == null)
                {
                    System.Windows.Forms.MessageBox.Show(Properties.Resources.msgErrorImagen, Properties.Resources.msgTituloErrorImagen, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
                {
                    using (FileStream flujoArchivo = new FileStream(ruta, FileMode.Open, FileAccess.Read))
                    {
                        int tamañoenBytes = (int)flujoArchivo.Length;
                        int tamañoEnKB = tamañoenBytes / 1024;

                        if (tamañoEnKB <= 400)
                        {
                            return true;
                        }
                        else
                        {
                            System.Windows.Forms.MessageBox.Show(Properties.Resources.msgErrorTamañoImagen, Properties.Resources.msgErrorTituloImagenPermisos, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return false;
                        }
                    }
                }
            }
            catch (IOException ex)
            {
                log.LogWarn("Error al acceder a la imagen", ex);
                System.Windows.Forms.MessageBox.Show(Properties.Resources.msgErrorImagen, Properties.Resources.msgTituloErrorImagen, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            catch (UnauthorizedAccessException ex)
            {
                log.LogWarn("Error al acceder a la imagen", ex);
                System.Windows.Forms.MessageBox.Show(Properties.Resources.msgErrorImagen, Properties.Resources.msgTituloErrorImagen, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            catch (Exception ex)
            {
                log.LogFatal("Ha ocurrido un error inesperado", ex);
                System.Windows.Forms.MessageBox.Show(Properties.Resources.msgErrorImagen, Properties.Resources.msgTituloErrorImagen, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }
    }

}
