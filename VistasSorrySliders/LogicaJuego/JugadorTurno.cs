using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VistasSorrySliders.LogicaJuego
{
    public class JugadorTurno
    {
        private string _correoJugador;
        private int _turnoJugador;
        public string CorreoJugador { get => _correoJugador; set => _correoJugador = value; }
        public int TurnoJugador { get => _turnoJugador; set => _turnoJugador = value; }
    }
}
