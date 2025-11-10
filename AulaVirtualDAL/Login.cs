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
            Usuario usuarios = new Usuario();
            try
            {

                using (SqlConnection connection = new SqlConnection(Conexion))
                {

                    connection.Open();

                    using (SqlCommand command = new SqlCommand("spIniciarSesion", connection))
                    {

                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add(new SqlParameter("@pNombreUsuario", SqlDbType.NVarChar, 70) { Value = Usuario });
                        command.Parameters.Add(new SqlParameter("@pContrasena", SqlDbType.NVarChar, 70) { Value = Contrasena });

                        using (SqlDataReader reader = command.ExecuteReader())
                        {

                            if (reader.Read())
                            {

                                int Exito = (int)reader["Exito"];


                                if (Exito == 1)
                                {
                                    Usuario usuario = new Usuario
                                    {
                                        NombreUsuario = (string)reader["NombreCompleto"]

                                    };


                                    respuesta.Ok = true;
                                    respuesta.Mensaje = "Inicio de sesión exitoso!";
                                    respuesta.ValorRetorno = usuarios;


                                }
                                else
                                {

                                    respuesta.Ok = false;
                                    respuesta.Mensaje = "El usuario o  la contraseña no son correctos";
                                }


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
