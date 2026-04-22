using BarberiaOnlineNueva.Models;
using BarberiaOnlineNueva.Services;
using Microsoft.AspNetCore.Mvc;

namespace BarberiaOnlineNueva.Controllers
{
    public class CalificacionController : Controller
    {
        private readonly CalificacionService _calificacionService;
        private readonly BarberoService _barberoService;

        public CalificacionController(
     CalificacionService calificacionService,
     BarberoService barberoService)
        {
            _calificacionService = calificacionService;
            _barberoService = barberoService;
        }

        [HttpPost]
        public IActionResult Calificar(int idBarbero, int puntuacion, string comentario)
        {
            int idUsuario = HttpContext.Session.GetInt32("IdUsuario") ?? 0;

            var calificacion = new Calificacion
            {
                IdBarbero = idBarbero,
                IdUsuario = idUsuario,
                Puntuacion = puntuacion,
                Comentario = comentario
            };

            _calificacionService.CrearCalificacion(calificacion);

            return RedirectToAction("DashboardCliente", "Panel");
        }

        public IActionResult MisCalificaciones()
        {
            int? idUsuario = HttpContext.Session.GetInt32("IdUsuario");

            if (idUsuario == null)
                return RedirectToAction("Login", "Cuenta");

            var barbero = _barberoService.ObtenerPorUsuario(idUsuario.Value);

            if (barbero == null)
                return Content("No eres barbero");

            var lista = _calificacionService.ObtenerPorBarbero(barbero.IdBarbero);

            return View(lista);
        }
    }
}