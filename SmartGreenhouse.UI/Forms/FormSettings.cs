using System;
using System.Drawing;
using System.Windows.Forms;
using SmartGreenhouse.DAL;

namespace SmartGreenhouse.UI.Forms
{
    public partial class FormSettings : Form
    {
        private TextBox txtConnectionString;
        private Button btnSave;
        private Button btnCancel;
        private CheckBox chkAutoSimulation;

        public FormSettings()
        {
            InitializeComponent();
            LoadCurrentSettings();
        }

        private void InitializeComponent()
        {
            this.Text = "Setări Sistem";
            this.Size = new Size(500, 400);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // Connection String
            Label lblConnection = new Label
            {
                Text = "Connection String:",
                Location = new Point(20, 20),
                Size = new Size(150, 25),
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            this.Controls.Add(lblConnection);

            txtConnectionString = new TextBox
            {
                Location = new Point(20, 50),
                Size = new Size(450, 25),
                Font = new Font("Segoe UI", 9),
                ReadOnly = true // For display only, or make editable
            };
            this.Controls.Add(txtConnectionString);

            // Auto Simulation
            chkAutoSimulation = new CheckBox
            {
                Text = "Pornire automată simulare la start",
                Location = new Point(20, 100),
                Size = new Size(250, 25),
                Font = new Font("Segoe UI", 10),
                Checked = true
            };
            this.Controls.Add(chkAutoSimulation);

            // Buttons
            btnSave = new Button
            {
                Text = "Salvează",
                Location = new Point(200, 320),
                Size = new Size(100, 35),
                Font = new Font("Segoe UI", 10),
                DialogResult = DialogResult.OK
            };
            btnSave.Click += BtnSave_Click;
            this.Controls.Add(btnSave);

            btnCancel = new Button
            {
                Text = "Anulează",
                Location = new Point(320, 320),
                Size = new Size(100, 35),
                Font = new Font("Segoe UI", 10),
                DialogResult = DialogResult.Cancel
            };
            this.Controls.Add(btnCancel);

            this.AcceptButton = btnSave;
            this.CancelButton = btnCancel;
        }

        private void LoadCurrentSettings()
        {
            // Load connection string from DatabaseHelper or config
            txtConnectionString.Text = DatabaseHelper.GetConnectionString();
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            // Save settings
            // For now, just show message
            MessageBox.Show("Setări salvate! (Funcționalitate de bază)", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}