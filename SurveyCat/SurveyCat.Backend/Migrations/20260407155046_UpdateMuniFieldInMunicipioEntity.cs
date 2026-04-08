using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SurveyCat.Backend.Migrations
{
    /// <inheritdoc />
    public partial class UpdateMuniFieldInMunicipioEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CodMun",
                table: "Municipios",
                newName: "CodMuni");

            migrationBuilder.RenameIndex(
                name: "IX_Municipios_CodMun",
                table: "Municipios",
                newName: "IX_Municipios_CodMuni");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CodMuni",
                table: "Municipios",
                newName: "CodMun");

            migrationBuilder.RenameIndex(
                name: "IX_Municipios_CodMuni",
                table: "Municipios",
                newName: "IX_Municipios_CodMun");
        }
    }
}
