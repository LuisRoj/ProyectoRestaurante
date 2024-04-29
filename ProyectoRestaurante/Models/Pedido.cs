using System.ComponentModel.DataAnnotations;

namespace ProyectoRestaurante.Models
{
    public class Pedido
    {
        [Display(Name = " DNI Cliente")] public string dnicliente { get; set; }
        [Display(Name = " Nombre Cliente")] public string nombrecliente { get; set; }
        [Display(Name = " Email Cliente")] public string emailcliente { get; set; }
        [Display(Name = "Telefono")] public string fonocliente { get; set; }
    }
}
