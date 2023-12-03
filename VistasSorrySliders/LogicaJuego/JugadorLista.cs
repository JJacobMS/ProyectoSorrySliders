using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VistasSorrySliders.LogicaJuego
{
    public class JugadorLista
    {
        public string Nickname { get; set; }
        public string CorreoElectronico { get; set; }
        public bool EstaExpulsado { get; set; }
        public bool EsHost { get; set; }
        public bool EstaEnLinea { get; set; }

        public JugadorLista()
        {
            
        }


    }
}
