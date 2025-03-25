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
    public class CN_Productos
    {
        private bool IsAlphanumeric(string input)
        {
            if (input == null)
            {
                return true;
            }
            return Regex.IsMatch(input, "^[a-zA-Z0-9ñÑáéíóúÁÉÍÓÚüÜ ]*$");
        }

        private CD_Productos objCapaDato = new CD_Productos();
        public List<Productos> Listar()
        {
            return objCapaDato.Listar();
        }
     

        public List<Productos> ListarporIDTipos(int idTipo)
        {
            return objCapaDato.ListarProductosporTipos(idTipo);
        }

        public List<Productos> ListarProductosporCI(string idCodigo)
        {
            return objCapaDato.ListarProductosporCI(idCodigo);
        }

        public int Registrar(Productos obj, out string Mensaje)
        {
            List<Productos> productosExistentes = objCapaDato.Listar();
            Mensaje = string.Empty;

            if (string.IsNullOrEmpty(obj.Detalle) || string.IsNullOrWhiteSpace(obj.Detalle))
            {
                Mensaje = "Ingresar detalle";
            }
            else if (productosExistentes.Any(t =>
                t.Detalle.Equals(obj.Detalle, StringComparison.OrdinalIgnoreCase) &&
                t.oRubros.IdRubro == obj.oRubros.IdRubro &&
                t.oTipos.IdTipo == obj.oTipos.IdTipo))
            {
                Mensaje = "El producto ya existe con el mismo detalle y rubro.";
            }

            if (obj.oRubros.IdRubro == 0)
            {
                Mensaje = "Ingresar rubro";
            }

            else if (obj.oTipos.IdTipo == 0)
            {
                Mensaje = "Ingresar tipo";
            }

            else if (!IsAlphanumeric(obj.CodigoId))
            {
                Mensaje = "El codigo ID debe contener letras y/o numeros";
            }

            else if (obj.StockMinimo < 0)
            {
                Mensaje = "Ingresar 0 o un stock minimo";
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

        public bool Editar(Productos obj, out string Mensaje)
        {
            List<Productos> productosExistentes = objCapaDato.Listar();
            Mensaje = string.Empty;

            if (string.IsNullOrEmpty(obj.Detalle) || string.IsNullOrWhiteSpace(obj.Detalle))
            {
                Mensaje = "Ingresar detalle";
            }
            else if (obj.oRubros.IdRubro == 0)
            {
                Mensaje = "Ingresar rubro";
            }

            else if (obj.oTipos.IdTipo == 0)
            {
                Mensaje = "Ingresar tipo";
            }

            //else if (!IsAlphanumeric(obj.CodigoId))
            //{
            //    Mensaje = "Ingresar codigo";
            //}

            else if (obj.StockMinimo < 0)
            {
                Mensaje = "Ingresar 0 o un stock minimo";
            }
            else if (obj.IdUsuario == 0)
            {
                Mensaje = "Ingresar usuario";
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
