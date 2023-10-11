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
        public RegistroUsuariosPagina()
        {
            InitializeComponent();
            EstablecerImagenPorDefecto();
        }
        private String rutaImagen;
        private byte[] avatarByte = null;
        private void EstablecerImagenPorDefecto() 
        {
            rutaImagen = "pack://application:,,,/Recursos/avatarPredefinido.png";
            try
            {
                imgBrushAvatar.ImageSource = new BitmapImage(new Uri(rutaImagen));
                BitmapImage bitmap = new BitmapImage(new Uri(rutaImagen));
                JpegBitmapEncoder encoder = new JpegBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bitmap));
                using (MemoryStream ms = new MemoryStream())
                {
                    encoder.Save(ms);
                    avatarByte = ms.ToArray();
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(Properties.Resources.msgErrorAvatarDefault,Properties.Resources.msgTituloErrorAvatarDefault, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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

            if (!string.IsNullOrWhiteSpace(txtBoxCorreoElectronico.Text) && ValidarCorreo(txtBoxCorreoElectronico.Text) && ValidarExistenciaCorreo())
            {
                txtBoxCorreoElectronico.Style = estiloTxtBoxAzul;
            }
            else
            {
                txtBoxCorreoElectronico.Style = estiloTxtBoxRojo;
                validacionCampos = false;
            }

            if (!string.IsNullOrWhiteSpace(pssBoxContrasena.Password) && ValidarContraseña(pssBoxContrasena.Password))
            {
                pssBoxContrasena.Style = estiloPssBoxAzul;
            }
            else
            {
                pssBoxContrasena.Style = estiloPssBoxRojo;
                validacionCampos = false;
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
            catch (RegexMatchTimeoutException e)
            {
                return false;
            }
            catch (ArgumentException e)
            {
                return false;
            }
        }
        private bool ValidarContraseña(string contraseña)
        {
            string patron = @"^(?=.*[0-9!@#$%^&*()\-=_+.,:;])[A-Za-z0-9!@#$%^&*()\-=_+.,:;]{8,}$";
            Regex regex = new Regex(patron);
            bool correoValidado = regex.IsMatch(contraseña);
            if (correoValidado)
            {
                return correoValidado;
            }
            System.Windows.Forms.MessageBox.Show(Properties.Resources.msgErrorContrasenaInvalida, Properties.Resources.msgTituloContraseñaInvalida, MessageBoxButtons.OK, MessageBoxIcon.Error);
            return correoValidado;  
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
                Avatar = avatarByte, 
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
            catch (CommunicationException excepcion)
            {
                resultado = Constantes.ERROR_CONEXION_SERVIDOR;
                System.Windows.Forms.MessageBox.Show(Properties.Resources.msgErrorConexion);
            }
            catch (TimeoutException excepcion)
            {
                resultado = Constantes.ERROR_CONEXION_SERVIDOR;
                System.Windows.Forms.MessageBox.Show(Properties.Resources.msgErrorConexion);

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
            catch (CommunicationException excepcion)
            {
                resultado = Constantes.ERROR_CONEXION_SERVIDOR;
                Console.WriteLine(excepcion);
            }

            switch (resultado)
            {
                case Constantes.OPERACION_EXITOSA:
                    System.Windows.Forms.MessageBox.Show(Properties.Resources.msgCorreoEncontrado, Properties.Resources.msgTituloCorreoEncontrado, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                case Constantes.OPERACION_EXITOSA_VACIA:
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
        private void ClicCrearCuenta(object sender, RoutedEventArgs e)
        {
            if (ValidarCampos() && ValidarExistenciaImagen())
            {
                try
                {
                    AñadirCuenta();
                    IrInicioSesion();
                }
                catch (CommunicationException ex)
                {
                    System.Windows.Forms.MessageBox.Show(Properties.Resources.msgErrorConexion);
                }
            }

        }
        private void IrInicioSesion() 
        {
            this.NavigationService.GoBack();
        }
        private void ClicCancelar(object sender, RoutedEventArgs e)
        {
            IrInicioSesion();
        }

        private void SeleccionarImagen(object sender, MouseButtonEventArgs e)
        {
            OpenFileDialog abrirBiblioteca = new OpenFileDialog();
            abrirBiblioteca.Filter = "Archivos de imagen|*.jpg;*.jpeg";
            if (abrirBiblioteca.ShowDialog() == DialogResult.OK)
            {
                rutaImagen = abrirBiblioteca.FileName;
                string[] formatosSoportados = { ".jpg", ".jpeg"};
                if (formatosSoportados.Any(ext => rutaImagen.EndsWith(ext, StringComparison.OrdinalIgnoreCase)))
                {
                    BitmapImage mapaBits = new BitmapImage(new Uri(rutaImagen));
                    imgBrushAvatar.ImageSource = mapaBits;
                    try
                    {
                        avatarByte = File.ReadAllBytes(rutaImagen);
                    }
                    catch (Exception ex)
                    {
                        System.Windows.Forms.MessageBox.Show(Properties.Resources.msgErrorImagen, Properties.Resources.msgTituloErrorImagen, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show(Properties.Resources.msgImagenInvalida, Properties.Resources.msgTituloErrorImagen, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void ClicRemoverImagen(object sender, RoutedEventArgs e)
        {
            EstablecerImagenPorDefecto();
        }

        private void txtBoxNombre_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtBoxNombre.Text.Length > 60)
            {
                txtBoxNombre.Text = txtBoxNombre.Text.Substring(0, 60);
                txtBoxNombre.SelectionStart = txtBoxNombre.Text.Length;
            }
        }

        private void txtBoxNickname_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtBoxNickname.Text.Length > 50)
            {
                txtBoxNickname.Text = txtBoxNickname.Text.Substring(0, 50);
                txtBoxNickname.SelectionStart = txtBoxNickname.Text.Length;
            }
        }

        private void txtBoxApellidos_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtBoxApellidos.Text.Length > 30)
            {
                txtBoxApellidos.Text = txtBoxApellidos.Text.Substring(0, 30);
                txtBoxApellidos.SelectionStart = txtBoxApellidos.Text.Length;
            }
        }

        private void txtBoxCorreoElectronico_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtBoxCorreoElectronico.Text.Length > 100)
            {
                txtBoxCorreoElectronico.Text = txtBoxCorreoElectronico.Text.Substring(0, 100);
                txtBoxCorreoElectronico.SelectionStart = txtBoxCorreoElectronico.Text.Length;
            }
        }
    }
}
