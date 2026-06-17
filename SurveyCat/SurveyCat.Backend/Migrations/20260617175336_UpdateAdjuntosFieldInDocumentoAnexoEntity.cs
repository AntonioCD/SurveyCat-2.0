using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SurveyCat.Backend.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAdjuntosFieldInDocumentoAnexoEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Adjunto_DocumentosAnexos_DocumentoAnexoId",
                table: "Adjunto");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Adjunto",
                table: "Adjunto");

            migrationBuilder.RenameTable(
                name: "Adjunto",
                newName: "Adjuntos");

            migrationBuilder.RenameIndex(
                name: "IX_Adjunto_DocumentoAnexoId_ItemPagina",
                table: "Adjuntos",
                newName: "IX_Adjuntos_DocumentoAnexoId_ItemPagina");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Adjuntos",
                table: "Adjuntos",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Adjuntos_DocumentosAnexos_DocumentoAnexoId",
                table: "Adjuntos",
                column: "DocumentoAnexoId",
                principalTable: "DocumentosAnexos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Adjuntos_DocumentosAnexos_DocumentoAnexoId",
                table: "Adjuntos");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Adjuntos",
                table: "Adjuntos");

            migrationBuilder.RenameTable(
                name: "Adjuntos",
                newName: "Adjunto");

            migrationBuilder.RenameIndex(
                name: "IX_Adjuntos_DocumentoAnexoId_ItemPagina",
                table: "Adjunto",
                newName: "IX_Adjunto_DocumentoAnexoId_ItemPagina");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Adjunto",
                table: "Adjunto",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Adjunto_DocumentosAnexos_DocumentoAnexoId",
                table: "Adjunto",
                column: "DocumentoAnexoId",
                principalTable: "DocumentosAnexos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
