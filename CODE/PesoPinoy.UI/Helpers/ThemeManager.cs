using System.Drawing;
using System.Windows.Forms;

namespace PesoPinoy.UI.Helpers
{
    public static class ThemeManager
    {
        public static Color PrimaryColor = Color.FromArgb(2, 58, 128);
        public static Color SecondaryColor = Color.White;
        public static Color SuccessColor = Color.FromArgb(46, 204, 113);
        public static Color InfoColor = Color.FromArgb(52, 152, 219);
        public static Color WarningColor = Color.FromArgb(241, 196, 15);
        public static Color DangerColor = Color.FromArgb(231, 76, 60);
        public static Color AccentColor = Color.FromArgb(155, 89, 182);

        public static void ApplyTheme(Form form)
        {
            form.BackColor = SecondaryColor;
            ApplyThemeToControls(form.Controls);
        }

        private static void ApplyThemeToControls(Control.ControlCollection controls)
        {
            foreach (Control control in controls)
            {
                if (control is Button button)
                {
                    if (button.BackColor == PrimaryColor || button.BackColor == Color.FromArgb(2, 58, 128))
                    {
                        button.FlatStyle = FlatStyle.Flat;
                        button.FlatAppearance.BorderSize = 0;
                    }
                }
                else if (control is Label label)
                {
                    if (label.ForeColor == PrimaryColor || label.ForeColor == Color.FromArgb(2, 58, 128))
                    {
                        // Keep primary color
                    }
                }
                else if (control is Panel || control is GroupBox)
                {
                    ApplyThemeToControls(control.Controls);
                }
            }
        }

        public static void StyleDataGridView(DataGridView dgv)
        {
            dgv.BackgroundColor = SecondaryColor;
            dgv.BorderStyle = BorderStyle.FixedSingle;
            dgv.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgv.GridColor = Color.LightGray;
            dgv.RowHeadersVisible = false;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            dgv.DefaultCellStyle.Font = new Font("Segoe UI", 9);
            dgv.DefaultCellStyle.ForeColor = Color.Black;
            dgv.DefaultCellStyle.BackColor = SecondaryColor;
            dgv.DefaultCellStyle.SelectionBackColor = PrimaryColor;
            dgv.DefaultCellStyle.SelectionForeColor = SecondaryColor;
            dgv.DefaultCellStyle.Padding = new Padding(5);

            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgv.ColumnHeadersDefaultCellStyle.BackColor = PrimaryColor;
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = SecondaryColor;
            dgv.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgv.ColumnHeadersHeight = 40;
            dgv.EnableHeadersVisualStyles = false;
        }

        public static Button CreateStyledButton(string text, Color color, int width = 100, int height = 35)
        {
            return new Button
            {
                Text = text,
                Size = new Size(width, height),
                BackColor = color,
                ForeColor = SecondaryColor,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                FlatAppearance = { BorderSize = 0 }
            };
        }

        public static TextBox CreateStyledTextBox(int width = 200)
        {
            return new TextBox
            {
                Size = new Size(width, 25),
                Font = new Font("Segoe UI", 9),
                BorderStyle = BorderStyle.FixedSingle
            };
        }

        public static ComboBox CreateStyledComboBox(int width = 200)
        {
            return new ComboBox
            {
                Size = new Size(width, 25),
                Font = new Font("Segoe UI", 9),
                DropDownStyle = ComboBoxStyle.DropDownList,
                FlatStyle = FlatStyle.Flat
            };
        }
    }
}