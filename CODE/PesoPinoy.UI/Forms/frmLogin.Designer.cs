namespace PesoPinoy.UI.Forms
{
    partial class frmLogin
    {
        private System.ComponentModel.IContainer components = null;
        private Panel mainPanel;
        private Panel leftPanel;
        private Panel rightPanel;
        private Panel titleBar;
        private Button btnClose;
        private Label lblAppName;
        private Label lblSlogan;
        private Label lblSDG;
        private Label lblLogin;
        private Label lblUsername;
        private Label lblPassword;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(700, 500);
            this.Text = "PesoPinoy Login";
            this.StartPosition = FormStartPosition.CenterScreen;
        }
    }
}