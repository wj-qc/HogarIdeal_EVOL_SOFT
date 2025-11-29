using CapaEntidad;
using CapaEntidad.Interfaces;
using System;
using System.Collections.Generic;

namespace CapaNegocio.Servicios
{
    public class ServicioProducto : IServicioProducto
    {
        private readonly IRepositorioProducto _repositorio;
        private readonly IValidacionProducto _validacion;

        public ServicioProducto(IRepositorioProducto repositorio, IValidacionProducto validacion)
        {
            _repositorio = repositorio ?? throw new ArgumentNullException(nameof(repositorio));
            _validacion = validacion ?? throw new ArgumentNullException(nameof(validacion));
        }

        public List<Producto> Listar()
        {
            return _repositorio.Listar();
        }

        public int Registrar(Producto obj, out string mensaje)
        {
            mensaje = string.Empty;

            // Validar el objeto
            string validacionMensaje = _validacion.Validar(obj);
            if (!string.IsNullOrEmpty(validacionMensaje))
            {
                mensaje = validacionMensaje;
                return 0;
            }

            // Verificar si el código ya existe
            var productoExistente = _repositorio.ObtenerPorCodigo(obj.Codigo);
            if (productoExistente != null)
            {
                mensaje = "Ya existe un producto con este código\n";
                return 0;
            }

            return _repositorio.Registrar(obj, out mensaje);
        }

        public bool Editar(Producto obj, out string mensaje)
        {
            mensaje = string.Empty;

            // Validar el objeto
            string validacionMensaje = _validacion.Validar(obj);
            if (!string.IsNullOrEmpty(validacionMensaje))
            {
                mensaje = validacionMensaje;
                return false;
            }

            // Verificar si el código ya existe en otro producto
            var productoExistente = _repositorio.ObtenerPorCodigo(obj.Codigo);
            if (productoExistente != null && productoExistente.IdProducto != obj.IdProducto)
            {
                mensaje = "Ya existe otro producto con este código\n";
                return false;
            }

            return _repositorio.Editar(obj, out mensaje);
        }

        public bool Eliminar(Producto obj, out string mensaje)
        {
            if (obj == null || obj.IdProducto <= 0)
            {
                mensaje = "Producto inválido para eliminar\n";
                return false;
            }

            return _repositorio.Eliminar(obj, out mensaje);
        }

        public List<Producto> ListarPorCategoria(int idCategoria)
        {
            if (idCategoria <= 0)
            {
                return new List<Producto>();
            }

            return _repositorio.ListarPorCategoria(idCategoria);
        }

        public List<Producto> ListarActivos()
        {
            return _repositorio.ListarActivos();
        }

        public Producto ObtenerPorCodigo(string codigo)
        {
            if (string.IsNullOrWhiteSpace(codigo))
            {
                return null;
            }

            return _repositorio.ObtenerPorCodigo(codigo);
        }
    }
}
