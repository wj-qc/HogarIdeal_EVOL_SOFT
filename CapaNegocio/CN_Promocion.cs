using CapaDatos;
using CapaEntidad;
using System;

namespace CapaNegocio
{
    public class CN_Promocion
    {
        private CD_Promocion objcd_Promocion = new CD_Promocion();

        public System.Collections.Generic.List<Promocion> Listar()
        {
            return objcd_Promocion.Listar();
        }

        public int Registrar(Promocion obj, out string Mensaje)
        {
            Mensaje = string.Empty;

            if (string.IsNullOrWhiteSpace(obj.Nombre))
            {
                Mensaje += "Es necesario el nombre de la promoción\n";
            }

            if (obj.PorcentajeDescuento < 0 || obj.PorcentajeDescuento > 100)
            {
                Mensaje += "El porcentaje de descuento debe estar entre 0 y 100\n";
            }

            if (obj.FechaFin <= obj.FechaInicio)
            {
                Mensaje += "La fecha de fin debe ser mayor a la fecha de inicio\n";
            }

            if (Mensaje != string.Empty)
            {
                return 0;
            }
            else
            {
                return objcd_Promocion.Registrar(obj, out Mensaje);
            }
        }

        public bool Editar(Promocion obj, out string Mensaje)
        {
            Mensaje = string.Empty;

            if (string.IsNullOrWhiteSpace(obj.Nombre))
            {
                Mensaje += "Es necesario el nombre de la promoción\n";
            }

            if (obj.PorcentajeDescuento < 0 || obj.PorcentajeDescuento > 100)
            {
                Mensaje += "El porcentaje de descuento debe estar entre 0 y 100\n";
            }

            if (obj.FechaFin <= obj.FechaInicio)
            {
                Mensaje += "La fecha de fin debe ser mayor a la fecha de inicio\n";
            }

            if (Mensaje != string.Empty)
            {
                return false;
            }
            else
            {
                return objcd_Promocion.Editar(obj, out Mensaje);
            }
        }

        public bool Eliminar(Promocion obj, out string Mensaje)
        {
            return objcd_Promocion.Eliminar(obj, out Mensaje);
        }
    }
}


