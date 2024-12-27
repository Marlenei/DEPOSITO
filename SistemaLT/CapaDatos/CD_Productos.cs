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
    public class CD_Productos
    {
        public List<Productos> Listar()
        {
            List<Productos> lista = new List<Productos>();
            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("SELECT p.IdProducto, ");
                    sb.AppendLine("r.IdRubro, r.Rubro, ");
                    sb.AppendLine("t.IdTipo, t.Tipo, ");
                    sb.AppendLine("COALESCE(p.StockActual, 0) AS StockActual, ");
                    sb.AppendLine("p.Detalle, p.StockMinimo, p.StockActual, p.Activo, p.IdUsuario, p.CodigoId ");
                    sb.AppendLine("FROM Tonner_Productos p ");
                    sb.AppendLine("inner join Tonner_Rubros r on r.IdRubro = p.IdRubro");
                    sb.AppendLine("inner join Tonner_Tipos t on t.IdTipo = p.IdTipo ");

                    SqlCommand cmd = new SqlCommand(sb.ToString(), oconexion);
                    cmd.CommandType = CommandType.Text;
                    oconexion.Open();
                    using (SqlDataReader rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            lista.Add(new Productos()
                            {
                                IdProducto = Convert.ToInt32(rdr["IdProducto"]),
                                oRubros = new Rubros() { IdRubro = Convert.ToInt32(rdr["IdRubro"]), Rubro = rdr["Rubro"].ToString() },
                                oTipos = new Tipos() { IdTipo = Convert.ToInt32(rdr["IdTipo"]), Tipo = rdr["Tipo"].ToString() },
                                Detalle = rdr["Detalle"].ToString(),
                                StockMinimo = Convert.ToInt32(rdr["StockMinimo"]),
                                StockActual = Convert.ToInt32(rdr["StockActual"]),
                                CodigoId = rdr["CodigoId"].ToString(),
                                Activo = Convert.ToBoolean(rdr["Activo"]),
                                IdUsuario = Convert.ToInt32(rdr["IdUsuario"]),
                            });
                        }
                    }
                }
            }
            catch
            {
                lista = new List<Productos>();
            }
            return lista;
        }


        public int Registrar(Productos obj)
        {
            string Mensaje;
            int idautogenerado = 0;
            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    SqlCommand cmd = new SqlCommand("T_InsertarProductos", oconexion);
                    cmd.Parameters.AddWithValue("IdRubro", obj.oRubros.IdRubro);
                    cmd.Parameters.AddWithValue("IdTipo", obj.oTipos.IdTipo);
                    cmd.Parameters.AddWithValue("Detalle", obj.Detalle);
                    cmd.Parameters.AddWithValue("StockMinimo", obj.StockMinimo);
                    cmd.Parameters.AddWithValue("CodigoId", obj.CodigoId);
                    cmd.Parameters.AddWithValue("IdUsuario", obj.IdUsuario);
                    cmd.Parameters.AddWithValue("Activo", obj.Activo);
                    cmd.Parameters.Add("resultado", SqlDbType.Int).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("Mensaje", SqlDbType.VarChar, 100).Direction = ParameterDirection.Output;
                    cmd.CommandType = CommandType.StoredProcedure;

                    oconexion.Open();

                    cmd.ExecuteNonQuery();

                    idautogenerado = Convert.ToInt32(cmd.Parameters["resultado"].Value);
                    Mensaje = cmd.Parameters["Mensaje"].Value.ToString();
                }
            }
            catch (Exception ex)
            {
                Mensaje = ex.Message;
                idautogenerado = 0;
            }
            return idautogenerado;
        }

        public bool Editar(Productos obj, out string Mensaje)
        {
            bool resultado = false;
            Mensaje = string.Empty;
            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    SqlCommand cmd = new SqlCommand("T_ModificarProductos", oconexion);
                    cmd.Parameters.AddWithValue("IdProducto", obj.IdProducto);
                    cmd.Parameters.AddWithValue("IdRubro", obj.oRubros.IdRubro);
                    cmd.Parameters.AddWithValue("IdTipo", obj.oTipos.IdTipo);
                    cmd.Parameters.AddWithValue("Detalle", obj.Detalle);
                    cmd.Parameters.AddWithValue("StockMinimo", obj.StockMinimo);
                    cmd.Parameters.AddWithValue("CodigoId", obj.CodigoId);
                    cmd.Parameters.AddWithValue("IdUsuario", obj.IdUsuario);
                    cmd.Parameters.AddWithValue("Activo", obj.Activo);
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
