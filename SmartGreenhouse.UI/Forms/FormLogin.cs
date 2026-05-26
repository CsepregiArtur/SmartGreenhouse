using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using SmartGreenhouse.BLL.Services;
using SmartGreenhouse.DAL.Repositories;
using SmartGreenhouse.Models.Entities;

namespace SmartGreenhouse.UI.Forms
{
    /// <summary>
    /// Formular de autentificare cu efecte WOW:
    /// - Gradient background
    /// - Animații la hover
    /// - Efect de glow
    /// - Tranziții smooth
    /// </summary>
    public partial class FormLogin : Form
    {
        private readonly UserRepository _userRepo;
        private TextBox txtUsername;
        private TextBox txtPassword;
        private Button btnLogin;
        private Label lblTitle;
        private Label lblStatus;
        private Label lblHint;
        private PictureBox pictureBox;
        private System.Windows.Forms.Timer animationTimer;
        private float glowIntensity = 0;
        private int enterCount = 0;
        private Panel loginPanel;

        public User AuthenticatedUser { get; private set; }

        public FormLogin()
        {
            _userRepo = new UserRepository();
            InitializeComponent();
            SetupAnimations();
            LoadUserHint();
        }

        private void LoadUserHint()
        {
            try
            {
                var users = _userRepo.GetAllUsers();
                if (users == null || users.Count == 0)
                {
                    lblHint.Text = "Nu există utilizatori în baza de date.";
                    return;
                }

                var activeUsers = users.FindAll(u => u.IsActive);
                if (activeUsers.Count == 0)
                {
                    lblHint.Text = "Nu există utilizatori activi.";
                    return;
                }

                var names = string.Join(", ", activeUsers.ConvertAll(u => u.Username));
                lblHint.Text = $"Utilizatori disponibili: {names}\nParolă: același cu username-ul";
            }
            catch (Exception ex)
            {
                lblHint.Text = "Eroare la încărcarea utilizatorilor.";
                // If we are in a debug session, log to console as well
                Console.WriteLine($"[Login] Failed to load user list: {ex}");
            }
        }

        private void InitializeComponent()
        {
            this.Text = "Smart Greenhouse - Login";
            this.Size = new Size(450, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.BackColor = Color.FromArgb(45, 66, 91);
            
            // Setează regiunea formei pentru colțuri rotunjite
            this.Paint += FormLogin_Paint;

            // Titlu
            lblTitle = new Label
            {
                Text = "🌿 Smart Greenhouse",
                Font = new Font("Segoe UI", 24, FontStyle.Bold),
                ForeColor = Color.White,
                Size = new Size(400, 50),
                Location = new Point(25, 50),
                TextAlign = ContentAlignment.MiddleCenter
            };
            this.Controls.Add(lblTitle);

            // Subtitlu
            Label lblSubtitle = new Label
            {
                Text = "Monitorizare și Control Inteligent",
                Font = new Font("Segoe UI", 12, FontStyle.Regular),
                ForeColor = Color.FromArgb(200, 200, 200),
                Size = new Size(400, 30),
                Location = new Point(25, 100),
                TextAlign = ContentAlignment.MiddleCenter
            };
            this.Controls.Add(lblSubtitle);

            // Hint pentru utilizatori disponibili
            lblHint = new Label
            {
                Text = "Se încarcă utilizatorii...",
                Font = new Font("Segoe UI", 9, FontStyle.Italic),
                ForeColor = Color.FromArgb(150, 150, 150),
                Size = new Size(400, 40),
                Location = new Point(25, 130),
                TextAlign = ContentAlignment.MiddleCenter
            };
            this.Controls.Add(lblHint);

            // Panel pentru login
            loginPanel = new Panel
            {
                Size = new Size(350, 300),
                Location = new Point(50, 200),
                BackColor = Color.FromArgb(30, 40, 50)
            };
            loginPanel.Paint += LoginPanel_Paint;
            this.Controls.Add(loginPanel);

            // Username label
            Label lblUsername = new Label
            {
                Text = "Username:",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(20, 30),
                Size = new Size(100, 25)
            };
            loginPanel.Controls.Add(lblUsername);

            // Username textbox
            txtUsername = new TextBox
            {
                Location = new Point(20, 60),
                Size = new Size(310, 30),
                Font = new Font("Segoe UI", 11),
                BackColor = Color.FromArgb(80, 90, 100),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            txtUsername.Enter += TxtUsername_Enter;
            txtUsername.Leave += TxtUsername_Leave;
            loginPanel.Controls.Add(txtUsername);

            // Password label
            Label lblPassword = new Label
            {
                Text = "Password:",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(20, 100),
                Size = new Size(100, 25)
            };
            loginPanel.Controls.Add(lblPassword);

            // Password textbox
            txtPassword = new TextBox
            {
                Location = new Point(20, 130),
                Size = new Size(310, 30),
                Font = new Font("Segoe UI", 11),
                UseSystemPasswordChar = true,
                BackColor = Color.FromArgb(80, 90, 100),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            txtPassword.Enter += TxtPassword_Enter;
            txtPassword.Leave += TxtPassword_Leave;
            txtPassword.KeyPress += TxtPassword_KeyPress;
            loginPanel.Controls.Add(txtPassword);

            // Login button
            btnLogin = new Button
            {
                Text = "🔐 Autentificare",
                Location = new Point(20, 190),
                Size = new Size(310, 45),
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                BackColor = Color.FromArgb(0, 150, 136),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnLogin.FlatAppearance.BorderSize = 0;
            btnLogin.MouseEnter += BtnLogin_MouseEnter;
            btnLogin.MouseLeave += BtnLogin_MouseLeave;
            btnLogin.Click += BtnLogin_Click;
            loginPanel.Controls.Add(btnLogin);

            // Status label
            lblStatus = new Label
            {
                Text = "",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.OrangeRed,
                Location = new Point(20, 250),
                Size = new Size(310, 30),
                TextAlign = ContentAlignment.MiddleCenter,
                Visible = false
            };
            loginPanel.Controls.Add(lblStatus);

            // Versiune
            Label lblVersion = new Label
            {
                Text = "v2.0.0 | UTCN 2026",
                Font = new Font("Segoe UI", 8),
                ForeColor = Color.Gray,
                Location = new Point(170, 560),
                Size = new Size(120, 20),
                TextAlign = ContentAlignment.MiddleCenter
            };
            this.Controls.Add(lblVersion);
        }

        private void SetupAnimations()
        {
            animationTimer = new System.Windows.Forms.Timer();
            animationTimer.Interval = 50;
            animationTimer.Tick += AnimationTimer_Tick;
        }

        private void FormLogin_Paint(object sender, PaintEventArgs e)
        {
            // Gradient background
            using (LinearGradientBrush brush = new LinearGradientBrush(
                this.ClientRectangle,
                Color.FromArgb(45, 66, 91),
                Color.FromArgb(26, 37, 51),
                LinearGradientMode.Vertical))
            {
                e.Graphics.FillRectangle(brush, this.ClientRectangle);
            }

            // Colțuri rotunjite
            using (GraphicsPath path = new GraphicsPath())
            {
                int radius = 20;
                Rectangle rect = this.ClientRectangle;
                path.AddArc(rect.X, rect.Y, radius, radius, 180, 90);
                path.AddArc(rect.Right - radius, rect.Y, radius, radius, 270, 90);
                path.AddArc(rect.Right - radius, rect.Bottom - radius, radius, radius, 0, 90);
                path.AddArc(rect.X, rect.Bottom - radius, radius, radius, 90, 90);
                path.CloseFigure();
                this.Region = new Region(path);
            }
        }

        private void LoginPanel_Paint(object sender, PaintEventArgs e)
        {
            Panel panel = sender as Panel;
            
            // Fundal semi-transparent
            using (SolidBrush brush = new SolidBrush(Color.FromArgb(30, 40, 50)))
            {
                e.Graphics.FillRectangle(brush, panel.ClientRectangle);
            }

            // Border fix
            using (Pen pen = new Pen(Color.FromArgb(0, 150, 136), 2))
            {
                e.Graphics.DrawRectangle(pen, 0, 0, panel.Width - 1, panel.Height - 1);
            }
        }

        private void TxtUsername_Enter(object sender, EventArgs e)
        {
            txtUsername.BackColor = Color.FromArgb(70, 80, 90);
            enterCount++;
            if (enterCount == 1) animationTimer.Start();
        }

        private void TxtUsername_Leave(object sender, EventArgs e)
        {
            txtUsername.BackColor = Color.FromArgb(80, 90, 100);
            enterCount--;
            if (enterCount == 0) animationTimer.Stop();
        }

        private void TxtPassword_Enter(object sender, EventArgs e)
        {
            txtPassword.BackColor = Color.FromArgb(70, 80, 90);
            enterCount++;
            if (enterCount == 1) animationTimer.Start();
        }

        private void TxtPassword_Leave(object sender, EventArgs e)
        {
            txtPassword.BackColor = Color.FromArgb(80, 90, 100);
            enterCount--;
            if (enterCount == 0) animationTimer.Stop();
        }

        private void BtnLogin_MouseEnter(object sender, EventArgs e)
        {
            btnLogin.BackColor = Color.FromArgb(0, 170, 156);
            btnLogin.Size = new Size(320, 47);
            btnLogin.Location = new Point(15, 189);
        }

        private void BtnLogin_MouseLeave(object sender, EventArgs e)
        {
            btnLogin.BackColor = Color.FromArgb(0, 150, 136);
            btnLogin.Size = new Size(310, 45);
            btnLogin.Location = new Point(20, 190);
        }

        private void AnimationTimer_Tick(object sender, EventArgs e)
        {
            // Removed glow animation
        }

        private void TxtPassword_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                BtnLogin_Click(sender, e);
            }
        }

        private async void BtnLogin_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUsername.Text) || string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                ShowStatus("Completați toate câmpurile!", Color.OrangeRed);
                return;
            }

            btnLogin.Enabled = false;
            btnLogin.Text = "⏳ Se autentifică...";

            try
            {
                // Simulăm o întârziere pentru efect
                await System.Threading.Tasks.Task.Delay(500);

                // Hash simplu pentru parolă (în realitate ar fi mai complex)
                string passwordHash = ComputeHash(txtPassword.Text);

                AuthenticatedUser = _userRepo.Authenticate(txtUsername.Text, passwordHash, txtPassword.Text);

                if (AuthenticatedUser != null)
                {
                    ShowStatus("Autentificare reușită! Se încarcă...", Color.LightGreen);
                    
                    // Efect de fade out
                    for (double i = 1; i >= 0; i -= 0.1)
                    {
                        this.Opacity = i;
                        await System.Threading.Tasks.Task.Delay(30);
                    }

                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    ShowStatus("Username sau parolă incorecte!", Color.OrangeRed);
                    txtPassword.Clear();
                    txtUsername.Focus();
                }
            }
            catch (Exception ex)
            {
                ShowStatus($"Eroare: {ex.Message}", Color.Red);
            }
            finally
            {
                btnLogin.Enabled = true;
                btnLogin.Text = "🔐 Autentificare";
            }
        }

        private void ShowStatus(string message, Color color)
        {
            lblStatus.Text = message;
            lblStatus.ForeColor = color;
            lblStatus.Visible = true;

            // Ascunde mesajul după 3 secunde
            System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
            timer.Interval = 3000;
            timer.Tick += (s, e) => {
                lblStatus.Visible = false;
                timer.Stop();
                timer.Dispose();
            };
            timer.Start();
        }

        private string ComputeHash(string input)
        {
            // Hash simplu pentru demonstrație
            using (var sha = System.Security.Cryptography.SHA256.Create())
            {
                var bytes = System.Text.Encoding.UTF8.GetBytes(input);
                var hash = sha.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }
    }
}