using BarberiaOnlineNueva.Models;
using BarberiaOnlineNueva.Services;
using Microsoft.AspNetCore.Mvc;

namespace BarberiaOnlineNueva.Controllers
{
    public class ServicioController : Controller
    {
        private readonly ServicioService _servicioService;

        public ServicioController(ServicioService servicioService)
        {
            _servicioService = servicioService;
        }

        public IActionResult Index()
        {
            var lista = _servicioService.ObtenerServicios();
            return View(lista);
        }

   
        public IActionResult Crear()
        {
            return View();
        }


        [HttpPost]
        public IActionResult Crear(Servicio servicio)
        {
            if (!ModelState.IsValid)
            {
                return View(servicio);
            }

            _servicioService.CrearServicio(servicio);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Eliminar(int id)
        {
            _servicioService.EliminarServicio(id);
            return RedirectToAction("Index");
        }
        // GET
        public IActionResult Editar(int id)
        {
            var servicio = _servicioService.ObtenerPorId(id);
            return View(servicio);
        }

        [HttpPost]
        public IActionResult Editar(Servicio servicio)
        {
            if (!ModelState.IsValid)
            {
                return View(servicio);
            }

            _servicioService.ActualizarServicio(servicio);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult EliminarServicio(int id)
        {
            if (_servicioService.TieneCitasAsociadas(id))
            {
                TempData["Error"] = "No puedes eliminar este servicio porque tiene citas asociadas.";
                return RedirectToAction("Index");
            }

            _servicioService.EliminarServicio(id);

            TempData["Mensaje"] = "Servicio eliminado correctamente.";
            return RedirectToAction("Index");
        }
    }
}
