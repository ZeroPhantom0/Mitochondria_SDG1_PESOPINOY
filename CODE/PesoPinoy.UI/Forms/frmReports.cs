using Microsoft.Extensions.DependencyInjection;
using PesoPinoy.BLL.Services;
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
    public partial class frmReports : Form
    {
        private readonly ReportService _reportService = null!;
        private readonly RiskAnalysisService _riskService = null!;
        private Panel headerPanel = null!;
        private TabControl tabControl = null!;
        private FormsPlot collectionsChart = null!;
        private FormsPlot riskChart = null!;
        private FormsPlot loanPerformanceChart = null!;
        private DataGridView dgvSummary = null!;
        private FormsLabel lblStatus = null!;
        private ComboBox cmbYear = null!;
        private DataGridView dgvRisk = null!;
        private ListBox lstHighRisk = null!;
        private DataGridView dgvEmployment = null!;
        private DataGridView dgvIncome = null!;

        public frmReports()
        {
            _reportService = Program.ServiceProvider!.GetService<ReportService>()!;
            _riskService = Program.ServiceProvider!.GetService<RiskAnalysisService>()!;
            this.BackColor = DrawingColor.White;
            this.Text = "PesoPinoy - Reports & Analytics";
            this.Size = new Size(1200, 700);
            this.StartPosition = FormStartPosition.CenterScreen;

            CreateHeader();
            CreateTabControl();

            this.Load += async (s, e) => await LoadAllReportsAsync();
        }

        private void CreateHeader()
        {
            headerPanel = new Panel
            {
                Height = 60,
                Dock = DockStyle.Top,
                BackColor = DrawingColor.FromArgb(2, 58, 128)
            };

            FormsLabel lblTitle = new FormsLabel
            {
                Text = "Reports & Analytics",
                Font = new Font(new FontFamily("Segoe UI"), 18, DrawingFontStyle.Bold),
                ForeColor = DrawingColor.White,
                Location = new Point(20, 15),
                AutoSize = true
            };

            Button btnClose = new Button
            {
                Text = "X",
                Size = new Size(40, 40),
                Location = new Point(this.Width - 50, 10),
                FlatStyle = FlatStyle.Flat,
                BackColor = DrawingColor.FromArgb(2, 58, 128),
                ForeColor = DrawingColor.White,
                Font = new Font(new FontFamily("Segoe UI"), 12, DrawingFontStyle.Bold)
            };
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.Click += (s, e) => this.Close();

            Button btnRefresh = new Button
            {
                Text = "REFRESH",
                Size = new Size(100, 35),
                Location = new Point(this.Width - 160, 15),
                BackColor = DrawingColor.FromArgb(46, 204, 113),
                ForeColor = DrawingColor.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font(new FontFamily("Segoe UI"), 10, DrawingFontStyle.Bold)
            };
            btnRefresh.FlatAppearance.BorderSize = 0;
            btnRefresh.Click += async (s, e) => await LoadAllReportsAsync();

            headerPanel.Controls.Add(lblTitle);
            headerPanel.Controls.Add(btnClose);
            headerPanel.Controls.Add(btnRefresh);
            this.Controls.Add(headerPanel);
        }

        private void CreateTabControl()
        {
            tabControl = new TabControl
            {
                Location = new Point(20, 80),
                Size = new Size(1150, 580),
                Font = new Font(new FontFamily("Segoe UI"), 10, DrawingFontStyle.Regular)
            };

            // Dashboard Tab
            TabPage dashboardTab = new TabPage("Dashboard Overview");
            CreateDashboardTab(dashboardTab);
            tabControl.TabPages.Add(dashboardTab);

            // Collections Tab
            TabPage collectionsTab = new TabPage("Monthly Collections");
            CreateCollectionsTab(collectionsTab);
            tabControl.TabPages.Add(collectionsTab);

            // Risk Analysis Tab
            TabPage riskTab = new TabPage("Risk Analysis");
            CreateRiskTab(riskTab);
            tabControl.TabPages.Add(riskTab);

            // Demographics Tab
            TabPage demoTab = new TabPage("Borrower Demographics");
            CreateDemographicsTab(demoTab);
            tabControl.TabPages.Add(demoTab);

            this.Controls.Add(tabControl);

            lblStatus = new FormsLabel
            {
                Location = new Point(20, 670),
                Size = new Size(1150, 20),
                ForeColor = DrawingColor.FromArgb(2, 58, 128),
                Font = new Font(new FontFamily("Segoe UI"), 9, DrawingFontStyle.Regular)
            };
            this.Controls.Add(lblStatus);
        }

        private void CreateDashboardTab(TabPage tabPage)
        {
            // Summary Cards
            Panel cardPanel = new Panel
            {
                Location = new Point(10, 10),
                Size = new Size(1100, 100),
                BackColor = DrawingColor.Transparent
            };

            string[] cardTitles = { "Total Borrowers", "Active Loans", "Total Collections", "Avg Risk Score" };
            DrawingColor[] cardColors = { DrawingColor.FromArgb(52, 152, 219), DrawingColor.FromArgb(46, 204, 113),
                                   DrawingColor.FromArgb(155, 89, 182), DrawingColor.FromArgb(230, 126, 34) };
            string[] cardValues = { "0", "0", "₱0", "0%" };

            for (int i = 0; i < 4; i++)
            {
                Panel card = new Panel
                {
                    Location = new Point(i * 270, 0),
                    Size = new Size(250, 80),
                    BackColor = cardColors[i],
                    Tag = i
                };

                FormsLabel lblTitle = new FormsLabel
                {
                    Text = cardTitles[i],
                    Location = new Point(10, 10),
                    Size = new Size(230, 20),
                    ForeColor = DrawingColor.White,
                    Font = new Font(new FontFamily("Segoe UI"), 10, DrawingFontStyle.Regular)
                };

                FormsLabel lblValue = new FormsLabel
                {
                    Text = cardValues[i],
                    Location = new Point(10, 35),
                    Size = new Size(230, 30),
                    ForeColor = DrawingColor.White,
                    Font = new Font(new FontFamily("Segoe UI"), 16, DrawingFontStyle.Bold),
                    TextAlign = ContentAlignment.MiddleLeft,
                    Name = $"lblCard{i}"
                };

                card.Controls.Add(lblTitle);
                card.Controls.Add(lblValue);
                cardPanel.Controls.Add(card);
            }

            tabPage.Controls.Add(cardPanel);

            // Charts
            loanPerformanceChart = new FormsPlot
            {
                Location = new Point(10, 120),
                Size = new Size(540, 200)
            };

            riskChart = new FormsPlot
            {
                Location = new Point(560, 120),
                Size = new Size(530, 200)
            };

            tabPage.Controls.Add(loanPerformanceChart);
            tabPage.Controls.Add(riskChart);

            // Summary Grid
            dgvSummary = new DataGridView
            {
                Location = new Point(10, 330),
                Size = new Size(1080, 200),
                BackgroundColor = DrawingColor.White,
                BorderStyle = BorderStyle.FixedSingle,
                RowHeadersVisible = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false
            };

            dgvSummary.Columns.Add("Metric", "Metric");
            dgvSummary.Columns.Add("Value", "Value");
            dgvSummary.Columns.Add("Description", "Description");

            tabPage.Controls.Add(dgvSummary);
        }

        private void CreateCollectionsTab(TabPage tabPage)
        {
            cmbYear = new ComboBox
            {
                Location = new Point(10, 10),
                Size = new Size(100, 25),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font(new FontFamily("Segoe UI"), 9, DrawingFontStyle.Regular)
            };

            for (int year = DateTime.Now.Year - 2; year <= DateTime.Now.Year; year++)
            {
                cmbYear.Items.Add(year);
            }
            cmbYear.SelectedItem = DateTime.Now.Year;
            cmbYear.SelectedIndexChanged += async (s, e) =>
            {
                if (cmbYear.SelectedItem != null)
                    await LoadCollectionsChartAsync((int)cmbYear.SelectedItem);
            };

            tabPage.Controls.Add(cmbYear);

            collectionsChart = new FormsPlot
            {
                Location = new Point(10, 45),
                Size = new Size(1080, 480)
            };

            tabPage.Controls.Add(collectionsChart);
        }

        private void CreateRiskTab(TabPage tabPage)
        {
            dgvRisk = new DataGridView
            {
                Location = new Point(10, 10),
                Size = new Size(1080, 250),
                BackgroundColor = DrawingColor.White,
                BorderStyle = BorderStyle.FixedSingle,
                RowHeadersVisible = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                Font = new Font(new FontFamily("Segoe UI"), 9, DrawingFontStyle.Regular)
            };

            dgvRisk.Columns.Add("Category", "Risk Category");
            dgvRisk.Columns.Add("Count", "Number of Borrowers");
            dgvRisk.Columns.Add("Percentage", "Percentage");
            dgvRisk.Columns.Add("Exposure", "Loan Exposure");

            tabPage.Controls.Add(dgvRisk);

            FormsLabel lblHighRisk = new FormsLabel
            {
                Location = new Point(10, 270),
                Size = new Size(1080, 30),
                Font = new Font(new FontFamily("Segoe UI"), 12, DrawingFontStyle.Bold),
                ForeColor = DrawingColor.FromArgb(231, 76, 60),
                Text = "High Risk Borrowers Needing Review:"
            };
            tabPage.Controls.Add(lblHighRisk);

            lstHighRisk = new ListBox
            {
                Location = new Point(10, 310),
                Size = new Size(1080, 200),
                Font = new Font(new FontFamily("Segoe UI"), 10, DrawingFontStyle.Regular)
            };
            tabPage.Controls.Add(lstHighRisk);
        }

        private void CreateDemographicsTab(TabPage tabPage)
        {
            FormsLabel lblEmployment = new FormsLabel
            {
                Location = new Point(10, 10),
                Size = new Size(1080, 25),
                Font = new Font(new FontFamily("Segoe UI"), 12, DrawingFontStyle.Bold),
                ForeColor = DrawingColor.FromArgb(2, 58, 128),
                Text = "Employment Status Distribution"
            };
            tabPage.Controls.Add(lblEmployment);

            dgvEmployment = new DataGridView
            {
                Location = new Point(10, 40),
                Size = new Size(1080, 120),
                BackgroundColor = DrawingColor.White,
                BorderStyle = BorderStyle.FixedSingle,
                RowHeadersVisible = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                Font = new Font(new FontFamily("Segoe UI"), 9, DrawingFontStyle.Regular)
            };
            dgvEmployment.Columns.Add("Status", "Employment Status");
            dgvEmployment.Columns.Add("Count", "Number of Borrowers");
            dgvEmployment.Columns.Add("Percentage", "Percentage");
            tabPage.Controls.Add(dgvEmployment);

            FormsLabel lblIncome = new FormsLabel
            {
                Location = new Point(10, 170),
                Size = new Size(1080, 25),
                Font = new Font(new FontFamily("Segoe UI"), 12, DrawingFontStyle.Bold),
                ForeColor = DrawingColor.FromArgb(2, 58, 128),
                Text = "Income Bracket Distribution"
            };
            tabPage.Controls.Add(lblIncome);

            dgvIncome = new DataGridView
            {
                Location = new Point(10, 200),
                Size = new Size(1080, 120),
                BackgroundColor = DrawingColor.White,
                BorderStyle = BorderStyle.FixedSingle,
                RowHeadersVisible = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                Font = new Font(new FontFamily("Segoe UI"), 9, DrawingFontStyle.Regular)
            };
            dgvIncome.Columns.Add("Bracket", "Income Bracket");
            dgvIncome.Columns.Add("Count", "Number of Borrowers");
            dgvIncome.Columns.Add("Percentage", "Percentage");
            tabPage.Controls.Add(dgvIncome);
        }

        private async Task LoadAllReportsAsync()
        {
            try
            {
                lblStatus.Text = "Loading reports...";

                await LoadDashboardData();
                await LoadCollectionsChartAsync(DateTime.Now.Year);
                await LoadRiskData();
                await LoadDemographicsData();

                lblStatus.Text = "Reports loaded successfully.";
            }
            catch (Exception ex)
            {
                lblStatus.Text = $"Error loading reports: {ex.Message}";
                MessageBox.Show($"Error loading reports: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task LoadDashboardData()
        {
            try
            {
                var riskAnalysis = await _riskService.GetPortfolioRiskAnalysisAsync();
                var loanPerformance = await _reportService.GetLoanPerformanceReportAsync();

                // Calculate risk percentage correctly
                decimal riskPercentage = 0;
                if (riskAnalysis.ContainsKey("totalLoanAmount") && riskAnalysis.ContainsKey("highRiskExposure"))
                {
                    decimal totalLoanAmountValue = Convert.ToDecimal(riskAnalysis["totalLoanAmount"]);
                    decimal highRiskExposure = Convert.ToDecimal(riskAnalysis["highRiskExposure"]);
                    riskPercentage = totalLoanAmountValue > 0 ? (highRiskExposure / totalLoanAmountValue) * 100 : 0;
                }

                // Update cards
                var totalBorrowers = riskAnalysis.ContainsKey("totalBorrowers") ? riskAnalysis["totalBorrowers"].ToString() : "0";
                var activeLoans = riskAnalysis.ContainsKey("activeLoans") ? riskAnalysis["activeLoans"].ToString() : "0";
                var totalLoanAmountText = riskAnalysis.ContainsKey("totalLoanAmount") ? $"₱{riskAnalysis["totalLoanAmount"]:N0}" : "₱0";

                // This should now show correct percentage
                var riskPercentageText = $"{riskPercentage:F1}%";

                // Update labels
                foreach (Control control in tabControl.TabPages[0].Controls)
                {
                    if (control is Panel panel)
                    {
                        foreach (Control cardControl in panel.Controls)
                        {
                            if (cardControl is Panel card)
                            {
                                foreach (Control label in card.Controls)
                                {
                                    if (label is FormsLabel lbl)
                                    {
                                        if (lbl.Name == "lblCard0") lbl.Text = totalBorrowers;
                                        if (lbl.Name == "lblCard1") lbl.Text = activeLoans;
                                        if (lbl.Name == "lblCard2") lbl.Text = totalLoanAmountText;
                                        if (lbl.Name == "lblCard3") lbl.Text = riskPercentageText;
                                    }
                                }
                            }
                        }
                    }
                }

                // Loan Performance Chart
                if (loanPerformance != null)
                {
                    var perf = loanPerformance.GetType();
                    var active = (int)(perf.GetProperty("activeLoans")?.GetValue(loanPerformance) ?? 0);
                    var completed = (int)(perf.GetProperty("completedLoans")?.GetValue(loanPerformance) ?? 0);
                    var defaulted = (int)(perf.GetProperty("defaultedLoans")?.GetValue(loanPerformance) ?? 0);

                    double[] values = { active, completed, defaulted };

                    loanPerformanceChart.Plot.Clear();
                    var bars = loanPerformanceChart.Plot.Add.Bars(values);
                    bars.Color = ScottPlotColor.FromHex("#023A80");

                    loanPerformanceChart.Plot.Title("Loan Performance");
                    loanPerformanceChart.Plot.XLabel("Status");
                    loanPerformanceChart.Plot.YLabel("Number of Loans");

                    // Use AxisLimits.Bottom and AxisLimits.Top (AxisLimits does not have YMin/YMax)
                    var limits = loanPerformanceChart.Plot.Axes.GetLimits();
                    loanPerformanceChart.Plot.Axes.SetLimits(-0.5, 2.5, limits.Bottom, limits.Top);

                    loanPerformanceChart.Refresh();
                }

                // Summary Grid
                dgvSummary.Rows.Clear();
                if (riskAnalysis != null)
                {
                    dgvSummary.Rows.Add("Total Portfolio", $"₱{riskAnalysis["totalLoanAmount"]:N2}", "Total outstanding loans");
                    dgvSummary.Rows.Add("High Risk Exposure", $"₱{riskAnalysis["highRiskExposure"]:N2}", "Loans to high-risk borrowers");
                    dgvSummary.Rows.Add("Risk Ratio", $"{riskPercentage:F1}%", "Percentage of high-risk loans");
                    dgvSummary.Rows.Add("Overdue Loans", riskAnalysis["overdueLoans"].ToString(), "Loans with missed payments");
                }
            }
            catch
            {
                // Silently handle
            }
        }
        

        private async Task LoadCollectionsChartAsync(int year)
        {
            try
            {
                var monthlyData = await _reportService.GetMonthlyCollectionsReportAsync(year) as System.Collections.IList;

                if (collectionsChart != null && monthlyData != null)
                {
                    double[] values = new double[monthlyData.Count];

                    for (int i = 0; i < monthlyData.Count; i++)
                    {
                        var item = monthlyData[i];
                        var amount = (decimal)item.GetType().GetProperty("Amount").GetValue(item);

                        values[i] = Convert.ToDouble(amount);
                    }

                    collectionsChart.Plot.Clear();
                    var barPlot = collectionsChart.Plot.Add.Bars(values);
                    barPlot.Color = ScottPlotColor.FromHex("#023A80");

                    collectionsChart.Plot.Title($"Monthly Collections - {year}");
                    collectionsChart.Plot.XLabel("Month");
                    collectionsChart.Plot.YLabel("Amount (₱)");

                    var limits = collectionsChart.Plot.Axes.GetLimits();
                    collectionsChart.Plot.Axes.SetLimits(-0.5, values.Length - 0.5, limits.Bottom, limits.Top);

                    collectionsChart.Refresh();
                }
            }
            catch
            {
                // Silently handle
            }
        }

        private async Task LoadRiskData()
        {
            try
            {
                var riskAnalysis = await _riskService.GetPortfolioRiskAnalysisAsync();
                var riskDistribution = riskAnalysis.ContainsKey("riskDistribution") ?
                    riskAnalysis["riskDistribution"] as System.Collections.Generic.Dictionary<string, int> :
                    new System.Collections.Generic.Dictionary<string, int>();

                // Risk Chart
                if (riskChart != null && riskDistribution.Any())
                {
                    double[] values = riskDistribution.Values.Select(v => (double)v).ToArray();

                    riskChart.Plot.Clear();
                    var bars = riskChart.Plot.Add.Bars(values);
                    bars.Color = ScottPlotColor.FromHex("#023A80");

                    riskChart.Plot.Title("Risk Distribution");
                    riskChart.Plot.XLabel("Risk Categories");
                    riskChart.Plot.YLabel("Number of Borrowers");

                    var limits = riskChart.Plot.Axes.GetLimits();
                    riskChart.Plot.Axes.SetLimits(-0.5, values.Length - 0.5, limits.Bottom, limits.Top);

                    riskChart.Refresh();
                }

                // Update risk grid
                dgvRisk.Rows.Clear();
                int totalBorrowers = riskDistribution.Values.Sum();
                foreach (var risk in riskDistribution)
                {
                    double percentage = totalBorrowers > 0 ? (risk.Value * 100.0 / totalBorrowers) : 0;
                    dgvRisk.Rows.Add(risk.Key, risk.Value, $"{percentage:F1}%", "₱0");
                }

                // Load high risk borrowers
                lstHighRisk.Items.Clear();
                var highRiskBorrowers = await _riskService.GetBorrowersNeedingReviewAsync();
                foreach (var borrower in highRiskBorrowers.Take(10))
                {
                    lstHighRisk.Items.Add($"{borrower.LastName}, {borrower.FirstName} - Risk Score: {borrower.RiskScore:F1} ({borrower.RiskClassification})");
                }
            }
            catch
            {
                // Silently handle
            }
        }

        private async Task LoadDemographicsData()
        {
            try
            {
                var demographics = await _reportService.GetBorrowerDemographicsReportAsync();

                // Employment stats
                dgvEmployment.Rows.Clear();
                var employmentStats = demographics.GetType().GetProperty("employmentStats")?.GetValue(demographics) as System.Collections.IList;
                if (employmentStats != null)
                {
                    int total = 0;
                    foreach (var stat in employmentStats)
                    {
                        var count = (int)stat.GetType().GetProperty("Count").GetValue(stat);
                        total += count;
                    }

                    foreach (var stat in employmentStats)
                    {
                        var status = (string)stat.GetType().GetProperty("Status").GetValue(stat);
                        var count = (int)stat.GetType().GetProperty("Count").GetValue(stat);
                        double percentage = total > 0 ? (count * 100.0 / total) : 0;
                        dgvEmployment.Rows.Add(status, count, $"{percentage:F1}%");
                    }
                }

                // Income stats
                dgvIncome.Rows.Clear();
                var incomeStats = demographics.GetType().GetProperty("incomeStats")?.GetValue(demographics) as System.Collections.IList;
                if (incomeStats != null)
                {
                    int total = 0;
                    foreach (var stat in incomeStats)
                    {
                        var count = (int)stat.GetType().GetProperty("Count").GetValue(stat);
                        total += count;
                    }

                    foreach (var stat in incomeStats)
                    {
                        var bracket = (string)stat.GetType().GetProperty("Bracket").GetValue(stat);
                        var count = (int)stat.GetType().GetProperty("Count").GetValue(stat);
                        double percentage = total > 0 ? (count * 100.0 / total) : 0;
                        dgvIncome.Rows.Add(bracket, count, $"{percentage:F1}%");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}