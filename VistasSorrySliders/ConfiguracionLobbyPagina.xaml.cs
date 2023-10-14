﻿using System;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using VistasSorrySliders.ServicioSorrySliders;

namespace VistasSorrySliders
{
    /// <summary>
    /// Lógica de interacción para ConfiguracionLobby.xaml
    /// </summary>
    public partial class ConfiguracionLobby : Page
    {
        private CuentaSet _cuentaUsuario;
        public CuentaSet CuentaUsuario { get => _cuentaUsuario; set => _cuentaUsuario = value; }

        public ConfiguracionLobby(CuentaSet cuentaUsuario)
        {
            InitializeComponent();
            CuentaUsuario = new CuentaSet();
            CuentaUsuario.CorreoElectronico = cuentaUsuario.CorreoElectronico;
            CuentaUsuario.Nickname = cuentaUsuario.Nickname;
            CuentaUsuario.Avatar = cuentaUsuario.Avatar;
            txtBlockNickname.Text = cuentaUsuario.Nickname;
            Console.WriteLine("Clic");
        }

        private Border selectedBorder;

        private void MouseLeftButtonDownSeleccionarTablero(object sender, MouseButtonEventArgs e)
        {
            Border clickedBorder = sender as Border;

            if (selectedBorder != null)
            {
                selectedBorder.BorderBrush = Brushes.Black;
                selectedBorder.BorderThickness = new Thickness(0);
            }
            clickedBorder.BorderThickness = new Thickness(4);
            clickedBorder.BorderBrush = Brushes.Red; 
            selectedBorder = clickedBorder;

            
            btnCrearLobby.IsEnabled = (selectedBorder != null);
            e.Handled = false;
        }

        private void ClickCrearLobby(object sender, RoutedEventArgs e)
        {
            if (selectedBorder != null)
            {
                int numeroJugadores = 0;
                if (selectedBorder.Name == "brdTablero4Jugadores")
                {
                    numeroJugadores = 4;
                }
                else if (selectedBorder.Name == "brdTablero3Jugadores")
                {
                    numeroJugadores = 3;
                }
                else if (selectedBorder.Name == "brdTablero2Jugadores")
                {
                    numeroJugadores = 2;
                }
                JuegoYLobbyVentana juegolobby = new JuegoYLobbyVentana(numeroJugadores, CuentaUsuario);
                juegolobby.Show();
            }
        }

        private void ClickSalirConfigurarLobby(object sender, RoutedEventArgs e)
        {
            MenuPrincipalPagina menu = new MenuPrincipalPagina(CuentaUsuario.CorreoElectronico);
            this.NavigationService.Navigate(menu);
        }
    }
}