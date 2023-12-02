using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VistasSorrySliders.LogicaJuego
{
    public class JugadorGanador
    {
        private string _correoElectronico;
        private string _nickname;
        private int _posicion;

        public string CorreoElectronico { get => _correoElectronico; set => _correoElectronico = value; }
        public string Nickname { get => _nickname; set => _nickname = value; }
        public int Posicion { get => _posicion; set => _posicion = value; }
    }
}
