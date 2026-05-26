using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Linq;
using SmartGreenhouse.Models.Entities;
using SmartGreenhouse.BLL.Simulation;
using SmartGreenhouse.BLL.Services;
using SmartGreenhouse.DAL.Repositories;
using System.Collections.Generic;
using ScottPlot.WinForms;

namespace SmartGreenhouse.UI.Forms
{
    /// <summary>
    /// Dashboard principal cu efecte WOW:
    /// - Carduri moderne cu gradient
    /// - Valori în timp real
    /// - Animații la actualizare
    /// - Grafic 3D simplu
    /// - Notificări toast
    /// </summary>
    public partial class FormDashboard : Form
    {
        private readonly User _currentUser;
        private readonly SensorSimulator _simulator;
        private readonly AlertService _alertService;
        private readonly SensorRepository _sensorRepo;
        
        private FlowLayoutPanel pnlSensors;
        private Label lblTime;
        private Panel pnlAlerts;
        private ListBox lstAlerts;
        private System.Windows.Forms.Timer uiTimer;
        private NotifyIcon notifyIcon;
        private MenuStrip menuStrip;
        private DataGridView dgvSensors;
        private FormsPlot formsPlot;
        private Dictionary<int, List<SensorReading>> sensorHistory = new Dictionary<int, List<SensorReading>>();

        public FormDashboard(User user)
        {
            try
            {
                _currentUser = user;
                _simulator = new SensorSimulator();
                _alertService = new AlertService();
                _sensorRepo = new SensorRepository();
                
                InitializeComponent();
                ConfigureUIBasedOnRole();
                SetupEventHandlers();
                LoadSensors();
                // StartSimulation(); moved to OnLoad to ensure handle is created
            }
            catch (Exception ex)
            {
                LogDashboardError(ex);
                MessageBox.Show($"Eroare la inițializarea dashboard-ului:\n{ex.Message}", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw;
            }
        }

        private void LogDashboardError(Exception ex)
        {
            try
            {
                var logDir = System.IO.Path.GetFullPath(System.IO.Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "logs"));
                System.IO.Directory.CreateDirectory(logDir);
                var logPath = System.IO.Path.Combine(logDir, "SmartGreenhouse_dashboard_error.log");
                System.IO.File.AppendAllText(logPath, $"[{DateTime.Now:O}] {ex}\n");
            }
            catch
            {
            }
        }
        
        private void InitializeComponent()
        {
            this.Text = $"Smart Greenhouse - Dashboard ({_currentUser.Username})";
            this.Size = new Size(1400, 900);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(240, 245, 250);
            this.WindowState = FormWindowState.Maximized;

            // MenuStrip
            menuStrip = new MenuStrip();
            
            var fileMenu = new ToolStripMenuItem("Fișier");
            fileMenu.DropDownItems.Add("Setări", null, Settings_Click);
            fileMenu.DropDownItems.Add("Schimbă utilizator", null, SwitchUser_Click);
            fileMenu.DropDownItems.Add("-");
            fileMenu.DropDownItems.Add("Ieșire", null, (s, e) => Application.Exit());

            var sensorsMenu = new ToolStripMenuItem("Senzori");
            sensorsMenu.DropDownItems.Add("Gestionare Senzori", null, ManageSensors_Click);
            sensorsMenu.DropDownItems.Add("Rapoarte", null, Reports_Click);

            var usersMenu = new ToolStripMenuItem("Utilizatori");
            usersMenu.DropDownItems.Add("Gestionare Utilizatori", null, ManageUsers_Click);

            var alarmsMenu = new ToolStripMenuItem("Alarme");
            alarmsMenu.DropDownItems.Add("Gestionare Alarme", null, ManageAlarms_Click);

            var helpMenu = new ToolStripMenuItem("Ajutor");
            helpMenu.DropDownItems.Add("Despre", null, About_Click);

            menuStrip.Items.Add(fileMenu);
            menuStrip.Items.Add(sensorsMenu);
            menuStrip.Items.Add(usersMenu);
            menuStrip.Items.Add(alarmsMenu);
            menuStrip.Items.Add(helpMenu);

            this.MainMenuStrip = menuStrip;
            this.Controls.Add(menuStrip);

            // Header
            Panel pnlHeader = new Panel
            {
                Height = 60,
                Dock = DockStyle.Top,
                BackColor = Color.FromArgb(45, 66, 91)
            };
            pnlHeader.Paint += PnlHeader_Paint;

            Label lblWelcome = new Label
            {
                Text = $"Bine ai venit, {_currentUser.Username}!",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(30, 10),
                AutoSize = true
            };
            pnlHeader.Controls.Add(lblWelcome);

            lblTime = new Label
            {
                Font = new Font("Segoe UI", 12),
                ForeColor = Color.FromArgb(200, 200, 200),
                Location = new Point(30, 35),
                AutoSize = true
            };
            pnlHeader.Controls.Add(lblTime);

            // Status
            Label lblStatus = new Label
            {
                Text = _currentUser.Role == UserRole.Admin ? "👑 Administrator" : "👤 Operator",
                Font = new Font("Segoe UI Symbol", 10),
                ForeColor = Color.FromArgb(150, 150, 150),
                Location = new Point(this.Width - 200, 20),
                AutoSize = true
            };
            pnlHeader.Controls.Add(lblStatus);

            this.Controls.Add(pnlHeader);

            // Panel Principal
            TableLayoutPanel mainPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 2,
                Padding = new Padding(20, 90, 20, 20),
                BackColor = Color.Transparent
            };
            mainPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60F));
            mainPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40F));
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));

            // Senzori (FlowLayoutPanel cu carduri)
            pnlSensors = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent,
                AutoScroll = true,
                Padding = new Padding(10),
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = true
            };
            mainPanel.Controls.Add(pnlSensors, 0, 0);
            mainPanel.SetRowSpan(pnlSensors, 2);

            // Panou Grafice
            Panel pnlCharts = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(10)
            };
            pnlCharts.Paint += PnlCharts_Paint;
            mainPanel.Controls.Add(pnlCharts, 1, 0);

            // Panou Alerte
            pnlAlerts = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(10)
            };
            pnlAlerts.Paint += PnlAlerts_Paint;
            
            Label lblAlertsTitle = new Label
            {
                Text = "🔔 Alerte Recente",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                Location = new Point(10, 10),
                AutoSize = true
            };
            pnlAlerts.Controls.Add(lblAlertsTitle);

            lstAlerts = new ListBox
            {
                Location = new Point(10, 40),
                Size = new Size(pnlAlerts.Width - 40, pnlAlerts.Height - 60),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                BackColor = Color.FromArgb(250, 250, 250),
                BorderStyle = BorderStyle.None,
                Font = new Font("Segoe UI Emoji", 10),
                DrawMode = DrawMode.OwnerDrawFixed,
                ItemHeight = 30
            };
            lstAlerts.DrawItem += LstAlerts_DrawItem;
            pnlAlerts.Controls.Add(lstAlerts);

            mainPanel.Controls.Add(pnlAlerts, 1, 1);

            this.Controls.Add(mainPanel);

            // NotifyIcon pentru notificări toast
            notifyIcon = new NotifyIcon
            {
                Icon = SystemIcons.Information,
                Visible = true,
                Text = "Smart Greenhouse"
            };
        }
        private void ConfigureUIBasedOnRole()
        {
            if (_currentUser.Role != UserRole.Admin)
            {
                // Ascunde butoanele de administrare
                foreach (ToolStripItem item in menuStrip.Items)
                {
                    if (item.Text == "Senzori" || item.Text == "Utilizatori")
                    {
                        item.Visible = false;
                    }
                }
                
                // Dezactivează editarea în grid-uri (dacă există)
                // Note: DataGridView nu este încă implementat în UI
            }
        }
        private void PnlHeader_Paint(object sender, PaintEventArgs e)
        {
            Panel pnl = sender as Panel;
            using (LinearGradientBrush brush = new LinearGradientBrush(
                pnl.ClientRectangle,
                Color.FromArgb(45, 66, 91),
                Color.FromArgb(26, 37, 51),
                LinearGradientMode.Horizontal))
            {
                e.Graphics.FillRectangle(brush, pnl.ClientRectangle);
            }
        }

        private void PnlCharts_Paint(object sender, PaintEventArgs e)
        {
            Panel pnl = sender as Panel;
            
            // Desenează un grafic WOW
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

            // Fundal cu gradient
            using (LinearGradientBrush bgBrush = new LinearGradientBrush(
                pnl.ClientRectangle,
                Color.FromArgb(250, 250, 250),
                Color.FromArgb(240, 240, 240),
                LinearGradientMode.Vertical))
            {
                g.FillRectangle(bgBrush, pnl.ClientRectangle);
            }

            // Titlu
            using (Font titleFont = new Font("Segoe UI Emoji", 12, FontStyle.Bold))
            {
                g.DrawString("📊 Evoluție Temperatură - Ultimele 24h", titleFont, Brushes.Black, 10, 5);
            }

            // Desenează axele
            int margin = 60;
            int width = pnl.Width - 2 * margin;
            int height = pnl.Height - 2 * margin - 50;
            
            // Grid lines
            using (Pen gridPen = new Pen(Color.FromArgb(200, 200, 200), 1))
            {
                // Vertical grid
                for (int i = 0; i <= 10; i++)
                {
                    float x = margin + (i * width / 10f);
                    g.DrawLine(gridPen, x, margin, x, margin + height);
                }
                // Horizontal grid
                for (int i = 0; i <= 5; i++)
                {
                    float y = margin + (i * height / 5f);
                    g.DrawLine(gridPen, margin, y, margin + width, y);
                }
            }

            using (Pen axisPen = new Pen(Color.Black, 2))
            {
                g.DrawLine(axisPen, margin, margin + height, margin + width, margin + height); // axa X
                g.DrawLine(axisPen, margin, margin, margin, margin + height); // axa Y
            }

            // Labels
            using (Font labelFont = new Font("Segoe UI", 8))
            {
                // Y axis labels (temperature)
                for (int i = 0; i <= 5; i++)
                {
                    float temp = 40 - (i * 40f / 5f); // Assuming 0-40°C
                    float y = margin + (i * height / 5f);
                    g.DrawString($"{temp:F0}°C", labelFont, Brushes.Gray, margin - 35, y - 5);
                }
                // X axis labels (hours)
                for (int i = 0; i <= 10; i++)
                {
                    float x = margin + (i * width / 10f);
                    string hour = $"{24 - i * 2.4:F0}h";
                    g.DrawString(hour, labelFont, Brushes.Gray, x - 10, margin + height + 5);
                }
            }

            // Generăm date mai realiste
            Random rand = new Random(42); // Seed for consistency
            List<PointF> points = new List<PointF>();
            float baseTemp = 22f;
            
            for (int i = 0; i <= 24; i++) // 24 points for 24 hours
            {
                float time = i;
                float temp = baseTemp + (float)(rand.NextDouble() - 0.5) * 10f + (float)Math.Sin(time / 24f * 2 * Math.PI) * 5f;
                temp = Math.Max(0, Math.Min(40, temp));
                float x = margin + (time / 24f * width);
                float y = margin + height - (temp / 40f * height);
                points.Add(new PointF(x, y));
            }

            // Fill under the curve
            if (points.Count > 1)
            {
                PointF[] fillPoints = new PointF[points.Count + 2];
                points.CopyTo(fillPoints, 0);
                fillPoints[points.Count] = new PointF(points[points.Count - 1].X, margin + height);
                fillPoints[points.Count + 1] = new PointF(points[0].X, margin + height);
                
                using (SolidBrush fillBrush = new SolidBrush(Color.FromArgb(100, 0, 150, 136)))
                {
                    g.FillPolygon(fillBrush, fillPoints);
                }
            }

            // Desenează umbra
            using (Pen shadowPen = new Pen(Color.FromArgb(100, 0, 150, 136), 4))
            {
                for (int i = 0; i < points.Count - 1; i++)
                {
                    g.DrawLine(shadowPen, 
                        points[i].X + 2, points[i].Y + 2,
                        points[i + 1].X + 2, points[i + 1].Y + 2);
                }
            }

            // Desenează linia principală
            using (Pen linePen = new Pen(Color.FromArgb(0, 150, 136), 3))
            {
                for (int i = 0; i < points.Count - 1; i++)
                {
                    g.DrawLine(linePen, points[i], points[i + 1]);
                }
            }

            // Desenează punctele
            foreach (var point in points)
            {
                using (SolidBrush pointBrush = new SolidBrush(Color.Orange))
                {
                    g.FillEllipse(pointBrush, point.X - 3, point.Y - 3, 6, 6);
                }
                g.DrawEllipse(Pens.Black, point.X - 3, point.Y - 3, 6, 6);
            }
        }

        private void PnlAlerts_Paint(object sender, PaintEventArgs e)
        {
            Panel pnl = sender as Panel;
            
            // Desenează border
            using (Pen pen = new Pen(Color.FromArgb(200, 200, 200), 1))
            {
                e.Graphics.DrawRectangle(pen, 0, 0, pnl.Width - 1, pnl.Height - 1);
            }
        }

        private void LstAlerts_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0) return;

            ListBox list = sender as ListBox;
            string text = list.Items[e.Index].ToString();

            e.DrawBackground();

            // Desenează iconiță în funcție de tipul alertei
            string icon = "!";
            Color backColor = Color.LightYellow;

            if (text.Contains("CRITICAL"))
            {
                icon = "▲";
                backColor = Color.LightCoral;
            }
            else if (text.Contains("WARNING"))
            {
                icon = "●";
                backColor = Color.LightGoldenrodYellow;
            }

            using (SolidBrush bgBrush = new SolidBrush(backColor))
            {
                e.Graphics.FillRectangle(bgBrush, e.Bounds);
            }

            // Desenează textul
            using (Font drawFont = new Font("Segoe UI", 10))
            {
                e.Graphics.DrawString($"{icon} {text}", drawFont, Brushes.Black, 
                    e.Bounds.X + 5, e.Bounds.Y + 5);
            }

            e.DrawFocusRectangle();
        }

        private void SetupEventHandlers()
        {
            // Evenimente de la simulator
            _simulator.NewReadingGenerated += OnNewReadingGenerated;
            _simulator.SimulationStarted += (s, e) => {
                this.Invoke((MethodInvoker)delegate {
                    // Afișează notificare că simularea a pornit
                });
            };

            // Evenimente de la alert service
            _alertService.AlertGenerated += OnAlertGenerated;
            _alertService.CriticalAlertGenerated += OnCriticalAlertGenerated;

            // Timer pentru actualizarea orei
            uiTimer = new System.Windows.Forms.Timer();
            uiTimer.Interval = 1000;
            uiTimer.Tick += (s, e) => {
                lblTime.Text = DateTime.Now.ToString("dddd, dd MMMM yyyy HH:mm:ss");
            };
            uiTimer.Start();
        }

        private void LoadSensors()
        {
            try
            {
                var sensors = _sensorRepo.GetAllSensors();
                
                foreach (var sensor in sensors)
                {
                    var card = CreateSensorCard(sensor);
                    pnlSensors.Controls.Add(card);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Eroare la încărcarea senzorilor: {ex.Message}", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            // Start simulation after the form handle is created to avoid Invoke errors
            StartSimulation();
        }

        private Panel CreateSensorCard(Sensor sensor)
        {
            Panel card = new Panel
            {
                Size = new Size(280, 180),
                BackColor = Color.White,
                Margin = new Padding(10)
            };
            
            // Efect de umbră
            card.Paint += (s, e) => {
                Panel p = s as Panel;
                
                // Desenează umbra
                using (Pen shadowPen = new Pen(Color.FromArgb(50, 0, 0, 0), 2))
                {
                    e.Graphics.DrawRectangle(shadowPen, 2, 2, p.Width - 3, p.Height - 3);
                }

                // Desenează bordură colorată în funcție de tip
                Color borderColor = sensor.Type switch
                {
                    SensorType.Temperature => Color.Red,
                    SensorType.Humidity => Color.Blue,
                    SensorType.Light => Color.Yellow,
                    _ => Color.Gray
                };

                using (Pen borderPen = new Pen(borderColor, 3))
                {
                    e.Graphics.DrawLine(borderPen, 0, 0, p.Width, 0);
                }
            };

            // Iconiță
            Label lblIcon = new Label
            {
                Text = GetSensorIcon(sensor.Type),
                Font = new Font("Segoe UI Symbol", 20),
                Location = new Point(10, 10),
                AutoSize = true
            };
            card.Controls.Add(lblIcon);

            // Nume senzor
            Label lblName = new Label
            {
                Text = sensor.Name,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Location = new Point(50, 15),
                AutoSize = true
            };
            card.Controls.Add(lblName);

            // Locație
            Label lblLocation = new Label
            {
                Text = sensor.Location,
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.Gray,
                Location = new Point(60, 40),
                AutoSize = true
            };
            card.Controls.Add(lblLocation);

            // Valoare curentă
            Label lblValue = new Label
            {
                Text = $"--- {sensor.Unit}",
                Font = new Font("Segoe UI", 24, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 150, 136),
                Location = new Point(20, 80),
                AutoSize = true,
                Tag = sensor.SensorId // Pentru identificare
            };
            card.Controls.Add(lblValue);

            // Status bar
            int scale = 10;
            int minScaled = (int)(sensor.MinValue * scale);
            int maxScaled = (int)(sensor.MaxValue * scale);
            ProgressBar pbStatus = new ProgressBar
            {
                Location = new Point(20, 150),
                Size = new Size(240, 10),
                Minimum = 0,
                Maximum = maxScaled - minScaled,
                Value = 0,
                Style = ProgressBarStyle.Continuous
            };
            // pbStatus.SetStyle(ControlStyles.UserPaint, true);
            pbStatus.Paint += (s, e) => {
                ProgressBar pb = s as ProgressBar;
                Rectangle rect = pb.ClientRectangle;
                e.Graphics.FillRectangle(Brushes.LightGray, rect);
                
                double percent = (double)(pb.Value - pb.Minimum) / (pb.Maximum - pb.Minimum);
                int fillWidth = (int)(rect.Width * percent);
                
                using (SolidBrush brush = new SolidBrush(Color.FromArgb(0, 150, 136)))
                {
                    e.Graphics.FillRectangle(brush, 0, 0, fillWidth, rect.Height);
                }
            };
            card.Controls.Add(pbStatus);

            return card;
        }

        private void ShowSensorHistory(Sensor sensor)
        {
            MessageBox.Show($"Istoric pentru senzorul {sensor.Name} - Funcționalitate în dezvoltare", "Istoric Senzor", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private string GetSensorIcon(SensorType type)
        {
            return type switch
            {
                SensorType.Temperature => "🌡️",
                SensorType.Humidity => "💧",
                SensorType.SoilMoisture => "🌱",
                SensorType.Light => "☀️",
                SensorType.CO2 => "📟",
                _ => "📟"
            };
        }

        private void OnNewReadingGenerated(object sender, NewReadingEventArgs e)
        {
            if (this.IsDisposed || !this.IsHandleCreated)
                return;

            if (this.InvokeRequired)
            {
                try
                {
                    this.BeginInvoke((MethodInvoker)delegate {
                        UpdateSensorValue(e.Sensor, e.Reading);
                    });
                }
                catch
                {
                    // Ignore if invoke fails because the handle is not ready anymore.
                }
            }
            else
            {
                UpdateSensorValue(e.Sensor, e.Reading);
            }
        }

        private void UpdateSensorValue(Sensor sensor, SensorReading reading)
        {
            // Găsește cardul corespunzător și actualizează valoarea
            foreach (Control control in pnlSensors.Controls)
            {
                if (control is Panel card)
                {
                    foreach (Control subControl in card.Controls)
                    {
                        if (subControl is Label lbl && subControl.Tag != null && 
                            (int)subControl.Tag == sensor.SensorId)
                        {
                            // Efect de animație la actualizare
                            AnimateValueChange(lbl, $"{reading.Value:F1} {sensor.Unit}");
                            
                            // Actualizează progress bar
                            var pb = card.Controls.OfType<ProgressBar>().FirstOrDefault();
                            if (pb != null)
                            {
                                int newValue = (int)(reading.Value * 10) - (int)(sensor.MinValue * 10);
                                pb.Value = Math.Max(0, Math.Min(pb.Maximum, newValue));
                                pb.Invalidate();
                            }
                            
                            break;
                        }
                    }
                }
            }

            // Check for alerts
            _alertService.CheckThresholds(sensor, reading);
        }

        private async void AnimateValueChange(Label lbl, string newValue)
        {
            // Efect de fade pentru valoarea nouă
            Color originalColor = lbl.ForeColor;
            
            for (int i = 0; i < 3; i++)
            {
                lbl.ForeColor = Color.Gold;
                await System.Threading.Tasks.Task.Delay(50);
                lbl.ForeColor = originalColor;
                await System.Threading.Tasks.Task.Delay(50);
            }
            
            lbl.Text = newValue;
        }

        private void OnAlertGenerated(object sender, AlertEventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke((MethodInvoker)delegate {
                    AddAlertToList(e);
                    
                    // Notificare toast
                    notifyIcon.ShowBalloonTip(
                        3000,
                        "Alertă Sistem",
                        e.Alert.Message,
                        ToolTipIcon.Warning
                    );
                });
            }
            else
            {
                AddAlertToList(e);
            }
        }

        private void OnCriticalAlertGenerated(object sender, EventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke((MethodInvoker)delegate {
                    // Efect special pentru alerte critice
                    pnlAlerts.BackColor = Color.LightCoral;
                    
                    System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
                    timer.Interval = 1000;
                    timer.Tick += (s, ev) => {
                        pnlAlerts.BackColor = Color.White;
                        timer.Stop();
                        timer.Dispose();
                    };
                    timer.Start();
                });
            }
        }

        private void AddAlertToList(AlertEventArgs e)
        {
            string timeString = e.Timestamp.ToString("HH:mm:ss");
            string alertText = $"[{timeString}] {e.SensorName}: {e.Alert.Message}";
            
            lstAlerts.Items.Insert(0, alertText);
            
            // Păstrează doar ultimele 50 de alerte
            while (lstAlerts.Items.Count > 50)
            {
                lstAlerts.Items.RemoveAt(lstAlerts.Items.Count - 1);
            }
        }

        private void StartSimulation()
        {
            _simulator.StartSimulation();
        }

        private void ManageUsers_Click(object sender, EventArgs e)
        {
            if (_currentUser.Role == UserRole.Admin)
            {
                using (var form = new FormUserManagement())
                {
                    form.ShowDialog();
                }
            }
            else
            {
                MessageBox.Show("Nu aveți drepturi de administrator!", "Acces Restricționat",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void SwitchUser_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Sigur doriți să schimbați utilizatorul?", "Confirmare", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Program.RequestLoginAgain = true;
                this.Close();
            }
        }

        private void ManageAlarms_Click(object sender, EventArgs e)
        {
            if (_currentUser.Role == UserRole.Admin)
            {
                FormAlarmManagement form = new FormAlarmManagement(_currentUser);
                form.ShowDialog();
            }
            else
            {
                MessageBox.Show("Nu aveți drepturi de administrator!", "Acces Restricționat",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void ManageSensors_Click(object sender, EventArgs e)
        {
            if (_currentUser.Role == UserRole.Admin)
            {
                FormSensorsManagement form = new FormSensorsManagement(_currentUser);
                form.ShowDialog();
            }
            else
            {
                MessageBox.Show("Nu aveți drepturi de administrator!", "Acces Restricționat",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void Reports_Click(object sender, EventArgs e)
        {
            using (var reportsForm = new FormReports())
            {
                reportsForm.ShowDialog();
            }
        }

        private void Settings_Click(object sender, EventArgs e)
        {
            using (var settingsForm = new FormSettings())
            {
                settingsForm.ShowDialog();
            }
        }

        private void About_Click(object sender, EventArgs e)
        {
            MessageBox.Show(
                "Smart Greenhouse Manager v2.0\n" +
                "Proiect Informatica Industrială\n" +
                "UTCN 2026\n\n" +
                "Echipă: Csepregi Artur",
                "Despre",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _simulator.StopSimulation();
            uiTimer.Stop();
            notifyIcon.Visible = false;
            base.OnFormClosing(e);
        }
    }
}