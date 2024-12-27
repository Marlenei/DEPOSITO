using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CapaDatos;
using CapaEntidad;

namespace CapaNegocio
{
    public class CN_Egresos

    {
        // Método para verificar si la cadena es alfanumérica
        private bool IsAlphanumeric(string input)
        {
            // Expresión regular que permite solo letras y números
            return Regex.IsMatch(input, "^[a-zA-Z0-9]*$");
        }

        private CD_Egresos objCapaDato = new CD_Egresos();

        public List<Egresos> Listar()
        {
            return objCapaDato.Listar();
        }

        public int Registrar(Egresos objeto, out string Mensaje)
        {
            Mensaje = string.Empty;

            

             if (objeto.oProductos.IdProducto == 0)
            {
                Mensaje = "Ingresar producto";
            }

            else if (string.IsNullOrEmpty(objeto.CodigoId) || string.IsNullOrWhiteSpace(objeto.CodigoId))
            {
                Mensaje = "Ingresar codigo";
            }
            
            else if (objeto.IdUsuario == 0)
            {
                Mensaje = "Ingresar usuario";
            }

            if (string.IsNullOrEmpty(Mensaje))
            {
                return objCapaDato.Registrar(objeto);
            }
            else
            {
                return 0;
            }

        }
        public bool Editar(Egresos objeto, out string Mensaje)
        {
            Mensaje = string.Empty;
            

             if (objeto.oProductos.IdProducto == 0)
            {
                Mensaje = "Ingresar producto";
            }

            else if (string.IsNullOrEmpty(objeto.CodigoId) || string.IsNullOrWhiteSpace(objeto.CodigoId))
            {
                Mensaje = "Ingresar codigo";
            }

            else if (string.IsNullOrEmpty(objeto.Observaciones) || string.IsNullOrWhiteSpace(objeto.Observaciones))
            {
                Mensaje = "Ingresar codigo";
            }

            if (string.IsNullOrEmpty(Mensaje))
            {
                return objCapaDato.Editar(objeto, out Mensaje);
            }
            else
            {
                return false;
            }




        }

    }
}
