using Microsoft.EntityFrameworkCore;

namespace Ejercicio_Sesión_1.Entidades
{
    [Keyless] //4.10
    public class ConsultaKeyLess
    {
        public int EditorialId { get; set; }
        public string NombreEditorial { get; set; }
        public int LibroId { get; set; }
        public string NombreLibro { get; set; }
        public int Paginas { get; set; }
    }
}
