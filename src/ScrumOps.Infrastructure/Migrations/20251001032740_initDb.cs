using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ScrumOps.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class initDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "ProductBacklog");

            migrationBuilder.EnsureSchema(
                name: "SprintManagement");

            migrationBuilder.EnsureSchema(
                name: "TeamManagement");

            migrationBuilder.CreateTable(
                name: "ProductBacklogs",
                schema: "ProductBacklog",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TeamId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastRefinedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Notes = table.Column<string>(type: "character varying(5000)", maxLength: 5000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductBacklogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Sprints",
                schema: "SprintManagement",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TeamId = table.Column<Guid>(type: "uuid", nullable: false),
                    Goal = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CapacityHours = table.Column<int>(type: "integer", nullable: false),
                    ActualVelocity = table.Column<int>(type: "integer", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ActualStartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ActualEndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sprints", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Teams",
                schema: "TeamManagement",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    SprintLengthWeeks = table.Column<int>(type: "integer", nullable: false),
                    CurrentVelocityValue = table.Column<decimal>(type: "numeric", nullable: false, defaultValue: 0m),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Teams", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProductBacklogItems",
                schema: "ProductBacklog",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductBacklogId = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    Type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Priority = table.Column<int>(type: "integer", nullable: false),
                    StoryPoints = table.Column<int>(type: "integer", nullable: true),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    AcceptanceCriteria = table.Column<string>(type: "character varying(5000)", maxLength: 5000, nullable: true),
                    CreatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductBacklogItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductBacklogItems_ProductBacklogs_ProductBacklogId",
                        column: x => x.ProductBacklogId,
                        principalSchema: "ProductBacklog",
                        principalTable: "ProductBacklogs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SprintBacklogItems",
                schema: "SprintManagement",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SprintId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductBacklogItemId = table.Column<Guid>(type: "uuid", nullable: false),
                    StoryPoints = table.Column<int>(type: "integer", nullable: false),
                    OriginalEstimate = table.Column<int>(type: "integer", nullable: false),
                    RemainingWork = table.Column<int>(type: "integer", nullable: false),
                    AddedToSprintDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CompletedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SprintBacklogItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SprintBacklogItems_Sprints_SprintId",
                        column: x => x.SprintId,
                        principalSchema: "SprintManagement",
                        principalTable: "Sprints",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                schema: "TeamManagement",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TeamId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "character varying(320)", maxLength: 320, nullable: false),
                    Role = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    RoleIsSingleton = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastLoginDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_Teams_TeamId",
                        column: x => x.TeamId,
                        principalSchema: "TeamManagement",
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Tasks",
                schema: "SprintManagement",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SprintBacklogItemId = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    OriginalEstimateHours = table.Column<int>(type: "integer", nullable: false),
                    RemainingHours = table.Column<int>(type: "integer", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    StartedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CompletedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tasks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tasks_SprintBacklogItems_SprintBacklogItemId",
                        column: x => x.SprintBacklogItemId,
                        principalSchema: "SprintManagement",
                        principalTable: "SprintBacklogItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductBacklogItems_CreatedDate",
                schema: "ProductBacklog",
                table: "ProductBacklogItems",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_ProductBacklogItems_Priority",
                schema: "ProductBacklog",
                table: "ProductBacklogItems",
                column: "Priority");

            migrationBuilder.CreateIndex(
                name: "IX_ProductBacklogItems_ProductBacklogId",
                schema: "ProductBacklog",
                table: "ProductBacklogItems",
                column: "ProductBacklogId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductBacklogItems_Status",
                schema: "ProductBacklog",
                table: "ProductBacklogItems",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_ProductBacklogItems_Title",
                schema: "ProductBacklog",
                table: "ProductBacklogItems",
                column: "Title");

            migrationBuilder.CreateIndex(
                name: "IX_ProductBacklogItems_Type",
                schema: "ProductBacklog",
                table: "ProductBacklogItems",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_ProductBacklogs_CreatedDate",
                schema: "ProductBacklog",
                table: "ProductBacklogs",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_ProductBacklogs_LastRefinedDate",
                schema: "ProductBacklog",
                table: "ProductBacklogs",
                column: "LastRefinedDate");

            migrationBuilder.CreateIndex(
                name: "IX_ProductBacklogs_TeamId",
                schema: "ProductBacklog",
                table: "ProductBacklogs",
                column: "TeamId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SprintBacklogItems_CompletedDate",
                schema: "SprintManagement",
                table: "SprintBacklogItems",
                column: "CompletedDate");

            migrationBuilder.CreateIndex(
                name: "IX_SprintBacklogItems_ProductBacklogItemId",
                schema: "SprintManagement",
                table: "SprintBacklogItems",
                column: "ProductBacklogItemId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SprintBacklogItems_SprintId",
                schema: "SprintManagement",
                table: "SprintBacklogItems",
                column: "SprintId");

            migrationBuilder.CreateIndex(
                name: "IX_SprintBacklogItems_SprintId_CompletedDate",
                schema: "SprintManagement",
                table: "SprintBacklogItems",
                columns: new[] { "SprintId", "CompletedDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Sprints_CreatedDate",
                schema: "SprintManagement",
                table: "Sprints",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_Sprints_EndDate",
                schema: "SprintManagement",
                table: "Sprints",
                column: "EndDate");

            migrationBuilder.CreateIndex(
                name: "IX_Sprints_StartDate",
                schema: "SprintManagement",
                table: "Sprints",
                column: "StartDate");

            migrationBuilder.CreateIndex(
                name: "IX_Sprints_Status",
                schema: "SprintManagement",
                table: "Sprints",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Sprints_TeamId",
                schema: "SprintManagement",
                table: "Sprints",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_Sprints_TeamId_Status",
                schema: "SprintManagement",
                table: "Sprints",
                columns: new[] { "TeamId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_CreatedDate",
                schema: "SprintManagement",
                table: "Tasks",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_SprintBacklogItemId",
                schema: "SprintManagement",
                table: "Tasks",
                column: "SprintBacklogItemId");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_Status",
                schema: "SprintManagement",
                table: "Tasks",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Teams_CreatedDate",
                schema: "TeamManagement",
                table: "Teams",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_Teams_IsActive",
                schema: "TeamManagement",
                table: "Teams",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Teams_Name",
                schema: "TeamManagement",
                table: "Teams",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                schema: "TeamManagement",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_IsActive",
                schema: "TeamManagement",
                table: "Users",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Users_TeamId",
                schema: "TeamManagement",
                table: "Users",
                column: "TeamId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductBacklogItems",
                schema: "ProductBacklog");

            migrationBuilder.DropTable(
                name: "Tasks",
                schema: "SprintManagement");

            migrationBuilder.DropTable(
                name: "Users",
                schema: "TeamManagement");

            migrationBuilder.DropTable(
                name: "ProductBacklogs",
                schema: "ProductBacklog");

            migrationBuilder.DropTable(
                name: "SprintBacklogItems",
                schema: "SprintManagement");

            migrationBuilder.DropTable(
                name: "Teams",
                schema: "TeamManagement");

            migrationBuilder.DropTable(
                name: "Sprints",
                schema: "SprintManagement");
        }
    }
}
