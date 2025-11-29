using CapaEntidad.Interfaces;
using CapaNegocio.Containers;
using CapaNegocio.Factories;
using System;

namespace CapaNegocio.Servicios
{
    public static class ServiceLocator
    {
        private static SimpleDIContainer _container;
        private static bool _initialized = false;

        public static void Inicializar()
        {
            if (_initialized) return;

            _container = new SimpleDIContainer();
            
            // Registrar dependencias
            _container.Registrar<IConexion, CapaDatos.Implementaciones.ConexionSql>(true);
            _container.Registrar<IRepositorioProducto, CapaDatos.Implementaciones.RepositorioProducto>();
            _container.Registrar<IValidacionProducto, CapaNegocio.Validaciones.ValidacionProducto>();
            _container.Registrar<IServicioProducto, CapaNegocio.Servicios.ServicioProducto>();

            _initialized = true;
        }

        public static T ObtenerServicio<T>() where T : class
        {
            if (!_initialized)
            {
                Inicializar();
            }

            return _container.Resolver<T>();
        }

        public static void RegistrarServicio<TInterface, TImplementation>(bool singleton = false) 
            where TInterface : class 
            where TImplementation : class, TInterface
        {
            if (!_initialized)
            {
                Inicializar();
            }

            _container.Registrar<TInterface, TImplementation>(singleton);
        }

        public static void RegistrarInstancia<TInterface>(TInterface instance) where TInterface : class
        {
            if (!_initialized)
            {
                Inicializar();
            }

            _container.RegistrarInstancia(instance);
        }
    }
}
