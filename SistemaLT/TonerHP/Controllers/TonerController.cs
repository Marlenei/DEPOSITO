using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using CapaDatos;
using CapaEntidad;
using CapaNegocio;
using Newtonsoft.Json;


namespace TonerHP.Controllers
{
    public class TonerController : Controller
    {
        // GET: Toner

        public JsonResult GetApiUrls()
        {
            var apiUrls = new
            {
                ApiUrlDev = ConfigurationManager.AppSettings["ApiUrlDev"],
                ApiUrlProd = ConfigurationManager.AppSettings["ApiUrlProd"]
            };

            return Json(apiUrls, JsonRequestBehavior.AllowGet);
        }

        public ActionResult _BusAvan()
        {
            return PartialView("_BusAvan");
        }
        [Authorize]

        public ActionResult Proveedores()
        {
            var permisos = Session["PermissionsCode"] as List<Permiso>;
            if (permisos == null)
            {
                return RedirectToAction("Login", "Usuario");
            }
            var tieneAcceso = permisos.Any(p => p.Accesos == 182 || p.Accesos == 186);
            if (!tieneAcceso)
            {
                return RedirectToAction("Error", "Home");
            }

            return View();
        }
        [Authorize]

        public ActionResult Tipos()
        {
            var permisos = Session["PermissionsCode"] as List<Permiso>;
            if (permisos == null)
            {
                return RedirectToAction("Login", "Usuario");
            }
            var tieneAcceso = permisos.Any(p => p.Accesos == 182);
            if (!tieneAcceso)
            {
                return RedirectToAction("Error", "Home");
            }

            return View();
        }
        [Authorize]

        public ActionResult Productos()
        {
            var permisos = Session["PermissionsCode"] as List<Permiso>;
            if (permisos == null)
            {
                return RedirectToAction("Login", "Usuario");
            }
            var tieneAcceso = permisos.Any(p => p.Accesos == 182 || p.Accesos == 186);
            if (!tieneAcceso)
            {
                return RedirectToAction("Error", "Home");
            }

            return View();
        }
        [Authorize]
        public ActionResult Ingresos()
        {
            var permisos = Session["PermissionsCode"] as List<Permiso>;
            if (permisos == null)
            {
                return RedirectToAction("Login", "Usuario");
            }
            var tieneAcceso = permisos.Any(p => p.Accesos == 24 || p.Accesos == 184);
            if (!tieneAcceso)
            {
                return RedirectToAction("Error", "Home");
            }

            return View();
        }

        [Authorize]
        public ActionResult Egresos()
        {

            var permisos = Session["PermissionsCode"] as List<Permiso>;
            if (permisos == null)
            {
                return RedirectToAction("Login", "Usuario");
            }
            var tieneAcceso = permisos.Any(p => p.Accesos == 24 || p.Accesos == 25 || p.Accesos == 183 || p.Accesos == 184);
            if (!tieneAcceso)
            {
                return RedirectToAction("Error", "Home");

            }

            return View();
        }
        [Authorize]

        public ActionResult Rubros()
        {
            var permisos = Session["PermissionsCode"] as List<Permiso>;
            if (permisos == null)
            {
                return RedirectToAction("Login", "Usuario");
            }
            var tieneAcceso = permisos.Any(p => p.Accesos == 182);
            if (!tieneAcceso)
            {
                return RedirectToAction("Error", "Home");
            }

            return View();
        }

        public ActionResult Reportes()
        {
            var permisos = Session["PermissionsCode"] as List<Permiso>;
            if (permisos == null)
            {
                return RedirectToAction("Login", "Usuario");
            }
            var tieneAcceso = permisos.Any(p => p.Accesos == 59);
            if (!tieneAcceso)
            {
                return RedirectToAction("Error", "Home");
            }

            return View();
        }



        public ActionResult SolicitudPedidos()
        {

            var permisos = Session["PermissionsCode"] as List<Permiso>;
            var tieneAcceso = permisos.Any(p => p.Accesos == 23);
            if (!tieneAcceso)
            {
                return RedirectToAction("Error", "Home");
            }


            return View();
        }

        public ActionResult EntregaPedidos()
        {

            var permisos = Session["PermissionsCode"] as List<Permiso>;
            var tieneAcceso = permisos.Any(p => p.Accesos == 24 || p.Accesos == 184);
            if (!tieneAcceso)
            {
                return RedirectToAction("Error", "Home");
            }

            return View();
        }


        //INGRESOS
        #region INGRESOS
        [HttpGet]
        public JsonResult ListarIngresos()
        {
            List<Ingresos> oLista = new List<Ingresos>();
            oLista = new CN_Ingresos().Listar();
            return Json(new {data= oLista}, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GuardarIngreso(Ingresos objeto)
        {
            object resultado;
            string mensaje = string.Empty;

            if (objeto.IdIngreso == 0)
            {
                resultado = new CN_Ingresos().Registrar(objeto, out mensaje);
            }
            else
            {
                resultado = new CN_Ingresos().Editar(objeto, out mensaje);
            }
            return Json(new { resultado = resultado, mensaje = mensaje }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        //RUBROS
        #region RUBROS
        [HttpGet]
        public JsonResult ListarRubros(int? allowedRubroId = null) // Parámetro opcional
        {
            // 1. Obtener la lista completa desde la capa de negocio
            List<Rubros> oListaCompleta = new CN_Rubros().Listar();

            // --- INICIO CAMBIO: Filtrar por Activo ---
            // 2. Filtrar la lista para incluir solo los rubros donde Activo es true
            //    (Asegúrate de que tu clase Rubros tenga la propiedad booleana 'Activo')
            IEnumerable<Rubros> rubrosActivosQuery = oListaCompleta.Where(r => r.Activo);
            // --- FIN CAMBIO: Filtrar por Activo ---

            // 3. Aplicar el filtro opcional por Rubro Específico (si se pasó el parámetro)
            //    Este filtro se aplica sobre la lista que ya contiene solo los activos.
            if (allowedRubroId.HasValue)
            {
                rubrosActivosQuery = rubrosActivosQuery.Where(r => r.IdRubro == allowedRubroId.Value);
            }

            // 4. Convertir el resultado final a una lista
            List<Rubros> oListaFiltradaFinal = rubrosActivosQuery.ToList();

            // 5. Devolver la lista filtrada
            return Json(new { data = oListaFiltradaFinal }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GuardarRubros(Rubros objeto)
        {
            object resultado;
            string mensaje = string.Empty;

            if (objeto.IdRubro == 0)
            {
                resultado = new CN_Rubros().Registrar(objeto, out mensaje);
            }
            else
            {
                resultado = new CN_Rubros().Editar(objeto, out mensaje);
            }
            return Json(new { resultado = resultado, mensaje = mensaje }, JsonRequestBehavior.AllowGet);
        }
            #endregion

        //PROVEEDORES
        #region PROVEEDORES
        [HttpGet]
        public JsonResult ListarProveedores()
        {
            List<Proveedores> oLista = new List<Proveedores>();
            oLista = new CN_Proveedores().Listar();
            return Json(new { data = oLista }, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public JsonResult GuardarProveedor(Proveedores objeto)
        {
            object resultado;
            string mensaje = string.Empty;

            if (objeto.IdProveedor == 0)
            {
                resultado = new CN_Proveedores().Registrar(objeto, out mensaje);
            }
            else
            {
                resultado = new CN_Proveedores().Editar(objeto, out mensaje);
            }
            return Json(new { resultado = resultado, mensaje = mensaje }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult EliminarProveedor(int id)
        {
            bool respuesta = false;
            string mensaje = string.Empty;
            respuesta = new CN_Proveedores().Eliminar(id, out mensaje);
            return Json(new { resultado = respuesta, mensaje = mensaje }, JsonRequestBehavior.AllowGet);

        }
        #endregion

        //TIPOS
        #region TIPOS
        [HttpGet]
        public JsonResult ListarTipos()
        {
            List<Tipos> oLista = new List<Tipos>();
            oLista = new CN_Tipos().Listar();
            return Json(new { data = oLista }, JsonRequestBehavior.AllowGet);
        }
        
        [HttpGet]
        public JsonResult ListarTiposPorRubro(int idRubro)
        {
            var tipos = new CN_Tipos().ListarporIDRubro(idRubro);
            return Json(tipos, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GuardarTipos(Tipos objeto)
        {
            object resultado;
            string mensaje = string.Empty;

            if (objeto.IdTipo == 0)
            {
                resultado = new CN_Tipos().Registrar(objeto, out mensaje);
            }
            else
            {
                resultado = new CN_Tipos().Editar(objeto, out mensaje);
            }
            return Json(new { resultado = resultado, mensaje = mensaje }, JsonRequestBehavior.AllowGet);
        }
        #endregion


        //PRODUCTOS
        #region PRODUCTOS
        [HttpGet]
        public JsonResult ListarProductos()
        {
            List<Productos> oLista = new List<Productos>();
            oLista = new CN_Productos().Listar();
            return Json(new { data = oLista }, JsonRequestBehavior.AllowGet);
        }
    

        [HttpGet]
        public JsonResult ListarProductosPorTipo(int idTipo, int idRubro)
        {
            var productos = new CN_Productos().ListarporIDTipos(idTipo, idRubro);
            return Json(productos, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult ListarProductosporCI(string idCodigo)
        {
            var productos = new CN_Productos().ListarProductosporCI(idCodigo);
            return Json(productos, JsonRequestBehavior.AllowGet);
        }



        [HttpPost]

        public JsonResult GuardarProductos(Productos objeto)
        {
            object resultado;
            string mensaje = string.Empty;

            if (objeto.IdProducto == 0)
            {
                resultado = new CN_Productos().Registrar(objeto, out mensaje);
            }
            else
            {
                resultado = new CN_Productos().Editar(objeto, out mensaje);
            }
            return Json(new { resultado = resultado, mensaje = mensaje }, JsonRequestBehavior.AllowGet);
        }

      


        #endregion

        //EGRESOS

        #region Egresos
        [HttpGet]
        public JsonResult ListarEgresos()
        {
            List<Egresos> oLista = new List<Egresos>();
            oLista = new CN_Egresos().Listar();
            return Json(new { data = oLista }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GuardarEgresos(Egresos objeto)
        {
            object resultado;
            string mensaje = string.Empty;

            if (objeto.IdEgreso == 0)
            {
                resultado = new CN_Egresos().Registrar(objeto, out mensaje);
            }
            else
            {
                resultado = new CN_Egresos().Editar(objeto, out mensaje);
            }
            return Json(new { resultado = resultado, mensaje = mensaje }, JsonRequestBehavior.AllowGet);
        }
        #endregion



    }
}
