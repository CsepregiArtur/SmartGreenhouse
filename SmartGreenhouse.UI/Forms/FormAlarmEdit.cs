using System;
using System.Drawing;
using System.Windows.Forms;
using SmartGreenhouse.Models.Entities;
using SmartGreenhouse.DAL.Repositories;
using System.Linq;

namespace SmartGreenhouse.UI.Forms
{
    public partial class FormAlarmEdit : Form
    {
        private readonly Alert _alarm;
        private readonly SensorRepository _sensorRepo;
        private readonly AlertRepository _alertRepo;
        private ComboBox cmbSensor;
        private ComboBox cmbLevel;
        private TextBox txtMessage;
        private TextBox txtValue;
        private DateTimePicker dtpTimestamp;
        private Button btnSave;
        private Button btnCancel;
        private Label lblSensor;
        private Label lblLevel;
        private Label lblMessage;
        private Label lblValue;
        private Label lblTimestamp;

        public FormAlarmEdit(Alert alarm = null)
        {
            _alarm = alarm;
            _sensorRepo = new SensorRepository();
            _alertRepo = new AlertRepository();
            InitializeComponent();
            LoadData();
        }

        private void InitializeComponent()
        {
            this.Text = _alarm == null ? "Adăugare Alarmă" : "Editare Alarmă";
            this.Size = new Size(400, 350);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            int y = 20;
            int labelWidth = 100;
            int controlWidth = 250;
            int height = 25;
            int spacing = 35;

            // Sensor
            lblSensor = new Label
            {
                Text = "Senzor:",
                Location = new Point(20, y),
                Size = new Size(labelWidth, height)
            };
            this.Controls.Add(lblSensor);

            cmbSensor = new ComboBox
            {
                Location = new Point(130, y),
                Size = new Size(controlWidth, height),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            this.Controls.Add(cmbSensor);

            y += spacing;

            // Level
            lblLevel = new Label
            {
                Text = "Nivel:",
                Location = new Point(20, y),
                Size = new Size(labelWidth, height)
            };
            this.Controls.Add(lblLevel);

            cmbLevel = new ComboBox
            {
                Location = new Point(130, y),
                Size = new Size(controlWidth, height),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbLevel.Items.AddRange(new object[] { "Info", "Warning", "Critical" });
            this.Controls.Add(cmbLevel);

            y += spacing;

            // Message
            lblMessage = new Label
            {
                Text = "Mesaj:",
                Location = new Point(20, y),
                Size = new Size(labelWidth, height)
            };
            this.Controls.Add(lblMessage);

            txtMessage = new TextBox
            {
                Location = new Point(130, y),
                Size = new Size(controlWidth, height),
                Multiline = true,
                Height = 60
            };
            this.Controls.Add(txtMessage);

            y += 70;

            // Value
            lblValue = new Label
            {
                Text = "Valoare:",
                Location = new Point(20, y),
                Size = new Size(labelWidth, height)
            };
            this.Controls.Add(lblValue);

            txtValue = new TextBox
            {
                Location = new Point(130, y),
                Size = new Size(controlWidth, height)
            };
            this.Controls.Add(txtValue);

            y += spacing;

            // Timestamp
            lblTimestamp = new Label
            {
                Text = "Timestamp:",
                Location = new Point(20, y),
                Size = new Size(labelWidth, height)
            };
            this.Controls.Add(lblTimestamp);

            dtpTimestamp = new DateTimePicker
            {
                Location = new Point(130, y),
                Size = new Size(controlWidth, height),
                Format = DateTimePickerFormat.Custom,
                CustomFormat = "dd/MM/yyyy HH:mm:ss"
            };
            this.Controls.Add(dtpTimestamp);

            y += spacing + 20;

            // Buttons
            btnSave = new Button
            {
                Text = "Salvează",
                Location = new Point(130, y),
                Size = new Size(80, 30),
                DialogResult = DialogResult.OK
            };
            btnSave.Click += BtnSave_Click;
            this.Controls.Add(btnSave);

            btnCancel = new Button
            {
                Text = "Anulează",
                Location = new Point(220, y),
                Size = new Size(80, 30),
                DialogResult = DialogResult.Cancel
            };
            this.Controls.Add(btnCancel);
        }

        private void LoadData()
        {
            try
            {
                // Load sensors
                var sensors = _sensorRepo.GetAllSensors();
                cmbSensor.Items.Add(new { Id = (int?)null, Name = "(Niciun senzor)" });
                foreach (var sensor in sensors)
                {
                    cmbSensor.Items.Add(new { Id = (int?)sensor.SensorId, Name = sensor.Name });
                }
                cmbSensor.DisplayMember = "Name";
                cmbSensor.ValueMember = "Id";

                if (_alarm != null)
                {
                    // Edit mode
                    cmbSensor.SelectedValue = _alarm.SensorId;
                    cmbLevel.SelectedItem = _alarm.Level.ToString();
                    txtMessage.Text = _alarm.Message;
                    txtValue.Text = _alarm.Value?.ToString();
                    dtpTimestamp.Value = _alarm.Timestamp;
                }
                else
                {
                    // Add mode
                    cmbSensor.SelectedIndex = 0;
                    cmbLevel.SelectedIndex = 0;
                    dtpTimestamp.Value = DateTime.Now;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Eroare la încărcarea datelor: {ex.Message}", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtMessage.Text))
                {
                    MessageBox.Show("Mesajul este obligatoriu!", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Validation
                double? value = null;
                if (!string.IsNullOrWhiteSpace(txtValue.Text))
                {
                    if (!double.TryParse(txtValue.Text, out double parsedValue))
                    {
                        MessageBox.Show("Valoarea trebuie să fie un număr valid!", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    value = parsedValue;
                }

                // Create or update alert
                if (_alarm == null)
                {
                    // Add new alert
                    var newAlert = new Alert
                    {
                        SensorId = (cmbSensor.SelectedItem as dynamic)?.Id,
                        Level = (AlertLevel)Enum.Parse(typeof(AlertLevel), cmbLevel.SelectedItem.ToString()),
                        Message = txtMessage.Text,
                        Value = value,
                        Timestamp = dtpTimestamp.Value,
                        IsAcknowledged = false
                    };

                    _alertRepo.AddAlert(newAlert);
                    MessageBox.Show("Alarmă adăugată cu succes!", "Succes", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    // Update existing alert
                    _alarm.SensorId = (cmbSensor.SelectedItem as dynamic)?.Id;
                    _alarm.Level = (AlertLevel)Enum.Parse(typeof(AlertLevel), cmbLevel.SelectedItem.ToString());
                    _alarm.Message = txtMessage.Text;
                    _alarm.Value = value;
                    _alarm.Timestamp = dtpTimestamp.Value;

                    _alertRepo.UpdateAlert(_alarm);
                    MessageBox.Show("Alarmă actualizată cu succes!", "Succes", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Eroare la salvare: {ex.Message}", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}