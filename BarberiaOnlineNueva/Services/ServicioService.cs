using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;

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
    }
}
