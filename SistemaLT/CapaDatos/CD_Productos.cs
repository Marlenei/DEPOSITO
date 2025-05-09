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
                    SqlCommand cmd = new SqlCommand("ListarProductos", oconexion);
                    cmd.CommandType = CommandType.StoredProcedure;
                    oconexion.Open();
                    using (SqlDataReader rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            lista.Add(new Productos()
                            {
                                IdProducto = Convert.ToInt32(rdr["IdProducto"]),
                                oRubros = new Rubros() { 
                                    IdRubro = Convert.ToInt32(rdr["IdRubro"]),
                                    Rubro = rdr["Rubro"].ToString() 
                                },
                                oTipos = new Tipos() { 
                                    IdTipo = Convert.ToInt32(rdr["IdTipo"]), 
                                    Tipo = rdr["Tipo"].ToString() 
                                },
                                Detalle = rdr["Detalle"].ToString(),
                                StockMinimo = Convert.ToInt32(rdr["StockMinimo"]),
                                StockActual = Convert.ToInt32(rdr["StockActual"]),
                                CodigoId = rdr["CodigoId"].ToString(),
                                Activo = Convert.ToBoolean(rdr["Activo"]),
                                FechaAlta = Convert.ToDateTime(rdr["FechaAlta"]),
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

        public List<Productos> ListarProductosporTipos(int idTipo, int idRubro)
        {
            List<Productos> lista = new List<Productos>();
            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    string query = "SELECT * FROM Tonner_Productos WHERE IdTipo = @IdTipo and IdRubro = @IdRubro";
                    SqlCommand cmd = new SqlCommand(query, oconexion);
                    cmd.Parameters.AddWithValue("@IdTipo", idTipo);
                    cmd.Parameters.AddWithValue("@IdRubro", idRubro);
                    cmd.CommandType = CommandType.Text;
                    oconexion.Open();
                    using (SqlDataReader rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            lista.Add(new Productos()
                            {
                                IdProducto = Convert.ToInt32(rdr["IdProducto"]),
                                Detalle = rdr["Detalle"].ToString(),
                                StockActual = Convert.ToInt32(rdr["StockActual"]),
                                Activo = Convert.ToBoolean(rdr["Activo"]),
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

        public List<Productos> ListarProductosporCI(string idCodigo)
        {
            List<Productos> lista = new List<Productos>();
            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    string query = "SELECT * FROM Tonner_Productos WHERE CodigoId = @IdCodigo";
                    SqlCommand cmd = new SqlCommand(query, oconexion);
                    cmd.Parameters.AddWithValue("@IdCodigo", idCodigo);
                    cmd.CommandType = CommandType.Text;
                    oconexion.Open();
                    using (SqlDataReader rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            lista.Add(new Productos()
                            {
                                IdProducto = Convert.ToInt32(rdr["IdProducto"]),
                                Detalle = rdr["Detalle"].ToString(),
                                StockActual = Convert.ToInt32(rdr["StockActual"]),
                                Activo = Convert.ToBoolean(rdr["Activo"]),
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
                    cmd.Parameters.AddWithValue("CodigoId", (object)obj.CodigoId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("IdUsuario", obj.IdUsuario);
                    cmd.Parameters.AddWithValue("Activo", obj.Activo);
                    cmd.Parameters.Add("Resultado", SqlDbType.Int).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("Mensaje", SqlDbType.VarChar, 100).Direction = ParameterDirection.Output;
                    cmd.CommandType = CommandType.StoredProcedure;

                    oconexion.Open();

                    cmd.ExecuteNonQuery();

                    idautogenerado = Convert.ToInt32(cmd.Parameters["Resultado"].Value);
                    Mensaje = cmd.Parameters["Mensaje"].Value.ToString();
                }
            }
            catch (Exception )
            {
                Mensaje = "No se guardo el producto";
                idautogenerado = 0;
            }
            return idautogenerado ;

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
                    cmd.Parameters.AddWithValue("CodigoId", (object)obj.CodigoId ?? DBNull.Value);
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
