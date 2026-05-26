using System;
using System.Drawing;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using SmartGreenhouse.Models.Entities;

namespace SmartGreenhouse.UI.Forms
{
    public partial class FormUserEdit : Form
    {
        public User User { get; private set; }
        public string PasswordHash { get; private set; }

        private TextBox txtUsername;
        private TextBox txtEmail;
        private ComboBox cmbRole;
        private CheckBox chkIsActive;
        private TextBox txtPassword;

        public FormUserEdit(User user = null)
        {
            User = user ?? new User { IsActive = true, Role = UserRole.Operator };
            InitializeComponent();
            LoadData();
        }

        private void InitializeComponent()
        {
            this.Text = User.UserId == 0 ? "Adaugă utilizator" : "Editează utilizator";
            this.Size = new Size(450, 380);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.White;

            var lblUsername = new Label { Text = "Username:", Location = new Point(20, 20), Size = new Size(100, 25) };
            txtUsername = new TextBox { Location = new Point(130, 20), Size = new Size(280, 25) };

            var lblEmail = new Label { Text = "Email:", Location = new Point(20, 60), Size = new Size(100, 25) };
            txtEmail = new TextBox { Location = new Point(130, 60), Size = new Size(280, 25) };

            var lblRole = new Label { Text = "Rol:", Location = new Point(20, 100), Size = new Size(100, 25) };
            cmbRole = new ComboBox { Location = new Point(130, 100), Size = new Size(280, 25), DropDownStyle = ComboBoxStyle.DropDownList };
            cmbRole.Items.AddRange(new[] { "Admin", "Operator", "Viewer" });

            var lblActive = new Label { Text = "Activ:", Location = new Point(20, 140), Size = new Size(100, 25) };
            chkIsActive = new CheckBox { Location = new Point(130, 140), Size = new Size(20, 25) };

            var lblPassword = new Label { Text = "Parolă:", Location = new Point(20, 180), Size = new Size(100, 25) };
            txtPassword = new TextBox { Location = new Point(130, 180), Size = new Size(280, 25), UseSystemPasswordChar = true };
            var lblPasswordHint = new Label
            {
                Text = "(lăsați gol pentru a păstra parola curentă)",
                ForeColor = Color.Gray,
                Location = new Point(130, 210),
                Size = new Size(280, 20)
            };

            var btnSave = new Button
            {
                Text = "Salvează",
                Location = new Point(130, 250),
                Size = new Size(120, 40),
                BackColor = Color.FromArgb(0, 150, 136),
                ForeColor = Color.White
            };
            btnSave.Click += BtnSave_Click;

            var btnCancel = new Button
            {
                Text = "Anulează",
                Location = new Point(290, 250),
                Size = new Size(120, 40),
                BackColor = Color.LightGray,
                ForeColor = Color.Black
            };
            btnCancel.Click += (s, e) => this.DialogResult = DialogResult.Cancel;

            this.Controls.Add(lblUsername);
            this.Controls.Add(txtUsername);
            this.Controls.Add(lblEmail);
            this.Controls.Add(txtEmail);
            this.Controls.Add(lblRole);
            this.Controls.Add(cmbRole);
            this.Controls.Add(lblActive);
            this.Controls.Add(chkIsActive);
            this.Controls.Add(lblPassword);
            this.Controls.Add(txtPassword);
            this.Controls.Add(lblPasswordHint);
            this.Controls.Add(btnSave);
            this.Controls.Add(btnCancel);
        }

        private void LoadData()
        {
            txtUsername.Text = User.Username;
            txtEmail.Text = User.Email;
            cmbRole.SelectedIndex = (int)User.Role - 1;
            chkIsActive.Checked = User.IsActive;
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUsername.Text))
            {
                MessageBox.Show("Username este obligatoriu.", "Atenție", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            User.Username = txtUsername.Text.Trim();
            User.Email = txtEmail.Text.Trim();
            User.Role = (UserRole)(cmbRole.SelectedIndex + 1);
            User.IsActive = chkIsActive.Checked;

            if (!string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                PasswordHash = ComputeHash(txtPassword.Text);
            }

            this.DialogResult = DialogResult.OK;
        }

        private string ComputeHash(string input)
        {
            using (var sha = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(input);
                var hash = sha.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }
    }
}
