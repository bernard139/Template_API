using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Template.Identity.Migrations
{
    public partial class otp : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OTPs",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OTP = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OTPType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Token = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OTPs", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7f8df141-8a3e-4f3f-82d3-0a89626a4b1c",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "34bb5181-26c4-4e5e-a919-429083a11a10", "AQAAAAIAAYagAAAAEOn7Lo5qHdxQ4wJexnivUVCnHgk7J0Me0vZhmLkmEf6xbgcqE4ls/Tt7Ch5n5cIFvQ==", "507d3eb0-fd77-4f51-881e-07dd895f9f0a" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "b25a925a-9fbd-4e49-89f1-8ec446a8f023",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "3c06f2d1-1a7d-4ba6-a806-f0d8398a2896", "AQAAAAIAAYagAAAAEC3nQM0BbHGI3ATYI/C6P1ilwRKNBy3bJ+w2gDcJ04VEB4io3ufMp6DFfFX2yCYXDg==", "b9b24d92-7982-459c-a660-f0bf233efda2" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OTPs");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7f8df141-8a3e-4f3f-82d3-0a89626a4b1c",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "32416955-d7c3-459f-8f4c-b8a9bb760263", "AQAAAAIAAYagAAAAEO0I4GtTQTrWFHU+JVXGNTl9Prm1d0943RIr+6eyjocKSQUCmQion9Fzx+RJTHRVfw==", "25785c42-14f2-4ec5-a3f1-22a919764fb4" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "b25a925a-9fbd-4e49-89f1-8ec446a8f023",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "3b7b9932-03f2-47a4-9a58-f256106541a7", "AQAAAAIAAYagAAAAEOa0lT8YSJgJGrMcZiSIchK6vl+b0c6AMzKKbT1jy/Jczfa41BIekVGsNe3zOBq34Q==", "4c3ad355-344d-44f3-95ee-d57236315e26" });
        }
    }
}
