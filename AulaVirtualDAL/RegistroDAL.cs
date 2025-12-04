using Entities;
using Microsoft.Data.SqlClient;
using System.Data;

namespace AulaVirtualDAL
{
    public class RegistroDAL
    {
        public Respuesta<Usuario> RegistrarUsuario(string nombre, string apellido1, string apellido2, 
            string cedula, string telefono, string correo, string sede, string contrasena, string conexion)
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
                        command.Parameters.Add(new SqlParameter("@pApellido1", SqlDbType.NVarChar, 100) { Value = apellido1 });
                        command.Parameters.Add(new SqlParameter("@pApellido2", SqlDbType.NVarChar, 100) { Value = apellido2 });
                        command.Parameters.Add(new SqlParameter("@pCedula", SqlDbType.NVarChar, 20) { Value = cedula });
                        command.Parameters.Add(new SqlParameter("@pTelefono", SqlDbType.NVarChar, 20) { Value = telefono });
                        command.Parameters.Add(new SqlParameter("@pCorreo", SqlDbType.NVarChar, 150) { Value = correo });
                        command.Parameters.Add(new SqlParameter("@pSede", SqlDbType.NVarChar, 150) { Value = sede });
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
                                        Id = Convert.ToInt32(reader["id_usuario"]),
                                        NombreCompleto = reader["NombreCompleto"].ToString(),
                                        RolId = Convert.ToInt32(reader["id_rol"])
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

        public Respuesta<bool> VerificarCedulaExistente(string cedula, string conexion)
        {
            Respuesta<bool> respuesta = new Respuesta<bool>();

            try
            {
                using (SqlConnection connection = new SqlConnection(conexion))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand("spVerificarCedula", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add(new SqlParameter("@pCedula", SqlDbType.NVarChar, 20) { Value = cedula });

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
