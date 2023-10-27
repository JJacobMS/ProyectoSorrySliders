using System;
using System.Collections.Generic;
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
        public RegistroUsuariosPagina()
        {
            InitializeComponent();
            InicializarDatosCrearUsuario();
            EstablecerImagenPorDefecto();
            
            _esModificacion = false;
        }
        public RegistroUsuariosPagina(CuentaSet cuentaUsuario,  UsuarioSet usuario)
        {
            InitializeComponent();
            _esModificacion = true;
            _cuenta = cuentaUsuario;
            _usuario = usuario;
            InicializarDatosModificarUsuario();
        }
        private String _rutaImagen;
        private byte[] _avatarByte = null;
        private void EstablecerImagenPorDefecto() 
        {
            _rutaImagen = "pack://application:,,,/Recursos/avatarPredefinido.jpg";
            try
            {
                imgBrushAvatar.ImageSource = new BitmapImage(new Uri(_rutaImagen));
                BitmapImage bitmap = new BitmapImage(new Uri(_rutaImagen));
                JpegBitmapEncoder encoder = new JpegBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bitmap));
                using (MemoryStream ms = new MemoryStream())
                {
                    encoder.Save(ms);
                    _avatarByte = ms.ToArray();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                System.Windows.Forms.MessageBox.Show(Properties.Resources.msgErrorAvatarDefault,Properties.Resources.msgTituloErrorAvatarDefault, MessageBoxButtons.OK, MessageBoxIcon.Error);
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

            if (_esModificacion == false)
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


            if(!string.IsNullOrWhiteSpace(txtBoxNickname.Text))
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
            if (string.IsNullOrWhiteSpace(correo))
            {
                return  false;
            }
            try
            {
                correo = Regex.Replace(correo, @"(@)(.+)$", DomainMapper,RegexOptions.None, TimeSpan.FromMilliseconds(200));
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
                Console.WriteLine(ex);
                return false;
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }
        
        private void AñadirCuenta() 
        {
            String nombre = txtBoxNombre.Text;
            String apellidos = txtBoxApellidos.Text;
            String correoElectronico = txtBoxCorreoElectronico.Text;
            String contraseña = pssBoxContrasena.Password;

            String nickname = txtBoxNickname.Text;
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
            Constantes resultado = Constantes.OPERACION_EXITOSA;
            try
            {
                RegistroUsuarioClient proxyRegistrarUsuario = new RegistroUsuarioClient();
                resultado = proxyRegistrarUsuario.AgregarUsuario(usuarioNuevo,nuevaCuenta);
                proxyRegistrarUsuario.Close();
            }
            catch (CommunicationException ex)
            {
                Console.WriteLine(ex);
                resultado = Constantes.ERROR_CONEXION_SERVIDOR;
                System.Windows.Forms.MessageBox.Show(Properties.Resources.msgErrorConexion);
            }
            catch (TimeoutException ex)
            {
                Console.WriteLine(ex);
                resultado = Constantes.ERROR_CONEXION_SERVIDOR;
                System.Windows.Forms.MessageBox.Show(Properties.Resources.msgErrorConexion);

            }
            switch (resultado)
            {
                case Constantes.OPERACION_EXITOSA:
                    System.Windows.Forms.MessageBox.Show(Properties.Resources.msgCuentaCreada, Properties.Resources.msgTituloCuentaCreada, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    break;
                case Constantes.OPERACION_EXITOSA_VACIA:
                    break;
                case Constantes.ERROR_CONEXION_BD:
                    System.Windows.Forms.MessageBox.Show(Properties.Resources.msgErrorBaseDatos);
                    break;
                case Constantes.ERROR_CONSULTA:
                    System.Windows.Forms.MessageBox.Show(Properties.Resources.msgErrorBaseDatos);
                    break;
                case Constantes.ERROR_CONEXION_SERVIDOR:
                    System.Windows.Forms.MessageBox.Show(Properties.Resources.msgErrorConexion);
                    break;
            }
            

        }

        private void ActualizarCuenta()
        {
            String nombre = txtBoxNombre.Text;
            String apellidos = txtBoxApellidos.Text;
            String nickname = txtBoxNickname.Text;
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
            Constantes resultado = Constantes.OPERACION_EXITOSA;
            try
            {
                RegistroUsuarioClient proxyRegistrarUsuario = new RegistroUsuarioClient();
                resultado = proxyRegistrarUsuario.ActualizarUsuario(usuarioActualizado, cuentaActualizada);
                proxyRegistrarUsuario.Close();
            }
            catch (CommunicationException ex)
            {
                Console.WriteLine(ex);
                resultado = Constantes.ERROR_CONEXION_SERVIDOR;
                System.Windows.Forms.MessageBox.Show(Properties.Resources.msgErrorConexion);
            }
            catch (TimeoutException ex)
            {
                Console.WriteLine(ex);
                resultado = Constantes.ERROR_CONEXION_SERVIDOR;
                System.Windows.Forms.MessageBox.Show(Properties.Resources.msgErrorConexion);

            }
            switch (resultado)
            {
                case Constantes.OPERACION_EXITOSA:
                    break;
                case Constantes.OPERACION_EXITOSA_VACIA:
                    System.Windows.Forms.MessageBox.Show(Properties.Resources.msgErrorBaseDatos);
                    break;
                case Constantes.ERROR_CONEXION_BD:
                    System.Windows.Forms.MessageBox.Show(Properties.Resources.msgErrorBaseDatos);
                    break;
                case Constantes.ERROR_CONSULTA:
                    System.Windows.Forms.MessageBox.Show(Properties.Resources.msgErrorBaseDatos);
                    break;
                case Constantes.ERROR_CONEXION_SERVIDOR:
                    System.Windows.Forms.MessageBox.Show(Properties.Resources.msgErrorConexion);
                    break;
            }


        }


        private bool ValidarExistenciaImagen() 
        {
            bool archivoExiste = false;
            if (imgBrushAvatar.ImageSource is BitmapImage bitmapImage)
            {
                Uri uri = bitmapImage.UriSource;
                if (uri.IsAbsoluteUri)
                {
                    if (uri.IsFile)
                    {
                        string rutaArchivo = uri.LocalPath;
                        archivoExiste = File.Exists(rutaArchivo);
                    }
                    else
                    {
                        archivoExiste = true;
                    }
                }
            }
            if (archivoExiste)
            {
                return archivoExiste;
            }
            else 
            {
                System.Windows.Forms.MessageBox.Show(Properties.Resources.msgErrorImagen, Properties.Resources.msgTituloErrorImagen, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return archivoExiste;
            }
        }

        private bool ValidarExistenciaCorreo() 
        {
            string correoIngresado = txtBoxCorreoElectronico.Text;
            Constantes resultado = Constantes.OPERACION_EXITOSA_VACIA;
            try
            {
                InicioSesionClient proxyInicioSesion = new InicioSesionClient();
                resultado = proxyInicioSesion.VerificarExistenciaCorreoCuenta(correoIngresado);
                proxyInicioSesion.Close();
            }
            catch (CommunicationException ex)
            {
                resultado = Constantes.ERROR_CONEXION_SERVIDOR;
                Console.WriteLine(ex);
            }

            switch (resultado)
            {
                case Constantes.OPERACION_EXITOSA:
                    System.Windows.Forms.MessageBox.Show(Properties.Resources.msgCorreoEncontrado, Properties.Resources.msgTituloCorreoEncontrado, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                case Constantes.OPERACION_EXITOSA_VACIA:
                    System.Windows.Forms.MessageBox.Show(Properties.Resources.msgErrorBaseDatos);
                    return true;
                case Constantes.ERROR_CONEXION_BD:
                    return false;
                case Constantes.ERROR_CONSULTA:
                    System.Windows.Forms.MessageBox.Show(Properties.Resources.msgErrorBaseDatos);
                    return false;
                case Constantes.ERROR_CONEXION_SERVIDOR:
                    System.Windows.Forms.MessageBox.Show(Properties.Resources.msgErrorConexion);
                    return false;
            }
            return false;
        }
        private void ClickCrearCuenta(object sender, RoutedEventArgs e)
        {
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
                    if (ValidarCampos() && ValidarExistenciaImagen())
                    {
                        AñadirCuenta();
                        IrInicioSesion();
                    }
                }
            }
            catch (CommunicationException ex)
            {
                Console.WriteLine(ex);
                System.Windows.Forms.MessageBox.Show(Properties.Resources.msgErrorConexion);
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
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine(ex);
                Console.WriteLine(Properties.Resources.msgErrorImagenPermisos, Properties.Resources.msgErrorTituloImagenPermisos);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                System.Windows.Forms.MessageBox.Show(Properties.Resources.msgErrorImagen, Properties.Resources.msgTituloErrorImagen, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ClickRemoverImagen(object sender, RoutedEventArgs e)
        {
            EstablecerImagenPorDefecto();
        }

        private void TextChangedTamañoNombre(object sender, TextChangedEventArgs e)
        {
            if (txtBoxNombre.Text.Length > 60)
            {
                txtBoxNombre.Text = txtBoxNombre.Text.Substring(0, 60);
                txtBoxNombre.SelectionStart = txtBoxNombre.Text.Length;
            }
        }

        private void TextChangedTamañoNickname(object sender, TextChangedEventArgs e)
        {
            if (txtBoxNickname.Text.Length > 30)
            {
                txtBoxNickname.Text = txtBoxNickname.Text.Substring(0, 30);
                txtBoxNickname.SelectionStart = txtBoxNickname.Text.Length;
            }
        }

        private void TextChangedTamañoApellidos(object sender, TextChangedEventArgs e)
        {
            if (txtBoxApellidos.Text.Length > 30)
            {
                txtBoxApellidos.Text = txtBoxApellidos.Text.Substring(0, 30);
                txtBoxApellidos.SelectionStart = txtBoxApellidos.Text.Length;
            }
        }

        private void TextChangedTamañoCorreoElectronico(object sender, TextChangedEventArgs e)
        {
            if (txtBoxCorreoElectronico.Text.Length > 100)
            {
                txtBoxCorreoElectronico.Text = txtBoxCorreoElectronico.Text.Substring(0, 100);
                txtBoxCorreoElectronico.SelectionStart = txtBoxCorreoElectronico.Text.Length;
            }
        }

        static bool ValidarTamañoImagen(string ruta)
        {
            try
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
            catch (IOException ex)
            {
                Console.WriteLine(ex);
                System.Windows.Forms.MessageBox.Show(Properties.Resources.msgErrorImagen, Properties.Resources.msgTituloErrorImagen, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }
    }

}
