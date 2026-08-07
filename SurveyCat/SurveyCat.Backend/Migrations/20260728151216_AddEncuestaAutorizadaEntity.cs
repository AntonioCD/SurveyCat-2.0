using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SurveyCat.Backend.Migrations
{
    /// <inheritdoc />
    public partial class AddEncuestaAutorizadaEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "CodParcela",
                table: "Fichas",
                type: "nvarchar(25)",
                maxLength: 25,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CodEncuesta",
                table: "Fichas",
                type: "nvarchar(25)",
                maxLength: 25,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.CreateTable(
                name: "EncuestasAutorizadas",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CodEncuesta = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
                    TipoSector = table.Column<int>(type: "int", nullable: false),
                    MunicipioId = table.Column<int>(type: "int", nullable: false),
                    BarrioComarcaId = table.Column<int>(type: "int", nullable: true),
                    CaserioId = table.Column<int>(type: "int", nullable: true),
                    FechaCarga = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UsuarioCargaId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Observacion = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EncuestasAutorizadas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EncuestasAutorizadas_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EncuestasAutorizadas_BarriosComarcas_BarrioComarcaId",
                        column: x => x.BarrioComarcaId,
                        principalTable: "BarriosComarcas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EncuestasAutorizadas_Caserios_CaserioId",
                        column: x => x.CaserioId,
                        principalTable: "Caserios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EncuestasAutorizadas_Municipios_MunicipioId",
                        column: x => x.MunicipioId,
                        principalTable: "Municipios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EncuestasAutorizadas_BarrioComarcaId",
                table: "EncuestasAutorizadas",
                column: "BarrioComarcaId");

            migrationBuilder.CreateIndex(
                name: "IX_EncuestasAutorizadas_CaserioId",
                table: "EncuestasAutorizadas",
                column: "CaserioId");

            migrationBuilder.CreateIndex(
                name: "IX_EncuestasAutorizadas_CodEncuesta",
                table: "EncuestasAutorizadas",
                column: "CodEncuesta",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EncuestasAutorizadas_MunicipioId",
                table: "EncuestasAutorizadas",
                column: "MunicipioId");

            migrationBuilder.CreateIndex(
                name: "IX_EncuestasAutorizadas_UserId",
                table: "EncuestasAutorizadas",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EncuestasAutorizadas");

            migrationBuilder.AlterColumn<string>(
                name: "CodParcela",
                table: "Fichas",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(25)",
                oldMaxLength: 25,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CodEncuesta",
                table: "Fichas",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(25)",
                oldMaxLength: 25);
        }
    }
}
