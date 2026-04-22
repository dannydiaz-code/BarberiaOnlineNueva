using BarberiaOnlineNueva.Models;
using Microsoft.Data.SqlClient;
using System.Data;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BarberiaOnlineNueva.Services
{
    public class BarberoService
    {
        private readonly ConexionService _conexionService;

        public BarberoService(ConexionService conexionService)
        {
            _conexionService = conexionService;
        }

        public void AgregarBarbero(Barbero barbero)
        {
            using (SqlConnection conexion = _conexionService.ObtenerConexion())
            {
                using (SqlCommand cmd = new SqlCommand(@"
            INSERT INTO Barberos 
            (Nombre, Especialidad, Telefono, Estado, IdUsuario, Descripcion, Experiencia)
            VALUES 
            (@Nombre, @Especialidad, @Telefono, 1, @IdUsuario, @Descripcion, @Experiencia)
        ", conexion))
                {
                    cmd.Parameters.AddWithValue("@Nombre", barbero.Nombre);
                    cmd.Parameters.AddWithValue("@Especialidad", barbero.Especialidad);
                    cmd.Parameters.AddWithValue("@Telefono", barbero.Telefono);

                    
                    cmd.Parameters.AddWithValue("@IdUsuario", barbero.IdUsuario);

                    cmd.Parameters.AddWithValue("@Descripcion", barbero.Descripcion ?? "");
                    cmd.Parameters.AddWithValue("@Experiencia", barbero.Experiencia ?? "");

                    conexion.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public List<Barbero> ObtenerBarberos()
        {
            List<Barbero> lista = new List<Barbero>();

            using (SqlConnection conexion = _conexionService.ObtenerConexion())
            {
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM Barberos", conexion))
                {
                    conexion.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(new Barbero
                            {
                                IdBarbero = Convert.ToInt32(reader["IdBarbero"]),
                                Nombre = reader["Nombre"].ToString(),
                                Especialidad = reader["Especialidad"].ToString(),
                                Telefono = reader["Telefono"].ToString(),
                                Descripcion = reader["Descripcion"].ToString(),
                                Experiencia = reader["Experiencia"].ToString(),
                            });
                        }
                    }
                }
            }

            return lista;
        }
        public List<SelectListItem> ObtenerBarberosSelect()
        {
            List<SelectListItem> lista = new List<SelectListItem>();

            using (SqlConnection conexion = _conexionService.ObtenerConexion())
            {
                using (SqlCommand cmd = new SqlCommand("SELECT IdBarbero, Nombre FROM Barberos", conexion))
                {
                    conexion.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(new SelectListItem
                            {
                                Value = reader["IdBarbero"].ToString(),
                                Text = reader["Nombre"].ToString()
                            });
                        }
                    }
                }
            }

            return lista;
        }
        public Barbero ObtenerPorUsuario(int idUsuario)
        {
            Barbero barbero = null;

            using (SqlConnection conexion = _conexionService.ObtenerConexion())
            {
                string query = "SELECT * FROM Barberos WHERE IdUsuario = @IdUsuario";

                using (SqlCommand cmd = new SqlCommand(query, conexion))
                {
                    cmd.Parameters.AddWithValue("@IdUsuario", idUsuario);

                    conexion.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            barbero = new Barbero
                            {
                                IdBarbero = Convert.ToInt32(reader["IdBarbero"]),
                                Nombre = reader["Nombre"].ToString(),
                                Especialidad = reader["Especialidad"].ToString(),
                                Telefono = reader["Telefono"].ToString(),
                                IdUsuario = Convert.ToInt32(reader["IdUsuario"])
                            };
                        }
                    }
                }
            }

            return barbero;
        }

        public void ActualizarPerfil(Barbero barbero)
        {
            using (SqlConnection conexion = _conexionService.ObtenerConexion())
            {
                string query = @"
        UPDATE Barberos
        SET Descripcion = @Descripcion,
            Experiencia = @Experiencia
        WHERE IdBarbero = @IdBarbero";

                using (SqlCommand cmd = new SqlCommand(query, conexion))
                {
                    cmd.Parameters.AddWithValue("@Descripcion", barbero.Descripcion ?? "");
                    cmd.Parameters.AddWithValue("@Experiencia", barbero.Experiencia ?? "");
                    cmd.Parameters.AddWithValue("@IdBarbero", barbero.IdBarbero);

                    conexion.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

    }
}

