using System.ComponentModel.DataAnnotations;

namespace Ejercicio_Sesión_1.Entidades
{
    public class Editorial
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(50)]
        public string Nombre { get; set; } //texto no más de 50 caracteres
        public bool Eliminado { get; set; } // 4.9.a
        public ICollection<Libro> Libros { get; set; }
    }
}
