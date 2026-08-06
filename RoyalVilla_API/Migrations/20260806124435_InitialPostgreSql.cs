using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace RoyalVilla_API.Migrations
{
    /// <inheritdoc />
    public partial class InitialPostgreSql : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Email = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Password = table.Column<string>(type: "text", nullable: false),
                    Role = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Villas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Details = table.Column<string>(type: "text", nullable: true),
                    Rate = table.Column<double>(type: "double precision", nullable: false),
                    Sqft = table.Column<int>(type: "integer", nullable: false),
                    Occupancy = table.Column<int>(type: "integer", nullable: false),
                    ImageUrl = table.Column<string>(type: "text", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Villas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VillaAmenities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    VillaId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VillaAmenities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VillaAmenities_Villas_VillaId",
                        column: x => x.VillaId,
                        principalTable: "Villas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Villas",
                columns: new[] { "Id", "CreatedDate", "Details", "ImageUrl", "Name", "Occupancy", "Rate", "Sqft", "UpdatedDate" },
                values: new object[,]
                {
                    { 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Luxurious villa with stunning ocean views and private beach access.", "https://dotnetmasteryimages.blob.core.windows.net/bluevillaimages/villa1.jpg", "Royal Villa", 6, 500.0, 2500, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 2, new DateTime(2024, 1, 15, 0, 0, 0, 0, DateTimeKind.Utc), "Elegant villa with marble interiors and panoramic mountain views.", "https://dotnetmasteryimages.blob.core.windows.net/bluevillaimages/villa2.jpg", "Diamond Villa", 8, 750.0, 3200, new DateTime(2024, 1, 15, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 3, new DateTime(2024, 2, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Modern villa featuring an infinity pool and outdoor entertainment area.", "https://dotnetmasteryimages.blob.core.windows.net/bluevillaimages/villa3.jpg", "Pool Villa", 4, 350.0, 1800, new DateTime(2024, 2, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 4, new DateTime(2024, 2, 14, 0, 0, 0, 0, DateTimeKind.Utc), "Premium villa with spa facilities and concierge services.", "https://dotnetmasteryimages.blob.core.windows.net/bluevillaimages/villa4.jpg", "Luxury Villa", 10, 900.0, 4000, new DateTime(2024, 2, 14, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 5, new DateTime(2024, 3, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Charming villa surrounded by tropical gardens and nature trails.", "https://dotnetmasteryimages.blob.core.windows.net/bluevillaimages/villa5.jpg", "Garden Villa", 3, 275.0, 1500, new DateTime(2024, 3, 1, 0, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.CreateIndex(
                name: "IX_VillaAmenities_VillaId",
                table: "VillaAmenities",
                column: "VillaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "VillaAmenities");

            migrationBuilder.DropTable(
                name: "Villas");
        }
    }
}
