using System.ComponentModel.DataAnnotations;
namespace ProyectoRestaurante.Models

{
    public class Comentario
    {
        public int? id { set; get; }
        public int? id_ciudad { get; set; }
        public string? comentario { get; set; }

    }
}
