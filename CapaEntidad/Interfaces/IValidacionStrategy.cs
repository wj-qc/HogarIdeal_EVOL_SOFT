using System;

namespace CapaEntidad.Interfaces
{
    public interface IValidacionStrategy<T> where T : class
    {
        string Validar(T obj);
        bool EsAplicable(T obj);
    }
}
