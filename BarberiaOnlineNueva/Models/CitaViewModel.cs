using Microsoft.AspNetCore.Mvc.Rendering;

namespace BarberiaOnlineNueva.Models
{
    public class CitaViewModel
    {
        public int IdBarbero { get; set; }
        public int IdServicio { get; set; }
        public DateTime Fecha { get; set; }
        public string Hora { get; set; }

        public List<SelectListItem> Barberos { get; set; }
        public List<SelectListItem> Servicios { get; set; }
    }
}