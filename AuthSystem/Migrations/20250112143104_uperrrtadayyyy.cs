using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuthSystem.Migrations
{
    /// <inheritdoc />
    public partial class uperrrtadayyyy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExamSubmissions_Subject_SubjectId",
                table: "ExamSubmissions");

            migrationBuilder.DropIndex(
                name: "IX_ExamSubmissions_SubjectId",
                table: "ExamSubmissions");

            migrationBuilder.DropColumn(
                name: "SubjectId",
                table: "ExamSubmissions");

            migrationBuilder.AddColumn<int>(
                name: "ExamSubmissionId",
                table: "Subject",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SubjectIds",
                table: "ExamSubmissions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "[]");

            migrationBuilder.CreateIndex(
                name: "IX_Subject_ExamSubmissionId",
                table: "Subject",
                column: "ExamSubmissionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Subject_ExamSubmissions_ExamSubmissionId",
                table: "Subject",
                column: "ExamSubmissionId",
                principalTable: "ExamSubmissions",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Subject_ExamSubmissions_ExamSubmissionId",
                table: "Subject");

            migrationBuilder.DropIndex(
                name: "IX_Subject_ExamSubmissionId",
                table: "Subject");

            migrationBuilder.DropColumn(
                name: "ExamSubmissionId",
                table: "Subject");

            migrationBuilder.DropColumn(
                name: "SubjectIds",
                table: "ExamSubmissions");

            migrationBuilder.AddColumn<int>(
                name: "SubjectId",
                table: "ExamSubmissions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ExamSubmissions_SubjectId",
                table: "ExamSubmissions",
                column: "SubjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_ExamSubmissions_Subject_SubjectId",
                table: "ExamSubmissions",
                column: "SubjectId",
                principalTable: "Subject",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
