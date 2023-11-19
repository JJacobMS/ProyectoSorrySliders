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

        public JugadorLista()
        {
            
        }

        public JugadorLista(string nickname, string correoElectronico, bool estaExpulsado, bool esHost)
        {
            Nickname = nickname;
            CorreoElectronico = correoElectronico;
            EstaExpulsado = estaExpulsado;
            EsHost = esHost;
        }

    }
}
