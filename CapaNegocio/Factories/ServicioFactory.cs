using CapaDatos.Implementaciones;
using CapaEntidad.Interfaces;
using CapaNegocio.Servicios;
using CapaNegocio.Validaciones;

namespace CapaNegocio.Factories
{
    public static class ServicioFactory
    {
        public static IServicioProducto CrearServicioProducto()
        {
            IConexion conexion = new ConexionSql();
            IRepositorioProducto repositorio = new RepositorioProducto(conexion);
            IValidacionProducto validacion = new ValidacionProducto();
            
            return new ServicioProducto(repositorio, validacion);
        }

        public static IServicioProducto CrearServicioProducto(IConexion conexion, IRepositorioProducto repositorio, IValidacionProducto validacion)
        {
            return new ServicioProducto(repositorio, validacion);
        }
    }
}
