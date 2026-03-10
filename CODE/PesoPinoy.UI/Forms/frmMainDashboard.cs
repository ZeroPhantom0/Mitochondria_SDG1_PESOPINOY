using Microsoft.Extensions.DependencyInjection;
using PesoPinoy.BLL.Services;
using PesoPinoy.Models.Enums;
using ScottPlot;
using ScottPlot.WinForms;
using System;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DrawingColor = System.Drawing.Color;
using DrawingFontStyle = System.Drawing.FontStyle;
using FormsLabel = System.Windows.Forms.Label;
using ScottPlotColor = ScottPlot.Color;

namespace PesoPinoy.UI.Forms
{
    public partial class frmMainDashboard : Form
    {
        private Panel sidePanel;
        private Panel contentPanel;
        private FormsLabel lblWelcome;
        private FormsPlot collectionsChart;
        private FormsPlot riskChart;
        private FormsLabel lblCard1Value, lblCard2Value, lblCard3Value, lblCard4Value;

        private readonly BorrowerService _borrowerService;
        private readonly LoanService _loanService;
        private readonly PaymentService _paymentService;
        private readonly RiskAnalysisService _riskService;

        public frmMainDashboard()
        {
            InitializeComponent();

            try
            {
                // Get services
                if (Program.ServiceProvider != null)
                {
                    _borrowerService = Program.ServiceProvider.GetService<BorrowerService>();
                    _loanService = Program.ServiceProvider.GetService<LoanService>();
                    _paymentService = Program.ServiceProvider.GetService<PaymentService>();
                    _riskService = Program.ServiceProvider.GetService<RiskAnalysisService>();
                }

                this.WindowState = FormWindowState.Maximized;
                this.BackColor = DrawingColor.White;
                this.Text = "PesoPinoy - Administrator Dashboard";
                this.StartPosition = FormStartPosition.CenterScreen;

                // Create ALL UI elements first
                CreateSidePanel();
                CreateContentArea();

                // Then load data
                this.Load += async (s, e) => await LoadDashboardData();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error initializing dashboard: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CreateSidePanel()
        {
            sidePanel = new Panel
            {
                Width = 250,
                Dock = DockStyle.Left,
                BackColor = DrawingColor.FromArgb(2, 58, 128)
            };

            // Logo
            var lblLogo = new FormsLabel
            {
                Text = "PesoPinoy",
                Font = new Font(new FontFamily("Segoe UI"), 18, DrawingFontStyle.Bold),
                ForeColor = DrawingColor.White,
                Location = new Point(30, 30),
                AutoSize = true
            };
            sidePanel.Controls.Add(lblLogo);

            // Navigation buttons
            string[] menuItems = { "Borrowers", "Loans", "Savings", "Insurance",
                                   "Payments", "Reports", "Backup & Restore", "Logout" };
            int yPos = 100;

            for (int i = 0; i < menuItems.Length; i++)
            {
                var btn = new Button
                {
                    Text = menuItems[i],
                    Width = 200,
                    Height = 45,
                    Location = new Point(25, yPos),
                    FlatStyle = FlatStyle.Flat,
                    BackColor = DrawingColor.Transparent,
                    ForeColor = DrawingColor.White,
                    Font = new Font(new FontFamily("Segoe UI"), 11, DrawingFontStyle.Regular),
                    TextAlign = ContentAlignment.MiddleLeft,
                    FlatAppearance = { BorderSize = 0 },
                    Tag = i
                };

                btn.MouseEnter += (s, e) => btn.BackColor = DrawingColor.FromArgb(0, 40, 100);
                btn.MouseLeave += (s, e) => btn.BackColor = DrawingColor.Transparent;

                // Assign click events
                switch (i)
                {
                    case 0: btn.Click += (s, e) => OpenForm<frmBorrowerManagement>(); break;
                    case 1: btn.Click += (s, e) => OpenForm<frmLoanManagement>(); break;
                    case 2: btn.Click += (s, e) => OpenForm<frmSavingsManagement>(); break;
                    case 3: btn.Click += (s, e) => OpenForm<frmInsuranceManagement>(); break;
                    case 4: btn.Click += (s, e) => OpenForm<frmPaymentScheduler>(); break;
                    case 5: btn.Click += (s, e) => OpenForm<frmReports>(); break;
                    case 6: btn.Click += (s, e) => OpenForm<frmBackupRestore>(); break;
                    case 7: btn.Click += (s, e) => Logout(); break;
                }

                sidePanel.Controls.Add(btn);
                yPos += 55;
            }

            // User info at bottom
            var lblUser = new FormsLabel
            {
                Text = $"Logged in as:\n{Program.CurrentUser?.FullName ?? "Administrator"}",
                Font = new Font(new FontFamily("Segoe UI"), 9, DrawingFontStyle.Regular),
                ForeColor = DrawingColor.White,
                Location = new Point(30, this.Height - 100),
                AutoSize = true
            };
            sidePanel.Controls.Add(lblUser);

            this.Controls.Add(sidePanel);
        }

        private void CreateContentArea()
        {
            contentPanel = new Panel
            {
                Location = new Point(250, 0),  // Start after sidebar
                Width = this.ClientSize.Width - 250,
                Height = this.ClientSize.Height,
                BackColor = DrawingColor.White,
                AutoScroll = true
            };

            // Welcome header
            lblWelcome = new FormsLabel
            {
                Text = $"Welcome, {Program.CurrentUser?.FullName ?? "Administrator"}",
                Font = new Font(new FontFamily("Segoe UI"), 16, DrawingFontStyle.Bold),
                ForeColor = DrawingColor.FromArgb(2, 58, 128),
                Location = new Point(20, 20),
                AutoSize = true
            };
            contentPanel.Controls.Add(lblWelcome);

            CreateDashboardCards();
            CreateCharts();

            this.Controls.Add(contentPanel);

            // Handle resize
            this.Resize += (s, e) =>
            {
                contentPanel.Width = this.ClientSize.Width - 250;
                contentPanel.Height = this.ClientSize.Height;
                RepositionCharts();
            };
        }

        private void CreateDashboardCards()
        {
            string[] cardTitles = { "Total Borrowers", "Active Loans", "Monthly Collections", "Avg Risk Score" };
            DrawingColor[] cardColors = {
                DrawingColor.FromArgb(52, 152, 219),
                DrawingColor.FromArgb(46, 204, 113),
                DrawingColor.FromArgb(155, 89, 182),
                DrawingColor.FromArgb(230, 126, 34)
            };

            int cardWidth = 220;
            int cardHeight = 100;
            int spacing = 20;
            int startX = 20;
            int startY = 70;

            for (int i = 0; i < cardTitles.Length; i++)
            {
                var card = new Panel
                {
                    Width = cardWidth,
                    Height = cardHeight,
                    Location = new Point(startX + (i * (cardWidth + spacing)), startY),
                    BackColor = cardColors[i],
                    BorderStyle = BorderStyle.None
                };

                var lblTitle = new FormsLabel
                {
                    Text = cardTitles[i],
                    Font = new Font(new FontFamily("Segoe UI"), 10, DrawingFontStyle.Bold),
                    ForeColor = DrawingColor.White,
                    Location = new Point(15, 15),
                    AutoSize = true
                };

                var lblValue = new FormsLabel
                {
                    Text = "Loading...",
                    Font = new Font(new FontFamily("Segoe UI"), 20, DrawingFontStyle.Bold),
                    ForeColor = DrawingColor.White,
                    Location = new Point(15, 45),
                    AutoSize = true,
                    Name = $"lblCard{i}Value"
                };

                // Store references
                switch (i)
                {
                    case 0: lblCard1Value = lblValue; break;
                    case 1: lblCard2Value = lblValue; break;
                    case 2: lblCard3Value = lblValue; break;
                    case 3: lblCard4Value = lblValue; break;
                }

                card.Controls.Add(lblTitle);
                card.Controls.Add(lblValue);
                contentPanel.Controls.Add(card);
            }
        }

        private void CreateCharts()
        {
            try
            {
                int chartY = 200;

                // Collections chart
                collectionsChart = new FormsPlot
                {
                    Width = 500,
                    Height = 300,
                    Location = new Point(20, chartY)
                };

                collectionsChart.Plot.Title("Monthly Collections (Last 6 Months)");
                collectionsChart.Plot.YLabel("Amount (₱)");
                collectionsChart.Plot.XLabel("Month");
                collectionsChart.Plot.Add.Text("Loading data...", 0, 0);
                collectionsChart.Refresh();

                contentPanel.Controls.Add(collectionsChart);

                // Risk distribution chart
                riskChart = new FormsPlot
                {
                    Width = 450,
                    Height = 300,
                    Location = new Point(540, chartY)
                };

                riskChart.Plot.Title("Risk Distribution");
                riskChart.Plot.XLabel("Risk Categories");
                riskChart.Plot.YLabel("Number of Borrowers");
                riskChart.Plot.Add.Text("Loading data...", 0, 0);
                riskChart.Refresh();

                contentPanel.Controls.Add(riskChart);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error creating charts: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RepositionCharts()
        {
            if (collectionsChart != null)
                collectionsChart.Location = new Point(20, 200);
            if (riskChart != null)
                riskChart.Location = new Point(540, 200);
        }

        private async Task LoadDashboardData()
        {
            try
            {
                if (_borrowerService == null || _loanService == null ||
                    _paymentService == null || _riskService == null) return;

                // Load data
                var borrowers = await _borrowerService.GetAllBorrowersAsync();
                var loans = await _loanService.GetAllLoansAsync();

                // Calculate statistics
                int totalBorrowers = borrowers.Count();
                int activeLoans = loans.Count(l => l.Status == LoanStatus.Active);

                // Get current month collections
                var now = DateTime.Now;
                var startOfMonth = new DateTime(now.Year, now.Month, 1);
                var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);
                var monthlyCollections = await _paymentService.GetTotalCollectionsAsync(startOfMonth, endOfMonth);

                // Calculate average risk score (0-100 scale)
                double avgRiskScore = borrowers.Any() ? Math.Round((double)borrowers.Average(b => b.RiskScore), 1) : 0;

                // Update cards on UI thread
                this.Invoke(new Action(() =>
                {
                    if (lblCard1Value != null) lblCard1Value.Text = totalBorrowers.ToString("N0");
                    if (lblCard2Value != null) lblCard2Value.Text = activeLoans.ToString("N0");
                    if (lblCard3Value != null) lblCard3Value.Text = $"₱{monthlyCollections:N0}";
                    if (lblCard4Value != null) lblCard4Value.Text = $"{avgRiskScore:F1}";
                }));

                // Update charts
                await LoadMonthlyCollectionsChart();
                await LoadRiskDistributionChart(borrowers);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading dashboard data: {ex.Message}");
            }
        }

        private async Task LoadMonthlyCollectionsChart()
        {
            try
            {
                if (collectionsChart == null || _paymentService == null) return;

                var now = DateTime.Now;
                var values = new double[6];
                var labels = new string[6];

                // Get last 6 months of collections
                for (int i = 5; i >= 0; i--)
                {
                    var month = now.AddMonths(-i);
                    var startDate = new DateTime(month.Year, month.Month, 1);
                    var endDate = startDate.AddMonths(1).AddDays(-1);

                    var collections = await _paymentService.GetTotalCollectionsAsync(startDate, endDate);
                    values[5 - i] = (double)collections;
                    labels[5 - i] = month.ToString("MMM");
                }

                // Update chart on UI thread
                this.Invoke(new Action(() =>
                {
                    try
                    {
                        if (collectionsChart == null) return;

                        collectionsChart.Plot.Clear();

                        if (values.Sum() > 0)
                        {
                            var barPlot = collectionsChart.Plot.Add.Bars(values);
                            barPlot.Color = ScottPlotColor.FromHex("#023A80");

                            // Add month labels
                            for (int i = 0; i < labels.Length; i++)
                            {
                                collectionsChart.Plot.Add.Text(labels[i], i, -values.Max() * 0.05);
                            }

                            collectionsChart.Plot.Title("Monthly Collections (Last 6 Months)");
                            collectionsChart.Plot.YLabel("Amount (₱)");

                            collectionsChart.Plot.Axes.SetLimits(-0.5, 5.5, 0, values.Max() * 1.2);
                        }
                        else
                        {
                            collectionsChart.Plot.Add.Text("No collection data available", 0, 0);
                        }

                        collectionsChart.Refresh();
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Chart update error: {ex.Message}");
                    }
                }));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"LoadMonthlyCollectionsChart error: {ex.Message}");
            }
        }

        private async Task LoadRiskDistributionChart(IEnumerable<PesoPinoy.Models.Entities.Borrower> borrowers)
        {
            try
            {
                if (riskChart == null) return;

                // Group borrowers by risk classification
                var riskGroups = borrowers
                    .GroupBy(b => b.RiskClassification)
                    .Select(g => new { Risk = g.Key.ToString(), Count = g.Count() })
                    .OrderBy(r => r.Risk)
                    .ToList();

                // Update chart on UI thread
                this.Invoke(new Action(() =>
                {
                    try
                    {
                        if (riskChart == null) return;

                        riskChart.Plot.Clear();

                        if (riskGroups.Any())
                        {
                            double[] values = riskGroups.Select(r => (double)r.Count).ToArray();
                            string[] labels = riskGroups.Select(r => r.Risk).ToArray();

                            var barPlot = riskChart.Plot.Add.Bars(values);
                            barPlot.Color = ScottPlotColor.FromHex("#023A80");

                            // Add labels
                            for (int i = 0; i < labels.Length; i++)
                            {
                                riskChart.Plot.Add.Text(labels[i], i, -values.Max() * 0.05);
                            }

                            riskChart.Plot.Title("Risk Distribution");
                            riskChart.Plot.YLabel("Number of Borrowers");

                            riskChart.Plot.Axes.SetLimits(-0.5, values.Length - 0.5, 0, values.Max() * 1.2);
                        }
                        else
                        {
                            riskChart.Plot.Add.Text("No borrower data available", 0, 0);
                        }

                        riskChart.Refresh();
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Risk chart error: {ex.Message}");
                    }
                }));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"LoadRiskDistributionChart error: {ex.Message}");
            }
        }

        private void OpenForm<T>() where T : Form, new()
        {
            try
            {
                T form = new T();
                form.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening form: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Logout()
        {
            var result = MessageBox.Show("Are you sure you want to logout?", "Logout",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                Program.CurrentUser = null;
                var login = new frmLogin();
                login.Show();
                this.Close();
            }
        }
    }
}