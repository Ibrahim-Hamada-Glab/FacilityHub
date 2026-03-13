using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FacilityHub.Infra.Migrations
{
    /// <inheritdoc />
    public partial class UpdateManagerFacilityRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Facilities_Users_ManagerId",
                table: "Facilities");

            migrationBuilder.AddForeignKey(
                name: "FK_Facilities_Users_ManagerId",
                table: "Facilities",
                column: "ManagerId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Facilities_Users_ManagerId",
                table: "Facilities");

            migrationBuilder.AddForeignKey(
                name: "FK_Facilities_Users_ManagerId",
                table: "Facilities",
                column: "ManagerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
