using System;
using System.Windows.Forms;
using SmartGreenhouse.UI.Forms;

namespace SmartGreenhouse.UI
{
    static class Program
    {
        public static bool RequestLoginAgain { get; set; } = false;

        /// <summary>
        /// Punctul principal de intrare în aplicație.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Prepare log directory/file
            var logDir = System.IO.Path.GetFullPath(System.IO.Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "logs"));
            System.IO.Directory.CreateDirectory(logDir);
            var logPath = System.IO.Path.Combine(logDir, "SmartGreenhouse_error.log");

            void Log(string message)
            {
                try
                {
                    System.IO.File.AppendAllText(logPath, $"[{DateTime.Now:O}] {message}\n");
                }
                catch
                {
                }
            }

            // Global exception handlers for WinForms
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            Application.ThreadException += (sender, e) =>
            {
                Log($"[UI Thread] {e.Exception}");
                MessageBox.Show($"Unexpected error: {e.Exception.Message}\n\n{e.Exception}\n\nSee log: {logPath}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            };

            AppDomain.CurrentDomain.FirstChanceException += (sender, e) =>
            {
                Log($"[FirstChance] {e.Exception}");
            };

            AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
            {
                if (e.ExceptionObject is Exception ex)
                {
                    Log($"[AppDomain] {ex}");
                    MessageBox.Show($"Unhandled exception: {ex.Message}\n\n{ex}\n\nSee log: {logPath}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };

            Log("Application starting");

            // Autentificare + suport pentru "schimb utilizator"
            try
            {
                do
                {
                    RequestLoginAgain = false;

                    using (var login = new FormLogin())
                    {
                        var dialog = login.ShowDialog();
                        Log($"Login dialog result: {dialog}");

                        if (dialog == DialogResult.OK)
                        {
                            Log($"Starting dashboard for user {login.AuthenticatedUser?.Username ?? "(null)"}");
                            Application.Run(new FormDashboard(login.AuthenticatedUser));
                        }
                        else
                        {
                            Log("Login cancelled");
                        }
                    }
                } while (RequestLoginAgain);
            }
            catch (Exception ex)
            {
                Log($"Exception during login/dashboard startup: {ex}");
                MessageBox.Show($"Eroare la pornirea aplicației: {ex.Message}\n\n{ex}\n\nVezi log: {logPath}", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            Log("Application exiting");
        }
    }
}