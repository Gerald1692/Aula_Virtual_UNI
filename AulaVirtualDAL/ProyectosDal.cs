using Entities;
using Microsoft.Data.SqlClient;
using System.Data;

namespace AulaVirtualDAL
{
    public class ProyectosDal
    {
        private readonly string _connectionString;

        public ProyectosDal(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<IEnumerable<Proyecto>> ObtenerProyectosAsync()
        {
            var proyectos = new List<Proyecto>();

            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            await using var command = new SqlCommand(
                "SELECT Id, Nombre, Curso, FechaEntrega, Descripcion FROM Proyectos ORDER BY FechaEntrega DESC",
                connection);

            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                proyectos.Add(new Proyecto
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    Nombre = reader.GetString(reader.GetOrdinal("Nombre")),
                    Curso = reader.GetString(reader.GetOrdinal("Curso")),
                    FechaEntrega = reader.GetDateTime(reader.GetOrdinal("FechaEntrega")),
                    Descripcion = reader.GetString(reader.GetOrdinal("Descripcion"))
                });
            }

            return proyectos;
        }

        public async Task<Proyecto> CrearProyectoAsync(Proyecto proyecto)
        {
            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            await using var command = new SqlCommand(
                "INSERT INTO Proyectos (Nombre, Curso, FechaEntrega, Descripcion) OUTPUT INSERTED.Id VALUES (@Nombre, @Curso, @FechaEntrega, @Descripcion)",
                connection);

            command.Parameters.Add(new SqlParameter("@Nombre", SqlDbType.NVarChar, 200) { Value = proyecto.Nombre });
            command.Parameters.Add(new SqlParameter("@Curso", SqlDbType.NVarChar, 100) { Value = proyecto.Curso });
            command.Parameters.Add(new SqlParameter("@FechaEntrega", SqlDbType.Date) { Value = proyecto.FechaEntrega });
            command.Parameters.Add(new SqlParameter("@Descripcion", SqlDbType.NVarChar, -1) { Value = proyecto.Descripcion });

            var newId = (int)await command.ExecuteScalarAsync();
            proyecto.Id = newId;

            return proyecto;
        }

        public async Task<Proyecto?> ActualizarProyectoAsync(int id, Proyecto proyecto)
        {
            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            await using var command = new SqlCommand(
                "UPDATE Proyectos SET Nombre = @Nombre, Curso = @Curso, FechaEntrega = @FechaEntrega, Descripcion = @Descripcion WHERE Id = @Id",
                connection);

            command.Parameters.Add(new SqlParameter("@Nombre", SqlDbType.NVarChar, 200) { Value = proyecto.Nombre });
            command.Parameters.Add(new SqlParameter("@Curso", SqlDbType.NVarChar, 100) { Value = proyecto.Curso });
            command.Parameters.Add(new SqlParameter("@FechaEntrega", SqlDbType.Date) { Value = proyecto.FechaEntrega });
            command.Parameters.Add(new SqlParameter("@Descripcion", SqlDbType.NVarChar, -1) { Value = proyecto.Descripcion });
            command.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = id });

            var affected = await command.ExecuteNonQueryAsync();
            if (affected == 0)
            {
                return null;
            }

            proyecto.Id = id;
            return proyecto;
        }

        public async Task<bool> EliminarProyectoAsync(int id)
        {
            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            await using var command = new SqlCommand("DELETE FROM Proyectos WHERE Id = @Id", connection);
            command.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = id });

            var affected = await command.ExecuteNonQueryAsync();
            return affected > 0;
        }
    }
}
