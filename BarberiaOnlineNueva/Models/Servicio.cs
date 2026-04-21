using System;
using System.ComponentModel.DataAnnotations;

namespace BarberiaOnlineNueva.Models
{
    public class Servicio
    {
        public int IdServicio { get; set; }

        public string NombreServicio { get; set; }

        public decimal Precio { get; set; }

        [Required(ErrorMessage = "La duración es obligatoria")]
        [Range(1, 300, ErrorMessage = "La duración debe ser mayor a 0")]
        public int DuracionMinutos { get; set; }
    }
}