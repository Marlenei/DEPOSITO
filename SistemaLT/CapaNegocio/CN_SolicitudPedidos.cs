using CapaDatos;
using CapaEntidad;
using System;
using System.Collections.Generic;
using System.Net.Http;
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

        // Modificado para incluir el número de pedido generado
        public int Registrar(SolicitudPedidos obj, out string Mensaje, out string NroPedidoGenerado)
        {

            Mensaje = string.Empty;
            NroPedidoGenerado = string.Empty;

            try
            {
                // Validaciones de negocio
                if (obj == null)
                {
                    Mensaje = "El objeto de solicitud no puede ser nulo.";
                    return 0;
                }

                if (obj.CantidadPedida <= 0)
                    Mensaje += "La cantidad pedida debe ser mayor a 0. ";

                if (obj.oProductos?.IdProducto == 0)
                    Mensaje += "Seleccionar un producto. ";

                if (obj.CantidadEntregada > obj.CantidadPedida)
                    Mensaje += "La cantidad entregada no puede ser mayor a la pedida. ";

                if (!string.IsNullOrEmpty(Mensaje))
                    return 0;

                // Llamada a capa de datos con nuevo parámetro
                int idGenerado = objCapaDato.Registrar(obj, out NroPedidoGenerado);

                if (string.IsNullOrEmpty(NroPedidoGenerado))
                    throw new Exception("No se pudo generar el número de pedido");

                return idGenerado;
            }
            catch (Exception ex)
            {
                Mensaje = $"Error al registrar pedido: {ex.Message}";
                return 0;
            }
        }

        public bool Editar(SolicitudPedidos obj, out string Mensaje)
        {
            Mensaje = string.Empty;

            try
            {
                if (obj == null)
                {
                    Mensaje = "El objeto de solicitud no puede ser nulo.";
                    return false;
                }

                if (string.IsNullOrWhiteSpace(obj.Observaciones))
                    Mensaje += "Las observaciones no pueden estar vacías. ";

                if (obj.CantidadEntregada < 0)
                    Mensaje += "La cantidad entregada no puede ser negativa. ";

                if (!string.IsNullOrEmpty(Mensaje))
                    return false;

                return objCapaDato.Editar(obj, out Mensaje);
            }
            catch (Exception ex)
            {
                Mensaje = $"Error al editar pedido: {ex.Message}";
                return false;
            }
        }

        public List<UsuarioDatos> ObtenerAreas()
        {
            return objCapaDato.ObtenerAreas();
        }

        public List<UsuarioDatos> ObtenerSectoresPorArea(int codigoArea)
        {
            return objCapaDato.ObtenerSectoresPorArea(codigoArea);
        }
    }
}