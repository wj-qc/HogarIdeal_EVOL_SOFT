using CapaEntidad;
using CapaEntidad.Interfaces;
using System.Text;
using System.Text.RegularExpressions;

namespace CapaNegocio.Validaciones.Strategies
{
    public class ValidacionProductoAvanzada : IValidacionStrategy<Producto>
    {
        public string Validar(Producto obj)
        {
            StringBuilder mensaje = new StringBuilder();

            if (obj == null)
            {
                mensaje.AppendLine("El objeto producto no puede ser nulo");
                return mensaje.ToString();
            }

            // Validación de código
            if (string.IsNullOrWhiteSpace(obj.Codigo))
            {
                mensaje.AppendLine("Es necesario el código del Producto");
            }
            else if (!Regex.IsMatch(obj.Codigo, @"^\d{4}$"))
            {
                mensaje.AppendLine("El código debe ser de 4 dígitos numéricos");
            }

            // Validación de nombre
            if (string.IsNullOrWhiteSpace(obj.Nombre))
            {
                mensaje.AppendLine("Es necesario el nombre del Producto");
            }
            else if (!Regex.IsMatch(obj.Nombre, @"^[a-zA-ZñÑáéíóúÁÉÍÓÚ\s]+$"))
            {
                mensaje.AppendLine("El nombre debe contener solo letras, espacios y puede incluir tildes o 'ñ'");
            }
            else if (obj.Nombre.Length < 2 || obj.Nombre.Length > 100)
            {
                mensaje.AppendLine("El nombre debe tener entre 2 y 100 caracteres");
            }

            // Validación de descripción
            if (string.IsNullOrWhiteSpace(obj.Descripcion))
            {
                mensaje.AppendLine("Es necesario la descripción del Producto");
            }
            else if (!Regex.IsMatch(obj.Descripcion, @"^(?=.*[a-zA-ZñÑáéíóúÁÉÍÓÚ])[a-zA-ZñÑáéíóúÁÉÍÓÚ0-9\s]*(\.[a-zA-ZñÑáéíóúÁÉÍÓÚ0-9\s]*)?$"))
            {
                mensaje.AppendLine("La descripción debe contener al menos una letra, puede incluir números, tildes, ñ, y puntos solo si están acompañados de letras");
            }
            else if (obj.Descripcion.Length < 5 || obj.Descripcion.Length > 500)
            {
                mensaje.AppendLine("La descripción debe tener entre 5 y 500 caracteres");
            }

            return mensaje.ToString();
        }

        public bool EsAplicable(Producto obj)
        {
            return obj != null;
        }
    }
}
