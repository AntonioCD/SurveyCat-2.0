using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SurveyCat.Backend.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUniqueIndexInConflicto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Conflictos_FichaId",
                table: "Conflictos");

            migrationBuilder.DropIndex(
                name: "IX_Conflictos_TipoConflictoId_ViaGestionId_ConEstado",
                table: "Conflictos");

            migrationBuilder.CreateIndex(
                name: "IX_Conflictos_FichaId_TipoConflictoId_ViaGestionId_ConEstado",
                table: "Conflictos",
                columns: new[] { "FichaId", "TipoConflictoId", "ViaGestionId", "ConEstado" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Conflictos_TipoConflictoId",
                table: "Conflictos",
                column: "TipoConflictoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Conflictos_FichaId_TipoConflictoId_ViaGestionId_ConEstado",
                table: "Conflictos");

            migrationBuilder.DropIndex(
                name: "IX_Conflictos_TipoConflictoId",
                table: "Conflictos");

            migrationBuilder.CreateIndex(
                name: "IX_Conflictos_FichaId",
                table: "Conflictos",
                column: "FichaId");

            migrationBuilder.CreateIndex(
                name: "IX_Conflictos_TipoConflictoId_ViaGestionId_ConEstado",
                table: "Conflictos",
                columns: new[] { "TipoConflictoId", "ViaGestionId", "ConEstado" },
                unique: true);
        }
    }
}
