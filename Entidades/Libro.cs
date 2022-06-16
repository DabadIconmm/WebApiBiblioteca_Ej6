using System.ComponentModel.DataAnnotations;

namespace Ejercicio_Sesión_1.Entidades
{
    public class Libro
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(50)]
        public string Titulo { get; set; } //añadir limite de caracteres 150max
        [Range(1, 10000)]
        public int Paginas { get; set; } //rango entre 1 - 10000
        public int EditorialId { get; set; }
        public Editorial Editorial { get; set; }
    }
}
