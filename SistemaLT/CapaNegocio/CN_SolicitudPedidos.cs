using CapaDatos;
using CapaEntidad;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace CapaNegocio
{
    public class CN_SolicitudPedidos
    {
        //private readonly CD_SolicitudPedidos objCapaDato;
        private CD_SolicitudPedidos objCapaDato = new CD_SolicitudPedidos();
        //public CN_SolicitudPedidos(HttpClient httpClient)
        //{
        //    objCapaDato = new CD_SolicitudPedidos(httpClient);
        //}



        public List<SolicitudPedidos> Listar()
        {
            return objCapaDato.Listar();
        }


        public int Registrar(SolicitudPedidos obj, out string Mensaje)
        {
            Mensaje = string.Empty;

            if (obj == null)
            {
                Mensaje = "El objeto de solicitud no puede ser nulo.";
                return 0;
            }

            if (obj.CantidadPedida <= 0) Mensaje += "La cantidad pedida debe ser mayor a 0. ";
            if (obj.oProductos?.IdProducto == 0) Mensaje += "Seleccionar un producto. ";
            if (obj.CantidadEntregada > obj.CantidadPedida) Mensaje += "La cantidad entregada no puede ser mayor a la cantidad pedida. ";

            return string.IsNullOrEmpty(Mensaje) ? objCapaDato.Registrar(obj) : 0;
        }

        //public async Task<(UsuarioDatos usuarioDatos, string mensaje)> ObtenerDatosUsuario(string usuario, int codigoUnico)
        //{
        //    string mensaje = string.Empty;

        //    if (string.IsNullOrWhiteSpace(usuario))
        //    {
        //        mensaje = "El nombre de usuario no puede estar vacío.";
        //        return (null, mensaje);
        //    }

        //    if (codigoUnico <= 0)
        //    {
        //        mensaje = "El código único debe ser un valor positivo.";
        //        return (null, mensaje);
        //    }

        //    try
        //    {
        //        var datosUsuario = await objCapaDato.ObtenerDatosUsuario(usuario, codigoUnico);
        //        return (datosUsuario, mensaje);
        //    }
        //    catch (Exception ex)
        //    {
        //        mensaje = $"Error al obtener datos del usuario: {ex.Message}";
        //        return (null, mensaje);
        //    }
        //}

        public bool Editar(SolicitudPedidos obj, out string Mensaje)
        {
            Mensaje = string.Empty;

            if (obj == null)
            {
                Mensaje = "El objeto de solicitud no puede ser nulo.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(obj.Observaciones)) Mensaje = "Las observaciones no pueden estar vacías.";
            if (obj.CantidadEntregada < 0) Mensaje += "La cantidad entregada no puede ser negativa.";

            return string.IsNullOrEmpty(Mensaje) ? objCapaDato.Editar(obj, out Mensaje) : false;
        }

        public List<UsuarioDatos> ObtenerAreas()
        {
            return objCapaDato.ObtenerAreas();
        }

        public List<UsuarioDatos> ObtenerSectoresPorArea(int codigoArea)
        {
            return objCapaDato.ObtenerSectoresPorArea(codigoArea);
        }
    }
}