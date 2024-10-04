using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TasksMVC.Migrations
{
    /// <inheritdoc />
    public partial class AdminRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
            IF NOT EXISTS(SELECT Id from AspNetRoles WHERE Id = 'bacf9df4-b6be-4995-97cc-2c8e924c3eb3')
            BEGIN
	            INSERT AspNetRoles (Id, [Name], [NormalizedName])
	            VALUES ('bacf9df4-b6be-4995-97cc-2c8e924c3eb3', 'admin', 'ADMIN')
            END");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE AspNetRoles WHERE Id = 'bacf9df4-b6be-4995-97cc-2c8e924c3eb3'");
        }
    }
}
