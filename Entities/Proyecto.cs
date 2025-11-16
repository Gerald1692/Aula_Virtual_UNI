namespace Entities
{
    public class Proyecto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Curso { get; set; } = string.Empty;
        public DateTime FechaEntrega { get; set; }
        public string Descripcion { get; set; } = string.Empty;
    }
}
