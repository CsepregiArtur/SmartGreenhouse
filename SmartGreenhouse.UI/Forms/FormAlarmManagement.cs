using System;
using System.Drawing;
using System.Windows.Forms;
using SmartGreenhouse.DAL;
using SmartGreenhouse.Models.Entities;
using System.Data;
using Microsoft.Data.SqlClient;
using SmartGreenhouse.DAL.Repositories;

namespace SmartGreenhouse.UI.Forms
{
    public partial class FormAlarmManagement : Form
    {
        private DataGridView dgvAlerts;
        private Button btnAdd;
        private Button btnEdit;
        private Button btnDelete;
        private Button btnAcknowledge;
        private DatabaseHelper _dbHelper;
        private AlertRepository _alertRepo;
        private User _currentUser;

        public FormAlarmManagement(User currentUser)
        {
            _currentUser = currentUser;
            _dbHelper = new DatabaseHelper();
            _alertRepo = new AlertRepository();
            InitializeComponent();
            LoadAlerts();
        }

        private void InitializeComponent()
        {
            this.Text = "Gestionare Alarme";
            this.Size = new Size(1000, 600);
            this.StartPosition = FormStartPosition.CenterParent;

            // DataGridView
            dgvAlerts = new DataGridView
            {
                Location = new Point(20, 20),
                Size = new Size(950, 450),
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };
            this.Controls.Add(dgvAlerts);

            // Buttons
            btnAdd = new Button
            {
                Text = "Adaugă Alarmă",
                Location = new Point(20, 490),
                Size = new Size(120, 35)
            };
            btnAdd.Click += BtnAdd_Click;
            this.Controls.Add(btnAdd);

            btnEdit = new Button
            {
                Text = "Editează",
                Location = new Point(160, 490),
                Size = new Size(120, 35)
            };
            btnEdit.Click += BtnEdit_Click;
            this.Controls.Add(btnEdit);

            btnDelete = new Button
            {
                Text = "Șterge",
                Location = new Point(300, 490),
                Size = new Size(120, 35)
            };
            btnDelete.Click += BtnDelete_Click;
            this.Controls.Add(btnDelete);

            btnAcknowledge = new Button
            {
                Text = "Confirmă",
                Location = new Point(440, 490),
                Size = new Size(120, 35)
            };
            btnAcknowledge.Click += BtnAcknowledge_Click;
            this.Controls.Add(btnAcknowledge);
        }

        private void LoadAlerts()
        {
            try
            {
                var alerts = _alertRepo.GetAllAlerts();
                dgvAlerts.DataSource = alerts.Select(a => new
                {
                    a.AlertId,
                    SensorName = a.Sensor?.Name ?? "N/A",
                    Level = a.Level.ToString(),
                    a.Message,
                    a.Value,
                    a.Timestamp,
                    a.IsAcknowledged
                }).ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Eroare la încărcarea alarmelor: {ex.Message}", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            // using (var form = new FormAlarmEdit(null))
            // {
            //     if (form.ShowDialog() == DialogResult.OK)
            //     {
            //         LoadAlerts();
            //     }
            // }
            MessageBox.Show("Adăugare alarmă personalizată în dezvoltare.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            if (dgvAlerts.SelectedRows.Count == 0) return;

            var selectedRow = dgvAlerts.SelectedRows[0];
            var alertId = (int)selectedRow.Cells["AlertId"].Value;

            var alert = _alertRepo.GetAlertById(alertId);
            if (alert != null)
            {
                using (var form = new FormAlarmEdit(alert))
                {
                    if (form.ShowDialog() == DialogResult.OK)
                    {
                        LoadAlerts();
                    }
                }
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (dgvAlerts.SelectedRows.Count == 0) return;

            var selectedRow = dgvAlerts.SelectedRows[0];
            var alertId = (int)selectedRow.Cells["AlertId"].Value;

            if (MessageBox.Show("Sigur ștergeți alarma?", "Confirmare", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                try
                {
                    _alertRepo.DeleteAlert(alertId);
                    LoadAlerts();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Eroare la ștergere: {ex.Message}", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void BtnAcknowledge_Click(object sender, EventArgs e)
        {
            if (dgvAlerts.SelectedRows.Count == 0) return;

            var selectedRow = dgvAlerts.SelectedRows[0];
            var alertId = (int)selectedRow.Cells["AlertId"].Value;

            try
            {
                _alertRepo.AcknowledgeAlert(alertId, _currentUser.UserId);
                LoadAlerts();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Eroare la confirmare: {ex.Message}", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}