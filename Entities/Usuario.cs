namespace Entities
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Apellido1 { get; set; } = string.Empty;
        public string? Apellido2 { get; set; }
        public string Cedula { get; set; } = string.Empty;
        public string? Telefono { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Sede { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public int RolId { get; set; }

        // Propiedades adicionales para la vista
        public string? NombreCompleto { get; set; }
        public string? Rol { get; set; }
    }
}

