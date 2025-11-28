using Entities;
using Microsoft.Data.SqlClient;
using System.Data;

namespace AulaVirtualDAL
{
    public class RegistroDAL
    {
        public Respuesta<Usuario> RegistrarUsuario(string nombre, string apellido, string matricula, 
            string carrera, string email, string contrasena, string conexion)
        {
            Respuesta<Usuario> respuesta = new Respuesta<Usuario>();

            try
            {
                using (SqlConnection connection = new SqlConnection(conexion))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand("spRegistrarUsuario", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Parámetros de entrada
                        command.Parameters.Add(new SqlParameter("@pNombre", SqlDbType.NVarChar, 100) { Value = nombre });
                        command.Parameters.Add(new SqlParameter("@pApellido", SqlDbType.NVarChar, 100) { Value = apellido });
                        command.Parameters.Add(new SqlParameter("@pMatricula", SqlDbType.NVarChar, 50) { Value = matricula });
                        command.Parameters.Add(new SqlParameter("@pCarrera", SqlDbType.NVarChar, 150) { Value = carrera });
                        command.Parameters.Add(new SqlParameter("@pEmail", SqlDbType.NVarChar, 150) { Value = email });
                        command.Parameters.Add(new SqlParameter("@pContrasena", SqlDbType.NVarChar, 50) { Value = contrasena });

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                int exito = reader.GetInt32(reader.GetOrdinal("Exito"));
                                string mensaje = reader.GetString(reader.GetOrdinal("Mensaje"));

                                if (exito == 1)
                                {
                                    Usuario usuario = new Usuario
                                    {
                                        IdUsuario = Convert.ToInt32(reader["id_usuario"]),
                                        NombreUsuario = reader["NombreCompleto"].ToString(),
                                        Rol = reader["id_rol"].ToString()
                                    };

                                    respuesta.Ok = true;
                                    respuesta.Mensaje = mensaje;
                                    respuesta.ValorRetorno = usuario;
                                }
                                else
                                {
                                    respuesta.Ok = false;
                                    respuesta.Mensaje = mensaje;
                                }
                            }
                            else
                            {
                                respuesta.Ok = false;
                                respuesta.Mensaje = "Error al procesar el registro.";
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                respuesta.Ok = false;
                respuesta.Mensaje = "Error: " + ex.Message;
            }

            return respuesta;
        }

        public Respuesta<bool> VerificarMatriculaExistente(string matricula, string conexion)
        {
            Respuesta<bool> respuesta = new Respuesta<bool>();

            try
            {
                using (SqlConnection connection = new SqlConnection(conexion))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand("spVerificarMatricula", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add(new SqlParameter("@pMatricula", SqlDbType.NVarChar, 50) { Value = matricula });

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                bool existe = reader.GetInt32(reader.GetOrdinal("Existe")) == 1;
                                respuesta.Ok = true;
                                respuesta.ValorRetorno = existe;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                respuesta.Ok = false;
                respuesta.Mensaje = "Error: " + ex.Message;
            }

            return respuesta;
        }
    }
}
