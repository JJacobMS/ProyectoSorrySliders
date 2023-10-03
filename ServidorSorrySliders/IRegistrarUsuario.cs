using BibliotecaClasesSorrySliders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ServidorSorrySliders
{
    [ServiceContract]
    public interface IRegistroUsuario
    {
        [OperationContract]
        int AgregarUsuario(UsuarioSet usuarioNuevo, CuentaSet cuentaNueva);
    }

}
