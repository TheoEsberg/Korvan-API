using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Korvan_API.Migrations
{
    /// <inheritdoc />
    public partial class templates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ScheduleTemplates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    CreatedById = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScheduleTemplates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScheduleTemplates_Users_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TemplateDayPlans",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ShiftDate = table.Column<DateOnly>(type: "date", nullable: false),
                    TemplateId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsPublished = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedById = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TemplateDayPlans", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TemplateDayPlans_ScheduleTemplates_TemplateId",
                        column: x => x.TemplateId,
                        principalTable: "ScheduleTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TemplateDayPlans_Users_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TemplateSlots",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TemplateId = table.Column<Guid>(type: "uuid", nullable: false),
                    StartTime = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
                    EndTime = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
                    SortOrder = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TemplateSlots", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TemplateSlots_ScheduleTemplates_TemplateId",
                        column: x => x.TemplateId,
                        principalTable: "ScheduleTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TemplateSlotPlans",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DayPlanId = table.Column<Guid>(type: "uuid", nullable: false),
                    SlotId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TemplateSlotPlans", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TemplateSlotPlans_TemplateDayPlans_DayPlanId",
                        column: x => x.DayPlanId,
                        principalTable: "TemplateDayPlans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TemplateSlotPlans_TemplateSlots_SlotId",
                        column: x => x.SlotId,
                        principalTable: "TemplateSlots",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TemplateSlotPlans_Users_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ScheduleTemplates_CreatedById",
                table: "ScheduleTemplates",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_TemplateDayPlans_CreatedById",
                table: "TemplateDayPlans",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_TemplateDayPlans_TemplateId",
                table: "TemplateDayPlans",
                column: "TemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_TemplateSlotPlans_DayPlanId",
                table: "TemplateSlotPlans",
                column: "DayPlanId");

            migrationBuilder.CreateIndex(
                name: "IX_TemplateSlotPlans_EmployeeId",
                table: "TemplateSlotPlans",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_TemplateSlotPlans_SlotId",
                table: "TemplateSlotPlans",
                column: "SlotId");

            migrationBuilder.CreateIndex(
                name: "IX_TemplateSlots_TemplateId",
                table: "TemplateSlots",
                column: "TemplateId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TemplateSlotPlans");

            migrationBuilder.DropTable(
                name: "TemplateDayPlans");

            migrationBuilder.DropTable(
                name: "TemplateSlots");

            migrationBuilder.DropTable(
                name: "ScheduleTemplates");
        }
    }
}
