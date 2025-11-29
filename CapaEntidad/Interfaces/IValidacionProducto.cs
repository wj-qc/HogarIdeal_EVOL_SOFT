namespace CapaEntidad.Interfaces
{
    public interface IValidacionProducto : IValidacion<Producto>
    {
        string ValidarCodigo(string codigo);
        string ValidarNombre(string nombre);
        string ValidarDescripcion(string descripcion);
    }
}
