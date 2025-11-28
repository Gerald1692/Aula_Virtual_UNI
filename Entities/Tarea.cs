namespace Entities
{
    public class Tarea
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Curso { get; set; } = string.Empty;
        public DateTime FechaEntrega { get; set; }
        public string Descripcion { get; set; } = string.Empty;
        public string Estado { get; set; } = "pendiente";
        public int id_profesor { get; set; }
    }
}
