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
        private bool IsAlphanumeric(string input)
        {
            if(input == null)
            {
                return true;
            }
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

            else if (!IsAlphanumeric(objeto.CodigoId))
            {
                Mensaje = "Codigo solo debe contener numeros o letras";
            }

            else if (objeto.IdUsuario == 0)
            {
                Mensaje = "Ingresar usuario";
            }

            else if (!IsAlphanumeric(objeto.Observaciones))
            {
                Mensaje = "Observaciones solo debe contener numeros o letras";
            }

            else if (objeto.TipoSalida == '\0')
            {
                Mensaje = "Debe elegir un tipo de salida";
            }

            else if (objeto.CodigoArea == 0)
            {
                Mensaje = "Ingresar Area";

            }

            else if (objeto.CodigoSector == 0)
            {
                Mensaje = "Ingresar Sector";

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

        public object Registrar(SolicitudPedidos objeto, out string mensaje)
        {
            throw new NotImplementedException();
        }
    }
}
