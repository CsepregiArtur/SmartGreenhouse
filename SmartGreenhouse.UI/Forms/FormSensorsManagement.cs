using System;
using System.Drawing;
using System.Windows.Forms;
using SmartGreenhouse.Models.Entities;
using SmartGreenhouse.DAL.Repositories;
using System.Linq;

namespace SmartGreenhouse.UI.Forms
{
    public partial class FormSensorsManagement : Form
    {
        private readonly User _currentUser;
        private readonly SensorRepository _sensorRepo;
        private DataGridView dgvSensors;

        public FormSensorsManagement(User user)
        {
            _currentUser = user;
            _sensorRepo = new SensorRepository();
            InitializeComponent();
            LoadSensors();
        }

        private void InitializeComponent()
        {
            this.Text = "Gestionare Senzori";
            this.Size = new Size(1000, 600);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.White;

            // Toolbar
            ToolStrip toolStrip = new ToolStrip();
            toolStrip.Items.Add(new ToolStripButton("➕ Adaugă", null, AddSensor_Click));
            toolStrip.Items.Add(new ToolStripButton("✏️ Editează", null, EditSensor_Click));
            toolStrip.Items.Add(new ToolStripButton("🗑️ Șterge", null, DeleteSensor_Click));
            toolStrip.Items.Add(new ToolStripSeparator());
            toolStrip.Items.Add(new ToolStripButton("🔄 Reîmprospătare", null, Refresh_Click));

            this.Controls.Add(toolStrip);

            // DataGridView
            dgvSensors = new DataGridView
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

            this.Controls.Add(dgvSensors);
        }

        private void LoadSensors()
        {
            var sensors = _sensorRepo.GetAllSensors();
            
            dgvSensors.DataSource = null;
            dgvSensors.DataSource = sensors.Select(s => new
            {
                s.SensorId,
                s.Name,
                Type = s.Type.ToString(),
                s.Unit,
                s.Location,
                s.MinValue,
                s.MaxValue,
                s.WarningLow,
                s.WarningHigh,
                s.IsActive
            }).ToList();

            // Formatare coloane
            dgvSensors.Columns["SensorId"].HeaderText = "ID";
            dgvSensors.Columns["Name"].HeaderText = "Nume";
            dgvSensors.Columns["Type"].HeaderText = "Tip";
            dgvSensors.Columns["Unit"].HeaderText = "Unitate";
            dgvSensors.Columns["Location"].HeaderText = "Locație";
            dgvSensors.Columns["MinValue"].HeaderText = "Min";
            dgvSensors.Columns["MaxValue"].HeaderText = "Max";
            dgvSensors.Columns["WarningLow"].HeaderText = "Avertizare Jos";
            dgvSensors.Columns["WarningHigh"].HeaderText = "Avertizare Sus";
            dgvSensors.Columns["IsActive"].HeaderText = "Activ";
        }

        private void AddSensor_Click(object sender, EventArgs e)
        {
            using (var form = new FormSensorEdit())
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    form.Sensor.CreatedByUserId = _currentUser.UserId;
                    
                    if (_sensorRepo.AddSensor(form.Sensor))
                    {
                        MessageBox.Show("Senzor adăugat cu succes!", "Succes",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadSensors();
                    }
                    else
                    {
                        MessageBox.Show("Eroare la adăugarea senzorului!", "Eroare",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void EditSensor_Click(object sender, EventArgs e)
        {
            if (dgvSensors.SelectedRows.Count == 0)
            {
                MessageBox.Show("Selectați un senzor pentru editare!", "Atenție",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int sensorId = (int)dgvSensors.SelectedRows[0].Cells["SensorId"].Value;
            var sensor = _sensorRepo.GetSensorById(sensorId);

            if (sensor != null)
            {
                using (var form = new FormSensorEdit(sensor))
                {
                    if (form.ShowDialog() == DialogResult.OK)
                    {
                        if (_sensorRepo.UpdateSensor(form.Sensor))
                        {
                            MessageBox.Show("Senzor actualizat cu succes!", "Succes",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                            LoadSensors();
                        }
                        else
                        {
                            MessageBox.Show("Eroare la actualizarea senzorului!", "Eroare",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
        }

        private void DeleteSensor_Click(object sender, EventArgs e)
        {
            if (dgvSensors.SelectedRows.Count == 0)
            {
                MessageBox.Show("Selectați un senzor pentru ștergere!", "Atenție",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var result = MessageBox.Show("Sigur doriți să ștergeți acest senzor?",
                "Confirmare ștergere", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                int sensorId = (int)dgvSensors.SelectedRows[0].Cells["SensorId"].Value;
                
                if (_sensorRepo.DeleteSensor(sensorId))
                {
                    MessageBox.Show("Senzor șters cu succes!", "Succes",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadSensors();
                }
                else
                {
                    MessageBox.Show("Eroare la ștergerea senzorului!", "Eroare",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void Refresh_Click(object sender, EventArgs e)
        {
            LoadSensors();
        }
    }
}