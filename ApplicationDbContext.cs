using Ejercicio_Sesión_1.Entidades;
using Ejercicio_Sesión_1.Entidades.Seed;
using Microsoft.EntityFrameworkCore;

namespace Ejercicio_Sesión_1
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
            


        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Libro>().HasKey(x => x.Id);
            modelBuilder.Entity<Libro>().Property(x => x.Titulo).HasMaxLength(150);
            modelBuilder.Entity<Libro>().Property(x => x.Paginas).HasMaxLength(10000);

            modelBuilder.Entity<Editorial>().HasQueryFilter(x => !x.Eliminado); // 4.9.d
            modelBuilder.Entity<Editorial>().HasKey(x => x.Id);
            modelBuilder.Entity<Editorial>().Property(x => x.Nombre).HasMaxLength(50);

            modelBuilder.Entity<ConsultaKeyLess>().HasNoKey().
               ToSqlQuery("SELECT Editoriales.EditorialId,Editoriales.Nombre as NombreEditorial, LibroId, Libros.Nombre as NombreLibro, paginas FROM Editoriales INNER JOIN Libros ON Editoriales.EditorialId=Libros.EditorialID").ToView(null); // 4.10

            SeedData.Seed(modelBuilder);

        }

        public DbSet<Libro> Libros { get; set; }
        public DbSet<Editorial> Editoriales { get; set; }
        public DbSet<ConsultaKeyLess> ConsultaKeyLess { get; set; } // 4.10
        public DbSet<Usuario> Usuarios { get; set; }
    }
}
