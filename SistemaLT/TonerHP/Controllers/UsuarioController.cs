using CapaEntidad;
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
        private const string ApiBaseUrl = "http://10.4.51.49/SI_Apis/";

        public UsuarioController()
        {
            _httpClient = new HttpClient();
            _httpClient.Timeout = TimeSpan.FromSeconds(30);
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(Acceso acceso)
        {
            if (!ModelState.IsValid) return View(acceso);

            try
            {
                // 1. Autenticación inicial
                var authResult = await AutenticarUsuario(acceso);
                if (!authResult.Success) return View(acceso);

                // 2. Obtener datos completos del usuario
                var datosUsuario = await ObtenerDatosUsuario(acceso.usuario, authResult.UserId);
                if (datosUsuario == null) return View(acceso);

                // 3. Obtener y validar permisos
                var permisos = await ObtenerPermisosUsuario(datosUsuario.CodigoUnico);
                if (!ValidarPermisos(permisos)) return View(acceso);

                // 4. Configurar sesión y autenticación
                ConfigurarSesion(datosUsuario, authResult.UserId, permisos);
                FormsAuthentication.SetAuthCookie(acceso.usuario, false);

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error crítico: {ex.Message}");
                return View(acceso);
            }
        }

        private async Task<AuthResult> AutenticarUsuario(Acceso acceso)
        {
            var result = new AuthResult();

            var response = await _httpClient.PostAsync(
                $"{ApiBaseUrl}api/access/loginuser",
                new StringContent(JsonConvert.SerializeObject(acceso), Encoding.UTF8, "application/json"));

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Error de autenticación: Credenciales inválidas");
                return result;
            }

            var content = await response.Content.ReadAsStringAsync();
            var accesoResultado = JsonConvert.DeserializeObject<AccesoResultado>(content);

            if (accesoResultado.result <= 0)
            {
                ModelState.AddModelError("", accesoResultado.message);
                return result;
            }

            result.Success = true;
            result.UserId = accesoResultado.result;
            return result;
        }

        private async Task<UsuarioDatos> ObtenerDatosUsuario(string UserId, int CodigoUnico)
        {
            var response = await _httpClient.GetAsync(
                $"{ApiBaseUrl}home/buscardatosdelusuario?" +
                $"usuario={HttpUtility.UrlEncode(UserId)}&" +
                $"codigounico={CodigoUnico}");

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Error obteniendo datos del usuario");
                return null;
            }

            var content = await response.Content.ReadAsStringAsync();
            var datos = JsonConvert.DeserializeObject<List<UsuarioDatos>>(content);

            var datosUsuario = datos?.FirstOrDefault();
            //if (datosUsuario?.CodigoArea <= 0 || datosUsuario?.CodigoSector <= 0)
            //{
            //    ModelState.AddModelError("", "Configuración de área/sector inválida");
            //    return null;
            //}

            return datosUsuario;
        }

        private async Task<List<Permiso>> ObtenerPermisosUsuario(int CodigoUnico)
        {
            var response = await _httpClient.GetAsync(
                $"{ApiBaseUrl}home/BuscarPermisosdelUsuario?codigousuario={CodigoUnico}");

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Error obteniendo permisos de usuario");
                return null;
            }

            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<Permiso>>(content);
        }

        private bool ValidarPermisos(List<Permiso> permisos)
        {
            var permisosRequeridos = new[] { 23, 24, 25, 59 };
            if (permisos?.Any(p => permisosRequeridos.Contains(p.Accesos)) != true)
            {
                ModelState.AddModelError("", "No tiene los permisos necesarios");
                return false;
            }
            return true;
        }

        private void ConfigurarSesion(UsuarioDatos datosUsuario, int accessCode, List<Permiso> permisos)
        {
            Session["CodigoUnico"] = datosUsuario.CodigoUnico;
            Session["CodigoArea"] = datosUsuario.CodigoArea;
            Session["CodigoSector"] = datosUsuario.CodigoSector;
            Session["NombreArea"] = datosUsuario.NombreArea;
            Session["NombreSector"] = datosUsuario.NombreSector;
            Session["AccesCode"] = accessCode;
            Session["PermissionsCode"] = permisos;
        }

        public ActionResult CerrarSesion()
        {
            FormsAuthentication.SignOut();
            Session.Abandon();
            return RedirectToAction("Login", "Usuario");
        }

        // Clases auxiliares
        private class AuthResult
        {
            public bool Success { get; set; }
            public int UserId { get; set; }
        }

        [Authorize]
        public async Task<ActionResult> ActualizarDatosSesion()
        {
            var datosUsuario = await ObtenerDatosUsuario(User.Identity.Name, (int)Session["CodigoUnico"]);
            ConfigurarSesion(datosUsuario, (int)Session["AccesCode"], (List<Permiso>)Session["PermissionsCode"]);
            return Redirect(Request.UrlReferrer?.ToString() ?? Url.Action("Index", "Home"));
        }
    }
}