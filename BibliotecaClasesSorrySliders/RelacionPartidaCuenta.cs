//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace BibliotecaClasesSorrySliders
{
    using System;
    using System.Collections.Generic;
    
    public partial class RelacionPartidaCuenta
    {
        public System.Guid CodigoPartida { get; set; }
        public string CorreoElectronico { get; set; }
        public Nullable<int> Posicion { get; set; }
    
        public virtual Cuenta Cuenta { get; set; }
        public virtual Partida Partida { get; set; }
    }
}
