﻿using System;
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
        public Form1()
        {
            InitializeComponent();
            int pos = (this.ClientSize.Width - progressBar1.Width) / 2;
            progressBar1.Left = pos;
            label1.Left = pos;
            Process[] localByName = Process.GetProcessesByName("MDFSetup-stable");
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
            if ((e.CloseReason != CloseReason.ApplicationExitCall)&&(MessageBox.Show("Mulgyeol Distance Fetcher 업데이트를 취소하시겠습니까?", "Mulgyeol Software Update", MessageBoxButtons.YesNo) == DialogResult.No))
            {
                e.Cancel = true;
            }
            else
            {
                Console.WriteLine(sender.ToString());
                try
                {
                    sprocess.Kill();
                }
                catch
                {
                    Console.WriteLine("Already Exited");
                }
            }
        }
    }
}
