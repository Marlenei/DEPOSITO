using CapaEntidad;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaNegocio
{
    public class CN_Usuario
    {
        public int Ingresar(Acceso obj, out string Mensaje)
        {
            Mensaje = string.Empty;
            if (string.IsNullOrEmpty(obj.usuario) || string.IsNullOrWhiteSpace(obj.usuario))
            {
                Mensaje = "Ingresar nombre de usuario";
                return 0;
            }
            else if (string.IsNullOrEmpty(obj.clave) || string.IsNullOrWhiteSpace(obj.clave))
            {
                Mensaje = "Ingresar contraseña";
                return 0;
            }
            else
            {
                return 0;
            }
        }
    }
}
