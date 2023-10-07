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
    /// Lógica de interacción para AjustesVentana.xaml
    /// </summary>
    public partial class AjustesVentana : Window
    {
        public AjustesVentana()
        {
            InitializeComponent();
            SeleccionarIdioma(System.Threading.Thread.CurrentThread.CurrentUICulture.ToString());
        }

        private void ClicSalir(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ClicCambiarIdioma(object sender, SelectionChangedEventArgs e)
        {
            string idiomaCambio = "";
            switch (cmbBoxAjustesIdioma.SelectedIndex)
            {
                case 0:
                    idiomaCambio = "es-MX";
                    break;
                case 1:
                    idiomaCambio = "en";
                    break;
            }
            CambiarIdioma(idiomaCambio);
        }

        private void CambiarIdioma(string idiomaCambio)
        {
            switch (idiomaCambio)
            {
                case "es-MX":
                case "en":
                    App.CambiarIdioma(idiomaCambio);
                    break;
            }
        }

        private void SeleccionarIdioma(string idiomaCambio)
        {
            switch (idiomaCambio)
            {
                case "es-MX":
                    cmbBoxAjustesIdioma.SelectedIndex = 0;
                    break;
                case "en":
                    cmbBoxAjustesIdioma.SelectedIndex = 1;
                    break;
            }
        }
    }
}
