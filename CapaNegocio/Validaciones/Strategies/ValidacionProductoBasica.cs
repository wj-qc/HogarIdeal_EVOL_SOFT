using CapaEntidad;
using CapaEntidad.Interfaces;
using System.Text;

namespace CapaNegocio.Validaciones.Strategies
{
    public class ValidacionProductoBasica : IValidacionStrategy<Producto>
    {
        public string Validar(Producto obj)
        {
            StringBuilder mensaje = new StringBuilder();

            if (obj == null)
            {
                mensaje.AppendLine("El objeto producto no puede ser nulo");
                return mensaje.ToString();
            }

            if (string.IsNullOrWhiteSpace(obj.Codigo))
            {
                mensaje.AppendLine("Es necesario el código del Producto");
            }

            if (string.IsNullOrWhiteSpace(obj.Nombre))
            {
                mensaje.AppendLine("Es necesario el nombre del Producto");
            }

            if (string.IsNullOrWhiteSpace(obj.Descripcion))
            {
                mensaje.AppendLine("Es necesario la descripción del Producto");
            }

            return mensaje.ToString();
        }

        public bool EsAplicable(Producto obj)
        {
            return obj != null;
        }
    }
}
