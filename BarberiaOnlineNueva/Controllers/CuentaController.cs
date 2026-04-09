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

            if (user != null)
            {
                HttpContext.Session.SetInt32("IdUsuario", user.IdUsuario);

               
                HttpContext.Session.SetString("RolUsuario", user.NombreRol);

                if (user.NombreRol == "Administrador")
                    return RedirectToAction("DashboardAdmin", "Panel");
                else
                    return RedirectToAction("DashboardCliente", "Panel");
            }

            ViewBag.Mensaje = "Correo o contraseña incorrectos.";
            return View();
        }

        [HttpGet]
        public IActionResult Registro()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Registro(Usuario usuario)
        {
            if (!ModelState.IsValid)
            {
                return View(usuario);
            }

            _usuarioService.Registrar(usuario);

            TempData["Mensaje"] = "Usuario registrado correctamente.";
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