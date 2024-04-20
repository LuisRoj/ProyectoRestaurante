using System.ComponentModel.DataAnnotations;

namespace ProyectoRestaurante.Models
{
    public class Platillo
    {
        public int id { get; set; }
        public string nombre { get; set; }
        public string descripcion { get; set; }
        public decimal precio { get; set; }
        public int stock { get; set; }
        public string imagen { get; set; }
    }
}
