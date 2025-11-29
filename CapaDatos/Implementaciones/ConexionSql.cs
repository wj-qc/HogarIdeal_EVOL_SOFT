using CapaEntidad.Interfaces;
using System.Configuration;

namespace CapaDatos.Implementaciones
{
    public class ConexionSql : IConexion
    {
        public string ObtenerCadenaConexion()
        {
            return ConfigurationManager.ConnectionStrings["cadena_conexion"].ToString();
        }
    }
}
