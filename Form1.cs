using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
//using System.Linq;
using System.Text;
//using System.Threading.Tasks;
using System.Windows.Forms;
using System.ServiceProcess;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.IO;
using NETCONLib;
using System.Net;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        public const string RegPath = @"SOFTWARE\ZYWifi";
        public const string RemenberPW = "RemenberPW";
        public const string WifiID = "WifiID";
        public const string WifiKey = "WifiKey";
        public const string StuID = "StuID";
        public const string StuKey = "StuKey";
        public const string RememberYes = "Yes";
        public const string RememberNo = "No";
        public const string SharedNet = "ShareNetName";
        public const string WifiNet = "WifiNetName";

        public ServiceController getFirewallService()
        {
            ServiceController[] services = System.ServiceProcess.ServiceController.GetServices();
            foreach (ServiceController sc in services)
            {
                if (sc.DisplayName.Contains("Windows Firewall"))
                {
                    return sc;
                }
            }
            return null;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Toggle the Telnet service:
            // If it is started (running, paused, etc), stop the service.
            // If it is stopped, start the service.状态
            //ServiceController sc = new ServiceController("Windows Firewall");
            ServiceController sc = getFirewallService();
            //Console.WriteLine("The Windows Firewall service status is currently set to {0} ", sc.Status.ToString());

            if ((sc.Status.Equals(ServiceControllerStatus.Stopped)) || (sc.Status.Equals(ServiceControllerStatus.StopPending)))
            {
                // Start the service if the current status is stopped.
                Console.WriteLine("Starting the Windows Firewall service... ");
                sc.Start();
            }
            else
            {
                MessageBox.Show(this, "服务已开启，不能重复开启！");
                return;
            }
            // Refresh and display the current service status.
            sc.Refresh();
            lblStatus.Text = sc.Status.ToString();
            //Console.WriteLine("The Windows Firewall service status is now set to {0}. ", sc.Status.ToString());
            MessageBox.Show(this, "服务开启成功！");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // Toggle the Telnet service:
            // If it is started (running, paused, etc), stop the service.
            // If it is stopped, start the service.
            ServiceController sc = new ServiceController("Windows Firewall");
            //Console.WriteLine("The Windows Firewall service status is currently set to {0} ", sc.Status.ToString());

            if (!((sc.Status.Equals(ServiceControllerStatus.Stopped)) || (sc.Status.Equals(ServiceControllerStatus.StopPending))))
            {
                // Stop the service if its status is not set to "Stopped ".
                Console.WriteLine("Stopping the Windows Firewall service... ");
                sc.Stop();
            }
            else
            {
                MessageBox.Show(this, "服务已关闭，不能重复关闭！");
                return;
            }

            // Refresh and display the current service status.
            sc.Refresh();
            lblStatus.Text = sc.Status.ToString();
            MessageBox.Show(this, "服务成功关闭！");
            //Console.WriteLine("The Windows Firewall service status is now set to {0}. ", sc.Status.ToString());
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //ServiceController sc = new ServiceController("%Windows Firewall%");
            this.Location = new Point(this.Location.X,this.Location.Y-100);

            ServiceController sc = getFirewallService();

            lblStatus.Text = sc.Status.ToString();

            if (SearchItemRegEdit(RegPath, StuID) && SearchItemRegEdit(RegPath, StuKey))
            {
                txtStuID.Text = getValueRegEdit(RegPath, StuID);
                txtStuKEY.Text = getValueRegEdit(RegPath, StuKey);
            }


            if (SearchItemRegEdit(RegPath, RemenberPW) && getValueRegEdit(RegPath, RemenberPW) == RememberYes)
            {
                checkBox2.Checked = true;
                if (SearchItemRegEdit(RegPath, WifiID) && SearchItemRegEdit(RegPath, WifiKey))
                {
                    txtName.Text = getValueRegEdit(RegPath, WifiID);
                    txtPassword.Text = getValueRegEdit(RegPath, WifiKey);
                }
            }
            else
            {
                try
                {
                    if (SearchItemRegEdit(RegPath, WifiID))
                    {
                        txtName.Text = getValueRegEdit(RegPath, WifiID);
                    }
                }
                catch
                {
                    txtName.Text = "";
                }
                checkBox2.Checked = false;
            }
            bindCombox();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //ServiceController sc = new ServiceController("Windows Firewall");
            ServiceController sc = getFirewallService();

            if ((sc.Status.Equals(ServiceControllerStatus.Stopped)) || (sc.Status.Equals(ServiceControllerStatus.StopPending)))
            {
                MessageBox.Show(this, "服务未开启，请先开启服务！");
                return;
            }
            if (txtName.Text.Trim() != "" && txtPassword.Text.Trim() != "")
            {
                if (txtPassword.Text.Trim().Length < 6)
                {
                    MessageBox.Show("密码长度至少6位！");
                    return;
                }
                string strCommand1 = string.Format("netsh wlan set hostednetwork mode=allow ssid={0} key={1}", txtName.Text.Trim(), txtPassword.Text.Trim());
                string strCommand2 = string.Format("netsh wlan start hostednetwork");
                string result1 = CommandDo.Execute(strCommand1, 10000);
                if (result1 == "")
                {
                    MessageBox.Show(this, "Wifi热点建立失败！");
                    return;
                }
                string result2 = CommandDo.Execute(strCommand2, 10000);
                if (result2 != "")
                {
                    MessageBox.Show(this, "Wifi热点成功开启！");
                }
                else
                {
                    MessageBox.Show(this, "Wifi热点开启失败！");
                }
            }
            if (checkBox2.Checked)
            {
                try
                {

                    if (SearchItemRegEdit(RegPath, WifiID) && SearchItemRegEdit(RegPath, WifiKey))
                    {
                        SetValueRegEdit(RegPath, WifiID, txtName.Text.Trim());
                        SetValueRegEdit(RegPath, WifiKey, txtPassword.Text.Trim());
                        SetValueRegEdit(RegPath, RemenberPW, RememberYes);
                        SetValueRegEdit(RegPath, SharedNet, comboBox1.Text);
                        SetValueRegEdit(RegPath, WifiNet, comboBox2.Text);
                    }
                    else
                    {
                        if (SearchItemRegEdit(RegPath, StuID) && SearchItemRegEdit(RegPath, StuKey))
                        {
                            SetValueRegEdit(RegPath, WifiID, txtName.Text.Trim());
                            SetValueRegEdit(RegPath, WifiKey, txtPassword.Text.Trim());
                            SetValueRegEdit(RegPath, RemenberPW, RememberYes);
                            SetValueRegEdit(RegPath, SharedNet, comboBox1.Text);
                            SetValueRegEdit(RegPath, WifiNet, comboBox2.Text);
                        }
                        else
                        {
                            CreateItemRegEdit(RegPath);
                            SetValueRegEdit(RegPath, WifiID, txtName.Text.Trim());
                            SetValueRegEdit(RegPath, WifiKey, txtPassword.Text.Trim());
                            SetValueRegEdit(RegPath, RemenberPW, RememberYes);
                            SetValueRegEdit(RegPath, SharedNet, comboBox1.Text);
                            SetValueRegEdit(RegPath, WifiNet, comboBox2.Text);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else
            {
                SetValueRegEdit(RegPath, WifiID, txtName.Text.Trim());
                SetValueRegEdit(RegPath, WifiKey, "");
                SetValueRegEdit(RegPath, RemenberPW, RememberNo);
                SetValueRegEdit(RegPath, SharedNet, comboBox1.Text);
                SetValueRegEdit(RegPath, WifiNet, comboBox2.Text);
            }
            SetShare();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            string strCommand1 = string.Format("netsh wlan stop hostednetwork");
            string result1 = CommandDo.Execute(strCommand1, 10000);
            if (result1 == "")
            {
                MessageBox.Show(this, "Wifi热点关闭失败！");
            }
            else
            {
                MessageBox.Show(this, "Wifi热点已关闭！");
            }
            CloseShare();
        }

        /// DOS命令输出类
        /// 
        public class CommandDo
        {
            /**/
            /// 

            /// 执行DOS命令，返回DOS命令的输出
            /// 
            /// dos命令
            /// 返回输出，如果发生异常，返回空字符串
            public static string Execute(string dosCommand)
            {
                return Execute(dosCommand, 6 * 1000);
            }
            /**/
            /// 

            /// 执行DOS命令，返回DOS命令的输出
            /// 
            /// dos命令
            /// 等待命令执行的时间（单位：毫秒），如果设定为0，则无限等待
            /// 返回输出，如果发生异常，返回空字符串
            public static string Execute(string dosCommand, int milliseconds)
            {
                string output = "";     //输出字符串
                if (dosCommand != null && dosCommand != "")
                {
                    Process process = new Process();     //创建进程对象
                    ProcessStartInfo startInfo = new ProcessStartInfo();
                    startInfo.FileName = "cmd.exe";      //设定需要执行的命令
                    startInfo.Arguments = "/C " + dosCommand;   //设定参数，其中的“/C”表示执行完命令后马上退出
                    startInfo.UseShellExecute = false;     //不使用系统外壳程序启动
                    startInfo.RedirectStandardInput = false;   //不重定向输入
                    startInfo.RedirectStandardOutput = true;   //重定向输出
                    startInfo.CreateNoWindow = true;     //不创建窗口
                    process.StartInfo = startInfo;
                    try
                    {
                        if (process.Start())       //开始进程
                        {
                            if (milliseconds == 0)
                                process.WaitForExit();     //这里无限等待进程结束
                            else
                                process.WaitForExit(milliseconds);  //这里等待进程结束，等待时间为指定的毫秒
                            output = process.StandardOutput.ReadToEnd();//读取进程的输出
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                    finally
                    {
                        if (process != null)
                            process.Close();
                    }
                }
                return output;
            }
        }

        private void label3_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(this, "退出后将关闭wifi热点和退出网络，确定退出？", "提示", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                string strCommand1 = string.Format("netsh wlan stop hostednetwork");
                string result1 = CommandDo.Execute(strCommand1, 10000);
                if (result1 == "")
                {
                    MessageBox.Show(this, "Wifi热点关闭失败！");
                }
                else
                {
                    //setInfo(checkBox2.Checked, txtName.Text.Trim(), txtPassword.Text.Trim());
                    Application.Exit();
                }
                CloseShare();
                if (lblMSG.Text.Contains("成功") || lblMSG.Text.Trim() =="")
                {
                    btnLogout_Click(sender, e);
                }
            }
        }

        private void label5_Click(object sender, EventArgs e)
        {
            if (this.WindowState != FormWindowState.Minimized)
            {

                this.Visible = false;   //隐藏窗体
                notifyIcon1.Visible = true; //任务栏显示图标
                notifyIcon1.ShowBalloonTip(3500, "提示", "双击恢复窗口", ToolTipIcon.Info); //出显汽泡提示，可以不用
                this.ShowInTaskbar = false; //从状态栏中隐藏

            }
        }

        private void notifyIcon1_DoubleClick(object sender, EventArgs e)
        {
            this.Visible = true; //显示窗体
            WindowState = FormWindowState.Normal;  //恢复窗体默认大小
            this.TopMost = true;
            this.TopMost = false;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                txtPassword.PasswordChar = (char)0;
            }
            else
            {
                txtPassword.PasswordChar = '*';
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            string strCommand1 = string.Format("netsh wlan set hostednetwork mode=allow");
            string result1 = CommandDo.Execute(strCommand1, 10000);
            if (result1 == "")
            {
                MessageBox.Show(this, "启用虚拟WiFi网卡失败！");
                return;
            }
            else
            {
                MessageBox.Show(this, "启用虚拟WiFi网卡成功！");
                return;
            }
        }

        private Point m_MousePoint;
        private Point m_LastPoint;

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            this.m_LastPoint = this.Location;
            this.m_MousePoint = this.PointToScreen(e.Location);
            this.Cursor = Cursors.SizeAll; ;
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            this.Cursor = Cursors.Default;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (e.Button == MouseButtons.Left)
            {
                Point t = this.PointToScreen(e.Location);
                Point l = this.m_LastPoint;

                l.Offset(t.X - this.m_MousePoint.X, t.Y - this.m_MousePoint.Y);
                this.Location = l;
            }
        }

        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();
        [DllImport("user32.dll")]
        public static extern bool SendMessage(IntPtr hwnd, int wMsg, int wParam, int lParam);
        public const int WM_SYSCOMMAND = 0x0112;
        public const int SC_MOVE = 0xF010;
        public const int HTCAPTION = 0x0002;

        public void ShapedForm_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, WM_SYSCOMMAND, SC_MOVE + HTCAPTION, 0);
        }

        //private string getInfo(string infoName)
        //{
        //    string fileName = Application.StartupPath + @"/WifiController/WifiData.wf";
        //    if (File.Exists(fileName))
        //    {
        //        try
        //        {
        //            StreamReader sr = new StreamReader(fileName, System.Text.Encoding.Default);
        //            string line = "";
        //            string info = "";
        //            while ((line = sr.ReadLine()) != null)
        //            {
        //                if (line.Contains(infoName))
        //                {
        //                    info = line.Substring(line.IndexOf('=') + 1).Trim();
        //                }
        //            }
        //            sr.Close();
        //            return info;
        //        }
        //        catch(Exception ex)
        //        {
        //            MessageBox.Show(ex.Message);
        //            return "";
        //        }
        //    }
        //    else
        //    {
        //        return "";
        //    }
        //}

        //private bool setInfo(bool remember,string UserName,string Password)
        //{
        //    string fileName = Application.StartupPath + @"/WifiController/WifiData.wf";
        //    string path = Application.StartupPath + @"/WifiController";
        //    try
        //    {
        //        if (!Directory.Exists(path))
        //        {
        //            //目标目录不存在则创建
        //            try
        //            {
        //                Directory.CreateDirectory(path);
        //                using (File.Create(fileName)) { };
        //            }
        //            catch (Exception ex)
        //            {
        //                throw new Exception("创建目标目录失败：" + ex.Message);
        //            }
        //        }
        //        using (StreamWriter sw = new StreamWriter(fileName, false, Encoding.Default, 512))
        //        {
        //            try
        //            {
        //                if (remember)
        //                {
        //                    sw.WriteLine("UserName = " + UserName);
        //                    sw.WriteLine("Password = " + Password);
        //                    sw.WriteLine("Remember = 1");
        //                }
        //                else
        //                {
        //                    sw.WriteLine("UserName = ");
        //                    sw.WriteLine("Password = ");
        //                    sw.WriteLine("Remember = 0");
        //                }
        //                return true;
        //            }
        //            catch
        //            {
        //                return false;
        //            }
        //        }
        //    }
        //    catch
        //    {
        //        return false;
        //    }
        //}

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {

            //if (!setInfo(checkBox2.Checked, txtName.Text.Trim(), txtPassword.Text.Trim()))
            //{
            //    MessageBox.Show(this, "文件读写出错，请重试！");
            //    checkBox2.Checked = false;
            //}
        }
        #region

        /// <summary>
        /// 获得要找到的注册表项
        /// </summary>
        /// <param name="path">注册表路经</param>
        /// <returns>返回注册表对象</returns>
        public bool CreateItemRegEdit(string path)
        {
            try
            {
                Microsoft.Win32.RegistryKey obj = Microsoft.Win32.Registry.LocalMachine;

                obj.CreateSubKey(path);

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }

        /// <summary>
        /// 设置注册表项下面的值
        /// </summary>
        /// <param name="path">路经</param>
        /// <param name="name">名称</param>
        /// <param name="value">值</param>
        /// <returns>是否成功</returns>
        public int SetValueRegEdit(string path, string name, string value)
        {
            try
            {
                Microsoft.Win32.RegistryKey obj = Microsoft.Win32.Registry.LocalMachine;
                Microsoft.Win32.RegistryKey objItem = obj.OpenSubKey(path, true);
                objItem.SetValue(name, value);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return 0;
            }
            return 1;
        }

        /// <summary>
        /// 查看注册表指定项的值
        /// </summary>
        /// <param name="path">路经</param>
        /// <param name="name">项名称</param>
        /// <returns>项值</returns>
        public string getValueRegEdit(string path, string name)
        {
            string value;
            try
            {
                Microsoft.Win32.RegistryKey obj = Microsoft.Win32.Registry.LocalMachine;
                Microsoft.Win32.RegistryKey objItem = obj.OpenSubKey(path);
                value = objItem.GetValue(name).ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return "";
            }
            return value;
        }


        /// <summary>
        /// 查看注册表项是否存在
        /// </summary>
        /// <param name="value">路经</param>
        /// <param name="name">项名称</param>
        /// <returns>是否存在</returns>
        public bool SearchItemRegEdit(string path, string name)
        {
            //string[] subkeyNames;
            try
            {
                Microsoft.Win32.RegistryKey hkml = Microsoft.Win32.Registry.LocalMachine;
                Microsoft.Win32.RegistryKey software = hkml.OpenSubKey(path);
                if (software.GetValue(name) != null && software.GetValue(name).ToString() != "")
                {
                    hkml.Close();
                    return true;
                }
                else
                {
                    hkml.Close();
                    return false;
                }

                //subkeyNames = software.GetSubKeyNames();
                ////取得该项下所有子项的名称的序列，并传递给预定的数组中  
                //foreach (string keyName in subkeyNames)   //遍历整个数组                 
                //{
                //    if (keyName.ToUpper() == name.ToUpper()) //判断子项的名称  
                //    {
                //        hkml.Close();
                //        return true;
                //    }

                //}
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }

        }

        /// <summary>
        /// 查看注册表的值是否存在
        /// </summary>
        /// <param name="value">路经</param>
        /// <param name="value">查看的值</param>
        /// <returns>是否成功</returns>
        public bool SearchValueRegEdit(string path, string value)
        {

            string[] subkeyNames;

            Microsoft.Win32.RegistryKey hkml = Microsoft.Win32.Registry.LocalMachine;

            Microsoft.Win32.RegistryKey software = hkml.OpenSubKey(path);

            subkeyNames = software.GetValueNames();

            //取得该项下所有键值的名称的序列，并传递给预定的数组中  

            foreach (string keyName in subkeyNames)
            {

                if (keyName.ToUpper() == value.ToUpper())   //判断键值的名称  
                {

                    hkml.Close();

                    return true;

                }

            }

            hkml.Close();

            return false;

        }
        #endregion

        public void bindCombox()
        {
            NetSharingManager manager = new NetSharingManager();
            var connections = manager.EnumEveryConnection;
            foreach (INetConnection c in connections)
            {
                var props = manager.NetConnectionProps[c];
                comboBox1.Items.Add(props.Name);
                comboBox2.Items.Add(props.Name);
            }
            try
            {

                if (SearchItemRegEdit(RegPath, SharedNet) && SearchItemRegEdit(RegPath, WifiNet))
                {
                    string share = getValueRegEdit(RegPath, SharedNet);
                    string wifi = getValueRegEdit(RegPath, WifiNet);
                    comboBox1.Text = share;
                    comboBox2.Text = wifi;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void SetShare()
        {
            try
            {
                string connectionToShare = comboBox1.Text; // 被共享的网络连接
                string sharedForConnection = comboBox2.Text; // 需要共享的网络连接

                var manager = new NetSharingManager();
                var connections = manager.EnumEveryConnection;

                foreach (INetConnection c in connections)
                {
                    var props = manager.NetConnectionProps[c];
                    var sharingCfg = manager.INetSharingConfigurationForINetConnection[c];
                    if (props.Name == connectionToShare)
                    {
                        sharingCfg.EnableSharing(tagSHARINGCONNECTIONTYPE.ICSSHARINGTYPE_PUBLIC);
                    }
                    else if (props.Name == sharedForConnection)
                    {
                        sharingCfg.EnableSharing(tagSHARINGCONNECTIONTYPE.ICSSHARINGTYPE_PRIVATE);
                    }
                }
            }
            catch
            {
                MessageBox.Show("请打开网络和共享中心·查看是不是已经连接Internet！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        public void CloseShare()
        {
            try
            {
                string connectionToShare = comboBox1.Text; // 被共享的网络连接
                //string sharedForConnection = comboBox2.Text; // 需要共享的网络连接

                var manager = new NetSharingManager();
                var connections = manager.EnumEveryConnection;

                foreach (INetConnection c in connections)
                {
                    var props = manager.NetConnectionProps[c];
                    var sharingCfg = manager.INetSharingConfigurationForINetConnection[c];
                    if (props.Name == connectionToShare)
                    {
                        sharingCfg.DisableSharing();
                    }
                    //else if (props.Name == sharedForConnection)
                    //{
                    //    sharingCfg.EnableSharing(tagSHARINGCONNECTIONTYPE.ICSSHARINGTYPE_PRIVATE);
                    //}
                }
            }
            catch
            {
                MessageBox.Show("请打开网络和共享中心·查看是不是已经连接Internet！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void lblWIFI_Click(object sender, EventArgs e)
        {
            if (lblWIFI.Text.Trim() == "展开WIFI")
            {
                this.ClientSize = new Size(285, 400);
                lblWIFI.Text = "隐藏WIFI";
            }
            else
            {
                this.ClientSize = new Size(285, 157);
                lblWIFI.Text = "展开WIFI";
            }
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtStuID.Text.Trim() != ""  && txtStuKEY.Text.Trim() != "")
                {
                    if (SearchItemRegEdit(RegPath, StuID) && SearchItemRegEdit(RegPath, StuKey))
                    {
                        if (getValueRegEdit(RegPath, StuID) != txtStuID.Text.Trim())
                        {
                            SetValueRegEdit(RegPath, StuID, txtStuID.Text.Trim());
                            SetValueRegEdit(RegPath, StuKey, txtStuKEY.Text.Trim());
                        }
                    }
                    else
                    {
                        if (SearchItemRegEdit(RegPath, WifiID) && SearchItemRegEdit(RegPath, WifiKey))
                        {
                            SetValueRegEdit(RegPath, StuID, txtStuID.Text.Trim());
                            SetValueRegEdit(RegPath, StuKey, txtStuKEY.Text.Trim());
                        }
                        else
                        {
                            CreateItemRegEdit(RegPath);
                            SetValueRegEdit(RegPath, StuID, txtStuID.Text.Trim());
                            SetValueRegEdit(RegPath, StuKey, txtStuKEY.Text.Trim());
                        }
                    }

                    lblMSG.Text = "等待……";
                    Uri uri = new Uri("http://172.30.16.53:80/cgi-bin/srun_portal");
                    WebRequest objWebRequest = WebRequest.Create(uri);
                    objWebRequest.Credentials = CredentialCache.DefaultCredentials;
                    //((HttpWebRequest)objWebRequest).UserAgent = "5DO8";
                    objWebRequest.Method = "POST";
                    objWebRequest.ContentType = "application/x-www-form-urlencoded";
                    objWebRequest.ContentType = "text/html";
                    Stream dataStream = objWebRequest.GetRequestStream();
                    string data = String.Format("action=login&username={0}&password={1}&drop=0&pop=1&type=2&n=116&ip=1640269834&mbytes=0&minutes=0&ac_id=3&mac=94%3a39%3ae5%3a02%3a86%3a4b&nas_ip=172.30.12.244&cguid=%7b8C23438A-17AD-2B84-E2E0-CAB7D79D6709%7d",txtStuID.Text.Trim(),txtStuKEY.Text.Trim());
                    UTF8Encoding f8 = new UTF8Encoding();
                    byte[] byteArray = f8.GetBytes(data);

                    dataStream.Write(byteArray, 0, byteArray.Length);
                    dataStream.Close();

                    WebResponse objResponse = objWebRequest.GetResponse();
                    lblMSG.Text = ((HttpWebResponse)objResponse).StatusDescription;

                    Stream rpData = objResponse.GetResponseStream();
                    StreamReader r = new StreamReader(rpData);
                    string msg = r.ReadToEnd();
                    if (msg == "online_num_error")
                    {
                        lblMSG.Text = "已有其他终端在线，请先注销！";
                    }
                    else if(msg.Contains("login_ok"))
                    {
                        lblMSG.Text = "登录成功";
                        txtStuID.Enabled = false;
                        txtStuKEY.Enabled = false;
                        btnLogin.Enabled = false;
                        btnLogin.Cursor = Cursors.No;
                    }
                }
                else
                {
                    MessageBox.Show("用户名或密码为空！");
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            lblMSG.Text = "等待……";
            Uri uri = new Uri("http://172.30.16.53:80/cgi-bin/srun_portal");
            WebRequest objWebRequest = WebRequest.Create(uri);
            objWebRequest.Credentials = CredentialCache.DefaultCredentials;
            //((HttpWebRequest)objWebRequest).UserAgent = "5DO8";
            objWebRequest.Method = "POST";
            objWebRequest.ContentType = "application/x-www-form-urlencoded";
            objWebRequest.ContentType = "text/html";
            Stream dataStream = objWebRequest.GetRequestStream();
            string data = String.Format("action=logout&ac_id=3&username={0}&type=3&mac=94%3a39%3ae5%3a02%3a86%3a4b&nas_ip=172.30.12.244",txtStuID.Text.Trim());
            UTF8Encoding f8 = new UTF8Encoding();
            byte[] byteArray = f8.GetBytes(data);

            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Close();

            WebResponse objResponse = objWebRequest.GetResponse();
            lblMSG.Text = ((HttpWebResponse)objResponse).StatusDescription;

            Stream rpData = objResponse.GetResponseStream();
            StreamReader r = new StreamReader(rpData);
            lblMSG.Text = r.ReadToEnd();

            txtStuID.Enabled = true;
            txtStuKEY.Enabled = true;
            btnLogin.Enabled = true;
            btnLogin.Cursor = Cursors.Hand;
        }

        private void linkSchoolHome_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://i.whut.edu.cn/");
        }

        private void linkSelfServer_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://selfaaa.whut.edu.cn/");
        }

        private void linkLib_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://lib.whut.edu.cn/");
        }

        private void linkMDHome_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://som.whut.edu.cn/");
        }

        private void notifyIcon1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                contextMenuStrip1.Show(contextMenuStrip1.Left+contextMenuStrip1.Width,contextMenuStrip1.Top);

            }
        }

        private void ToolStripMenuItemLogin_Click(object sender, EventArgs e)
        {
            notifyIcon1_DoubleClick(sender, e);
            btnLogin_Click(sender,e);
        }

        private void toolStripMenuItemLogout_Click(object sender, EventArgs e)
        {
            notifyIcon1_DoubleClick(sender, e);
            btnLogout_Click(sender, e);
        }

        private void toolStripMenuItemExit_Click(object sender, EventArgs e)
        {
            notifyIcon1_DoubleClick(sender, e);
            label3_Click(sender, e);
        }

        private void ToolStripMenuItemShowMainForm_Click(object sender, EventArgs e)
        {
            notifyIcon1_DoubleClick(sender, e);
        }

       
    }
}

