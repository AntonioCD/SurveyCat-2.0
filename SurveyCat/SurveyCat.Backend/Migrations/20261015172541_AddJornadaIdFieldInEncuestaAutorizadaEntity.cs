using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SurveyCat.Backend.Migrations
{
    /// <inheritdoc />
    public partial class AddJornadaIdFieldInEncuestaAutorizadaEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "JornadaId",
                table: "EncuestasAutorizadas",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_EncuestasAutorizadas_JornadaId",
                table: "EncuestasAutorizadas",
                column: "JornadaId");

            migrationBuilder.AddForeignKey(
                name: "FK_EncuestasAutorizadas_Jornadas_JornadaId",
                table: "EncuestasAutorizadas",
                column: "JornadaId",
                principalTable: "Jornadas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EncuestasAutorizadas_Jornadas_JornadaId",
                table: "EncuestasAutorizadas");

            migrationBuilder.DropIndex(
                name: "IX_EncuestasAutorizadas_JornadaId",
                table: "EncuestasAutorizadas");

            migrationBuilder.DropColumn(
                name: "JornadaId",
                table: "EncuestasAutorizadas");
        }
    }
}
