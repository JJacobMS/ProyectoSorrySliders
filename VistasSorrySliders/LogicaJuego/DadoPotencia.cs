using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows;
using System.Windows.Controls;

namespace VistasSorrySliders.LogicaJuego
{
    public class DadoPotencia
    {
        public int NumeroDado { get; set; }
        public Dictionary<int, BitmapImage> ImagenDadoCorrespondiente { get; set; }
        public Image ImagenDado { get; set; }
        public Point PosicionCanva { get; set; }

        public DadoPotencia(int numeroInicial, Point posicion, int tamanoDado)
        {
            ImagenDadoCorrespondiente = new Dictionary<int, BitmapImage>
            {
                { 1, new BitmapImage(new Uri(Properties.Resources.uriImagenDadoNumero1)) },
                { 2, new BitmapImage(new Uri(Properties.Resources.uriImagenDadoNumero2)) },
                { 3, new BitmapImage(new Uri(Properties.Resources.uriImagenDadoNumero3)) },
                { 4, new BitmapImage(new Uri(Properties.Resources.uriImagenDadoNumero4)) },
                { 5, new BitmapImage(new Uri(Properties.Resources.uriImagenDadoNumero5)) },
                { 6, new BitmapImage(new Uri(Properties.Resources.uriImagenDadoNumero6)) }
            };
            PosicionCanva = posicion;
            NumeroDado = numeroInicial;
            ImagenDado = new Image { Width = tamanoDado, Source = ImagenDadoCorrespondiente[numeroInicial] };
        }

        public void CambiarNumeroDado()
        {
            NumeroDado = (NumeroDado + 1 > 6) ? 1 : NumeroDado + 1;
            ImagenDado.Source = ImagenDadoCorrespondiente[NumeroDado];
        }

        public void AsignarPosicionDado(int numeroDado)
        {
            NumeroDado = numeroDado;
            ImagenDado.Source = ImagenDadoCorrespondiente[NumeroDado];
        }
    }
}
