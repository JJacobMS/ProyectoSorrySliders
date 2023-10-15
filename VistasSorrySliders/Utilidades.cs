﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

    }
}