using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Ridely.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "com");

            migrationBuilder.EnsureSchema(
                name: "drv");

            migrationBuilder.EnsureSchema(
                name: "rds");

            migrationBuilder.EnsureSchema(
                name: "rdr");

            migrationBuilder.EnsureSchema(
                name: "usr");

            migrationBuilder.EnsureSchema(
                name: "trx");

            migrationBuilder.CreateTable(
                name: "Bank",
                schema: "com",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Code = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    Type = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bank", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Cab",
                schema: "drv",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Manufacturer = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Color = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Model = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    LicensePlateNo = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Year = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    CabType = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cab", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DriverWallet",
                schema: "drv",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DriverId = table.Column<long>(type: "bigint", nullable: false),
                    Pin = table.Column<string>(type: "text", nullable: true),
                    PinResetCode = table.Column<string>(type: "text", nullable: true),
                    PinResetCodeExpiry = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    AvailableBalance = table.Column<decimal>(type: "numeric", nullable: false),
                    TotalBalance = table.Column<decimal>(type: "numeric", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DriverWallet", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OutboxMessages",
                schema: "com",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OccurredAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Type = table.Column<string>(type: "text", nullable: false),
                    Content = table.Column<string>(type: "jsonb", nullable: false),
                    ProcessedOnUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Error = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutboxMessages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Payment",
                schema: "rds",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Amount = table.Column<long>(type: "bigint", nullable: false),
                    Method = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    PaymentCardId = table.Column<long>(type: "bigint", nullable: true),
                    Error = table.Column<string>(type: "jsonb", nullable: true),
                    DiscountInPercent = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payment", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Permission",
                schema: "usr",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(70)", maxLength: 70, nullable: false),
                    Code = table.Column<string>(type: "character varying(70)", maxLength: 70, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permission", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Rider",
                schema: "rdr",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FirstName = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    LastName = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    PhoneNo = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: false),
                    Email = table.Column<string>(type: "character varying(70)", maxLength: 70, nullable: false),
                    Gender = table.Column<int>(type: "integer", nullable: false),
                    DateOfBirth = table.Column<DateOnly>(type: "date", nullable: true),
                    ProfileImageUrl = table.Column<string>(type: "text", nullable: false),
                    ProfileImageUrlExpiry = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ReferralCode = table.Column<string>(type: "text", nullable: true),
                    ReferredByUserId = table.Column<long>(type: "bigint", nullable: true),
                    ReferredByUser = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Lat = table.Column<double>(type: "double precision", nullable: false),
                    Long = table.Column<double>(type: "double precision", nullable: false),
                    DeviceTokenId = table.Column<string>(type: "text", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    RefreshToken = table.Column<string>(type: "text", nullable: true),
                    IsDeactivated = table.Column<bool>(type: "boolean", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    IsBarred = table.Column<bool>(type: "boolean", nullable: false),
                    EmailValidated = table.Column<bool>(type: "boolean", nullable: false),
                    RefreshTokenExpiry = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CurrentRideId = table.Column<long>(type: "bigint", nullable: true),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rider", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RiderWallet",
                schema: "rdr",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RiderId = table.Column<long>(type: "bigint", nullable: false),
                    Pin = table.Column<string>(type: "text", nullable: true),
                    PinResetCode = table.Column<string>(type: "text", nullable: true),
                    PinResetCodeExpiry = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    AvailableBalance = table.Column<decimal>(type: "numeric", nullable: false),
                    TotalBalance = table.Column<decimal>(type: "numeric", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RiderWallet", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Role",
                schema: "usr",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Code = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Role", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Settings",
                schema: "com",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    BaseFare = table.Column<decimal>(type: "numeric", nullable: false),
                    RatePerKilometer = table.Column<decimal>(type: "numeric", nullable: false),
                    DeliveryRatePerKilometer = table.Column<decimal>(type: "numeric", nullable: false),
                    DriverCommissionFromRide = table.Column<decimal>(type: "numeric", nullable: false),
                    RatePerMinute = table.Column<decimal>(type: "numeric", nullable: false),
                    SupportEmails = table.Column<string>(type: "text", nullable: false),
                    SupportPhoneNo = table.Column<string>(type: "text", nullable: false),
                    EmergencyLines = table.Column<string>(type: "text", nullable: false),
                    PremiumCab = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Settings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TransactionLog",
                schema: "trx",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Reference = table.Column<string>(type: "character varying(26)", nullable: false),
                    Content = table.Column<string>(type: "jsonb", nullable: false),
                    Event = table.Column<string>(type: "text", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransactionLog", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TransactionReferenceMap",
                schema: "trx",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Reference = table.Column<string>(type: "character varying(26)", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransactionReferenceMap", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WaitTime",
                schema: "rds",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Amount = table.Column<long>(type: "bigint", nullable: false),
                    Minute = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WaitTime", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Driver",
                schema: "drv",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FirstName = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    LastName = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    Email = table.Column<string>(type: "character varying(70)", maxLength: 70, nullable: false),
                    PhoneNo = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: false),
                    ProfileImageUrl = table.Column<string>(type: "text", nullable: false),
                    ProfileImageUrlExpiry = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Gender = table.Column<int>(type: "integer", nullable: false),
                    DateOfBirth = table.Column<DateOnly>(type: "date", nullable: true),
                    IdentityValidated = table.Column<bool>(type: "boolean", nullable: false),
                    LicenseNo = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    LicenseImageUrl = table.Column<string>(type: "text", nullable: false),
                    IdentityType = table.Column<int>(type: "integer", nullable: false),
                    IdentityTypeImageUrl = table.Column<string>(type: "text", nullable: false),
                    IdentityNo = table.Column<string>(type: "text", nullable: false),
                    LicenseImageUrlExpiry = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Lat = table.Column<double>(type: "double precision", nullable: false),
                    Long = table.Column<double>(type: "double precision", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    CabId = table.Column<long>(type: "bigint", nullable: false),
                    CompletedTrips = table.Column<int>(type: "integer", nullable: false),
                    RidesRated = table.Column<int>(type: "integer", nullable: false),
                    AvgRatings = table.Column<decimal>(type: "numeric", nullable: false),
                    RideRequestsDeclined = table.Column<int>(type: "integer", nullable: false),
                    DriverService = table.Column<int>(type: "integer", nullable: false),
                    ReferralCode = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    ReferredByUserId = table.Column<long>(type: "bigint", nullable: true),
                    ReferredByUser = table.Column<int>(type: "integer", nullable: false),
                    DeviceTokenId = table.Column<string>(type: "text", nullable: true),
                    RefreshToken = table.Column<string>(type: "text", nullable: true),
                    RefreshTokenExpiry = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeactivated = table.Column<bool>(type: "boolean", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    IsBarred = table.Column<bool>(type: "boolean", nullable: false),
                    EmailValidated = table.Column<bool>(type: "boolean", nullable: false),
                    CurrentRideId = table.Column<long>(type: "bigint", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Driver", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Driver_Cab_CabId",
                        column: x => x.CabId,
                        principalSchema: "drv",
                        principalTable: "Cab",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PaymentDetail",
                schema: "rds",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Reference = table.Column<string>(type: "character varying(26)", nullable: false),
                    PaymentId = table.Column<long>(type: "bigint", nullable: false),
                    PaymentFor = table.Column<int>(type: "integer", nullable: false),
                    Amount = table.Column<long>(type: "bigint", nullable: false),
                    AmountDue = table.Column<long>(type: "bigint", nullable: false),
                    Credit = table.Column<long>(type: "bigint", nullable: false),
                    Sequence = table.Column<int>(type: "integer", nullable: false),
                    PaymentStatus = table.Column<int>(type: "integer", nullable: false),
                    Error = table.Column<string>(type: "text", nullable: true),
                    PaidAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PaymentDetail_Payment_PaymentId",
                        column: x => x.PaymentId,
                        principalSchema: "rds",
                        principalTable: "Payment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PaymentCard",
                schema: "rdr",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RiderId = table.Column<long>(type: "bigint", nullable: false),
                    AuthorizationCode = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    Last4Digits = table.Column<string>(type: "character varying(6)", maxLength: 6, nullable: false),
                    Bank = table.Column<string>(type: "character varying(70)", maxLength: 70, nullable: false),
                    CardType = table.Column<int>(type: "integer", nullable: false),
                    Email = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Signature = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    ExpiryMonth = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    ExpiryYear = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentCard", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PaymentCard_Rider_RiderId",
                        column: x => x.RiderId,
                        principalSchema: "rdr",
                        principalTable: "Rider",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RiderDiscount",
                schema: "rdr",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RiderId = table.Column<long>(type: "bigint", nullable: false),
                    DiscountInPercent = table.Column<int>(type: "integer", nullable: false),
                    Slots = table.Column<int>(type: "integer", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ExpiredAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RiderDiscount", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RiderDiscount_Rider_RiderId",
                        column: x => x.RiderId,
                        principalSchema: "rdr",
                        principalTable: "Rider",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RiderReferrers",
                schema: "rdr",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RiderId = table.Column<long>(type: "bigint", nullable: false),
                    ReferredUserId = table.Column<long>(type: "bigint", nullable: false),
                    ReferredUser = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RiderReferrers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RiderReferrers_Rider_RiderId",
                        column: x => x.RiderId,
                        principalSchema: "rdr",
                        principalTable: "Rider",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SavedLocation",
                schema: "rdr",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RiderId = table.Column<long>(type: "bigint", nullable: false),
                    LocationType = table.Column<int>(type: "integer", nullable: false),
                    Coordinates = table.Column<string>(type: "text", nullable: false),
                    Address = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SavedLocation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SavedLocation_Rider_RiderId",
                        column: x => x.RiderId,
                        principalSchema: "rdr",
                        principalTable: "Rider",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RolePermission",
                schema: "usr",
                columns: table => new
                {
                    RoleId = table.Column<long>(type: "bigint", nullable: false),
                    PermissionId = table.Column<long>(type: "bigint", nullable: false),
                    Id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolePermission", x => new { x.RoleId, x.PermissionId });
                    table.ForeignKey(
                        name: "FK_RolePermission_Permission_PermissionId",
                        column: x => x.PermissionId,
                        principalSchema: "usr",
                        principalTable: "Permission",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RolePermission_Role_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "usr",
                        principalTable: "Role",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "User",
                schema: "usr",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FirstName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    LastName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    PhoneNo = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: false),
                    Password = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    RoleId = table.Column<long>(type: "bigint", nullable: false),
                    RefreshToken = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    RefreshTokenExpiry = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<long>(type: "bigint", nullable: true),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                    table.ForeignKey(
                        name: "FK_User_Role_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "usr",
                        principalTable: "Role",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BankAccount",
                schema: "drv",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DriverId = table.Column<long>(type: "bigint", nullable: false),
                    AccountNo = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    AccountName = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    BankId = table.Column<long>(type: "bigint", nullable: false),
                    RecipientCode = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BankAccount", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BankAccount_Bank_BankId",
                        column: x => x.BankId,
                        principalSchema: "com",
                        principalTable: "Bank",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BankAccount_Driver_DriverId",
                        column: x => x.DriverId,
                        principalSchema: "drv",
                        principalTable: "Driver",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DriverDiscount",
                schema: "drv",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DriverId = table.Column<long>(type: "bigint", nullable: false),
                    Slots = table.Column<int>(type: "integer", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ExpiredAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DriverDiscount", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DriverDiscount_Driver_DriverId",
                        column: x => x.DriverId,
                        principalSchema: "drv",
                        principalTable: "Driver",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DriverReferrers",
                schema: "drv",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DriverId = table.Column<long>(type: "bigint", nullable: false),
                    ReferredUserId = table.Column<long>(type: "bigint", nullable: false),
                    ReferredUser = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DriverReferrers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DriverReferrers_Driver_DriverId",
                        column: x => x.DriverId,
                        principalSchema: "drv",
                        principalTable: "Driver",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Ride",
                schema: "rds",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RiderId = table.Column<long>(type: "bigint", nullable: false),
                    DriverId = table.Column<long>(type: "bigint", nullable: true),
                    HaveConversation = table.Column<bool>(type: "boolean", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    MusicGenre = table.Column<int>(type: "integer", nullable: false),
                    PaymentId = table.Column<long>(type: "bigint", nullable: false),
                    EstimatedFare = table.Column<long>(type: "bigint", nullable: false),
                    EstimatedDeliveryFare = table.Column<long>(type: "bigint", nullable: false),
                    SourceAddress = table.Column<string>(type: "text", nullable: false),
                    WaypointAddresses = table.Column<string>(type: "text", nullable: false),
                    DestinationAddress = table.Column<string>(type: "text", nullable: false),
                    SourceCordinates = table.Column<string>(type: "text", nullable: false),
                    WaypointCordinates = table.Column<string>(type: "text", nullable: false),
                    DestinationCordinates = table.Column<string>(type: "text", nullable: false),
                    DistanceInMeters = table.Column<double>(type: "double precision", nullable: false),
                    EstimatedDurationInSeconds = table.Column<int>(type: "integer", nullable: false),
                    ReassignFromId = table.Column<long>(type: "bigint", nullable: true),
                    Category = table.Column<int>(type: "integer", nullable: false),
                    WasRerouted = table.Column<bool>(type: "boolean", nullable: false),
                    RerouteTo = table.Column<string>(type: "text", nullable: false),
                    CancellationReason = table.Column<string>(type: "text", nullable: true),
                    CancelledBy = table.Column<int>(type: "integer", nullable: false),
                    ReassignReason = table.Column<string>(type: "text", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ride", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Ride_Driver_DriverId",
                        column: x => x.DriverId,
                        principalSchema: "drv",
                        principalTable: "Driver",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Ride_Payment_PaymentId",
                        column: x => x.PaymentId,
                        principalSchema: "rds",
                        principalTable: "Payment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Ride_Ride_ReassignFromId",
                        column: x => x.ReassignFromId,
                        principalSchema: "rds",
                        principalTable: "Ride",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Ride_Rider_RiderId",
                        column: x => x.RiderId,
                        principalSchema: "rdr",
                        principalTable: "Rider",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CallLog",
                schema: "rds",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RideId = table.Column<long>(type: "bigint", nullable: false),
                    Recipient = table.Column<int>(type: "integer", nullable: false),
                    Caller = table.Column<int>(type: "integer", nullable: false),
                    DurationInSeconds = table.Column<int>(type: "integer", nullable: false),
                    CallStartUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CallEndUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CallLog", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CallLog_Ride_RideId",
                        column: x => x.RideId,
                        principalSchema: "rds",
                        principalTable: "Ride",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Chat",
                schema: "rds",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RideId = table.Column<long>(type: "bigint", nullable: false),
                    Sender = table.Column<int>(type: "integer", nullable: false),
                    Recipient = table.Column<int>(type: "integer", nullable: false),
                    Message = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    IsRead = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Chat", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Chat_Ride_RideId",
                        column: x => x.RideId,
                        principalSchema: "rds",
                        principalTable: "Ride",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DriverTransactionHistory",
                schema: "drv",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Reference = table.Column<string>(type: "character varying(26)", nullable: false),
                    DriverId = table.Column<long>(type: "bigint", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    Error = table.Column<string>(type: "text", nullable: true),
                    BankAccountDetails = table.Column<string>(type: "jsonb", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    RideId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DriverTransactionHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DriverTransactionHistory_Driver_DriverId",
                        column: x => x.DriverId,
                        principalSchema: "drv",
                        principalTable: "Driver",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DriverTransactionHistory_Ride_RideId",
                        column: x => x.RideId,
                        principalSchema: "rds",
                        principalTable: "Ride",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Ratings",
                schema: "rds",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RideId = table.Column<long>(type: "bigint", nullable: false),
                    Rating = table.Column<int>(type: "integer", nullable: false),
                    Feedback = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ratings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Ratings_Ride_RideId",
                        column: x => x.RideId,
                        principalSchema: "rds",
                        principalTable: "Ride",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RideLog",
                schema: "rds",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RideId = table.Column<long>(type: "bigint", nullable: false),
                    Details = table.Column<string>(type: "text", nullable: true),
                    Event = table.Column<int>(type: "integer", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RideLog", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RideLog_Ride_RideId",
                        column: x => x.RideId,
                        principalSchema: "rds",
                        principalTable: "Ride",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RiderTransactionHistory",
                schema: "rdr",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Reference = table.Column<string>(type: "character varying(26)", nullable: false),
                    RiderId = table.Column<long>(type: "bigint", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    Error = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    PaymentProvider = table.Column<int>(type: "integer", nullable: false),
                    RideId = table.Column<long>(type: "bigint", nullable: true),
                    RidePaymentReferences = table.Column<string>(type: "text", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RiderTransactionHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RiderTransactionHistory_Ride_RideId",
                        column: x => x.RideId,
                        principalSchema: "rds",
                        principalTable: "Ride",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_RiderTransactionHistory_Rider_RiderId",
                        column: x => x.RiderId,
                        principalSchema: "rdr",
                        principalTable: "Rider",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_BankAccount_BankId",
                schema: "drv",
                table: "BankAccount",
                column: "BankId");

            migrationBuilder.CreateIndex(
                name: "IX_BankAccount_DriverId",
                schema: "drv",
                table: "BankAccount",
                column: "DriverId");

            migrationBuilder.CreateIndex(
                name: "IX_CallLog_RideId",
                schema: "rds",
                table: "CallLog",
                column: "RideId");

            migrationBuilder.CreateIndex(
                name: "IX_Chat_RideId",
                schema: "rds",
                table: "Chat",
                column: "RideId");

            migrationBuilder.CreateIndex(
                name: "IX_Driver_CabId",
                schema: "drv",
                table: "Driver",
                column: "CabId");

            migrationBuilder.CreateIndex(
                name: "IX_Driver_Email",
                schema: "drv",
                table: "Driver",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Driver_PhoneNo",
                schema: "drv",
                table: "Driver",
                column: "PhoneNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Driver_ReferralCode",
                schema: "drv",
                table: "Driver",
                column: "ReferralCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DriverDiscount_DriverId",
                schema: "drv",
                table: "DriverDiscount",
                column: "DriverId");

            migrationBuilder.CreateIndex(
                name: "IX_DriverReferrers_DriverId",
                schema: "drv",
                table: "DriverReferrers",
                column: "DriverId");

            migrationBuilder.CreateIndex(
                name: "IX_DriverTransactionHistory_DriverId",
                schema: "drv",
                table: "DriverTransactionHistory",
                column: "DriverId");

            migrationBuilder.CreateIndex(
                name: "IX_DriverTransactionHistory_Reference",
                schema: "drv",
                table: "DriverTransactionHistory",
                column: "Reference");

            migrationBuilder.CreateIndex(
                name: "IX_DriverTransactionHistory_RideId",
                schema: "drv",
                table: "DriverTransactionHistory",
                column: "RideId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentCard_RiderId",
                schema: "rdr",
                table: "PaymentCard",
                column: "RiderId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentDetail_PaymentId",
                schema: "rds",
                table: "PaymentDetail",
                column: "PaymentId");

            migrationBuilder.CreateIndex(
                name: "IX_Ratings_RideId",
                schema: "rds",
                table: "Ratings",
                column: "RideId");

            migrationBuilder.CreateIndex(
                name: "IX_Ride_DriverId",
                schema: "rds",
                table: "Ride",
                column: "DriverId");

            migrationBuilder.CreateIndex(
                name: "IX_Ride_PaymentId",
                schema: "rds",
                table: "Ride",
                column: "PaymentId");

            migrationBuilder.CreateIndex(
                name: "IX_Ride_ReassignFromId",
                schema: "rds",
                table: "Ride",
                column: "ReassignFromId");

            migrationBuilder.CreateIndex(
                name: "IX_Ride_RiderId",
                schema: "rds",
                table: "Ride",
                column: "RiderId");

            migrationBuilder.CreateIndex(
                name: "IX_RideLog_RideId",
                schema: "rds",
                table: "RideLog",
                column: "RideId");

            migrationBuilder.CreateIndex(
                name: "IX_Rider_Email",
                schema: "rdr",
                table: "Rider",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Rider_PhoneNo",
                schema: "rdr",
                table: "Rider",
                column: "PhoneNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Rider_ReferralCode",
                schema: "rdr",
                table: "Rider",
                column: "ReferralCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RiderDiscount_RiderId",
                schema: "rdr",
                table: "RiderDiscount",
                column: "RiderId");

            migrationBuilder.CreateIndex(
                name: "IX_RiderReferrers_RiderId",
                schema: "rdr",
                table: "RiderReferrers",
                column: "RiderId");

            migrationBuilder.CreateIndex(
                name: "IX_RiderTransactionHistory_Reference",
                schema: "rdr",
                table: "RiderTransactionHistory",
                column: "Reference");

            migrationBuilder.CreateIndex(
                name: "IX_RiderTransactionHistory_RideId",
                schema: "rdr",
                table: "RiderTransactionHistory",
                column: "RideId");

            migrationBuilder.CreateIndex(
                name: "IX_RiderTransactionHistory_RiderId",
                schema: "rdr",
                table: "RiderTransactionHistory",
                column: "RiderId");

            migrationBuilder.CreateIndex(
                name: "IX_RolePermission_PermissionId",
                schema: "usr",
                table: "RolePermission",
                column: "PermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_SavedLocation_RiderId",
                schema: "rdr",
                table: "SavedLocation",
                column: "RiderId");

            migrationBuilder.CreateIndex(
                name: "IX_User_Email",
                schema: "usr",
                table: "User",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_User_PhoneNo",
                schema: "usr",
                table: "User",
                column: "PhoneNo");

            migrationBuilder.CreateIndex(
                name: "IX_User_RoleId",
                schema: "usr",
                table: "User",
                column: "RoleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BankAccount",
                schema: "drv");

            migrationBuilder.DropTable(
                name: "CallLog",
                schema: "rds");

            migrationBuilder.DropTable(
                name: "Chat",
                schema: "rds");

            migrationBuilder.DropTable(
                name: "DriverDiscount",
                schema: "drv");

            migrationBuilder.DropTable(
                name: "DriverReferrers",
                schema: "drv");

            migrationBuilder.DropTable(
                name: "DriverTransactionHistory",
                schema: "drv");

            migrationBuilder.DropTable(
                name: "DriverWallet",
                schema: "drv");

            migrationBuilder.DropTable(
                name: "OutboxMessages",
                schema: "com");

            migrationBuilder.DropTable(
                name: "PaymentCard",
                schema: "rdr");

            migrationBuilder.DropTable(
                name: "PaymentDetail",
                schema: "rds");

            migrationBuilder.DropTable(
                name: "Ratings",
                schema: "rds");

            migrationBuilder.DropTable(
                name: "RideLog",
                schema: "rds");

            migrationBuilder.DropTable(
                name: "RiderDiscount",
                schema: "rdr");

            migrationBuilder.DropTable(
                name: "RiderReferrers",
                schema: "rdr");

            migrationBuilder.DropTable(
                name: "RiderTransactionHistory",
                schema: "rdr");

            migrationBuilder.DropTable(
                name: "RiderWallet",
                schema: "rdr");

            migrationBuilder.DropTable(
                name: "RolePermission",
                schema: "usr");

            migrationBuilder.DropTable(
                name: "SavedLocation",
                schema: "rdr");

            migrationBuilder.DropTable(
                name: "Settings",
                schema: "com");

            migrationBuilder.DropTable(
                name: "TransactionLog",
                schema: "trx");

            migrationBuilder.DropTable(
                name: "TransactionReferenceMap",
                schema: "trx");

            migrationBuilder.DropTable(
                name: "User",
                schema: "usr");

            migrationBuilder.DropTable(
                name: "WaitTime",
                schema: "rds");

            migrationBuilder.DropTable(
                name: "Bank",
                schema: "com");

            migrationBuilder.DropTable(
                name: "Ride",
                schema: "rds");

            migrationBuilder.DropTable(
                name: "Permission",
                schema: "usr");

            migrationBuilder.DropTable(
                name: "Role",
                schema: "usr");

            migrationBuilder.DropTable(
                name: "Driver",
                schema: "drv");

            migrationBuilder.DropTable(
                name: "Payment",
                schema: "rds");

            migrationBuilder.DropTable(
                name: "Rider",
                schema: "rdr");

            migrationBuilder.DropTable(
                name: "Cab",
                schema: "drv");
        }
    }
}
