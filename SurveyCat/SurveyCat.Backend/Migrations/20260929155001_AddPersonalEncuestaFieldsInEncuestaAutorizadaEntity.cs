using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SurveyCat.Backend.Migrations
{
    /// <inheritdoc />
    public partial class AddPersonalEncuestaFieldsInEncuestaAutorizadaEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CoordinadorId",
                table: "EncuestasAutorizadas",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EncuestadorId",
                table: "EncuestasAutorizadas",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TecnicoCatastralId",
                table: "EncuestasAutorizadas",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_EncuestasAutorizadas_CoordinadorId",
                table: "EncuestasAutorizadas",
                column: "CoordinadorId");

            migrationBuilder.CreateIndex(
                name: "IX_EncuestasAutorizadas_EncuestadorId",
                table: "EncuestasAutorizadas",
                column: "EncuestadorId");

            migrationBuilder.CreateIndex(
                name: "IX_EncuestasAutorizadas_TecnicoCatastralId",
                table: "EncuestasAutorizadas",
                column: "TecnicoCatastralId");

            migrationBuilder.AddForeignKey(
                name: "FK_EncuestasAutorizadas_PersonalEncuestas_CoordinadorId",
                table: "EncuestasAutorizadas",
                column: "CoordinadorId",
                principalTable: "PersonalEncuestas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EncuestasAutorizadas_PersonalEncuestas_EncuestadorId",
                table: "EncuestasAutorizadas",
                column: "EncuestadorId",
                principalTable: "PersonalEncuestas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EncuestasAutorizadas_PersonalEncuestas_TecnicoCatastralId",
                table: "EncuestasAutorizadas",
                column: "TecnicoCatastralId",
                principalTable: "PersonalEncuestas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EncuestasAutorizadas_PersonalEncuestas_CoordinadorId",
                table: "EncuestasAutorizadas");

            migrationBuilder.DropForeignKey(
                name: "FK_EncuestasAutorizadas_PersonalEncuestas_EncuestadorId",
                table: "EncuestasAutorizadas");

            migrationBuilder.DropForeignKey(
                name: "FK_EncuestasAutorizadas_PersonalEncuestas_TecnicoCatastralId",
                table: "EncuestasAutorizadas");

            migrationBuilder.DropIndex(
                name: "IX_EncuestasAutorizadas_CoordinadorId",
                table: "EncuestasAutorizadas");

            migrationBuilder.DropIndex(
                name: "IX_EncuestasAutorizadas_EncuestadorId",
                table: "EncuestasAutorizadas");

            migrationBuilder.DropIndex(
                name: "IX_EncuestasAutorizadas_TecnicoCatastralId",
                table: "EncuestasAutorizadas");

            migrationBuilder.DropColumn(
                name: "CoordinadorId",
                table: "EncuestasAutorizadas");

            migrationBuilder.DropColumn(
                name: "EncuestadorId",
                table: "EncuestasAutorizadas");

            migrationBuilder.DropColumn(
                name: "TecnicoCatastralId",
                table: "EncuestasAutorizadas");
        }
    }
}
