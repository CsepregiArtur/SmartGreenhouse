using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using SmartGreenhouse.DAL.Repositories;
using SmartGreenhouse.Models.Entities;

namespace SmartGreenhouse.UI.Forms
{
    public partial class FormUserManagement : Form
    {
        private readonly UserRepository _userRepo;
        private DataGridView dgvUsers;

        public FormUserManagement()
        {
            _userRepo = new UserRepository();
            InitializeComponent();
            LoadUsers();
        }

        private void InitializeComponent()
        {
            this.Text = "Gestionare Utilizatori";
            this.Size = new Size(900, 600);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.White;

            ToolStrip toolStrip = new ToolStrip();
            toolStrip.Items.Add(new ToolStripButton("➕ Adaugă", null, AddUser_Click));
            toolStrip.Items.Add(new ToolStripButton("✏️ Editează", null, EditUser_Click));
            toolStrip.Items.Add(new ToolStripButton("🗑️ Deactivează", null, DeleteUser_Click));
            toolStrip.Items.Add(new ToolStripSeparator());
            toolStrip.Items.Add(new ToolStripButton("🔄 Reîmprospătare", null, Refresh_Click));

            this.Controls.Add(toolStrip);

            dgvUsers = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                ReadOnly = true,
                AllowUserToAddRows = false,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                RowHeadersVisible = false
            };

            this.Controls.Add(dgvUsers);
        }

        private void LoadUsers()
        {
            var users = _userRepo.GetAllUsers();
            dgvUsers.DataSource = null;
            dgvUsers.DataSource = users.Select(u => new
            {
                u.UserId,
                u.Username,
                Role = u.Role.ToString(),
                u.Email,
                u.IsActive,
                LastLogin = u.LastLogin?.ToString("g") ?? "-"
            }).ToList();

            dgvUsers.Columns["UserId"].HeaderText = "ID";
            dgvUsers.Columns["Username"].HeaderText = "Username";
            dgvUsers.Columns["Role"].HeaderText = "Rol";
            dgvUsers.Columns["Email"].HeaderText = "Email";
            dgvUsers.Columns["IsActive"].HeaderText = "Activ";
            dgvUsers.Columns["LastLogin"].HeaderText = "Ultima autentificare";
        }

        private void AddUser_Click(object sender, EventArgs e)
        {
            using (var form = new FormUserEdit())
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    var user = form.User;
                    string passwordHash = form.PasswordHash;

                    if (_userRepo.AddUser(user, passwordHash))
                    {
                        MessageBox.Show("Utilizator adăugat cu succes!", "Succes", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadUsers();
                    }
                    else
                    {
                        MessageBox.Show("Eroare la adăugarea utilizatorului.", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void EditUser_Click(object sender, EventArgs e)
        {
            if (dgvUsers.SelectedRows.Count == 0)
            {
                MessageBox.Show("Selectați un utilizator pentru editare!", "Atenție", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int userId = (int)dgvUsers.SelectedRows[0].Cells["UserId"].Value;
            var user = _userRepo.GetAllUsers().FirstOrDefault(u => u.UserId == userId);

            if (user == null)
                return;

            using (var form = new FormUserEdit(user))
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    var updatedUser = form.User;
                    if (_userRepo.UpdateUser(updatedUser))
                    {
                        if (!string.IsNullOrEmpty(form.PasswordHash))
                        {
                            _userRepo.UpdatePassword(updatedUser.UserId, form.PasswordHash);
                        }

                        MessageBox.Show("Utilizator actualizat cu succes!", "Succes", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadUsers();
                    }
                    else
                    {
                        MessageBox.Show("Eroare la actualizarea utilizatorului.", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void DeleteUser_Click(object sender, EventArgs e)
        {
            if (dgvUsers.SelectedRows.Count == 0)
            {
                MessageBox.Show("Selectați un utilizator pentru dezactivare!", "Atenție", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int userId = (int)dgvUsers.SelectedRows[0].Cells["UserId"].Value;

            if (MessageBox.Show("Sigur doriți să dezactivați acest utilizator?", "Confirmare", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                if (_userRepo.DeleteUser(userId))
                {
                    MessageBox.Show("Utilizator dezactivat cu succes!", "Succes", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadUsers();
                }
                else
                {
                    MessageBox.Show("Eroare la dezactivarea utilizatorului.", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void Refresh_Click(object sender, EventArgs e)
        {
            LoadUsers();
        }
    }
}
