using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace MSU
{
    public partial class Updater : Form
    {
        private bool auto_run = false;
        private BackgroundWorker update_worker;

        public Updater()
        {
            Environment.CurrentDirectory = Path.GetDirectoryName(Application.ExecutablePath);
            InitializeComponent();
            this.Location = new Point(Screen.PrimaryScreen.WorkingArea.Width - this.Width, Screen.PrimaryScreen.WorkingArea.Height - this.Height);
            int pos = (this.ClientSize.Width - progressBar1.Width) / 2;
            progressBar1.Left = pos;
            label1.Left = pos;

            string[] args = Environment.GetCommandLineArgs();

            if (!args.Contains("/start"))
            {
                Environment.Exit(0);
            }

            if (args.Contains("/autorun"))
            {
                auto_run = true;
            }
        }

        private void Updater_Load(object sender, EventArgs e)
        {
            init_update_worker();

            update_worker.RunWorkerAsync();
        }

        private void init_update_worker()
        {
            update_worker = new BackgroundWorker();
            update_worker.WorkerReportsProgress = false;

            update_worker.WorkerSupportsCancellation = true;

            update_worker.DoWork += new DoWorkEventHandler(do_update);

            update_worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(update_completed);
        }

        private void do_update(object sender, DoWorkEventArgs e)
        {
            install_update("_");
        }

        private void update_completed(object sender, RunWorkerCompletedEventArgs e)
        {
            Environment.Exit(0);
        }

        private void install_update(string update_folder_name)
        {
            do
            {
                delete_dirs_and_files(update_folder_name);

                Console.WriteLine("Wait for deleting...");

                Thread.Sleep(1000);
            } while (!check_deletion(update_folder_name));

            move_from_update_dir(update_folder_name);

            if (auto_run)
            {
                Process.Start("MKBot.exe");
            }
        }

        private void move_from_update_dir(string update_folder_name)
        {
            var target = Path.GetDirectoryName(Application.ExecutablePath) + "/";
            var di = new DirectoryInfo(target + update_folder_name);

            foreach (FileInfo file in di.EnumerateFiles())
            {
                Console.WriteLine(file.Name);

                file.MoveTo(target + file.Name);
            }

            foreach (DirectoryInfo dir in di.EnumerateDirectories())
            {
                Console.WriteLine(dir.Name);

                dir.MoveTo(target + dir.Name);
            }

            di.Delete(true);
        }

        private bool check_deletion(string update_folder_name)
        {
            bool check_file = false;
            bool check_dir = false;

            var di = new DirectoryInfo(Path.GetDirectoryName(Application.ExecutablePath));

            if (!check_file)
            {
                string[] files = di.GetFiles().Select(f => f.Name).ToArray();
                if (!files.Except(new[] { "Update.exe" }).Any())
                {
                    check_file = true;
                }
            }

            if (!check_dir)
            {
                string[] dirs = di.GetDirectories().Select(d => d.Name).ToArray();
                if (!dirs.Except(new[] { update_folder_name }).Any())
                {
                    check_dir = true;
                }
            }

            return check_file && check_dir;
        }

        private void delete_dirs_and_files(string update_folder_name)
        {
            var di = new DirectoryInfo(Path.GetDirectoryName(Application.ExecutablePath));

            foreach (FileInfo file in di.EnumerateFiles())
            {
                Console.WriteLine(file.Name);

                if (file.Name == "Update.exe")
                {
                    continue;
                }

                try
                {
                    file.Delete();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            foreach (DirectoryInfo dir in di.EnumerateDirectories())
            {
                Console.WriteLine(dir.Name);

                if (dir.Name == update_folder_name)
                {
                    continue;
                }

                try
                {
                    dir.Delete(true);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
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