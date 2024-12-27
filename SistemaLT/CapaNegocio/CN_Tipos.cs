using CapaDatos;
using CapaEntidad;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaNegocio
{
    public class CN_Tipos
    {
        private CD_Tipos objCapaDato = new CD_Tipos();
        public List<Tipos> Listar()
        {
            return objCapaDato.Listar();
        }

        public int Registrar(Tipos obj, out string Mensaje)
        {
            Mensaje = string.Empty;
            if (string.IsNullOrEmpty(obj.Tipo) || string.IsNullOrWhiteSpace(obj.Tipo))
            {
                Mensaje = "Ingresar tipo";
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
            if (string.IsNullOrEmpty(obj.Tipo) || string.IsNullOrWhiteSpace(obj.Tipo))
            {
                Mensaje = "Ingresar tipo";
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

        public bool Eliminar(int id, out string Mensaje)
        {
            return objCapaDato.Eliminar(id, out Mensaje);

        }
    }
}
