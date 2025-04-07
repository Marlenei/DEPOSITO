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
        public List<SolicitudPedidos> ListarFiltrados(int? codArea, int? codSector, string nroPedido, DateTime? fechaInicio, DateTime? fechaFin, bool soloPendientes)
        {
            List<SolicitudPedidos> lista = new List<SolicitudPedidos>();

            using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
            {
                SqlCommand cmd = new SqlCommand("usp_ObtenerPedidosFiltrados", oconexion);
                cmd.CommandType = CommandType.StoredProcedure;

                // Parámetros con manejo de valores nulos
                cmd.Parameters.AddWithValue("@CodArea", codArea.HasValue ? (object)codArea.Value : DBNull.Value);
                cmd.Parameters.AddWithValue("@CodSector", codSector.HasValue ? (object)codSector.Value : DBNull.Value);
                cmd.Parameters.AddWithValue("@NroPedido", string.IsNullOrEmpty(nroPedido) ? (object)DBNull.Value : nroPedido);
                cmd.Parameters.AddWithValue("@SoloPendientes", soloPendientes);

                // Manejo de fechas
                if (fechaInicio.HasValue && fechaFin.HasValue)
                {
                    cmd.Parameters.AddWithValue("@FechaInicio", fechaInicio.Value);
                    cmd.Parameters.AddWithValue("@FechaFin", fechaFin.Value);
                }
                else
                {
                    // Si no se proporcionan fechas, establecerlas para el mes actual
                    DateTime fechaActual = DateTime.Now;
                    DateTime primerDiaDelMes = new DateTime(fechaActual.Year, fechaActual.Month, 1);
                    DateTime ultimoDiaDelMes = primerDiaDelMes.AddMonths(1).AddDays(-1);

                    cmd.Parameters.AddWithValue("@FechaInicio", primerDiaDelMes);
                    cmd.Parameters.AddWithValue("@FechaFin", ultimoDiaDelMes);
                }

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
                                Detalle = reader["Producto"].ToString()
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
                            NombreSector = reader["NombreSector"].ToString()
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
        public List<SolicitudPedidos> ListarFiltradosNro(
        string nroPedido = null,
         bool soloPendientes = false)
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
                                Detalle = reader["Producto"].ToString()
                            },
                            CantidadPedida = Convert.ToInt32(reader["CantidadPedida"]),
                            CantidadEntregada = reader["CantidadEntregada"] != DBNull.Value ? Convert.ToInt32(reader["CantidadEntregada"]) : 0,
                            FechaEntrega = reader["FechaEntrega"] != DBNull.Value ? Convert.ToDateTime(reader["FechaEntrega"]).ToString("yyyy-MM-dd HH:mm:ss") : null,
                            NroPedido = reader["NroPedido"].ToString(),
                            Visado = Convert.ToBoolean(reader["Visado"]),
                            FechaPedido = Convert.ToDateTime(reader["FechaPedido"]).ToString("yyyy-MM-dd HH:mm:ss"),
                            CodigoArea = Convert.ToInt32(reader["CodigoArea"]),
                            CodigoSector = Convert.ToInt32(reader["CodigoSector"])
                        });
                    }
                }
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
                    SqlTransaction transaction = oconexion.BeginTransaction();

                    try
                    {
                        // 1. Generar NroPedido
                        using (SqlCommand cmdComprobante = new SqlCommand("Act_ComprobanteDP", oconexion, transaction))
                        {
                            cmdComprobante.CommandType = CommandType.StoredProcedure;
                            SqlParameter paramNcom = new SqlParameter("@pNcom", SqlDbType.VarChar, 15);
                            paramNcom.Direction = ParameterDirection.Output;
                            cmdComprobante.Parameters.Add(paramNcom);
                            cmdComprobante.ExecuteNonQuery();
                            NroPedidoGenerado = paramNcom.Value.ToString();
                            obj.NroPedido = NroPedidoGenerado;
                        }

                        // 2. Registrar Pedido
                        using (SqlCommand cmd = new SqlCommand("T_RegistrarP", oconexion, transaction))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;

                            // Parámetro nuevo
                            cmd.Parameters.AddWithValue("@IdUsuarioEntrega", DBNull.Value); // Valor por defecto

                            // Resto de parámetros existentes
                            cmd.Parameters.AddWithValue("@IdProducto", obj.oProductos.IdProducto);
                            cmd.Parameters.AddWithValue("@CantidadPedida", obj.CantidadPedida);
                            cmd.Parameters.AddWithValue("@CantidadEntregada", DBNull.Value);
                            cmd.Parameters.AddWithValue("@FechaPedido", DateTime.ParseExact(
                                obj.FechaPedido, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture));
                            cmd.Parameters.AddWithValue("@FechaEntrega", DBNull.Value);
                            cmd.Parameters.AddWithValue("@IdUsuarioPedido", obj.IdUsuarioPedido);
                            cmd.Parameters.AddWithValue("@CodigoArea", obj.CodigoArea);
                            cmd.Parameters.AddWithValue("@CodigoSector", obj.CodigoSector);
                            cmd.Parameters.AddWithValue("@Observaciones", (object)obj.Observaciones ?? DBNull.Value);
                            cmd.Parameters.AddWithValue("@NroPedido", obj.NroPedido);
                            cmd.Parameters.AddWithValue("@Visado", false);
                            cmd.Parameters.AddWithValue("@UsuarioVisado", DBNull.Value); // Valor por defecto

                            SqlParameter paramResult = new SqlParameter("@Resultado", SqlDbType.Int);
                            paramResult.Direction = ParameterDirection.Output;
                            cmd.Parameters.Add(paramResult);

                            cmd.ExecuteNonQuery();
                            idautogenerado = Convert.ToInt32(paramResult.Value);
                        }

                        // Commit the transaction
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw new Exception("Error en transacción: " + ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al registrar solicitud: {ex.Message}", ex);
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
                                    Detalle = reader["Producto"].ToString()
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
                    // Step 1: Retrieve current values
                    int cantidadAnterior = 0;
                    int idProducto = 0;
                    string nroPedido = "";
                    string observaciones = null;

                    string querySelect = @"
            SELECT 
                CantidadEntregada, 
                IdProducto,
                NroPedido,
                Observaciones
            FROM Tonner_Pedidos 
            WHERE IdSolicitud = @IdSolicitud";

                    using (SqlCommand cmdSelect = new SqlCommand(querySelect, conexion, transaction))
                    {
                        cmdSelect.Parameters.AddWithValue("@IdSolicitud", pedido.IdSolicitud);

                        using (SqlDataReader reader = cmdSelect.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                cantidadAnterior = reader.IsDBNull(0) ? 0 : reader.GetInt32(0);
                                idProducto = reader.GetInt32(1);
                                nroPedido = reader.GetString(2);
                                observaciones = reader.IsDBNull(3) ? null : reader.GetString(3);
                            }
                            else
                            {
                                transaction.Rollback();
                                mensaje = "Pedido no encontrado";
                                return false;
                            }
                        }
                    }

                    // Step 2: Calculate the difference
                    int? diferencia = pedido.CantidadEntregada - cantidadAnterior;

                    // Step 3: Update the order
                    string queryUpdate = @"UPDATE Tonner_Pedidos SET 
                CantidadEntregada = @CantidadEntregada,
                FechaEntrega = @FechaEntrega,
                FechaHoraActualizacion = GETDATE(),
                IdUsuarioEntrega = @IdUsuarioEntrega
                WHERE IdSolicitud = @IdSolicitud AND Visado = 0";

                    using (SqlCommand cmdUpdate = new SqlCommand(queryUpdate, conexion, transaction))
                    {
                        cmdUpdate.Parameters.AddWithValue("@CantidadEntregada", pedido.CantidadEntregada);
                        cmdUpdate.Parameters.AddWithValue("@FechaEntrega", DateTime.Now);
                        cmdUpdate.Parameters.AddWithValue("@IdUsuarioEntrega", pedido.IdUsuarioEntrega);
                        cmdUpdate.Parameters.AddWithValue("@IdSolicitud", pedido.IdSolicitud);

                        int affected = cmdUpdate.ExecuteNonQuery();
                        if (affected == 0)
                        {
                            transaction.Rollback();
                            mensaje = "No se puede modificar un pedido visado";
                            return false;
                        }
                    }

                    
                    if (diferencia != 0)
                    {
                        // Insert Egreso
                        string insertEgreso = @"
                INSERT INTO Tonner_Egresos (
                    IdProducto, CodigoId, Cantidad, FechaEgreso, 
                    Observaciones, IdUsuario, StockActual,FechayHoraAct, CodigoArea, CodigoSector,
                    TipoSalida
                ) VALUES (
                    @IdProducto, @CodigoId, @Cantidad, @FechaEgreso, 
                    @Observaciones, @IdUsuario, @StockActual, GETDATE(), @CodigoArea, @CodigoSector, 
                    'P'
                )";

                        using (SqlCommand cmdEgreso = new SqlCommand(insertEgreso, conexion, transaction))
                        {
                            int stockActual = GetCurrentStock(idProducto, conexion, transaction);

                            cmdEgreso.Parameters.AddWithValue("@IdProducto", idProducto);
                            cmdEgreso.Parameters.AddWithValue("@CodigoId", nroPedido);
                            cmdEgreso.Parameters.AddWithValue("@Cantidad", Math.Abs((int)diferencia)); // Asegúrate de que sea un int
                            cmdEgreso.Parameters.AddWithValue("@FechaEgreso", DateTime.Now);
                            cmdEgreso.Parameters.AddWithValue("@Observaciones", observaciones ?? (object)DBNull.Value);
                            cmdEgreso.Parameters.AddWithValue("@IdUsuario", pedido.IdUsuarioEntrega); // Asegúrate de pasar el ID del usuario correspondiente
                            cmdEgreso.Parameters.AddWithValue("@StockActual", stockActual); // Asegúrate de que sea un int
                            cmdEgreso.Parameters.AddWithValue("@CodigoArea", pedido.CodigoArea); // Asegúrate de pasar el código de área correspondiente
                            cmdEgreso.Parameters.AddWithValue("@CodigoSector", pedido.CodigoSector); // Asegúrate de pasar el código de sector correspondiente

                            cmdEgreso.ExecuteNonQuery();
                        }

                        // Step 5: Update Stock
                        string updateStock = @"
                UPDATE Tonner_Productos 
                SET StockActual = StockActual - @Cantidad , 
                CodigoId = @CodigoId
                WHERE IdProducto = @IdProducto";

                        using (SqlCommand cmdStock = new SqlCommand(updateStock, conexion, transaction))
                        {
                            cmdStock.Parameters.AddWithValue("@Cantidad", (int)(diferencia ?? 0));
                            cmdStock.Parameters.AddWithValue("@CodigoId", nroPedido);
                            cmdStock.Parameters.AddWithValue("@IdProducto", idProducto);
                            cmdStock.ExecuteNonQuery();
                        }
                    }

                    transaction.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    mensaje = ex.Message;
                    return false;
                }
            }
        }

        private int GetCurrentStock(int idProducto, SqlConnection conexion, SqlTransaction transaction)
        {
            string queryStock = "SELECT StockActual FROM Tonner_Productos WHERE IdProducto = @IdProducto";
            using (SqlCommand cmdStock = new SqlCommand(queryStock, conexion, transaction))
            {
                cmdStock.Parameters.AddWithValue("@IdProducto", idProducto);
                return Convert.ToInt32(cmdStock.ExecuteScalar());
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





