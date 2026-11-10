using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace SystemProgrammingApp
{
    public class PerformanceForm : Form
    {
        private PerformanceCounter cpuCounter;
        private PerformanceCounter memoryCounter;
        private PerformanceCounter diskCounter;
        private Timer monitorTimer;

        private Label lblCpuValue, lblMemValue, lblDiskValue;
        private ProgressBar pbCpu, pbMem, pbDisk;
        private Button btnToggle;
        private bool monitoring = false;

        public PerformanceForm()
        {
            InitializeComponent();
            InitializeCounters();
        }

        private void InitializeComponent()
        {
            this.Text = "Module 3 - System Performance Monitor";
            this.Size = new Size(620, 420);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.FromArgb(30, 30, 30);
            this.ForeColor = Color.White;

            var lblTitle = new Label
            {
                Text = "System Performance Monitor",
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 172, 238),
                AutoSize = true,
                Location = new Point(20, 15),
                Size = new Size(220, 30)
            };

            string[] metricLabels = new string[]
            {
                "CPU Usage (%)",
                "Available Memory (MB)",
                "Disk Activity (%)"
            };

            int y = 60;
            for (int i = 0; i < metricLabels.Length; i++)
            {
                var lbl = new Label
                {
                    Text = metricLabels[i],
                    Location = new Point(20, y),
                    Size = new Size(150, 20),
                    ForeColor = Color.White,
                    Font = new Font("Segoe UI", 9F, FontStyle.Bold)
                };
                var val = new Label
                {
                    Text = "--",
                    Location = new Point(190, y),
                    Size = new Size(100, 20),
                    ForeColor = Color.FromArgb(0, 172, 238),
                    Font = new Font("Segoe UI", 9F, FontStyle.Bold)
                };
                var pb = new ProgressBar
                {
                    Location = new Point(310, y),
                    Size = new Size(280, 20),
                    Minimum = 0,
                    Maximum = 100,
                    Value = 0
                };

                if (i == 0) { lblCpuValue = val; pbCpu = pb; }
                else if (i == 1) { lblMemValue = val; pbMem = pb; }
                else { lblDiskValue = val; pbDisk = pb; }

                this.Controls.Add(lbl);
                this.Controls.Add(val);
                this.Controls.Add(pb);
                y += 55;
            }

            btnToggle = new Button
            {
                Text = "Start Monitoring",
                Location = new Point(20, y + 10),
                Size = new Size(160, 35),
                BackColor = Color.FromArgb(0, 150, 136),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold)
            };
            btnToggle.FlatAppearance.BorderColor = Color.FromArgb(0, 120, 106);
            btnToggle.Click += new EventHandler(ToggleMonitoring_Click);

            var lblStatus = new Label
            {
                Text = "Uses System.Diagnostics.PerformanceCounter. Admin rights may be needed for some counters.",
                Font = new Font("Segoe UI", 8F, FontStyle.Italic),
                ForeColor = Color.LightGray,
                AutoSize = true,
                Location = new Point(20, y + 55),
                Size = new Size(560, 20)
            };

            this.Controls.Add(lblTitle);
            this.Controls.Add(btnToggle);
            this.Controls.Add(lblStatus);

            monitorTimer = new Timer { Interval = 1000 };
            monitorTimer.Tick += new EventHandler(MonitorTimer_Tick);
        }

        private void InitializeCounters()
        {
            try { cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total"); } catch { cpuCounter = null; }
            try { memoryCounter = new PerformanceCounter("Memory", "Available MBytes"); } catch { memoryCounter = null; }
            try { diskCounter = new PerformanceCounter("PhysicalDisk", "% Disk Time", "_Total"); } catch { diskCounter = null; }
        }

        private void ToggleMonitoring_Click(object sender, EventArgs e)
        {
            monitoring = !monitoring;
            if (monitoring)
            {
                monitorTimer.Start();
                btnToggle.Text = "Stop Monitoring";
                btnToggle.BackColor = Color.FromArgb(183, 28, 28);
            }
            else
            {
                monitorTimer.Stop();
                btnToggle.Text = "Start Monitoring";
                btnToggle.BackColor = Color.FromArgb(0, 150, 136);
            }
        }

        private void MonitorTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                if (cpuCounter != null)
                {
                    float cpu = cpuCounter.NextValue();
                    lblCpuValue.Text = string.Format("{0}%", cpu.ToString("0.0"));
                    pbCpu.Value = (int)Math.Min(100, Math.Max(0, cpu));
                }
                if (memoryCounter != null)
                {
                    float mem = memoryCounter.NextValue();
                    lblMemValue.Text = string.Format("{0} MB", mem.ToString("0"));
                    pbMem.Value = (int)Math.Min(100, Math.Max(0, mem / 10));
                }
                if (diskCounter != null)
                {
                    float disk = diskCounter.NextValue();
                    lblDiskValue.Text = string.Format("{0}%", disk.ToString("0.0"));
                    pbDisk.Value = (int)Math.Min(100, Math.Max(0, disk));
                }
            }
            catch (Exception ex)
            {
                lblCpuValue.Text = "Error";
                string msg = ex.Message.Length > 20 ? ex.Message.Substring(0, 20) : ex.Message;
                lblMemValue.Text = msg;
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (monitorTimer != null) monitorTimer.Stop();
            if (cpuCounter != null) cpuCounter.Dispose();
            if (memoryCounter != null) memoryCounter.Dispose();
            if (diskCounter != null) diskCounter.Dispose();
            base.OnFormClosing(e);
        }
    }
}
