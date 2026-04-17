using BarberiaOnlineNueva.Models;
using BarberiaOnlineNueva.Services;
using Microsoft.AspNetCore.Mvc;

namespace BarberiaOnlineNueva.Controllers
{
    public class BarberoController : Controller
    {
        private readonly BarberoService _barberoService;
        private readonly CitaService _citaService;
        private readonly UsuarioService _usuarioService;

        public BarberoController(BarberoService barberoService, CitaService citaService, UsuarioService usuarioService)
        {
            _barberoService = barberoService;
            _citaService = citaService;
            _usuarioService = usuarioService;
        }

        public IActionResult Disponibles()
        {
            var barberos = _barberoService.ObtenerBarberos();
            return View(barberos);
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
        public IActionResult Crear(Barbero barbero, string correo, string contrasena)
        {
            // 1️⃣ Crear usuario
            int idUsuario = _usuarioService.CrearUsuario(correo, contrasena, "barbero", barbero.Nombre);

            // 2️⃣ Asignar al barbero
            barbero.IdUsuario = idUsuario;

            // 3️⃣ Guardar barbero
            _barberoService.AgregarBarbero(barbero);

            return RedirectToAction("Index");
        }

        public IActionResult Panel(int idBarbero)
        {
            var citas = _citaService.ObtenerCitasPorBarbero(idBarbero);
            return View(citas);
        }

        public IActionResult GestionarCitas()
        {
            int? idUsuario = HttpContext.Session.GetInt32("IdUsuario");

            if (idUsuario == null)
                return RedirectToAction("Login", "Cuenta");

            var barbero = _barberoService.ObtenerPorUsuario(idUsuario.Value);

            if (barbero == null)
                return Content("No vinculado");

            var citas = _citaService.ObtenerCitasPorBarbero(barbero.IdBarbero) ?? new List<Cita>();

            return View(citas);
        }

        public IActionResult Perfil()
        {
            int idUsuario = HttpContext.Session.GetInt32("IdUsuario") ?? 0;

            var barbero = _barberoService.ObtenerPorUsuario(idUsuario);

            return View(barbero);
        }

        [HttpPost]
        public IActionResult Perfil(Barbero barbero)
        {
            _barberoService.ActualizarPerfil(barbero);

            return RedirectToAction("DashboardBarbero", "Panel");
        }
    }
}