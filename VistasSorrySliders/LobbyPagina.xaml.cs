using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using VistasSorrySliders.ServicioSorrySliders;

namespace VistasSorrySliders
{
    /// <summary>
    /// Lógica de interacción para LobbyPagina.xaml
    /// </summary>
    public partial class LobbyPagina : Page
    {
        private CuentaSet _cuentaUsuario;

        public LobbyPagina()
        {
            InitializeComponent();
        }

        public void RecuperarDatosPartida(string codigoPartida) 
        {
            //Recuperar PartidaSet
            InicializarDatosPartida();
            CuentaSet[] cuentas = new CuentaSet[4];
            Constantes respuesta = Constantes.OPERACION_EXITOSA_VACIA;
            try
            {
                UnirsePartidaClient proxyRecuperarJugadores = new UnirsePartidaClient();
                (respuesta, cuentas) = proxyRecuperarJugadores.RecuperarJugadoresLobby(codigoPartida);
            }
            catch(CommunicationException ex)
            {
                respuesta = Constantes.ERROR_CONEXION_SERVIDOR;
            }
            switch (respuesta)
            {
                case Constantes.ERROR_CONEXION_BD:
                    Console.WriteLine("ERROR_CONEXION_BD");
                    break;
                case Constantes.ERROR_CONSULTA:
                    Console.WriteLine("ERROR_CONSULTA");
                    break;
                case Constantes.ERROR_CONEXION_SERVIDOR:
                    Console.WriteLine("ERROR_CONEXION_SERVIDOR");
                    break;
                case Constantes.OPERACION_EXITOSA:
                    CrearBorders(cuentas);
                    CrearEllipses(cuentas);
                    CrearLabels(cuentas);
                    break;
                case Constantes.OPERACION_EXITOSA_VACIA:
                    Console.WriteLine("OPERACION_EXITOSA_VACIA");

                    break;
                default:
                    break;
            }
        }

        private void CrearBorders(CuentaSet[] cuentas) 
        {
            //Cambiar numeroJugadores por la cantidad de registros en CuentaSet 
            int contador = 0;
            foreach(var cuenta in cuentas) 
            {
                Rectangle rectanguloNuevo = XamlReader.Parse(XamlWriter.Save(rctJugador)) as Rectangle;
                rectanguloNuevo.Name = "rctJugador" + (contador + 1);
                Grid.SetRow(rectanguloNuevo, contador);
                grdJugadores.Children.Add(rectanguloNuevo);
                contador = contador + 1;
            }
        }
        private void CrearEllipses(CuentaSet[] cuentas)
        {

        }
        private void CrearLabels(CuentaSet[] cuentas) 
        {
            int contador = 0;
            foreach (var cuenta in cuentas)
            {
                Label nuevoLabel = XamlReader.Parse(XamlWriter.Save(lblJugador)) as Label;
                nuevoLabel.Name = "lblJugador" + (contador + 1);
                nuevoLabel.Content = cuenta.Nickname;
                Grid.SetRow(nuevoLabel, contador);
                grdJugadores.Children.Add(nuevoLabel);
                contador = contador + 1;
            }
        }

        private void InicializarDatosPartida() 
        {
            txtBoxCodigoPartida.Text = "codigoPartida";// partida.codigoPartida;
            txtBoxHost.Text = "nicknameHost";// partida.nicknameHost;
            txtBoxJugadores.Text = "numeroJugadores";//partida.numeroJugadores;
        }

        private void ClickSalirLobbyJugadores(object sender, RoutedEventArgs e)
        {
            var ventanaPrincipal = new MainWindow();
            
            MenuPrincipalPagina menu = new MenuPrincipalPagina(_cuentaUsuario.CorreoElectronico);
            ventanaPrincipal.Content = menu;
            ventanaPrincipal.Show();
            Window.GetWindow(this).Close();
        }
    }
}
