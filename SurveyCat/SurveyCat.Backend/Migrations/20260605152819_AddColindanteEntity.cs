using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SurveyCat.Backend.Migrations
{
    /// <inheritdoc />
    public partial class AddColindanteEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Colindantes",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FichaId = table.Column<long>(type: "bigint", nullable: false),
                    PuntoCardinalId = table.Column<int>(type: "int", nullable: false),
                    PersonaId = table.Column<long>(type: "bigint", nullable: false),
                    PresentaConflicto = table.Column<bool>(type: "bit", nullable: false),
                    ConflictoId = table.Column<int>(type: "int", nullable: true),
                    ViaGestionId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Colindantes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Colindantes_Diccionarios_ConflictoId",
                        column: x => x.ConflictoId,
                        principalTable: "Diccionarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Colindantes_Diccionarios_PuntoCardinalId",
                        column: x => x.PuntoCardinalId,
                        principalTable: "Diccionarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Colindantes_Diccionarios_ViaGestionId",
                        column: x => x.ViaGestionId,
                        principalTable: "Diccionarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Colindantes_Fichas_FichaId",
                        column: x => x.FichaId,
                        principalTable: "Fichas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Colindantes_Personas_PersonaId",
                        column: x => x.PersonaId,
                        principalTable: "Personas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Colindantes_ConflictoId",
                table: "Colindantes",
                column: "ConflictoId");

            migrationBuilder.CreateIndex(
                name: "IX_Colindantes_FichaId_PuntoCardinalId_PersonaId",
                table: "Colindantes",
                columns: new[] { "FichaId", "PuntoCardinalId", "PersonaId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Colindantes_PersonaId",
                table: "Colindantes",
                column: "PersonaId");

            migrationBuilder.CreateIndex(
                name: "IX_Colindantes_PuntoCardinalId",
                table: "Colindantes",
                column: "PuntoCardinalId");

            migrationBuilder.CreateIndex(
                name: "IX_Colindantes_ViaGestionId",
                table: "Colindantes",
                column: "ViaGestionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Colindantes");
        }
    }
}
