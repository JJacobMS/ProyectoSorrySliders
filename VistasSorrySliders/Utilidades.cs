using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using VistasSorrySliders.ServicioSorrySliders;

namespace VistasSorrySliders
{
    public static class Utilidades
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
                MostrarUnMensajeError(Properties.Resources.msgErrorImagen, Properties.Resources.msgTituloErrorImagen);
                log.LogError("Se ha proporcionado un argumento invalido", ex);
            }
            catch (OutOfMemoryException ex)
            {
                MostrarUnMensajeError(Properties.Resources.msgErrorImagen, Properties.Resources.msgTituloErrorImagen);
                log.LogError("Se ha agotado la memoria", ex);
            }
            catch (IOException ex)
            {
                MostrarUnMensajeError(Properties.Resources.msgErrorImagen, Properties.Resources.msgTituloErrorImagen);
                log.LogError("Error al acceder a la imagen", ex);
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
                MostrarUnMensajeError(Properties.Resources.msgErrorImagen, Properties.Resources.msgTituloErrorImagen);
                log.LogError("Se ha proporcionado un argumento invalido", ex);
            }
            catch (OutOfMemoryException ex)
            {
                MostrarUnMensajeError(Properties.Resources.msgErrorImagen, Properties.Resources.msgTituloErrorImagen);
                log.LogError("Se ha agotado la memoria", ex);
            }
            catch (IOException ex)
            {
                MostrarUnMensajeError(Properties.Resources.msgErrorImagen, Properties.Resources.msgTituloErrorImagen);
                log.LogError("Error al acceder a la imagen", ex);
            }

            return null;
        }

        public static byte[] GenerarImagenDefectoBytes()
        {
            string rutaImagen = Properties.Resources.uriAvatarPorDefecto;
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
                MostrarUnMensajeError(Properties.Resources.msgErrorImagen, Properties.Resources.msgTituloErrorImagen);
                log.LogError("Error al acceder a la imagen", ex);
                return new byte[0];
            }
            catch (ArgumentException ex)
            {
                MostrarUnMensajeError(Properties.Resources.msgErrorImagen, Properties.Resources.msgTituloErrorImagen);
                log.LogError("Se ha proporcionado un argumento invalido", ex);
                return new byte[0];
            }
            catch (OutOfMemoryException ex)
            {
                MostrarUnMensajeError(Properties.Resources.msgErrorImagen, Properties.Resources.msgTituloErrorImagen);
                log.LogError("Se ha agotado la memoria", ex);
                return new byte[0];
            }

        }

        public static bool ValidarContraseña(string contraseña)
        {
            List<char> caracteresEspeciales = new List<char> { '°','!','"','#','$','%','&','/','(',')','=','|','¬',(char)92,'¿','?','-','_','+','{','}' };
            if (string.IsNullOrEmpty(contraseña) || contraseña.Length < 10)
            {
                return false;
            }
            int numeroCaracteres = 0;
            for (int i = 0; i < caracteresEspeciales.Count; i++)
            {
                if (caracteresEspeciales.Contains(caracteresEspeciales[i]))
                {
                    numeroCaracteres++;
                }
            }
            if (numeroCaracteres <= 0)
            {
                return false;
            }
            if (!contraseña.Any(char.IsDigit))
            {
                return false;
            }
            return true;
        }

        public static void MostrarMensajesError(Constantes respuesta)
        {
            switch (respuesta)
            {
                case Constantes.ERROR_CONEXION_BD:
                    MostrarUnMensajeError(Properties.Resources.msgErrorBaseDatos);
                    break;
                case Constantes.ERROR_CONSULTA:
                    MostrarUnMensajeError(Properties.Resources.msgErrorConsulta);
                    break;
                case Constantes.ERROR_CONEXION_SERVIDOR:
                    MostrarUnMensajeError(Properties.Resources.msgErrorConexion);
                    break;
                case Constantes.ERROR_TIEMPO_ESPERA_SERVIDOR:
                    MostrarUnMensajeError(Properties.Resources.msgErrorTiempoEsperaServidor);
                    break;
                case Constantes.ERROR_CONEXION_DEFECTUOSA:
                    MostrarUnMensajeError(Properties.Resources.msgEstadoDefectuoso);
                    break;
            }
        }

        public static void MostrarUnMensajeError(string mensaje)
        {
            InformacionVentana informacion = new InformacionVentana(mensaje, true);
            informacion.Show();
        }

        public static void MostrarUnMensajeError(string mensaje, string titulo)
        {
            InformacionVentana informacion = new InformacionVentana(mensaje, titulo, true);
            informacion.Show();
        }

        public static void MostrarMensajeInformacion(string mensaje, string titulo)
        {
            InformacionVentana informacion = new InformacionVentana(mensaje, titulo, false);
            informacion.Show();
        }
        public static void MostrarMensajeInformacion(string mensaje)
        {
            InformacionVentana informacion = new InformacionVentana(mensaje, false);
            informacion.Show();
        }

        public static void SalirInicioSesionDesdeVentanaPrincipal(Page pagina)
        {
            VentanaPrincipal ventanaPrincipal = Window.GetWindow(pagina) as VentanaPrincipal;

            MostrarInicioSesion(ventanaPrincipal);
        }

        public static void SalirHastaInicioSesionDesdeJuegoYLobbyVentana(Page pagina)
        {
            JuegoYLobbyVentana ventanaJuego = Window.GetWindow(pagina) as JuegoYLobbyVentana;
            ventanaJuego?.DesuscribirseDeCerrarVentana();

            MostrarInicioSesion(ventanaJuego);
        }

        public static void MostrarInicioSesion(Window ventanaAnterior)
        {
            VentanaPrincipal ventana = new VentanaPrincipal();
            InicioSesionPagina inicio = new InicioSesionPagina();
            ventana.Content = inicio;

            ventana.Show();
            if (ventanaAnterior != null)
            {
                ventanaAnterior.Close();
            }
        }

        public static void MostrarInicioSesion()
        {
            VentanaPrincipal ventana = new VentanaPrincipal();
            InicioSesionPagina inicio = new InicioSesionPagina();
            ventana.Content = inicio;

            ventana.Show();
        }

        public static void MostrarInicioSesion(Window ventanaAnterior, Window ventanaNueva)
        {
            InicioSesionPagina inicio = new InicioSesionPagina();
            ventanaNueva.Content = inicio;

            ventanaNueva.Show();
            ventanaAnterior.Close();
        }
    }
}
