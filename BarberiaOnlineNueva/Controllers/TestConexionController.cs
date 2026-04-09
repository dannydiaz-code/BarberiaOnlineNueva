using Microsoft.AspNetCore.Mvc;
using BarberiaOnlineNueva.Services;

namespace BarberiaOnlineNueva.Controllers
{
    public class TestConexionController : Controller
    {
        private readonly ConexionService _conexionService;

        public TestConexionController(ConexionService conexionService)
        {
            _conexionService = conexionService;
        }

        public IActionResult Index()
        {
            try
            {
                using (var conexion = _conexionService.ObtenerConexion())
                {
                    conexion.Open();
                    ViewBag.Mensaje = "Conexión exitosa con BarberiaDB en AWS.";
                }
            }
            catch (Exception ex)
            {
                ViewBag.Mensaje = "Error de conexión: " + ex.Message;
            }

            return View();
        }
    }
}