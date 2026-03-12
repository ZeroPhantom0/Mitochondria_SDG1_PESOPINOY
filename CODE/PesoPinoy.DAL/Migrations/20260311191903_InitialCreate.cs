using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PesoPinoy.DAL.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Borrowers",
                columns: table => new
                {
                    BorrowerId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FirstName = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    LastName = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    MiddleName = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    DateOfBirth = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ContactNumber = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    Email = table.Column<string>(type: "TEXT", nullable: false),
                    Address = table.Column<string>(type: "TEXT", nullable: false),
                    EmploymentStatus = table.Column<int>(type: "INTEGER", nullable: false),
                    MonthlyIncome = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    EmployerName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    GuarantorName = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    GuarantorContact = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    ReasonForLoan = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    RiskScore = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    RiskClassification = table.Column<int>(type: "INTEGER", nullable: false),
                    RegistrationDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Borrowers", x => x.BorrowerId);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Username = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    PasswordHash = table.Column<string>(type: "TEXT", nullable: false),
                    Salt = table.Column<string>(type: "TEXT", nullable: false),
                    FullName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "TEXT", nullable: false),
                    LastLoginDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "InsurancePolicies",
                columns: table => new
                {
                    InsurancePolicyId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PolicyNumber = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    BorrowerId = table.Column<int>(type: "INTEGER", nullable: false),
                    PolicyType = table.Column<string>(type: "TEXT", nullable: false),
                    CoverageAmount = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    PremiumAmount = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    StartDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EndDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    BeneficiaryName = table.Column<string>(type: "TEXT", nullable: true),
                    BeneficiaryRelation = table.Column<string>(type: "TEXT", nullable: true),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    StatusRemarks = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InsurancePolicies", x => x.InsurancePolicyId);
                    table.ForeignKey(
                        name: "FK_InsurancePolicies_Borrowers_BorrowerId",
                        column: x => x.BorrowerId,
                        principalTable: "Borrowers",
                        principalColumn: "BorrowerId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SavingsAccounts",
                columns: table => new
                {
                    SavingsAccountId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AccountNumber = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    BorrowerId = table.Column<int>(type: "INTEGER", nullable: false),
                    CurrentBalance = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    InterestRate = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    OpenedDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    LastTransactionDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SavingsAccounts", x => x.SavingsAccountId);
                    table.ForeignKey(
                        name: "FK_SavingsAccounts_Borrowers_BorrowerId",
                        column: x => x.BorrowerId,
                        principalTable: "Borrowers",
                        principalColumn: "BorrowerId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AuditLogs",
                columns: table => new
                {
                    AuditLogId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Timestamp = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UserName = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Action = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    EntityType = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    EntityId = table.Column<int>(type: "INTEGER", nullable: true),
                    OldValues = table.Column<string>(type: "TEXT", maxLength: 4000, nullable: true),
                    NewValues = table.Column<string>(type: "TEXT", maxLength: 4000, nullable: true),
                    Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    IpAddress = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    UserId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLogs", x => x.AuditLogId);
                    table.ForeignKey(
                        name: "FK_AuditLogs_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "InsuranceClaims",
                columns: table => new
                {
                    InsuranceClaimId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    InsurancePolicyId = table.Column<int>(type: "INTEGER", nullable: false),
                    ClaimNumber = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    FilingDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IncidentDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ClaimAmount = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    ClaimReason = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    Status = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    ProcessedDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ApprovedAmount = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: true),
                    Remarks = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    SupportingDocuments = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InsuranceClaims", x => x.InsuranceClaimId);
                    table.ForeignKey(
                        name: "FK_InsuranceClaims_InsurancePolicies_InsurancePolicyId",
                        column: x => x.InsurancePolicyId,
                        principalTable: "InsurancePolicies",
                        principalColumn: "InsurancePolicyId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InsurancePayments",
                columns: table => new
                {
                    InsurancePaymentId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    InsurancePolicyId = table.Column<int>(type: "INTEGER", nullable: false),
                    PaymentNumber = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    Amount = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    DueDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    PaymentDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    PaymentStatus = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    PaymentMethod = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    ReferenceNumber = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    Remarks = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InsurancePayments", x => x.InsurancePaymentId);
                    table.ForeignKey(
                        name: "FK_InsurancePayments_InsurancePolicies_InsurancePolicyId",
                        column: x => x.InsurancePolicyId,
                        principalTable: "InsurancePolicies",
                        principalColumn: "InsurancePolicyId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Loans",
                columns: table => new
                {
                    LoanId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    LoanNumber = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    BorrowerId = table.Column<int>(type: "INTEGER", nullable: false),
                    PrincipalAmount = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    InterestRate = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    TermInMonths = table.Column<int>(type: "INTEGER", nullable: false),
                    ApplicationDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ApprovalDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DisbursementDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    FirstPaymentDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    MaturityDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    MonthlyPayment = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    TotalInterest = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    TotalAmountPayable = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    BalanceRemaining = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    Purpose = table.Column<string>(type: "TEXT", nullable: true),
                    InsurancePolicyId = table.Column<int>(type: "INTEGER", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedBy = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Loans", x => x.LoanId);
                    table.ForeignKey(
                        name: "FK_Loans_Borrowers_BorrowerId",
                        column: x => x.BorrowerId,
                        principalTable: "Borrowers",
                        principalColumn: "BorrowerId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Loans_InsurancePolicies_InsurancePolicyId",
                        column: x => x.InsurancePolicyId,
                        principalTable: "InsurancePolicies",
                        principalColumn: "InsurancePolicyId",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "SavingsTransactions",
                columns: table => new
                {
                    SavingsTransactionId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SavingsAccountId = table.Column<int>(type: "INTEGER", nullable: false),
                    TransactionDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    TransactionType = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    Amount = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    BalanceBefore = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    BalanceAfter = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    ReferenceNumber = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    ProcessedBy = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SavingsTransactions", x => x.SavingsTransactionId);
                    table.ForeignKey(
                        name: "FK_SavingsTransactions_SavingsAccounts_SavingsAccountId",
                        column: x => x.SavingsAccountId,
                        principalTable: "SavingsAccounts",
                        principalColumn: "SavingsAccountId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AmortizationSchedules",
                columns: table => new
                {
                    AmortizationScheduleId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    LoanId = table.Column<int>(type: "INTEGER", nullable: false),
                    PaymentNumber = table.Column<int>(type: "INTEGER", nullable: false),
                    DueDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    PaidDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    BeginningBalance = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    PaymentAmount = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    PrincipalAmount = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    InterestAmount = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    EndingBalance = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    Remarks = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AmortizationSchedules", x => x.AmortizationScheduleId);
                    table.ForeignKey(
                        name: "FK_AmortizationSchedules_Loans_LoanId",
                        column: x => x.LoanId,
                        principalTable: "Loans",
                        principalColumn: "LoanId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    PaymentId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PaymentNumber = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    LoanId = table.Column<int>(type: "INTEGER", nullable: false),
                    PaymentDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DueDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    AmountDue = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    AmountPaid = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    LatePenalty = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    DaysLate = table.Column<int>(type: "INTEGER", nullable: false),
                    PrincipalPaid = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    InterestPaid = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    PaymentMethod = table.Column<string>(type: "TEXT", nullable: true),
                    ReferenceNumber = table.Column<string>(type: "TEXT", nullable: true),
                    Remarks = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedBy = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.PaymentId);
                    table.ForeignKey(
                        name: "FK_Payments_Loans_LoanId",
                        column: x => x.LoanId,
                        principalTable: "Loans",
                        principalColumn: "LoanId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AmortizationSchedules_LoanId",
                table: "AmortizationSchedules",
                column: "LoanId");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_UserId",
                table: "AuditLogs",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Borrowers_Email",
                table: "Borrowers",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InsuranceClaims_InsurancePolicyId",
                table: "InsuranceClaims",
                column: "InsurancePolicyId");

            migrationBuilder.CreateIndex(
                name: "IX_InsurancePayments_InsurancePolicyId",
                table: "InsurancePayments",
                column: "InsurancePolicyId");

            migrationBuilder.CreateIndex(
                name: "IX_InsurancePolicies_BorrowerId",
                table: "InsurancePolicies",
                column: "BorrowerId");

            migrationBuilder.CreateIndex(
                name: "IX_InsurancePolicies_PolicyNumber",
                table: "InsurancePolicies",
                column: "PolicyNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Loans_BorrowerId",
                table: "Loans",
                column: "BorrowerId");

            migrationBuilder.CreateIndex(
                name: "IX_Loans_InsurancePolicyId",
                table: "Loans",
                column: "InsurancePolicyId");

            migrationBuilder.CreateIndex(
                name: "IX_Loans_LoanNumber",
                table: "Loans",
                column: "LoanNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Payments_LoanId",
                table: "Payments",
                column: "LoanId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_PaymentNumber",
                table: "Payments",
                column: "PaymentNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SavingsAccounts_AccountNumber",
                table: "SavingsAccounts",
                column: "AccountNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SavingsAccounts_BorrowerId",
                table: "SavingsAccounts",
                column: "BorrowerId");

            migrationBuilder.CreateIndex(
                name: "IX_SavingsTransactions_SavingsAccountId",
                table: "SavingsTransactions",
                column: "SavingsAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AmortizationSchedules");

            migrationBuilder.DropTable(
                name: "AuditLogs");

            migrationBuilder.DropTable(
                name: "InsuranceClaims");

            migrationBuilder.DropTable(
                name: "InsurancePayments");

            migrationBuilder.DropTable(
                name: "Payments");

            migrationBuilder.DropTable(
                name: "SavingsTransactions");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Loans");

            migrationBuilder.DropTable(
                name: "SavingsAccounts");

            migrationBuilder.DropTable(
                name: "InsurancePolicies");

            migrationBuilder.DropTable(
                name: "Borrowers");
        }
    }
}
