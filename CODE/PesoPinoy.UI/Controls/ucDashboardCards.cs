using System;
using System.Drawing;
using System.Windows.Forms;

namespace PesoPinoy.UI.Controls
{
    public partial class ucDashboardCards : UserControl
    {
        private Panel cardPanel;
        private Label lblTitle;
        private Label lblValue;
        private Label lblIcon;

        public ucDashboardCards()
        {
            InitializeComponent();
            this.Size = new Size(250, 120);
            CreateCard();
        }

        private void CreateCard()
        {
            cardPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(2, 58, 128),
                Padding = new Padding(10)
            };

            lblIcon = new Label
            {
                Location = new Point(10, 10),
                Size = new Size(40, 40),
                Font = new Font("Segoe UI", 20),
                ForeColor = Color.White,
                Text = "📊"
            };

            lblTitle = new Label
            {
                Location = new Point(60, 15),
                Size = new Size(170, 25),
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.White
            };

            lblValue = new Label
            {
                Location = new Point(60, 45),
                Size = new Size(170, 40),
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                ForeColor = Color.White,
                TextAlign = ContentAlignment.MiddleLeft
            };

            cardPanel.Controls.Add(lblIcon);
            cardPanel.Controls.Add(lblTitle);
            cardPanel.Controls.Add(lblValue);
            this.Controls.Add(cardPanel);
        }

        public void SetCard(string title, string value, string icon, Color color)
        {
            lblTitle.Text = title;
            lblValue.Text = value;
            lblIcon.Text = icon;
            cardPanel.BackColor = color;
        }

        public void UpdateValue(string value)
        {
            lblValue.Text = value;
        }

    }
}