using CapaEntidad;
using CapaEntidad.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CapaNegocio.Servicios
{
    public class GeneradorCodigoProducto
    {
        private readonly IServicioProducto _servicioProducto;

        public GeneradorCodigoProducto(IServicioProducto servicioProducto)
        {
            _servicioProducto = servicioProducto ?? throw new ArgumentNullException(nameof(servicioProducto));
        }

        public string GenerarSiguienteCodigo()
        {
            List<Producto> productos = _servicioProducto.Listar();

            if (productos.Count == 0)
            {
                return "0001";
            }

            // Obtener todos los códigos existentes en la lista de productos
            List<int> existingCodes = productos
                .Select(p => int.TryParse(p.Codigo, out int code) ? code : 0)
                .Where(code => code > 0)
                .OrderBy(code => code)
                .ToList();

            // Buscar el primer hueco en los códigos secuenciales
            int nextCode = 1;
            foreach (int code in existingCodes)
            {
                if (code != nextCode)
                {
                    break;
                }
                nextCode++;
            }

            // Retornar el código formateado
            return nextCode.ToString("D4");
        }
    }
}
