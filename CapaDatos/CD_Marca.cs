using CapaEntidad;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaDatos
{
    public class CD_Marca
    {
            public List<Marca> Listar()
            {
                List<Marca> lista = new List<Marca>();

                using (SqlConnection oconexion = new SqlConnection(Conexion.cadena))
                {
                    try
                    {
                        StringBuilder query = new StringBuilder();
                        query.AppendLine("SELECT IdMarca, Nombre, LugarOrigen, Estado FROM MARCA");

                        SqlCommand cmd = new SqlCommand(query.ToString(), oconexion);
                        cmd.CommandType = CommandType.Text;

                        oconexion.Open();

                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                lista.Add(new Marca()
                                {
                                    IdMarca = Convert.ToInt32(dr["IdMarca"]),
                                    Nombre = dr["Nombre"].ToString(),
                                    LugarOrigen = dr["LugarOrigen"].ToString(),
                                    Estado = Convert.ToBoolean(dr["Estado"])
                                });
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        lista = new List<Marca>();
                    }
                }

                return lista;
            }

            public int Registrar(Marca obj, out string Mensaje)
            {
                int idMarcagenerado = 0;
                Mensaje = string.Empty;

                try
                {
                    using (SqlConnection oconexion = new SqlConnection(Conexion.cadena))
                    {
                        SqlCommand cmd = new SqlCommand("sp_RegistrarMarca", oconexion);
                        cmd.Parameters.AddWithValue("Nombre", obj.Nombre);
                        cmd.Parameters.AddWithValue("LugarOrigen", obj.LugarOrigen);
                        cmd.Parameters.AddWithValue("Estado", obj.Estado);
                        cmd.Parameters.Add("Resultado", SqlDbType.Int).Direction = ParameterDirection.Output;
                        cmd.Parameters.Add("Mensaje", SqlDbType.VarChar, 500).Direction = ParameterDirection.Output;
                        cmd.CommandType = CommandType.StoredProcedure;

                        oconexion.Open();
                        cmd.ExecuteNonQuery();

                        idMarcagenerado = Convert.ToInt32(cmd.Parameters["Resultado"].Value);
                        Mensaje = cmd.Parameters["Mensaje"].Value.ToString();
                    }
                }
                catch (Exception ex)
                {
                    idMarcagenerado = 0;
                    Mensaje = ex.Message;
                }

                return idMarcagenerado;
            }


            public bool Editar(Marca obj, out string Mensaje)
            {
                bool respuesta = false;
                Mensaje = string.Empty;

                try
                {
                    using (SqlConnection oconexion = new SqlConnection(Conexion.cadena))
                    {
                        SqlCommand cmd = new SqlCommand("sp_ModificarMarca", oconexion);

                        cmd.Parameters.AddWithValue("IdMarca", obj.IdMarca);
                        cmd.Parameters.AddWithValue("Nombre", obj.Nombre);
                        cmd.Parameters.AddWithValue("LugarOrigen", obj.LugarOrigen);
                        cmd.Parameters.AddWithValue("Estado", obj.Estado);
                        cmd.Parameters.Add("Resultado", SqlDbType.Bit).Direction = ParameterDirection.Output;
                        cmd.Parameters.Add("Mensaje", SqlDbType.VarChar, 500).Direction = ParameterDirection.Output;
                        cmd.CommandType = CommandType.StoredProcedure;

                        oconexion.Open();
                        cmd.ExecuteNonQuery();

                        respuesta = Convert.ToBoolean(cmd.Parameters["Resultado"].Value);
                        Mensaje = cmd.Parameters["Mensaje"].Value.ToString();
                    }
                }
                catch (Exception ex)
                {
                    respuesta = false;
                    Mensaje = ex.Message;
                }

                return respuesta;
            }


            public bool Eliminar(Marca obj, out string Mensaje)
            {
                bool respuesta = false;
                Mensaje = string.Empty;

                try
                {
                    using (SqlConnection oconexion = new SqlConnection(Conexion.cadena))
                    {
                        SqlCommand cmd = new SqlCommand("DELETE FROM MARCA WHERE IdMarca = @id", oconexion);
                        cmd.Parameters.AddWithValue("@id", obj.IdMarca);
                        cmd.CommandType = CommandType.Text;

                        oconexion.Open();
                        respuesta = cmd.ExecuteNonQuery() > 0 ? true : false;
                    }
                }
                catch (Exception ex)
                {
                    respuesta = false;
                    Mensaje = ex.Message;
                }

                return respuesta;
            }
        }
    }
