using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CapaDatos;
using CapaEntidad;

namespace CapaNegocio
{
    public class CN_EntregaPedidos
    {
        private CD_SolicitudPedidos objCapaDato = new CD_SolicitudPedidos();

      
        public bool ActualizarEntrega(SolicitudPedidos pedido, out string mensaje)
        {
            mensaje = string.Empty;
            try
            {
                var pedidoExistente = objCapaDato.ObtenerPedido(pedido.IdSolicitud);

                if (pedidoExistente == null)
                {
                    mensaje = "Pedido no encontrado";
                    return false;
                }

                if (pedidoExistente.Visado)
                {
                    mensaje = "No se puede modificar un pedido visado";
                    return false;
                }

                if ((DateTime.Now - pedidoExistente.FechaHoraActualizacion).TotalHours > 24)
                {
                    mensaje = "El periodo de modificación ha expirado (24 horas)";
                    return false;
                }

                return objCapaDato.ActualizarEntrega(pedido,  out mensaje);
            }
            catch (Exception ex)
            {
                mensaje = ex.Message;
                return false;
            }
        }
    }
}
