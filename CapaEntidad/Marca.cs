using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad
{
    public class Marca
    {
        public int IdMarca { get; set; }
        public string Nombre { get; set; }
        public string LugarOrigen { get; set; }
        public bool Estado { get; set; }
        public string FechaRegistro { get; set; }
    }
}
