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
        private string update_dir_name = "_";

        public Updater()
        {
            Environment.CurrentDirectory = Path.GetDirectoryName(Application.ExecutablePath);
            InitializeComponent();
            this.Location = new Point(Screen.PrimaryScreen.WorkingArea.Width - this.Width, Screen.PrimaryScreen.WorkingArea.Height - this.Height);
            int pos = (this.ClientSize.Width - progressBar1.Width) / 2;
            progressBar1.Left = pos;
            label1.Left = pos;

            string[] args = Environment.GetCommandLineArgs();

            if (!(args.Contains("/start") && check_ready_to_update()))
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
            install_update(update_dir_name);
        }

        private void update_completed(object sender, RunWorkerCompletedEventArgs e)
        {
            Environment.Exit(0);
        }

        private void install_update(string update_folder_name)
        {
            while (true)
            {
                kill_process();

                delete_dirs_and_files(update_folder_name);

                if (check_deletion(update_folder_name))
                {
                    break;
                }

                Console.WriteLine("Wait for deleting...");

                Thread.Sleep(1000);
            }

            move_from_update_dir(update_folder_name);

            if (auto_run)
            {
                Process.Start("MKBot.exe");
            }
        }

        private bool check_ready_to_update()
        {
            return File.Exists("./Update.flag") && Directory.Exists(update_dir_name);
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

            string[] files = di.GetFiles().Select(f => f.Name).ToArray();

            if (!files.Except(new[] { "Update.exe" }).Any())
            {
                check_file = true;
            }

            string[] dirs = di.GetDirectories().Select(d => d.Name).ToArray();

            if (!dirs.Except(new[] { update_folder_name }).Any())
            {
                check_dir = true;
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

        private void kill_process()
        {
            string[] all_exes = new DirectoryInfo(Path.GetDirectoryName(Application.ExecutablePath)).GetFiles("*.exe", SearchOption.AllDirectories).Where(x => x.Name != "Update.exe").Select(x => x.FullName).ToArray();

            Process[] runningProcesses = Process.GetProcesses();

            foreach (Process process in runningProcesses)
            {
                try
                {
                    if (process.MainModule != null && all_exes.Contains(process.MainModule.FileName))
                    {
                        Console.WriteLine("Kill " + process.MainModule.FileName);
                        process.Kill();
                    }
                }
                catch (Exception)
                {
                    //Console.WriteLine(ex.Message);
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