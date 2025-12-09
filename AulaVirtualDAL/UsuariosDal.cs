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

        public Respuesta<List<Usuario>> ObtenerEstudiantesDeOtrosProyectos(int EstudianteId, string Conexion)
        {
            Respuesta<List<Usuario>> respuesta = new Respuesta<List<Usuario>>();
            List<Usuario> ListaEstudiantes = new List<Usuario>();
            try
            {
                using (SqlConnection connection = new SqlConnection(Conexion))
                {
                    connection.Open();

                    // Obtener estudiantes que NO están en los mismos proyectos que el estudiante actual
                    string query = @"
                        SELECT DISTINCT
                            u.id_usuario AS Id,
                            CONCAT(u.nombre, ' ', u.apellido1, ' ', ISNULL(u.apellido2, '')) AS NombreCompleto,
                            u.cedula     AS Matricula,
                            u.correo     AS Email
                        FROM dbo.usuarios u
                        INNER JOIN dbo.proyecto_estudiante pe ON u.id_usuario = pe.id_estudiante
                        WHERE u.id_rol = 1  -- Solo estudiantes
                          AND u.id_usuario != @EstudianteId  -- Excluir al estudiante actual
                          AND pe.id_proyecto NOT IN (
                              -- Proyectos donde está el estudiante actual
                              SELECT pe2.id_proyecto
                              FROM dbo.proyecto_estudiante pe2
                              WHERE pe2.id_estudiante = @EstudianteId
                          )
                        ORDER BY u.nombre, u.apellido1";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.Add(new SqlParameter("@EstudianteId", SqlDbType.Int) { Value = EstudianteId });

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

        public Respuesta<Usuario> ObtenerUsuarioPorId(int UsuarioId, string Conexion)
        {
            Respuesta<Usuario> respuesta = new Respuesta<Usuario>();
            try
            {
                using (SqlConnection connection = new SqlConnection(Conexion))
                {
                    connection.Open();

                    string query = @"
                        SELECT 
                            u.id_usuario AS Id,
                            u.nombre AS Nombre,
                            u.apellido1 AS Apellido1,
                            u.apellido2 AS Apellido2,
                            CONCAT(u.nombre, ' ', u.apellido1, ' ', ISNULL(u.apellido2, '')) AS NombreCompleto,
                            u.cedula AS Cedula,
                            u.correo AS Email
                        FROM dbo.usuarios u
                        WHERE u.id_usuario = @UsuarioId";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.Add(new SqlParameter("@UsuarioId", SqlDbType.Int) { Value = UsuarioId });

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                Usuario usuario = new Usuario
                                {
                                    Id = (int)reader["Id"],
                                    Nombre = reader["Nombre"] != DBNull.Value ? (string)reader["Nombre"] : string.Empty,
                                    Apellido1 = reader["Apellido1"] != DBNull.Value ? (string)reader["Apellido1"] : string.Empty,
                                    Apellido2 = reader["Apellido2"] != DBNull.Value ? (string)reader["Apellido2"] : null,
                                    NombreCompleto = reader["NombreCompleto"] != DBNull.Value ? (string)reader["NombreCompleto"] : string.Empty,
                                    Cedula = reader["Cedula"] != DBNull.Value ? (string)reader["Cedula"] : string.Empty,
                                    Email = reader["Email"] != DBNull.Value ? (string)reader["Email"] : string.Empty
                                };

                                respuesta.Ok = true;
                                respuesta.Mensaje = "Usuario obtenido de manera exitosa";
                                respuesta.ValorRetorno = usuario;
                            }
                            else
                            {
                                respuesta.Ok = false;
                                respuesta.Mensaje = "Usuario no encontrado";
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
