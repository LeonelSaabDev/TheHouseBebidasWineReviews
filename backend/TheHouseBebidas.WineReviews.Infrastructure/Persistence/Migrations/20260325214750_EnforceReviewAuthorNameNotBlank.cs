using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TheHouseBebidas.WineReviews.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class EnforceReviewAuthorNameNotBlank : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                UPDATE [Reviews]
                SET [AuthorName] = 'Legacy User'
                WHERE LTRIM(RTRIM(ISNULL([AuthorName], ''))) = '';
                """);

            migrationBuilder.Sql("""
                IF NOT EXISTS (
                    SELECT 1
                    FROM sys.check_constraints
                    WHERE [name] = N'CK_Reviews_AuthorName_NotBlank'
                )
                BEGIN
                    ALTER TABLE [Reviews]
                    ADD CONSTRAINT [CK_Reviews_AuthorName_NotBlank]
                    CHECK (LEN(LTRIM(RTRIM([AuthorName]))) > 0);
                END
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Reviews_AuthorName_NotBlank",
                table: "Reviews");
        }
    }
}
