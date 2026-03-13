using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FacilityHub.Infra.Migrations
{
    /// <inheritdoc />
    public partial class AddCodeNumber : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Code",
                table: "Facilities");

            migrationBuilder.CreateSequence(
                name: "FacilityCodeNumber",
                maxValue: 10000000L);

            migrationBuilder.AddColumn<long>(
                name: "CodeNumber",
                table: "Facilities",
                type: "bigint",
                maxLength: 256,
                nullable: false,
                defaultValueSql: "NEXT VALUE FOR FacilityCodeNumber");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CodeNumber",
                table: "Facilities");

            migrationBuilder.DropSequence(
                name: "FacilityCodeNumber");

            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "Facilities",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "");
        }
    }
}
