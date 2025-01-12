using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuthSystem.Migrations
{
    /// <inheritdoc />
    public partial class uperrrtadayyyyyp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Subject_ExamSubmissions_ExamSubmissionId",
                table: "Subject");

            migrationBuilder.DropTable(
                name: "ExamSubmissions");

            migrationBuilder.DropIndex(
                name: "IX_Subject_ExamSubmissionId",
                table: "Subject");

            migrationBuilder.DropColumn(
                name: "ExamSubmissionId",
                table: "Subject");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ExamSubmissionId",
                table: "Subject",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ExamSubmissions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExamPeriodId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SubjectIds = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SubmissionDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExamSubmissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExamSubmissions_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ExamSubmissions_ExamPeriods_ExamPeriodId",
                        column: x => x.ExamPeriodId,
                        principalTable: "ExamPeriods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Subject_ExamSubmissionId",
                table: "Subject",
                column: "ExamSubmissionId");

            migrationBuilder.CreateIndex(
                name: "IX_ExamSubmissions_ExamPeriodId",
                table: "ExamSubmissions",
                column: "ExamPeriodId");

            migrationBuilder.CreateIndex(
                name: "IX_ExamSubmissions_UserId",
                table: "ExamSubmissions",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Subject_ExamSubmissions_ExamSubmissionId",
                table: "Subject",
                column: "ExamSubmissionId",
                principalTable: "ExamSubmissions",
                principalColumn: "Id");
        }
    }
}
