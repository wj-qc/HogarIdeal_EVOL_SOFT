using System;

namespace CapaEntidad.Interfaces
{
    public interface IValidacion<T> where T : class
    {
        string Validar(T obj);
    }
}
