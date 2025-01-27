using CapaDatos;
using CapaEntidad;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaNegocio
{
    public class CN_SolicitudPedidos
    {
        private CD_SolicitudPedidos objCapaDato = new CD_SolicitudPedidos();

        public List<SolicitudPedidos> Listar()
        {
            return objCapaDato.Listar();
        }


        public int Registrar(SolicitudPedidos obj, out string Mensaje)
        {
            Mensaje = string.Empty;

            // Validaciones de negocio
            if (obj == null)
            {
                Mensaje = "El objeto de solicitud no puede ser nulo. ";
                return 0;
            }

            if (obj.CantidadPedida == 0)
            {
                Mensaje += "Ingresar Cantidad. ";
            }

            if (obj.CantidadPedida < 0)
            {
                Mensaje += "La cantidad pedida no puede ser negativa. ";
            }

            if (obj.oProductos == null || obj.oProductos.IdProducto == 0)
            {
                Mensaje += "Seleccionar un producto. ";
            }

           

            if (obj.CantidadEntregada > obj.CantidadPedida)
            {
                Mensaje += "La cantidad entregada no puede ser mayor a la cantidad pedida. ";
            }

            return string.IsNullOrEmpty(Mensaje) ? objCapaDato.Registrar(obj) : 0;
        }


        public bool Editar(SolicitudPedidos obj, out string Mensaje)
        {
            Mensaje = string.Empty;

            if (obj == null)
            {
                Mensaje = "El objeto de solicitud no puede ser nulo";
                return false;
            }

            // Validaciones de negocio para edición
            if (string.IsNullOrWhiteSpace(obj.Observaciones))
            {
                Mensaje = "Las observaciones no pueden estar vacías. ";
            }

            if (obj.CantidadEntregada < 0)
            {
                Mensaje += "La cantidad entregada no puede ser negativa. ";
            }


            return string.IsNullOrEmpty(Mensaje) ? objCapaDato.Editar(obj, out Mensaje) : false;
        }
    }

 }