using CapaEntidad;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;


namespace TonerHP.Controllers
{
    public class UsuarioController : Controller
    {
        public async Task<ActionResult> Acceso()
        {
            var httpClient = new HttpClient();
            var json = await httpClient.GetStringAsync("http://10.4.51.49/SI_Apis/access/loginuser");
            var usuarioList = JsonConvert.DeserializeObject<List<Acceso>>(json);
            return View(usuarioList);
        }
        public async Task<ActionResult> Permiso()
        {
            var httpClient = new HttpClient();
            var json = await httpClient.GetStringAsync("http://10.4.51.49/SI_Apis/home/BuscarPermisosdelUsuario?codigousuario=2");
            var PermisoList = JsonConvert.DeserializeObject<List<Permiso>>(json);
            return View(PermisoList);
        }

        public ActionResult Login() 
        { 
            return View(); 
        }
    }
}