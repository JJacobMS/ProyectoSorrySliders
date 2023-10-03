using BibliotecaClasesSorrySliders;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServidorSorrySliders
{
    public class RegistroUsuarioServicio : IRegistroUsuario
    {
        public int AgregarUsuario(UsuarioSet usuarioNuevo, CuentaSet cuentaNueva)
        {
            try
            {
                using (var context = new BaseDeDatosSorrySlidersEntities())
                {
                    context.UsuarioSet.Add(usuarioNuevo);
                    context.CuentaSet.Add(cuentaNueva);
                    context.SaveChanges();
                }
                return 1;
            }
            catch (EntitySqlException ex) //Errores sintaxis consulta
            {
                Console.WriteLine(ex.ToString());
                return -1;
            }
            catch (SqlException ex) //Error Conexion BD abajo
            {
                Console.WriteLine(ex.ToString());
                return -1;
            }
            catch (EntityException ex) //Errores entity
            {
                Console.WriteLine(ex.ToString());
                return -1;
            }
        }
    }
}
