using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SurveyCat.Backend.Migrations
{
    /// <inheritdoc />
    public partial class AddPropietarioEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Propietarios",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FichaId = table.Column<long>(type: "bigint", nullable: false),
                    PersonaId = table.Column<long>(type: "bigint", nullable: false),
                    Perfil = table.Column<int>(type: "int", nullable: true),
                    EspecificarPerfil = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Hijos = table.Column<int>(type: "int", nullable: true),
                    Hijas = table.Column<int>(type: "int", nullable: true),
                    TipoDerecho = table.Column<int>(type: "int", nullable: false),
                    PresentaDocumento = table.Column<bool>(type: "bit", nullable: false),
                    DocumentoId = table.Column<int>(type: "int", nullable: true),
                    AutorDocumento = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    FechaDocumento = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AreaTitulada = table.Column<double>(type: "float", nullable: true),
                    UnidadMedidaId = table.Column<int>(type: "int", nullable: true),
                    FechaAdquisicion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Finca = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Tomo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Folio = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Asiento = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Propietarios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Propietarios_Diccionarios_DocumentoId",
                        column: x => x.DocumentoId,
                        principalTable: "Diccionarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Propietarios_Diccionarios_UnidadMedidaId",
                        column: x => x.UnidadMedidaId,
                        principalTable: "Diccionarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Propietarios_Fichas_FichaId",
                        column: x => x.FichaId,
                        principalTable: "Fichas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Propietarios_Personas_PersonaId",
                        column: x => x.PersonaId,
                        principalTable: "Personas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Propietarios_DocumentoId",
                table: "Propietarios",
                column: "DocumentoId");

            migrationBuilder.CreateIndex(
                name: "IX_Propietarios_FichaId_PersonaId",
                table: "Propietarios",
                columns: new[] { "FichaId", "PersonaId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Propietarios_PersonaId",
                table: "Propietarios",
                column: "PersonaId");

            migrationBuilder.CreateIndex(
                name: "IX_Propietarios_UnidadMedidaId",
                table: "Propietarios",
                column: "UnidadMedidaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Propietarios");
        }
    }
}
