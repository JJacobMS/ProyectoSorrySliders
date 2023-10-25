using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
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
                Console.WriteLine(ex);
                Console.WriteLine("Argumento no válido al cargar la imagen");
            }
            catch (OutOfMemoryException ex)
            {
                Console.WriteLine(ex);
                Console.WriteLine("Error de memoria al cargar la imagen");
            }
            catch (System.IO.IOException ex)
            {
                Console.WriteLine(ex);
                Console.WriteLine("Error de lectura en el MemoryStream");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Console.WriteLine("Error de lectura al cargar la imagen");
            }
        }

        public static ImageBrush ConvertirBytesAImageBrush(byte[] imagenBytes)
        {
            ImageBrush imagen = new ImageBrush();
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
                Console.WriteLine(ex);
                Console.WriteLine("Argumento no válido al cargar la imagen");
            }
            catch (OutOfMemoryException ex)
            {
                Console.WriteLine(ex);
                Console.WriteLine("Error de memoria al cargar la imagen");
            }
            catch (System.IO.IOException ex)
            {
                Console.WriteLine(ex);
                Console.WriteLine("Error de lectura en el MemoryStream");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Console.WriteLine("Error de lectura al cargar la imagen");
            }

            return null;
        }

        public static byte[] GenerarImagenDefectoBytes()
        {
            string rutaImagen = "pack://application:,,,/Recursos/avatarPredefinido.jpg";
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
                return null;
            }
            catch (OutOfMemoryException ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }

    }
}
