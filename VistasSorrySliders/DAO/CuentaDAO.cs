using BibliotecaClasesSorrySliders;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VistasSorrySliders.InterfacesDAO;

namespace VistasSorrySliders.DAO
{
    public class CuentaDAO: ICuentaDAO
    {/*
        //Cheacr existencia correo
        //Cehcar existencia contraseña del correo
        public List<Cuenta> VerificarExistenciaCorreoCuenta(Cuenta cuentaPorVerificar)
        {
            try
            {
                using (var contexto = new SorrySlidersBDEntities())
                {
                    List<Cuenta> cuentaVerificada = new List<Cuenta>();
                    cuentaVerificada = contexto.Cuenta.
                    Where(cuenta => cuenta.CorreoElectronico == cuentaPorVerificar.CorreoElectronico).
                    ToList();
                    return cuentaVerificada;
                }
            }
            catch (EntityException ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
        }
        public List<string> VerificarContrasenaDeCuenta(Cuenta cuentaPorVerificar)
        {
            try
            {
                using (var contexto = new SorrySlidersBDEntities())
                {
                    List<string> correoCuenta = new List<string>();
                    correoCuenta = contexto.Database.
                        SqlQuery<string>("SELECT CorreoElectronico From Cuenta WHERE HASHBYTES('SHA2_256', @contrasena) = Contrasena AND CorreoElectronico = @correo", 
                        new SqlParameter("@contrasena", cuentaPorVerificar.Contrasena), new SqlParameter("@correo",cuentaPorVerificar.CorreoElectronico)).
                    ToList();

                    return correoCuenta;
                }
            }
            catch (EntityException ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
        }*/
    }
}
