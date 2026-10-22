using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SurveyCat.Backend.Migrations
{
    /// <inheritdoc />
    public partial class AddJornadaEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "JornadaId",
                table: "Fichas",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Jornadas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CodJornada = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    FechaInicio = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FechaFin = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Jornadas", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Fichas_JornadaId",
                table: "Fichas",
                column: "JornadaId");

            migrationBuilder.CreateIndex(
                name: "IX_Jornadas_CodJornada",
                table: "Jornadas",
                column: "CodJornada",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Fichas_Jornadas_JornadaId",
                table: "Fichas",
                column: "JornadaId",
                principalTable: "Jornadas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Fichas_Jornadas_JornadaId",
                table: "Fichas");

            migrationBuilder.DropTable(
                name: "Jornadas");

            migrationBuilder.DropIndex(
                name: "IX_Fichas_JornadaId",
                table: "Fichas");

            migrationBuilder.DropColumn(
                name: "JornadaId",
                table: "Fichas");
        }
    }
}
