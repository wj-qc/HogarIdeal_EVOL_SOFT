using CapaEntidad;
using CapaEntidad.Interfaces;
using System.Text.RegularExpressions;

namespace CapaNegocio.Validaciones
{
    public class ValidacionProducto : IValidacionProducto
    {
        public string Validar(Producto obj)
        {
            StringBuilder mensaje = new StringBuilder();

            if (obj == null)
            {
                mensaje.AppendLine("El objeto producto no puede ser nulo");
                return mensaje.ToString();
            }

            mensaje.Append(ValidarCodigo(obj.Codigo));
            mensaje.Append(ValidarNombre(obj.Nombre));
            mensaje.Append(ValidarDescripcion(obj.Descripcion));

            return mensaje.ToString();
        }

        public string ValidarCodigo(string codigo)
        {
            if (string.IsNullOrWhiteSpace(codigo))
            {
                return "Es necesario el código del Producto\n";
            }

            if (!Regex.IsMatch(codigo, @"^\d{4}$"))
            {
                return "El código debe ser de 4 dígitos numéricos\n";
            }

            return string.Empty;
        }

        public string ValidarNombre(string nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre))
            {
                return "Es necesario el nombre del Producto\n";
            }

            if (!Regex.IsMatch(nombre, @"^[a-zA-ZñÑáéíóúÁÉÍÓÚ\s]+$"))
            {
                return "El nombre debe contener solo letras, espacios y puede incluir tildes o 'ñ'\n";
            }

            if (nombre.Length < 2 || nombre.Length > 100)
            {
                return "El nombre debe tener entre 2 y 100 caracteres\n";
            }

            return string.Empty;
        }

        public string ValidarDescripcion(string descripcion)
        {
            if (string.IsNullOrWhiteSpace(descripcion))
            {
                return "Es necesario la descripción del Producto\n";
            }

            if (!Regex.IsMatch(descripcion, @"^(?=.*[a-zA-ZñÑáéíóúÁÉÍÓÚ])[a-zA-ZñÑáéíóúÁÉÍÓÚ0-9\s]*(\.[a-zA-ZñÑáéíóúÁÉÍÓÚ0-9\s]*)?$"))
            {
                return "La descripción debe contener al menos una letra, puede incluir números, tildes, ñ, y puntos solo si están acompañados de letras\n";
            }

            if (descripcion.Length < 5 || descripcion.Length > 500)
            {
                return "La descripción debe tener entre 5 y 500 caracteres\n";
            }

            return string.Empty;
        }
    }
}
