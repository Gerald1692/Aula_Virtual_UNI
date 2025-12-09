using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class Proyectos
    {

        public int id_proyecto { get; set; }
        public string? nombre { get; set; }
        public string? descripcion { get; set; }
        public DateTime fecha_inicio { get; set; }
        public DateTime fecha_finalizacion { get; set; }
        public int id_profesor { get; set; }
        public string? curso { get; set; }
        public string? estado { get; set; }
        public string? id_asignado { get; set; }
        public string? NombreAsignado { get; set; }
        public string? NombreProfesor { get; set; }
        public List<Usuario>? Integrantes { get; set; }

        


    }
}
