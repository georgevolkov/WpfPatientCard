using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CardIndexDal.Migrations
{
    public partial class AddTablesCardIndex : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PatientCards",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Fio = table.Column<string>(nullable: false),
                    Gender = table.Column<int>(nullable: false),
                    DateOfBirth = table.Column<DateTime>(nullable: false),
                    Address = table.Column<string>(nullable: true),
                    Phone = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PatientCards", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VisitCards",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VisitDate = table.Column<DateTime>(nullable: false),
                    VisitType = table.Column<int>(nullable: false),
                    Diagnosis = table.Column<string>(nullable: false),
                    PatientCardId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VisitCards", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VisitCards_PatientCards_PatientCardId",
                        column: x => x.PatientCardId,
                        principalTable: "PatientCards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_VisitCards_PatientCardId",
                table: "VisitCards",
                column: "PatientCardId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VisitCards");

            migrationBuilder.DropTable(
                name: "PatientCards");
        }
    }
}
