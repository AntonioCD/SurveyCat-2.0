using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SurveyCat.Backend.Migrations
{
    /// <inheritdoc />
    public partial class AddConflictoEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Conflictos",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FichaId = table.Column<long>(type: "bigint", nullable: false),
                    ConflictoId = table.Column<int>(type: "int", nullable: false),
                    TipoConflictoId = table.Column<int>(type: "int", nullable: true),
                    ViaGestionId = table.Column<int>(type: "int", nullable: false),
                    ConEstado = table.Column<bool>(type: "bit", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Conflictos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Conflictos_Diccionarios_TipoConflictoId",
                        column: x => x.TipoConflictoId,
                        principalTable: "Diccionarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Conflictos_Diccionarios_ViaGestionId",
                        column: x => x.ViaGestionId,
                        principalTable: "Diccionarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Conflictos_Fichas_FichaId",
                        column: x => x.FichaId,
                        principalTable: "Fichas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Conflictos_ConflictoId_ViaGestionId_ConEstado",
                table: "Conflictos",
                columns: new[] { "ConflictoId", "ViaGestionId", "ConEstado" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Conflictos_FichaId",
                table: "Conflictos",
                column: "FichaId");

            migrationBuilder.CreateIndex(
                name: "IX_Conflictos_TipoConflictoId",
                table: "Conflictos",
                column: "TipoConflictoId");

            migrationBuilder.CreateIndex(
                name: "IX_Conflictos_ViaGestionId",
                table: "Conflictos",
                column: "ViaGestionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Conflictos");
        }
    }
}
