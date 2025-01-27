using CapaEntidad;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaDatos
{
    public class CD_SolicitudPedidos
    {
        public List<SolicitudPedidos> Listar()
        {
            List<SolicitudPedidos> lista = new List<SolicitudPedidos>();
            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("SELECT r.IdSolicitud, p.IdProducto, p.Detalle,");
                    sb.AppendLine("r.CantidadPedida, r.CantidadEntregada, r.FechaPedido,");
                    sb.AppendLine("r.FechaEntrega, r.IdUsuarioPedido, r.FechaHoraActualizacion,");
                    sb.AppendLine("r.CodigoArea, r.CodigoSector, r.IdUsuarioEntrega,");
                    sb.AppendLine("r.Observaciones, r.NroPedido, r.Visado, r.UsuarioVisado");
                    sb.AppendLine("FROM Tonner_Pedidos r");
                    sb.AppendLine("INNER JOIN Tonner_Productos p ON r.IdProducto = p.IdProducto");
                    sb.AppendLine("ORDER BY r.IdSolicitud DESC");

                    SqlCommand cmd = new SqlCommand(sb.ToString(), oconexion);
                    oconexion.Open();

                    using (SqlDataReader rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            lista.Add(new SolicitudPedidos()
                            {
                                IdSolicitud = Convert.ToInt32(rdr["IdSolicitud"]),
                                oProductos = new Productos()
                                {
                                    IdProducto = Convert.ToInt32(rdr["IdProducto"]),
                                    Detalle = rdr["Detalle"].ToString()
                                },
                                CantidadPedida = Convert.ToInt32(rdr["CantidadPedida"]),
                                CantidadEntregada = rdr["CantidadEntregada"] != DBNull.Value ? Convert.ToInt32(rdr["CantidadEntregada"]) : 0,
                                FechaPedido = Convert.ToDateTime(rdr["FechaPedido"]),
                                FechaEntrega = rdr["FechaEntrega"] != DBNull.Value ? Convert.ToDateTime(rdr["FechaEntrega"]) : DateTime.MinValue,
                                IdUsuarioPedido = Convert.ToInt32(rdr["IdUsuarioPedido"]),
                                //FechaHoraActualizacion = Convert.ToDateTime(rdr["FechaHoraActualizacion"]),
                                CodigoArea = Convert.ToInt32(rdr["CodigoArea"]),
                                CodigoSector = Convert.ToInt32(rdr["CodigoSector"]),
                                IdUsuarioEntrega = rdr["IdUsuarioEntrega"] != DBNull.Value ? Convert.ToInt32(rdr["IdUsuarioEntrega"]) : 0,
                                Observaciones = rdr["Observaciones"]?.ToString(),
                                NroPedido = rdr["NroPedido"]?.ToString(),
                                Visado = Convert.ToBoolean(rdr["Visado"]),
                                UsuarioVisado = rdr["UsuarioVisado"] != DBNull.Value ? Convert.ToInt32(rdr["UsuarioVisado"]) : 0
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al listar solicitudes: {ex.Message}", ex);
            }
            return lista;
        }

        public int Registrar(SolicitudPedidos obj)
        {
            int idautogenerado = 0;

            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    SqlCommand cmd = new SqlCommand("T_RegistrarPedidos", oconexion);
                    cmd.Parameters.AddWithValue("@IdProducto", obj.oProductos.IdProducto);
                    cmd.Parameters.AddWithValue("@CantidadPedida", obj.CantidadPedida);
                    cmd.Parameters.AddWithValue("@CantidadEntregada", (object)obj.CantidadEntregada ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@FechaPedido", obj.FechaPedido);
                    cmd.Parameters.AddWithValue("@FechaEntrega", (object)obj.FechaEntrega ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@IdUsuarioPedido", obj.IdUsuarioPedido);
                    //cmd.Parameters.AddWithValue("@FechaHoraActualizacion", DateTime.Now);
                    cmd.Parameters.AddWithValue("@CodigoArea", obj.CodigoArea);
                    cmd.Parameters.AddWithValue("@CodigoSector", obj.CodigoSector);
                    cmd.Parameters.AddWithValue("@IdUsuarioEntrega", (object)obj.IdUsuarioEntrega ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Observaciones", (object)obj.Observaciones ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@NroPedido", (object)obj.NroPedido ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Visado", obj.Visado);
                    cmd.Parameters.AddWithValue("@UsuarioVisado", (object)obj.UsuarioVisado ?? DBNull.Value);

                    SqlParameter paramResult = cmd.Parameters.Add("@Resultado", SqlDbType.Int);
                    paramResult.Direction = ParameterDirection.Output;

                    cmd.CommandType = CommandType.StoredProcedure;

                    oconexion.Open();
                    cmd.ExecuteNonQuery();

                    idautogenerado = Convert.ToInt32(paramResult.Value);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al registrar solicitud: {ex.Message}", ex);
            }
            return idautogenerado;
        }

        public bool Editar(SolicitudPedidos obj, out string Mensaje)
        {
            bool resultado = false;
            Mensaje = string.Empty;
            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    SqlCommand cmd = new SqlCommand("T_ModificarPedido", oconexion);
                    cmd.Parameters.AddWithValue("@IdSolicitud", obj.IdSolicitud);
                    cmd.Parameters.AddWithValue("@IdProducto", obj.oProductos.IdProducto);
                    cmd.Parameters.AddWithValue("@CantidadPedida", obj.CantidadPedida);
                    cmd.Parameters.AddWithValue("@CantidadEntregada", (object)obj.CantidadEntregada ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@FechaEntrega", (object)obj.FechaEntrega ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@IdUsuarioEntrega", (object)obj.IdUsuarioEntrega ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Observaciones", (object)obj.Observaciones ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Visado", obj.Visado);
                    cmd.Parameters.AddWithValue("@UsuarioVisado", (object)obj.UsuarioVisado ?? DBNull.Value);
                    //cmd.Parameters.AddWithValue("@FechaHoraActualizacion", DateTime.Now);

                    SqlParameter paramResult = cmd.Parameters.Add("@Resultado", SqlDbType.Bit);
                    paramResult.Direction = ParameterDirection.Output;

                    cmd.CommandType = CommandType.StoredProcedure;

                    oconexion.Open();
                    cmd.ExecuteNonQuery();

                    resultado = Convert.ToBoolean(paramResult.Value);
                }
            }
            catch (Exception ex)
            {
                resultado = false;
                Mensaje = ex.Message;
            }
            return resultado;
        }
    }
}
    

     
    

