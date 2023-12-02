using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace VistasSorrySliders
{
    /// <summary>
    /// Lógica de interacción para InformacionVentana.xaml
    /// </summary>
    public partial class InformacionVentana : Window
    {
        public InformacionVentana(string mensajeInformacion, string titulo, bool esError)
        {
            InitializeComponent();
            if (!esError)
            {
                CambiarEstiloInformacion();
            }
            txtBlockMensaje.Text = mensajeInformacion;
            Title = titulo;
        }

        public InformacionVentana(string mensajeInformacion, bool esError)
        {
            InitializeComponent();
            if (!esError)
            {
                CambiarEstiloInformacion();
            }
            txtBlockMensaje.Text = mensajeInformacion;
        }

        private void CambiarEstiloInformacion()
        {
            BrushConverter convertirColor = new BrushConverter();
            grdFondoPrincipal.Background = (Brush)convertirColor.ConvertFrom("#DEFDEE");
            grdFondoSecundario.Background = (Brush)convertirColor.ConvertFrom("#2DC17A");

            btnAceptar.Style = (Style)FindResource("estiloBtnAceptarNotificacion");
            mgIconoInformacion.Source = new BitmapImage(new Uri(Properties.Resources.uriIconoInformacion));
        }

        private void ClickAceptar(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
