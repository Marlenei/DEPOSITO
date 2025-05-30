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
using Microsoft.Win32;
using System.Collections;

namespace CapaDatos
{
    public class CD_SolicitudPedidos
    {
        // En CapaDatos/CD_SolicitudPedidos.cs
        public List<SolicitudPedidos> ListarFiltrados(int? codArea, int? codSector, string nroPedido, DateTime? fechaInicio, DateTime? fechaFin, bool soloPendientes)
        {
            List<SolicitudPedidos> lista = new List<SolicitudPedidos>();

            using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
            {
                SqlCommand cmd = new SqlCommand("usp_ObtenerPedidosFiltrados", oconexion);
                cmd.CommandType = CommandType.StoredProcedure;

                // ... (tus parámetros permanecen iguales) ...
                cmd.Parameters.AddWithValue("@CodArea", codArea.HasValue ? (object)codArea.Value : DBNull.Value);
                cmd.Parameters.AddWithValue("@CodSector", (codSector.HasValue && codSector.Value == 0) ? DBNull.Value : (codSector.HasValue ? (object)codSector.Value : DBNull.Value));
                cmd.Parameters.AddWithValue("@NroPedido", string.IsNullOrEmpty(nroPedido) ? (object)DBNull.Value : nroPedido);
                cmd.Parameters.AddWithValue("@SoloPendientes", soloPendientes);
                cmd.Parameters.AddWithValue("@FechaInicio", fechaInicio.HasValue ? (object)fechaInicio.Value : DBNull.Value);
                cmd.Parameters.AddWithValue("@FechaFin", fechaFin.HasValue ? (object)fechaFin.Value : DBNull.Value);

                oconexion.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        lista.Add(new SolicitudPedidos()
                        {
                            IdSolicitud = Convert.ToInt32(reader["IdSolicitud"]),
                            oProductos = new Productos()
                            {
                                IdProducto = Convert.ToInt32(reader["IdProducto"]),
                                Detalle = reader["Producto"].ToString(),
                                StockActual = Convert.ToInt32(reader["StockActual"])
                            },
                            CantidadPedida = Convert.ToInt32(reader["CantidadPedida"]),
                            CantidadEntregada = reader["CantidadEntregada"] != DBNull.Value ? Convert.ToInt32(reader["CantidadEntregada"]) : 0,
                            FechaEntrega = reader["FechaEntrega"] != DBNull.Value ? Convert.ToDateTime(reader["FechaEntrega"]).ToString("yyyy-MM-dd HH:mm:ss") : null,
                            NroPedido = reader["NroPedido"].ToString(),
                            Visado = Convert.ToBoolean(reader["Visado"]),
                            FechaPedido = Convert.ToDateTime(reader["FechaPedido"]).ToString("yyyy-MM-dd HH:mm:ss"),
                            CodigoArea = Convert.ToInt32(reader["CodigoArea"]),
                            CodigoSector = Convert.ToInt32(reader["CodigoSector"]),
                            NombreArea = reader["NombreArea"].ToString(),
                            NombreSector = reader["NombreSector"].ToString(),
                            // --- MAPEANDO LOS NUEVOS CAMPOS ---
                            NombreUsuarioPedido = reader["NombreUsuarioPedido"] != DBNull.Value ? reader["NombreUsuarioPedido"].ToString() : "N/A", // O string.Empty
                            IdUsuarioPedido = Convert.ToInt32(reader["IdUsuarioPedido"]), // Asegúrate que el tipo coincida con tu modelo
                        });
                    }
                }
            }
            return lista;
        }

        public List<SolicitudPedidos> Listar(int idUsuario)
        {
            List<SolicitudPedidos> lista = new List<SolicitudPedidos>();
            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    string query = @"SELECT p.IdSolicitud, 
                            prod.IdProducto,
                            prod.Detalle AS Producto,
                            p.CantidadPedida,
                            p.CantidadEntregada,
                            p.FechaPedido, 
                            p.FechaEntrega,
                            p.NroPedido,
                            p.Visado
                     FROM Tonner_Pedidos p
                     INNER JOIN Tonner_Productos prod ON prod.IdProducto = p.IdProducto
                     WHERE p.IdUsuarioPedido = @IdUsuario
                     ORDER BY p.FechaPedido DESC";

                    SqlCommand cmd = new SqlCommand(query, oconexion);
                    cmd.Parameters.AddWithValue("@IdUsuario", idUsuario);

                    oconexion.Open();

                    using (SqlDataReader rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            lista.Add(new SolicitudPedidos()
                            {
                                IdSolicitud = Convert.ToInt32(rdr["IdSolicitud"]),
                                oProductos = new Productos
                                {
                                    IdProducto = Convert.ToInt32(rdr["IdProducto"]), 
                                    Detalle = rdr["Producto"].ToString()
                                },
                                CantidadPedida = Convert.ToInt32(rdr["CantidadPedida"]),
                                CantidadEntregada = rdr["CantidadEntregada"] != DBNull.Value
                                                    ? Convert.ToInt32(rdr["CantidadEntregada"])
                                                    : 0,
                                FechaPedido = rdr["FechaPedido"] != DBNull.Value
                        ? Convert.ToDateTime(rdr["FechaPedido"]).ToString("yyyy-MM-dd HH:mm:ss")
                        : string.Empty,
                                FechaEntrega = rdr["FechaEntrega"] != DBNull.Value ? Convert.ToDateTime(rdr["FechaEntrega"]).ToString("yyyy-MM-dd HH:mm:ss") : null,
                                NroPedido = rdr["NroPedido"] != DBNull.Value
                                            ? rdr["NroPedido"].ToString()
                                            : string.Empty,
                                Visado = rdr["Visado"] != DBNull.Value
                                         && Convert.ToBoolean(rdr["Visado"])
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener pedidos: " + ex.Message);
            }
            return lista;
        }

        public List<SolicitudPedidos> ListarFiltradosNro(string nroPedido = null,bool soloPendientes = false)
        {
            List<SolicitudPedidos> lista = new List<SolicitudPedidos>();

            using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
            {
                SqlCommand cmd = new SqlCommand("usp_ObtenerPedidosFiltradosNROPEDIDO", oconexion);
                cmd.CommandType = CommandType.StoredProcedure;

                // Manejo de parámetros
                cmd.Parameters.AddWithValue("@NroPedido", string.IsNullOrEmpty(nroPedido) ? (object)DBNull.Value : nroPedido);
                cmd.Parameters.AddWithValue("@SoloPendientes", soloPendientes);

                oconexion.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        lista.Add(new SolicitudPedidos()
                        {
                            IdSolicitud = Convert.ToInt32(reader["IdSolicitud"]),
                            oProductos = new Productos()
                            {
                                IdProducto = Convert.ToInt32(reader["IdProducto"]),
                                Detalle = reader["Producto"].ToString(),
                                StockActual = Convert.ToInt32(reader["StockActual"])
                            },
                            CantidadPedida = Convert.ToInt32(reader["CantidadPedida"]),
                            CantidadEntregada = reader["CantidadEntregada"] != DBNull.Value ? Convert.ToInt32(reader["CantidadEntregada"]) : 0,
                            FechaEntrega = reader["FechaEntrega"] != DBNull.Value ? Convert.ToDateTime(reader["FechaEntrega"]).ToString("yyyy-MM-dd HH:mm:ss") : null,
                            NroPedido = reader["NroPedido"].ToString(),
                            Visado = Convert.ToBoolean(reader["Visado"]),
                            FechaPedido = Convert.ToDateTime(reader["FechaPedido"]).ToString("yyyy-MM-dd HH:mm:ss"),
                            CodigoArea = Convert.ToInt32(reader["CodigoArea"]),
                            CodigoSector = Convert.ToInt32(reader["CodigoSector"]),
                            NombreArea = reader["NombreArea"].ToString(), // Asegurar que existe en el SP
                            NombreSector = reader["NombreSector"].ToString() // Asegurar que existe en el SP
                        });
                    }
                }
            }
            return lista;
        }



        public int Registrar(SolicitudPedidos obj, out string NroPedidoGenerado, List<SolicitudPedidos> listaProductos)
        {
            int idautogenerado = 0;
            NroPedidoGenerado = string.Empty;

            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    oconexion.Open();
                    SqlTransaction transaction = oconexion.BeginTransaction();

                    try
                    {
                        // 1. Generar NroPedido
                        using (SqlCommand cmdComprobante = new SqlCommand("Act_ComprobanteDP", oconexion, transaction))
                        {
                            cmdComprobante.CommandType = CommandType.StoredProcedure;
                            SqlParameter paramNcom = new SqlParameter("@pNcom", SqlDbType.VarChar, 15)
                            {
                                Direction = ParameterDirection.Output
                            };
                            cmdComprobante.Parameters.Add(paramNcom);
                            cmdComprobante.ExecuteNonQuery();
                            NroPedidoGenerado = paramNcom.Value.ToString();
                            obj.NroPedido = NroPedidoGenerado;
                        }

                        // 2. Registrar Pedido
                        using (SqlCommand cmd = new SqlCommand("T_RegistrarP", oconexion, transaction))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;

                            // Definir parámetros una sola vez
                            cmd.Parameters.Add("@IdProducto", SqlDbType.Int);
                            cmd.Parameters.Add("@CantidadPedida", SqlDbType.Int);
                            cmd.Parameters.Add("@CantidadEntregada", SqlDbType.Int);
                            cmd.Parameters.Add("@FechaPedido", SqlDbType.DateTime);
                            cmd.Parameters.Add("@FechaEntrega", SqlDbType.DateTime);
                            cmd.Parameters.Add("@IdUsuarioPedido", SqlDbType.Int);
                            cmd.Parameters.Add("@CodigoArea", SqlDbType.Int);
                            cmd.Parameters.Add("@CodigoSector", SqlDbType.Int);
                            cmd.Parameters.Add("@IdUsuarioEntrega", SqlDbType.Int);
                            cmd.Parameters.Add("@Observaciones", SqlDbType.VarChar, 250);
                            cmd.Parameters.Add("@NroPedido", SqlDbType.VarChar, 15);
                            cmd.Parameters.Add("@Visado", SqlDbType.Bit);
                            cmd.Parameters.Add("@UsuarioVisado", SqlDbType.Int);
                            SqlParameter resultado = cmd.Parameters.Add("@Resultado", SqlDbType.Int);
                            resultado.Direction = ParameterDirection.Output;

                            foreach (var producto in listaProductos)
                            {
                                // Establecer valores para el producto actual
                                cmd.Parameters["@IdProducto"].Value = producto.oProductos.IdProducto;
                                cmd.Parameters["@CantidadPedida"].Value = producto.CantidadPedida;
                                cmd.Parameters["@CantidadEntregada"].Value = producto.CantidadEntregada.HasValue ? (object)producto.CantidadEntregada.Value : DBNull.Value;

                                // Usar las propiedades del objeto principal para los datos del pedido
                                DateTime fechaPedido = DateTime.ParseExact(obj.FechaPedido, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
                                cmd.Parameters["@FechaPedido"].Value = fechaPedido;
                                cmd.Parameters["@IdUsuarioPedido"].Value = obj.IdUsuarioPedido;
                                cmd.Parameters["@CodigoArea"].Value = obj.CodigoArea;
                                cmd.Parameters["@CodigoSector"].Value = obj.CodigoSector;
                                cmd.Parameters["@IdUsuarioEntrega"].Value = obj.IdUsuarioEntrega ?? (object)DBNull.Value;
                                cmd.Parameters["@Observaciones"].Value = producto.Observaciones ?? (object)DBNull.Value;
                                cmd.Parameters["@NroPedido"].Value = obj.NroPedido;
                                cmd.Parameters["@Visado"].Value = obj.Visado;
                                cmd.Parameters["@UsuarioVisado"].Value = obj.UsuarioVisado ?? (object)DBNull.Value;

                                // Manejar FechaEntrega como NULL si es necesario
                                if (obj.FechaEntrega == null)
                                {
                                    cmd.Parameters["@FechaEntrega"].Value = DBNull.Value;
                                }
                                else
                                {
                                    cmd.Parameters["@FechaEntrega"].Value = DateTime.ParseExact(obj.FechaEntrega, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
                                }

                                cmd.ExecuteNonQuery();
                            }

                            idautogenerado = Convert.ToInt32(resultado.Value);
                        }

                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al registrar el pedido: " + ex.Message);
            }

            return idautogenerado;
        }




        public SolicitudPedidos ObtenerPedido(int idPedido)
        {
            using (SqlConnection conexion = new SqlConnection(Conexion.cn))
            {
                string query = @"SELECT 
            p.IdSolicitud,
            p.IdProducto,
            p.CantidadPedida,
            p.CantidadEntregada,
            p.FechaPedido,
            p.FechaEntrega,
            p.NroPedido,
            p.Visado,
            p.CodigoArea,
            p.CodigoSector,
            p.IdUsuarioPedido,
            p.IdUsuarioEntrega,
            p.Observaciones,
            pr.Detalle AS Producto,
            a.NombreArea,
            s.Nombre AS NombreSector
        FROM Tonner_Pedidos p
        INNER JOIN Tonner_Productos pr ON pr.IdProducto = p.IdProducto 
        INNER JOIN AHS.dbo.Mio_Areas a ON a.CodigoArea = p.CodigoArea
        INNER JOIN AHS.dbo.Mio_Sectores s ON s.CodigoSector = p.CodigoSector
        WHERE p.IdSolicitud = @IdSolicitud";

                SqlCommand cmd = new SqlCommand(query, conexion);
                cmd.Parameters.AddWithValue("@IdSolicitud", idPedido);

                try
                {
                    conexion.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new SolicitudPedidos()
                            {
                                IdSolicitud = Convert.ToInt32(reader["IdSolicitud"]),
                                oProductos = new Productos
                                {
                                    IdProducto = Convert.ToInt32(reader["IdProducto"]),
                                    Detalle = reader["Producto"].ToString(),
                                },
                                CantidadPedida = Convert.ToInt32(reader["CantidadPedida"]),
                                CantidadEntregada = reader["CantidadEntregada"] != DBNull.Value
                                    ? Convert.ToInt32(reader["CantidadEntregada"])
                                    : 0,
                                FechaPedido = reader["FechaPedido"] != DBNull.Value
                                ? Convert.ToDateTime(reader["FechaPedido"]).ToString("yyyy-MM-dd HH:mm:ss")
                                : string.Empty,
                                                            FechaEntrega = reader["FechaEntrega"] != DBNull.Value
                                ? Convert.ToDateTime(reader["FechaEntrega"]).ToString("yyyy-MM-dd HH:mm:ss")
                                : string.Empty,
                                NroPedido = reader["NroPedido"].ToString(),
                                Visado = reader["Visado"] != DBNull.Value
                                    ? Convert.ToBoolean(reader["Visado"])
                                    : false,
                                CodigoArea = Convert.ToInt32(reader["CodigoArea"]),
                                CodigoSector = Convert.ToInt32(reader["CodigoSector"]),
                                IdUsuarioPedido = reader["IdUsuarioPedido"] != DBNull.Value
                                    ? Convert.ToInt32(reader["IdUsuarioPedido"])
                                    : 0,
                                IdUsuarioEntrega = reader["IdUsuarioEntrega"] != DBNull.Value
                                    ? Convert.ToInt32(reader["IdUsuarioEntrega"])
                                    : 0,
                                Observaciones = reader["Observaciones"] != DBNull.Value
                                    ? reader["Observaciones"].ToString()
                                    : string.Empty,
                                NombreArea = reader["NombreArea"].ToString(),
                                NombreSector = reader["NombreSector"].ToString()
                            };
                        }
                        else
                        {
                            // No se encontró el pedido
                            return null;
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Manejo de errores
                    throw new Exception("Error al obtener el pedido: " + ex.Message);
                }
            }
        }

        public bool ActualizarEntrega(SolicitudPedidos pedido, out string mensaje)
        {
            mensaje = string.Empty;
            using (SqlConnection conexion = new SqlConnection(Conexion.cn))
            {
                conexion.Open();
                SqlTransaction transaction = conexion.BeginTransaction();

                try
                {
                    int cantidadAnterior = 0;
                    int idProducto = 0;
                    string nroPedido = "";
                    string observaciones = null;
                    int cantidadPedida = 0; 
                    bool? visado = null;    

                    
                    string querySelect = @"
             SELECT
                CantidadEntregada,
                IdProducto,
                NroPedido,
                Observaciones,
                CantidadPedida, 
                Visado          
             FROM Tonner_Pedidos
             WHERE IdSolicitud = @IdSolicitud";

                    using (SqlCommand cmdSelect = new SqlCommand(querySelect, conexion, transaction))
                    {
                        cmdSelect.Parameters.AddWithValue("@IdSolicitud", pedido.IdSolicitud);

                        using (SqlDataReader reader = cmdSelect.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                cantidadAnterior = reader["CantidadEntregada"] != DBNull.Value ? Convert.ToInt32(reader["CantidadEntregada"]) : 0;
                                idProducto = Convert.ToInt32(reader["IdProducto"]); // Assuming IdProducto is never null
                                nroPedido = reader["NroPedido"] != DBNull.Value ? reader["NroPedido"].ToString() : "";
                                observaciones = reader["Observaciones"] != DBNull.Value ? reader["Observaciones"].ToString() : null;
                                cantidadPedida = Convert.ToInt32(reader["CantidadPedida"]); // Added
                                var visadoObj = reader["Visado"]; // Added
                                visado = visadoObj != DBNull.Value ? (bool?)Convert.ToBoolean(visadoObj) : null; 

                            }
                            else
                            {
                                
                                mensaje = "Pedido no encontrado";
                            
                                return false; 
                            }
                        } 
                    }

                 
                    if (visado == true)
                    {
                        
                        mensaje = "No se puede modificar un pedido visado.";
                        return false;
                    }

                    
                    if (pedido.CantidadEntregada < 0)
                    {
                        
                        mensaje = "La cantidad entregada no puede ser negativa.";
                        return false;
                    }
                    if (pedido.CantidadEntregada > cantidadPedida)
                    {
                       
                        mensaje = "La cantidad entregada no puede superar la cantidad pedida (" + cantidadPedida + ").";
                        return false;
                    }


               
                    int diferencia = (int)(pedido.CantidadEntregada - cantidadAnterior);

                 
                    if (diferencia == 0)
                    {
                      
                        mensaje = "No se realizaron cambios en la cantidad entregada.";
                        return true;
                    }

                    
                    if (diferencia > 0)
                    {
                        int stockActual = GetCurrentStock(idProducto, conexion, transaction); 

                        if (stockActual < diferencia)
                        {
                            transaction.Rollback(); 
                            mensaje = $"Stock insuficiente para entregar {diferencia} unidad(es) más. Stock actual: {stockActual}.";
                            return false;
                        }
                    }

                    
                    string queryUpdate = @"UPDATE Tonner_Pedidos SET
                 CantidadEntregada = @CantidadEntregada,
                 FechaEntrega = @FechaEntrega,
                 FechaHoraActualizacion = GETDATE(),
                 IdUsuarioEntrega = @IdUsuarioEntrega
                 WHERE IdSolicitud = @IdSolicitud AND (Visado = 0 OR Visado IS NULL)"; 

                    using (SqlCommand cmdUpdate = new SqlCommand(queryUpdate, conexion, transaction))
                    {
                        
                        cmdUpdate.Parameters.AddWithValue("@CantidadEntregada", pedido.CantidadEntregada);
                        cmdUpdate.Parameters.AddWithValue("@FechaEntrega", (pedido.CantidadEntregada > 0) ? (object)DateTime.Now : DBNull.Value); 
                        cmdUpdate.Parameters.AddWithValue("@IdUsuarioEntrega", pedido.IdUsuarioEntrega); 
                        cmdUpdate.Parameters.AddWithValue("@IdSolicitud", pedido.IdSolicitud);

                        int affected = cmdUpdate.ExecuteNonQuery();
                        if (affected == 0)
                        {
                          
                            transaction.Rollback();
                            mensaje = "No se pudo actualizar el pedido. Puede que haya sido visado o ya no exista.";
                            return false;
                        }
                    }

                  
                    int stockParaEgreso = GetCurrentStock(idProducto, conexion, transaction);

                    string insertEgreso = @"
             INSERT INTO Tonner_Egresos (
                 IdProducto, CodigoId, Cantidad, FechaEgreso,
                 Observaciones, IdUsuario, StockActual, FechayHoraAct, CodigoArea, CodigoSector,
                 TipoSalida
             ) VALUES (
                 @IdProducto, @CodigoId, @Cantidad, @FechaEgreso,
                 @Observaciones, @IdUsuario, @StockActual, GETDATE(), @CodigoArea, @CodigoSector,
                 'P' -- Tipo Salida Pedido
             )";

                    using (SqlCommand cmdEgreso = new SqlCommand(insertEgreso, conexion, transaction))
                    {
                        cmdEgreso.Parameters.AddWithValue("@IdProducto", idProducto);
                        cmdEgreso.Parameters.AddWithValue("@CodigoId", nroPedido); 
                        cmdEgreso.Parameters.AddWithValue("@Cantidad", Math.Abs(diferencia)); 
                        cmdEgreso.Parameters.AddWithValue("@FechaEgreso", DateTime.Now);
                       
                        cmdEgreso.Parameters.AddWithValue("@Observaciones", observaciones ?? (object)DBNull.Value);
                        cmdEgreso.Parameters.AddWithValue("@IdUsuario", pedido.IdUsuarioEntrega);
                        cmdEgreso.Parameters.AddWithValue("@StockActual", stockParaEgreso);
                        cmdEgreso.Parameters.AddWithValue("@CodigoArea", pedido.CodigoArea); 
                        cmdEgreso.Parameters.AddWithValue("@CodigoSector", pedido.CodigoSector); 

                        cmdEgreso.ExecuteNonQuery();
                    }

                  
                    string updateStock = @"
             UPDATE Tonner_Productos
             SET StockActual = StockActual - @Diferencia -- Subtract difference (negative diff means adding)
             WHERE IdProducto = @IdProducto";
                   

                    using (SqlCommand cmdStock = new SqlCommand(updateStock, conexion, transaction))
                    {
                        cmdStock.Parameters.AddWithValue("@Diferencia", diferencia); 
                                                                                     
                        cmdStock.Parameters.AddWithValue("@IdProducto", idProducto);

                        int stockAffected = cmdStock.ExecuteNonQuery();
                        if (stockAffected == 0)
                        {
                            
                            throw new Exception("Error: No se pudo actualizar el stock del producto (ID: " + idProducto + ").");
                        }
                    }


                    
                    transaction.Commit();
                    mensaje = "Entrega actualizada y stock modificado correctamente."; 
                    return true;
                }
                catch (Exception ex)
                {
                   
                    try { transaction?.Rollback(); } catch { /* Log rollback failure */ }
                    mensaje = "Error al actualizar entrega: " + ex.Message; 
                                                                            
                    return false;
                }
                
            } 
        }

       
        private int GetCurrentStock(int idProducto, SqlConnection conexion, SqlTransaction transaction)
        {
            string queryStock = "SELECT StockActual FROM Tonner_Productos WHERE IdProducto = @IdProducto";
            // *** Reemplaza Tonner_Productos, StockActual, IdProducto si los nombres son diferentes ***
            using (SqlCommand cmdStock = new SqlCommand(queryStock, conexion, transaction))
            {
                cmdStock.Parameters.AddWithValue("@IdProducto", idProducto);
                object result = cmdStock.ExecuteScalar();
                if (result != null && result != DBNull.Value)
                {
                    return Convert.ToInt32(result);
                }
                else
                {
                    
                    throw new Exception($"No se pudo obtener el stock para el producto ID: {idProducto}");
                }
            }
        }

        public bool Actualizar(SolicitudPedidos obj, out string mensaje)
        {
            mensaje = string.Empty;
            using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
            {
                try
                {
                    oconexion.Open();
                    SqlTransaction transaction = oconexion.BeginTransaction();

                    // 1. Actualizar el pedido
                    string query = @"
                UPDATE Tonner_Pedidos 
                SET 
                    IdProducto = @IdProducto,
                    CantidadPedida = @CantidadPedida,
                    CantidadEntregada = @CantidadEntregada,
                    FechaEntrega = @FechaEntrega,
                    Observaciones = @Observaciones,
                    Visado = @Visado,
                    CodigoArea = @CodigoArea,
                    CodigoSector = @CodigoSector
                WHERE 
                    IdSolicitud = @IdSolicitud";

                    using (SqlCommand cmd = new SqlCommand(query, oconexion, transaction))
                    {
                        cmd.Parameters.AddWithValue("@IdSolicitud", obj.IdSolicitud);
                        cmd.Parameters.AddWithValue("@IdProducto", obj.oProductos.IdProducto);
                        cmd.Parameters.AddWithValue("@CantidadPedida", obj.CantidadPedida);
                        cmd.Parameters.AddWithValue("@CantidadEntregada", (object)obj.CantidadEntregada ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@FechaEntrega", (object)obj.FechaEntrega ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Observaciones", (object)obj.Observaciones ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Visado", obj.Visado);
                        cmd.Parameters.AddWithValue("@CodigoArea", obj.CodigoArea);
                        cmd.Parameters.AddWithValue("@CodigoSector", obj.CodigoSector);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected == 0)
                        {
                            transaction.Rollback();
                            mensaje = "No se encontró el pedido para actualizar.";
                            return false;
                        }
                    }

                    // 2. Commit de la transacción
                    transaction.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    mensaje = ex.Message;
                    return false;
                }
            }
        }
        public bool RegistrarVisado(int idPedido, int idUsuario, out string mensaje)
        {
            mensaje = string.Empty;
            using (SqlConnection conexion = new SqlConnection(Conexion.cn))
            {
                // Consulta corregida con sintaxis válida
                string query = @"UPDATE Tonner_Pedidos 
                        SET 
                            Visado = 1,
                            UsuarioVisado = @UsuarioVisado
                        WHERE 
                            IdSolicitud = @IdSolicitud
                            AND CantidadEntregada > 0";

                using (SqlCommand cmd = new SqlCommand(query, conexion))
                {
                    cmd.Parameters.AddWithValue("@UsuarioVisado", idUsuario);
                    cmd.Parameters.AddWithValue("@IdSolicitud", idPedido);

                    try
                    {
                        conexion.Open();
                        int affectedRows = cmd.ExecuteNonQuery();

                        if (affectedRows == 0)
                        {
                            mensaje = "No se cumplen las condiciones para visar";
                            return false;
                        }
                        return true;
                    }
                    catch (SqlException ex)
                    {
                        mensaje = $"Error SQL: {ex.Message}";
                        return false;
                    }
                }
            }
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


        public List<UsuarioDatos> ObtenerAreas()
        {
            List<UsuarioDatos> areas = new List<UsuarioDatos>();
            using (SqlConnection connection = new SqlConnection(Conexion.cn))
            {
                SqlCommand cmd = new SqlCommand("[AHS].[dbo].[Mio_Listado_TodasAreas]", connection);
                cmd.CommandType = CommandType.StoredProcedure;
               

                connection.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        areas.Add(new UsuarioDatos
                        {
                            CodArea = Convert.ToInt32(reader["CodigoArea"]),
                            NombreArea = reader["NombreArea"].ToString()
                        });
                    }
                }
            }
            return areas;
        }

        public List<UsuarioDatos> ObtenerSectoresPorArea(int? codArea = null)
        {
            Console.WriteLine(codArea);
            List<UsuarioDatos> sectores = new List<UsuarioDatos>();
            using (SqlConnection connection = new SqlConnection(Conexion.cn))
            {
                SqlCommand cmd = new SqlCommand("[AHS].[dbo].[Mio_Listado_Sectores]", connection);
                cmd.CommandType = CommandType.StoredProcedure;
                if (codArea.HasValue)
                    cmd.Parameters.AddWithValue("@CodigoArea", codArea.Value);
                else
                    cmd.Parameters.AddWithValue("@CodigoArea", DBNull.Value);

                connection.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        sectores.Add(new UsuarioDatos
                        {
                            CodSector = Convert.ToInt32(reader["CodigoSector"]),
                            Nombre = reader["NombreSector"].ToString(),
                          
                        });
                    }
                }
            }
            return sectores;
        }


    }
}





