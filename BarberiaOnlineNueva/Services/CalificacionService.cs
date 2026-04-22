using BarberiaOnlineNueva.Models;
using Microsoft.Data.SqlClient;

namespace BarberiaOnlineNueva.Services
{
    public class CalificacionService
    {
        private readonly ConexionService _conexionService;

        public CalificacionService(ConexionService conexionService)
        {
            _conexionService = conexionService;
        }

        
        public void CrearCalificacion(Calificacion calificacion)
        {
            using (var conexion = _conexionService.ObtenerConexion())
            {
                string query = @"INSERT INTO Calificaciones 
                (IdBarbero, IdUsuario, Puntuacion, Comentario)
                VALUES (@IdBarbero, @IdUsuario, @Puntuacion, @Comentario)";

                using (var cmd = new SqlCommand(query, conexion))
                {
                    cmd.Parameters.AddWithValue("@IdBarbero", calificacion.IdBarbero);
                    cmd.Parameters.AddWithValue("@IdUsuario", calificacion.IdUsuario);
                    cmd.Parameters.AddWithValue("@Puntuacion", calificacion.Puntuacion);
                    cmd.Parameters.AddWithValue("@Comentario", calificacion.Comentario ?? "");

                    conexion.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }


        public List<Calificacion> ObtenerPorBarbero(int idBarbero)
        {
            List<Calificacion> lista = new List<Calificacion>();

            using (var conexion = _conexionService.ObtenerConexion())
            {
                string query = @"
        SELECT c.*, u.Nombre
        FROM Calificaciones c
        INNER JOIN Usuarios u ON c.IdUsuario = u.IdUsuario
        WHERE c.IdBarbero = @IdBarbero";

                using (var cmd = new SqlCommand(query, conexion))
                {
                    cmd.Parameters.AddWithValue("@IdBarbero", idBarbero);

                    conexion.Open();

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(new Calificacion
                            {
                                IdBarbero = Convert.ToInt32(reader["IdBarbero"]),
                                IdUsuario = Convert.ToInt32(reader["IdUsuario"]),
                                Puntuacion = Convert.ToInt32(reader["Puntuacion"]),
                                Comentario = reader["Comentario"].ToString(),
                                Fecha = Convert.ToDateTime(reader["Fecha"]),

                               
                                NombreUsuario = reader["Nombre"].ToString()
                            });
                        }
                    }
                }
            }

            return lista;
        }


        public double ObtenerPromedio(int idBarbero)
        {
            double promedio = 0;

            using (var conexion = _conexionService.ObtenerConexion())
            {
                string query = "SELECT AVG(CAST(Puntuacion AS FLOAT)) FROM Calificaciones WHERE IdBarbero = @IdBarbero";

                using (var cmd = new SqlCommand(query, conexion))
                {
                    cmd.Parameters.AddWithValue("@IdBarbero", idBarbero);

                    conexion.Open();

                    var resultado = cmd.ExecuteScalar();

                    if (resultado != DBNull.Value)
                    {
                        promedio = Convert.ToDouble(resultado);
                    }
                }
            }

            return promedio;
        }
    }
}
