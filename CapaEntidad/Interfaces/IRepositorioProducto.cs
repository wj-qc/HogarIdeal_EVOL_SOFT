using System.Collections.Generic;

namespace CapaEntidad.Interfaces
{
    public interface IRepositorioProducto : IRepositorioBase<Producto>
    {
        List<Producto> ListarPorCategoria(int idCategoria);
        List<Producto> ListarActivos();
        Producto ObtenerPorCodigo(string codigo);
    }
}
