using AdHate.Properties;
using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Text;
using System.Windows.Forms;

namespace AdHate
{
    public partial class Form1 : Form
    {
        private string etc = Environment.GetFolderPath(Environment.SpecialFolder.System) + @"\drivers\etc\";

        public Form1()
        {
            InitializeComponent();
            try
            {
                // backup hosts
                if (File.Exists(etc + "hosts") && Settings.Default.firstrun == false || !File.Exists(etc + "hosts.bak"))
                {
                    File.Copy(etc + "hosts", etc + "hosts.bak", true);
                    Settings.Default.firstrun = true;
                    Settings.Default.Save();
                }
            }
            catch (Exception ex)
            {
                textBox1.ForeColor = Color.Red;
                textBox1.Text = textBox1.Text + "\r\nWARNING: " + Convert.ToString(ex);
                textBox1.Text = textBox1.Text + "\r\nNo changes to your system have been made.";
                progressBar1.Value = 0;
                button1.Enabled = true;
                button2.Enabled = true;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                // just in case
                textBox1.ForeColor = Color.White;
                button1.Enabled = false;
                button2.Enabled = false;

                // take ownership of hosts
                textBox1.Text = "Taking ownership of hosts";

                System.Diagnostics.Process process = new System.Diagnostics.Process();
                System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
                startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                startInfo.FileName = "cmd.exe";
                startInfo.WorkingDirectory = etc;
                startInfo.Arguments = "/c takeown /f hosts && icacls hosts /grant administrators:F";
                process.StartInfo = startInfo;
                process.Start();

                textBox1.Text = textBox1.Text + "\r\nTook ownership of hosts";

                progressBar1.Value = 10;

                // get mega-hosts file
                textBox1.Text = textBox1.Text + "\r\nGetting mega-hosts file";

                StringBuilder sb = new StringBuilder();

                byte[] buf = new byte[8192];

                HttpWebRequest request = (HttpWebRequest)
                WebRequest.Create("https://cdn.rawgit.com/StevenBlack/hosts/master/hosts");

                HttpWebResponse response = (HttpWebResponse)
                request.GetResponse();

                Stream resStream = response.GetResponseStream();

                string tempString = null;
                int count = 0;

                do
                {
                    count = resStream.Read(buf, 0, buf.Length);

                    if (count != 0)
                    {
                        tempString = Encoding.ASCII.GetString(buf, 0, count);

                        sb.Append(tempString);
                    }
                }
                while (count > 0);

                textBox1.Text = textBox1.Text + "\r\nGot mega-hosts file (by StevenBlack on GitHub)";

                progressBar1.Value = 80;

                // write to hosts
                textBox1.Text = textBox1.Text + "\r\nWriting to hosts";

                File.AppendAllText(etc + "hosts", "\r\n# Begin mega-hosts file\r\n\r\n" + sb.ToString());

                textBox1.Text = textBox1.Text + "\r\nWritten to hosts";

                progressBar1.Value = 90;

                //flush dns
                textBox1.Text = textBox1.Text + "\r\nFlushing DNS cache";

                startInfo.Arguments = "/c ipconfig /flushdns";
                process.StartInfo = startInfo;
                process.Start();

                textBox1.Text = textBox1.Text + "\r\nFlushed DNS cache\r\nDone.";

                textBox1.ForeColor = Color.Lime;

                progressBar1.Value = 100;

                button1.Enabled = true;
                button2.Enabled = true;
            }
            // if bad things happen
            catch (Exception ex)
            {
                textBox1.ForeColor = Color.Red;
                textBox1.Text = textBox1.Text + "\r\nWARNING: " + Convert.ToString(ex);
                textBox1.Text = textBox1.Text + "\r\nNo changes to your system have been made.";
                progressBar1.Value = 0;
                button1.Enabled = true;
                button2.Enabled = true;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                // just in case
                textBox1.ForeColor = Color.White;
                button1.Enabled = false;
                button2.Enabled = false;

                // restore hosts
                textBox1.Text = "Restoring hosts";

                progressBar1.Value = 0;

                if (File.Exists(etc + "hosts"))
                {
                    File.Delete(etc + "hosts");
                }

                File.Copy(etc + "hosts.bak", etc + "hosts", true);

                progressBar1.Value = 90;

                textBox1.Text = textBox1.Text + "\r\nRestored hosts";

                // flush dns
                textBox1.Text = textBox1.Text + "\r\nFlushing DNS cache";

                System.Diagnostics.Process process = new System.Diagnostics.Process();
                System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
                startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                startInfo.FileName = "cmd.exe";
                startInfo.Arguments = "/c ipconfig /flushdns";
                process.StartInfo = startInfo;
                process.Start();

                textBox1.Text = textBox1.Text + "\r\nFlushed DNS cache\r\nDone.";

                textBox1.ForeColor = Color.Lime;

                progressBar1.Value = 100;
                button1.Enabled = true;
                button2.Enabled = true;
            }
            // if more bad things happen
            catch (Exception ex)
            {
                textBox1.ForeColor = Color.Red;
                textBox1.Text = textBox1.Text + "\r\n\r\nWARNING: " + Convert.ToString(ex);
                textBox1.Text = textBox1.Text + "\r\nNo changes to your system have been made.";
                progressBar1.Value = 0;
                button1.Enabled = true;
                button2.Enabled = true;
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            textBox1.SelectionStart = textBox1.Text.Length;
            textBox1.ScrollToCaret();
        }
    }
}