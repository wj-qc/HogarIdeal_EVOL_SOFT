using System;
using System.Collections.Generic;

namespace CapaEntidad.Interfaces
{
    public interface IRepositorioExtensible<T> : IRepositorioBase<T> where T : class
    {
        void AgregarExtension(IRepositorioExtension<T> extension);
        void RemoverExtension(IRepositorioExtension<T> extension);
    }

    public interface IRepositorioExtension<T> where T : class
    {
        void AntesDeRegistrar(T obj);
        void DespuesDeRegistrar(T obj, int idGenerado);
        void AntesDeEditar(T obj);
        void DespuesDeEditar(T obj, bool resultado);
        void AntesDeEliminar(T obj);
        void DespuesDeEliminar(T obj, bool resultado);
    }
}
