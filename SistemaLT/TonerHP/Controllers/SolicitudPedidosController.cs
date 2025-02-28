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


        //[HttpGet]
        //[Authorize]
        //public ActionResult SolicitudPedidos()
        //{
        //    // Obtener áreas desde la capa de datos
        //    var listaAreas = _cdSolicitudPedidos.ObtenerAreas();
        //    ViewBag.Areas = listaAreas;

        //    // Obtener sectores del usuario actual
        //    int codigoAreaUsuario = (int)Session["CodigoArea"];
        //    var listaSectores = _cdSolicitudPedidos.ObtenerSectoresPorArea(codigoAreaUsuario);
        //    ViewBag.Sectores = listaSectores;

        //    return View();
        //}



        //[HttpGet]
        //public JsonResult ObtenerAreas()
        //{
        //    try
        //    {
        //        var areas = _cdSolicitudPedidos.ObtenerAreas();
        //        return Json(new { data = areas }, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { error = ex.Message }, JsonRequestBehavior.AllowGet);
        //    }
        //}

        //[HttpPost]
        //public JsonResult ObtenerSectoresPorArea(int codigoArea)
        //{
        //    try
        //    {
        //        var sectores = _cdSolicitudPedidos.ObtenerSectoresPorArea(codigoArea);
        //        return Json(new { data = sectores });
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { error = ex.Message });
        //    }
        //}


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



    }
}