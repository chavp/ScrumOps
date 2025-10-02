using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ScrumOps.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addProductBacklogIdTeamId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ProductBacklogs_TeamId",
                schema: "ProductBacklog",
                table: "ProductBacklogs");

            migrationBuilder.CreateIndex(
                name: "IX_ProductBacklogs_Id_TeamId",
                schema: "ProductBacklog",
                table: "ProductBacklogs",
                columns: new[] { "Id", "TeamId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ProductBacklogs_Id_TeamId",
                schema: "ProductBacklog",
                table: "ProductBacklogs");

            migrationBuilder.CreateIndex(
                name: "IX_ProductBacklogs_TeamId",
                schema: "ProductBacklog",
                table: "ProductBacklogs",
                column: "TeamId",
                unique: true);
        }
    }
}
