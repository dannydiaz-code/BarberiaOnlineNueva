using System;

namespace BarberiaOnlineNueva.Models
{
    public class Servicio
    {
        public int IdServicio { get; set; }
        public string NombreServicio { get; set; }
        public decimal Precio { get; set; }

        public int DuracionMinutos { get; set; }
    }
}