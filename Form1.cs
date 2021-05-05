using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace MSU
{
    public partial class Form1 : Form
    {
        private Process sprocess = new Process();
        private ProcessStartInfo psi1 = new ProcessStartInfo();

        public Form1()
        {
            Environment.CurrentDirectory = Path.GetDirectoryName(Application.ExecutablePath);
            InitializeComponent();
            this.Location = new Point(Screen.PrimaryScreen.WorkingArea.Width - this.Width, Screen.PrimaryScreen.WorkingArea.Height - this.Height);
            int pos = (this.ClientSize.Width - progressBar1.Width) / 2;
            progressBar1.Left = pos;
            label1.Left = pos;
            ///
            psi1.FileName = Environment.GetEnvironmentVariable("TEMP") + "\\mkbot-update\\MKBotSetup.exe";
            psi1.CreateNoWindow = true;
            psi1.UseShellExecute = false;
            sprocess.StartInfo = psi1;
            sprocess.EnableRaisingEvents = true;
            sprocess.Exited += new EventHandler(ProcessExitd);
        }

        private void ProcessExitd(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string[] args = Environment.GetCommandLineArgs();
            
            if (!args.Contains("/start"))
            {
                Environment.Exit(0);
            }

            if (args.Contains("/autorun"))
            {
                psi1.Arguments = "/S /update /autorun";
            } 
            else
            {
                psi1.Arguments = "/S /update";
            }
            try
            {
                sprocess.Start();
            }
            catch
            {
                Process.Start("MKBot.exe");
                Environment.Exit(0);
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason != CloseReason.ApplicationExitCall)
            {
                e.Cancel = true;
            }
        }
    }
}