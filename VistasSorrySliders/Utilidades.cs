using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace VistasSorrySliders
{
    public class Utilidades
    {
        public static void IngresarImagen(byte[] avatar, ImageBrush mgBrush)
        {
            Logger log = new Logger(typeof(Utilidades));
            try
            {
                BitmapImage bitmapImage = new BitmapImage();
                using (MemoryStream stream = new MemoryStream(avatar))
                {

                    bitmapImage.BeginInit();
                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImage.StreamSource = stream;
                    bitmapImage.DecodePixelWidth = 100;
                    bitmapImage.EndInit();
                    mgBrush.ImageSource = bitmapImage;
                }
            }
            catch (ArgumentException ex)
            {
                System.Windows.Forms.MessageBox.Show(Properties.Resources.msgErrorImagen, Properties.Resources.msgTituloErrorImagen, MessageBoxButtons.OK, MessageBoxIcon.Information);
                log.LogWarn("Se ha proporcionado un argumento invalido", ex);
                Console.WriteLine(ex);
            }
            catch (OutOfMemoryException ex)
            {
                System.Windows.Forms.MessageBox.Show(Properties.Resources.msgErrorImagen, Properties.Resources.msgTituloErrorImagen, MessageBoxButtons.OK, MessageBoxIcon.Information);
                log.LogWarn("Se ha agotado la memoria", ex);
                Console.WriteLine(ex);
            }
            catch (System.IO.IOException ex)
            {
                System.Windows.Forms.MessageBox.Show(Properties.Resources.msgErrorImagen, Properties.Resources.msgTituloErrorImagen, MessageBoxButtons.OK, MessageBoxIcon.Information);
                log.LogWarn("Error al acceder a la imagen", ex);
                Console.WriteLine(ex);
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(Properties.Resources.msgErrorImagen, Properties.Resources.msgTituloErrorImagen, MessageBoxButtons.OK, MessageBoxIcon.Information);
                Console.WriteLine(ex);
                log.LogFatal("Ha ocurrido un error inesperado", ex);
            }
        }

        public static ImageBrush ConvertirBytesAImageBrush(byte[] imagenBytes)
        {
            ImageBrush imagen = new ImageBrush();
            Logger log = new Logger(typeof(Utilidades));
            try
            {
                BitmapImage bitmapImage = new BitmapImage();
                using (MemoryStream stream = new MemoryStream(imagenBytes))
                {

                    bitmapImage.BeginInit();
                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImage.StreamSource = stream;
                    bitmapImage.DecodePixelWidth = 100;
                    bitmapImage.EndInit();
                    imagen.ImageSource = bitmapImage;
                    return imagen;
                }
            }
            catch (ArgumentException ex)
            {
                System.Windows.Forms.MessageBox.Show(Properties.Resources.msgErrorImagen, Properties.Resources.msgTituloErrorImagen, MessageBoxButtons.OK, MessageBoxIcon.Information);
                log.LogWarn("Se ha proporcionado un argumento invalido", ex);
                Console.WriteLine(ex);
            }
            catch (OutOfMemoryException ex)
            {
                System.Windows.Forms.MessageBox.Show(Properties.Resources.msgErrorImagen, Properties.Resources.msgTituloErrorImagen, MessageBoxButtons.OK, MessageBoxIcon.Information);
                log.LogWarn("Se ha agotado la memoria", ex);
                Console.WriteLine(ex);
            }
            catch (System.IO.IOException ex)
            {
                System.Windows.Forms.MessageBox.Show(Properties.Resources.msgErrorImagen, Properties.Resources.msgTituloErrorImagen, MessageBoxButtons.OK, MessageBoxIcon.Information);
                log.LogWarn("Error al acceder a la imagen", ex);
                Console.WriteLine(ex);
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(Properties.Resources.msgErrorImagen, Properties.Resources.msgTituloErrorImagen, MessageBoxButtons.OK, MessageBoxIcon.Information);
                log.LogFatal("Ha ocurrido un error inesperado", ex);
                Console.WriteLine(ex);
            }

            return null;
        }

        public static byte[] GenerarImagenDefectoBytes()
        {
            string rutaImagen = "pack://application:,,,/Recursos/avatarPredefinido.jpg";
            Logger log = new Logger(typeof(Utilidades));
            try
            {
                BitmapImage bitmap = new BitmapImage(new Uri(rutaImagen));
                JpegBitmapEncoder encoder = new JpegBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bitmap));
                using (MemoryStream ms = new MemoryStream())
                {
                    encoder.Save(ms);
                    return ms.ToArray();
                }
            }
            catch (IOException ex)
            {
                Console.WriteLine(ex);
                System.Windows.Forms.MessageBox.Show(Properties.Resources.msgErrorImagen, Properties.Resources.msgTituloErrorImagen, MessageBoxButtons.OK, MessageBoxIcon.Information);
                log.LogWarn("Error al acceder a la imagen", ex);
                return null;
            }
            catch (ArgumentException ex)
            {
                System.Windows.Forms.MessageBox.Show(Properties.Resources.msgErrorImagen, Properties.Resources.msgTituloErrorImagen, MessageBoxButtons.OK, MessageBoxIcon.Information);
                log.LogWarn("Se ha proporcionado un argumento invalido", ex);
                Console.WriteLine(ex);
                return null;
            }
            catch (OutOfMemoryException ex)
            {
                Console.WriteLine(ex);
                System.Windows.Forms.MessageBox.Show(Properties.Resources.msgErrorImagen, Properties.Resources.msgTituloErrorImagen, MessageBoxButtons.OK, MessageBoxIcon.Information);
                log.LogWarn("Se ha agotado la memoria", ex);
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                System.Windows.Forms.MessageBox.Show(Properties.Resources.msgErrorImagen, Properties.Resources.msgTituloErrorImagen, MessageBoxButtons.OK, MessageBoxIcon.Information);
                log.LogFatal("Ha ocurrido un error inesperado", ex);
                return null;
            }

        }

        public static bool ValidarContraseña(string contraseña)
        {
            Logger log = new Logger(typeof(Utilidades));
            try
            {
                string patron = @"^(?=.*[0-9!@#$%^&*()\-=_+.,:;])[A-Za-z0-9!@#$%^&*()\-=_+.,:;]{8,}$";
                Regex regex = new Regex(patron);
                bool correoValidado = regex.IsMatch(contraseña);
                if (correoValidado)
                {
                    return correoValidado;
                }
                MessageBox.Show(Properties.Resources.msgErrorContrasenaInvalida, Properties.Resources.msgTituloContraseñaInvalida, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return correoValidado;
            }
            catch (RegexMatchTimeoutException ex)
            {
                Console.WriteLine(ex);
                log.LogWarn("El tiempo de espera para la expresión se ha agotado", ex);
                //MessageBox.Show();
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                log.LogFatal("Ha ocurrido un error inesperado", ex);
                //MessageBox.Show();
                return false;
            }
        }

    }
}
