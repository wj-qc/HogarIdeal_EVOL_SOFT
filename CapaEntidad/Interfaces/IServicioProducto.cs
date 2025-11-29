using System.Collections.Generic;

namespace CapaEntidad.Interfaces
{
    public interface IServicioProducto : IServicioBase<Producto>
    {
        List<Producto> ListarPorCategoria(int idCategoria);
        List<Producto> ListarActivos();
        Producto ObtenerPorCodigo(string codigo);
    }
}
