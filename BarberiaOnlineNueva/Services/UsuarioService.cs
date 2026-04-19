using BarberiaOnlineNueva.Models;
using Microsoft.Data.SqlClient;
using System.Net;
using System.Net.Mail;

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
        public string GenerarToken(int idUsuario)
        {
            string token = Guid.NewGuid().ToString();

            using (SqlConnection conexion = _conexionService.ObtenerConexion())
            {
                string query = @"INSERT INTO RecuperacionPassword (IdUsuario, Token, FechaExpiracion)
                         VALUES (@IdUsuario, @Token, DATEADD(HOUR, 1, GETDATE()))";

                using (SqlCommand cmd = new SqlCommand(query, conexion))
                {
                    cmd.Parameters.AddWithValue("@IdUsuario", idUsuario);
                    cmd.Parameters.AddWithValue("@Token", token);

                    conexion.Open();
                    cmd.ExecuteNonQuery();
                }
            }

            return token;
        }

        public int? ValidarToken(string token)
        {
            using (SqlConnection conexion = _conexionService.ObtenerConexion())
            {
                string query = @"SELECT IdUsuario 
                         FROM RecuperacionPassword 
                         WHERE Token = @Token 
                         AND FechaExpiracion > GETDATE()";

                using (SqlCommand cmd = new SqlCommand(query, conexion))
                {
                    cmd.Parameters.AddWithValue("@Token", token);

                    conexion.Open();
                    var result = cmd.ExecuteScalar();

                    if (result != null)
                        return (int)result;
                }
            }

            return null;
        }

        public Usuario ObtenerPorCorreo(string correo)
        {
            Usuario usuario = null;

            using (SqlConnection conexion = _conexionService.ObtenerConexion())
            {
                string query = "SELECT * FROM Usuarios WHERE Correo = @Correo";

                using (SqlCommand cmd = new SqlCommand(query, conexion))
                {
                    cmd.Parameters.AddWithValue("@Correo", correo);

                    conexion.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            usuario = new Usuario
                            {
                                IdUsuario = Convert.ToInt32(reader["IdUsuario"]),
                                Correo = reader["Correo"].ToString()
                            };
                        }
                    }
                }
            }

            return usuario;
        }

        public void ActualizarPassword(int idUsuario, string nuevaPassword)
        {
            using (SqlConnection conexion = _conexionService.ObtenerConexion())
            {
                string query = "UPDATE Usuarios SET Contrasena = @Pass WHERE IdUsuario = @Id";

                using (SqlCommand cmd = new SqlCommand(query, conexion))
                {
                    cmd.Parameters.AddWithValue("@Pass", nuevaPassword);
                    cmd.Parameters.AddWithValue("@Id", idUsuario);

                    conexion.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void EnviarCorreo(string correoDestino, string token)
        {
            string link = $"https://localhost:7212/Cuenta/ResetPassword?token={token}";

            MailMessage mensaje = new MailMessage();
            mensaje.To.Add(correoDestino);
            mensaje.Subject = "Recuperar contraseña - Barbería";
            mensaje.Body = $"Hola,\n\nHaz clic en el siguiente enlace para cambiar tu contraseña:\n{link}";
            mensaje.From = new MailAddress("TU_CORREO@gmail.com");

            SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587)
            {
                Credentials = new NetworkCredential(
                    "dannydiaza0@gmail.com",
                    "vprbmvmzxsbxgnjs"
                ),
                EnableSsl = true
            };

            smtp.Send(mensaje);
        }
    }
}