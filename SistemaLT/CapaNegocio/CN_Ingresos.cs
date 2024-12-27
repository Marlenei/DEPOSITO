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
    public class CN_Ingresos
    {
        // Método para verificar si la cadena es alfanumérica
        private bool IsAlphanumeric(string input)
        {
            if (input == null)
            {
                return true; // O false, dependiendo de tu lógica de negocio
            }

            // Expresión regular que permite solo letras y números
            return Regex.IsMatch(input, "^[a-zA-Z0-9ñÑáéíóúÁÉÍÓÚüÜ ]*$");
        }



        private CD_Ingresos objCapaDato = new CD_Ingresos();

        public List<Ingresos> Listar()
        {
            return objCapaDato.Listar();
        }

        public int Registrar(Ingresos obj, out string Mensaje)
        {
            Mensaje = string.Empty;

            if (obj.oProveedores.IdProveedor == 0)
            {
                Mensaje = "Ingresar proveedor o razon social";
            }

            else if (obj.oProductos.IdProducto == 0)
            {
                Mensaje = "Ingresar producto";
            }

            else if (!IsAlphanumeric(obj.CodigoId))
            {
                Mensaje = "CodigoId solo pueden contener letras y números";
            }
            else if (char.IsWhiteSpace(obj.TipoIngreso))
            {
                Mensaje = "Ingresar el tipo de ingreso";
            }
            else if (!IsAlphanumeric(obj.Observaciones))
            {
                Mensaje = "Observaciones solo pueden contener letras y números.";
            }
            else if (obj.Cantidad < 0)
            {
                Mensaje = "Cantidad debe ser un número positivo.";
            }
            else if (obj.IdUsuario == 0)
            {
                Mensaje = "Ingresar usuario";
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

        public bool Editar(Ingresos obj, out string Mensaje)
        {
            Mensaje = string.Empty;
            if (obj.oProveedores.IdProveedor == 0)
            {
                Mensaje = "Ingresar proveedor o razon social";
            }

            else if (obj.oProductos.IdProducto == 0)
            {
                Mensaje = "Ingresar producto";
            }

            else if (!IsAlphanumeric(obj.CodigoId))
            {
                Mensaje = "CodigoId solo pueden contener letras y números";
            }
            else if (char.IsWhiteSpace(obj.TipoIngreso))
            {
                Mensaje = "Ingresar el tipo de ingreso";
            }
            else if (!IsAlphanumeric(obj.Observaciones))
            {
                Mensaje = "Observaciones solo pueden contener letras y números.";
            }
            else if (obj.IdUsuario == 0)
            {
                Mensaje = "Ingresar usuario";
            }
            else if (obj.Cantidad < 0)
            {
                Mensaje = "Cantidad debe ser un número positivo.";
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
