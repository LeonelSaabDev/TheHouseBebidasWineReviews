using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TheHouseBebidas.WineReviews.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddWineSecondaryImageUrl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SecondaryImageUrl",
                table: "Wines",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SecondaryImageUrl",
                table: "Wines");
        }
    }
}
