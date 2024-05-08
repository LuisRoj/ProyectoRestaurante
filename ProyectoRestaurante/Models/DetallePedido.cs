namespace ProyectoRestaurante.Models
{
    public class DetallePedido
    {
        public int Id { get; set; }
        public string Npedido { get; set; }
        public DateTime FechaPedido { get; set; }
        public string NombrePlatillo { get; set; }
        public decimal Precio { get; set; }
        public int Cantidad { get; set; }
        public decimal Subtotal { get; set; }
    }


}
