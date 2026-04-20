using BarberiaOnlineNueva.Models;
using BarberiaOnlineNueva.Services;
using Microsoft.AspNetCore.Mvc;

namespace BarberiaOnlineNueva.Controllers
{
    public class CuentaController : Controller
    {
        private readonly UsuarioService _usuarioService;
        private readonly EmailService _emailService;

        public CuentaController(UsuarioService usuarioService, EmailService emailService)
        {
            _usuarioService = usuarioService;
            _emailService = emailService;
        }


        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(Usuario usuario)
        {
            var user = _usuarioService.ValidarLogin(usuario.Correo, usuario.Contrasena);

            if (user == null)
            {
                ViewBag.Mensaje = "Correo o contraseña incorrectos.";
                return View();
            }

            HttpContext.Session.SetInt32("IdUsuario", user.IdUsuario);
            HttpContext.Session.SetString("RolUsuario", user.NombreRol?.Trim() ?? "");
            HttpContext.Session.SetString("NombreUsuario", user.Nombre ?? "");

            string rol = user.NombreRol.Trim().ToLower();

            if (rol == "administrador")
            {
                return RedirectToAction("DashboardAdmin", "Panel");
            }
            if (rol == "barbero")
            {
                return RedirectToAction("DashboardBarbero", "Panel");
            }

            return RedirectToAction("DashboardCliente", "Panel");
        }

        [HttpGet]
        public IActionResult Registro()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Registro(Usuario usuario)
        {
            ModelState.Remove("NombreRol");
            if (!ModelState.IsValid)
            {
                return View(usuario);
            }

            usuario.IdRol = 2;

            _usuarioService.Registrar(usuario);

            TempData["Mensaje"] = "Usuario registrado correctamente";

            return RedirectToAction("Login");
        }


        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Recuperar()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Recuperar(string correo)
        {
            var usuario = _usuarioService.ObtenerPorCorreo(correo);

            if (usuario == null)
            {
                ViewBag.Error = "Correo no encontrado";
                return View();
            }

            var token = _usuarioService.GenerarToken(usuario.IdUsuario);

            
            string link = $"https://localhost:7212/Cuenta/ResetPassword?token={token}";

            _emailService.EnviarCorreo(correo, "Recuperar contraseña", link);

            return View();
        }

        public IActionResult ResetPassword(string token)
        {
            ViewBag.Token = token;
            return View();
        }

        [HttpPost]
        public IActionResult ResetPassword(string token, string nuevaPassword)
        {
            var idUsuario = _usuarioService.ValidarToken(token);

            if (idUsuario == null)
            {
                return Content("Token inválido o expirado");
            }

            _usuarioService.ActualizarPassword(idUsuario.Value, nuevaPassword);

            TempData["Mensaje"] = "Contraseña actualizada correctamente";
            return RedirectToAction("Login", "Cuenta");
        }
    }
}