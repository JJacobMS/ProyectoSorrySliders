using BibliotecaClasesSorrySliders;
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
                System.Windows.Forms.MessageBox.Show("Error al leer la imagen", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



        private bool ValidarCampos()
        {
            bool validacionCampos = true;
            Style estiloTxtBoxRojo = (Style)System.Windows.Application.Current.FindResource("estiloTxtBoxRojo");
            Style estiloTxtBoxAzul = (Style)System.Windows.Application.Current.FindResource("estiloTxtBoxAzul");
            Style estiloPssBoxRojo = (Style)System.Windows.Application.Current.FindResource("estiloPssBoxRojo");
            Style estiloPssBoxAzul = (Style)System.Windows.Application.Current.FindResource("estiloPssBoxAzul");

            if (!string.IsNullOrEmpty(txtBoxNombre.Text))
            {
                txtBoxNombre.Style = estiloTxtBoxAzul;
            }
            else
            {
                txtBoxNombre.Style = estiloTxtBoxRojo;
                validacionCampos = false;
            }

            if (!string.IsNullOrEmpty(txtBoxApellidos.Text))
            {
                txtBoxApellidos.Style = estiloTxtBoxAzul;
            }
            else
            {
                txtBoxApellidos.Style = estiloTxtBoxRojo;
                validacionCampos = false;
            }

            if (!string.IsNullOrEmpty(txtBoxCorreoElectronico.Text) && ValidarCorreo(txtBoxCorreoElectronico.Text) && ValidarExistenciaCorreo())
            {
                txtBoxCorreoElectronico.Style = estiloTxtBoxAzul;
            }
            else
            {
                txtBoxCorreoElectronico.Style = estiloTxtBoxRojo;
                validacionCampos = false;
            }

            if (!string.IsNullOrEmpty(pssBoxContrasena.Password) && ValidarContraseña(pssBoxContrasena.Password))
            {
                pssBoxContrasena.Style = estiloPssBoxAzul;
            }
            else
            {
                pssBoxContrasena.Style = estiloPssBoxRojo;
                validacionCampos = false;
            }

            if(!string.IsNullOrEmpty(txtBoxNickname.Text))
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
            }
            catch (RegexMatchTimeoutException e)
            {
                return false;
            }
            catch (ArgumentException e)
            {
                return false;
            }
            try
            {
                return Regex.IsMatch(correo,@"^[^@\s]+@[^@\s]+\.[^@\s]+$",RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
            catch (ArgumentException)
            {
                return false;
            }
        }
        private bool ValidarContraseña(string contraseña)
        {
            string patron = @"^(?=.*[0-9!@#$%^&*()-=_+])[A-Za-z0-9!@#$%^&*()-=_+]{8,}$";
            Regex regex = new Regex(patron);
            if (!regex.IsMatch(contraseña))
            {
                System.Windows.Forms.MessageBox.Show("No cumple con los estandares de contraseñas, 8 caracteres con una letra o caracter especial", "Contraseña Invalida", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return true;//regex.IsMatch(contraseña);  
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
                Avatar = avatarByte, //Error por limite de tamaño de mensajes appcofig de servidor
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
                        System.Windows.Forms.MessageBox.Show("Cuenta registrada", "Cuenta registrada", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        break;
                    case Constantes.OPERACION_EXITOSA_VACIA:
                        break;
                    case Constantes.ERROR_CONEXION_BD:

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
                System.Windows.Forms.MessageBox.Show("Error de conexion añadirCuenta "+excepcion, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            return archivoExiste;
        }

        private bool ValidarExistenciaCorreo() 
        {
            bool correoExiste = false;
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
                    System.Windows.Forms.MessageBox.Show("Este correo ya ha sido registrado", "Cuenta registrada", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    
                    return correoExiste = false;
                case Constantes.OPERACION_EXITOSA_VACIA:
                    return true;
                case Constantes.ERROR_CONEXION_BD:
                    
                    return correoExiste = false;
                case Constantes.ERROR_CONSULTA:
                    System.Windows.Forms.MessageBox.Show(Properties.Resources.msgErrorBaseDatos);
                    return correoExiste = false;
                case Constantes.ERROR_CONEXION_SERVIDOR:
                    System.Windows.Forms.MessageBox.Show(Properties.Resources.msgErrorConexion);
                    return correoExiste = false;
            }
            return correoExiste = false;
        }
        private void ClicCrearCuenta(object sender, RoutedEventArgs e)
        {
            //validar y contraseña
            //Excepciones
            if (ValidarCampos() && ValidarExistenciaImagen())
            {
                try
                {
                    AñadirCuenta();
                    IrInicioSesion();
                }
                catch (CommunicationException ex)
                {
                    System.Windows.Forms.MessageBox.Show("No se puede establecer una conexión con el servidor SQL Server", "Error con la Base de Datos", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            abrirBiblioteca.Filter = "Archivos de imagen|*.jpg;*.jpeg;*.png";
            if (abrirBiblioteca.ShowDialog() == DialogResult.OK)
            {
                rutaImagen = abrirBiblioteca.FileName;
                string[] formatosSoportados = { ".jpg", ".jpeg", ".png" };
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
                        System.Windows.Forms.MessageBox.Show("Error al leer la imagen", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show("Por favor, seleccione un archivo de imagen válida.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void ClicRemoverImagen(object sender, RoutedEventArgs e)
        {
            EstablecerImagenPorDefecto();
        }
    }
}
