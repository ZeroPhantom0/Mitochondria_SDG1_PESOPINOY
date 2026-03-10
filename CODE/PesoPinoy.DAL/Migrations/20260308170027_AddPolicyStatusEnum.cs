using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PesoPinoy.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddPolicyStatusEnum : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "InsurancePolicies",
                newName: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Status",
                table: "InsurancePolicies",
                newName: "IsActive");
        }
    }
}
