using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CapaDatos;
using CapaEntidad;
using CapaNegocio;
using System.Data.SqlClient;
using System.Data;
using System.Web.Mvc;

namespace TonerHP.Controllers
{
    public class EntregaPedidosController : Controller
    {
        private readonly CN_SolicitudPedidos _cnPedidos = new CN_SolicitudPedidos();
     
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
        public ActionResult ListarFiltradosNro(string nroPedido = null, bool? soloPendientes = null)
        {
            try
            {
                /*int codAreaUsuario = (int)Session["CodArea"];*/ // Obtener de la sesión aquí
                var pedidos = _cnPedidos.ListarFiltradosNro(
                    nroPedido: nroPedido,
                    soloPendientes: soloPendientes ?? false
                );
                return Json(new { data = pedidos }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult ActualizarEntrega(SolicitudPedidos pedido)
        {
            try
            {
                // Obtener el pedido completo desde la base de datos
                var pedidoCompleto = _cnPedidos.ObtenerPedido(pedido.IdSolicitud);

                if (pedidoCompleto == null)
                {
                    return Json(new { resultado = false, mensaje = "Pedido no encontrado" });
                }

                // Asignar los datos necesarios
                pedido.IdSolicitud = pedidoCompleto.IdSolicitud;
                pedido.oProductos = pedidoCompleto.oProductos;
                pedido.IdUsuarioEntrega = (int)Session["AccesCode"];

                string mensaje;
                bool resultado = _cnPedidos.ActualizarEntrega(pedido, out mensaje);

                return Json(new { resultado, mensaje });
            }
            catch (Exception ex)
            {
                return Json(new { resultado = false, mensaje = ex.Message });
            }
        }
    }
}
