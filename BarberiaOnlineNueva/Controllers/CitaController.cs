using BarberiaOnlineNueva.Models;
using BarberiaOnlineNueva.Services;
using Microsoft.AspNetCore.Mvc;

namespace BarberiaOnlineNueva.Controllers
{
    public class CitaController : Controller
    {
        private readonly CitaService _citaService;
        private readonly BarberoService _barberoService;
        private readonly ServicioService _servicioService;

        public CitaController(CitaService citaService, BarberoService barberoService, ServicioService servicioService)
        {
            _citaService = citaService;
            _barberoService = barberoService;
            _servicioService = servicioService;
        }

        [HttpGet]
        public IActionResult Crear()
        {
            string rol = HttpContext.Session.GetString("RolUsuario");

            if (rol != "Cliente")
            {
                return RedirectToAction("Login", "Cuenta");
            }
            int? idUsuario = HttpContext.Session.GetInt32("IdUsuario");

            if (idUsuario == null)
            {
                return RedirectToAction("Login", "Cuenta");
            }

            CitaViewModel model = new CitaViewModel
            {
                Barberos = _barberoService.ObtenerBarberosSelect(),
                Servicios = _servicioService.ObtenerServiciosSelect(),
                Fecha = DateTime.Today
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult Crear(CitaViewModel model)
        {
            string rol = HttpContext.Session.GetString("RolUsuario");

            if (rol != "Cliente")
            {
                return RedirectToAction("Login", "Cuenta");
            }
            model.Barberos = _barberoService.ObtenerBarberosSelect();
            model.Servicios = _servicioService.ObtenerServiciosSelect();
            int? idUsuario = HttpContext.Session.GetInt32("IdUsuario");

            if (idUsuario == null)
            {
                return RedirectToAction("Login", "Cuenta");
            }

            try
            {
                TimeSpan horaSeleccionada = TimeSpan.Parse(model.Hora);

                if (_citaService.ExisteCitaDuplicada(model.IdBarbero, model.Fecha, horaSeleccionada))
                {
                    ViewBag.Mensaje = "Ya existe una cita registrada para ese barbero en esa fecha y hora.";
                    return View(model);
                }

                Cita cita = new Cita
                {
                    IdUsuario = idUsuario.Value,
                    IdBarbero = model.IdBarbero,
                    IdServicio = model.IdServicio,
                    Fecha = model.Fecha,
                    Hora = horaSeleccionada,
                    Estado = "Pendiente"
                };

                _citaService.AgregarCita(cita);

                TempData["Mensaje"] = "Cita registrada correctamente.";
                return RedirectToAction("Crear");
            }
            catch (Exception ex)
            {
                ViewBag.Mensaje = "Error al guardar la cita: " + ex.Message;
                return View(model);
            }
        }
        [HttpGet]
        public IActionResult MisCitas()
        {
            string rol = HttpContext.Session.GetString("RolUsuario");

            if (rol != "Cliente")
            {
                return RedirectToAction("Login", "Cuenta");
            }
            int? idUsuario = HttpContext.Session.GetInt32("IdUsuario");

            if (idUsuario == null)
            {
                return RedirectToAction("Login", "Cuenta");
            }

            List<Cita> citas = _citaService.ObtenerCitasPorUsuario(idUsuario.Value);
            return View(citas);
        }


        [HttpPost]
        public IActionResult Cancelar(int idCita)
        {
            int? idUsuario = HttpContext.Session.GetInt32("IdUsuario");

            if (idUsuario == null)
            {
                return RedirectToAction("Login", "Cuenta");
            }

            _citaService.CancelarCita(idCita, idUsuario.Value);

            TempData["Mensaje"] = "La cita fue cancelada correctamente.";
            return RedirectToAction("MisCitas");
        }

        public IActionResult VerTodas()
        {
            var rol = HttpContext.Session.GetString("RolUsuario");

            if (string.IsNullOrEmpty(rol) || rol != "Administrador")
            {
                return RedirectToAction("Login", "Cuenta");
            }

            var citas = _citaService.ObtenerTodasLasCitas();

            return View(citas);
        }

        public IActionResult CambiarEstado(int id, string estado)
        {
            _citaService.CambiarEstado(id, estado);
            return RedirectToAction("DashboardBarbero", "Panel");
        }
    }
}