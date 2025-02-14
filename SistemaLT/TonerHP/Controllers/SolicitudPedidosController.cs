using CapaDatos;
using CapaEntidad;
using CapaNegocio;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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
        private readonly CD_SolicitudPedidos _cdSolicitudPedidos;
        private readonly CN_SolicitudPedidos _cnSolicitudPedidos;
        private readonly HttpClient _httpClient;

      

        //public SolicitudPedidosController()
        //{
        //    _httpClient = new HttpClient();
        //    _cdSolicitudPedidos = new CD_SolicitudPedidos(_httpClient);
        //    _cnSolicitudPedidos = new CN_SolicitudPedidos(_httpClient);
        //}
        [HttpGet]


        [Authorize]
        public ActionResult SolicitudPedidos()
        {
            if (Session["CodigoArea"] == null || Session["CodigoSector"] == null)
            {
                return RedirectToAction("Login", "Usuario");
            }

            int codigoArea = (int)Session["CodigoArea"];
            int codigoSector = (int)Session["CodigoSector"];

            try
            {
                // 1. Obtener listas completas de áreas y sectores
                var listaAreas = _cdSolicitudPedidos.ObtenerAreas() ?? new List<UsuarioDatos>();
                var listaSectores = _cdSolicitudPedidos.ObtenerSectoresPorArea(codigoArea) ?? new List<UsuarioDatos>();

                // 2. Asignar al ViewBag
                ViewBag.Areas = listaAreas; // <-- Esto falta en tu código actual
                ViewBag.Sectores = listaSectores; // <-- Esto falta en tu código actual

                // 3. Datos para mostrar en la vista
                var area = listaAreas.FirstOrDefault(a => a.CodigoArea == codigoArea);
                var sector = listaSectores.FirstOrDefault(s => s.CodigoSector == codigoSector);

                ViewBag.NombreArea = area?.NombreArea ?? "Área no reconocida";
                ViewBag.NombreSector = sector?.NombreSector ?? "Sector no reconocido";
                ViewBag.CodigoArea = codigoArea;
                ViewBag.CodigoSector = codigoSector;

                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error inicializando sesión: {ex.Message}";
                ViewBag.Areas = new List<UsuarioDatos>(); // Asegurar que no sea null
                ViewBag.Sectores = new List<UsuarioDatos>();
                return View();
            }
        }
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> ObtenerDatosUsuario(Acceso acceso)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return View("SolicitudPedidos");
        //    }

        //    try
        //    {
        //        // 1. Obtener usuario de sesión
        //        var usuario = User.Identity.Name;
        //        if (string.IsNullOrEmpty(usuario))
        //        {
        //            ModelState.AddModelError("", "Sesión no autenticada");
        //            return RedirectToAction("Login", "Usuario");
        //        }

        //        // 2. Llamar a la API
        //        var response = await _httpClient.GetAsync(
        //            $"http://10.4.51.49/SI_Apis/home/buscardatosdelusuario?" +
        //            $"usuario={HttpUtility.UrlEncode(usuario)}&" +
        //            $"codigounico={acceso.CodigoUnico}"
        //        );

        //        if (!response.IsSuccessStatusCode)
        //        {
        //            ModelState.AddModelError("", $"Error API: {response.StatusCode} - {response.ReasonPhrase}");
        //            return View("SolicitudPedidos");
        //        }

        //        // 3. Procesar respuesta
        //        var responseContent = await response.Content.ReadAsStringAsync();
        //        var resultadoApi = JsonConvert.DeserializeObject<List<UsuarioDatos>>(responseContent);

        //        if (resultadoApi == null || !resultadoApi.Any())
        //        {
        //            ModelState.AddModelError("", "La API no devolvió datos válidos");
        //            return View("SolicitudPedidos");
        //        }

        //        var datosUsuario = resultadoApi.First();

        //        // 4. Validar estructura básica
        //        if (datosUsuario.CodigoArea <= 0 || datosUsuario.CodigoSector <= 0)
        //        {
        //            ModelState.AddModelError("", "El usuario no tiene asignación válida de área/sector");
        //            return View("SolicitudPedidos");
        //        }

        //        // 5. Almacenar en sesión sin validar en BD
        //        Session["CodigoArea"] = datosUsuario.CodigoArea;
        //        Session["CodigoSector"] = datosUsuario.CodigoSector;
        //        Session["DatosUsuario"] = datosUsuario;

        //        return RedirectToAction("SolicitudPedidos");
        //    }
        //    catch (Exception ex)
        //    {
        //        ModelState.AddModelError("", $"Error crítico: {ex.Message}");
        //        return View("SolicitudPedidos");
        //    }
        //}

        //[HttpGet]
        //public JsonResult ListarPedidos()
        //{
        //    try
        //    {
        //        List<SolicitudPedidos> lista = new CN_SolicitudPedidos().Listar();

        //        // Transformamos los datos para asegurar un formato de fecha consistente
        //        var resultado = lista.Select(p => new
        //        {
        //            IdSolicitud = p.IdSolicitud,
        //            oProductos = p.oProductos,
        //            CantidadPedida = p.CantidadPedida,
        //            CantidadEntregada = p.CantidadEntregada,
        //            FechaPedido = p.FechaPedido.ToString("dd/MM/yyyy"),
        //            // Verificamos si FechaEntrega es el valor por defecto de DateTime
        //            FechaEntrega = p.FechaEntrega == DateTime.MinValue ? "" : p.FechaEntrega.ToString("dd/MM/yyyy"),
        //            IdUsuarioPedido = p.IdUsuarioPedido,
        //            CodigoArea = p.CodigoArea,
        //            CodigoSector = p.CodigoSector,
        //            IdUsuarioEntrega = p.IdUsuarioEntrega,
        //            Observaciones = p.Observaciones,
        //            NroPedido = p.NroPedido,
        //            Visado = p.Visado
        //        }).ToList();

        //        System.Diagnostics.Debug.WriteLine($"Cantidad de registros encontrados: {lista.Count}");

        //        // Agregar logging para verificar el formato de las fechas
        //        if (resultado.Any())
        //        {
        //            System.Diagnostics.Debug.WriteLine($"Ejemplo de fecha pedido: {resultado.First().FechaPedido}");
        //            System.Diagnostics.Debug.WriteLine($"Ejemplo de fecha entrega: {resultado.First().FechaEntrega}");
        //        }

        //        return Json(new { data = resultado }, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {
        //        System.Diagnostics.Debug.WriteLine($"Error en ListarPedidos: {ex.Message}");
        //        return Json(new { error = ex.Message }, JsonRequestBehavior.AllowGet);
        //    }
        //}

        [HttpPost]
        public JsonResult GuardarPedidos(SolicitudPedidos objeto)
        {
            try
            {
                // Validar sesión activa
                if (Session["CodigoArea"] == null || Session["CodigoSector"] == null)
                {
                    return Json(new { resultado = false, mensaje = "Sesión expirada" });
                }

                // Asignar códigos automáticamente desde la sesión
                objeto.CodigoArea = (int)Session["CodigoArea"];
                objeto.CodigoSector = (int)Session["CodigoSector"];

                string mensaje = string.Empty;
                bool resultado;

                if (objeto.IdSolicitud == 0)
                {
                    int id = _cnSolicitudPedidos.Registrar(objeto, out mensaje);
                    resultado = id > 0;
                }
                else
                {
                    resultado = _cnSolicitudPedidos.Editar(objeto, out mensaje);
                }

                return Json(new
                {
                    resultado = resultado,
                    mensaje = string.IsNullOrEmpty(mensaje) ? "Operación exitosa" : mensaje
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

       
        public ActionResult CerrarSesion()
        {
            FormsAuthentication.SignOut();
            Session.Abandon();
            return RedirectToAction("Login", "Usuario");
        }
    }
}