using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad
{
    public class RecursoInterno
    {
        public int IdRecurso { get; set; }
        public string NombreRecurso { get; set; }
        public string TipoRecurso { get; set; }
        public int Cantidad { get; set; }
        public string Ubicacion { get; set; }
        public bool Estado { get; set; }
        public string FechaRegistro { get; set; }
    }
}
