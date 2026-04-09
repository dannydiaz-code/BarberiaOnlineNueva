using BarberiaOnlineNueva.Models;
using BarberiaOnlineNueva.Services;
using Microsoft.AspNetCore.Mvc;

namespace BarberiaOnlineNueva.Controllers
{
    public class BarberoController : Controller
    {
        private readonly BarberoService _barberoService;

        public BarberoController(BarberoService barberoService)
        {
            _barberoService = barberoService;
        }

        public IActionResult Index()
        {
            string rol = HttpContext.Session.GetString("RolUsuario");

            if (rol != "Administrador")
            {
                return RedirectToAction("Login", "Cuenta");
            }
            var lista = _barberoService.ObtenerBarberos();
            return View(lista);
        }

        [HttpGet]
        public IActionResult Crear()
        {
            string rol = HttpContext.Session.GetString("RolUsuario");

            if (rol != "Administrador")
            {
                return RedirectToAction("Login", "Cuenta");
            }
            return View();
        }

        [HttpPost]
        public IActionResult Crear(Barbero barbero)
        {
            _barberoService.AgregarBarbero(barbero);
            return RedirectToAction("Index");
        }
    }
}