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
    public class CD_Egresos
    {
        public List<Egresos> Listar()
        {
            List<Egresos> lista = new List<Egresos>();
            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("SELECT i.IdEgreso,");
                    sb.AppendLine("i.CodigoId,");
                    sb.AppendLine("prod.IdProducto, prod.Detalle, prod.StockActual,");
                    sb.AppendLine("i.CodigoId, i.Cantidad, i.Observaciones, i.IdUsuario, i.TipoSalida, i.FechaEgreso,");
                    sb.AppendLine("i.CodigoArea, i.CodigoSector "); // Asegúrate de que estos campos existan en la tabla
                    sb.AppendLine("FROM Tonner_Egresos i ");
                    sb.AppendLine("INNER JOIN Tonner_Productos prod ON prod.IdProducto = i.IdProducto");

                    SqlCommand cmd = new SqlCommand(sb.ToString(), oconexion);
                    cmd.CommandType = CommandType.Text;
                    oconexion.Open();
                    using (SqlDataReader rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            lista.Add(new Egresos()
                            {
                                oProductos = new Productos() { IdProducto = Convert.ToInt32(rdr["IdProducto"]), Detalle = rdr["Detalle"].ToString() },
                                oStockActual = new Productos() { IdProducto = Convert.ToInt32(rdr["IdProducto"]), StockActual = Convert.ToInt32(rdr["StockActual"]) },
                                CodigoId = rdr["CodigoId"].ToString(),
                                Cantidad = Convert.ToInt32(rdr["Cantidad"]),
                                CodigoArea = Convert.ToInt32(rdr["CodigoArea"]),
                                CodigoSector = Convert.ToInt32(rdr["CodigoSector"]),
                                Observaciones = rdr["Observaciones"].ToString(),
                                TipoSalida = Convert.ToChar(rdr["TipoSalida"]),
                                IdUsuario = Convert.ToInt32(rdr["IdUsuario"]),
                                FechaEgreso = Convert.ToDateTime(rdr["FechaEgreso"]), // Asegúrate de que sea DateTime
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Manejo de excepciones
                // Puedes registrar el error o lanzar una excepción personalizada
                throw new Exception("Error al listar egresos: " + ex.Message);
            }
            return lista;
        }


        public int Registrar(Egresos obj, DateTime? fechaEgreso = null)
        {
            int idautogenerado = 0;
            using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
            {
                SqlCommand cmd = new SqlCommand("T_InsertarEgresos", oconexion);
                cmd.CommandType = CommandType.StoredProcedure; // Asegúrate de establecer el tipo de comando

                // Asegúrate de que todos los parámetros tengan el símbolo '@'
                cmd.Parameters.AddWithValue("@IdProducto", obj.oProductos.IdProducto);
                cmd.Parameters.AddWithValue("@Cantidad", obj.Cantidad);
               
                cmd.Parameters.AddWithValue("@CodigoId", obj.CodigoId);
                cmd.Parameters.AddWithValue("@Observaciones", obj.Observaciones);
                cmd.Parameters.AddWithValue("@IdUsuario", obj.IdUsuario);
                cmd.Parameters.AddWithValue("@TipoSalida", obj.TipoSalida);
                cmd.Parameters.AddWithValue("@CodigoArea", obj.CodigoArea);
                cmd.Parameters.AddWithValue("@CodigoSector", obj.CodigoSector);



                // Convertir la cadena a DateTime antes de pasarla
                DateTime fechaEgresoFinal = fechaEgreso ?? DateTime.Now; // Si fechaEgreso es null, usar DateTime.Now
                cmd.Parameters.AddWithValue("@FechaEgreso", fechaEgresoFinal);

              


                cmd.Parameters.Add("@Resultado", SqlDbType.Int).Direction = ParameterDirection.Output;

                try
                {
                    oconexion.Open();
                    cmd.ExecuteNonQuery();
                    idautogenerado = Convert.ToInt32(cmd.Parameters["@Resultado"].Value);
                }
                catch (Exception ex)
                {
                    // Manejo de excepciones
                    throw new Exception("Error al registrar egreso: " + ex.Message);
                }
            }
            return idautogenerado;
        }

        public bool Editar(Egresos obj, out string Mensaje)
        {
            bool resultado = false;
            Mensaje = string.Empty;
            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    SqlCommand cmd = new SqlCommand("EditarEgresos", oconexion);
                    cmd.Parameters.AddWithValue("IdEgresos", obj.IdEgreso);
                    
                    cmd.Parameters.AddWithValue("IdTipo", obj.oProductos.IdProducto);
                    cmd.Parameters.AddWithValue("Cantidad", obj.Cantidad);
                    cmd.Parameters.AddWithValue("CodigoId", obj.CodigoId);
                    cmd.Parameters.AddWithValue("Observaciones", obj.Observaciones);
                    cmd.Parameters.AddWithValue("TipoSalida", obj.CodigoId);
                    cmd.Parameters.AddWithValue("IdUsuario", obj.IdUsuario);
                    cmd.Parameters.AddWithValue("FechaEgreso", obj.FechaEgreso);
                    cmd.Parameters.AddWithValue("CodigoArea", obj.CodigoArea);
                    cmd.Parameters.AddWithValue("CodigoSector", obj.CodigoSector);
                    cmd.Parameters.Add("Resultado", SqlDbType.Bit).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("Mensaje", SqlDbType.VarChar, 500).Direction = ParameterDirection.Output;
                    cmd.CommandType = CommandType.StoredProcedure;

                    oconexion.Open();

                    cmd.ExecuteNonQuery();
                    resultado = Convert.ToBoolean(cmd.Parameters["Resultado"].Value);
                    Mensaje = cmd.Parameters["Mensaje"].Value.ToString();
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
