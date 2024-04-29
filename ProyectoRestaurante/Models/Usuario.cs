using System.ComponentModel.DataAnnotations;

namespace ProyectoRestaurante.Models
{
    // Este modelo se utiliza tanto para el registro de nuevos usuarios como para el inicio de sesión.
    // En el contexto del inicio de sesión, solo se requieren el correo electrónico y la contraseña.
    public class Usuario
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Correo { get; set; }
        public string Telefono { get; set; }
        public string Direccion { get; set; }
      
        [Required(ErrorMessage = "La contraseña es requerida.")]
        [DataType(DataType.Password)]
        public string Contraseña { get; set; }

        [Required(ErrorMessage = "Por favor, confirme la contraseña.")]
        [DataType(DataType.Password)]
        [Compare("Contraseña", ErrorMessage = "Las contraseñas no coinciden.")]
        public string ConfirmarContraseña { get; set; }
    }
}
