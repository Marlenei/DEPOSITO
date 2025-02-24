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
        public JsonResult GuardarPedidos(SolicitudPedidos objeto, CN_SolicitudPedidos _cnSolicitudPedidos)
        {
            try
            {
                // Validación básica del objeto
                if (objeto == null)
                {
                    return Json(new { resultado = false, mensaje = "No se recibieron datos del pedido" });
                }

                // Validar sesión activa
                if (Session["CodigoArea"] == null || Session["CodigoSector"] == null)
                {
                    return Json(new { resultado = false, mensaje = "Sesión expirada. Vuelva a iniciar sesión" });
                }

                // Asignar códigos desde la sesión
                objeto.CodigoArea = (int)Session["CodigoArea"];
                objeto.CodigoSector = (int)Session["CodigoSector"];

                // Validaciones de negocio
                if (objeto.CantidadPedida <= 0)
                {
                    return Json(new { resultado = false, mensaje = "La cantidad pedida debe ser mayor a 0" });
                }

                if (objeto.oProductos?.IdProducto == 0)
                {
                    return Json(new { resultado = false, mensaje = "Seleccione un producto válido" });
                }

                // Lógica de fechas
                if (objeto.IdSolicitud == 0) // Nuevo pedido
                {
                    objeto.FechaPedido = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                    if (objeto.CantidadEntregada > 0)
                    {
                        objeto.FechaEntrega = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    }
                }

                string mensaje = string.Empty;
                string nroPedidoGenerado = string.Empty;
                bool resultadoOperacion;

                // Registrar o Editar
                if (objeto.IdSolicitud == 0)
                {
                    System.Diagnostics.Debug.WriteLine("Registrando nuevo pedido...");
                    int idGenerado = _cnSolicitudPedidos.Registrar(objeto, out mensaje, out nroPedidoGenerado);
                    resultadoOperacion = idGenerado > 0;

                    if (resultadoOperacion)
                    {
                        System.Diagnostics.Debug.WriteLine($"Pedido registrado. ID: {idGenerado}, Nro: {nroPedidoGenerado}");
                        objeto.NroPedido = nroPedidoGenerado; // Asignar número generado
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"Editando pedido ID: {objeto.IdSolicitud}");
                    resultadoOperacion = _cnSolicitudPedidos.Editar(objeto, out mensaje);
                }

                return Json(new
                {
                    resultado = resultadoOperacion,
                    mensaje = resultadoOperacion ? "Operación exitosa" : mensaje,
                    nroPedido = nroPedidoGenerado // Devolver número generado
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error crítico: {ex.ToString()}");
                return Json(new
                {
                    resultado = false,
                    mensaje = $"Error interno: {ex.Message}"
                });
            }
        }

        #endregion
    }
}
