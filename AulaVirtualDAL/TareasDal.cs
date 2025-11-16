using Entities;
using Microsoft.Data.SqlClient;
using System.Data;

namespace AulaVirtualDAL
{
    public class TareasDal
    {
        private readonly string _connectionString;

        public TareasDal(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<IEnumerable<Tarea>> ObtenerTareasAsync()
        {
            var tareas = new List<Tarea>();

            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            await using var command = new SqlCommand(
                "SELECT Id, Nombre, Curso, FechaEntrega, Descripcion, Estado FROM Tareas ORDER BY FechaEntrega DESC",
                connection);

            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                tareas.Add(new Tarea
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    Nombre = reader.GetString(reader.GetOrdinal("Nombre")),
                    Curso = reader.GetString(reader.GetOrdinal("Curso")),
                    FechaEntrega = reader.GetDateTime(reader.GetOrdinal("FechaEntrega")),
                    Descripcion = reader.GetString(reader.GetOrdinal("Descripcion")),
                    Estado = reader.GetString(reader.GetOrdinal("Estado"))
                });
            }

            return tareas;
        }

        public async Task<Tarea> CrearTareaAsync(Tarea tarea)
        {
            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            await using var command = new SqlCommand(
                "INSERT INTO Tareas (Nombre, Curso, FechaEntrega, Descripcion, Estado) OUTPUT INSERTED.Id VALUES (@Nombre, @Curso, @FechaEntrega, @Descripcion, @Estado)",
                connection);

            command.Parameters.Add(new SqlParameter("@Nombre", SqlDbType.NVarChar, 200) { Value = tarea.Nombre });
            command.Parameters.Add(new SqlParameter("@Curso", SqlDbType.NVarChar, 100) { Value = tarea.Curso });
            command.Parameters.Add(new SqlParameter("@FechaEntrega", SqlDbType.Date) { Value = tarea.FechaEntrega });
            command.Parameters.Add(new SqlParameter("@Descripcion", SqlDbType.NVarChar, -1) { Value = tarea.Descripcion });
            command.Parameters.Add(new SqlParameter("@Estado", SqlDbType.NVarChar, 20) { Value = tarea.Estado });

            var newId = (int)await command.ExecuteScalarAsync();
            tarea.Id = newId;

            return tarea;
        }
    }
}
