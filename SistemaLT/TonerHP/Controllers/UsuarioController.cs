using CapaEntidad;
using CapaNegocio;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;


namespace TonerHP.Controllers
{
    
    public class UsuarioController : Controller
    {

        private readonly HttpClient _httpClient;

        public UsuarioController()
        {
            _httpClient = new HttpClient(); 
        }

        public UsuarioController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public ActionResult Login()
        {
            return View(); 
        }
        [HttpPost]
        public async Task<ActionResult> Login(Acceso acceso)
        {
            if (!ModelState.IsValid)
            {
                return View(); 
            }
            // Autenticación del usuario, llamada a API de USUARIOS
            var json = JsonConvert.SerializeObject(acceso);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("http://10.4.51.49/SI_Apis/api/access/loginuser", content);

            if (response.IsSuccessStatusCode)
            {
                var responseJson = await response.Content.ReadAsStringAsync();
                var accesoResultado = JsonConvert.DeserializeObject<AccesoResultado>(responseJson);

                if (accesoResultado.result > 0)
                {
                    //API de PERMISOS
                    var permisoResponse = await _httpClient.GetAsync($"http://10.4.51.49/SI_Apis/home/BuscarPermisosdelUsuario?codigousuario={accesoResultado.result}");
                    if (permisoResponse.IsSuccessStatusCode)
                    {
                        var permisoJson = await permisoResponse.Content.ReadAsStringAsync();
                        var permisos = JsonConvert.DeserializeObject<List<Permiso>>(permisoJson);
                        Session["AccesCode"] = accesoResultado.result;
                        var tieneAcceso = permisos.Any(p => p.Accesos == 23 || p.Accesos == 24 || p.Accesos == 25 || p.Accesos == 59);
                        if (tieneAcceso)
                        {
                            Session["PermissionsCode"] = permisos;
                            //Session["AccesCode"] = accesoResultado.result;
                            FormsAuthentication.SetAuthCookie(acceso.usuario, false); 
                            return RedirectToAction("Index", "Home");
                        }
                        else
                        {
                            ModelState.AddModelError("", "No tienes acceso a esta vista.");
                            return View(); 
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "Error al obtener permisos: " + permisoResponse.ReasonPhrase);
                        return View(); 
                    }
                }

                else
                {
                    ModelState.AddModelError("", accesoResultado.message);
                    return View();
                }
            }

            else
            {
                ModelState.AddModelError("", "Error al acceder a la API: " + response.ReasonPhrase);
                return View();
            }

        }
        public ActionResult CerrarSesion()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "Usuario");
        }
    }
}