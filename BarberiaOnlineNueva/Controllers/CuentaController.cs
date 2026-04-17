using BarberiaOnlineNueva.Models;
using BarberiaOnlineNueva.Services;
using Microsoft.AspNetCore.Mvc;

namespace BarberiaOnlineNueva.Controllers
{
    public class CuentaController : Controller
    {
        private readonly UsuarioService _usuarioService;

        public CuentaController(UsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
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
            HttpContext.Session.SetString("RolUsuario", user.NombreRol.Trim());

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
    }
}