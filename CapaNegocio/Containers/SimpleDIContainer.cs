using System;
using System.Collections.Generic;

namespace CapaNegocio.Containers
{
    public class SimpleDIContainer
    {
        private readonly Dictionary<Type, Func<object>> _registrations;
        private readonly Dictionary<Type, object> _singletons;

        public SimpleDIContainer()
        {
            _registrations = new Dictionary<Type, Func<object>>();
            _singletons = new Dictionary<Type, object>();
        }

        public void Registrar<TInterface, TImplementation>(bool singleton = false) 
            where TInterface : class 
            where TImplementation : class, TInterface
        {
            if (singleton)
            {
                _registrations[typeof(TInterface)] = () => GetSingleton<TImplementation>();
            }
            else
            {
                _registrations[typeof(TInterface)] = () => Activator.CreateInstance<TImplementation>();
            }
        }

        public void RegistrarInstancia<TInterface>(TInterface instance) where TInterface : class
        {
            _registrations[typeof(TInterface)] = () => instance;
        }

        public T Resolver<T>() where T : class
        {
            var type = typeof(T);
            
            if (_registrations.ContainsKey(type))
            {
                return (T)_registrations[type]();
            }

            throw new InvalidOperationException($"No se encontró registro para el tipo {type.Name}");
        }

        private T GetSingleton<T>() where T : class
        {
            var type = typeof(T);
            
            if (!_singletons.ContainsKey(type))
            {
                _singletons[type] = Activator.CreateInstance<T>();
            }
            
            return (T)_singletons[type];
        }
    }
}
