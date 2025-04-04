using CapaDatos;
using CapaEntidad;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace CapaNegocio
{
    public class CN_SolicitudPedidos
    {
        private CD_SolicitudPedidos objCapaDato = new CD_SolicitudPedidos();


        public List<SolicitudPedidos> Listar(int idUsuario)
        {
            return objCapaDato.Listar(idUsuario);
        }



        public List<SolicitudPedidos> ObtenerPedidos(int? codArea, int? codSector, string nroPedido, DateTime fechaInicio, DateTime fechaFin, bool soloPendientes)
        {
            CD_SolicitudPedidos cdSolicitudPedidos = new CD_SolicitudPedidos();
            return cdSolicitudPedidos.ListarFiltrados(codArea, codSector, nroPedido, fechaInicio, fechaFin, soloPendientes);
        }

        public List<SolicitudPedidos> ListarFiltradosNro(
      // Parámetro requerido primero
      string nroPedido = null,           // Parámetro opcional
      bool soloPendientes = false)       // Parámetro opcional
        {
            return objCapaDato.ListarFiltradosNro(

                nroPedido: nroPedido,
                soloPendientes: soloPendientes
            );
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
        public SolicitudPedidos ObtenerPedido(int idPedido)
        {
            try
            {
                return objCapaDato.ObtenerPedido(idPedido);
            }
            catch (Exception ex)
            {
                // Manejar excepciones o registrar errores
                throw new Exception($"Error al obtener pedido: {ex.Message}", ex);
            }
        }

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

                DateTime fechaPedido;
                if (!DateTime.TryParseExact(pedidoExistente.FechaPedido, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out fechaPedido))
                {
                    mensaje = "Formato de fecha no válido en el pedido.";
                    return false;
                }

                if ((DateTime.Now - fechaPedido).TotalHours > 24)
                {
                    mensaje = "El periodo de modificación ha expirado (24 horas)";
                    return false;
                }

                // Actualizar la fecha de la última modificación
                pedidoExistente.FechaHoraActualizacion = DateTime.Now;

                // Llamar a la capa de datos para realizar la actualización
                return objCapaDato.ActualizarEntrega(pedido, out mensaje);
            }
            catch (Exception ex)
            {
                mensaje = ex.Message;
                return false;
            }
        }

        public bool Actualizar(SolicitudPedidos pedido, out string mensaje)
        {
            return objCapaDato.Actualizar(pedido, out mensaje);
        }

        public bool RegistrarVisado(int idPedido, int idUsuario, out string mensaje)
        {
            try
            {
                var pedido = objCapaDato.ObtenerPedido(idPedido);

                if (pedido == null)
                {
                    mensaje = "Pedido no encontrado";
                    return false;
                }

                if (pedido.IdUsuarioPedido != idUsuario)
                {
                    mensaje = "Solo el solicitante puede visar el pedido";
                    return false;
                }

                if (pedido.CantidadEntregada <= 0)
                {
                    mensaje = "No se puede visar con cantidad entregada igual a cero";
                    return false;
                }

                return objCapaDato.RegistrarVisado(idPedido, idUsuario, out mensaje);
            }
            catch (Exception ex)
            {
                mensaje = ex.Message;
                return false;
            }
        }

        public bool Editar(SolicitudPedidos obj, out string Mensaje)
        {
            Mensaje = string.Empty;

            try
            {
                var pedidoExistente = objCapaDato.ObtenerPedido(obj.IdSolicitud);

                if (pedidoExistente.Visado)
                {
                    Mensaje = "No se puede editar un pedido visado";
                    return false;
                }

                // Resto de validaciones...
                return objCapaDato.Editar(obj, out Mensaje);
            }
            catch (Exception ex)
            {
                Mensaje = $"Error al editar pedido: {ex.Message}";
                return false;
            }
        }



        public string ObtenerNombreArea(int codArea)
        {
            using (SqlConnection connection = new SqlConnection(Conexion.cn))
            {
                SqlCommand cmd = new SqlCommand("[AHS].[dbo].[Mio_Listado_Areas]", connection);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@CodigoArea", codArea);

                connection.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return reader["NombreArea"].ToString();
                    }
                }
                return "Área no definida";
            }
        }

        public string ObtenerNombreSector(int codArea, int codSector)
        {
            using (SqlConnection connection = new SqlConnection(Conexion.cn))
            {
                SqlCommand cmd = new SqlCommand("[AHS].[dbo].[Mio_Listado_Sectores]", connection);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@CodigoArea", codArea); // Solo este parámetro

                connection.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (Convert.ToInt32(reader["CodigoSector"]) == codSector)
                        {
                            return reader["NombreSector"].ToString();
                        }
                    }
                }
            }
            return "Sector no definido";
        }

       



    }
}