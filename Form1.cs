using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MSU
{
    public partial class Form1 : Form
    {
        private Process sprocess;
        private string[] args;
        public Form1()
        {
            InitializeComponent();
            args = Environment.GetCommandLineArgs();
            int pos = (this.ClientSize.Width - progressBar1.Width) / 2;
            progressBar1.Left = pos;
            label1.Left = pos;
            Process[] localByName = Process.GetProcessesByName(args[2].Replace(".exe",""));
            sprocess = localByName[0];
            sprocess.EnableRaisingEvents = true;
            sprocess.Exited += new EventHandler(ProcessExitd);
        }

        private void ProcessExitd(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string[] args = Environment.GetCommandLineArgs();
            
            if (!args.Contains("/start"))
            {
                Application.Exit();
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