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

                    using (SqlCommand command = new SqlCommand("spInsertarTarea", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add(new SqlParameter("@pNombre", SqlDbType.NVarChar, 200) { Value = Tarea.Nombre });
                        command.Parameters.Add(new SqlParameter("@pDescripcion", SqlDbType.NVarChar, -1) { Value = Tarea.Descripcion });
                        command.Parameters.Add(new SqlParameter("@pFechaEntrega", SqlDbType.Date) { Value = Tarea.FechaEntrega });
                        command.Parameters.Add(new SqlParameter("@pIdProfesor", SqlDbType.Int) { Value = Tarea.id_profesor });
                        command.Parameters.Add(new SqlParameter("@pCurso", SqlDbType.NVarChar, 100) { Value = Tarea.Curso });
                        command.Parameters.Add(new SqlParameter("@pEstado", SqlDbType.NVarChar, 20) { Value = Tarea.Estado });


                        int FilasAfectadas = command.ExecuteNonQuery();


                        if (FilasAfectadas > 0)
                        {

                            respuesta.Ok = true;
                            respuesta.Mensaje = $"La tarea ha sido agregada de manera exitosa";


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

                    using (SqlCommand command = new SqlCommand("spObtenerTareas", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                       

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {

                                Tarea Tarea = new Tarea
                                {

                                    Id = (int)reader["Id"],
                                    Nombre = (string)reader["Nombre"],
                                    Descripcion = (string)reader["Descripcion"],
                                    FechaEntrega = (DateTime)reader["FechaEntrega"],
                                    id_profesor = (int)reader["id_profesor"],
                                    Curso = (string)reader["Curso"],
                                    Estado = (string)reader["Estado"]

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



    }
}
