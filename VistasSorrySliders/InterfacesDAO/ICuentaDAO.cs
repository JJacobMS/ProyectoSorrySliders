using BibliotecaClasesSorrySliders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VistasSorrySliders.InterfacesDAO
{
    public interface ICuentaDAO
    {
        List<Cuenta> VerificarExistenciaCorreoCuenta(Cuenta cuentaPorVerificar);
        List<string> VerificarContrasenaDeCuenta(Cuenta cuentaPorVerificar);
    }
}
