using Microsoft.AspNetCore.Mvc;

namespace BarberiaOnlineNueva.Controllers
{
    public class PanelController : Controller
    {
        public IActionResult DashboardAdmin()
        {
            string rol = HttpContext.Session.GetString("Rol");

            Console.WriteLine("ROL ACTUAL ADMIN: " + rol);

            if (string.IsNullOrEmpty(rol))
            {
                return RedirectToAction("Login", "Cuenta");
            }

            if (rol.Trim() != "Administrador")
            {
                return RedirectToAction("Login", "Cuenta");
            }

            return View();
        }
        public IActionResult DashboardCliente()
        {
            string rol = HttpContext.Session.GetString("RolUsuario");

            Console.WriteLine("ROL ACTUAL: " + rol);

            if (rol != "Cliente")
            {
                return RedirectToAction("Login", "Cuenta");
            }

            return View();
        }

        public IActionResult DashboardBarbero()
        {
            string rol = HttpContext.Session.GetString("RolUsuario");

            if (rol != "Barbero")
            {
                return RedirectToAction("Login", "Cuenta");
            }

            return View();
        }
    }
}