using System;
using System.Drawing;
using System.Windows.Forms;

namespace SystemProgrammingApp
{
    public class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "System Programming - Main Dashboard";
            this.Size = new Size(720, 520);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(30, 30, 30);
            this.ForeColor = Color.White;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            var lblTitle = new Label
            {
                Text = "SYSTEM PROGRAMMING ASSIGNMENT",
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 172, 238),
                AutoSize = true,
                Location = new Point(205, 40),
                Size = new Size(310, 30)
            };

            var lblSubtitle = new Label
            {
                Text = "Windows OS Resource Interaction Modules",
                Font = new Font("Segoe UI", 10F, FontStyle.Italic),
                ForeColor = Color.LightGray,
                AutoSize = true,
                Location = new Point(205, 75),
                Size = new Size(310, 20)
            };

            string[] buttonTexts = new string[]
            {
                "Module 1: Process Management",
                "Module 2: Windows Services Control",
                "Module 3: System Performance Monitor",
                "Module 4: Hardware & OS Information",
                "Module 5: Thread & Memory Inspector"
            };
            EventHandler[] handlers = new EventHandler[]
            {
                new EventHandler(OpenProcessForm),
                new EventHandler(OpenServicesForm),
                new EventHandler(OpenPerformanceForm),
                new EventHandler(OpenHardwareForm),
                new EventHandler(OpenThreadMemoryForm)
            };

            int y = 130;
            for (int i = 0; i < buttonTexts.Length; i++)
            {
                var btn = new Button
                {
                    Text = buttonTexts[i],
                    Size = new Size(310, 55),
                    Location = new Point(205, y),
                    BackColor = Color.FromArgb(45, 45, 48),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Font = new Font("Segoe UI", 10F, FontStyle.Regular)
                };
                btn.FlatAppearance.BorderColor = Color.FromArgb(60, 60, 60);
                btn.Click += handlers[i];
                btn.MouseEnter += delegate(object s, EventArgs e)
                {
                    var b = (Button)s;
                    b.BackColor = Color.FromArgb(60, 60, 68);
                };
                btn.MouseLeave += delegate(object s, EventArgs e)
                {
                    var b = (Button)s;
                    b.BackColor = Color.FromArgb(45, 45, 48);
                };
                this.Controls.Add(btn);
                y += 70;
            }

            var lblAdmin = new Label
            {
                Text = "NOTE: Run Visual Studio as Administrator for full OS access (process/service control).",
                Font = new Font("Segoe UI", 8F, FontStyle.Italic),
                ForeColor = Color.FromArgb(200, 100, 100),
                AutoSize = true,
                Location = new Point(20, 460),
                Size = new Size(680, 20)
            };

            this.Controls.Add(lblTitle);
            this.Controls.Add(lblSubtitle);
            this.Controls.Add(lblAdmin);
        }

        private void OpenProcessForm(object sender, EventArgs e)
        {
            OpenModule(new ProcessForm());
        }

        private void OpenServicesForm(object sender, EventArgs e)
        {
            OpenModule(new ServicesForm());
        }

        private void OpenPerformanceForm(object sender, EventArgs e)
        {
            OpenModule(new PerformanceForm());
        }

        private void OpenHardwareForm(object sender, EventArgs e)
        {
            OpenModule(new HardwareForm());
        }

        private void OpenThreadMemoryForm(object sender, EventArgs e)
        {
            OpenModule(new ThreadMemoryForm());
        }

        private void OpenModule(Form module)
        {
            module.StartPosition = FormStartPosition.CenterParent;
            module.ShowDialog(this);
        }
    }
}
