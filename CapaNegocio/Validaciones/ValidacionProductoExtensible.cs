using CapaEntidad;
using CapaEntidad.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace CapaNegocio.Validaciones
{
    public class ValidacionProductoExtensible : IValidacionProducto
    {
        private readonly List<IValidacionStrategy<Producto>> _estrategias;

        public ValidacionProductoExtensible()
        {
            _estrategias = new List<IValidacionStrategy<Producto>>();
        }

        public ValidacionProductoExtensible(List<IValidacionStrategy<Producto>> estrategias)
        {
            _estrategias = estrategias ?? new List<IValidacionStrategy<Producto>>();
        }

        public void AgregarEstrategia(IValidacionStrategy<Producto> estrategia)
        {
            if (estrategia != null)
            {
                _estrategias.Add(estrategia);
            }
        }

        public string Validar(Producto obj)
        {
            var mensajes = new List<string>();

            foreach (var estrategia in _estrategias.Where(e => e.EsAplicable(obj)))
            {
                string mensaje = estrategia.Validar(obj);
                if (!string.IsNullOrEmpty(mensaje))
                {
                    mensajes.Add(mensaje);
                }
            }

            return string.Join("\n", mensajes);
        }

        public string ValidarCodigo(string codigo)
        {
            var producto = new Producto { Codigo = codigo };
            return Validar(producto);
        }

        public string ValidarNombre(string nombre)
        {
            var producto = new Producto { Nombre = nombre };
            return Validar(producto);
        }

        public string ValidarDescripcion(string descripcion)
        {
            var producto = new Producto { Descripcion = descripcion };
            return Validar(producto);
        }
    }
}
