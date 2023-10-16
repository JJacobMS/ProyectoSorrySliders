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

        public LobbyPagina(CuentaSet cuentaUsuario)
        {
            InitializeComponent();
            _cuentaUsuario = cuentaUsuario;
        }

        public void RecuperarDatosPartida(string codigoPartida) 
        {
            InicializarDatosPartida(codigoPartida);
            CuentaSet[] cuentas = new CuentaSet[4];
            Constantes respuesta = Constantes.OPERACION_EXITOSA_VACIA;
            try
            {
                UnirsePartidaClient proxyRecuperarJugadores = new UnirsePartidaClient();
                (respuesta, cuentas) = proxyRecuperarJugadores.RecuperarJugadoresLobby(codigoPartida);
            }
            catch(CommunicationException ex)
            {
                Console.WriteLine(ex);
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
                    Console.WriteLine("OPERACION_EXITOSA_VACIA1");
                    break;
                default:
                    break;
            }
        }

        private void CrearBorders(CuentaSet[] cuentas) 
        {
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
            int contador = 0;
            foreach (var cuenta in cuentas)
            {
                //Falta inicializar mgBrushAvatar con la imagen de CuentaSet
                Ellipse nuevaEllipseAvatar = XamlReader.Parse(XamlWriter.Save(llpAvatar)) as Ellipse;
                nuevaEllipseAvatar.Name = "llpAvatarJugador" + (contador + 1);
                Grid.SetRow(nuevaEllipseAvatar, contador);
                grdJugadores.Children.Add(nuevaEllipseAvatar);


                Ellipse nuevaEllipseFondo = XamlReader.Parse(XamlWriter.Save(llpFondo)) as Ellipse;
                nuevaEllipseFondo.Name = "llpFondoJugador" + (contador + 1);
                Grid.SetRow(nuevaEllipseFondo, contador);
                grdJugadores.Children.Add(nuevaEllipseFondo);
                contador = contador + 1;
            }
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

        private void InicializarDatosPartida(string codigoPartida) 
        {
            PartidaSet partidaActual = new PartidaSet();
            Constantes respuesta = Constantes.OPERACION_EXITOSA_VACIA;
            try
            {
                UnirsePartidaClient proxyUnirsePartida = new UnirsePartidaClient();
                (respuesta, partidaActual) = proxyUnirsePartida.RecuperarPartida(codigoPartida);
            }
            catch(CommunicationException ex)
            {
                Console.WriteLine(ex);
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
                    txtBoxCodigoPartida.Text = partidaActual.CodigoPartida.ToString();
                    txtBoxHost.Text = partidaActual.CorreoElectronico;
                    txtBoxJugadores.Text = ""+partidaActual.CantidadJugadores;
                    break;
                case Constantes.OPERACION_EXITOSA_VACIA:
                    Console.WriteLine("OPERACION_EXITOSA_VACIA2");
                    break;
                default:
                    break;
            }
            
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
