using CapaEntidad;
using CapaNegocio;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        private class AuthResult
        {
            public bool Success { get; set; }
            public int UserId { get; set; }
        }

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
                    Session["Usuario"] = acceso.usuario;
                    Session["AccesCode"] = accesoResultado.result;


                    //API de PERMISOS
                    var permisoResponse = await _httpClient.GetAsync($"http://10.4.51.49/SI_Apis/home/BuscarPermisosdelUsuario?codigousuario={accesoResultado.result}");
                    if (permisoResponse.IsSuccessStatusCode)
                    {
                        var permisoJson = await permisoResponse.Content.ReadAsStringAsync();
                        var permisos = JsonConvert.DeserializeObject<List<Permiso>>(permisoJson);

                        var tieneAcceso = permisos.Any(p => p.Accesos == 23 || p.Accesos == 24 || p.Accesos == 25 || p.Accesos == 59);
                        if (tieneAcceso)
                        {
                            Session["PermissionsCode"] = permisos;

                            var areasectorResponse = await _httpClient.GetAsync($"http://10.4.51.49/SI_Apis/home/buscardatosdelusuario?usuario={acceso.usuario}&codigounico={accesoResultado.result}");
                            if (areasectorResponse.IsSuccessStatusCode)
                            {
                                var areasectorJson = await areasectorResponse.Content.ReadAsStringAsync();
                                var usuarioDatos = JsonConvert.DeserializeObject<List<UsuarioDatos>>(areasectorJson);

                                // Asegúrate de que la lista no esté vacía
                                if (usuarioDatos != null && usuarioDatos.Count > 0)
                                {
                                    var codArea = usuarioDatos[0].CodArea;
                                    var codSector = usuarioDatos[0].CodSector;

                                    // Obtener nombres usando la capa de datos
                                    var nombreArea = new CN_SolicitudPedidos().ObtenerNombreArea(codArea);
                                    var nombreSector = new CN_SolicitudPedidos().ObtenerNombreSector(codArea, codSector);


                                    // Guardar CodArea y CodSector en la sesión
                                    Session["CodArea"] = usuarioDatos[0].CodArea;
                                    Session["NombreArea"] = nombreArea ?? "Área no definida";
                                    Session["CodSector"] = usuarioDatos[0].CodSector;
                                    Session["NombreSector"] = nombreSector ?? "Sector no definido";



                                    Debug.WriteLine("CodArea: " + Session["CodArea"]);
                                    Debug.WriteLine("CodSector: " + Session["CodSector"]);
                                    // Verificar si CodArea y CodSector son los esperados
                                    //if (Session["CodArea"].ToString() != "3" || Session["CodSector"].ToString() != "9")
                                    //{
                                    //    ModelState.AddModelError("", "Acceso denegado: Código de área o sector incorrecto.");
                                    //    return View();
                                    //}

                                }
                            }
                            else
                            {
                                ModelState.AddModelError("", "Error al obtener datos del usuario: " + areasectorResponse.ReasonPhrase);
                                return View();
                            }




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

        public JsonResult ObtenerDatosUbicacion()
        {
            var codArea = Session["CodArea"] as int? ?? 0;
            var codSector = Session["CodSector"] as int? ?? 0;

            var nombreArea = new CN_SolicitudPedidos().ObtenerNombreArea(codArea);
            var nombreSector = new CN_SolicitudPedidos().ObtenerNombreSector(codArea, codSector);

            return Json(new
            {
                nombreArea = nombreArea ?? "Área no definida",
                nombreSector = nombreSector ?? "Sector no definido"
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CerrarSesion()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "Usuario");
        }

        public ActionResult CerrarSesion()
        {
            FormsAuthentication.SignOut();
            Session.Abandon();
            return RedirectToAction("Login", "Usuario");
        }
    }
}