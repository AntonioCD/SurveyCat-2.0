using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SurveyCat.Backend.Migrations
{
    /// <inheritdoc />
    public partial class AddMunicipioBarrioComarcaCaseriosEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Municipios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CodMun = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CodINIDE = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: true),
                    DepartamentoId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Municipios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Municipios_Departamentos_DepartamentoId",
                        column: x => x.DepartamentoId,
                        principalTable: "Departamentos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BarriosComarcas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CodBarrioComarca = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    EsBarrio = table.Column<bool>(type: "bit", nullable: false),
                    MunicipioId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BarriosComarcas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BarriosComarcas_Municipios_MunicipioId",
                        column: x => x.MunicipioId,
                        principalTable: "Municipios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Caserios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CodCaserio = table.Column<int>(type: "int", nullable: false),
                    ComarcaId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Caserios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Caserios_BarriosComarcas_ComarcaId",
                        column: x => x.ComarcaId,
                        principalTable: "BarriosComarcas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BarriosComarcas_CodBarrioComarca",
                table: "BarriosComarcas",
                column: "CodBarrioComarca",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BarriosComarcas_MunicipioId",
                table: "BarriosComarcas",
                column: "MunicipioId");

            migrationBuilder.CreateIndex(
                name: "IX_Caserios_CodCaserio",
                table: "Caserios",
                column: "CodCaserio",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Caserios_ComarcaId",
                table: "Caserios",
                column: "ComarcaId");

            migrationBuilder.CreateIndex(
                name: "IX_Municipios_CodMun",
                table: "Municipios",
                column: "CodMun",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Municipios_DepartamentoId",
                table: "Municipios",
                column: "DepartamentoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Caserios");

            migrationBuilder.DropTable(
                name: "BarriosComarcas");

            migrationBuilder.DropTable(
                name: "Municipios");
        }
    }
}
