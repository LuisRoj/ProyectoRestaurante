namespace ProyectoRestaurante.Models
{
    public class Registro
    {
        public int id { set; get; }
        public string nombre { get; set; }
        public decimal precio { get; set; }
        public int cantidad { get; set; }
        public string Imagen { get; set; }
        public decimal monto { get { return precio * cantidad; } }
    }
}
