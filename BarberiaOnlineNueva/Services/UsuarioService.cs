using BarberiaOnlineNueva.Models;
using Microsoft.Data.SqlClient;

namespace BarberiaOnlineNueva.Services
{
    public class UsuarioService
    {
        private readonly ConexionService _conexionService;

        public UsuarioService(ConexionService conexionService)
        {
            _conexionService = conexionService;
        }

        public Usuario ValidarLogin(string correo, string contrasena)
        {
            Usuario usuario = null;

            using (SqlConnection conexion = _conexionService.ObtenerConexion())
            {
                string query = @"SELECT U.IdUsuario, U.Nombre, U.Correo, U.Contrasena, U.IdRol, R.NombreRol
                 FROM Usuarios U
                 INNER JOIN Roles R ON U.IdRol = R.IdRol
                 WHERE U.Correo = @Correo AND U.Contrasena = @Contrasena";

                using (SqlCommand cmd = new SqlCommand(query, conexion))
                {
                    cmd.Parameters.AddWithValue("@Correo", correo);
                    cmd.Parameters.AddWithValue("@Contrasena", contrasena);

                    conexion.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            usuario = new Usuario
                            {
                                IdUsuario = Convert.ToInt32(reader["IdUsuario"]),
                                Nombre = reader["Nombre"].ToString(),
                                Correo = reader["Correo"].ToString(),
                                IdRol = Convert.ToInt32(reader["IdRol"]),
                                NombreRol = reader["NombreRol"].ToString()
                            };
                        }
                    }
                }
            }

            return usuario;
        }

        public void Registrar(Usuario usuario)
        {
            using (SqlConnection conexion = _conexionService.ObtenerConexion())
            {
                string query = @"INSERT INTO Usuarios (Nombre, Correo, Contrasena, IdRol)
                 VALUES (@Nombre, @Correo, @Contrasena, 2)";

                using (SqlCommand cmd = new SqlCommand(query, conexion))
                {
                    cmd.Parameters.AddWithValue("@Nombre", usuario.Nombre);
                    cmd.Parameters.AddWithValue("@Correo", usuario.Correo);
                    cmd.Parameters.AddWithValue("@Contrasena", usuario.Contrasena);

                    conexion.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}