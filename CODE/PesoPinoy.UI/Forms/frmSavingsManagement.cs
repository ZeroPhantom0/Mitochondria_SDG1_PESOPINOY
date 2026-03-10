using System;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Globalization;
using Microsoft.Extensions.DependencyInjection;
using PesoPinoy.BLL.Services;
using PesoPinoy.Models.Entities;
        
namespace PesoPinoy.UI.Forms
{
    public partial class frmSavingsManagement : Form
    {
        private readonly SavingsService _savingsService = null!;
        private readonly BorrowerService _borrowerService = null!;
        private Panel headerPanel = null!;
        private Panel contentPanel = null!;
        private ComboBox cmbBorrower = null!;
        private TextBox txtAccountNumber = null!, txtBalance = null!, txtAmount = null!;
        private Label lblAccountInfo = null!;
        private Button btnCreateAccount = null!, btnDeposit = null!, btnWithdraw = null!, btnRefresh = null!;
        private DataGridView dgvAccounts = null!, dgvTransactions = null!;
        private Label lblStatus = null!;
        private int selectedAccountId = 0;

        public frmSavingsManagement()
        {
            InitializeComponent();
            _savingsService = Program.ServiceProvider!.GetService<SavingsService>()!;
            _borrowerService = Program.ServiceProvider!.GetService<BorrowerService>()!;
            this.BackColor = Color.White;
            this.Text = "PesoPinoy - Savings Management";
            this.Size = new Size(1200, 700);
            this.StartPosition = FormStartPosition.CenterScreen;

            CreateHeader();
            CreateAccountForm();
            CreateAccountsGrid();
            CreateTransactionsGrid();

            this.Load += async (s, e) =>
            {
                await LoadBorrowersAsync();
                await LoadAccountsAsync();
            };
        }

        private void CreateHeader()
        {
            headerPanel = new Panel
            {
                Height = 60,
                Dock = DockStyle.Top,
                BackColor = Color.FromArgb(2, 58, 128)
            };

            Label lblTitle = new Label
            {
                Text = "Savings Management",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(20, 15),
                AutoSize = true
            };

            Button btnClose = new Button
            {
                Text = "X",
                Size = new Size(40, 40),
                Location = new Point(this.Width - 50, 10),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(2, 58, 128),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 12, FontStyle.Bold)
            };
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.Click += (s, e) => this.Close();

            headerPanel.Controls.Add(lblTitle);
            headerPanel.Controls.Add(btnClose);
            this.Controls.Add(headerPanel);
        }

        private void CreateAccountForm()
        {
            Panel formPanel = new Panel
            {
                Location = new Point(20, 80),
                Size = new Size(550, 200),
                BackColor = Color.FromArgb(240, 240, 240),
                BorderStyle = BorderStyle.FixedSingle
            };

            Label lblBorrower = new Label
            {
                Text = "Select Borrower:",
                Location = new Point(20, 20),
                Size = new Size(120, 25),
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.FromArgb(2, 58, 128)
            };

            cmbBorrower = new ComboBox
            {
                Location = new Point(150, 20),
                Size = new Size(350, 25),
                Font = new Font("Segoe UI", 10),
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            btnCreateAccount = new Button
            {
                Text = "CREATE ACCOUNT",
                Location = new Point(150, 60),
                Size = new Size(150, 35),
                BackColor = Color.FromArgb(2, 58, 128),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            btnCreateAccount.FlatAppearance.BorderSize = 0;
            btnCreateAccount.Click += async (s, e) => await CreateSavingsAccountAsync();

            lblAccountInfo = new Label
            {
                Location = new Point(20, 110),
                Size = new Size(500, 25),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(2, 58, 128)
            };

            Label lblAmount = new Label
            {
                Text = "Amount:",
                Location = new Point(20, 150),
                Size = new Size(60, 25),
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.FromArgb(2, 58, 128)
            };

            txtAmount = new TextBox
            {
                Location = new Point(90, 150),
                Size = new Size(120, 25),
                Font = new Font("Segoe UI", 10),
                BorderStyle = BorderStyle.FixedSingle
            };

            btnDeposit = new Button
            {
                Text = "DEPOSIT",
                Location = new Point(220, 150),
                Size = new Size(90, 30),
                BackColor = Color.FromArgb(46, 204, 113),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };
            btnDeposit.FlatAppearance.BorderSize = 0;
            btnDeposit.Click += async (s, e) => await DepositAsync();

            btnWithdraw = new Button
            {
                Text = "WITHDRAW",
                Location = new Point(320, 150),
                Size = new Size(90, 30),
                BackColor = Color.FromArgb(52, 152, 219),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };
            btnWithdraw.FlatAppearance.BorderSize = 0;
            btnWithdraw.Click += async (s, e) => await WithdrawAsync();

            formPanel.Controls.AddRange(new Control[] {
                lblBorrower, cmbBorrower, btnCreateAccount, lblAccountInfo,
                lblAmount, txtAmount, btnDeposit, btnWithdraw
            });

            this.Controls.Add(formPanel);
        }

        private void CreateAccountsGrid()
        {
            dgvAccounts = new DataGridView
            {
                Location = new Point(20, 300),
                Size = new Size(550, 150),
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                RowHeadersVisible = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                ReadOnly = true,
                AllowUserToAddRows = false
            };

            dgvAccounts.Columns.Add("AccountId", "ID");
            dgvAccounts.Columns.Add("AccountNumber", "Account #");
            dgvAccounts.Columns.Add("Borrower", "Borrower");
            dgvAccounts.Columns.Add("Balance", "Balance");
            dgvAccounts.Columns.Add("Status", "Status");

            dgvAccounts.Columns["Balance"].DefaultCellStyle.Format = "C2";
            dgvAccounts.Columns["Balance"].DefaultCellStyle.FormatProvider = new CultureInfo("en-PH");
            dgvAccounts.Columns["Balance"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            dgvAccounts.CellClick += DgvAccounts_CellClick;

            this.Controls.Add(dgvAccounts);
        }

        private void CreateTransactionsGrid()
        {
            dgvTransactions = new DataGridView
            {
                Location = new Point(590, 80),
                Size = new Size(580, 570),
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                RowHeadersVisible = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false
            };

            dgvTransactions.Columns.Add("Date", "Date");
            dgvTransactions.Columns.Add("Type", "Type");
            dgvTransactions.Columns.Add("Amount", "Amount");
            dgvTransactions.Columns.Add("Balance", "Balance");
            dgvTransactions.Columns.Add("Reference", "Reference");
            dgvTransactions.Columns.Add("Description", "Description");

            dgvTransactions.Columns["Amount"].DefaultCellStyle.Format = "C2";
            dgvTransactions.Columns["Amount"].DefaultCellStyle.FormatProvider = new CultureInfo("en-PH");
            dgvTransactions.Columns["Balance"].DefaultCellStyle.Format = "C2";
            dgvTransactions.Columns["Balance"].DefaultCellStyle.FormatProvider = new CultureInfo("en-PH");
            dgvTransactions.Columns["Amount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvTransactions.Columns["Balance"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            this.Controls.Add(dgvTransactions);

            // Create a panel for the bottom controls to ensure proper layout
            Panel bottomPanel = new Panel
            {
                Location = new Point(20, 460),
                Size = new Size(550, 40),
                BackColor = Color.Transparent
            };

            lblStatus = new Label
            {
                Location = new Point(0, 10),
                Size = new Size(350, 20),
                ForeColor = Color.FromArgb(2, 58, 128),
                Font = new Font("Segoe UI", 9),
                TextAlign = ContentAlignment.MiddleLeft
            };

            btnRefresh = new Button
            {
                Text = "REFRESH",
                Location = new Point(430, 5),
                Size = new Size(100, 30),
                BackColor = Color.FromArgb(46, 204, 113),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Anchor = AnchorStyles.Right
            };
            btnRefresh.FlatAppearance.BorderSize = 0;
            btnRefresh.Click += async (s, e) => await LoadAccountsAsync();

            bottomPanel.Controls.Add(lblStatus);
            bottomPanel.Controls.Add(btnRefresh);
            this.Controls.Add(bottomPanel);
        }

        private async Task LoadBorrowersAsync()
        {
            try
            {
                var borrowers = await _borrowerService.GetAllBorrowersAsync();
                cmbBorrower.DisplayMember = "FullName";
                cmbBorrower.ValueMember = "BorrowerId";
                cmbBorrower.DataSource = borrowers.Select(b => new
                {
                    BorrowerId = b.BorrowerId,
                    FullName = $"{b.LastName}, {b.FirstName}"
                }).ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading borrowers: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task LoadAccountsAsync()
        {
            try
            {
                var accounts = await _savingsService.GetAllSavingsAccountsAsync();
                dgvAccounts.Rows.Clear();

                foreach (var account in accounts)
                {
                    dgvAccounts.Rows.Add(
                        account.SavingsAccountId,
                        account.AccountNumber,
                        $"{account.Borrower?.LastName}, {account.Borrower?.FirstName}",
                        account.CurrentBalance,
                        account.IsActive ? "Active" : "Inactive"
                    );
                }

                lblStatus.Text = $"Loaded {accounts.Count()} savings accounts.";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading accounts: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void DgvAccounts_CellClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && dgvAccounts.Rows[e.RowIndex].Cells["AccountId"].Value != null)
            {
                DataGridViewRow row = dgvAccounts.Rows[e.RowIndex];
                selectedAccountId = Convert.ToInt32(row.Cells["AccountId"].Value);

                await LoadAccountDetailsAsync(selectedAccountId);
            }
        }

        private async Task LoadAccountDetailsAsync(int accountId)
        {
            try
            {
                var account = await _savingsService.GetSavingsAccountByIdAsync(accountId);
                if (account != null)
                {
                    lblAccountInfo.Text = $"Account: {account.AccountNumber} | Balance: {account.CurrentBalance.ToString("C2", new CultureInfo("en-PH"))} | Last Transaction: {account.LastTransactionDate:MM/dd/yyyy}";

                    dgvTransactions.Rows.Clear();
                    foreach (var trans in account.Transactions.OrderByDescending(t => t.TransactionDate))
                    {
                        dgvTransactions.Rows.Add(
                            trans.TransactionDate.ToString("MM/dd/yyyy HH:mm"),
                            trans.TransactionType,
                            trans.Amount,
                            trans.BalanceAfter,
                            trans.ReferenceNumber,
                            trans.Description
                        );
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading account details: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task CreateSavingsAccountAsync()
        {
            try
            {
                if (cmbBorrower.SelectedValue == null)
                {
                    MessageBox.Show("Please select a borrower.", "Validation Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                int borrowerId = (int)cmbBorrower.SelectedValue;
                var account = await _savingsService.CreateSavingsAccountAsync(borrowerId);

                MessageBox.Show($"Savings account created successfully!\nAccount Number: {account.AccountNumber}",
                    "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                await LoadAccountsAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error creating account: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task DepositAsync()
        {
            try
            {
                if (selectedAccountId == 0)
                {
                    MessageBox.Show("Please select a savings account.", "Validation Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!decimal.TryParse(txtAmount.Text, out decimal amount) || amount <= 0)
                {
                    MessageBox.Show("Please enter a valid amount.", "Validation Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var transaction = await _savingsService.DepositAsync(selectedAccountId, amount, "Over-the-counter deposit");

                MessageBox.Show($"Deposit successful!\nAmount: {amount.ToString("C2", new CultureInfo("en-PH"))}\nReference: {transaction.ReferenceNumber}",
                    "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                txtAmount.Clear();
                await LoadAccountDetailsAsync(selectedAccountId);
                await LoadAccountsAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error processing deposit: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task WithdrawAsync()
        {
            try
            {
                if (selectedAccountId == 0)
                {
                    MessageBox.Show("Please select a savings account.", "Validation Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!decimal.TryParse(txtAmount.Text, out decimal amount) || amount <= 0)
                {
                    MessageBox.Show("Please enter a valid amount.", "Validation Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var transaction = await _savingsService.WithdrawAsync(selectedAccountId, amount, "Over-the-counter withdrawal");

                MessageBox.Show($"Withdrawal successful!\nAmount: {amount.ToString("C2", new CultureInfo("en-PH"))}\nReference: {transaction.ReferenceNumber}",
                    "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                txtAmount.Clear();
                await LoadAccountDetailsAsync(selectedAccountId);
                await LoadAccountsAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error processing withdrawal: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}