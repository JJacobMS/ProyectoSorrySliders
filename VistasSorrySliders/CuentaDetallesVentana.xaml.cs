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
using VistasSorrySliders.ServicioSorrySliders;

namespace VistasSorrySliders
{
    /// <summary>
    /// Lógica de interacción para CuentaDetallesVentana.xaml
    /// </summary>
    public partial class CuentaDetallesVentana : Window
    {
        private CuentaSet _cuenta;
        public CuentaDetallesVentana(CuentaSet cuenta)
        {
            InitializeComponent();
            _cuenta = cuenta;
            ColocarDatos();
        }

        private void ColocarDatos()
        {
            txtBoxCorreo.Text = _cuenta.CorreoElectronico;
            txtBoxNickname.Text = _cuenta.Nickname;
        }
    }
}
