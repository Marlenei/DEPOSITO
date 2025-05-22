using CapaEntidad;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace TonerHP.Controllers
{
    public class HomeController : Controller
    {
        [Authorize]
        public ActionResult Index()
        {

            var permisos = Session["PermissionsCode"] as List<Permiso> ?? new List<Permiso>();

            ViewBag.TieneAccesoIngresos = permisos.Any(p => p.Accesos == 24);
            ViewBag.TieneAccesoEgresos = permisos.Any(p => p.Accesos == 25);
            return View();

        }

        [Authorize]
        public ActionResult Error()
            {
                return View();
            }

        public ActionResult Prueba()
        {
            return View();
        }


       
    }
}