using CapaEntidad;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaDatos
{
    public class CD_Ingresos
    {
        public List<Ingresos> Listar()
        {
            List<Ingresos> lista = new List<Ingresos>();
            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    SqlCommand cmd = new SqlCommand("ListarIngresos", oconexion)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    oconexion.Open();
                    using (SqlDataReader rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            lista.Add(new Ingresos()
                            {
                                IdIngreso = Convert.ToInt32(rdr["IdIngreso"]),
                                oProveedores = new Proveedores() { 
                                    IdProveedor = Convert.ToInt32(rdr["IdProveedor"]), 
                                    RazonSocial = rdr["RazonSocial"].ToString() 
                                },
                                oProductos = new Productos() { 
                                    IdProducto = Convert.ToInt32(rdr["IdProducto"]), 
                                    Detalle = rdr["Detalle"].ToString(),
                                    StockActual = Convert.ToInt32(rdr["StockActual"])
                                },
                                CodigoId = rdr["CodigoId"].ToString(),
                                NroExpediente = rdr["NroExpediente"].ToString(),
                                Cantidad = Convert.ToInt32(rdr["Cantidad"]),
                                Observaciones = rdr["Observaciones"].ToString(),
                                TipoIngreso = Convert.ToChar(rdr["TipoIngreso"]),
                                FechaIngreso = rdr["FechaIngreso"].ToString(),
                                NombreyApellido = rdr["NombreyApellido"].ToString(),
                                FechaAct1 = rdr["FechayHoraAct"].ToString(),
                                FechaAct = Convert.ToDateTime(rdr["FechayHoraAct"]),
                            });
                        }
                    }
                }
            }catch
            {
                lista = new List<Ingresos>();
            }
            return lista;
        }


        public int Registrar(Ingresos obj)
        {
            int idautogenerado = 0;
            using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
            {
                SqlCommand cmd = new SqlCommand("T_InsertarIngresos", oconexion);
                cmd.Parameters.AddWithValue("IdProducto", obj.oProductos.IdProducto);
                cmd.Parameters.AddWithValue("IdProveedor", obj.oProveedores.IdProveedor);
                cmd.Parameters.AddWithValue("Observaciones", (object)obj.Observaciones ?? DBNull.Value);
                cmd.Parameters.AddWithValue("CodigoId", (object)obj.CodigoId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("Cantidad", obj.Cantidad);
                cmd.Parameters.AddWithValue("IdUsuario", obj.IdUsuario);
                cmd.Parameters.AddWithValue("TipoIngreso", obj.TipoIngreso);
                cmd.Parameters.AddWithValue("NroExpediente", (object)obj.NroExpediente ?? DBNull.Value);
                cmd.Parameters.AddWithValue("FechaIngreso", Convert.ToDateTime(obj.FechaIngreso));
                cmd.Parameters.Add("resultado", SqlDbType.Int).Direction = ParameterDirection.Output;
                //cmd.Parameters.Add("Mensaje", SqlDbType.VarChar, 100).Direction = ParameterDirection.Output;
                cmd.CommandType = CommandType.StoredProcedure;

                oconexion.Open();

                cmd.ExecuteNonQuery();

                idautogenerado = Convert.ToInt32(cmd.Parameters["resultado"].Value);
                //Mensaje = cmd.Parameters["Mensaje"].Value.ToString();
            }
            return idautogenerado;
        }

        public bool Editar (Ingresos obj, out string Mensaje)
        {
            bool resultado = false;
            Mensaje = string.Empty;
            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    SqlCommand cmd = new SqlCommand("T_ModificarIngresos", oconexion);
                    cmd.Parameters.AddWithValue("IdIngreso", obj.IdIngreso);
                    cmd.Parameters.AddWithValue("IdProducto", obj.oProductos.IdProducto);
                    cmd.Parameters.AddWithValue("IdProveedor", obj.oProveedores.IdProveedor);
                    cmd.Parameters.AddWithValue("Observaciones", (object)obj.Observaciones ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("CodigoId", (object)obj.CodigoId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("Cantidad", obj.Cantidad);
                    cmd.Parameters.AddWithValue("IdUsuario", obj.IdUsuario);
                    cmd.Parameters.AddWithValue("TipoIngreso", obj.TipoIngreso);
                    cmd.Parameters.AddWithValue("FechaIngreso", Convert.ToDateTime(obj.FechaIngreso));
                    cmd.Parameters.AddWithValue("NroExpediente", (object)obj.NroExpediente ?? DBNull.Value);

                    cmd.Parameters.Add("Resultado", SqlDbType.Bit).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("FechayHoraAct", SqlDbType.DateTime).Direction = ParameterDirection.Output;

                    cmd.CommandType = CommandType.StoredProcedure;

                    oconexion.Open();

                    cmd.ExecuteNonQuery();
                    resultado = Convert.ToBoolean(cmd.Parameters["Resultado"].Value);
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
