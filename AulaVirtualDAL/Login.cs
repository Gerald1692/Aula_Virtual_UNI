using Entities;
using Microsoft.Data.SqlClient;
using System.Data;

namespace AulaVirtualDAL
{
    public class Login
    {
        public Respuesta<Usuario> IniciarSesion(string Usuario, string Contrasena, string Conexion)
        {
            Respuesta<Usuario> respuesta = new Respuesta<Usuario>();

            try
            {
                using (SqlConnection connection = new SqlConnection(Conexion))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand("spIniciarSesion", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add(new SqlParameter("@pNombreUsuario", SqlDbType.NVarChar, 80) { Value = Usuario });
                        command.Parameters.Add(new SqlParameter("@pContrasena", SqlDbType.NVarChar, 50) { Value = Contrasena });

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                int exito = reader.GetInt32(reader.GetOrdinal("Exito"));

                                if (exito == 1)
                                {
                                    // Lectura robusta de columnas para soportar SP nuevo y viejo
                                    string nombreCompleto;
                                    try { nombreCompleto = reader["NombreCompleto"].ToString(); }
                                    catch { nombreCompleto = reader["NombreUsuario"].ToString(); }

                                    int rolId;
                                    try { rolId = Convert.ToInt32(reader["RolId"]); }
                                    catch { rolId = Convert.ToInt32(reader["id_rol"]); }

                                    int usuarioId;
                                    try { usuarioId = Convert.ToInt32(reader["UsuarioId"]); }
                                    catch { usuarioId = Convert.ToInt32(reader["id_usuario"]); }

                                    Usuario usuario = new Usuario
                                    {
                                        NombreCompleto = nombreCompleto,
                                        RolId = rolId,
                                        Id = usuarioId
                                    };

                                    respuesta.Ok = true;
                                    respuesta.Mensaje = "Inicio de sesión exitoso.";
                                    respuesta.ValorRetorno = usuario;
                                }
                                else
                                {
                                    respuesta.Ok = false;
                                    respuesta.Mensaje = "Usuario o contraseña incorrectos.";
                                }
                            }
                            else
                            {
                                respuesta.Ok = false;
                                respuesta.Mensaje = "Usuario o contraseña incorrectos.";
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                respuesta.Ok = false;
                respuesta.Mensaje = ex.Message;
            }

            return respuesta;
        }

        public Respuesta<string> RecuperarContrasena(string Usuario, string Conexion)
        {
            Respuesta<string> respuesta = new Respuesta<string>();

            try
            {
                using (SqlConnection connection = new SqlConnection(Conexion))
                {
                    connection.Open();

                    // Consulta directa para buscar la contraseña por correo o nombre de usuario
                    string query = @"
                        SELECT contrasena 
                        FROM usuarios 
                        WHERE correo = @Usuario OR nombre = @Usuario";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.Add(new SqlParameter("@Usuario", SqlDbType.NVarChar, 100) { Value = Usuario });

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string contrasena = reader["contrasena"].ToString();
                                respuesta.Ok = true;
                                respuesta.Mensaje = "Contraseña recuperada exitosamente.";
                                respuesta.ValorRetorno = contrasena;
                            }
                            else
                            {
                                respuesta.Ok = false;
                                respuesta.Mensaje = "No se encontró un usuario con ese correo o nombre de usuario.";
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                respuesta.Ok = false;
                respuesta.Mensaje = ex.Message;
            }

            return respuesta;
        }
    }
}
