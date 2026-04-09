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
                using (SqlCommand cmd = new SqlCommand("INSERT INTO Barberos (Nombre, Especialidad, Telefono) VALUES (@Nombre, @Especialidad, @Telefono)", conexion))
                {
                    cmd.Parameters.AddWithValue("@Nombre", barbero.Nombre);
                    cmd.Parameters.AddWithValue("@Especialidad", barbero.Especialidad);
                    cmd.Parameters.AddWithValue("@Telefono", barbero.Telefono);

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
                                Telefono = reader["Telefono"].ToString()
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

    }
}

