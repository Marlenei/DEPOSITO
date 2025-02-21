using CapaEntidad;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Text;
using System.Web;
using System.Reflection;
using System.Security.Cryptography;
using System.Globalization;

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
                                FechaPedido = Convert.ToString(rdr["FechaPedido"]),
                                FechaEntrega = Convert.ToString(rdr["FechaEntrega"]),
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



        public int Registrar(SolicitudPedidos obj, out string NroPedidoGenerado)
        {
            int idautogenerado = 0;
            NroPedidoGenerado = string.Empty;

            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    oconexion.Open();

                    // Generar número de pedido
                    using (SqlCommand cmdComprobante = new SqlCommand("Act_ComprobanteDP", oconexion))
                    {
                        cmdComprobante.CommandType = CommandType.StoredProcedure;
                        SqlParameter paramNcom = new SqlParameter("@pNcom", SqlDbType.VarChar, 15);
                        paramNcom.Direction = ParameterDirection.Output;
                        cmdComprobante.Parameters.Add(paramNcom);
                        cmdComprobante.ExecuteNonQuery();
                        NroPedidoGenerado = paramNcom.Value.ToString();
                        obj.NroPedido = NroPedidoGenerado;
                    }

                    // Paso 2: Registrar el pedido
                    using (SqlCommand cmd = new SqlCommand("T_RegistrarP", oconexion))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@IdProducto", obj.oProductos.IdProducto);
                        cmd.Parameters.AddWithValue("@CantidadPedida", obj.CantidadPedida);
                        cmd.Parameters.AddWithValue("@CantidadEntregada", (object)obj.CantidadEntregada ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@FechaPedido", DateTime.ParseExact(
                            obj.FechaPedido,
                            "yyyy-MM-dd HH:mm:ss",
                            CultureInfo.InvariantCulture
                        ));

                        if (!string.IsNullOrEmpty(obj.FechaEntrega))
                        {
                            cmd.Parameters.AddWithValue("@FechaEntrega", DateTime.ParseExact(
                                obj.FechaEntrega,
                                "yyyy-MM-dd HH:mm:ss",
                                CultureInfo.InvariantCulture
                            ));
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@FechaEntrega", DBNull.Value);
                        }
                        cmd.Parameters.AddWithValue("@IdUsuarioPedido", obj.IdUsuarioPedido);
                        cmd.Parameters.AddWithValue("@CodigoArea", obj.CodigoArea);
                        cmd.Parameters.AddWithValue("@CodigoSector", obj.CodigoSector);
                        cmd.Parameters.AddWithValue("@IdUsuarioEntrega", (object)obj.IdUsuarioEntrega ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Observaciones", (object)obj.Observaciones ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@NroPedido", obj.NroPedido);
                        cmd.Parameters.AddWithValue("@Visado", obj.Visado);
                        cmd.Parameters.AddWithValue("@UsuarioVisado", (object)obj.UsuarioVisado ?? DBNull.Value);

                        SqlParameter paramResult = new SqlParameter("@Resultado", SqlDbType.Int);
                        paramResult.Direction = ParameterDirection.Output;
                        cmd.Parameters.Add(paramResult);

                        cmd.ExecuteNonQuery();
                        idautogenerado = Convert.ToInt32(paramResult.Value);
                    }
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

        // Método para obtener áreas

        public List<UsuarioDatos> ObtenerAreas()
        {
            List<UsuarioDatos> areas = new List<UsuarioDatos>();
            using (SqlConnection connection = new SqlConnection(Conexion.cn))
            {
                // Especifica la base de datos: [NombreBaseDatos].[dbo].[SP]
                SqlCommand cmd = new SqlCommand("[AHS].[dbo].[Mio_Listado_Areas]", connection);
                cmd.CommandType = CommandType.StoredProcedure;
                connection.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        areas.Add(new UsuarioDatos
                        {
                            CodigoArea = Convert.ToInt32(reader["CodigoArea"]),
                            NombreArea = reader["NombreArea"].ToString()
                        });
                    }
                }
            }
            return areas;
        }

        // Método para obtener sectores por código de área
        public List<UsuarioDatos> ObtenerSectoresPorArea(int codigoArea)
        {
            List<UsuarioDatos> sectores = new List<UsuarioDatos>();
            using (SqlConnection connection = new SqlConnection(Conexion.cn))
            {
                // 1. Especificar la base de datos externa: [AHS].[dbo].[Mio_Listado_Sectores]
                SqlCommand cmd = new SqlCommand("[AHS].[dbo].[Mio_Listado_Sectores]", connection);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@CodigoArea", codigoArea);

                connection.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        sectores.Add(new UsuarioDatos
                        {
                            // 2. Asegurar que los nombres de las columnas coincidan con el SP
                            CodigoSector = Convert.ToInt32(reader["CodigoSector"]),
                            NombreSector = reader["NombreSector"].ToString(), // Nombre exacto de la columna
                            CodigoArea = Convert.ToInt32(reader["CodigoArea"])
                        });
                    }
                }
            }
            return sectores;
        }


    }
}





