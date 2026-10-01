using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PortfolioCite.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCertificateDisplayOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DisplayOrder",
                table: "Certificates",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.Sql("""
                UPDATE "Certificates" AS certificate
                SET "DisplayOrder" = ranked.position
                FROM (
                    SELECT "Id", (ROW_NUMBER() OVER (ORDER BY "IssuedOn" DESC, "Id" ASC) - 1)::integer AS position
                    FROM "Certificates"
                ) AS ranked
                WHERE certificate."Id" = ranked."Id";
                """);

            migrationBuilder.AlterColumn<int>(
                name: "DisplayOrder",
                table: "Certificates",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldDefaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DisplayOrder",
                table: "Certificates");
        }
    }
}
