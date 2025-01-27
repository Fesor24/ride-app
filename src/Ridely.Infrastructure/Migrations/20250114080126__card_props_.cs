using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ridely.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class _card_props_ : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Email",
                schema: "rdr",
                table: "PaymentCard",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Signature",
                schema: "rdr",
                table: "PaymentCard",
                type: "character varying(250)",
                maxLength: 250,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Email",
                schema: "rdr",
                table: "PaymentCard");

            migrationBuilder.DropColumn(
                name: "Signature",
                schema: "rdr",
                table: "PaymentCard");
        }
    }
}
