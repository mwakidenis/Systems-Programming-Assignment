using System;
using System.Drawing;
using System.IO;
using System.Management;
using System.Text;
using System.Windows.Forms;

namespace SystemProgrammingApp
{
    public class HardwareForm : Form
    {
        private TextBox txtOutput;

        public HardwareForm()
        {
            InitializeComponent();
            LoadSystemInfo();
        }

        private void InitializeComponent()
        {
            this.Text = "Module 4 - Hardware & OS Information";
            this.Size = new Size(740, 560);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.FromArgb(30, 30, 30);
            this.ForeColor = Color.White;

            var lblTitle = new Label
            {
                Text = "Hardware & OS Information",
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 172, 238),
                AutoSize = true,
                Location = new Point(20, 15),
                Size = new Size(220, 30)
            };

            txtOutput = new TextBox
            {
                Location = new Point(20, 55),
                Size = new Size(680, 420),
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Vertical,
                BackColor = Color.FromArgb(40, 40, 40),
                ForeColor = Color.FromArgb(0, 255, 128),
                Font = new Font("Consolas", 9F),
                WordWrap = false
            };

            var btnRefresh = new Button
            {
                Text = "Refresh",
                Location = new Point(20, 490),
                Size = new Size(100, 30),
                BackColor = Color.FromArgb(45, 45, 48),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F)
            };
            btnRefresh.FlatAppearance.BorderColor = Color.FromArgb(60, 60, 60);
            btnRefresh.Click += new EventHandler(delegate(object s, EventArgs e) { LoadSystemInfo(); });

            var btnClose = new Button
            {
                Text = "Back to Main Menu",
                Location = new Point(140, 490),
                Size = new Size(140, 30),
                BackColor = Color.FromArgb(45, 45, 48),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F)
            };
            btnClose.FlatAppearance.BorderColor = Color.FromArgb(60, 60, 60);
            btnClose.Click += new EventHandler(delegate(object s, EventArgs e) { this.Close(); });

            this.Controls.Add(lblTitle);
            this.Controls.Add(txtOutput);
            this.Controls.Add(btnRefresh);
            this.Controls.Add(btnClose);
        }

        private void LoadSystemInfo()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("========================================");
            sb.AppendLine("HARDWARE & OS INFORMATION");
            sb.AppendLine("========================================");
            sb.AppendLine();

            sb.AppendLine("[ OPERATING SYSTEM ]");
            sb.AppendLine(string.Format("  OS Version      : {0}", Environment.OSVersion.VersionString));
            sb.AppendLine(string.Format("  Platform        : {0}", Environment.OSVersion.Platform));
            sb.AppendLine(string.Format("  Computer Name   : {0}", Environment.MachineName));
            sb.AppendLine(string.Format("  User Domain     : {0}", Environment.UserDomainName));
            sb.AppendLine(string.Format("  Processor Count : {0}", Environment.ProcessorCount));
            sb.AppendLine(string.Format("  64-Bit OS       : {0}", Environment.Is64BitOperatingSystem));
            sb.AppendLine(string.Format("  64-Bit Process  : {0}", Environment.Is64BitProcess));
            sb.AppendLine();

            sb.AppendLine("[ MEMORY ]");
            try
            {
                ManagementObjectSearcher memSearcher = new ManagementObjectSearcher("SELECT TotalPhysicalMemory, AvailablePhysicalMemory FROM Win32_OperatingSystem");
                foreach (ManagementObject mo in memSearcher.Get())
                {
                    ulong total = ToULong(mo["TotalPhysicalMemory"]);
                    ulong avail = ToULong(mo["AvailablePhysicalMemory"]);
                    sb.AppendLine(string.Format("  Total Physical Memory : {0}", FormatBytes(total)));
                    sb.AppendLine(string.Format("  Available Memory      : {0}", FormatBytes(avail)));
                }
            }
            catch (Exception ex)
            {
                sb.AppendLine(string.Format("  Error reading memory: {0}", ex.Message));
            }
            sb.AppendLine();

            sb.AppendLine("[ PROCESSOR ]");
            try
            {
                ManagementObjectSearcher cpuSearcher = new ManagementObjectSearcher("SELECT Name, MaxClockSpeed, NumberOfCores, NumberOfLogicalProcessors FROM Win32_Processor");
                foreach (ManagementObject mo in cpuSearcher.Get())
                {
                    sb.AppendLine(string.Format("  Name                  : {0}", mo["Name"]));
                    sb.AppendLine(string.Format("  Max Clock Speed (MHz) : {0}", mo["MaxClockSpeed"]));
                    sb.AppendLine(string.Format("  Number of Cores       : {0}", mo["NumberOfCores"]));
                    sb.AppendLine(string.Format("  Logical Processors    : {0}", mo["NumberOfLogicalProcessors"]));
                }
            }
            catch (Exception ex)
            {
                sb.AppendLine(string.Format("  Error reading CPU: {0}", ex.Message));
            }
            sb.AppendLine();

            sb.AppendLine("[ LOGICAL DISK DRIVES ]");
            try
            {
                foreach (DriveInfo drive in DriveInfo.GetDrives())
                {
                    sb.AppendLine(string.Format("  Drive {0} : {1}", drive.Name, drive.DriveType));
                    if (drive.IsReady)
                    {
                        sb.AppendLine(string.Format("    Volume Label  : {0}", drive.VolumeLabel ?? "(none)"));
                        sb.AppendLine(string.Format("    File System   : {0}", drive.DriveFormat ?? "N/A"));
                    sb.AppendLine(string.Format("    Total Space   : {0}", FormatBytes((ulong)drive.TotalSize)));
                    sb.AppendLine(string.Format("    Free Space    : {0}", FormatBytes((ulong)drive.AvailableFreeSpace)));
                    }
                    else
                    {
                        sb.AppendLine("    Status        : Not Ready");
                    }
                }
            }
            catch (Exception ex)
            {
                sb.AppendLine(string.Format("  Error reading drives: {0}", ex.Message));
            }
            sb.AppendLine();

            txtOutput.Text = sb.ToString();
        }

        private static ulong ToULong(object value)
        {
            if (value == null) return 0;
            try { return Convert.ToUInt64(value); } catch { return 0; }
        }

        private static string FormatBytes(ulong bytes)
        {
            string[] sizes = new string[] { "B", "KB", "MB", "GB", "TB" };
            double len = (double)bytes;
            int order = 0;
            while (len >= 1024 && order < sizes.Length - 1)
            {
                len /= 1024;
                order++;
            }
            return string.Format("{0:0.##} {1}", len, sizes[order]);
        }
    }
}
