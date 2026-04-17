using BarberiaOnlineNueva.Models;
using BarberiaOnlineNueva.Services;
using Microsoft.AspNetCore.Mvc;

namespace BarberiaOnlineNueva.Controllers
{
    public class PanelController : Controller
    {
        private readonly CitaService _citaService;
        private readonly BarberoService _barberoService;

        public PanelController(CitaService citaService, BarberoService barberoService)
        {
            _citaService = citaService;
            _barberoService = barberoService;
        }
        public IActionResult DashboardAdmin()
        {
            var rol = HttpContext.Session.GetString("RolUsuario");

            Console.WriteLine("ROL EN PANEL ADMIN: " + rol);

            if (string.IsNullOrEmpty(rol))
                return RedirectToAction("Login", "Cuenta");

            if (rol.Trim().ToLower() != "administrador")
                return RedirectToAction("Login", "Cuenta");

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

        public IActionResult Dashboard()
        {
            string rol = HttpContext.Session.GetString("RolUsuario");

            if (rol == "Administrador")
                return RedirectToAction("DashboardAdmin");

            if (rol == "Cliente")
                return RedirectToAction("DashboardCliente");

            return RedirectToAction("Login", "Cuenta");
        }

        public IActionResult DashboardBarbero()
        {
            string rol = HttpContext.Session.GetString("RolUsuario");

            if (string.IsNullOrEmpty(rol) || rol.ToLower() != "barbero")
            {
                return RedirectToAction("Login", "Cuenta");
            }

            int? idUsuario = HttpContext.Session.GetInt32("IdUsuario");

            if (idUsuario == null)
            {
                return RedirectToAction("Login", "Cuenta");
            }

            var barbero = _barberoService.ObtenerPorUsuario(idUsuario.Value);

            if (barbero == null)
            {
                return Content("Este usuario no está vinculado a un barbero");
            }

         
            var citas = _citaService.ObtenerCitasPorBarbero(barbero.IdBarbero) ?? new List<Cita>();

            return View(citas);
        }
    }
}