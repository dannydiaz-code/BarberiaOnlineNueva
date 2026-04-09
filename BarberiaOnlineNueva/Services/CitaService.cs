using BarberiaOnlineNueva.Models;
using Microsoft.Data.SqlClient;

namespace BarberiaOnlineNueva.Services
{
    public class CitaService
    {
        private readonly ConexionService _conexionService;

        public CitaService(ConexionService conexionService)
        {
            _conexionService = conexionService;
        }

        public void AgregarCita(Cita cita)
        {
            using (SqlConnection conexion = _conexionService.ObtenerConexion())
            {
                string query = @"INSERT INTO Citas (IdUsuario, IdBarbero, IdServicio, Fecha, Hora, Estado)
                                 VALUES (@IdUsuario, @IdBarbero, @IdServicio, @Fecha, @Hora, @Estado)";

                using (SqlCommand cmd = new SqlCommand(query, conexion))
                {
                    cmd.Parameters.AddWithValue("@IdUsuario", cita.IdUsuario);
                    cmd.Parameters.AddWithValue("@IdBarbero", cita.IdBarbero);
                    cmd.Parameters.AddWithValue("@IdServicio", cita.IdServicio);
                    cmd.Parameters.AddWithValue("@Fecha", cita.Fecha);
                    cmd.Parameters.AddWithValue("@Hora", cita.Hora);
                    cmd.Parameters.AddWithValue("@Estado", "Pendiente");

                    conexion.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public bool ExisteCitaDuplicada(int idBarbero, DateTime fecha, TimeSpan hora)
        {
            using (SqlConnection conexion = _conexionService.ObtenerConexion())
            {
                string query = @"SELECT COUNT(*) 
                                 FROM Citas
                                 WHERE IdBarbero = @IdBarbero
                                 AND Fecha = @Fecha
                                 AND Hora = @Hora";

                using (SqlCommand cmd = new SqlCommand(query, conexion))
                {
                    cmd.Parameters.AddWithValue("@IdBarbero", idBarbero);
                    cmd.Parameters.AddWithValue("@Fecha", fecha.Date);
                    cmd.Parameters.AddWithValue("@Hora", hora);

                    conexion.Open();
                    int cantidad = (int)cmd.ExecuteScalar();

                    return cantidad > 0;
                }
            }
        }
        public List<Cita> ObtenerCitasPorUsuario(int idUsuario)
        {
            List<Cita> lista = new List<Cita>();

            using (SqlConnection conexion = _conexionService.ObtenerConexion())
            {
                string query = @"
                    SELECT 
                        c.IdCita,
                        c.IdUsuario,
                        c.IdBarbero,
                        c.IdServicio,
                        c.Fecha,
                        c.Hora,
                        c.Estado,
                        b.Nombre AS NombreBarbero,
                        s.NombreServicio AS NombreServicio
                    FROM Citas c
                    INNER JOIN Barberos b ON c.IdBarbero = b.IdBarbero
                    INNER JOIN Servicios s ON c.IdServicio = s.IdServicio
                    WHERE c.IdUsuario = @IdUsuario
                    ORDER BY c.Fecha DESC, c.Hora DESC";

                using (SqlCommand cmd = new SqlCommand(query, conexion))
                {
                    cmd.Parameters.AddWithValue("@IdUsuario", idUsuario);

                    conexion.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(new Cita
                            {
                                IdCita = Convert.ToInt32(reader["IdCita"]),
                                IdUsuario = Convert.ToInt32(reader["IdUsuario"]),
                                IdBarbero = Convert.ToInt32(reader["IdBarbero"]),
                                IdServicio = Convert.ToInt32(reader["IdServicio"]),
                                Fecha = Convert.ToDateTime(reader["Fecha"]),
                                Hora = (TimeSpan)reader["Hora"],
                                Estado = reader["Estado"].ToString(),
                                NombreBarbero = reader["NombreBarbero"].ToString(),
                                NombreServicio = reader["NombreServicio"].ToString()
                            });
                        }
                    }
                }
            }

            return lista;
        }
        public void CancelarCita(int idCita, int idUsuario)
        {
            using (SqlConnection conexion = _conexionService.ObtenerConexion())
            {
                string query = @"UPDATE Citas
                         SET Estado = 'Cancelada'
                         WHERE IdCita = @IdCita AND IdUsuario = @IdUsuario";

                using (SqlCommand cmd = new SqlCommand(query, conexion))
                {
                    cmd.Parameters.AddWithValue("@IdCita", idCita);
                    cmd.Parameters.AddWithValue("@IdUsuario", idUsuario);

                    conexion.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
