namespace Entities
{
    public class Tarea
    {
        public int Id { get; set; }
        public int ProyectoId { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public int? EstudianteAsignadoId { get; set; }
        public int? IdCreador { get; set; }
        public DateTime? FechaInicio { get; set; }
        public DateTime FechaLimite { get; set; }
        public string Estado { get; set; } = "pendiente";
        
        // Propiedades adicionales para la vista
        public string? NombreAsignado { get; set; }
        public string? Curso { get; set; }
        public string? NombreCreador { get; set; }
        
        // Lista de estudiantes asignados (múltiples)
        public List<Usuario>? EstudiantesAsignados { get; set; }
        
        // Lista de IDs de estudiantes asignados (para recibir desde el frontend)
        public List<int>? EstudiantesAsignadosIds { get; set; }
    }
}
