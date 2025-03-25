using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CapaEntidad;

namespace CapaDatos
{
    public class CD_EntregaPedidos
    {
       


        //public SolicitudPedidos ObtenerPedido(int idPedido)
        //{
        //    using (SqlConnection conexion = new SqlConnection(Conexion.cn))
        //    {
        //        string query = @"SELECT 
        //                p.IdSolicitud,
        //                p.IdProducto,
        //                p.CantidadEntregada,
        //                p.Visado,
        //                p.FechaHoraActualizacion,
        //                p.IdUsuarioPedido
        //                FROM Tonner_Pedidos p
        //                WHERE p.IdSolicitud = @IdSolicitud";

        //        SqlCommand cmd = new SqlCommand(query, conexion);
        //        cmd.Parameters.AddWithValue("@IdSolicitud", idPedido);

        //        conexion.Open();
        //        using (SqlDataReader reader = cmd.ExecuteReader())
        //        {
        //            if (reader.Read())
        //            {
        //                return new SolicitudPedidos()
        //                {
        //                    IdSolicitud = Convert.ToInt32(reader["IdSolicitud"]),
        //                    oProductos = new Productos
        //                    {
        //                        IdProducto = reader["IdProducto"] != DBNull.Value
        //                                    ? Convert.ToInt32(reader["IdProducto"])
        //                                    : 0
        //                    },
        //                    CantidadEntregada = reader["CantidadEntregada"] != DBNull.Value
        //                                      ? Convert.ToInt32(reader["CantidadEntregada"])
        //                                      : 0,
        //                    Visado = reader["Visado"] != DBNull.Value
        //                           ? Convert.ToBoolean(reader["Visado"])
        //                           : false,
        //                    FechaHoraActualizacion = reader["FechaHoraActualizacion"] != DBNull.Value
        //                                           ? Convert.ToDateTime(reader["FechaHoraActualizacion"])
        //                                           : DateTime.MinValue,
        //                    IdUsuarioPedido = reader["IdUsuarioPedido"] != DBNull.Value
        //                                    ? Convert.ToInt32(reader["IdUsuarioPedido"])
        //                                    : 0
        //                };
        //            }
        //        }
        //        return null;
        //    }
        //}

        //public bool ActualizarEntrega(SolicitudPedidos pedido, out string mensaje)
        //{
        //    mensaje = string.Empty;
        //    using (SqlConnection conexion = new SqlConnection(Conexion.cn))
        //    {
        //        conexion.Open();
        //        SqlTransaction transaction = conexion.BeginTransaction();

        //        try
        //        {
        //            string query = @"UPDATE Tonner_Pedidos SET 
        //                    CantidadEntregada = @CantidadEntregada,
        //                    FechaEntrega = @FechaEntrega,
        //                    FechaHoraActualizacion = GETDATE(),
        //                    IdUsuarioEntrega = @IdUsuarioEntrega
        //                    WHERE IdSolicitud = @IdSolicitud AND Visado = 0";

        //            using (SqlCommand cmd = new SqlCommand(query, conexion, transaction))
        //            {
        //                cmd.Parameters.AddWithValue("@CantidadEntregada", pedido.CantidadEntregada);
        //                cmd.Parameters.AddWithValue("@FechaEntrega", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
        //                cmd.Parameters.AddWithValue("@IdUsuarioEntrega", pedido.IdUsuarioEntrega);
        //                cmd.Parameters.AddWithValue("@IdSolicitud", pedido.IdSolicitud);

        //                int affected = cmd.ExecuteNonQuery();
        //                if (affected == 0)
        //                {
        //                    transaction.Rollback();
        //                    mensaje = "No se puede modificar un pedido visado";
        //                    return false;
        //                }
        //            }

        //            transaction.Commit();
        //            return true;
        //        }
        //        catch (Exception ex)
        //        {
        //            transaction.Rollback();
        //            mensaje = ex.Message;
        //            return false;
        //        }
        //    }
        //}


    }
}
