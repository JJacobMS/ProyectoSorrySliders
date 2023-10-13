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
    /// Lógica de interacción para JuegoYLobbyVentana.xaml
    /// </summary>
    public partial class JuegoYLobbyVentana : Window
    {
        public JuegoYLobbyVentana(int numeroJugadores, CuentaSet cuentaUsuario)
        {
            InitializeComponent();
            frameLobby.Content = new LobbyPagina();
            frameListaAmigos.Content = new ListaAmigosPagina();
        }
    }
}
