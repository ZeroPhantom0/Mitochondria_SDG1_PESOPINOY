using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PesoPinoy.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddInsurancePaymentAndStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "StatusRemarks",
                table: "InsurancePolicies",
                type: "TEXT",
                maxLength: 500,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StatusRemarks",
                table: "InsurancePolicies");
        }
    }
}
