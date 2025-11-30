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

                        command.Parameters.Add(new SqlParameter("@pNombre", SqlDbType.NVarChar, 50) { Value = Proyecto.nombre });
                        command.Parameters.Add(new SqlParameter("@pDescripcion", SqlDbType.NVarChar, 250) { Value = Proyecto.descripcion });
                        command.Parameters.Add(new SqlParameter("@pFechainicio", SqlDbType.Date) { Value = Proyecto.fecha_inicio });
                        command.Parameters.Add(new SqlParameter("@pFechaFinalizacion", SqlDbType.Date) { Value = Proyecto.fecha });
                        command.Parameters.Add(new SqlParameter("@pIdProfesor", SqlDbType.Int) { Value = Proyecto.id_profesor });
                        command.Parameters.Add(new SqlParameter("@pCurso", SqlDbType.NVarChar, 50) { Value = Proyecto.curso });
                        command.Parameters.Add(new SqlParameter("@pEstado", SqlDbType.NVarChar, 50) { Value = Proyecto.estado });


                        int FilasAfectadas = command.ExecuteNonQuery();


                        if (FilasAfectadas > 0)
                        {

                            respuesta.Ok = true;
                            respuesta.Mensaje = $"El proyecto ha sido agregado de manera exitosa";


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
                                
                                    id_proyecto = (int)reader["id_proyecto"],
                                   nombre = (string)reader["nombre"],
                                    descripcion = (string)reader["descripcion"],
                                    fecha_inicio = (DateTime)reader["fecha_inicio"],
                                    fecha = (DateTime)reader["fecha_finalizacion"],
                                    id_profesor = (int)reader["id_profesor"],
                                    curso = (string)reader["curso"],
                                    estado= (string)reader["estado"]
                                  
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

                    using (SqlCommand command = new SqlCommand("spEliminarProyecto", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add(new SqlParameter("@pIdProyecto", SqlDbType.Int) { Value = Id });

                        int FilasAfectadas = command.ExecuteNonQuery();

                        if (FilasAfectadas > 0 || FilasAfectadas == -1)
                        {
                            respuesta.Ok = true;
                            respuesta.Mensaje = "Proyecto eliminado exitosamente";
                            respuesta.ValorRetorno = true;
                        }
                        else
                        {
                            respuesta.Ok = false;
                            respuesta.Mensaje = "No se pudo eliminar el proyecto";
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
