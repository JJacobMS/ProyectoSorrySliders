using BibliotecaClasesSorrySliders;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
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
        String rutaImagen;
        private void EstablecerImagenPorDefecto() 
        {
            rutaImagen = "pack://application:,,,/Recursos/avatarPredefinido.png";
            mgBrushAvatar.ImageSource = new BitmapImage(new Uri(rutaImagen));
            
        }

        private bool ValidarCampos()
        {
            bool validacionCampos = true;
            Style estiloTxtBoxRojo = (Style)System.Windows.Application.Current.FindResource("estiloTxtBoxRojo");
            Style estiloTxtBoxAzul = (Style)System.Windows.Application.Current.FindResource("estiloTxtBoxAzul");

           
            if (!string.IsNullOrEmpty(txtBoxNombre.Text))
            {
                String nombre = txtBoxNombre.Text;
                txtBoxNombre.Style = estiloTxtBoxAzul;
            }
            else
            {
                txtBoxNombre.Style = estiloTxtBoxRojo;
                validacionCampos = false;
            }

            if (!string.IsNullOrEmpty(txtBoxApellidos.Text))
            {
                String apellidos = txtBoxApellidos.Text;
                txtBoxApellidos.Style = estiloTxtBoxAzul;
            }
            else
            {
                txtBoxApellidos.Style = estiloTxtBoxRojo;
                validacionCampos = false;
            }

            if (!string.IsNullOrEmpty(txtBoxCorreoElectronico.Text) && ValidarCorreo(txtBoxCorreoElectronico.Text))
            {
                String correoElectronico = txtBoxCorreoElectronico.Text;
                txtBoxCorreoElectronico.Style = estiloTxtBoxAzul;
            }
            else
            {
                txtBoxCorreoElectronico.Style = estiloTxtBoxRojo;
                validacionCampos = false;
            }

            if (!string.IsNullOrEmpty(txtBoxContrasena.Text) && ValidarContraseña(txtBoxContrasena.Text))
            {
                String contraseña = txtBoxContrasena.Text;
                txtBoxContrasena.Style = estiloTxtBoxAzul;
            }
            else
            {
                txtBoxContrasena.Style = estiloTxtBoxRojo;
                validacionCampos = false;
            }

            if(!string.IsNullOrEmpty(txtBoxNickname.Text))
            {
                String nickname = txtBoxNickname.Text;
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
                System.Windows.Forms.MessageBox.Show("La correo es invalido", "Correo invalido", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
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
                System.Windows.Forms.MessageBox.Show("La correo es invalido", "Correo invalido", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }
            catch (ArgumentException e)
            {
                System.Windows.Forms.MessageBox.Show("La correo es invalido", "Correo invalido", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }
            try
            {
                return Regex.IsMatch(correo,@"^[^@\s]+@[^@\s]+\.[^@\s]+$",RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
            }
            catch (RegexMatchTimeoutException)
            {
                System.Windows.Forms.MessageBox.Show("La correo es invalido", "Correo invalido", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }
        }
        private bool ValidarContraseña(string contraseña)
        {


            return true;
        }
        private void AñadirCuenta() 
        {
            /*using (var context = new SorrySlidersBDEntities())
            {
                context.Usuario.Add(new Usuario()
                {
                    Nombres = txtBoxNombre.Text,
                    Apellidos = txtBoxApellidos.Text
                });
                context.Cuenta.Add(new Cuenta()
                {
                    CorreoElectronico = txtBoxCorreoElectronico.Text,
                    Avatar = avatarByte,
                    Contrasena = txtBoxContrasena.Text,
                    Nickname = txtBoxNickname.Text
                });
                context.SaveChanges();
                txtBoxNombre.Text = "";
                txtBoxApellidos.Text = "";
                txtBoxCorreoElectronico.Text = "";
                txtBoxContrasena.Text = "";
                txtBoxNickname.Text = "";
                System.Windows.Forms.MessageBox.Show("La cuenta se ha registrado exitosamente", "Cuenta creada", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }*/
        }

        private bool ValidarExistenciaImagen() 
        {
            bool archivoExiste = false;
            if (mgBrushAvatar.ImageSource is BitmapImage bitmapImage)
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
        private void ClicCrearCuenta(object sender, RoutedEventArgs e)
        {
            //validar correo y contraseña
            //Excepciones
            if (ValidarCampos() && ValidarExistenciaImagen())
            {
                try
                {
                    AñadirCuenta();
                    IrInicioSesion();
                }
                catch (System.Data.SqlClient.SqlException ex)
                {
                    System.Windows.Forms.MessageBox.Show("No se puede establecer una conexión con el servidor SQL Server", "Error con la Base de Datos", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

        }
        private void IrInicioSesion() 
        {
            InicioSesionPagina paginaSesionPagina = new InicioSesionPagina();
            this.NavigationService.Navigate(paginaSesionPagina);
        }
        private void ClicCancelar(object sender, RoutedEventArgs e)
        {

            InicioSesionPagina paginaSesionPagina = new InicioSesionPagina();
            this.NavigationService.Navigate(paginaSesionPagina);
        }

        private void SeleccionarImagen(object sender, MouseButtonEventArgs e)
        {
            OpenFileDialog abrirBiblioteca = new OpenFileDialog();
            abrirBiblioteca.Filter = "Archivos de imagen|*.jpg;*.jpeg;*.png";
            if (abrirBiblioteca.ShowDialog() == DialogResult.OK)
            {
                /*
                rutaImagen = abrirBiblioteca.FileName;
                BitmapImage mapaBits = new BitmapImage(new Uri(rutaImagen));
                mgBrushAvatar.ImageSource = mapaBits;
                avatarByte = File.ReadAllBytes(rutaImagen);
                */
                rutaImagen = abrirBiblioteca.FileName;
                string[] formatosSoportados = { ".jpg", ".jpeg", ".png" }; //////////////
                if (formatosSoportados.Any(ext => rutaImagen.EndsWith(ext, StringComparison.OrdinalIgnoreCase)))
                {
                    BitmapImage mapaBits = new BitmapImage(new Uri(rutaImagen));
                    mgBrushAvatar.ImageSource = mapaBits;
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show("Por favor, seleccione un archivo de imagen válido.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                
            }
        }
    }
}
