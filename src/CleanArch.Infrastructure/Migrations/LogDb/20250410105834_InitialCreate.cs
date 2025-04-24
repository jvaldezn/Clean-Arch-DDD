using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CleanArch.Infrastructure.Migrations.LogDb
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "log",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    hostname = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    logged = table.Column<DateTime>(type: "datetime2", nullable: false),
                    level = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    message = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    logger = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    properties = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    callsite = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    exception = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    applicationId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_log", x => x.id);
                });

            migrationBuilder.InsertData(
                table: "log",
                columns: new[] { "id", "applicationId", "callsite", "exception", "level", "logged", "logger", "hostname", "message", "properties" },
                values: new object[,]
                {
                    { 1, 2, null, null, "ERROR", new DateTime(2025, 4, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "HOST1", "", null },
                    { 2, 1, null, null, "FATAL", new DateTime(2025, 4, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "HOST2", "", null },
                    { 3, 2, null, null, "WARNING", new DateTime(2025, 4, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "HOST3", "", null },
                    { 4, 1, null, null, "FATAL", new DateTime(2025, 4, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "HOST4", "", null },
                    { 5, 2, null, null, "ERROR", new DateTime(2025, 4, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "HOST5", "", null },
                    { 6, 1, null, null, "WARNING", new DateTime(2025, 4, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "HOST6", "", null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "log");
        }
    }
}
