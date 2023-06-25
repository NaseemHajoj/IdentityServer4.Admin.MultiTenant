using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Skoruba.IdentityServer4.Admin.EntityFramework.SqlServer.Migrations.DataProtection
{
    public partial class DbInit : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "idp");

            migrationBuilder.CreateTable(
                name: "DataProtectionKeys",
                schema: "idp",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FriendlyName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Xml = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataProtectionKeys", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DataProtectionKeys",
                schema: "idp");
        }
    }
}
