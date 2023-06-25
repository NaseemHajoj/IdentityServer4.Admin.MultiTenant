using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Skoruba.IdentityServer4.Admin.EntityFramework.SqlServer.Migrations.AuditLogging
{
    public partial class DbInit : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "idp");

            migrationBuilder.CreateTable(
                name: "AuditLog",
                schema: "idp",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Event = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Source = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Category = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SubjectIdentifier = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SubjectName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SubjectType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SubjectAdditionalData = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Action = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Data = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLog", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuditLog",
                schema: "idp");
        }
    }
}
