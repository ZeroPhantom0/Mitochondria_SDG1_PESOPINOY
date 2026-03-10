using System;
using System.Drawing;
using System.Windows.Forms;
using PesoPinoy.BLL.Helpers;

namespace PesoPinoy.UI.Controls
{
    public partial class ucLoanCalculator : UserControl
    {
        private TextBox txtPrincipal, txtRate, txtTerm;
        private Label lblMonthlyPayment, lblTotalAmount;
        private Button btnCalculate;

        public event EventHandler<LoanCalculationResult> CalculationCompleted;

        public ucLoanCalculator()
        {
            InitializeComponent();
            this.Size = new Size(400, 300);
            this.BackColor = Color.White;
            CreateCalculator();
        }

        private void CreateCalculator()
        {
            int yPos = 20;

            // Title
            Label lblTitle = new Label
            {
                Text = "Loan Calculator",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(2, 58, 128),
                Location = new Point(20, yPos),
                AutoSize = true
            };
            this.Controls.Add(lblTitle);

            // Principal
            yPos += 40;
            Label lblPrincipal = new Label
            {
                Text = "Principal Amount:",
                Location = new Point(20, yPos),
                Size = new Size(120, 25),
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.FromArgb(2, 58, 128)
            };

            txtPrincipal = new TextBox
            {
                Location = new Point(150, yPos),
                Size = new Size(150, 25),
                Font = new Font("Segoe UI", 10),
                BorderStyle = BorderStyle.FixedSingle,
                Text = "10000"
            };
            this.Controls.Add(lblPrincipal);
            this.Controls.Add(txtPrincipal);

            // Interest Rate
            yPos += 35;
            Label lblRate = new Label
            {
                Text = "Interest Rate (%):",
                Location = new Point(20, yPos),
                Size = new Size(120, 25),
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.FromArgb(2, 58, 128)
            };

            txtRate = new TextBox
            {
                Location = new Point(150, yPos),
                Size = new Size(150, 25),
                Font = new Font("Segoe UI", 10),
                BorderStyle = BorderStyle.FixedSingle,
                Text = "5"
            };
            this.Controls.Add(lblRate);
            this.Controls.Add(txtRate);

            // Term
            yPos += 35;
            Label lblTerm = new Label
            {
                Text = "Term (months):",
                Location = new Point(20, yPos),
                Size = new Size(120, 25),
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.FromArgb(2, 58, 128)
            };

            txtTerm = new TextBox
            {
                Location = new Point(150, yPos),
                Size = new Size(150, 25),
                Font = new Font("Segoe UI", 10),
                BorderStyle = BorderStyle.FixedSingle,
                Text = "12"
            };
            this.Controls.Add(lblTerm);
            this.Controls.Add(txtTerm);

            // Calculate Button
            yPos += 40;
            btnCalculate = new Button
            {
                Text = "CALCULATE",
                Location = new Point(150, yPos),
                Size = new Size(150, 35),
                BackColor = Color.FromArgb(2, 58, 128),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            btnCalculate.FlatAppearance.BorderSize = 0;
            btnCalculate.Click += BtnCalculate_Click;
            this.Controls.Add(btnCalculate);

            // Results
            yPos += 50;
            Label lblMonthly = new Label
            {
                Text = "Monthly Payment:",
                Location = new Point(20, yPos),
                Size = new Size(120, 25),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(2, 58, 128)
            };

            lblMonthlyPayment = new Label
            {
                Location = new Point(150, yPos),
                Size = new Size(150, 25),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(46, 204, 113),
                Text = "₱ 0.00"
            };
            this.Controls.Add(lblMonthly);
            this.Controls.Add(lblMonthlyPayment);

            yPos += 30;
            Label lblTotalInterest = new Label
            {
                Text = "Total Interest:",
                Location = new Point(20, yPos),
                Size = new Size(120, 25),
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.FromArgb(2, 58, 128)
            };

            lblTotalInterestAmt = new Label
            {
                Location = new Point(150, yPos),
                Size = new Size(150, 25),
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.FromArgb(52, 152, 219),
                Text = "₱ 0.00"
            };
            this.Controls.Add(lblTotalInterest);
            this.Controls.Add(lblTotalInterestAmt);

            yPos += 30;
            Label lblTotal = new Label
            {
                Text = "Total Amount:",
                Location = new Point(20, yPos),
                Size = new Size(120, 25),
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.FromArgb(2, 58, 128)
            };

            lblTotalAmount = new Label
            {
                Location = new Point(150, yPos),
                Size = new Size(150, 25),
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.FromArgb(155, 89, 182),
                Text = "₱ 0.00"
            };
            this.Controls.Add(lblTotal);
            this.Controls.Add(lblTotalAmount);
        }

        private Label lblTotalInterestAmt;

        private void BtnCalculate_Click(object sender, EventArgs e)
        {
            try
            {
                if (!ValidateInput()) return;

                decimal principal = decimal.Parse(txtPrincipal.Text);
                decimal rate = decimal.Parse(txtRate.Text);
                int term = int.Parse(txtTerm.Text);

                var calculator = new LoanCalculator();
                var result = calculator.CalculateLoan(principal, rate, term);

                lblMonthlyPayment.Text = $"₱ {result.MonthlyPayment:N2}";
                lblTotalInterestAmt.Text = $"₱ {result.TotalInterest:N2}";
                lblTotalAmount.Text = $"₱ {result.TotalAmount:N2}";

                CalculationCompleted?.Invoke(this, result);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error calculating loan: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool ValidateInput()
        {
            if (!decimal.TryParse(txtPrincipal.Text, out decimal principal) || principal <= 0)
            {
                MessageBox.Show("Please enter a valid principal amount.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (!decimal.TryParse(txtRate.Text, out decimal rate) || rate <= 0 || rate > 100)
            {
                MessageBox.Show("Please enter a valid interest rate (1-100).", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (!int.TryParse(txtTerm.Text, out int term) || term <= 0 || term > 60)
            {
                MessageBox.Show("Please enter a valid term (1-60 months).", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

    }
}