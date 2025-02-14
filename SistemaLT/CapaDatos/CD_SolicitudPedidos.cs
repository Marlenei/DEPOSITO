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



        public int Registrar(SolicitudPedidos obj)
        {
            int idautogenerado = 0;

            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    SqlCommand cmd = new SqlCommand("T_RegistrarP", oconexion);
                    cmd.Parameters.AddWithValue("@IdProducto", obj.oProductos.IdProducto);
                    cmd.Parameters.AddWithValue("@CantidadPedida", obj.CantidadPedida);
                    cmd.Parameters.AddWithValue("@CantidadEntregada", (object)obj.CantidadEntregada ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("FechaPedido", Convert.ToDateTime(obj.FechaPedido));
                    cmd.Parameters.AddWithValue("FechaEntrega", Convert.ToDateTime(obj.FechaEntrega));
                    cmd.Parameters.AddWithValue("@IdUsuarioPedido", obj.IdUsuarioPedido);
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

        ////public async Task<UsuarioDatos> ObtenerDatosUsuario(string usuario, int codigoUnico)
        ////{
        ////    try
        ////    {
        ////        var response = await _httpClient.GetAsync($"http://10.4.51.49/SI_Apis/home/buscardatosdelusuario?usuario={HttpUtility.UrlEncode(usuario)}&codigounico={codigoUnico}");

        ////        if (response.IsSuccessStatusCode)
        ////        {
        ////            var responseJson = await response.Content.ReadAsStringAsync();
        ////            return JsonConvert.DeserializeObject<UsuarioDatos>(responseJson);
        ////        }
        ////        else
        ////        {
        ////            throw new Exception("Error al obtener datos del usuario.");
        ////        }
        ////    }
        ////    catch (Exception ex)
        ////    {
        ////        throw new Exception($"Error en la llamada a la API: {ex.Message}", ex);
        ////    }
        //}
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
 
        public List<UsuarioDatos> ObtenerAreas() // Cambiar el nombre y retorno a List<Area>
        {
            List<UsuarioDatos> areas = new List<UsuarioDatos>();
            try
            {
                using (SqlConnection connection = new SqlConnection(Conexion.cn))
                {
                    // Usar el nombre del procedimiento almacenado
                    SqlCommand command = new SqlCommand("Mio_Listado_Areas", connection);
                    command.CommandType = CommandType.StoredProcedure; // Indicar que es un SP

                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            areas.Add(new UsuarioDatos() // Asignar propiedades según el SP
                            {
                                CodigoArea = reader.GetInt32(0),
                                NombreArea = reader.GetString(1),
                              
                               
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener áreas: {ex.Message}", ex);
            }
            return areas;
        }

        // Método para obtener sectores por código de área
        public List<UsuarioDatos> ObtenerSectoresPorArea(int codigoArea)
        {
            List<UsuarioDatos> sectores = new List<UsuarioDatos>();
            try
            {
                using (SqlConnection connection = new SqlConnection(Conexion.cn))
                {
                    // Usar el nombre del procedimiento almacenado
                    SqlCommand command = new SqlCommand("Mio_Listado_Sectores", connection);
                    command.CommandType = CommandType.StoredProcedure; // Indicar que es un SP
                    command.Parameters.AddWithValue("@CodigoArea", codigoArea);

                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            sectores.Add(new UsuarioDatos
                            {
                                CodigoSector = reader.GetInt32(0),
                                NombreSector = reader.GetString(1),
                                CodigoArea = codigoArea // Asignar el código de área desde el parámetro
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener sectores: {ex.Message}", ex);
            }
            return sectores;
        }


    }
}





