using CapaEntidad;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaDatos
{
    public class CD_RecursoInterno
    {
        public List<RecursoInterno> Listar()
        {
            List<RecursoInterno> lista = new List<RecursoInterno>();

            try
            {
                using (SqlConnection conexion = new SqlConnection(Conexion.cadena))
                {
                    string query = "SELECT IdRecurso, NombreRecurso, TipoRecurso, Cantidad, Ubicacion, Estado, CONVERT(VARCHAR(10), FechaRegistro, 103) AS FechaRegistro FROM RECURSO_INTERNO";

                    SqlCommand cmd = new SqlCommand(query, conexion);
                    cmd.CommandType = CommandType.Text;

                    conexion.Open();
                    SqlDataReader dr = cmd.ExecuteReader();

                    while (dr.Read())
                    {
                        lista.Add(new RecursoInterno()
                        {
                            IdRecurso = Convert.ToInt32(dr["IdRecurso"]),
                            NombreRecurso = dr["NombreRecurso"].ToString(),
                            TipoRecurso = dr["TipoRecurso"].ToString(),
                            Cantidad = Convert.ToInt32(dr["Cantidad"]),
                            Ubicacion = dr["Ubicacion"].ToString(),
                            Estado = Convert.ToBoolean(dr["Estado"]),
                            FechaRegistro = dr["FechaRegistro"].ToString()
                        });
                    }
                }
            }
            catch
            {
                lista = new List<RecursoInterno>();
            }

            return lista;
        }


        public int Registrar(RecursoInterno obj, out string mensaje)
        {
            mensaje = "";
            int idGenerado = 0;

            try
            {
                using (SqlConnection conexion = new SqlConnection(Conexion.cadena))
                {
                    string query = @"
                        INSERT INTO RECURSO_INTERNO (NombreRecurso, TipoRecurso, Cantidad, Ubicacion, Estado)
                        VALUES (@NombreRecurso, @TipoRecurso, @Cantidad, @Ubicacion, @Estado);
                        SELECT SCOPE_IDENTITY();
                    ";

                    SqlCommand cmd = new SqlCommand(query, conexion);
                    cmd.CommandType = CommandType.Text;

                    cmd.Parameters.AddWithValue("@NombreRecurso", obj.NombreRecurso);
                    cmd.Parameters.AddWithValue("@TipoRecurso", obj.TipoRecurso ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Cantidad", obj.Cantidad);
                    cmd.Parameters.AddWithValue("@Ubicacion", obj.Ubicacion ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Estado", obj.Estado);

                    conexion.Open();
                    idGenerado = Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
            catch (Exception ex)
            {
                idGenerado = 0;
                mensaje = ex.Message;
            }

            return idGenerado;
        }


        public bool Editar(RecursoInterno obj, out string mensaje)
        {
            mensaje = "";
            bool resultado = false;

            try
            {
                using (SqlConnection conexion = new SqlConnection(Conexion.cadena))
                {
                    string query = @"
                        UPDATE RECURSO_INTERNO
                        SET NombreRecurso = @NombreRecurso,
                            TipoRecurso = @TipoRecurso,
                            Cantidad = @Cantidad,
                            Ubicacion = @Ubicacion,
                            Estado = @Estado
                        WHERE IdRecurso = @IdRecurso
                    ";

                    SqlCommand cmd = new SqlCommand(query, conexion);
                    cmd.CommandType = CommandType.Text;

                    cmd.Parameters.AddWithValue("@IdRecurso", obj.IdRecurso);
                    cmd.Parameters.AddWithValue("@NombreRecurso", obj.NombreRecurso);
                    cmd.Parameters.AddWithValue("@TipoRecurso", obj.TipoRecurso ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Cantidad", obj.Cantidad);
                    cmd.Parameters.AddWithValue("@Ubicacion", obj.Ubicacion ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Estado", obj.Estado);

                    conexion.Open();
                    resultado = cmd.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception ex)
            {
                resultado = false;
                mensaje = ex.Message;
            }

            return resultado;
        }


        public bool Eliminar(int id, out string mensaje)
        {
            mensaje = "";
            bool respuesta = false;

            try
            {
                using (SqlConnection conexion = new SqlConnection(Conexion.cadena))
                {
                    string query = "DELETE FROM RECURSO_INTERNO WHERE IdRecurso = @IdRecurso";

                    SqlCommand cmd = new SqlCommand(query, conexion);
                    cmd.CommandType = CommandType.Text;

                    cmd.Parameters.AddWithValue("@IdRecurso", id);

                    conexion.Open();
                    respuesta = cmd.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception ex)
            {
                respuesta = false;
                mensaje = ex.Message;
            }

            return respuesta;
        }
    }
}
