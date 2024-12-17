using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace FlowerSellerTgBot.Migrations
{
    /// <inheritdoc />
    public partial class InitMigr : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "cartObjects",
                columns: table => new
                {
                    ChatId = table.Column<string>(type: "text", nullable: false),
                    CartKey = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cartObjects", x => x.ChatId);
                });

            migrationBuilder.CreateTable(
                name: "cartproductObjects",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Cartkey = table.Column<string>(type: "text", nullable: true),
                    ProductId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cartproductObjects", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "categoryObjects",
                columns: table => new
                {
                    CategoryId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NameOf = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_categoryObjects", x => x.CategoryId);
                });

            migrationBuilder.CreateTable(
                name: "photoObjects",
                columns: table => new
                {
                    FileId = table.Column<string>(type: "text", nullable: false),
                    PhotoKey = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_photoObjects", x => x.FileId);
                });

            migrationBuilder.CreateTable(
                name: "productObjects",
                columns: table => new
                {
                    ProductId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CategoryId = table.Column<int>(type: "integer", nullable: false),
                    SellerId = table.Column<int>(type: "integer", nullable: false),
                    MediaKey = table.Column<string>(type: "text", nullable: true),
                    ProductName = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Price = table.Column<string>(type: "text", nullable: true),
                    Amount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_productObjects", x => x.ProductId);
                });

            migrationBuilder.CreateTable(
                name: "sellerObjects",
                columns: table => new
                {
                    SellerId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ChatId = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sellerObjects", x => x.SellerId);
                });

            migrationBuilder.CreateTable(
                name: "videoObjects",
                columns: table => new
                {
                    FileId = table.Column<string>(type: "text", nullable: false),
                    VideoKey = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_videoObjects", x => x.FileId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "cartObjects");

            migrationBuilder.DropTable(
                name: "cartproductObjects");

            migrationBuilder.DropTable(
                name: "categoryObjects");

            migrationBuilder.DropTable(
                name: "photoObjects");

            migrationBuilder.DropTable(
                name: "productObjects");

            migrationBuilder.DropTable(
                name: "sellerObjects");

            migrationBuilder.DropTable(
                name: "videoObjects");
        }
    }
}
