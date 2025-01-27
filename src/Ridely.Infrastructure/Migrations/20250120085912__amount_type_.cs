using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ridely.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class _amount_type_ : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_RiderTransactionHistory_Reference",
                schema: "rdr",
                table: "RiderTransactionHistory");

            migrationBuilder.DropIndex(
                name: "IX_DriverTransactionHistory_Reference",
                schema: "drv",
                table: "DriverTransactionHistory");

            migrationBuilder.DropColumn(
                name: "RideId",
                schema: "rdr",
                table: "RiderTransactionHistory");

            migrationBuilder.DropColumn(
                name: "FromRidePaymentOption",
                schema: "rds",
                table: "Payment");

            migrationBuilder.DropColumn(
                name: "FromWallet",
                schema: "rds",
                table: "Payment");

            migrationBuilder.AlterColumn<long>(
                name: "EstimatedFare",
                schema: "rds",
                table: "Ride",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(20,0)");

            migrationBuilder.AddColumn<long>(
                name: "EstimatedDeliveryFare",
                schema: "rds",
                table: "Ride",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<int>(
                name: "EstimatedDurationInSeconds",
                schema: "rds",
                table: "Ride",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<long>(
                name: "Amount",
                schema: "rds",
                table: "Payment",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(20,0)");

            migrationBuilder.CreateIndex(
                name: "IX_RiderTransactionHistory_Reference",
                schema: "rdr",
                table: "RiderTransactionHistory",
                column: "Reference");

            migrationBuilder.CreateIndex(
                name: "IX_DriverTransactionHistory_Reference",
                schema: "drv",
                table: "DriverTransactionHistory",
                column: "Reference");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_RiderTransactionHistory_Reference",
                schema: "rdr",
                table: "RiderTransactionHistory");

            migrationBuilder.DropIndex(
                name: "IX_DriverTransactionHistory_Reference",
                schema: "drv",
                table: "DriverTransactionHistory");

            migrationBuilder.DropColumn(
                name: "EstimatedDeliveryFare",
                schema: "rds",
                table: "Ride");

            migrationBuilder.DropColumn(
                name: "EstimatedDurationInSeconds",
                schema: "rds",
                table: "Ride");

            migrationBuilder.AddColumn<int>(
                name: "RideId",
                schema: "rdr",
                table: "RiderTransactionHistory",
                type: "integer",
                nullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "EstimatedFare",
                schema: "rds",
                table: "Ride",
                type: "numeric(20,0)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<decimal>(
                name: "Amount",
                schema: "rds",
                table: "Payment",
                type: "numeric(20,0)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddColumn<decimal>(
                name: "FromRidePaymentOption",
                schema: "rds",
                table: "Payment",
                type: "numeric(20,0)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "FromWallet",
                schema: "rds",
                table: "Payment",
                type: "numeric(20,0)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateIndex(
                name: "IX_RiderTransactionHistory_Reference",
                schema: "rdr",
                table: "RiderTransactionHistory",
                column: "Reference",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DriverTransactionHistory_Reference",
                schema: "drv",
                table: "DriverTransactionHistory",
                column: "Reference",
                unique: true);
        }
    }
}
