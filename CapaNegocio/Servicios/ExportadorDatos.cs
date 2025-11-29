using CapaEntidad;
using CapaEntidad.Interfaces;
using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace CapaNegocio.Servicios
{
    public class ExportadorDatos
    {
        private readonly IServicioProducto _servicioProducto;

        public ExportadorDatos(IServicioProducto servicioProducto)
        {
            _servicioProducto = servicioProducto ?? throw new ArgumentNullException(nameof(servicioProducto));
        }

        public bool ExportarProductosAExcel(List<Producto> productos, string rutaArchivo)
        {
            try
            {
                if (productos == null || productos.Count == 0)
                {
                    return false;
                }

                DataTable dt = CrearDataTable(productos);
                
                XLWorkbook wb = new XLWorkbook();
                var hoja = wb.Worksheets.Add(dt, "Informe");
                hoja.ColumnsUsed().AdjustToContents();
                wb.SaveAs(rutaArchivo);
                
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private DataTable CrearDataTable(List<Producto> productos)
        {
            DataTable dt = new DataTable();
            
            // Definir columnas
            dt.Columns.Add("Código", typeof(string));
            dt.Columns.Add("Nombre", typeof(string));
            dt.Columns.Add("Descripción", typeof(string));
            dt.Columns.Add("Categoría", typeof(string));
            dt.Columns.Add("Stock", typeof(string));
            dt.Columns.Add("Precio Compra", typeof(string));
            dt.Columns.Add("Precio Venta", typeof(string));
            dt.Columns.Add("Estado", typeof(string));

            // Agregar datos
            foreach (var producto in productos)
            {
                dt.Rows.Add(
                    producto.Codigo,
                    producto.Nombre,
                    producto.Descripcion,
                    producto.oCategoria?.Descripcion ?? "",
                    producto.Stock.ToString(),
                    producto.PrecioCompra.ToString("0.00"),
                    producto.PrecioVenta.ToString("0.00"),
                    producto.Estado ? "Activo" : "No Activo"
                );
            }

            return dt;
        }
    }
}
