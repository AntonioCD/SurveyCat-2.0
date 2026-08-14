using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SurveyCat.Backend.Migrations
{
    /// <inheritdoc />
    public partial class UpdateOcupanteEntityWithDiccionario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Familias_Diccionarios_ParentescoId",
                table: "Familias");

            migrationBuilder.DropForeignKey(
                name: "FK_Familias_Fichas_FichaId",
                table: "Familias");

            migrationBuilder.DropForeignKey(
                name: "FK_Familias_Personas_PersonaId",
                table: "Familias");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Familias",
                table: "Familias");

            migrationBuilder.RenameTable(
                name: "Familias",
                newName: "Ocupantes");

            migrationBuilder.RenameIndex(
                name: "IX_Familias_PersonaId",
                table: "Ocupantes",
                newName: "IX_Ocupantes_PersonaId");

            migrationBuilder.RenameIndex(
                name: "IX_Familias_ParentescoId",
                table: "Ocupantes",
                newName: "IX_Ocupantes_ParentescoId");

            migrationBuilder.RenameIndex(
                name: "IX_Familias_FichaId_PersonaId",
                table: "Ocupantes",
                newName: "IX_Ocupantes_FichaId_PersonaId");

            migrationBuilder.AlterColumn<int>(
                name: "ParentescoId",
                table: "Ocupantes",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "TipoOcupanteId",
                table: "Ocupantes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Ocupantes",
                table: "Ocupantes",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Ocupantes_TipoOcupanteId",
                table: "Ocupantes",
                column: "TipoOcupanteId");

            migrationBuilder.AddForeignKey(
                name: "FK_Ocupantes_Diccionarios_ParentescoId",
                table: "Ocupantes",
                column: "ParentescoId",
                principalTable: "Diccionarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Ocupantes_Diccionarios_TipoOcupanteId",
                table: "Ocupantes",
                column: "TipoOcupanteId",
                principalTable: "Diccionarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Ocupantes_Fichas_FichaId",
                table: "Ocupantes",
                column: "FichaId",
                principalTable: "Fichas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Ocupantes_Personas_PersonaId",
                table: "Ocupantes",
                column: "PersonaId",
                principalTable: "Personas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ocupantes_Diccionarios_ParentescoId",
                table: "Ocupantes");

            migrationBuilder.DropForeignKey(
                name: "FK_Ocupantes_Diccionarios_TipoOcupanteId",
                table: "Ocupantes");

            migrationBuilder.DropForeignKey(
                name: "FK_Ocupantes_Fichas_FichaId",
                table: "Ocupantes");

            migrationBuilder.DropForeignKey(
                name: "FK_Ocupantes_Personas_PersonaId",
                table: "Ocupantes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Ocupantes",
                table: "Ocupantes");

            migrationBuilder.DropIndex(
                name: "IX_Ocupantes_TipoOcupanteId",
                table: "Ocupantes");

            migrationBuilder.DropColumn(
                name: "TipoOcupanteId",
                table: "Ocupantes");

            migrationBuilder.RenameTable(
                name: "Ocupantes",
                newName: "Familias");

            migrationBuilder.RenameIndex(
                name: "IX_Ocupantes_PersonaId",
                table: "Familias",
                newName: "IX_Familias_PersonaId");

            migrationBuilder.RenameIndex(
                name: "IX_Ocupantes_ParentescoId",
                table: "Familias",
                newName: "IX_Familias_ParentescoId");

            migrationBuilder.RenameIndex(
                name: "IX_Ocupantes_FichaId_PersonaId",
                table: "Familias",
                newName: "IX_Familias_FichaId_PersonaId");

            migrationBuilder.AlterColumn<int>(
                name: "ParentescoId",
                table: "Familias",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Familias",
                table: "Familias",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Familias_Diccionarios_ParentescoId",
                table: "Familias",
                column: "ParentescoId",
                principalTable: "Diccionarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Familias_Fichas_FichaId",
                table: "Familias",
                column: "FichaId",
                principalTable: "Fichas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Familias_Personas_PersonaId",
                table: "Familias",
                column: "PersonaId",
                principalTable: "Personas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
