using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoRestaurante.Models
{
    public class Platillo
    {
        public int id { get; set; }
        public string nombre { get; set; }
        public string descripcion { get; set; }
        [Column(TypeName = "decimal(10, 2)")]
        public decimal precio { get; set; }
        public int stock { get; set; }
        public string? imagen { get; set; }
    }
}
