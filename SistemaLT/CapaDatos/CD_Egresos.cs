using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CapaEntidad;
using System.Web;


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
                    SqlCommand cmd = new SqlCommand("ListarEgresos", oconexion)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    oconexion.Open();
                    Console.WriteLine("Ejecutando el procedimiento almacenado...");
                    using (SqlDataReader rdr = cmd.ExecuteReader())
                    {
                        Console.WriteLine("Procedimiento almacenado ejecutado correctamente.");
                        while (rdr.Read())
                        {
                            lista.Add(new Egresos()
                            {
                                IdEgreso = Convert.ToInt32(rdr["IdEgreso"]),
                                oProductos = new Productos()
                                {
                                    IdProducto = Convert.ToInt32(rdr["IdProducto"]),
                                    Detalle = rdr["Detalle"].ToString(),
                                    StockActual = Convert.ToInt32(rdr["StockActual"]),
                                    CodigoId = rdr["CodigoId"].ToString(),
                                    oRubros = new Rubros()
                                    {
                                        IdRubro = Convert.ToInt32(rdr["IdRubro"]),
                                        Rubro = rdr["Rubro"].ToString()
                                    },
                                    oTipos = new Tipos()
                                    {
                                        IdTipo = Convert.ToInt32(rdr["IdTipo"]),
                                        Tipo = rdr["Tipo"].ToString()
                                    },

                                },
                                CodigoId = rdr["CodigoId"].ToString(),
                                Cantidad = Convert.ToInt32(rdr["Cantidad"]),
                                NombreArea = rdr["NombreArea"].ToString(),
                                CodigoArea = Convert.ToInt32(rdr["CodigoArea"]),
                                NombreSector = rdr["NombreSector"].ToString(),
                                CodigoSector = Convert.ToInt32(rdr["CodigoSector"]),
                                Observaciones = rdr["Observaciones"].ToString(),
                                TipoSalida = Convert.ToChar(rdr["TipoSalida"]),
                                NombreyApellido = rdr["NombreyApellido"].ToString(),
                                FechaEgreso = Convert.ToDateTime(rdr["FechaEgreso"]),
                                FechaAct = Convert.ToDateTime(rdr["FechayHoraAct"]),
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
              throw new Exception("Error al listar egresos: " + ex.Message);
            }
            var permisos = HttpContext.Current.Session["PermissionsCode"] as List<Permiso>; 
            if (permisos != null)
            {
                bool tiene25 = permisos.Any(p => p.Accesos == 25);
                bool tiene183 = permisos.Any(p => p.Accesos == 183);


                if (tiene25)
                {
                    lista = lista.Where(e => e.oProductos.oRubros.Rubro != "Insumos Informaticos").ToList();
                }
                else if (tiene183)
                {
                    lista = lista.Where(e => e.oProductos.oRubros.Rubro == "Insumos Informaticos").ToList();
                }
            }
            return lista;
        }


        public int Registrar(Egresos obj, DateTime? fechaEgreso = null)
        {
            int idautogenerado = 0;
            using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
            {
                SqlCommand cmd = new SqlCommand("T_InsertarEgresos", oconexion);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdProducto", obj.oProductos.IdProducto);
                cmd.Parameters.AddWithValue("@Cantidad", obj.Cantidad);
                cmd.Parameters.AddWithValue("@CodigoId", (object)obj.CodigoId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Observaciones", (object)obj.Observaciones ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@IdUsuario", obj.IdUsuario);
                cmd.Parameters.AddWithValue("@TipoSalida", obj.TipoSalida);
                cmd.Parameters.AddWithValue("@CodigoArea", obj.CodigoArea);
                cmd.Parameters.AddWithValue("@CodigoSector", obj.CodigoSector);
                cmd.Parameters.AddWithValue("@FechaEgreso", Convert.ToDateTime(obj.FechaEgreso));         
                cmd.Parameters.Add("@Resultado", SqlDbType.Int).Direction = ParameterDirection.Output;

                try
                {
                    oconexion.Open();
                    cmd.ExecuteNonQuery();
                    idautogenerado = Convert.ToInt32(cmd.Parameters["@Resultado"].Value);
                }
                catch (Exception ex)
                {
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
                    cmd.Parameters.AddWithValue("IdEgreso", obj.IdEgreso);
                    cmd.Parameters.AddWithValue("IdProducto", obj.oProductos.IdProducto);
                    cmd.Parameters.AddWithValue("Cantidad", obj.Cantidad);
                    cmd.Parameters.AddWithValue("CodigoId", (object)obj.CodigoId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("Observaciones", (object)obj.Observaciones ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("TipoSalida", obj.TipoSalida);
                    cmd.Parameters.AddWithValue("IdUsuario", obj.IdUsuario);
                    cmd.Parameters.AddWithValue("FechaEgreso", obj.FechaEgreso);
                    cmd.Parameters.AddWithValue("CodigoArea", obj.CodigoArea);
                    cmd.Parameters.AddWithValue("CodigoSector", obj.CodigoSector);
                    cmd.Parameters.Add("FechayHoraAct", SqlDbType.SmallDateTime).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("Resultado", SqlDbType.Bit).Direction = ParameterDirection.Output;
                    //cmd.Parameters.Add("Mensaje", SqlDbType.VarChar, 500).Direction = ParameterDirection.Output;
                    cmd.CommandType = CommandType.StoredProcedure;

                    oconexion.Open();
                    cmd.ExecuteNonQuery();
                    resultado = Convert.ToBoolean(cmd.Parameters["Resultado"].Value);
                    //Mensaje = cmd.Parameters["Mensaje"].Value.ToString();
                }
            }
            catch (Exception ex)
            {
                resultado = false;
                Mensaje = ex.Message;
            }
            return resultado;
        }

        public void ActualizarStock(int idProducto, int diferencia)
        {
            try
            {
                using (SqlConnection conexion = new SqlConnection(Conexion.cn))
                {
                    string sql = "UPDATE Tonner_Productos SET StockActual = StockActual + @Diferencia, FechaUltModificacion = GETDATE() WHERE IdProducto = @IdProducto";
                    SqlCommand cmd = new SqlCommand(sql, conexion);
                    cmd.Parameters.AddWithValue("@Diferencia", diferencia);
                    cmd.Parameters.AddWithValue("@IdProducto", idProducto);

                    conexion.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error actualizando stock: " + ex.Message);
            }
        }

        public Egresos ObtenerEgresoPorId(int idEgreso)
        {
            Egresos egreso = null;
            try
            {
                using (SqlConnection conexion = new SqlConnection(Conexion.cn))
                {
                    string sql = "SELECT * FROM Tonner_Egresos WHERE IdEgreso = @IdEgreso";
                    SqlCommand cmd = new SqlCommand(sql, conexion);
                    cmd.Parameters.AddWithValue("@IdEgreso", idEgreso);

                    conexion.Open();
                    using (SqlDataReader rdr = cmd.ExecuteReader())
                    {
                        if (rdr.Read())
                        {
                            egreso = new Egresos();
                            egreso.IdEgreso = Convert.ToInt32(rdr["IdEgreso"]);
                            egreso.IdUsuario = Convert.ToInt32(rdr["IdUsuario"]);
                            egreso.oProductos = new Productos()
                            {
                                IdProducto = Convert.ToInt32(rdr["IdProducto"]),
                                StockActual = Convert.ToInt32(rdr["StockActual"]),
                            };
                            egreso.CodigoId = rdr["CodigoId"].ToString();
                            egreso.Cantidad = Convert.ToInt32(rdr["Cantidad"]);
                            egreso.CodigoArea = Convert.ToInt32(rdr["CodigoArea"]);
                            egreso.CodigoSector = Convert.ToInt32(rdr["CodigoSector"]);
                            egreso.Observaciones = rdr["Observaciones"].ToString();
                            egreso.TipoSalida = Convert.ToChar(rdr["TipoSalida"]);
                            egreso.FechaEgreso = Convert.ToDateTime(rdr["FechaEgreso"]);
                            egreso.FechaAct = Convert.ToDateTime(rdr["FechayHoraAct"]);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error obteniendo ingreso: " + ex.Message);
            }
            return egreso;
        }
    }
}
