using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CapaDatos;
using CapaEntidad;

namespace CapaNegocio
{
    public class CN_Egresos

    {
        private bool IsAlphanumeric(string input)
        {
            if(input == null)
            {
                return true;
            }
            return Regex.IsMatch(input, "^[a-zA-Z0-9ñÑáéíóúÁÉÍÓÚüÜ ]*$");
        }

        private CD_Egresos objCapaDato = new CD_Egresos();

        public List<Egresos> Listar()
        {
            return objCapaDato.Listar();
        }

        public int Registrar(Egresos objeto, out string Mensaje)
        {
            Mensaje = string.Empty;



            if (objeto.oProductos.IdProducto == 0)
            {
                Mensaje = "Ingresar producto";
            }

            else if (!IsAlphanumeric(objeto.CodigoId))
            {
                Mensaje = "Codigo solo debe contener numeros o letras";
            }

            else if (objeto.IdUsuario == 0)
            {
                Mensaje = "Ingresar usuario";
            }

            else if (!IsAlphanumeric(objeto.Observaciones))
            {
                Mensaje = "Observaciones solo debe contener numeros o letras";
            }

            else if (objeto.TipoSalida == '\0')
            {
                Mensaje = "Debe elegir un tipo de salida";
            }

            else if (objeto.CodigoArea == 0)
            {
                Mensaje = "Ingresar Area";

            }

            else if (objeto.CodigoSector == 0)
            {
                Mensaje = "Ingresar Sector";

            }

            if (string.IsNullOrEmpty(Mensaje))
            {
                return objCapaDato.Registrar(objeto);
            }
            else
            {
                return 0;
            }

        }

        private void ActualizarStockProducto(int idProducto, int diferencia)
        {
            objCapaDato.ActualizarStock(idProducto, diferencia);
        }

        private bool ActualizarEgresoSimple(Egresos objeto, out string mensaje)
        {
            return objCapaDato.Editar(objeto, out mensaje);
        }

        private Egresos ObtenerEgresoPorId(int idEgreso)
        {
            return objCapaDato.ObtenerEgresoPorId(idEgreso);
        }
        public bool Editar(Egresos objeto, out string Mensaje)
        {
            Mensaje = string.Empty;


            if (objeto.oProductos.IdProducto == 0)
            {
                Mensaje = "Ingresar producto";
            }

            else if (!IsAlphanumeric(objeto.CodigoId))
            {
                Mensaje = "Codigo solo debe contener numeros o letras";
            }

            else if (objeto.IdUsuario == 0)
            {
                Mensaje = "Ingresar usuario";
            }

            else if (!IsAlphanumeric(objeto.Observaciones))
            {
                Mensaje = "Observaciones solo debe contener numeros o letras";
            }

            else if (objeto.TipoSalida == '\0')
            {
                Mensaje = "Debe elegir un tipo de salida";
            }

            else if (objeto.CodigoArea == 0)
            {
                Mensaje = "Ingresar Area";

            }

            else if (objeto.CodigoSector == 0)
            {
                Mensaje = "Ingresar Sector";

            }
            try
            {
                Egresos egresoOriginal = ObtenerEgresoPorId(objeto.IdEgreso);

                if (egresoOriginal == null)
                {
                    Mensaje = "No se encontró el egreso original.";
                    return false;
                }

                int cantidadActual = egresoOriginal.Cantidad;
                int productoActual = egresoOriginal.oProductos.IdProducto;

                int diferencia = objeto.Cantidad - cantidadActual;
                bool productoCambiado = objeto.oProductos.IdProducto != productoActual;
                bool cantidadCambiada = objeto.Cantidad != cantidadActual;
                if (!productoCambiado && !cantidadCambiada)
                {
                    return ActualizarEgresoSimple(objeto, out Mensaje);
                }
                else if (!productoCambiado && cantidadCambiada)
                {
                    ActualizarStockProducto(objeto.oProductos.IdProducto, -diferencia);
                    return ActualizarEgresoSimple(objeto, out Mensaje);
                }
                else if (productoCambiado)
                {
                    bool resultado =  ActualizarEgresoSimple(objeto, out Mensaje);
                    if (resultado)
                    {
                        ActualizarStockProducto(productoActual, cantidadActual);
                        ActualizarStockProducto(objeto.oProductos.IdProducto, -objeto.Cantidad);
                    }
                    return resultado;
                    //return ActualizarEgresoSimple(objeto, out Mensaje);
                }
                else
                {
                    Mensaje = "No se pudo determinar la operación.";
                    return false;
                }
            }
            catch (Exception ex)
            {
                Mensaje = ex.Message;
                return false;
            }
        }



        public object Registrar(SolicitudPedidos objeto, out string mensaje)
        {
            throw new NotImplementedException();
        }
    }
}
