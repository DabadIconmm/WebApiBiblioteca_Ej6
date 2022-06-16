using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Ejercicio_Sesión_1.Migrations
{
    /// <inheritdoc />
    public partial class Inicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Editoriales",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Editoriales", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Libros",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Titulo = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Paginas = table.Column<int>(type: "int", maxLength: 10000, nullable: false),
                    EditorialId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Libros", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Libros_Editoriales_EditorialId",
                        column: x => x.EditorialId,
                        principalTable: "Editoriales",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Editoriales",
                columns: new[] { "Id", "Nombre" },
                values: new object[,]
                {
                    { 1, "Campbell" },
                    { 2, "Timunmas" }
                });

            migrationBuilder.InsertData(
                table: "Libros",
                columns: new[] { "Id", "EditorialId", "Paginas", "Titulo" },
                values: new object[,]
                {
                    { 1, 1, 300, "Warhammer" },
                    { 2, 2, 500, "Sherlock" },
                    { 3, 2, 340, "DragonLance" },
                    { 4, 1, 250, "Pesadillas" },
                    { 5, 1, 120, "Wally" },
                    { 6, 1, 200, "NameWind" },
                    { 7, 2, 340, "ManSapience" },
                    { 8, 1, 400, "LunaWolfes" },
                    { 9, 1, 450, "ImperialFists" },
                    { 10, 1, 333, "SpaceWolfes" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Libros_EditorialId",
                table: "Libros",
                column: "EditorialId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Libros");

            migrationBuilder.DropTable(
                name: "Editoriales");
        }
    }
}
