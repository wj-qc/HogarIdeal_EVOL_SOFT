using CapaDatos;
using CapaEntidad;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaNegocio
{
    public class CN_RecursoInterno
    {
        private CD_RecursoInterno objCapaDatos = new CD_RecursoInterno();

        public List<RecursoInterno> Listar()
        {
            return objCapaDatos.Listar();
        }

        public int Registrar(RecursoInterno obj, out string mensaje)
        {
            mensaje = string.Empty;

            // Validaciones básicas de negocio
            if (string.IsNullOrWhiteSpace(obj.NombreRecurso))
            {
                mensaje = "El nombre del recurso es obligatorio.";
                return 0;
            }

            if (obj.Cantidad < 0)
            {
                mensaje = "La cantidad no puede ser negativa.";
                return 0;
            }

            return objCapaDatos.Registrar(obj, out mensaje);
        }

        public bool Editar(RecursoInterno obj, out string mensaje)
        {
            mensaje = string.Empty;

            // Validaciones básicas de negocio
            if (obj.IdRecurso <= 0)
            {
                mensaje = "El identificador del recurso no es válido.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(obj.NombreRecurso))
            {
                mensaje = "El nombre del recurso es obligatorio.";
                return false;
            }

            if (obj.Cantidad < 0)
            {
                mensaje = "La cantidad no puede ser negativa.";
                return false;
            }

            return objCapaDatos.Editar(obj, out mensaje);
        }

        public bool Eliminar(int idRecurso, out string mensaje)
        {
            mensaje = string.Empty;

            if (idRecurso <= 0)
            {
                mensaje = "El identificador del recurso no es válido.";
                return false;
            }

            return objCapaDatos.Eliminar(idRecurso, out mensaje);
        }
    }
}
