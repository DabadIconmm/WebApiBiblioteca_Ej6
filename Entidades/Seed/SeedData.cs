using Microsoft.EntityFrameworkCore;

namespace Ejercicio_Sesión_1.Entidades.Seed
{
    public class SeedData
    {
        public static void Seed(ModelBuilder modelBuilder)
        {
            //Editorial
            var Campbell = new Editorial { Id = 1, Nombre = "Campbell" };
            var Timunmas = new Editorial { Id = 2, Nombre = "Timunmas" };

            modelBuilder.Entity<Editorial>().HasData(Campbell, Timunmas);
            //Libros
            var Warhammer = new Libro { Id = 1,EditorialId = 1, Paginas = 300, Titulo = "Warhammer" };
            var Sherlock = new Libro { Id = 2, EditorialId = 2, Paginas = 500, Titulo = "Sherlock" };
            var DragonLance = new Libro { Id = 3, EditorialId = 2, Paginas = 340, Titulo = "DragonLance" };
            var Pesadillas = new Libro { Id = 4, EditorialId = 1, Paginas = 250, Titulo = "Pesadillas" };
            var Wally = new Libro { Id = 5, EditorialId = 1, Paginas = 120, Titulo = "Wally" };
            var NameWind = new Libro { Id = 6, EditorialId = 1, Paginas = 200, Titulo = "NameWind" };
            var ManSapience = new Libro { Id = 7, EditorialId = 2, Paginas = 340, Titulo = "ManSapience" };
            var LunaWolfes = new Libro { Id = 8, EditorialId = 1, Paginas = 400, Titulo = "LunaWolfes" };
            var ImperialFists = new Libro { Id = 9, EditorialId = 1, Paginas = 450, Titulo = "ImperialFists" };
            var SpaceWolfes = new Libro { Id = 10, EditorialId = 1, Paginas = 333, Titulo = "SpaceWolfes" };

            modelBuilder.Entity<Libro>().HasData(Warhammer, Sherlock, DragonLance, Pesadillas, Wally, NameWind, ManSapience, LunaWolfes, ImperialFists, SpaceWolfes);

        }
    }
}
