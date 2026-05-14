using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SurveyCat.Backend.Migrations
{
    /// <inheritdoc />
    public partial class AddFichaEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Fichas",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MunicipioId = table.Column<int>(type: "int", nullable: false),
                    SectorId = table.Column<int>(type: "int", nullable: false),
                    BarrioComarcaId = table.Column<int>(type: "int", nullable: true),
                    CaserioId = table.Column<int>(type: "int", nullable: true),
                    Direccion = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    Manzana = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Lote = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    CodParcela = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    CodEncuesta = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    NombreFinca = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    EncuestadorId = table.Column<int>(type: "int", nullable: false),
                    CoordinadorId = table.Column<int>(type: "int", nullable: false),
                    TecnicoCatastralId = table.Column<int>(type: "int", nullable: false),
                    TipoEncuesta = table.Column<int>(type: "int", nullable: false),
                    TipoUso = table.Column<int>(type: "int", nullable: true),
                    DescripcionTipoUso = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    AreaEstimada = table.Column<double>(type: "float", nullable: true),
                    UnidadMedidaId = table.Column<int>(type: "int", nullable: true),
                    OrigenTierraId = table.Column<int>(type: "int", nullable: true),
                    Servidumbre = table.Column<bool>(type: "bit", nullable: false),
                    ServidumbreAguaId = table.Column<int>(type: "int", nullable: true),
                    ServidumbrePaseId = table.Column<int>(type: "int", nullable: true),
                    ServidumbreOtraId = table.Column<int>(type: "int", nullable: true),
                    PresentaConflicto = table.Column<bool>(type: "bit", nullable: false),
                    FechaEncuesta = table.Column<DateTime>(type: "datetime2", nullable: false),
                    VerificadoCoordinador = table.Column<bool>(type: "bit", nullable: false),
                    EstadoId = table.Column<int>(type: "int", nullable: false),
                    Observacion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InformanteId = table.Column<long>(type: "bigint", nullable: false),
                    RelacionInformanteParcelaId = table.Column<int>(type: "int", nullable: true),
                    RelacionInformantePropietarioId = table.Column<int>(type: "int", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatorUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdaterUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Fichas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Fichas_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Fichas_BarriosComarcas_BarrioComarcaId",
                        column: x => x.BarrioComarcaId,
                        principalTable: "BarriosComarcas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Fichas_Caserios_CaserioId",
                        column: x => x.CaserioId,
                        principalTable: "Caserios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Fichas_Diccionarios_EstadoId",
                        column: x => x.EstadoId,
                        principalTable: "Diccionarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Fichas_Diccionarios_OrigenTierraId",
                        column: x => x.OrigenTierraId,
                        principalTable: "Diccionarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Fichas_Diccionarios_RelacionInformanteParcelaId",
                        column: x => x.RelacionInformanteParcelaId,
                        principalTable: "Diccionarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Fichas_Diccionarios_RelacionInformantePropietarioId",
                        column: x => x.RelacionInformantePropietarioId,
                        principalTable: "Diccionarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Fichas_Diccionarios_ServidumbreAguaId",
                        column: x => x.ServidumbreAguaId,
                        principalTable: "Diccionarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Fichas_Diccionarios_ServidumbreOtraId",
                        column: x => x.ServidumbreOtraId,
                        principalTable: "Diccionarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Fichas_Diccionarios_ServidumbrePaseId",
                        column: x => x.ServidumbrePaseId,
                        principalTable: "Diccionarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Fichas_Diccionarios_UnidadMedidaId",
                        column: x => x.UnidadMedidaId,
                        principalTable: "Diccionarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Fichas_Municipios_MunicipioId",
                        column: x => x.MunicipioId,
                        principalTable: "Municipios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Fichas_PersonalEncuestas_CoordinadorId",
                        column: x => x.CoordinadorId,
                        principalTable: "PersonalEncuestas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Fichas_PersonalEncuestas_EncuestadorId",
                        column: x => x.EncuestadorId,
                        principalTable: "PersonalEncuestas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Fichas_PersonalEncuestas_TecnicoCatastralId",
                        column: x => x.TecnicoCatastralId,
                        principalTable: "PersonalEncuestas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Fichas_Personas_InformanteId",
                        column: x => x.InformanteId,
                        principalTable: "Personas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Fichas_Sectores_SectorId",
                        column: x => x.SectorId,
                        principalTable: "Sectores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Fichas_BarrioComarcaId",
                table: "Fichas",
                column: "BarrioComarcaId");

            migrationBuilder.CreateIndex(
                name: "IX_Fichas_CaserioId",
                table: "Fichas",
                column: "CaserioId");

            migrationBuilder.CreateIndex(
                name: "IX_Fichas_CodEncuesta",
                table: "Fichas",
                column: "CodEncuesta",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Fichas_CoordinadorId",
                table: "Fichas",
                column: "CoordinadorId");

            migrationBuilder.CreateIndex(
                name: "IX_Fichas_EncuestadorId",
                table: "Fichas",
                column: "EncuestadorId");

            migrationBuilder.CreateIndex(
                name: "IX_Fichas_EstadoId",
                table: "Fichas",
                column: "EstadoId");

            migrationBuilder.CreateIndex(
                name: "IX_Fichas_InformanteId",
                table: "Fichas",
                column: "InformanteId");

            migrationBuilder.CreateIndex(
                name: "IX_Fichas_MunicipioId",
                table: "Fichas",
                column: "MunicipioId");

            migrationBuilder.CreateIndex(
                name: "IX_Fichas_OrigenTierraId",
                table: "Fichas",
                column: "OrigenTierraId");

            migrationBuilder.CreateIndex(
                name: "IX_Fichas_RelacionInformanteParcelaId",
                table: "Fichas",
                column: "RelacionInformanteParcelaId");

            migrationBuilder.CreateIndex(
                name: "IX_Fichas_RelacionInformantePropietarioId",
                table: "Fichas",
                column: "RelacionInformantePropietarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Fichas_SectorId",
                table: "Fichas",
                column: "SectorId");

            migrationBuilder.CreateIndex(
                name: "IX_Fichas_ServidumbreAguaId",
                table: "Fichas",
                column: "ServidumbreAguaId");

            migrationBuilder.CreateIndex(
                name: "IX_Fichas_ServidumbreOtraId",
                table: "Fichas",
                column: "ServidumbreOtraId");

            migrationBuilder.CreateIndex(
                name: "IX_Fichas_ServidumbrePaseId",
                table: "Fichas",
                column: "ServidumbrePaseId");

            migrationBuilder.CreateIndex(
                name: "IX_Fichas_TecnicoCatastralId",
                table: "Fichas",
                column: "TecnicoCatastralId");

            migrationBuilder.CreateIndex(
                name: "IX_Fichas_UnidadMedidaId",
                table: "Fichas",
                column: "UnidadMedidaId");

            migrationBuilder.CreateIndex(
                name: "IX_Fichas_UserId",
                table: "Fichas",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Fichas");
        }
    }
}
