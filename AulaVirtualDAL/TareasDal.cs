using Entities;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AulaVirtualDAL
{
    public class TareasDAL
    {

        public Respuesta<Tarea> InsertarTarea(Tarea Tarea, string Conexion)
        {
            Respuesta<Tarea> respuesta = new Respuesta<Tarea>();

            try
            {
                using (SqlConnection connection = new SqlConnection(Conexion))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand("InsertarTarea", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add(new SqlParameter("@Titulo", SqlDbType.NVarChar, 50) { Value = Tarea.Titulo });
                        command.Parameters.Add(new SqlParameter("@Descripcion", SqlDbType.NVarChar, 250) { Value = Tarea.Descripcion });
                        command.Parameters.Add(new SqlParameter("@ProyectoId", SqlDbType.Int) { Value = Tarea.ProyectoId });
                        command.Parameters.Add(new SqlParameter("@EstudianteAsignadoId", SqlDbType.Int) { Value = Tarea.EstudianteAsignadoId ?? 0 });
                        command.Parameters.Add(new SqlParameter("@FechaInicio", SqlDbType.Date) { Value = Tarea.FechaInicio.HasValue ? (object)Tarea.FechaInicio.Value : DBNull.Value });
                        command.Parameters.Add(new SqlParameter("@FechaLimite", SqlDbType.Date) { Value = Tarea.FechaLimite });
                        command.Parameters.Add(new SqlParameter("@Estado", SqlDbType.NVarChar, 20) { Value = Tarea.Estado });


                        object result = command.ExecuteScalar();

                        if (result != null && int.TryParse(result.ToString(), out int newId))
                        {
                            Tarea.Id = newId;
                            respuesta.Ok = true;
                            respuesta.Mensaje = $"La tarea ha sido agregada de manera exitosa";
                            respuesta.ValorRetorno = Tarea;
                        }
                        else
                        {
                            respuesta.Ok = false;
                            respuesta.Mensaje = "Ha ocurrido un error a la hora de agregar la tarea";
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

        public Respuesta<List<Tarea>> ObtenerTareasPorEstudiante(int EstudianteId, string Conexion)
        {
            Respuesta<List<Tarea>> respuesta = new Respuesta<List<Tarea>>();
            List<Tarea> ListaTareas = new List<Tarea>();
            try
            {
                using (SqlConnection connection = new SqlConnection(Conexion))
                {
                    connection.Open();

                    string query = @"
                        SELECT DISTINCT
                            t.id_tarea AS Id,
                            t.titulo AS Titulo,
                            t.descripcion AS Descripcion,
                            t.id_proyecto AS ProyectoId,
                            t.id_asignado AS EstudianteAsignadoId,
                            t.fecha_inicio AS FechaInicio,
                            t.fecha_limite AS FechaLimite,
                            t.estado AS Estado,
                            p.curso AS Curso,
                            CONCAT(u.nombre, ' ', u.apellido1, ' ', ISNULL(u.apellido2, '')) AS NombreAsignado
                        FROM dbo.tareas t
                        INNER JOIN dbo.proyectos p ON t.id_proyecto = p.id_proyecto
                        LEFT JOIN dbo.usuarios u ON t.id_asignado = u.id_usuario
                        WHERE t.id_asignado = @EstudianteId
                           OR EXISTS (
                               SELECT 1 
                               FROM dbo.tarea_estudiante te 
                               WHERE te.id_tarea = t.id_tarea 
                                 AND te.id_estudiante = @EstudianteId
                           )
                        ORDER BY t.fecha_limite DESC";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.Add(new SqlParameter("@EstudianteId", SqlDbType.Int) { Value = EstudianteId });

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Tarea Tarea = new Tarea
                                {
                                    Id = reader.IsDBNull("Id") ? 0 : reader.GetInt32("Id"),
                                    Titulo = reader.IsDBNull("Titulo") ? string.Empty : reader.GetString("Titulo"),
                                    Descripcion = reader.IsDBNull("Descripcion") ? string.Empty : reader.GetString("Descripcion"),
                                    ProyectoId = reader.IsDBNull("ProyectoId") ? 0 : reader.GetInt32("ProyectoId"),
                                    EstudianteAsignadoId = reader.IsDBNull("EstudianteAsignadoId") ? null : reader.GetInt32("EstudianteAsignadoId"),
                                    FechaInicio = reader.IsDBNull("FechaInicio") ? null : (DateTime?)reader.GetDateTime("FechaInicio"),
                                    FechaLimite = reader.IsDBNull("FechaLimite") ? DateTime.MinValue : reader.GetDateTime("FechaLimite"),
                                    Estado = reader.IsDBNull("Estado") ? "pendiente" : reader.GetString("Estado"),
                                    NombreAsignado = reader.IsDBNull("NombreAsignado") ? null : reader.GetString("NombreAsignado"),
                                    Curso = reader.IsDBNull("Curso") ? null : reader.GetString("Curso"),
                                    EstudiantesAsignados = new List<Usuario>()
                                };

                                ListaTareas.Add(Tarea);
                            }
                        }

                        // Obtener todos los estudiantes asignados para cada tarea
                        foreach (var tarea in ListaTareas)
                        {
                            var estudiantesRespuesta = ObtenerEstudiantesPorTarea(tarea.Id, Conexion);
                            if (estudiantesRespuesta.Ok && estudiantesRespuesta.ValorRetorno != null)
                            {
                                tarea.EstudiantesAsignados = estudiantesRespuesta.ValorRetorno;
                                
                                if (tarea.EstudiantesAsignados.Count > 1)
                                {
                                    tarea.NombreAsignado = string.Join(", ", tarea.EstudiantesAsignados.Select(e => e.NombreCompleto));
                                }
                                else if (tarea.EstudiantesAsignados.Count == 1)
                                {
                                    tarea.NombreAsignado = tarea.EstudiantesAsignados[0].NombreCompleto;
                                }
                            }
                        }

                        respuesta.Ok = true;
                        respuesta.Mensaje = $"Datos obtenidos de manera exitosa";
                        respuesta.ValorRetorno = ListaTareas;
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

        public Respuesta<List<Tarea>> ObtenerTareasCreadasPorEstudiante(int EstudianteId, string Conexion)
        {
            Respuesta<List<Tarea>> respuesta = new Respuesta<List<Tarea>>();
            List<Tarea> ListaTareas = new List<Tarea>();
            try
            {
                using (SqlConnection connection = new SqlConnection(Conexion))
                {
                    connection.Open();

                    string query = @"
                        SELECT 
                            t.id_tarea AS Id,
                            t.titulo AS Titulo,
                            t.descripcion AS Descripcion,
                            t.id_proyecto AS ProyectoId,
                            t.id_asignado AS EstudianteAsignadoId,
                            t.fecha_inicio AS FechaInicio,
                            t.fecha_limite AS FechaLimite,
                            t.estado AS Estado,
                            p.curso AS Curso,
                            CONCAT(u.nombre, ' ', u.apellido1, ' ', ISNULL(u.apellido2, '')) AS NombreAsignado
                        FROM dbo.tareas t
                        INNER JOIN dbo.proyectos p ON t.id_proyecto = p.id_proyecto
                        LEFT JOIN dbo.usuarios u ON t.id_asignado = u.id_usuario
                        WHERE t.id_asignado = @EstudianteId
                          -- Excluir tareas que tienen registros en tarea_estudiante (asignadas por profesor)
                          AND NOT EXISTS (
                              SELECT 1 
                              FROM dbo.tarea_estudiante te 
                              WHERE te.id_tarea = t.id_tarea
                          )
                          -- Excluir tareas donde el estudiante está en proyecto_estudiante (proyecto asignado por profesor)
                          AND NOT EXISTS (
                              SELECT 1 
                              FROM dbo.proyecto_estudiante pe 
                              WHERE pe.id_proyecto = t.id_proyecto 
                                AND pe.id_estudiante = @EstudianteId
                          )
                          -- Solo mostrar tareas en proyectos asignados directamente al estudiante
                          -- (no proyectos asignados por profesor mediante proyecto_estudiante)
                          AND p.id_asignado = @EstudianteId
                        ORDER BY t.fecha_limite DESC";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.Add(new SqlParameter("@EstudianteId", SqlDbType.Int) { Value = EstudianteId });

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Tarea Tarea = new Tarea
                                {
                                    Id = reader.IsDBNull("Id") ? 0 : reader.GetInt32("Id"),
                                    Titulo = reader.IsDBNull("Titulo") ? string.Empty : reader.GetString("Titulo"),
                                    Descripcion = reader.IsDBNull("Descripcion") ? string.Empty : reader.GetString("Descripcion"),
                                    ProyectoId = reader.IsDBNull("ProyectoId") ? 0 : reader.GetInt32("ProyectoId"),
                                    EstudianteAsignadoId = reader.IsDBNull("EstudianteAsignadoId") ? null : reader.GetInt32("EstudianteAsignadoId"),
                                    FechaInicio = reader.IsDBNull("FechaInicio") ? null : (DateTime?)reader.GetDateTime("FechaInicio"),
                                    FechaLimite = reader.IsDBNull("FechaLimite") ? DateTime.MinValue : reader.GetDateTime("FechaLimite"),
                                    Estado = reader.IsDBNull("Estado") ? "pendiente" : reader.GetString("Estado"),
                                    NombreAsignado = reader.IsDBNull("NombreAsignado") ? null : reader.GetString("NombreAsignado"),
                                    Curso = reader.IsDBNull("Curso") ? null : reader.GetString("Curso"),
                                    EstudiantesAsignados = new List<Usuario>()
                                };

                                ListaTareas.Add(Tarea);
                            }
                        }

                        // Obtener todos los estudiantes asignados para cada tarea
                        foreach (var tarea in ListaTareas)
                        {
                            var estudiantesRespuesta = ObtenerEstudiantesPorTarea(tarea.Id, Conexion);
                            if (estudiantesRespuesta.Ok && estudiantesRespuesta.ValorRetorno != null)
                            {
                                tarea.EstudiantesAsignados = estudiantesRespuesta.ValorRetorno;
                                
                                if (tarea.EstudiantesAsignados.Count > 1)
                                {
                                    tarea.NombreAsignado = string.Join(", ", tarea.EstudiantesAsignados.Select(e => e.NombreCompleto));
                                }
                                else if (tarea.EstudiantesAsignados.Count == 1)
                                {
                                    tarea.NombreAsignado = tarea.EstudiantesAsignados[0].NombreCompleto;
                                }
                            }
                        }

                        respuesta.Ok = true;
                        respuesta.Mensaje = $"Datos obtenidos de manera exitosa";
                        respuesta.ValorRetorno = ListaTareas;
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

        public Respuesta<List<Tarea>> ObtenerTareas(string Conexion)
        {
            Respuesta<List<Tarea>> respuesta = new Respuesta<List<Tarea>>();
            List<Tarea> ListaTareas = new List<Tarea>();
            try
            {
                using (SqlConnection connection = new SqlConnection(Conexion))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand("ObtenerTareas", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Tarea Tarea = new Tarea
                                {
                                    Id = (int)reader["Id"],
                                    Titulo = (string)reader["Titulo"],
                                    Descripcion = (string)reader["Descripcion"],
                                    ProyectoId = (int)reader["ProyectoId"],
                                    EstudianteAsignadoId = reader["EstudianteAsignadoId"] != DBNull.Value ? (int)reader["EstudianteAsignadoId"] : null,
                                    FechaInicio = reader["FechaInicio"] != DBNull.Value ? (DateTime?)reader["FechaInicio"] : null,
                                    FechaLimite = (DateTime)reader["FechaLimite"],
                                    Estado = (string)reader["Estado"],
                                    NombreAsignado = reader["NombreAsignado"] != DBNull.Value ? (string)reader["NombreAsignado"] : null,
                                    Curso = (string)reader["Curso"],
                                    EstudiantesAsignados = new List<Usuario>()
                                };

                                ListaTareas.Add(Tarea);
                            }
                        }

                        // Obtener todos los estudiantes asignados para cada tarea
                        foreach (var tarea in ListaTareas)
                        {
                            var estudiantesRespuesta = ObtenerEstudiantesPorTarea(tarea.Id, Conexion);
                            if (estudiantesRespuesta.Ok && estudiantesRespuesta.ValorRetorno != null)
                            {
                                tarea.EstudiantesAsignados = estudiantesRespuesta.ValorRetorno;
                                
                                // Actualizar NombreAsignado con todos los estudiantes si hay múltiples
                                if (tarea.EstudiantesAsignados.Count > 1)
                                {
                                    tarea.NombreAsignado = string.Join(", ", tarea.EstudiantesAsignados.Select(e => e.NombreCompleto));
                                }
                                else if (tarea.EstudiantesAsignados.Count == 1)
                                {
                                    tarea.NombreAsignado = tarea.EstudiantesAsignados[0].NombreCompleto;
                                }
                            }
                        }

                        respuesta.Ok = true;
                        respuesta.Mensaje = $"Datos obtenidos de manera exitosa";
                        respuesta.ValorRetorno = ListaTareas;
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

        public Respuesta<List<Usuario>> ObtenerEstudiantesPorTarea(int TareaId, string Conexion)
        {
            Respuesta<List<Usuario>> respuesta = new Respuesta<List<Usuario>>();
            List<Usuario> ListaEstudiantes = new List<Usuario>();
            try
            {
                using (SqlConnection connection = new SqlConnection(Conexion))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand("ObtenerEstudiantesPorTarea", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add(new SqlParameter("@TareaId", SqlDbType.Int) { Value = TareaId });

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Usuario estudiante = new Usuario
                                {
                                    Id = (int)reader["Id"],
                                    NombreCompleto = (string)reader["NombreCompleto"],
                                    Cedula = reader["Matricula"] != DBNull.Value ? (string)reader["Matricula"] : string.Empty,
                                    Email = reader["Email"] != DBNull.Value ? (string)reader["Email"] : string.Empty
                                };

                                ListaEstudiantes.Add(estudiante);
                            }

                            respuesta.Ok = true;
                            respuesta.Mensaje = "Estudiantes obtenidos exitosamente";
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



        public Respuesta<bool> ActualizarTarea(Tarea Tarea, string Conexion)
        {
            Respuesta<bool> respuesta = new Respuesta<bool>();

            try
            {
                using (SqlConnection connection = new SqlConnection(Conexion))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand("ActualizarTarea", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = Tarea.Id });
                        command.Parameters.Add(new SqlParameter("@Titulo", SqlDbType.NVarChar, 50) { Value = Tarea.Titulo });
                        command.Parameters.Add(new SqlParameter("@Descripcion", SqlDbType.NVarChar, 250) { Value = Tarea.Descripcion });
                        command.Parameters.Add(new SqlParameter("@FechaLimite", SqlDbType.Date) { Value = Tarea.FechaLimite });
                        command.Parameters.Add(new SqlParameter("@Estado", SqlDbType.NVarChar, 20) { Value = Tarea.Estado });
                        command.Parameters.Add(new SqlParameter("@EstudianteAsignadoId", SqlDbType.Int) { Value = Tarea.EstudianteAsignadoId ?? (object)DBNull.Value });

                        int FilasAfectadas = command.ExecuteNonQuery();

                        if (FilasAfectadas > 0 || FilasAfectadas == -1)
                        {
                            respuesta.Ok = true;
                            respuesta.Mensaje = "Tarea actualizada exitosamente";
                            respuesta.ValorRetorno = true;
                        }
                        else
                        {
                            respuesta.Ok = false;
                            respuesta.Mensaje = "No se pudo actualizar la tarea";
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

        public Respuesta<bool> EliminarTarea(int Id, string Conexion)
        {
            Respuesta<bool> respuesta = new Respuesta<bool>();

            try
            {
                using (SqlConnection connection = new SqlConnection(Conexion))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand("EliminarTarea", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = Id });

                        int FilasAfectadas = command.ExecuteNonQuery();

                        if (FilasAfectadas > 0 || FilasAfectadas == -1)
                        {
                            respuesta.Ok = true;
                            respuesta.Mensaje = "Tarea eliminada exitosamente";
                            respuesta.ValorRetorno = true;
                        }
                        else
                        {
                            respuesta.Ok = false;
                            respuesta.Mensaje = "No se pudo eliminar la tarea";
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

        public Respuesta<bool> ActualizarEstadoTarea(int Id, string Estado, string Conexion)
        {
            Respuesta<bool> respuesta = new Respuesta<bool>();

            try
            {
                using (SqlConnection connection = new SqlConnection(Conexion))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand("ActualizarEstadoTarea", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = Id });
                        command.Parameters.Add(new SqlParameter("@Estado", SqlDbType.NVarChar, 20) { Value = Estado });

                        int FilasAfectadas = command.ExecuteNonQuery();

                        if (FilasAfectadas > 0 || FilasAfectadas == -1)
                        {
                            respuesta.Ok = true;
                            respuesta.Mensaje = "Estado actualizado exitosamente";
                            respuesta.ValorRetorno = true;
                        }
                        else
                        {
                            respuesta.Ok = false;
                            respuesta.Mensaje = "No se pudo actualizar el estado";
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

        public Respuesta<bool> AsignarEstudiantesATarea(int TareaId, List<int> EstudianteIds, string Conexion)
        {
            Respuesta<bool> respuesta = new Respuesta<bool>();

            try
            {
                if (EstudianteIds == null || EstudianteIds.Count == 0)
                {
                    respuesta.Ok = false;
                    respuesta.Mensaje = "Debe seleccionar al menos un estudiante";
                    return respuesta;
                }

                using (SqlConnection connection = new SqlConnection(Conexion))
                {
                    connection.Open();

                    int asignacionesExitosas = 0;
                    int asignacionesDuplicadas = 0;

                    foreach (int estudianteId in EstudianteIds)
                    {
                        using (SqlCommand command = new SqlCommand("AsignarEstudianteATarea", connection))
                        {
                            command.CommandType = CommandType.StoredProcedure;
                            command.Parameters.Add(new SqlParameter("@TareaId", SqlDbType.Int) { Value = TareaId });
                            command.Parameters.Add(new SqlParameter("@EstudianteId", SqlDbType.Int) { Value = estudianteId });

                            try
                            {
                                command.ExecuteNonQuery();
                                asignacionesExitosas++;
                            }
                            catch (SqlException sqlEx)
                            {
                                // Si es un error de duplicado, lo ignoramos
                                if (sqlEx.Number == 2627 || sqlEx.Number == 2601) // Violación de clave única
                                {
                                    asignacionesDuplicadas++;
                                }
                                else
                                {
                                    throw;
                                }
                            }
                        }
                    }

                    respuesta.Ok = true;
                    if (asignacionesDuplicadas > 0)
                    {
                        respuesta.Mensaje = $"Se asignaron {asignacionesExitosas} estudiante(s). {asignacionesDuplicadas} ya estaban asignados.";
                    }
                    else
                    {
                        respuesta.Mensaje = $"Se asignaron {asignacionesExitosas} estudiante(s) exitosamente";
                    }
                    respuesta.ValorRetorno = true;
                }
            }
            catch (Exception ex)
            {
                respuesta.Ok = false;
                respuesta.Mensaje = ex.Message;
            }

            return respuesta;
        }

        public Respuesta<bool> EliminarAsignacionesEstudiantes(int TareaId, string Conexion)
        {
            Respuesta<bool> respuesta = new Respuesta<bool>();

            try
            {
                using (SqlConnection connection = new SqlConnection(Conexion))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand("EliminarAsignacionesEstudiantes", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add(new SqlParameter("@TareaId", SqlDbType.Int) { Value = TareaId });

                        command.ExecuteNonQuery();

                        respuesta.Ok = true;
                        respuesta.Mensaje = "Asignaciones eliminadas exitosamente";
                        respuesta.ValorRetorno = true;
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
