using System;

namespace BarberiaOnlineNueva.Models
{
    public class Calificacion
    {
        public int IdCalificacion { get; set; }
        public int IdBarbero { get; set; }
        public int IdUsuario { get; set; }
        public int Puntuacion { get; set; }
        public string Comentario { get; set; }
        public DateTime Fecha { get; set; }

        public string NombreUsuario { get; set; }
    }
}