using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SurveyCat.Backend.Migrations
{
    /// <inheritdoc />
    public partial class AddPersonaEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Personas",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TipoPersona = table.Column<int>(type: "int", nullable: false),
                    TipoIdentificacionId = table.Column<int>(type: "int", nullable: true),
                    Identificacion = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    PrimerNombre = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    SegundoNombre = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PrimerApellido = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    SegundoApellido = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    NombreCompleto = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Genero = table.Column<int>(type: "int", nullable: true),
                    Edad = table.Column<int>(type: "int", nullable: true),
                    EstadoCivilId = table.Column<int>(type: "int", nullable: true),
                    ProfesionId = table.Column<int>(type: "int", nullable: true),
                    MunicipioId = table.Column<int>(type: "int", nullable: true),
                    BarrioComarcaId = table.Column<int>(type: "int", nullable: true),
                    CaserioId = table.Column<int>(type: "int", nullable: true),
                    Direccion = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    TipoPersonaJuridicaId = table.Column<int>(type: "int", nullable: true),
                    RegistradaEn = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    FechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatorUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdaterUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Personas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Personas_BarriosComarcas_BarrioComarcaId",
                        column: x => x.BarrioComarcaId,
                        principalTable: "BarriosComarcas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Personas_Caserios_CaserioId",
                        column: x => x.CaserioId,
                        principalTable: "Caserios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Personas_Diccionarios_EstadoCivilId",
                        column: x => x.EstadoCivilId,
                        principalTable: "Diccionarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Personas_Diccionarios_ProfesionId",
                        column: x => x.ProfesionId,
                        principalTable: "Diccionarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Personas_Diccionarios_TipoIdentificacionId",
                        column: x => x.TipoIdentificacionId,
                        principalTable: "Diccionarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Personas_Diccionarios_TipoPersonaJuridicaId",
                        column: x => x.TipoPersonaJuridicaId,
                        principalTable: "Diccionarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Personas_Municipios_MunicipioId",
                        column: x => x.MunicipioId,
                        principalTable: "Municipios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Personas_BarrioComarcaId",
                table: "Personas",
                column: "BarrioComarcaId");

            migrationBuilder.CreateIndex(
                name: "IX_Personas_CaserioId",
                table: "Personas",
                column: "CaserioId");

            migrationBuilder.CreateIndex(
                name: "IX_Personas_EstadoCivilId",
                table: "Personas",
                column: "EstadoCivilId");

            migrationBuilder.CreateIndex(
                name: "IX_Personas_Identificacion",
                table: "Personas",
                column: "Identificacion",
                unique: true,
                filter: "[Identificacion] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Personas_MunicipioId",
                table: "Personas",
                column: "MunicipioId");

            migrationBuilder.CreateIndex(
                name: "IX_Personas_ProfesionId",
                table: "Personas",
                column: "ProfesionId");

            migrationBuilder.CreateIndex(
                name: "IX_Personas_TipoIdentificacionId",
                table: "Personas",
                column: "TipoIdentificacionId");

            migrationBuilder.CreateIndex(
                name: "IX_Personas_TipoPersonaJuridicaId",
                table: "Personas",
                column: "TipoPersonaJuridicaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Personas");
        }
    }
}
