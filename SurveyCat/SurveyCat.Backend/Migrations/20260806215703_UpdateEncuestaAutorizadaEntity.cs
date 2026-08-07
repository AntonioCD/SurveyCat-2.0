using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SurveyCat.Backend.Migrations
{
    /// <inheritdoc />
    public partial class UpdateEncuestaAutorizadaEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EncuestasAutorizadas_AspNetUsers_UserId",
                table: "EncuestasAutorizadas");

            migrationBuilder.DropIndex(
                name: "IX_EncuestasAutorizadas_UserId",
                table: "EncuestasAutorizadas");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "EncuestasAutorizadas");

            migrationBuilder.CreateIndex(
                name: "IX_EncuestasAutorizadas_UsuarioCargaId",
                table: "EncuestasAutorizadas",
                column: "UsuarioCargaId");

            migrationBuilder.AddForeignKey(
                name: "FK_EncuestasAutorizadas_AspNetUsers_UsuarioCargaId",
                table: "EncuestasAutorizadas",
                column: "UsuarioCargaId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EncuestasAutorizadas_AspNetUsers_UsuarioCargaId",
                table: "EncuestasAutorizadas");

            migrationBuilder.DropIndex(
                name: "IX_EncuestasAutorizadas_UsuarioCargaId",
                table: "EncuestasAutorizadas");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "EncuestasAutorizadas",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_EncuestasAutorizadas_UserId",
                table: "EncuestasAutorizadas",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_EncuestasAutorizadas_AspNetUsers_UserId",
                table: "EncuestasAutorizadas",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
