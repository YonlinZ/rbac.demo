using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Library.API.Migrations
{
    public partial class SeedDataByZyl2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Authors",
                columns: new[] { "Id", "BirthDate", "BirthPlace", "Email", "Name" },
                values: new object[] { new Guid("72d5b5f5-3008-49b7-b0d6-cc337f1a3330"), new DateTimeOffset(new DateTime(1960, 11, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 8, 0, 0, 0)), "Place 1", "author1@xxx.com", "Author 1" });

            migrationBuilder.InsertData(
                table: "Authors",
                columns: new[] { "Id", "BirthDate", "BirthPlace", "Email", "Name" },
                values: new object[] { new Guid("7d04a48e-be4e-468e-8ce2-3ac0a0c79549"), new DateTimeOffset(new DateTime(1968, 10, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 8, 0, 0, 0)), "Place 2", "author2@xxx.com", "Author 2" });

            migrationBuilder.InsertData(
                table: "Books",
                columns: new[] { "Id", "AuthorId", "Description", "Pages", "Title" },
                values: new object[,]
                {
                    { new Guid("7d8ebda9-2634-4c0f-9469-0695d6132153"), new Guid("72d5b5f5-3008-49b7-b0d6-cc337f1a3330"), "Description of Book 1", 281, "Book 1" },
                    { new Guid("1ed47697-aa7d-48c2-aa39-305d0e13b3aa"), new Guid("72d5b5f5-3008-49b7-b0d6-cc337f1a3330"), "Description of Book 2", 370, "Book 2" },
                    { new Guid("5f82c852-375d-4926-a3b7-84b63fc1bfae"), new Guid("7d04a48e-be4e-468e-8ce2-3ac0a0c79549"), "Description of Book 3", 229, "Book 3" },
                    { new Guid("418a5b20-460b-4604-be17-2b0809e19acd"), new Guid("7d04a48e-be4e-468e-8ce2-3ac0a0c79549"), "Description of Book 4", 440, "Book 4" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: new Guid("1ed47697-aa7d-48c2-aa39-305d0e13b3aa"));

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: new Guid("418a5b20-460b-4604-be17-2b0809e19acd"));

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: new Guid("5f82c852-375d-4926-a3b7-84b63fc1bfae"));

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: new Guid("7d8ebda9-2634-4c0f-9469-0695d6132153"));

            migrationBuilder.DeleteData(
                table: "Authors",
                keyColumn: "Id",
                keyValue: new Guid("72d5b5f5-3008-49b7-b0d6-cc337f1a3330"));

            migrationBuilder.DeleteData(
                table: "Authors",
                keyColumn: "Id",
                keyValue: new Guid("7d04a48e-be4e-468e-8ce2-3ac0a0c79549"));
        }
    }
}
