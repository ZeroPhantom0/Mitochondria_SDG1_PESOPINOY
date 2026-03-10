using System;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Globalization;
using Microsoft.Extensions.DependencyInjection;
using PesoPinoy.BLL.Services;
using PesoPinoy.Models.Entities;
using PesoPinoy.Models.Enums;

namespace PesoPinoy.UI.Forms
{
    public partial class frmBorrowerManagement : Form
    {
        private readonly BorrowerService _borrowerService;

        // Main containers
        private Panel headerPanel;
        private Panel leftPanel;
        private Panel rightPanel;

        // Form fields
        private TextBox txtFirstName, txtLastName, txtMiddleName;
        private TextBox txtContact, txtEmail, txtAddress;
        private TextBox txtIncome, txtEmployer, txtGuarantor;
        private TextBox txtGuarantorContact, txtReason;
        private ComboBox cmbEmploymentStatus;
        private DateTimePicker dtpBirthDate;

        // Buttons
        private Button btnSave, btnUpdate, btnDelete;
        private Button btnClear, btnRefresh;

        // Grid
        private DataGridView dgvBorrowers;

        // Status
        private Label lblStatus;
        private int selectedBorrowerId = 0;

        // View Profile Form
        private Form viewProfileForm;

        public frmBorrowerManagement()
        {
            InitializeComponent();
            _borrowerService = Program.ServiceProvider!.GetService<BorrowerService>()!;

            // Form settings
            this.Text = "PesoPinoy - Borrower Management";
            this.WindowState = FormWindowState.Maximized;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(245, 245, 245);
            this.DoubleBuffered = true;

            CreateHeader();
            CreateMainLayout();
            this.Load += async (s, e) => await LoadBorrowersAsync();
        }

        private void CreateHeader()
        {
            headerPanel = new Panel
            {
                Height = 70,
                Dock = DockStyle.Top,
                BackColor = Color.FromArgb(2, 58, 128)
            };

            Label lblTitle = new Label
            {
                Text = "Borrower Management",
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(30, 18),
                AutoSize = true
            };

            Label lblDate = new Label
            {
                Text = DateTime.Now.ToString("MMMM dd, yyyy"),
                Font = new Font("Segoe UI", 11),
                ForeColor = Color.FromArgb(200, 200, 200),
                Location = new Point(this.Width - 200, 25),
                AutoSize = true,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };

            Button btnClose = new Button
            {
                Text = "×",
                Size = new Size(45, 45),
                Location = new Point(this.Width - 70, 12),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(2, 58, 128),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Cursor = Cursors.Hand
            };
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.Click += (s, e) => this.Close();

            headerPanel.Controls.Add(lblTitle);
            headerPanel.Controls.Add(lblDate);
            headerPanel.Controls.Add(btnClose);
            this.Controls.Add(headerPanel);
        }

        private void CreateMainLayout()
        {
            // Main container with padding
            Panel container = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(20),
                BackColor = Color.FromArgb(245, 245, 245)
            };

            // Left Panel - Borrower Form
            leftPanel = new Panel
            {
                Location = new Point(20, 90),
                Size = new Size(600, 550),
                BackColor = Color.White,
                Padding = new Padding(20)
            };
            CreateLeftPanelContent();

            // Right Panel - Borrower List Grid
            rightPanel = new Panel
            {
                Location = new Point(640, 90),
                Size = new Size(620, 550),
                BackColor = Color.White,
                Padding = new Padding(20)
            };
            CreateRightPanelContent();

            // Status label
            lblStatus = new Label
            {
                Dock = DockStyle.Bottom,
                Height = 35,
                BackColor = Color.FromArgb(240, 240, 240),
                ForeColor = Color.FromArgb(2, 58, 128),
                Font = new Font("Segoe UI", 10),
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(25, 0, 0, 0),
                Text = "Ready"
            };

            container.Controls.Add(leftPanel);
            container.Controls.Add(rightPanel);
            this.Controls.Add(container);
            this.Controls.Add(lblStatus);
        }

        private void CreateLeftPanelContent()
        {
            int currentY = 10;
            int labelWidth = 100;
            int rowHeight = 35;
            int colSpacing = 320;

            // Title
            Label lblFormTitle = new Label
            {
                Text = "Borrower Details",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(2, 58, 128),
                Location = new Point(0, currentY),
                AutoSize = true
            };
            leftPanel.Controls.Add(lblFormTitle);

            Panel titleSeparator = new Panel
            {
                Height = 2,
                Width = leftPanel.Width - 40,
                Location = new Point(0, currentY + 35),
                BackColor = Color.FromArgb(230, 230, 230)
            };
            leftPanel.Controls.Add(titleSeparator);
            currentY += 55;

            // PERSONAL INFORMATION
            Label lblPersonalTitle = new Label
            {
                Text = "PERSONAL INFORMATION",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(2, 58, 128),
                Location = new Point(0, currentY),
                AutoSize = true
            };
            leftPanel.Controls.Add(lblPersonalTitle);
            currentY += 25;

            // Row 1: First Name and Last Name
            CreateLabelAndTextBox("First Name:", ref txtFirstName, 0, currentY, labelWidth, 180);
            CreateLabelAndTextBox("Last Name:", ref txtLastName, colSpacing, currentY, labelWidth, 180);
            currentY += rowHeight;

            // Row 2: Middle Name and Birth Date
            CreateLabelAndTextBox("Middle Name:", ref txtMiddleName, 0, currentY, labelWidth, 180);
            CreateLabelAndDateTime("Birth Date:", ref dtpBirthDate, colSpacing, currentY, labelWidth, 180);
            currentY += rowHeight + 10;

            // CONTACT INFORMATION
            Label lblContactTitle = new Label
            {
                Text = "CONTACT INFORMATION",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(2, 58, 128),
                Location = new Point(0, currentY),
                AutoSize = true
            };
            leftPanel.Controls.Add(lblContactTitle);
            currentY += 25;

            // Row 3: Contact and Email
            CreateLabelAndTextBox("Contact:", ref txtContact, 0, currentY, labelWidth, 180);
            CreateLabelAndTextBox("Email:", ref txtEmail, colSpacing, currentY, labelWidth, 180);
            currentY += rowHeight;

            // Row 4: Address (Full Width)
            CreateLabelAndTextBox("Address:", ref txtAddress, 0, currentY, labelWidth, 440);
            currentY += rowHeight + 10;

            // EMPLOYMENT FINANCIAL
            Label lblEmploymentTitle = new Label
            {
                Text = "EMPLOYMENT FINANCIAL",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(2, 58, 128),
                Location = new Point(0, currentY),
                AutoSize = true
            };
            leftPanel.Controls.Add(lblEmploymentTitle);
            currentY += 25;

            // Row 5: Employment Status
            CreateLabelAndComboBox("Employment:", ref cmbEmploymentStatus, 0, currentY, labelWidth, 180);
            cmbEmploymentStatus.Items.AddRange(Enum.GetNames(typeof(EmploymentStatus)));
            currentY += rowHeight;

            // Row 6: Monthly Income and Employer
            CreateLabelAndTextBox("Monthly Income:", ref txtIncome, 0, currentY, labelWidth, 180);
            CreateLabelAndTextBox("Employer:", ref txtEmployer, colSpacing, currentY, labelWidth, 180);
            currentY += rowHeight + 10;

            // GUARANTOR INFORMATION
            Label lblGuarantorTitle = new Label
            {
                Text = "GUARANTOR INFORMATION",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(2, 58, 128),
                Location = new Point(0, currentY),
                AutoSize = true
            };
            leftPanel.Controls.Add(lblGuarantorTitle);
            currentY += 25;

            // Row 7: Guarantor and Guarantor Contact
            CreateLabelAndTextBox("Guarantor:", ref txtGuarantor, 0, currentY, labelWidth, 180);
            CreateLabelAndTextBox("Guarantor #:", ref txtGuarantorContact, colSpacing, currentY, labelWidth, 180);
            currentY += rowHeight;

            // Row 8: Loan Reason (Full Width)
            CreateLabelAndTextBox("Loan Reason:", ref txtReason, 0, currentY, labelWidth, 440);
            currentY += 50;

            // BUTTON PANEL
            Panel buttonPanel = new Panel
            {
                Location = new Point(0, currentY),
                Size = new Size(leftPanel.Width - 40, 60),
                BackColor = Color.Transparent
            };

            int buttonWidth = 90;
            int buttonSpacing = 10;
            int currentX = 0;

            btnSave = CreateButton("SAVE", currentX, 10, Color.FromArgb(2, 58, 128));
            btnSave.Click += async (s, e) => await SaveBorrowerAsync();

            currentX += buttonWidth + buttonSpacing;
            btnUpdate = CreateButton("UPDATE", currentX, 10, Color.FromArgb(52, 152, 219));
            btnUpdate.Click += async (s, e) => await UpdateBorrowerAsync();
            btnUpdate.Enabled = false;

            currentX += buttonWidth + buttonSpacing;
            btnDelete = CreateButton("DELETE", currentX, 10, Color.FromArgb(231, 76, 60));
            btnDelete.Click += async (s, e) => await DeleteBorrowerAsync();
            btnDelete.Enabled = false;

            currentX += buttonWidth + buttonSpacing;
            btnClear = CreateButton("CLEAR", currentX, 10, Color.FromArgb(155, 89, 182));
            btnClear.Click += (s, e) => ClearForm();

            currentX += buttonWidth + buttonSpacing;
            btnRefresh = CreateButton("REFRESH", currentX, 10, Color.FromArgb(46, 204, 113));
            btnRefresh.Click += async (s, e) => await LoadBorrowersAsync();

            buttonPanel.Controls.AddRange(new Control[] { btnSave, btnUpdate, btnDelete, btnClear, btnRefresh });
            leftPanel.Controls.Add(buttonPanel);
        }

        private void CreateLabelAndTextBox(string labelText, ref TextBox textBox, int x, int y, int labelWidth, int fieldWidth)
        {
            Label label = new Label
            {
                Text = labelText,
                Location = new Point(x, y + 5),
                Size = new Size(labelWidth, 25),
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.FromArgb(80, 80, 80),
                TextAlign = ContentAlignment.MiddleRight
            };

            textBox = new TextBox
            {
                Location = new Point(x + labelWidth, y),
                Size = new Size(fieldWidth, 30),
                Font = new Font("Segoe UI", 9),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White
            };

            leftPanel.Controls.Add(label);
            leftPanel.Controls.Add(textBox);
        }

        private void CreateLabelAndDateTime(string labelText, ref DateTimePicker dtp, int x, int y, int labelWidth, int fieldWidth)
        {
            Label label = new Label
            {
                Text = labelText,
                Location = new Point(x, y + 5),
                Size = new Size(labelWidth, 25),
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.FromArgb(80, 80, 80),
                TextAlign = ContentAlignment.MiddleRight
            };

            dtp = new DateTimePicker
            {
                Location = new Point(x + labelWidth, y),
                Size = new Size(fieldWidth, 30),
                Font = new Font("Segoe UI", 9),
                Format = DateTimePickerFormat.Short
            };

            leftPanel.Controls.Add(label);
            leftPanel.Controls.Add(dtp);
        }

        private void CreateLabelAndComboBox(string labelText, ref ComboBox cmb, int x, int y, int labelWidth, int fieldWidth)
        {
            Label label = new Label
            {
                Text = labelText,
                Location = new Point(x, y + 5),
                Size = new Size(labelWidth, 25),
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.FromArgb(80, 80, 80),
                TextAlign = ContentAlignment.MiddleRight
            };

            cmb = new ComboBox
            {
                Location = new Point(x + labelWidth, y),
                Size = new Size(fieldWidth, 30),
                Font = new Font("Segoe UI", 9),
                DropDownStyle = ComboBoxStyle.DropDownList,
                FlatStyle = FlatStyle.Flat
            };

            leftPanel.Controls.Add(label);
            leftPanel.Controls.Add(cmb);
        }

        private Button CreateButton(string text, int x, int y, Color color)
        {
            Button btn = new Button
            {
                Text = text,
                Location = new Point(x, y),
                Size = new Size(90, 35),
                BackColor = color,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Cursor = Cursors.Hand,
                TextAlign = ContentAlignment.MiddleCenter
            };
            btn.FlatAppearance.BorderSize = 0;

            // Hover effect
            btn.MouseEnter += (s, e) => btn.BackColor = ControlPaint.Light(color, 0.2f);
            btn.MouseLeave += (s, e) => btn.BackColor = color;

            return btn;
        }

        private void CreateRightPanelContent()
        {
            // Title
            Label lblBorrowerList = new Label
            {
                Text = "Borrower List",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(2, 58, 128),
                Location = new Point(0, 10),
                AutoSize = true
            };
            rightPanel.Controls.Add(lblBorrowerList);

            Panel separator = new Panel
            {
                Height = 2,
                Width = rightPanel.Width - 40,
                Location = new Point(0, 45),
                BackColor = Color.FromArgb(230, 230, 230)
            };
            rightPanel.Controls.Add(separator);

            // Grid
            dgvBorrowers = new DataGridView
            {
                Location = new Point(0, 60),
                Size = new Size(rightPanel.Width - 40, rightPanel.Height - 80),
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                RowHeadersVisible = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                ReadOnly = true,
                AllowUserToAddRows = false,
                RowTemplate = { Height = 35 }
            };

            // Add columns
            dgvBorrowers.Columns.Add("BorrowerId", "ID");
            dgvBorrowers.Columns.Add("FullName", "Full Name");
            dgvBorrowers.Columns.Add("Contact", "Contact");
            dgvBorrowers.Columns.Add("Email", "Email");
            dgvBorrowers.Columns.Add("Income", "Monthly Income");
            dgvBorrowers.Columns.Add("RiskClass", "Risk Class");

            // Column styling
            dgvBorrowers.Columns["BorrowerId"].Width = 40;
            dgvBorrowers.Columns["FullName"].Width = 150;
            dgvBorrowers.Columns["Income"].DefaultCellStyle.Format = "C2";
            dgvBorrowers.Columns["Income"].DefaultCellStyle.FormatProvider = new CultureInfo("en-PH");

            // Header styling
            dgvBorrowers.EnableHeadersVisualStyles = false;
            dgvBorrowers.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(2, 58, 128);
            dgvBorrowers.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvBorrowers.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgvBorrowers.ColumnHeadersHeight = 40;

            // Cell styling
            dgvBorrowers.DefaultCellStyle.Font = new Font("Segoe UI", 9);
            dgvBorrowers.DefaultCellStyle.SelectionBackColor = Color.FromArgb(230, 242, 255);
            dgvBorrowers.DefaultCellStyle.SelectionForeColor = Color.FromArgb(2, 58, 128);
            dgvBorrowers.DefaultCellStyle.Padding = new Padding(5);

            // Add alternating row color
            dgvBorrowers.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 249, 250);

            dgvBorrowers.CellMouseDoubleClick += DgvBorrowers_CellMouseDoubleClick;
            dgvBorrowers.CellClick += DgvBorrowers_CellClick;

            rightPanel.Controls.Add(dgvBorrowers);
        }

        private void CreateViewProfileForm(Borrower borrower)
        {
            viewProfileForm = new Form
            {
                Text = "Borrower Profile",
                Size = new Size(550, 650),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false,
                BackColor = Color.White
            };

            int yPos = 30;
            int labelWidth = 150;
            int valueWidth = 320;

            // Profile Header
            Label lblProfileTitle = new Label
            {
                Text = "Borrower Information",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(2, 58, 128),
                Location = new Point(20, 20),
                AutoSize = true
            };

            Panel separator = new Panel
            {
                Height = 2,
                Width = 490,
                Location = new Point(20, 55),
                BackColor = Color.FromArgb(230, 230, 230)
            };

            viewProfileForm.Controls.Add(lblProfileTitle);
            viewProfileForm.Controls.Add(separator);

            // Full Name
            yPos = 80;
            AddProfileField("Full Name:", $"{borrower.LastName}, {borrower.FirstName} {borrower.MiddleName}", ref yPos, labelWidth, valueWidth);

            // Personal Details
            AddProfileField("Birth Date:", borrower.DateOfBirth.ToString("MMMM dd, yyyy"), ref yPos, labelWidth, valueWidth);
            AddProfileField("Age:", CalculateAge(borrower.DateOfBirth).ToString(), ref yPos, labelWidth, valueWidth);

            // Contact Information
            yPos += 10;
            Label lblContactTitle = new Label
            {
                Text = "Contact Information",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(2, 58, 128),
                Location = new Point(20, yPos),
                AutoSize = true
            };
            viewProfileForm.Controls.Add(lblContactTitle);
            yPos += 25;

            AddProfileField("Contact:", borrower.ContactNumber, ref yPos, labelWidth, valueWidth);
            AddProfileField("Email:", borrower.Email, ref yPos, labelWidth, valueWidth);
            AddProfileField("Address:", borrower.Address, ref yPos, labelWidth, valueWidth);

            // Employment & Financial
            yPos += 10;
            Label lblFinancialTitle = new Label
            {
                Text = "Employment & Financial",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(2, 58, 128),
                Location = new Point(20, yPos),
                AutoSize = true
            };
            viewProfileForm.Controls.Add(lblFinancialTitle);
            yPos += 25;

            AddProfileField("Employment:", borrower.EmploymentStatus.ToString(), ref yPos, labelWidth, valueWidth);
            AddProfileField("Monthly Income:", borrower.MonthlyIncome.ToString("C2", new CultureInfo("en-PH")), ref yPos, labelWidth, valueWidth);
            AddProfileField("Employer:", borrower.EmployerName ?? "N/A", ref yPos, labelWidth, valueWidth);

            // Guarantor
            yPos += 10;
            Label lblGuarantorTitle = new Label
            {
                Text = "Guarantor Information",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(2, 58, 128),
                Location = new Point(20, yPos),
                AutoSize = true
            };
            viewProfileForm.Controls.Add(lblGuarantorTitle);
            yPos += 25;

            AddProfileField("Guarantor:", borrower.GuarantorName ?? "N/A", ref yPos, labelWidth, valueWidth);
            AddProfileField("Guarantor Contact:", borrower.GuarantorContact ?? "N/A", ref yPos, labelWidth, valueWidth);

            // Loan Reason
            if (!string.IsNullOrEmpty(borrower.ReasonForLoan))
            {
                yPos += 10;
                AddProfileField("Loan Reason:", borrower.ReasonForLoan, ref yPos, labelWidth, valueWidth);
            }

            // Risk Classification
            yPos += 10;
            Panel riskPanel = new Panel
            {
                Location = new Point(20, yPos + 10),
                Size = new Size(490, 45),
                BackColor = GetRiskColor(borrower.RiskClassification)
            };

            Label lblRisk = new Label
            {
                Text = $"Risk Classification: {borrower.RiskClassification}",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(15, 12),
                AutoSize = true
            };

            riskPanel.Controls.Add(lblRisk);
            viewProfileForm.Controls.Add(riskPanel);

            // Close Button
            Button btnCloseProfile = new Button
            {
                Text = "Close",
                Size = new Size(120, 40),
                Location = new Point(205, yPos + 80),
                BackColor = Color.FromArgb(2, 58, 128),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnCloseProfile.FlatAppearance.BorderSize = 0;
            btnCloseProfile.Click += (s, e) => viewProfileForm.Close();

            viewProfileForm.Controls.Add(btnCloseProfile);
        }

        private void AddProfileField(string label, string value, ref int yPos, int labelWidth, int valueWidth)
        {
            Label lblLabel = new Label
            {
                Text = label,
                Location = new Point(20, yPos),
                Size = new Size(labelWidth, 25),
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.FromArgb(80, 80, 80),
                TextAlign = ContentAlignment.MiddleRight
            };

            Label lblValue = new Label
            {
                Text = value,
                Location = new Point(20 + labelWidth, yPos),
                Size = new Size(valueWidth, 25),
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.FromArgb(50, 50, 50),
                TextAlign = ContentAlignment.MiddleLeft
            };

            viewProfileForm.Controls.Add(lblLabel);
            viewProfileForm.Controls.Add(lblValue);

            yPos += 30;
        }

        private Color GetRiskColor(RiskClassification risk)
        {
            return risk switch
            {
                RiskClassification.Low => Color.FromArgb(46, 204, 113),
                RiskClassification.Medium => Color.FromArgb(241, 196, 15),
                RiskClassification.High => Color.FromArgb(231, 76, 60),
                _ => Color.FromArgb(52, 152, 219)
            };
        }

        private int CalculateAge(DateTime birthDate)
        {
            DateTime today = DateTime.Today;
            int age = today.Year - birthDate.Year;
            if (birthDate.Date > today.AddYears(-age)) age--;
            return age;
        }

        private async Task LoadBorrowersAsync()
        {
            try
            {
                var borrowers = await _borrowerService.GetAllBorrowersAsync();

                this.Invoke(new Action(() =>
                {
                    dgvBorrowers.Rows.Clear();
                    foreach (var borrower in borrowers)
                    {
                        dgvBorrowers.Rows.Add(
                            borrower.BorrowerId,
                            $"{borrower.LastName}, {borrower.FirstName}",
                            borrower.ContactNumber,
                            borrower.Email,
                            borrower.MonthlyIncome,
                            borrower.RiskClassification.ToString()
                        );
                    }
                    lblStatus.Text = $"✓ Loaded {borrowers.Count()} borrowers successfully.";
                }));
            }
            catch (Exception ex)
            {
                this.Invoke(new Action(() =>
                {
                    MessageBox.Show($"Error loading borrowers: {ex.Message}", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }));
            }
        }

        private void DgvBorrowers_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && dgvBorrowers.Rows[e.RowIndex].Cells["BorrowerId"].Value != null)
            {
                DataGridViewRow row = dgvBorrowers.Rows[e.RowIndex];
                selectedBorrowerId = Convert.ToInt32(row.Cells["BorrowerId"].Value);
                LoadBorrowerToForm(selectedBorrowerId);

                btnSave.Enabled = false;
                btnUpdate.Enabled = true;
                btnDelete.Enabled = true;
            }
        }

        private void DgvBorrowers_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex >= 0 && dgvBorrowers.Rows[e.RowIndex].Cells["BorrowerId"].Value != null)
            {
                int borrowerId = Convert.ToInt32(dgvBorrowers.Rows[e.RowIndex].Cells["BorrowerId"].Value);
                ShowBorrowerProfile(borrowerId);
            }
        }

        private async void ShowBorrowerProfile(int borrowerId)
        {
            try
            {
                var borrower = await _borrowerService.GetBorrowerByIdAsync(borrowerId);
                if (borrower != null)
                {
                    CreateViewProfileForm(borrower);
                    viewProfileForm.ShowDialog(this);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading borrower profile: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void LoadBorrowerToForm(int borrowerId)
        {
            try
            {
                var borrower = await _borrowerService.GetBorrowerByIdAsync(borrowerId);
                if (borrower != null)
                {
                    txtFirstName.Text = borrower.FirstName ?? "";
                    txtLastName.Text = borrower.LastName ?? "";
                    txtMiddleName.Text = borrower.MiddleName ?? "";
                    dtpBirthDate.Value = borrower.DateOfBirth;
                    txtContact.Text = borrower.ContactNumber ?? "";
                    txtEmail.Text = borrower.Email ?? "";
                    txtAddress.Text = borrower.Address ?? "";

                    if (borrower.EmploymentStatus != null)
                        cmbEmploymentStatus.SelectedItem = borrower.EmploymentStatus.ToString();

                    txtIncome.Text = borrower.MonthlyIncome.ToString("N2");
                    txtEmployer.Text = borrower.EmployerName ?? "";
                    txtGuarantor.Text = borrower.GuarantorName ?? "";
                    txtGuarantorContact.Text = borrower.GuarantorContact ?? "";
                    txtReason.Text = borrower.ReasonForLoan ?? "";

                    lblStatus.Text = $"✓ Loaded borrower: {borrower.LastName}, {borrower.FirstName}";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading borrower: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task SaveBorrowerAsync()
        {
            try
            {
                if (!ValidateForm()) return;

                var borrower = new Borrower
                {
                    FirstName = txtFirstName.Text.Trim(),
                    LastName = txtLastName.Text.Trim(),
                    MiddleName = txtMiddleName.Text.Trim(),
                    DateOfBirth = dtpBirthDate.Value,
                    ContactNumber = txtContact.Text.Trim(),
                    Email = txtEmail.Text.Trim(),
                    Address = txtAddress.Text.Trim(),
                    EmploymentStatus = (EmploymentStatus)Enum.Parse(typeof(EmploymentStatus),
                        cmbEmploymentStatus.SelectedItem?.ToString() ?? "Employed"),
                    MonthlyIncome = decimal.Parse(txtIncome.Text),
                    EmployerName = txtEmployer.Text.Trim(),
                    GuarantorName = txtGuarantor.Text.Trim(),
                    GuarantorContact = txtGuarantorContact.Text.Trim(),
                    ReasonForLoan = txtReason.Text.Trim()
                };

                await _borrowerService.AddBorrowerAsync(borrower);

                MessageBox.Show("✓ Borrower saved successfully!", "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                ClearForm();
                await LoadBorrowersAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving borrower: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task UpdateBorrowerAsync()
        {
            try
            {
                if (!ValidateForm()) return;

                var borrower = new Borrower
                {
                    BorrowerId = selectedBorrowerId,
                    FirstName = txtFirstName.Text.Trim(),
                    LastName = txtLastName.Text.Trim(),
                    MiddleName = txtMiddleName.Text.Trim(),
                    DateOfBirth = dtpBirthDate.Value,
                    ContactNumber = txtContact.Text.Trim(),
                    Email = txtEmail.Text.Trim(),
                    Address = txtAddress.Text.Trim(),
                    EmploymentStatus = (EmploymentStatus)Enum.Parse(typeof(EmploymentStatus),
                        cmbEmploymentStatus.SelectedItem.ToString()),
                    MonthlyIncome = decimal.Parse(txtIncome.Text),
                    EmployerName = txtEmployer.Text.Trim(),
                    GuarantorName = txtGuarantor.Text.Trim(),
                    GuarantorContact = txtGuarantorContact.Text.Trim(),
                    ReasonForLoan = txtReason.Text.Trim()
                };

                await _borrowerService.UpdateBorrowerAsync(borrower);

                MessageBox.Show("✓ Borrower updated successfully!", "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                ClearForm();
                await LoadBorrowersAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating borrower: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task DeleteBorrowerAsync()
        {
            var result = MessageBox.Show("Are you sure you want to delete this borrower?",
                "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                try
                {
                    await _borrowerService.DeleteBorrowerAsync(selectedBorrowerId);

                    MessageBox.Show("✓ Borrower deleted successfully!", "Success",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    ClearForm();
                    await LoadBorrowersAsync();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error deleting borrower: {ex.Message}", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private bool ValidateForm()
        {
            if (string.IsNullOrWhiteSpace(txtFirstName.Text) ||
                string.IsNullOrWhiteSpace(txtLastName.Text) ||
                string.IsNullOrWhiteSpace(txtContact.Text) ||
                string.IsNullOrWhiteSpace(txtEmail.Text) ||
                string.IsNullOrWhiteSpace(txtAddress.Text) ||
                cmbEmploymentStatus.SelectedItem == null ||
                string.IsNullOrWhiteSpace(txtIncome.Text))
            {
                MessageBox.Show("Please fill in all required fields.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (!decimal.TryParse(txtIncome.Text, out decimal income) || income < 0)
            {
                MessageBox.Show("Please enter a valid monthly income.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        private void ClearForm()
        {
            txtFirstName.Clear();
            txtLastName.Clear();
            txtMiddleName.Clear();
            dtpBirthDate.Value = DateTime.Now.AddYears(-25);
            txtContact.Clear();
            txtEmail.Clear();
            txtAddress.Clear();
            cmbEmploymentStatus.SelectedIndex = -1;
            txtIncome.Clear();
            txtEmployer.Clear();
            txtGuarantor.Clear();
            txtGuarantorContact.Clear();
            txtReason.Clear();

            selectedBorrowerId = 0;

            btnSave.Enabled = true;
            btnUpdate.Enabled = false;
            btnDelete.Enabled = false;

            lblStatus.Text = "✓ Form cleared";
        }
    }
}