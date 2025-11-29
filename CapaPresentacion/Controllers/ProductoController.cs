using CapaEntidad;
using CapaEntidad.Interfaces;
using CapaNegocio.Servicios;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CapaPresentacion.Controllers
{
    public class ProductoController
    {
        private readonly IServicioProducto _servicioProducto;
        private readonly GeneradorCodigoProducto _generadorCodigo;
        private readonly ExportadorDatos _exportador;

        public ProductoController()
        {
            _servicioProducto = ServiceLocator.ObtenerServicio<IServicioProducto>();
            _generadorCodigo = new GeneradorCodigoProducto(_servicioProducto);
            _exportador = new ExportadorDatos(_servicioProducto);
        }

        public ProductoController(IServicioProducto servicioProducto, GeneradorCodigoProducto generadorCodigo, ExportadorDatos exportador)
        {
            _servicioProducto = servicioProducto ?? throw new ArgumentNullException(nameof(servicioProducto));
            _generadorCodigo = generadorCodigo ?? throw new ArgumentNullException(nameof(generadorCodigo));
            _exportador = exportador ?? throw new ArgumentNullException(nameof(exportador));
        }

        public List<Producto> ObtenerTodosLosProductos()
        {
            return _servicioProducto.Listar();
        }

        public List<Producto> ObtenerProductosActivos()
        {
            return _servicioProducto.ListarActivos();
        }

        public List<Producto> ObtenerProductosPorCategoria(int idCategoria)
        {
            return _servicioProducto.ListarPorCategoria(idCategoria);
        }

        public Producto ObtenerProductoPorCodigo(string codigo)
        {
            return _servicioProducto.ObtenerPorCodigo(codigo);
        }

        public ResultadoOperacion<int> RegistrarProducto(Producto producto)
        {
            try
            {
                string mensaje;
                int idGenerado = _servicioProducto.Registrar(producto, out mensaje);
                
                return new ResultadoOperacion<int>
                {
                    Exitoso = idGenerado > 0,
                    Valor = idGenerado,
                    Mensaje = mensaje
                };
            }
            catch (Exception ex)
            {
                return new ResultadoOperacion<int>
                {
                    Exitoso = false,
                    Valor = 0,
                    Mensaje = ex.Message
                };
            }
        }

        public ResultadoOperacion<bool> EditarProducto(Producto producto)
        {
            try
            {
                string mensaje;
                bool resultado = _servicioProducto.Editar(producto, out mensaje);
                
                return new ResultadoOperacion<bool>
                {
                    Exitoso = resultado,
                    Valor = resultado,
                    Mensaje = mensaje
                };
            }
            catch (Exception ex)
            {
                return new ResultadoOperacion<bool>
                {
                    Exitoso = false,
                    Valor = false,
                    Mensaje = ex.Message
                };
            }
        }

        public ResultadoOperacion<bool> EliminarProducto(int idProducto)
        {
            try
            {
                var producto = new Producto { IdProducto = idProducto };
                string mensaje;
                bool resultado = _servicioProducto.Eliminar(producto, out mensaje);
                
                return new ResultadoOperacion<bool>
                {
                    Exitoso = resultado,
                    Valor = resultado,
                    Mensaje = mensaje
                };
            }
            catch (Exception ex)
            {
                return new ResultadoOperacion<bool>
                {
                    Exitoso = false,
                    Valor = false,
                    Mensaje = ex.Message
                };
            }
        }

        public string GenerarSiguienteCodigo()
        {
            return _generadorCodigo.GenerarSiguienteCodigo();
        }

        public bool ExportarProductosAExcel(List<Producto> productos, string rutaArchivo)
        {
            return _exportador.ExportarProductosAExcel(productos, rutaArchivo);
        }
    }

    public class ResultadoOperacion<T>
    {
        public bool Exitoso { get; set; }
        public T Valor { get; set; }
        public string Mensaje { get; set; }
    }
}
