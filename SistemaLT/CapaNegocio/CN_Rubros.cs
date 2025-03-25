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
    public class CN_Rubros
    {
        private bool IsAlphanumeric(string input)
        {
            if (input == null)
            {
                return true;
            }
            return Regex.IsMatch(input, "^[a-zA-Z0-9ñÑáéíóúÁÉÍÓÚüÜ ]*$");
        }

        private CD_Rubros objCapaDato = new CD_Rubros();


        public List<Rubros> Listar()
        {
            return objCapaDato.Listar();
        }


        public int Registrar(Rubros obj, out string Mensaje)
        {
            List<Rubros> rubrosExistentes = objCapaDato.Listar();
            Mensaje = string.Empty;
            if (!IsAlphanumeric(obj.Rubro))
            {
                Mensaje = "Ingresar solamente numeros y/o letras";
            }
            else if (rubrosExistentes.Any(t => t.Rubro.Equals(obj.Rubro, StringComparison.OrdinalIgnoreCase)))
            {
                Mensaje = "El rubro ya existe";
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

        public bool Editar(Rubros obj, out string Mensaje)
        {
            List<Rubros> rubrosExistentes = objCapaDato.Listar();
            Mensaje = string.Empty;
            if (!IsAlphanumeric(obj.Rubro))
            {
                Mensaje = "Ingresar solamente numeros y/o letras";
            }
            else if (rubrosExistentes.Any(t => t.Rubro.Equals(obj.Rubro, StringComparison.OrdinalIgnoreCase)))
            {
                Mensaje = "El rubro ya existe";
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
