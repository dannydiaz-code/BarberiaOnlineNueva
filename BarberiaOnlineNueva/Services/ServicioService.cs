using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using BarberiaOnlineNueva.Models;

namespace BarberiaOnlineNueva.Services
{
    public class ServicioService
    {
        private readonly ConexionService _conexionService;

        public ServicioService(ConexionService conexionService)
        {
            _conexionService = conexionService;
        }

        public List<SelectListItem> ObtenerServiciosSelect()
        {
            List<SelectListItem> lista = new List<SelectListItem>();

            using (SqlConnection conexion = _conexionService.ObtenerConexion())
            {
                using (SqlCommand cmd = new SqlCommand("SELECT IdServicio, NombreServicio FROM Servicios", conexion))
                {
                    conexion.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(new SelectListItem
                            {
                                Value = reader["IdServicio"].ToString(),
                                Text = reader["NombreServicio"].ToString()
                            });
                        }
                    }
                }
            }

            return lista;
        }
        public List<Servicio> ObtenerServicios()
        {
            List<Servicio> lista = new List<Servicio>();

            using (SqlConnection conexion = _conexionService.ObtenerConexion())
            {
                string query = "SELECT * FROM Servicios";

                using (SqlCommand cmd = new SqlCommand(query, conexion))
                {
                    conexion.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(new Servicio
                            {
                                IdServicio = Convert.ToInt32(reader["IdServicio"]),
                                NombreServicio = reader["NombreServicio"].ToString(),
                                Precio = Convert.ToDecimal(reader["Precio"])
                            });
                        }
                    }
                }
            }

            return lista;
        }
        public void CrearServicio(Servicio servicio)
        {
            using (SqlConnection conexion = _conexionService.ObtenerConexion())
            {
                string query = @"INSERT INTO Servicios (NombreServicio, Precio, DuracionMinutos)
                               VALUES (@Nombre, @Precio, @Duracion)";

                using (SqlCommand cmd = new SqlCommand(query, conexion))
                {
                    cmd.Parameters.AddWithValue("@Nombre", servicio.NombreServicio);
                    cmd.Parameters.AddWithValue("@Precio", servicio.Precio);
                    cmd.Parameters.AddWithValue("@Duracion", servicio.DuracionMinutos);

                    conexion.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void EliminarServicio(int id)
        {
           
            if (TieneCitasAsociadas(id))
            {
                throw new Exception("No se puede eliminar el servicio porque tiene citas asociadas.");
            }

            using (SqlConnection conexion = _conexionService.ObtenerConexion())
            {
                string query = "DELETE FROM Servicios WHERE IdServicio = @Id";

                using (SqlCommand cmd = new SqlCommand(query, conexion))
                {
                    cmd.Parameters.AddWithValue("@Id", id);

                    conexion.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }
        public Servicio ObtenerPorId(int id)
        {
            Servicio servicio = null;

            using (SqlConnection conexion = _conexionService.ObtenerConexion())
            {
                string query = "SELECT * FROM Servicios WHERE IdServicio = @Id";

                using (SqlCommand cmd = new SqlCommand(query, conexion))
                {
                    cmd.Parameters.AddWithValue("@Id", id);

                    conexion.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            servicio = new Servicio
                            {
                                IdServicio = Convert.ToInt32(reader["IdServicio"]),
                                NombreServicio = reader["NombreServicio"].ToString(),
                                Precio = Convert.ToDecimal(reader["Precio"])
                            };
                        }
                    }
                }
            }

            return servicio;
        }

        public void ActualizarServicio(Servicio servicio)
        {
            using (SqlConnection conexion = _conexionService.ObtenerConexion())
            {
                string query = @"UPDATE Servicios 
                         SET NombreServicio = @Nombre,
                             Precio = @Precio,
                             DuracionMinutos = @Duracion
                         WHERE IdServicio = @Id";

                using (SqlCommand cmd = new SqlCommand(query, conexion))
                {
                    cmd.Parameters.AddWithValue("@Nombre", servicio.NombreServicio);
                    cmd.Parameters.AddWithValue("@Precio", servicio.Precio);
                    cmd.Parameters.AddWithValue("@Duracion", servicio.DuracionMinutos);
                    cmd.Parameters.AddWithValue("@Id", servicio.IdServicio);

                    conexion.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }
        public bool TieneCitasAsociadas(int idServicio)
        {
            using (SqlConnection conexion = _conexionService.ObtenerConexion())
            {
                string query = "SELECT COUNT(*) FROM Citas WHERE IdServicio = @IdServicio";

                using (SqlCommand cmd = new SqlCommand(query, conexion))
                {
                    cmd.Parameters.AddWithValue("@IdServicio", idServicio);

                    conexion.Open();
                    int cantidad = (int)cmd.ExecuteScalar();

                    return cantidad > 0;
                }
            }
        }
    }
}
