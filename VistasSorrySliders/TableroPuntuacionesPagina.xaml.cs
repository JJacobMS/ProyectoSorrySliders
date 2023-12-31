﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Lógica de interacción para TableroPuntuacionesPagina.xaml
    /// </summary>
    public partial class TableroPuntuacionesPagina : Page
    {
        private CuentaSet _cuenta;
        public ObservableCollection<Puntuacion> ListaPuntuaciones { get; set; }
        private Puntuacion[] _puntuaciones;


        public TableroPuntuacionesPagina(CuentaSet cuenta)
        {
            InitializeComponent();
            
            _cuenta = cuenta;
        }

        public Constantes InicializarPuntuaciones()
        {
            return RecuperarPuntuaciones();
        }

        private Constantes RecuperarPuntuaciones() 
        {
            Constantes resultado;
            Puntuacion[] puntuaciones = new Puntuacion[] { };
            Logger log = new Logger(this.GetType());
            try
            {
                PuntuacionClient proxyRecuperarPuntuaciones = new PuntuacionClient();
                (resultado, puntuaciones) = proxyRecuperarPuntuaciones.RecuperarPuntuaciones();
                proxyRecuperarPuntuaciones.Close();
            }
            catch (CommunicationException ex)
            {
                resultado = Constantes.ERROR_CONEXION_SERVIDOR;
                log.LogWarn("Error de Comunicación con el Servidor", ex);
            }
            catch (TimeoutException ex)
            {
                resultado = Constantes.ERROR_TIEMPO_ESPERA_SERVIDOR;
                log.LogWarn("Se agoto el tiempo de espera del servidor", ex);
            }
            switch (resultado)
            {
                case Constantes.OPERACION_EXITOSA:
                    _puntuaciones = puntuaciones;
                    AgregarPuntuacionesTablero();
                    break;
                case Constantes.OPERACION_EXITOSA_VACIA:
                    Utilidades.MostrarUnMensajeError(Properties.Resources.msgTablaVacia);
                    break;
                default:
                    Utilidades.MostrarMensajesError(resultado);
                    break;
            }
            return resultado;
        }

        private void AgregarPuntuacionesTablero() 
        {
            if (_puntuaciones != null) {
                ListaPuntuaciones = new ObservableCollection<Puntuacion>();
                foreach (var puntuacion in _puntuaciones)
                {
                    ListaPuntuaciones.Add(puntuacion);
                }
                this.DataContext = this;
            }
        }

        private void ClickSalirMenuPrincipal(object sender, RoutedEventArgs e)
        {
            MenuPrincipalPagina menu = new MenuPrincipalPagina(_cuenta);
            this.NavigationService.Navigate(menu);
        }
    }
}
