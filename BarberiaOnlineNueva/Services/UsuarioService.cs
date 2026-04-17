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
                string query = @"
                SELECT U.IdUsuario, U.Nombre, U.Correo, U.IdRol, R.NombreRol
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
                                 VALUES (@Nombre, @Correo, @Contrasena, @IdRol)";

                using (SqlCommand cmd = new SqlCommand(query, conexion))
                {
                    cmd.Parameters.AddWithValue("@Nombre", usuario.Nombre);
                    cmd.Parameters.AddWithValue("@Correo", usuario.Correo);
                    cmd.Parameters.AddWithValue("@Contrasena", usuario.Contrasena);
                    cmd.Parameters.AddWithValue("@IdRol", usuario.IdRol);

                    conexion.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public int CrearUsuario(string correo, string contrasena, string rol, string nombre)
        {
            int idUsuario = 0;

            using (SqlConnection conexion = _conexionService.ObtenerConexion())
            {
                string query = @"
                INSERT INTO Usuarios (Nombre, Correo, Contrasena, IdRol)
                OUTPUT INSERTED.IdUsuario
                VALUES (@Nombre, @Correo, @Contrasena,
                (SELECT IdRol FROM Roles WHERE NombreRol = @Rol))";

                using (SqlCommand cmd = new SqlCommand(query, conexion))
                {
                    cmd.Parameters.AddWithValue("@Nombre", nombre);
                    cmd.Parameters.AddWithValue("@Correo", correo);
                    cmd.Parameters.AddWithValue("@Contrasena", contrasena);
                    cmd.Parameters.AddWithValue("@Rol", rol);

                    conexion.Open();
                    idUsuario = (int)cmd.ExecuteScalar();
                }
            }

            return idUsuario;
        }
    }
}