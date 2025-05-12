using CapaDatos;
using CapaEntidad;
using CapaNegocio;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace TonerHP.Controllers
{
    public class SolicitudPedidosController : Controller
    {
        private readonly CN_SolicitudPedidos _cnPedidos = new CN_SolicitudPedidos();
        private readonly CD_SolicitudPedidos _cdPedidos = new CD_SolicitudPedidos();
        private readonly CN_Productos _cnProductos = new CN_Productos(); // Añadir esta línea


        public ActionResult Index()
        {
            // Obtener códigos de sesión

            var codArea = Session["CodArea"] as int? ?? 0;
            var codSector = Session["CodSector"] as int? ?? 0;

            // Obtener nombres actualizados
            ViewBag.NombreArea = _cnPedidos.ObtenerNombreArea(codArea);
            ViewBag.NombreSector = _cnPedidos.ObtenerNombreSector(codArea, codSector);

            return View();
        }

        [HttpGet]
        public JsonResult Listar()
        {
            if (Session["AccesCode"] == null)
            {
                return Json(new { error = "Usuario no autenticado" }, JsonRequestBehavior.AllowGet);
            }

            int idUsuario = (int)Session["AccesCode"];
            var lista = _cnPedidos.Listar(idUsuario);
            return Json(new { data = lista }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult ListarFiltrados(int? codArea, int? codSector, string nroPedido, DateTime? fechaInicio, DateTime? fechaFin, bool soloPendientes)
        {
            var pedidos = _cdPedidos.ListarFiltrados(codArea, codSector, nroPedido, fechaInicio, fechaFin, soloPendientes);
            return Json(new { data = pedidos }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult ListarFiltradosNro(string nroPedido = null, bool soloPendientes = false)
        {
            try
            {
                var pedidos = _cdPedidos.ListarFiltradosNro(nroPedido, soloPendientes);
                return Json(new { data = pedidos }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult ListarPendientesPorArea()
        {
            try
            {
                int codArea = Convert.ToInt32(Session["CodArea"]); // Obtener el código del área de la sesión
                int? codSector = null; // O puedes obtenerlo de la sesión si es necesario

                // Establecer valores por defecto para las fechas
                DateTime fechaInicio = DateTime.Now.AddMonths(-1); // Último mes
                DateTime fechaFin = DateTime.Now; // Fecha actual

                var pedidos = _cdPedidos.ListarFiltrados(codArea: codArea, codSector: codSector, nroPedido: null, fechaInicio: fechaInicio, fechaFin: fechaFin, soloPendientes: true);
                return Json(new { data = pedidos }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult ObtenerPedido(int idPedido)
        {
            var pedido = _cdPedidos.ObtenerPedido(idPedido);
            if (pedido == null)
            {
                // Manejar el caso en que no se encontró el pedido
                return HttpNotFound();
            }

            return View(pedido); // O devolver un JsonResult, según sea necesario
        }

      

        [HttpPost]
        public JsonResult RegistrarVisado(int idPedido)
        {
            try
            {
                int idUsuario = (int)Session["AccesCode"];
                string mensaje;
                bool resultado = _cnPedidos.RegistrarVisado(idPedido, idUsuario, out mensaje);

                return Json(new { resultado, mensaje });
            }
            catch (Exception ex)
            {
                return Json(new { resultado = false, mensaje = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult Actualizar(SolicitudPedidos objeto)
        {
            try
            {
                // Validación básica del objeto
                if (objeto == null || objeto.IdSolicitud <= 0)
                {
                    return Json(new { resultado = false, mensaje = "No se recibieron datos del pedido o el ID es inválido." });
                }

                // Asignar códigos desde la sesión
                objeto.CodigoArea = (int)Session["CodArea"];
                objeto.CodigoSector = (int)Session["CodSector"];

                // Validaciones de negocio
                if (objeto.CantidadPedida <= 0)
                {
                    return Json(new { resultado = false, mensaje = "La cantidad pedida debe ser mayor a 0" });
                }

                if (objeto.oProductos?.IdProducto == 0)
                {
                    return Json(new { resultado = false, mensaje = "Seleccione un producto válido" });
                }

                // Lógica para actualizar el pedido
                string mensaje;
                bool resultadoOperacion = _cnPedidos.Actualizar(objeto, out mensaje);

                return Json(new
                {
                    resultado = resultadoOperacion,
                    mensaje = resultadoOperacion ? "Pedido actualizado con éxito." : mensaje
                });
            }
            catch (Exception ex)
            {
                return Json(new { resultado = false, mensaje = $"Error al actualizar solicitud: {ex.Message}" });
            }
        }

        [HttpPost]
        public JsonResult GuardarPedidos(SolicitudPedidos objeto, List<SolicitudPedidos> listaProductos)
        {
            try
            {
                if (objeto == null || listaProductos == null || listaProductos.Count == 0)
                {
                    return Json(new { resultado = false, mensaje = "No se recibieron datos del pedido o la lista de productos está vacía" });
                }

                // Validar que la fecha de pedido no esté vacía
                if (string.IsNullOrEmpty(objeto.FechaPedido))
                {
                    return Json(new { resultado = false, mensaje = "La fecha de pedido no puede estar vacía" });
                }

                // Validar que la fecha de pedido tenga el formato correcto (yyyy-MM-dd)
                if (!DateTime.TryParseExact(objeto.FechaPedido, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture,
                    System.Globalization.DateTimeStyles.None, out DateTime fechaPedido))
                {
                    return Json(new { resultado = false, mensaje = "La fecha de pedido no tiene un formato válido. Use yyyy-MM-dd" });
                }

                // Validar que las cantidades sean correctas
                foreach (var producto in listaProductos)
                {
                    if (producto.CantidadPedida <= 0)
                    {
                        return Json(new { resultado = false, mensaje = "La cantidad pedida debe ser mayor a 0" });
                    }
                }

                // Asignar códigos del área y sector
                objeto.CodigoArea = (int)Session["CodArea"];
                objeto.CodigoSector = (int)Session["CodSector"];

                // Llamar a la capa de negocio
                string mensaje = string.Empty;
                string nroPedidoGenerado = string.Empty;
                int idGenerado = _cnPedidos.Registrar(objeto, listaProductos, out mensaje, out nroPedidoGenerado);

                bool esExito = idGenerado > 0;

                return Json(new
                {
                    resultado = esExito,
                    mensaje = esExito ? "Operación exitosa" : mensaje,
                    nroPedido = nroPedidoGenerado
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    resultado = false,
                    mensaje = $"Error interno: {ex.Message}"
                });
            }
        }



        [HttpGet]
        public JsonResult GenerarNumeroPedido()
        {
            try
            {
                string nroPedido;
                using (SqlConnection conn = new SqlConnection(Conexion.cn))
                {
                    SqlCommand cmd = new SqlCommand("Act_ComprobanteDP", conn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    SqlParameter paramNcom = new SqlParameter("@pNcom", SqlDbType.VarChar, 15);
                    paramNcom.Direction = ParameterDirection.Output;
                    cmd.Parameters.Add(paramNcom);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                    nroPedido = paramNcom.Value.ToString();
                }
                return Json(nroPedido, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json("Error: " + ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult ObtenerAreas()
        {
            var areas = _cdPedidos.ObtenerAreas();
            return Json(areas, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult ObtenerSectoresPorArea(int? codArea)
        {
            var sectores = _cdPedidos.ObtenerSectoresPorArea(codArea);
            return Json(sectores, JsonRequestBehavior.AllowGet);
        }

    }
}