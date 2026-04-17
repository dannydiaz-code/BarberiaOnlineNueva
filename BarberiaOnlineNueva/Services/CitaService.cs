using BarberiaOnlineNueva.Models;
using Microsoft.Data.SqlClient;
using System.Data;

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
                 AND Hora = @Hora
                 AND Estado = 'Pendiente'";

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
                    cmd.Parameters.Add("@IdUsuario", SqlDbType.Int).Value = idUsuario;

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
        public List<Cita> ObtenerTodasLasCitas()
        {
            List<Cita> lista = new List<Cita>();

            using (SqlConnection conexion = _conexionService.ObtenerConexion())
            {
                string query = @"SELECT * FROM Citas ORDER BY Fecha DESC, Hora DESC";

                using (SqlCommand cmd = new SqlCommand(query, conexion))
                {
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
                                Estado = reader["Estado"].ToString()
                            });
                        }
                    }
                }
            }

            return lista;
        }
        public List<Cita> ObtenerCitasPorBarbero(int idBarbero)
        {
            List<Cita> lista = new List<Cita>();

            using (SqlConnection conexion = _conexionService.ObtenerConexion())
            {
                string query = @"
            SELECT 
                c.IdCita,
                c.IdBarbero,
                c.IdUsuario,
                c.Fecha,
                c.Hora,
                c.Estado,
                s.NombreServicio,
                u.Nombre AS NombreCliente
            FROM Citas c
            INNER JOIN Servicios s ON c.IdServicio = s.IdServicio
            INNER JOIN Usuarios u ON c.IdUsuario = u.IdUsuario
            WHERE c.IdBarbero = @IdBarbero
            ORDER BY c.Fecha, c.Hora";

                using (SqlCommand cmd = new SqlCommand(query, conexion))
                {
                    cmd.Parameters.AddWithValue("@IdBarbero", idBarbero);

                    conexion.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(new Cita
                            {
                                IdCita = (int)reader["IdCita"],
                                IdBarbero = (int)reader["IdBarbero"],
                                IdUsuario = (int)reader["IdUsuario"],
                                Fecha = (DateTime)reader["Fecha"],
                                Hora = (TimeSpan)reader["Hora"],
                                Estado = reader["Estado"].ToString(),
                                NombreServicio = reader["NombreServicio"].ToString(),
                                NombreCliente = reader["NombreCliente"].ToString()
                            });
                        }
                    }
                }
            }

            return lista;
        }

        public void CambiarEstado(int idCita, string estado)
        {
            using (SqlConnection conexion = _conexionService.ObtenerConexion())
            {
                string query = "UPDATE Citas SET Estado = @Estado WHERE IdCita = @IdCita";

                using (SqlCommand cmd = new SqlCommand(query, conexion))
                {
                    cmd.Parameters.AddWithValue("@Estado", estado);
                    cmd.Parameters.AddWithValue("@IdCita", idCita);

                    conexion.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
