using Entities;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AulaVirtualDAL
{
    public class ProyectosDAL
    {

        public Respuesta<Proyectos> InsertarProyecto(Proyectos Proyecto, string Conexion)
        {
            Respuesta<Proyectos> respuesta = new Respuesta<Proyectos>();

            try
            {
                using (SqlConnection connection = new SqlConnection(Conexion))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand("spInsertarProyecto", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add(new SqlParameter("@pNombre", SqlDbType.NVarChar, 50) { Value = (object)Proyecto.nombre ?? DBNull.Value });
                        command.Parameters.Add(new SqlParameter("@pDescripcion", SqlDbType.NVarChar, 250) { Value = (object)Proyecto.descripcion ?? DBNull.Value });
                        command.Parameters.Add(new SqlParameter("@pFechainicio", SqlDbType.Date) { Value = (object)Proyecto.fecha_inicio ?? DBNull.Value });
                        command.Parameters.Add(new SqlParameter("@pFechaFinalizacion", SqlDbType.Date) { Value = (object)Proyecto.fecha_finalizacion ?? DBNull.Value });
                        command.Parameters.Add(new SqlParameter("@pIdProfesor", SqlDbType.Int) { Value = (object)Proyecto.id_profesor ?? DBNull.Value });
                        command.Parameters.Add(new SqlParameter("@pCurso", SqlDbType.NVarChar, 50) { Value = (object)Proyecto.curso ?? DBNull.Value });
                        command.Parameters.Add(new SqlParameter("@pEstado", SqlDbType.NVarChar, 50) { Value = (object)Proyecto.estado ?? DBNull.Value });
                        command.Parameters.Add(new SqlParameter("@EstudianteAsignadoId", SqlDbType.Int) { Value = (object)Proyecto.id_asignado ?? DBNull.Value });

                        object result = command.ExecuteScalar();

                        if (result != null && result != DBNull.Value)
                        {
                            respuesta.Ok = true;
                            respuesta.Mensaje = $"El proyecto ha sido agregado de manera exitosa";
                            Proyecto.id_proyecto = Convert.ToInt32(result);
                            respuesta.ValorRetorno = Proyecto;
                        }
                        else
                        {
                            respuesta.Ok = false;
                            respuesta.Mensaje = "Ha ocurrido un error a la hora de agregar el proyecto";
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

        public Respuesta<List<Proyectos>> ObtenerProyectos(string Conexion)
        {
            Respuesta<List<Proyectos>> respuesta = new Respuesta<List<Proyectos>>();
            List<Proyectos> ListaProyectos = new List<Proyectos>();
            try
            {
                using (SqlConnection connection = new SqlConnection(Conexion))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand("spObtenerProyectos", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                       

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {

                                Proyectos Proyecto = new Proyectos { 
                                
                                    id_proyecto = reader.IsDBNull("id_proyecto") ? 0 : reader.GetInt32("id_proyecto"),
                                    nombre = reader.IsDBNull("nombre") ? null : reader.GetString("nombre"),
                                    descripcion = reader.IsDBNull("descripcion") ? null : reader.GetString("descripcion"),
                                    fecha_inicio = reader.IsDBNull("fecha_inicio") ? DateTime.MinValue : reader.GetDateTime("fecha_inicio"),
                                    fecha_finalizacion = reader.IsDBNull("fecha_finalizacion") ? DateTime.MinValue : reader.GetDateTime("fecha_finalizacion"),
                                    id_profesor = reader.IsDBNull("id_profesor") ? 0 : reader.GetInt32("id_profesor"),
                                    curso = reader.IsDBNull("curso") ? null : reader.GetString("curso"),
                                    estado = reader.IsDBNull("estado") ? null : reader.GetString("estado")
                                  
                                };

                                ListaProyectos.Add(Proyecto);



                            }

                            respuesta.Ok = true;
                            respuesta.Mensaje = $"Datos obtenidos de manera exitosa";
                            respuesta.ValorRetorno = ListaProyectos;

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



        public Respuesta<bool> EliminarProyecto(int Id, string Conexion)
        {
            Respuesta<bool> respuesta = new Respuesta<bool>();

            try
            {
                using (SqlConnection connection = new SqlConnection(Conexion))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand("EliminarProyecto", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = Id });

                        // El procedimiento devuelve un SELECT, necesitamos leerlo
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // Leer Exito y Mensaje del resultado
                                int exito = reader.GetInt32(0);
                                string mensaje = reader.GetString(1);

                                respuesta.Ok = exito == 1;
                                respuesta.Mensaje = mensaje;
                                respuesta.ValorRetorno = exito == 1;
                            }
                            else
                            {
                                respuesta.Ok = false;
                                respuesta.Mensaje = "No se recibió respuesta del procedimiento almacenado";
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

        public Respuesta<List<Proyectos>> ObtenerProyectosPorEstudiante(int EstudianteId, string Conexion)
        {
            Respuesta<List<Proyectos>> respuesta = new Respuesta<List<Proyectos>>();
            List<Proyectos> ListaProyectos = new List<Proyectos>();
            try
            {
                using (SqlConnection connection = new SqlConnection(Conexion))
                {
                    connection.Open();

                    string query = @"
                        SELECT DISTINCT
                            p.id_proyecto,
                            p.nombre,
                            p.descripcion,
                            p.fecha_inicio,
                            p.fecha_finalizacion,
                            p.id_profesor,
                            p.curso,
                            p.estado
                        FROM dbo.proyectos p
                        INNER JOIN dbo.proyecto_estudiante pe ON p.id_proyecto = pe.id_proyecto
                        WHERE pe.id_estudiante = @EstudianteId
                        ORDER BY p.fecha_finalizacion DESC";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.Add(new SqlParameter("@EstudianteId", SqlDbType.Int) { Value = EstudianteId });

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Proyectos Proyecto = new Proyectos
                                {
                                    id_proyecto = reader.IsDBNull("id_proyecto") ? 0 : reader.GetInt32("id_proyecto"),
                                    nombre = reader.IsDBNull("nombre") ? null : reader.GetString("nombre"),
                                    descripcion = reader.IsDBNull("descripcion") ? null : reader.GetString("descripcion"),
                                    fecha_inicio = reader.IsDBNull("fecha_inicio") ? DateTime.MinValue : reader.GetDateTime("fecha_inicio"),
                                    fecha_finalizacion = reader.IsDBNull("fecha_finalizacion") ? DateTime.MinValue : reader.GetDateTime("fecha_finalizacion"),
                                    id_profesor = reader.IsDBNull("id_profesor") ? 0 : reader.GetInt32("id_profesor"),
                                    curso = reader.IsDBNull("curso") ? null : reader.GetString("curso"),
                                    estado = reader.IsDBNull("estado") ? null : reader.GetString("estado")
                                };

                                ListaProyectos.Add(Proyecto);
                            }

                            respuesta.Ok = true;
                            respuesta.Mensaje = $"Datos obtenidos de manera exitosa";
                            respuesta.ValorRetorno = ListaProyectos;
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

        public Respuesta<bool> AsignarEstudianteProyecto(int ProyectoId, int EstudianteId, string Conexion)
        {
            Respuesta<bool> respuesta = new Respuesta<bool>();

            try
            {
                using (SqlConnection connection = new SqlConnection(Conexion))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand("AsignarEstudianteProyecto", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add(new SqlParameter("@ProyectoId", SqlDbType.Int) { Value = ProyectoId });
                        command.Parameters.Add(new SqlParameter("@EstudianteId", SqlDbType.Int) { Value = EstudianteId });

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.HasRows && reader.Read())
                            {
                                int exito = 0;
                                string mensaje = "Error desconocido al asignar el estudiante.";

                                try
                                {
                                    if (reader.GetOrdinal("Exito") >= 0)
                                        exito = reader.GetInt32(reader.GetOrdinal("Exito"));
                                    if (reader.GetOrdinal("Mensaje") >= 0)
                                        mensaje = reader.GetString(reader.GetOrdinal("Mensaje"));
                                }
                                catch (IndexOutOfRangeException)
                                {
                                    if (reader.FieldCount >= 2)
                                    {
                                        exito = reader.GetInt32(0);
                                        mensaje = reader.GetString(1);
                                    }
                                    else
                                    {
                                        respuesta.Ok = false;
                                        respuesta.Mensaje = "El procedimiento almacenado no devolvió las columnas esperadas (Exito, Mensaje).";
                                        return respuesta;
                                    }
                                }

                                respuesta.Ok = exito == 1;
                                respuesta.Mensaje = mensaje;
                                respuesta.ValorRetorno = exito == 1;
                            }
                            else
                            {
                                respuesta.Ok = false;
                                respuesta.Mensaje = "No se recibió respuesta del procedimiento almacenado 'AsignarEstudianteProyecto'.";
                            }
                        }
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                respuesta.Ok = false;
                respuesta.Mensaje = $"Error de base de datos al asignar el estudiante: {sqlEx.Message}";
            }
            catch (Exception ex)
            {
                respuesta.Ok = false;
                respuesta.Mensaje = $"Error inesperado al asignar el estudiante: {ex.Message}";
            }

            return respuesta;
        }
    }
}
