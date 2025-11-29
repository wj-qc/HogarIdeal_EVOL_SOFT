using System;
using System.Collections.Generic;

namespace CapaEntidad.Interfaces
{
    public interface IRepositorioBase<T> where T : class
    {
        List<T> Listar();
        int Registrar(T obj, out string mensaje);
        bool Editar(T obj, out string mensaje);
        bool Eliminar(T obj, out string mensaje);
    }
}
