using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SurveyCat.Backend.Migrations
{
    /// <inheritdoc />
    public partial class AddDocumentoAnexoAndAdjuntoEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DocumentosAnexos",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FichaId = table.Column<long>(type: "bigint", nullable: false),
                    DocumentoId = table.Column<int>(type: "int", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    NumeroPaginas = table.Column<int>(type: "int", nullable: false),
                    Codigo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentosAnexos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DocumentosAnexos_Diccionarios_DocumentoId",
                        column: x => x.DocumentoId,
                        principalTable: "Diccionarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DocumentosAnexos_Fichas_FichaId",
                        column: x => x.FichaId,
                        principalTable: "Fichas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Adjunto",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DocumentoAnexoId = table.Column<long>(type: "bigint", nullable: false),
                    ItemPagina = table.Column<int>(type: "int", nullable: false),
                    NombreArchivo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Ruta = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Adjunto", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Adjunto_DocumentosAnexos_DocumentoAnexoId",
                        column: x => x.DocumentoAnexoId,
                        principalTable: "DocumentosAnexos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Adjunto_DocumentoAnexoId_ItemPagina",
                table: "Adjunto",
                columns: new[] { "DocumentoAnexoId", "ItemPagina" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DocumentosAnexos_DocumentoId",
                table: "DocumentosAnexos",
                column: "DocumentoId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentosAnexos_FichaId",
                table: "DocumentosAnexos",
                column: "FichaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Adjunto");

            migrationBuilder.DropTable(
                name: "DocumentosAnexos");
        }
    }
}
