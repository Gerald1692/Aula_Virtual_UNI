using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Aula_Virtual_UNI.Data
{
    public interface ISqlConnectionFactory
    {
        IDbConnection CreateConnection();
    }

    public class SqlConnectionFactory : ISqlConnectionFactory
    {
        private readonly string _connectionString;

        public SqlConnectionFactory(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("ConexionDB")
                ?? throw new InvalidOperationException("Connection string 'ConexionDB' not found.");
        }

        public IDbConnection CreateConnection()
        {
            return new SqlConnection(_connectionString);
        }
    }
}

