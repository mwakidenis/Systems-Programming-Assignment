using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace SystemProgrammingApp
{
    public class ProcessForm : Form
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr OpenProcess(uint dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool CloseHandle(IntPtr hObject);

        [DllImport("ntdll.dll")]
        private static extern int NtSuspendProcess(IntPtr processHandle);

        [DllImport("ntdll.dll")]
        private static extern int NtResumeProcess(IntPtr processHandle);

        private const uint PROCESS_SUSPEND_RESUME = 0x0800;
        private const uint PROCESS_QUERY_INFORMATION = 0x0400;
        private const uint PROCESS_VM_READ = 0x0010;

        private ListView listViewProcesses;
        private Button btnList, btnStart, btnKill, btnSuspend, btnResume;
        private TextBox txtPath;

        public ProcessForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Module 1 - Process Management";
            this.Size = new Size(920, 540);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.FromArgb(30, 30, 30);
            this.ForeColor = Color.White;

            var lblTitle = new Label
            {
                Text = "Process Management",
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 172, 238),
                AutoSize = true,
                Location = new Point(20, 15),
                Size = new Size(200, 30)
            };

            listViewProcesses = new ListView
            {
                Location = new Point(20, 55),
                Size = new Size(860, 330),
                View = View.Details,
                FullRowSelect = true,
                GridLines = true,
                BackColor = Color.FromArgb(40, 40, 40),
                ForeColor = Color.White,
                HeaderStyle = ColumnHeaderStyle.Nonclickable
            };
            listViewProcesses.Columns.Add("PID", 70);
            listViewProcesses.Columns.Add("Process Name", 220);
            listViewProcesses.Columns.Add("Memory (MB)", 120);
            listViewProcesses.Columns.Add("Start Time", 190);
            listViewProcesses.Columns.Add("Status", 140);

            var btnPanel = new Panel
            {
                Location = new Point(20, 400),
                Size = new Size(860, 60),
                BackColor = Color.FromArgb(30, 30, 30)
            };

            btnList = CreateButton("List Processes", 10, 10, new EventHandler(ListProcesses_Click));
            btnStart = CreateButton("Start Process", 150, 10, new EventHandler(StartProcess_Click));
            btnKill = CreateButton("Stop (Kill)", 280, 10, new EventHandler(KillProcess_Click));
            btnSuspend = CreateButton("Pause (Suspend)", 400, 10, new EventHandler(SuspendProcess_Click));
            btnResume = CreateButton("Resume", 530, 10, new EventHandler(ResumeProcess_Click));

            txtPath = new TextBox
            {
                Location = new Point(660, 10),
                Size = new Size(220, 25),
                Text = ""
            };

            btnPanel.Controls.AddRange(new Control[] { btnList, btnStart, btnKill, btnSuspend, btnResume, txtPath });

            var btnBack = new Button
            {
                Text = "Back to Main Menu",
                Location = new Point(20, 440),
                Size = new Size(140, 30),
                BackColor = Color.FromArgb(45, 45, 48),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F)
            };
            btnBack.FlatAppearance.BorderColor = Color.FromArgb(60, 60, 60);
            btnBack.Click += new EventHandler(delegate(object s, EventArgs e) { this.Close(); });

            var lblStatus = new Label
            {
                Text = "Select a process, then click an action button. Admin rights required for kill/suspend/resume on system processes.",
                Font = new Font("Segoe UI", 8F, FontStyle.Italic),
                ForeColor = Color.LightGray,
                AutoSize = true,
                Location = new Point(20, 480),
                Size = new Size(860, 20)
            };

            this.Controls.Add(lblTitle);
            this.Controls.Add(listViewProcesses);
            this.Controls.Add(btnPanel);
            this.Controls.Add(btnBack);
            this.Controls.Add(lblStatus);
        }

        private Button CreateButton(string text, int x, int y, EventHandler handler)
        {
            var btn = new Button
            {
                Text = text,
                Location = new Point(x, y),
                Size = new Size(120, 30),
                BackColor = Color.FromArgb(45, 45, 48),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F)
            };
            btn.FlatAppearance.BorderColor = Color.FromArgb(60, 60, 60);
            btn.Click += handler;
            return btn;
        }

        private void ListProcesses_Click(object sender, EventArgs e)
        {
            listViewProcesses.Items.Clear();
            try
            {
                Process[] processes = Process.GetProcesses();
                foreach (Process p in processes)
                {
                    try
                    {
                        string mem = "N/A";
                        string start = "N/A";
                        try { mem = string.Format("{0} MB", p.PrivateMemorySize64 / 1024 / 1024); } catch { }
                        try { start = p.StartTime.ToString("yyyy-MM-dd HH:mm:ss"); } catch { }
                        string status = p.Responding ? "Running" : "Not Responding";

                        ListViewItem item = new ListViewItem(new string[] { p.Id.ToString(), p.ProcessName, mem, start, status });
                        listViewProcesses.Items.Add(item);
                    }
                    catch { /* skip processes we cannot read */ }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("Error listing processes: {0}", ex.Message), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void StartProcess_Click(object sender, EventArgs e)
        {
            string path = txtPath.Text.Trim();
            if (string.IsNullOrEmpty(path))
            {
                using (OpenFileDialog dlg = new OpenFileDialog { Title = "Select program to start", Filter = "Executables|*.exe|All files|*.*" })
                {
                    if (dlg.ShowDialog(this) != DialogResult.OK) return;
                    path = dlg.FileName;
                }
            }
            try
            {
                Process.Start(path);
                MessageBox.Show(string.Format("Started: {0}", path), "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("Failed to start process: {0}", ex.Message), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private Process GetSelectedProcess()
        {
            if (listViewProcesses.SelectedItems.Count == 0)
            {
                MessageBox.Show("Please select a process from the list.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return null;
            }
            int pid = int.Parse(listViewProcesses.SelectedItems[0].SubItems[0].Text);
            try
            {
                return Process.GetProcessById(pid);
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("Could not access process {0}: {1}", pid, ex.Message), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        private void KillProcess_Click(object sender, EventArgs e)
        {
            Process p = GetSelectedProcess();
            if (p == null) return;
            try
            {
                p.Kill();
                MessageBox.Show(string.Format("Process {0} ({1}) terminated.", p.Id, p.ProcessName), "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Win32Exception ex)
            {
                if (ex.NativeErrorCode == 5)
                {
                    MessageBox.Show("Access denied. Run Visual Studio as Administrator.", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    MessageBox.Show(string.Format("Failed to kill process: {0}", ex.Message), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("Failed to kill process: {0}", ex.Message), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SuspendProcess_Click(object sender, EventArgs e)
        {
            Process p = GetSelectedProcess();
            if (p == null) return;
            IntPtr hProcess = IntPtr.Zero;
            try
            {
                hProcess = OpenProcess(PROCESS_SUSPEND_RESUME | PROCESS_QUERY_INFORMATION | PROCESS_VM_READ, false, p.Id);
                if (hProcess == IntPtr.Zero)
                {
                    int err = Marshal.GetLastWin32Error();
                    MessageBox.Show(string.Format("OpenProcess failed (error {0}). Access denied - run as Administrator.", err), "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                int status = NtSuspendProcess(hProcess);
                if (status == 0)
                    MessageBox.Show(string.Format("Process {0} ({1}) suspended.", p.Id, p.ProcessName), "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                else
                    MessageBox.Show(string.Format("NtSuspendProcess returned status 0x{0:X8}.", status), "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Win32Exception ex)
            {
                if (ex.NativeErrorCode == 5)
                {
                    MessageBox.Show("Access denied. Run Visual Studio as Administrator.", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    MessageBox.Show(string.Format("Failed to suspend process: {0}", ex.Message), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("Failed to suspend process: {0}", ex.Message), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (hProcess != IntPtr.Zero) CloseHandle(hProcess);
            }
        }

        private void ResumeProcess_Click(object sender, EventArgs e)
        {
            Process p = GetSelectedProcess();
            if (p == null) return;
            IntPtr hProcess = IntPtr.Zero;
            try
            {
                hProcess = OpenProcess(PROCESS_SUSPEND_RESUME | PROCESS_QUERY_INFORMATION | PROCESS_VM_READ, false, p.Id);
                if (hProcess == IntPtr.Zero)
                {
                    int err = Marshal.GetLastWin32Error();
                    MessageBox.Show(string.Format("OpenProcess failed (error {0}). Access denied - run as Administrator.", err), "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                int status = NtResumeProcess(hProcess);
                if (status == 0)
                    MessageBox.Show(string.Format("Process {0} ({1}) resumed.", p.Id, p.ProcessName), "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                else
                    MessageBox.Show(string.Format("NtResumeProcess returned status 0x{0:X8}.", status), "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Win32Exception ex)
            {
                if (ex.NativeErrorCode == 5)
                {
                    MessageBox.Show("Access denied. Run Visual Studio as Administrator.", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    MessageBox.Show(string.Format("Failed to resume process: {0}", ex.Message), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("Failed to resume process: {0}", ex.Message), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (hProcess != IntPtr.Zero) CloseHandle(hProcess);
            }
        }
    }
}
