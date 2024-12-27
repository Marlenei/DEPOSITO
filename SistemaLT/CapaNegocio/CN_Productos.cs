using CapaDatos;
using CapaEntidad;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaNegocio
{
    public class CN_Productos
    {
        private CD_Productos objCapaDato = new CD_Productos();
        public List<Productos> Listar()
        {
            return objCapaDato.Listar();
        }

        public int Registrar(Productos obj, out string Mensaje)
        {
            Mensaje = string.Empty;
            if (string.IsNullOrEmpty(obj.Detalle) || string.IsNullOrWhiteSpace(obj.Detalle))
            {
                Mensaje = "Ingresar detalle";
            }

            if (obj.oRubros.IdRubro == 0)
            {
                Mensaje = "Ingresar rubro";
            }

            else if (obj.oTipos.IdTipo == 0)
            {
                Mensaje = "Ingresar tipo";
            }

            else if (string.IsNullOrEmpty(obj.CodigoId) || string.IsNullOrWhiteSpace(obj.CodigoId))
            {
                Mensaje = "Ingresar codigo";
            }

            else if (obj.StockMinimo < 0)
            {
                Mensaje = "Ingresar 0 o un stock minimo";
            }
            else if (obj.IdUsuario == 0)
            {
                Mensaje = "Ingresar usuario";
            }
            //else if (obj.Activo != true && obj.Activo != false)
            //{
            //    Mensaje = "Ingrese activo o desactivado";
            //}

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

            else if (string.IsNullOrEmpty(obj.CodigoId) || string.IsNullOrWhiteSpace(obj.CodigoId))
            {
                Mensaje = "Ingresar codigo";
            }

            else if (obj.StockMinimo < 0)
            {
                Mensaje = "Ingresar 0 o un stock minimo";
            }
            else if (obj.IdUsuario == 0)
            {
                Mensaje = "Ingresar usuario";
            }
            //else if (obj.Activo == true || obj.Activo == false)
            //{
            //    Mensaje = "Ingrese activo o desactivado";
            //}

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
