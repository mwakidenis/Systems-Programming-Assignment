using System;
using System.Drawing;
using System.ServiceProcess;
using System.Windows.Forms;

namespace SystemProgrammingApp
{
    public class ServicesForm : Form
    {
        private ListView listViewServices;
        private Button btnRefresh, btnStart, btnStop, btnRestart;

        public ServicesForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Module 2 - Windows Services Control";
            this.Size = new Size(820, 540);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.FromArgb(30, 30, 30);
            this.ForeColor = Color.White;

            var lblTitle = new Label
            {
                Text = "Windows Services Control",
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 172, 238),
                AutoSize = true,
                Location = new Point(20, 15),
                Size = new Size(220, 30)
            };

            listViewServices = new ListView
            {
                Location = new Point(20, 55),
                Size = new Size(760, 330),
                View = View.Details,
                FullRowSelect = true,
                GridLines = true,
                BackColor = Color.FromArgb(40, 40, 40),
                ForeColor = Color.White,
                HeaderStyle = ColumnHeaderStyle.Nonclickable
            };
            listViewServices.Columns.Add("Service Name", 220);
            listViewServices.Columns.Add("Display Name", 300);
            listViewServices.Columns.Add("Status", 120);

            var btnPanel = new Panel
            {
                Location = new Point(20, 400),
                Size = new Size(760, 60),
                BackColor = Color.FromArgb(30, 30, 30)
            };

            btnRefresh = CreateButton("Refresh List", 10, 10, new EventHandler(RefreshServices_Click));
            btnStart = CreateButton("Start Service", 160, 10, new EventHandler(StartService_Click));
            btnStop = CreateButton("Stop Service", 310, 10, new EventHandler(StopService_Click));
            btnRestart = CreateButton("Restart Service", 470, 10, new EventHandler(RestartService_Click));

            btnPanel.Controls.AddRange(new Control[] { btnRefresh, btnStart, btnStop, btnRestart });

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
                Text = "Select a service, then click an action. Admin rights required to start/stop services.",
                Font = new Font("Segoe UI", 8F, FontStyle.Italic),
                ForeColor = Color.LightGray,
                AutoSize = true,
                Location = new Point(20, 480),
                Size = new Size(760, 20)
            };

            this.Controls.Add(lblTitle);
            this.Controls.Add(listViewServices);
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
                Size = new Size(130, 30),
                BackColor = Color.FromArgb(45, 45, 48),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F)
            };
            btn.FlatAppearance.BorderColor = Color.FromArgb(60, 60, 60);
            btn.Click += handler;
            return btn;
        }

        private void RefreshServices_Click(object sender, EventArgs e)
        {
            listViewServices.Items.Clear();
            try
            {
                ServiceController[] services = ServiceController.GetServices();
                foreach (ServiceController sc in services)
                {
                    try
                    {
                        string status = sc.Status.ToString();
                        ListViewItem item = new ListViewItem(new string[] { sc.ServiceName, sc.DisplayName, status });
                        if (status == "Running") item.ForeColor = Color.LightGreen;
                        else if (status == "Stopped") item.ForeColor = Color.LightCoral;
                        else item.ForeColor = Color.LightYellow;
                        listViewServices.Items.Add(item);
                    }
                    catch { /* skip inaccessible services */ }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("Error listing services: {0}", ex.Message), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private ServiceController GetSelectedService()
        {
            if (listViewServices.SelectedItems.Count == 0)
            {
                MessageBox.Show("Please select a service from the list.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return null;
            }
            string name = listViewServices.SelectedItems[0].SubItems[0].Text;
            try { return new ServiceController(name); }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("Could not access service {0}: {1}", name, ex.Message), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        private void StartService_Click(object sender, EventArgs e)
        {
            ServiceController sc = GetSelectedService();
            if (sc == null) return;
            try
            {
                if (sc.Status == ServiceControllerStatus.Running)
                {
                    MessageBox.Show(string.Format("{0} is already running.", sc.ServiceName), "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                sc.Start();
                sc.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(10));
                MessageBox.Show(string.Format("Service {0} started.", sc.ServiceName), "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (System.ComponentModel.Win32Exception ex)
            {
                if (ex.NativeErrorCode == 5)
                {
                    MessageBox.Show("Access denied. Run Visual Studio as Administrator.", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    MessageBox.Show(string.Format("Failed to start service: {0}", ex.Message), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("Failed to start service: {0}", ex.Message), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void StopService_Click(object sender, EventArgs e)
        {
            ServiceController sc = GetSelectedService();
            if (sc == null) return;
            try
            {
                if (sc.Status == ServiceControllerStatus.Stopped)
                {
                    MessageBox.Show(string.Format("{0} is already stopped.", sc.ServiceName), "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                sc.Stop();
                sc.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(10));
                MessageBox.Show(string.Format("Service {0} stopped.", sc.ServiceName), "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (System.ComponentModel.Win32Exception ex)
            {
                if (ex.NativeErrorCode == 5)
                {
                    MessageBox.Show("Access denied. Run Visual Studio as Administrator.", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    MessageBox.Show(string.Format("Failed to stop service: {0}", ex.Message), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("Failed to stop service: {0}", ex.Message), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RestartService_Click(object sender, EventArgs e)
        {
            ServiceController sc = GetSelectedService();
            if (sc == null) return;
            try
            {
                if (sc.Status == ServiceControllerStatus.Running)
                {
                    sc.Stop();
                    sc.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(10));
                }
                sc.Start();
                sc.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(10));
                MessageBox.Show(string.Format("Service {0} restarted.", sc.ServiceName), "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (System.ComponentModel.Win32Exception ex)
            {
                if (ex.NativeErrorCode == 5)
                {
                    MessageBox.Show("Access denied. Run Visual Studio as Administrator.", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    MessageBox.Show(string.Format("Failed to restart service: {0}", ex.Message), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("Failed to restart service: {0}", ex.Message), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
