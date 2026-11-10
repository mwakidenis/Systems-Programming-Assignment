using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SystemProgrammingApp
{
    public class ThreadMemoryForm : Form
    {
        private ComboBox cboProcesses;
        private Button btnInspect;
        private TextBox txtOutput;

        public ThreadMemoryForm()
        {
            InitializeComponent();
            RefreshProcessList();
        }

        private void InitializeComponent()
        {
            this.Text = "Module 5 - Thread & Memory Inspector";
            this.Size = new Size(820, 560);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.FromArgb(30, 30, 30);
            this.ForeColor = Color.White;

            var lblTitle = new Label
            {
                Text = "Thread & Memory Inspector",
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 172, 238),
                AutoSize = true,
                Location = new Point(20, 15),
                Size = new Size(220, 30)
            };

            var cboPanel = new Panel
            {
                Location = new Point(20, 55),
                Size = new Size(760, 40),
                BackColor = Color.FromArgb(30, 30, 30)
            };

            var lblSelect = new Label
            {
                Text = "Select Process:",
                Location = new Point(10, 12),
                Size = new Size(80, 20),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };

            cboProcesses = new ComboBox
            {
                Location = new Point(100, 10),
                Size = new Size(420, 25),
                DropDownStyle = ComboBoxStyle.DropDownList,
                BackColor = Color.FromArgb(40, 40, 40),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9F)
            };

            var btnRefreshList = new Button
            {
                Text = "Refresh List",
                Location = new Point(540, 10),
                Size = new Size(100, 25),
                BackColor = Color.FromArgb(45, 45, 48),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F)
            };
            btnRefreshList.FlatAppearance.BorderColor = Color.FromArgb(60, 60, 60);
            btnRefreshList.Click += new EventHandler(delegate(object s, EventArgs e) { RefreshProcessList(); });

            btnInspect = new Button
            {
                Text = "Inspect Threads & Modules",
                Location = new Point(660, 10),
                Size = new Size(160, 25),
                BackColor = Color.FromArgb(0, 150, 136),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };
            btnInspect.FlatAppearance.BorderColor = Color.FromArgb(0, 120, 106);
            btnInspect.Click += new EventHandler(Inspect_Click);

            cboPanel.Controls.AddRange(new Control[] { lblSelect, cboProcesses, btnRefreshList, btnInspect });

            txtOutput = new TextBox
            {
                Location = new Point(20, 105),
                Size = new Size(760, 360),
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Vertical,
                BackColor = Color.FromArgb(40, 40, 40),
                ForeColor = Color.FromArgb(0, 255, 128),
                Font = new Font("Consolas", 9F),
                WordWrap = false
            };

            this.Controls.Add(lblTitle);
            this.Controls.Add(cboPanel);
            this.Controls.Add(txtOutput);

            var btnBack = new Button
            {
                Text = "Back to Main Menu",
                Location = new Point(20, 475),
                Size = new Size(140, 30),
                BackColor = Color.FromArgb(45, 45, 48),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F)
            };
            btnBack.FlatAppearance.BorderColor = Color.FromArgb(60, 60, 60);
            btnBack.Click += new EventHandler(delegate(object s, EventArgs e) { this.Close(); });
            this.Controls.Add(btnBack);

            var lblStatus = new Label
            {
                Text = "Inspects process threads (concurrency) and loaded modules (DLL memory allocation). Admin rights may be needed.",
                Font = new Font("Segoe UI", 8F, FontStyle.Italic),
                ForeColor = Color.LightGray,
                AutoSize = true,
                Location = new Point(180, 482),
                Size = new Size(600, 20)
            };

            this.Controls.Add(lblStatus);
        }

        private void RefreshProcessList()
        {
            cboProcesses.Items.Clear();
            try
            {
                Process[] processes = Process.GetProcesses();
                var sorted = processes.OrderBy(p => p.ProcessName).ThenBy(p => p.Id).ToArray();
                foreach (Process p in sorted)
                {
                    try { cboProcesses.Items.Add(string.Format("{0} (PID {1})", p.ProcessName, p.Id)); } catch { }
                }
                if (cboProcesses.Items.Count > 0) cboProcesses.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("Error listing processes: {0}", ex.Message), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private Process GetSelectedProcess()
        {
            if (cboProcesses.SelectedItem == null) return null;
            string text = cboProcesses.SelectedItem.ToString();
            int start = text.IndexOf("(PID ") + 5;
            int end = text.IndexOf(")", start);
            if (start < 4 || end < 0) return null;
            int pid = int.Parse(text.Substring(start, end - start));
            try { return Process.GetProcessById(pid); }
            catch { return null; }
        }

        private void Inspect_Click(object sender, EventArgs e)
        {
            Process p = GetSelectedProcess();
            if (p == null) return;

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("========================================");
            sb.AppendLine("THREAD & MEMORY INSPECTION");
            sb.AppendLine(string.Format("Process : {0} (PID {1})", p.ProcessName, p.Id));
            sb.AppendLine("========================================");
            sb.AppendLine();

            sb.AppendLine("[ THREADS (Concurrency) ]");
            try
            {
                ProcessThreadCollection threads = p.Threads;
                sb.AppendLine(string.Format("  Total Threads : {0}", threads.Count));
                sb.AppendLine("  Thread ID     State                 Start Address");
                sb.AppendLine("  ----------    -----                 -------------");
                foreach (ProcessThread t in threads)
                {
                    sb.AppendLine(string.Format("  {0,-12} {1,-18} 0x{2:X}", t.Id, t.ThreadState, t.StartAddress.ToInt64()));
                }
            }
            catch (Exception ex)
            {
                sb.AppendLine(string.Format("  Error reading threads: {0}", ex.Message));
            }
            sb.AppendLine();

            sb.AppendLine("[ MODULES (DLL Memory Allocation) ]");
            try
            {
                ProcessModuleCollection modules = p.Modules;
                sb.AppendLine(string.Format("  Total Modules : {0}", modules.Count));
                sb.AppendLine("  Module Name                                      Base Address");
                sb.AppendLine("  ------------                                     ------------");
                foreach (ProcessModule m in modules)
                {
                    sb.AppendLine(string.Format("  {0,-40} 0x{1:X}", m.ModuleName, m.BaseAddress.ToInt64()));
                }
            }
            catch (Exception ex)
            {
                sb.AppendLine(string.Format("  Error reading modules: {0}", ex.Message));
            }
            sb.AppendLine();
            sb.AppendLine("Note: Thread state and module base addresses demonstrate how the OS");
            sb.AppendLine("allocates memory for DLLs and manages concurrent execution within a process.");

            txtOutput.Text = sb.ToString();
        }
    }
}
