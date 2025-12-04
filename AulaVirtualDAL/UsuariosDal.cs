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

                    // Consulta directa con el rol correcto: id_rol = 1 es Estudiante (según tabla roles)
                    // Nota: El stored procedure ObtenerEstudiantes filtra por id_rol = 2 (Profesor), lo cual es incorrecto
                    string query = @"
                        SELECT 
                            u.id_usuario AS Id,
                            CONCAT(u.nombre, ' ', u.apellido1, ' ', ISNULL(u.apellido2, '')) AS NombreCompleto,
                            u.cedula     AS Matricula,
                            u.correo     AS Email
                        FROM dbo.usuarios u
                        WHERE u.id_rol = 1  -- 1 = Estudiante (según tabla roles)
                        ORDER BY u.nombre, u.apellido1";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
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
