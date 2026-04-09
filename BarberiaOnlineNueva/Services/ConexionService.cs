using Microsoft.Data.SqlClient;

namespace BarberiaOnlineNueva.Services
{
    public class ConexionService
    {
        private readonly string _cadenaConexion;

        public ConexionService(IConfiguration configuration)
        {
            _cadenaConexion = configuration.GetConnectionString("ConexionBarberia");
        }

        public SqlConnection ObtenerConexion()
        {
            return new SqlConnection(_cadenaConexion);
        }
    }
}