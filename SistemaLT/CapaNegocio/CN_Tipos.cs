using CapaDatos;
using CapaEntidad;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CapaNegocio
{
    public class CN_Tipos
    {
        private bool IsAlphanumeric(string input)
        {
            if (input == null)
            {
                return true;
            }
            return Regex.IsMatch(input, "^[a-zA-Z0-9ñÑáéíóúÁÉÍÓÚüÜ ]*$");
        }

        private CD_Tipos objCapaDato = new CD_Tipos();
        public List<Tipos> Listar()
        {
            return objCapaDato.Listar();
        }

        public List<Tipos> ListarporIDRubro(int idRubro)
        {
            return objCapaDato.ListarTipoporRubro(idRubro);
        }

        public int Registrar(Tipos obj, out string Mensaje)
        {
            Mensaje = string.Empty;

            if (!IsAlphanumeric(obj.Tipo))
            {
                Mensaje = "Solo se aceptan letras y numeros";
            }

            else if (obj.oRubros.IdRubro == 0)
            {
                Mensaje = "Ingresar rubro";
            }
            if (string.IsNullOrEmpty(Mensaje))
            {
                return objCapaDato.Registrar(obj);
            }
            else
            {
                return 0;
            }
        }

        public bool Editar(Tipos obj, out string Mensaje)
        {
            Mensaje = string.Empty;

            if (!IsAlphanumeric(obj.Tipo))
            {
                Mensaje = "Solo se aceptan letras y numeros";
            }
            else if (obj.oRubros.IdRubro == 0)
            {
                Mensaje = "Ingresar rubro";
            }
            if (string.IsNullOrEmpty(Mensaje))
            {
                return objCapaDato.Editar(obj, out Mensaje);
            }
            else
            {
                return false;
            }
        }
    }
}
