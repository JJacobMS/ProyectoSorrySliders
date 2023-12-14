using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using VistasSorrySliders.ServicioSorrySliders;

namespace VistasSorrySliders
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class VentanaPrincipal : NavigationWindow
    {
        private string _correo;

        public VentanaPrincipal()
        {
            InitializeComponent();
        }
        public VentanaPrincipal(string correo)
        {
            InitializeComponent();
            _correo = correo;
        }

        public void IndicarCorreoCuenta(string correo)
        {
            _correo = correo;
        }
    }
}
