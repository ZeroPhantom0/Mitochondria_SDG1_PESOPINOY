using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Extensions.DependencyInjection;
using PesoPinoy.BLL.Services;

namespace PesoPinoy.UI.Forms
{
    public partial class frmBackupRestore : Form
    {
        private readonly BackupService _backupService;
        private Panel headerPanel;
        private Panel backupPanel;
        private Panel restorePanel;
        private ProgressBar progressBar;
        private Label lblStatus;
        private RichTextBox txtLog;
        private string _inputDataPath;

        public frmBackupRestore()
        {
            InitializeComponent();
            _backupService = Program.ServiceProvider.GetService<BackupService>();

            // Set the correct INPUT_DATA path
            _inputDataPath = GetInputDataPath();

            this.BackColor = Color.White;
            this.Text = "PesoPinoy - Backup & Restore";
            this.Size = new Size(800, 600);
            this.StartPosition = FormStartPosition.CenterScreen;

            CreateHeader();
            CreateBackupSection();
            CreateRestoreSection();
            CreateLogSection();
        }

        private string GetInputDataPath()
        {
            // Start from the executable directory
            string currentDir = Application.StartupPath;

            // Navigate up to the project root
            // From: ...\CODE\PesoPinoy.UI\bin\Debug\
            // To:   ...\INPUT_DATA\
            DirectoryInfo dir = new DirectoryInfo(currentDir);

            // Go up to the project root (Mitochondria_SDG1_PESOPINOY)
            while (dir != null && !dir.Name.Equals("Mitochondria_SDG1_PESOPINOY", StringComparison.OrdinalIgnoreCase))
            {
                dir = dir.Parent;
            }

            if (dir != null)
            {
                return Path.Combine(dir.FullName, "INPUT_DATA");
            }

            // Fallback to a relative path
            return Path.Combine(Application.StartupPath, "..\\..\\..\\..\\INPUT_DATA");
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
                Text = "Backup & Restore",
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

        private void CreateBackupSection()
        {
            backupPanel = new Panel
            {
                Location = new Point(20, 80),
                Size = new Size(360, 150),
                BackColor = Color.FromArgb(240, 240, 240),
                BorderStyle = BorderStyle.FixedSingle
            };

            Label lblBackupTitle = new Label
            {
                Text = "Create Backup",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(2, 58, 128),
                Location = new Point(15, 10),
                AutoSize = true
            };

            Label lblBackupDesc = new Label
            {
                Text = "Export all system data to JSON file",
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.Black,
                Location = new Point(15, 35),
                AutoSize = true
            };

            Button btnBackup = new Button
            {
                Text = "CREATE BACKUP",
                Size = new Size(150, 40),
                Location = new Point(100, 85),
                BackColor = Color.FromArgb(2, 58, 128),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            btnBackup.FlatAppearance.BorderSize = 0;
            btnBackup.Click += async (s, e) => await PerformBackupAsync();

            backupPanel.Controls.AddRange(new Control[] {
                lblBackupTitle, lblBackupDesc, btnBackup
            });
            this.Controls.Add(backupPanel);
        }

        private void CreateRestoreSection()
        {
            restorePanel = new Panel
            {
                Location = new Point(400, 80),
                Size = new Size(360, 150),
                BackColor = Color.FromArgb(240, 240, 240),
                BorderStyle = BorderStyle.FixedSingle
            };

            Label lblRestoreTitle = new Label
            {
                Text = "Restore from Backup",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(2, 58, 128),
                Location = new Point(15, 10),
                AutoSize = true
            };

            Label lblRestoreDesc = new Label
            {
                Text = "Import data from JSON backup file",
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.Black,
                Location = new Point(15, 35),
                AutoSize = true
            };

            TextBox txtBackupPath = new TextBox
            {
                Location = new Point(15, 70),
                Size = new Size(220, 25),
                Font = new Font("Segoe UI", 9),
                BorderStyle = BorderStyle.FixedSingle
            };

            Button btnBrowse = new Button
            {
                Text = "Browse...",
                Size = new Size(80, 25),
                Location = new Point(240, 70),
                BackColor = Color.FromArgb(52, 152, 219),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnBrowse.FlatAppearance.BorderSize = 0;
            btnBrowse.Click += (s, e) =>
            {
                OpenFileDialog ofd = new OpenFileDialog
                {
                    Filter = "JSON files (*.json)|*.json",
                    InitialDirectory = _inputDataPath
                };
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    txtBackupPath.Text = ofd.FileName;
                }
            };

            Button btnRestore = new Button
            {
                Text = "RESTORE",
                Size = new Size(150, 35),
                Location = new Point(100, 105),
                BackColor = Color.FromArgb(46, 204, 113),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            btnRestore.FlatAppearance.BorderSize = 0;
            btnRestore.Click += async (s, e) => await PerformRestoreAsync(txtBackupPath.Text);

            restorePanel.Controls.AddRange(new Control[] {
                lblRestoreTitle, lblRestoreDesc, txtBackupPath, btnBrowse, btnRestore
            });
            this.Controls.Add(restorePanel);
        }

        private void CreateLogSection()
        {
            progressBar = new ProgressBar
            {
                Location = new Point(20, 250),
                Size = new Size(740, 20),
                Style = ProgressBarStyle.Marquee,
                Visible = false
            };
            this.Controls.Add(progressBar);

            lblStatus = new Label
            {
                Location = new Point(20, 280),
                Size = new Size(740, 25),
                ForeColor = Color.FromArgb(2, 58, 128),
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            this.Controls.Add(lblStatus);

            Label lblLog = new Label
            {
                Text = "Operation Log:",
                Location = new Point(20, 320),
                Size = new Size(100, 20),
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.FromArgb(2, 58, 128)
            };
            this.Controls.Add(lblLog);

            txtLog = new RichTextBox
            {
                Location = new Point(20, 350),
                Size = new Size(740, 200),
                Font = new Font("Consolas", 9),
                BackColor = Color.Black,
                ForeColor = Color.LightGreen,
                ReadOnly = true
            };
            this.Controls.Add(txtLog);
        }

        private async Task PerformBackupAsync()
        {
            try
            {
                progressBar.Visible = true;
                lblStatus.Text = "Creating backup...";
                txtLog.Clear();

                // Ensure the INPUT_DATA directory exists
                if (!Directory.Exists(_inputDataPath))
                {
                    Directory.CreateDirectory(_inputDataPath);
                    txtLog.AppendText($"[{DateTime.Now:HH:mm:ss}] Created directory: {_inputDataPath}\n");
                }

                string backupPath = Path.Combine(_inputDataPath,
                    $"backup_{DateTime.Now:yyyyMMdd_HHmmss}.json");

                txtLog.AppendText($"[{DateTime.Now:HH:mm:ss}] Starting database backup...\n");
                txtLog.AppendText($"[{DateTime.Now:HH:mm:ss}] Target: {backupPath}\n");

                await _backupService.BackupDataAsync(backupPath, (message) =>
                {
                    this.Invoke(new Action(() =>
                    {
                        txtLog.AppendText($"[{DateTime.Now:HH:mm:ss}] {message}\n");
                    }));
                });

                lblStatus.Text = $"Backup completed successfully!";
                txtLog.AppendText($"[{DateTime.Now:HH:mm:ss}] Backup completed successfully!\n");

                MessageBox.Show($"Backup created successfully!\n\nLocation: {backupPath}",
                    "Backup Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Backup failed!";
                txtLog.AppendText($"[{DateTime.Now:HH:mm:ss}] ERROR: {ex.Message}\n");
                MessageBox.Show($"Error creating backup: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                progressBar.Visible = false;
            }
        }

        private async Task PerformRestoreAsync(string backupPath)
        {
            if (string.IsNullOrEmpty(backupPath) || !File.Exists(backupPath))
            {
                MessageBox.Show("Please select a valid backup file.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var result = MessageBox.Show("Restoring will overwrite existing data. Continue?",
                "Confirm Restore", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                try
                {
                    progressBar.Visible = true;
                    lblStatus.Text = "Restoring from backup...";
                    txtLog.Clear();

                    txtLog.AppendText($"[{DateTime.Now:HH:mm:ss}] Starting database restore...\n");
                    txtLog.AppendText($"[{DateTime.Now:HH:mm:ss}] Source: {backupPath}\n");

                    await _backupService.RestoreDataAsync(backupPath, (message) =>
                    {
                        this.Invoke(new Action(() =>
                        {
                            txtLog.AppendText($"[{DateTime.Now:HH:mm:ss}] {message}\n");
                        }));
                    });

                    lblStatus.Text = "Restore completed successfully!";
                    txtLog.AppendText($"[{DateTime.Now:HH:mm:ss}] Restore completed successfully!\n");

                    MessageBox.Show("Data restored successfully!", "Restore Complete",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    lblStatus.Text = "Restore failed!";
                    txtLog.AppendText($"[{DateTime.Now:HH:mm:ss}] ERROR: {ex.Message}\n");
                    MessageBox.Show($"Error restoring data: {ex.Message}", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    progressBar.Visible = false;
                }
            }
        }
    }
}