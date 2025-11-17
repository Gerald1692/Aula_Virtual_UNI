using System.Data;
using Aula_Virtual_UNI.Data;
using Microsoft.AspNetCore.Mvc;

namespace Aula_Virtual_UNI.Controllers
{
    public class DatabaseController : Controller
    {
        private readonly ISqlConnectionFactory _connectionFactory;

        public DatabaseController(ISqlConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        // GET: /Database/Test
        public IActionResult Test()
        {
            try
            {
                using IDbConnection conn = _connectionFactory.CreateConnection();
                conn.Open();
                var serverVersion = conn is Microsoft.Data.SqlClient.SqlConnection sc ? sc.ServerVersion : "unknown";
                return Content($"OK - Connected to SQL Server. Version: {serverVersion}");
            }
            catch (Exception ex)
            {
                Response.StatusCode = 500;
                return Content("ERROR - " + ex.Message);
            }
        }
    }
}

