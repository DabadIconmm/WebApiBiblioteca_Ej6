namespace Ejercicio_Sesión_1.DTOs
{
    public class DTOEditorialLibro
    {
        public int IdEditorial { get; set; } //Id de la editorial
        public string Nombre { get; set; }
        public List<DTOLibroItem> Libros { get; set; }
    }

    public class DTOLibroItem
    {
        public int IdLibro { get; set; }
        public string Nombre { get; set; }
        public int Paginas { get; set; }
    }
}
