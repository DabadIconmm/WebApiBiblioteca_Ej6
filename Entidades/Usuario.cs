using System.ComponentModel.DataAnnotations;

namespace Ejercicio_Sesión_1.Entidades
{
    public class Usuario
    {
        public int Id { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        public byte[] Salt { get; set; }
    }
}

