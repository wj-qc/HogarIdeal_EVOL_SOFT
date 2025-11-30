using CapaDatos;
using CapaEntidad;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaNegocio
{
    public class CN_Marca
    {
            private CD_Marca objcd_Marca = new CD_Marca();

            public List<Marca> Listar()
            {
                return objcd_Marca.Listar();
            }

            public int Registrar(Marca obj, out string Mensaje)
            {
                Mensaje = string.Empty;

                if (obj.Nombre == "")
                {
                    Mensaje += "Es necesario el nombre de la marca\n";
                }

                if (obj.LugarOrigen == "")
                {
                    Mensaje += "Es necesario el lugar de origen de la marca\n";
                }

                if (Mensaje != string.Empty)
                {
                    return 0;
                }
                else
                {
                    return objcd_Marca.Registrar(obj, out Mensaje);
                }
            }

            public bool Editar(Marca obj, out string Mensaje)
            {
                Mensaje = string.Empty;

                if (obj.Nombre == "")
                {
                    Mensaje += "Es necesario el nombre de la marca\n";
                }

                if (obj.LugarOrigen == "")
                {
                    Mensaje += "Es necesario el lugar de origen de la marca\n";
                }

                if (Mensaje != string.Empty)
                {
                    return false;
                }
                else
                {
                    return objcd_Marca.Editar(obj, out Mensaje);
                }
            }

            public bool Eliminar(Marca obj, out string Mensaje)
            {
                return objcd_Marca.Eliminar(obj, out Mensaje);
            }

        }
    }
