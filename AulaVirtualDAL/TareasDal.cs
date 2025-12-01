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
                                    Curso = (string)reader["Curso"]

                                };

                                ListaTareas.Add(Tarea);



                            }

                            respuesta.Ok = true;
                            respuesta.Mensaje = $"Datos obtenidos de manera exitosa";
                            respuesta.ValorRetorno = ListaTareas;

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
    }
}
