using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using CapaEntidad;
using CapaNegocio;
using Newtonsoft.Json;


namespace TonerHP.Controllers
{
    public class TonerController : Controller
    {
        // GET: Toner

        public ActionResult _BusAvan()
        {
            return PartialView("_BusAvan");
        }

        public ActionResult Proveedores()
        {
            //var userAccessCode = Session["AccesCode"] as int?;
            //if (userAccessCode == null || (userAccessCode != 23 && userAccessCode != 24))
            //{
            //    return RedirectToAction("Index", "Home");
            //}

            return View();
        }

        public ActionResult Tipos()
        {
            //var userAccessCode = Session["AccesCode"] as int?;
            //if (userAccessCode == null || (userAccessCode != 23 && userAccessCode != 24))
            //{
            //    return RedirectToAction("Index", "Home");
            //}

            //var userAccessCode = Session["AccesCode"] as int?;

            return View();
        }

        public ActionResult Productos()
        {
            //var userAccessCode = Session["AccesCode"] as int?;
            //if (userAccessCode == null || (userAccessCode != 23 && userAccessCode != 24))
            //{
            //    return RedirectToAction("Index", "Home");
            //}

            return View();
        }
        [Authorize]
        public ActionResult Ingresos()
        {
            var permisos = Session["PermissionsCode"] as List<Permiso>;
            var tieneAcceso = permisos.Any(p => p.Accesos == 24);
            if (!tieneAcceso)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        [Authorize]
        public ActionResult Egresos()
        {

            var permisos = Session["PermissionsCode"] as List<Permiso>;
            var tieneAcceso = permisos.Any(p => p.Accesos == 25);
            if (!tieneAcceso)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        public ActionResult Rubros()
        {
            //var userAccessCode = Session["AccesCode"] as int?;
            //if (userAccessCode == null || (userAccessCode != 23 && userAccessCode != 24))
            //{
            //    return RedirectToAction("Index", "Home");
            //}

            return View();
        }

        public ActionResult SolicitudPedidos()
        {

            //var userAccessCode = Session["AccesCode"] as int?;
            //if (userAccessCode == null || (userAccessCode != 23 && userAccessCode != 24))
            //{
            //    return RedirectToAction("Index", "Home");
            //}

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
        public JsonResult ListarRubros()
        {
            List<Rubros> oLista = new List<Rubros>();
            oLista = new CN_Rubros().Listar();
            return Json(new { data = oLista }, JsonRequestBehavior.AllowGet);
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
        public JsonResult ListarProductosPorTipo(int idTipo)
        {
            var productos = new CN_Productos().ListarporIDTipos(idTipo);
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


        //PEDIDOS

        #region Pedidos
        [HttpGet]
        public JsonResult ListarPedidos()
        {
            List<SolicitudPedidos> oLista = new List<SolicitudPedidos>();
            oLista = new CN_SolicitudPedidos().Listar();
            return Json(new { data = oLista }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult GuardarPedidos(SolicitudPedidos objeto)
        {
            try
            {
                // Validaciones básicas
                if (objeto == null)
                {
                    return Json(new { resultado = false, mensaje = "No se recibieron datos del pedido" }, JsonRequestBehavior.AllowGet);
                }

                // Validar campos requeridos
                if (objeto.CantidadPedida <= 0)
                {
                    return Json(new { resultado = false, mensaje = "La cantidad pedida debe ser mayor a 0" }, JsonRequestBehavior.AllowGet);
                }

                // Establecer fechas
                if (objeto.IdSolicitud == 0) // Es un nuevo registro
                {
                    //objeto.FechaPedido = DateTime.Now;
                    if (objeto.CantidadEntregada > 0) // Si hay cantidad entregada
                    {
                        //objeto.FechaEntrega = DateTime.Now;
                    }
                }

                // Variable para el resultado
                object resultado;
                string mensaje = string.Empty;

                // Registrar o editar según corresponda
                if (objeto.IdSolicitud == 0)
                {
                    System.Diagnostics.Debug.WriteLine("Registrando nuevo pedido...");
                    resultado = new CN_SolicitudPedidos().Registrar(objeto, out mensaje);

                    if (resultado != null && Convert.ToInt32(resultado) > 0)
                    {
                        System.Diagnostics.Debug.WriteLine($"Pedido registrado exitosamente. ID: {resultado}");
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"Error al registrar pedido: {mensaje}");
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"Editando pedido ID: {objeto.IdSolicitud}");
                    resultado = new CN_SolicitudPedidos().Editar(objeto, out mensaje);

                    if (Convert.ToBoolean(resultado))
                    {
                        System.Diagnostics.Debug.WriteLine("Pedido editado exitosamente");
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"Error al editar pedido: {mensaje}");
                    }
                }

                // Validar el resultado antes de devolverlo
                if (resultado == null)
                {
                    return Json(new
                    {
                        resultado = false,
                        mensaje = "Error al procesar la solicitud"
                    }, JsonRequestBehavior.AllowGet);
                }

                return Json(new
                {
                    resultado = resultado,
                    mensaje = string.IsNullOrEmpty(mensaje) ? "Operación exitosa" : mensaje
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error en GuardarPedidos: {ex.Message}");
                return Json(new
                {
                    resultado = false,
                    mensaje = "Se produjo un error al procesar la solicitud: " + ex.Message
                }, JsonRequestBehavior.AllowGet);
            }
        }
#endregion
    }
}
