using System;
using System.Drawing;
using System.Windows.Forms;
using SmartGreenhouse.DAL;
using System.Linq;

namespace SmartGreenhouse.UI.Forms
{
    public partial class FormReports : Form
    {
        private DataGridView dgvReports;
        private ComboBox cmbReportType;
        private Button btnGenerate;
        private DatabaseHelper _dbHelper;

        public FormReports()
        {
            _dbHelper = new DatabaseHelper();
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Rapoarte Senzori";
            this.Size = new Size(800, 600);
            this.StartPosition = FormStartPosition.CenterParent;

            // Report Type
            Label lblType = new Label
            {
                Text = "Tip Raport:",
                Location = new Point(20, 20),
                Size = new Size(100, 25)
            };
            this.Controls.Add(lblType);

            cmbReportType = new ComboBox
            {
                Location = new Point(120, 20),
                Size = new Size(200, 25),
                Items = { "Citiri Recente", "Alarme", "Statistici" },
                SelectedIndex = 0
            };
            this.Controls.Add(cmbReportType);

            btnGenerate = new Button
            {
                Text = "Generează Raport",
                Location = new Point(350, 20),
                Size = new Size(120, 30)
            };
            btnGenerate.Click += BtnGenerate_Click;
            this.Controls.Add(btnGenerate);

            // DataGridView
            dgvReports = new DataGridView
            {
                Location = new Point(20, 70),
                Size = new Size(750, 480),
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ReadOnly = true
            };
            this.Controls.Add(dgvReports);

            // Load initial data
            LoadRecentReadings();
        }

        private void BtnGenerate_Click(object sender, EventArgs e)
        {
            switch (cmbReportType.SelectedIndex)
            {
                case 0:
                    LoadRecentReadings();
                    break;
                case 1:
                    LoadAlerts();
                    break;
                case 2:
                    LoadStatistics();
                    break;
            }
        }

        private void LoadRecentReadings()
        {
            try
            {
                var readings = _dbHelper.ExecuteQuery(@"
                    SELECT s.Name as SensorName, s.Type, s.Unit, r.Value, r.Timestamp, r.Quality
                    FROM Sensors s
                    OUTER APPLY (
                        SELECT TOP 1 Value, Timestamp, Quality
                        FROM SensorReadings
                        WHERE SensorId = s.SensorId
                        ORDER BY Timestamp DESC
                    ) r
                    WHERE s.IsActive = 1
                    ORDER BY r.Timestamp DESC");
                dgvReports.DataSource = readings;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Eroare la încărcarea citirilor: {ex.Message}", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadAlerts()
        {
            try
            {
                var alerts = _dbHelper.ExecuteQuery(@"
                    SELECT a.AlertId, s.Name as SensorName, a.Level, a.Message, a.Value, a.Timestamp, a.IsAcknowledged
                    FROM Alerts a
                    LEFT JOIN Sensors s ON a.SensorId = s.SensorId
                    ORDER BY a.Timestamp DESC");
                dgvReports.DataSource = alerts;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Eroare la încărcarea alarmelor: {ex.Message}", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadStatistics()
        {
            try
            {
                var stats = _dbHelper.ExecuteQuery(@"
                    SELECT 
                        s.Name as SensorName,
                        COUNT(r.ReadingId) as TotalReadings,
                        AVG(r.Value) as AverageValue,
                        MIN(r.Value) as MinValue,
                        MAX(r.Value) as MaxValue,
                        COUNT(CASE WHEN r.Quality = 2 THEN 1 END) as SuspectReadings,
                        COUNT(CASE WHEN r.Quality = 3 THEN 1 END) as InvalidReadings
                    FROM Sensors s
                    LEFT JOIN SensorReadings r ON s.SensorId = r.SensorId
                    WHERE s.IsActive = 1
                    GROUP BY s.SensorId, s.Name
                    ORDER BY s.Name");
                dgvReports.DataSource = stats;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Eroare la încărcarea statisticilor: {ex.Message}", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}