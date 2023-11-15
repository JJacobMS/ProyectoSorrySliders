﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using VistasSorrySliders.ServicioSorrySliders;

namespace VistasSorrySliders
{
    /// <summary>
    /// Lógica de interacción para JuegoYLobbyVentana.xaml
    /// </summary>
    public partial class JuegoYLobbyVentana : Window
    {
        private LobbyPagina _frame;
        public JuegoYLobbyVentana()
        {
            InitializeComponent();
            //Revisar
            /*List<CuentaSet> lista = new List<CuentaSet> { new CuentaSet { Nickname = "1"}, new CuentaSet { Nickname = "2"},
                new CuentaSet {Nickname = "3"}, new CuentaSet {Nickname= "4"}
            };
            frameLobby.Content = new JuegoLanzamientoPagina(lista, 2);*/
        }

        public JuegoYLobbyVentana(CuentaSet cuenta, string codigoPartida, bool esInvitado)
        {
            InitializeComponent();
            _frame = new LobbyPagina(cuenta, codigoPartida, esInvitado, this);
            frameLobby.Content = _frame;
            
            if (!esInvitado)
            {
                frameListaAmigos.Content = new ListaAmigosPagina(cuenta, codigoPartida);
            }
        }

        private void CerrarVentana(object sender, CancelEventArgs e) 
        {
            _frame.SalirPartida();
        }
        public void CambiarFrameLobby(Page paginaNueva)
        {

            frameLobby.Content = paginaNueva;

        }
        public void CambiarFrameListaAmigos(Page paginaNueva)
        {
            //_frame = pagina nueva;, ponerle a pagina nueva salirPartida();, y en ese metodo poner el RecargarListaJugadores
            frameListaAmigos.Content = paginaNueva;
        }

    }
}
