namespace BarberiaOnlineNueva.Models
{
    public class Cita
    {
        public int IdCita { get; set; }
        public int IdUsuario { get; set; }
        public int IdBarbero { get; set; }
        public int IdServicio { get; set; }
        public DateTime Fecha { get; set; }
        public TimeSpan Hora { get; set; }
        public string Estado { get; set; }
        public string NombreBarbero { get; set; }
        public string NombreServicio { get; set; }
        public string NombreCliente { get; set; }
        public string NombreUsuario { get; set; }

    }
}
