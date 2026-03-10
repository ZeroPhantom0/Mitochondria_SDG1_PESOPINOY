using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Extensions.DependencyInjection;
using PesoPinoy.BLL.Services;
using PesoPinoy.Models.Entities;

namespace PesoPinoy.UI.Forms
{
    public partial class frmLogin : Form
    {
        private readonly AuthenticationService? _authService;
        private bool _isDragging = false;
        private Point _startPoint = new Point(0, 0);
        private TextBox txtUsername;
        private TextBox txtPassword;
        private Label lblStatus;
        private Button btnLogin;
        private PictureBox logoPictureBox; // Add this for logo

        public frmLogin()
        {
            InitializeComponent();

            try
            {
                if (Program.ServiceProvider != null)
                {
                    _authService = Program.ServiceProvider.GetService<AuthenticationService>();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error initializing: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = Color.White;
            this.Size = new Size(700, 500);

            CreateLoginUI();
        }

        private void CreateLoginUI()
        {
            mainPanel = new Panel { Dock = DockStyle.Fill, BackColor = Color.White };

            // Left Panel
            leftPanel = new Panel
            {
                Width = 300,
                Height = this.Height,
                Location = new Point(0, 0),
                BackColor = Color.FromArgb(2, 58, 128)
            };

            // Logo - Add this section
            logoPictureBox = new PictureBox
            {
                Size = new Size(120, 120),
                Location = new Point(90, 90), 
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.Transparent
            };

            // Try to load logo from different possible locations
            LoadLogoImage();

            lblAppName = new Label
            {
                Text = "PesoPinoy",
                Font = new Font("Segoe UI", 24, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = true,
                Location = new Point(50, 230)
            };

            lblSlogan = new Label
            {
                Text = "Community Microfinance\n& Loan Management System",
                Font = new Font("Segoe UI", 12),
                ForeColor = Color.White,
                AutoSize = true,
                Location = new Point(50, 280),
                TextAlign = ContentAlignment.MiddleCenter
            };


            leftPanel.Controls.AddRange(new Control[] {
                logoPictureBox,  // Add logo first
                lblAppName,
                lblSlogan,
                lblSDG
            });

            // Right Panel
            rightPanel = new Panel
            {
                Width = 400,
                Height = this.Height,
                Location = new Point(300, 0),
                BackColor = Color.White
            };

            lblLogin = new Label
            {
                Text = "Administrator Login",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.FromArgb(2, 58, 128),
                AutoSize = true,
                Location = new Point(80, 100)
            };

            lblUsername = new Label
            {
                Text = "Username:",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.Black,
                AutoSize = true,
                Location = new Point(50, 180)
            };

            txtUsername = new TextBox
            {
                Width = 250,
                Height = 30,
                Location = new Point(50, 210),
                Font = new Font("Segoe UI", 10),
                BorderStyle = BorderStyle.FixedSingle
            };

            lblPassword = new Label
            {
                Text = "Password:",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.Black,
                AutoSize = true,
                Location = new Point(50, 250)
            };

            txtPassword = new TextBox
            {
                Width = 250,
                Height = 30,
                Location = new Point(50, 280),
                Font = new Font("Segoe UI", 10),
                BorderStyle = BorderStyle.FixedSingle,
                PasswordChar = '*'
            };

            btnLogin = new Button
            {
                Text = "LOGIN",
                Width = 120,
                Height = 40,
                Location = new Point(115, 330),
                BackColor = Color.FromArgb(2, 58, 128),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            btnLogin.FlatAppearance.BorderSize = 0;
            btnLogin.Click += async (s, e) => await LoginAsync();

            // Add hover effect
            btnLogin.MouseEnter += (s, e) => btnLogin.BackColor = ControlPaint.Light(Color.FromArgb(2, 58, 128), 0.2f);
            btnLogin.MouseLeave += (s, e) => btnLogin.BackColor = Color.FromArgb(2, 58, 128);

            lblStatus = new Label
            {
                Text = "",
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.Red,
                AutoSize = true,
                Location = new Point(50, 380)
            };

            rightPanel.Controls.AddRange(new Control[] {
                lblLogin, lblUsername, txtUsername, lblPassword,
                txtPassword, btnLogin, lblStatus
            });

            // Title Bar
            titleBar = new Panel
            {
                Height = 30,
                Dock = DockStyle.Top,
                BackColor = Color.FromArgb(2, 58, 128)
            };

            btnClose = new Button
            {
                Text = "X",
                Size = new Size(30, 30),
                Location = new Point(this.Width - 30, 0),
                FlatStyle = FlatStyle.Flat,
                ForeColor = Color.White,
                BackColor = Color.FromArgb(2, 58, 128)
            };
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.Click += (s, e) => Application.Exit();

            // Add hover effect for close button
            btnClose.MouseEnter += (s, e) => btnClose.BackColor = Color.FromArgb(231, 76, 60);
            btnClose.MouseLeave += (s, e) => btnClose.BackColor = Color.FromArgb(2, 58, 128);

            titleBar.Controls.Add(btnClose);
            titleBar.MouseDown += (s, e) => { _isDragging = true; _startPoint = new Point(e.X, e.Y); };
            titleBar.MouseMove += (s, e) => { if (_isDragging) { Point p = PointToScreen(e.Location); Location = new Point(p.X - _startPoint.X, p.Y - _startPoint.Y); } };
            titleBar.MouseUp += (s, e) => _isDragging = false;

            this.Controls.Add(mainPanel);
            mainPanel.Controls.Add(leftPanel);
            mainPanel.Controls.Add(rightPanel);
            mainPanel.Controls.Add(titleBar);
            titleBar.BringToFront();
        }

        private void LoadLogoImage()
        {
            try
            {
                // List of possible logo locations
                string[] possiblePaths = new string[]
                {
                    Path.Combine(Application.StartupPath, "logo.png"),
                    Path.Combine(Application.StartupPath, "Images", "logo.png"),
                    Path.Combine(Application.StartupPath, "Resources", "logo.png"),
                    Path.Combine(Application.StartupPath, "Assets", "logo.png"),
                    Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logo.png"),
                    @"C:\Users\user\source\repos\Mitochondria_SDG1_PESOPINOY\CODE\PesoPinoy.UI\PesoPinoy.UI", // Adjust this path to where your logo is
                    @"C:\Users\user\source\repos\Mitochondria_SDG1_PESOPINOY\CODE\PesoPinoy.UI\logo.png" // Add your actual logo path
                };

                foreach (string path in possiblePaths)
                {
                    if (File.Exists(path))
                    {
                        using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read))
                        {
                            logoPictureBox.Image = Image.FromStream(fs);
                        }
                        break;
                    }
                }

                if (logoPictureBox.Image == null)
                {
 
                    logoPictureBox.Visible = false;
                }
            }
            catch (Exception ex)
            {
                // If error loading logo, just hide it
                logoPictureBox.Visible = false;
                Console.WriteLine($"Logo loading error: {ex.Message}");
            }
        }

        private void CreatePlaceholderLogo()
        {
            Bitmap bmp = new Bitmap(120, 120);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.FromArgb(2, 58, 128));
                using (Brush brush = new SolidBrush(Color.White))
                using (Font font = new Font("Segoe UI", 48, FontStyle.Bold))
                {
                    StringFormat sf = new StringFormat
                    {
                        Alignment = StringAlignment.Center,
                        LineAlignment = StringAlignment.Center
                    };
                    g.DrawString("P", font, brush, new Rectangle(0, 0, 120, 120), sf);
                }
            }
            logoPictureBox.Image = bmp;
        }

        private async Task LoginAsync()
        {
            try
            {
                // Disable login button
                if (btnLogin != null)
                {
                    btnLogin.Enabled = false;
                    btnLogin.Text = "LOGGING IN...";
                }

                string username = txtUsername?.Text ?? "";
                string password = txtPassword?.Text ?? "";

                lblStatus.Text = "Logging in...";
                lblStatus.ForeColor = Color.Blue;

                if (_authService == null)
                {
                    lblStatus.Text = "Authentication service not available";
                    lblStatus.ForeColor = Color.Red;
                    return;
                }

                if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                {
                    lblStatus.Text = "Username and password are required";
                    lblStatus.ForeColor = Color.Red;
                    return;
                }

                var user = await _authService.AuthenticateAsync(username, password);

                if (user != null)
                {
                    Program.CurrentUser = user;
                    var dashboard = new frmMainDashboard();
                    dashboard.Show();
                    this.Hide();
                }
                else
                {
                    lblStatus.Text = "Invalid username or password";
                    lblStatus.ForeColor = Color.Red;
                }
            }
            catch (Exception ex)
            {
                if (lblStatus != null)
                {
                    lblStatus.Text = $"Login error: {ex.Message}";
                    lblStatus.ForeColor = Color.Red;
                }
                MessageBox.Show($"Login error: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // Re-enable login button using stored reference
                if (btnLogin != null)
                {
                    btnLogin.Enabled = true;
                    btnLogin.Text = "LOGIN";
                }
            }
        }
    }
}