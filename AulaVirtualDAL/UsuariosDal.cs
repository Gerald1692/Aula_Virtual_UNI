using Entities;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace AulaVirtualDAL
{
    public class UsuariosDAL
    {
        public Respuesta<List<Usuario>> ObtenerEstudiantes(string Conexion)
        {
            Respuesta<List<Usuario>> respuesta = new Respuesta<List<Usuario>>();
            List<Usuario> ListaEstudiantes = new List<Usuario>();
            try
            {
                using (SqlConnection connection = new SqlConnection(Conexion))
                {
                    connection.Open();

<<<<<<< HEAD
=======
<<<<<<< HEAD
>>>>>>> e11b38268a0ef68b2d9ae13e5c352ba303e8ce26
                    // Consulta directa con el rol correcto: id_rol = 1 es Estudiante
                    string query = @"
                        SELECT 
                            u.id_usuario AS Id,
                            CONCAT(u.nombre, ' ', u.apellido1, ' ', ISNULL(u.apellido2, '')) AS NombreCompleto,
                            u.cedula     AS Matricula,
                            u.correo     AS Email
                        FROM dbo.usuarios u
                        WHERE u.id_rol = 1  -- 1 = Estudiante
                        ORDER BY u.nombre, u.apellido1";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
<<<<<<< HEAD
=======
=======
                    using (SqlCommand command = new SqlCommand("ObtenerEstudiantes", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

>>>>>>> f84fcff83333d92b5c4b91115f1c0fe0cbab39f6
>>>>>>> e11b38268a0ef68b2d9ae13e5c352ba303e8ce26
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Usuario usuario = new Usuario
                                {
                                    Id = (int)reader["Id"],
                                    NombreCompleto = (string)reader["NombreCompleto"],
                                    Cedula = (string)reader["Matricula"],
                                    Email = (string)reader["Email"]
                                };

                                ListaEstudiantes.Add(usuario);
                            }

                            respuesta.Ok = true;
                            respuesta.Mensaje = "Datos obtenidos de manera exitosa";
                            respuesta.ValorRetorno = ListaEstudiantes;
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
