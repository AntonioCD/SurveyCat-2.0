using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SurveyCat.Backend.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTipoConflictoIdFieldInConflictoEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Conflictos_ConflictoId_ViaGestionId_ConEstado",
                table: "Conflictos");

            migrationBuilder.DropIndex(
                name: "IX_Conflictos_TipoConflictoId",
                table: "Conflictos");

            migrationBuilder.DropColumn(
                name: "ConflictoId",
                table: "Conflictos");

            migrationBuilder.AlterColumn<int>(
                name: "TipoConflictoId",
                table: "Conflictos",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Conflictos_TipoConflictoId_ViaGestionId_ConEstado",
                table: "Conflictos",
                columns: new[] { "TipoConflictoId", "ViaGestionId", "ConEstado" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Conflictos_TipoConflictoId_ViaGestionId_ConEstado",
                table: "Conflictos");

            migrationBuilder.AlterColumn<int>(
                name: "TipoConflictoId",
                table: "Conflictos",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "ConflictoId",
                table: "Conflictos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Conflictos_ConflictoId_ViaGestionId_ConEstado",
                table: "Conflictos",
                columns: new[] { "ConflictoId", "ViaGestionId", "ConEstado" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Conflictos_TipoConflictoId",
                table: "Conflictos",
                column: "TipoConflictoId");
        }
    }
}
