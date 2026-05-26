using System;
using System.Drawing;
using System.Windows.Forms;
using SmartGreenhouse.Models.Entities;

namespace SmartGreenhouse.UI.Forms
{
    public partial class FormSensorEdit : Form
    {
        public Sensor Sensor { get; private set; }

        private TextBox txtName;
        private ComboBox cmbType;
        private TextBox txtUnit;
        private TextBox txtLocation;
        private NumericUpDown nudMinValue;
        private NumericUpDown nudMaxValue;
        private NumericUpDown nudWarningLow;
        private NumericUpDown nudWarningHigh;
        private NumericUpDown nudCriticalLow;
        private NumericUpDown nudCriticalHigh;

        public FormSensorEdit(Sensor sensor = null)
        {
            Sensor = sensor ?? new Sensor { IsActive = true, InstalledDate = DateTime.Now };
            InitializeComponent();
            if (sensor != null)
            {
                LoadSensorData();
            }
        }

        private void InitializeComponent()
        {
            this.Text = Sensor?.SensorId > 0 ? "Editare Senzor" : "Adăugare Senzor Nou";
            this.Size = new Size(500, 500);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            TableLayoutPanel layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                Padding = new Padding(20),
                BackColor = Color.White
            };
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30F));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70F));

            int row = 0;

            // Nume
            AddLabel(layout, "Nume Senzor:", row, 0);
            txtName = new TextBox { Dock = DockStyle.Fill };
            layout.Controls.Add(txtName, 1, row++);

            // Tip
            AddLabel(layout, "Tip Senzor:", row, 0);
            cmbType = new ComboBox 
            { 
                Dock = DockStyle.Fill,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbType.DataSource = Enum.GetValues(typeof(SensorType));
            layout.Controls.Add(cmbType, 1, row++);

            // Unitate
            AddLabel(layout, "Unitate Măsură:", row, 0);
            txtUnit = new TextBox { Dock = DockStyle.Fill };
            layout.Controls.Add(txtUnit, 1, row++);

            // Locație
            AddLabel(layout, "Locație:", row, 0);
            txtLocation = new TextBox { Dock = DockStyle.Fill };
            layout.Controls.Add(txtLocation, 1, row++);

            // Valoare minimă
            AddLabel(layout, "Valoare Minimă:", row, 0);
            nudMinValue = new NumericUpDown 
            { 
                Dock = DockStyle.Fill,
                DecimalPlaces = 1,
                Minimum = -100,
                Maximum = 1000
            };
            layout.Controls.Add(nudMinValue, 1, row++);

            // Valoare maximă
            AddLabel(layout, "Valoare Maximă:", row, 0);
            nudMaxValue = new NumericUpDown 
            { 
                Dock = DockStyle.Fill,
                DecimalPlaces = 1,
                Minimum = -100,
                Maximum = 1000
            };
            layout.Controls.Add(nudMaxValue, 1, row++);

            // Prag avertizare jos
            AddLabel(layout, "Avertizare Jos:", row, 0);
            nudWarningLow = new NumericUpDown 
            { 
                Dock = DockStyle.Fill,
                DecimalPlaces = 1,
                Minimum = -100,
                Maximum = 1000
            };
            layout.Controls.Add(nudWarningLow, 1, row++);

            // Prag avertizare sus
            AddLabel(layout, "Avertizare Sus:", row, 0);
            nudWarningHigh = new NumericUpDown 
            { 
                Dock = DockStyle.Fill,
                DecimalPlaces = 1,
                Minimum = -100,
                Maximum = 1000
            };
            layout.Controls.Add(nudWarningHigh, 1, row++);

            // Prag critic jos
            AddLabel(layout, "Critic Jos:", row, 0);
            nudCriticalLow = new NumericUpDown 
            { 
                Dock = DockStyle.Fill,
                DecimalPlaces = 1,
                Minimum = -100,
                Maximum = 1000
            };
            layout.Controls.Add(nudCriticalLow, 1, row++);

            // Prag critic sus
            AddLabel(layout, "Critic Sus:", row, 0);
            nudCriticalHigh = new NumericUpDown 
            { 
                Dock = DockStyle.Fill,
                DecimalPlaces = 1,
                Minimum = -100,
                Maximum = 1000
            };
            layout.Controls.Add(nudCriticalHigh, 1, row++);

            // Buttons
            FlowLayoutPanel pnlButtons = new FlowLayoutPanel
            {
                Dock = DockStyle.Bottom,
                Height = 50,
                FlowDirection = FlowDirection.RightToLeft,
                Padding = new Padding(10)
            };

            Button btnCancel = new Button
            {
                Text = "Anulează",
                Size = new Size(100, 35),
                DialogResult = DialogResult.Cancel
            };

            Button btnSave = new Button
            {
                Text = "Salvează",
                Size = new Size(100, 35),
                DialogResult = DialogResult.OK,
                BackColor = Color.FromArgb(0, 150, 136),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnSave.Click += BtnSave_Click;

            pnlButtons.Controls.Add(btnSave);
            pnlButtons.Controls.Add(btnCancel);

            this.Controls.Add(layout);
            this.Controls.Add(pnlButtons);
        }

        private void AddLabel(TableLayoutPanel layout, string text, int row, int col)
        {
            Label label = new Label
            {
                Text = text,
                TextAlign = ContentAlignment.MiddleLeft,
                Anchor = AnchorStyles.Left,
                Font = new Font("Segoe UI", 10)
            };
            layout.Controls.Add(label, col, row);
        }

        private void LoadSensorData()
        {
            txtName.Text = Sensor.Name;
            cmbType.SelectedItem = Sensor.Type;
            txtUnit.Text = Sensor.Unit;
            txtLocation.Text = Sensor.Location;
            nudMinValue.Value = (decimal)Sensor.MinValue;
            nudMaxValue.Value = (decimal)Sensor.MaxValue;
            nudWarningLow.Value = (decimal)Sensor.WarningLow;
            nudWarningHigh.Value = (decimal)Sensor.WarningHigh;
            nudCriticalLow.Value = (decimal)Sensor.CriticalLow;
            nudCriticalHigh.Value = (decimal)Sensor.CriticalHigh;
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            // Validări
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Numele senzorului este obligatoriu!", "Eroare",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.DialogResult = DialogResult.None;
                return;
            }

            if (nudMinValue.Value >= nudMaxValue.Value)
            {
                MessageBox.Show("Valoarea minimă trebuie să fie mai mică decât cea maximă!", "Eroare",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.DialogResult = DialogResult.None;
                return;
            }

            // Salvare date
            Sensor.Name = txtName.Text;
            Sensor.Type = (SensorType)cmbType.SelectedItem;
            Sensor.Unit = txtUnit.Text;
            Sensor.Location = txtLocation.Text;
            Sensor.MinValue = (double)nudMinValue.Value;
            Sensor.MaxValue = (double)nudMaxValue.Value;
            Sensor.WarningLow = (double)nudWarningLow.Value;
            Sensor.WarningHigh = (double)nudWarningHigh.Value;
            Sensor.CriticalLow = (double)nudCriticalLow.Value;
            Sensor.CriticalHigh = (double)nudCriticalHigh.Value;
        }
    }
}