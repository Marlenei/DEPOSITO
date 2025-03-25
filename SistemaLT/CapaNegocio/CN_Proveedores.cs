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
    public class CN_Proveedores
    {
        private bool IsAlphanumeric(string input)
        {
            if (input == null)
            {
                return true;
            }
            return Regex.IsMatch(input, "^[a-zA-Z0-9ñÑáéíóúÁÉÍÓÚüÜ ]*$");
        }
        private CD_Proveedores objCapaDato = new CD_Proveedores();
        public List<Proveedores> Listar()
        {
            return objCapaDato.Listar();
        }

        public int Registrar(Proveedores obj, out string Mensaje)
        {
            List<Proveedores> proveedoresExistentes = objCapaDato.Listar();
            Mensaje = string.Empty;
            if (!IsAlphanumeric(obj.RazonSocial))
            {
                Mensaje = "Ingresar razon social";
            }
            else if (proveedoresExistentes.Any(t => t.RazonSocial.Equals(obj.RazonSocial, StringComparison.OrdinalIgnoreCase)))
            {
                Mensaje = "El proveedor ya existe";
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

        public bool Editar(Proveedores obj, out string Mensaje)
        {
            Mensaje = string.Empty;
            if (!IsAlphanumeric(obj.RazonSocial))
            {
                Mensaje = "Ingresar razon social";
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

        public bool Eliminar(int id, out string Mensaje)
        {
            return objCapaDato.Eliminar(id, out Mensaje);

        }


    }
}
