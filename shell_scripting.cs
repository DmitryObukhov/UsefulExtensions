using System;
using System.IO;
using System.Diagnostics;
using IWshRuntimeLibrary; // Requires COM reference "Windows Script Host Object Model"
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.ComponentModel;

namespace UsefulExtensions
{
    public class ShellScripting
    {
        public string DisobidientWindow = string.Empty;
        public string ProcessArguments = "";
        public string ProcessCommand = "";
        public string ProcessStdio = "";
        public string ProcessStderr = "";
        public bool ProcessWasTerminated = false;
        public int ProcessExitCode = 0;
        public double ElapsedTime;
        private BackgroundWorker bckgnd;

        [DllImport("user32.dll")]
        private static extern bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);

        [DllImport("User32.Dll")]
        public static extern int FindWindow(string lpClassName, string lpWindowName);

        [DllImport("User32.Dll")]
        public static extern int GetClassName(int hwnd, StringBuilder lpClassName, int nMaxCount);

        [DllImport("user32.dll", EntryPoint = "FindWindow", SetLastError = true)]
        static extern IntPtr FindWindowByCaption(IntPtr ZeroOnly, string lpWindowName);


        //protected delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        protected static extern int GetWindowText(IntPtr hWnd, StringBuilder strText, int maxCount);
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        protected static extern int GetWindowTextLength(IntPtr hWnd);
        [DllImport("user32.dll")]
        protected static extern bool EnumWindows(EnumWindowsProc enumProc, IntPtr lParam);
        [DllImport("user32.dll")]
        protected static extern bool IsWindowVisible(IntPtr hWnd);

        //public bool EnumTheWindows(IntPtr hWnd, IntPtr lParam)
        //{
        //    int size = GetWindowTextLength(hWnd);
        //    if (size++ > 0 && IsWindowVisible(hWnd))
        //    {
        //        StringBuilder sb = new StringBuilder(size);
        //        GetWindowText(hWnd, sb, size);
        //        Console.WriteLine(sb.ToString());
        //    }
        //    return true;
        //}

        //[DllImport("user32.dll", CharSet = CharSet.Unicode)]
        //private static extern int GetWindowText(IntPtr hWnd, StringBuilder strText, int maxCount);

        //[DllImport("user32.dll", CharSet = CharSet.Unicode)]
        //private static extern int GetWindowTextLength(IntPtr hWnd);

        //[DllImport("user32.dll")]
        //private static extern bool EnumWindows(EnumWindowsProc enumProc, IntPtr lParam);

        // Delegate to filter which windows to include 
        public delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

        /// <summary> Get the text for the window pointed to by hWnd </summary>
        public static string GetWindowText(IntPtr hWnd)
        {
            int size = GetWindowTextLength(hWnd);
            if (size > 0)
            {
                var builder = new StringBuilder(size + 1);
                GetWindowText(hWnd, builder, builder.Capacity);
                return builder.ToString();
            }

            return String.Empty;
        }

        /// <summary> Find all windows that match the given filter </summary>
        /// <param name="filter"> A delegate that returns true for windows
        ///    that should be returned and false for windows that should
        ///    not be returned </param>
        public static IEnumerable<IntPtr> FindWindows(EnumWindowsProc filter)
        {
            IntPtr found = IntPtr.Zero;
            List<IntPtr> windows = new List<IntPtr>();

            EnumWindows(delegate (IntPtr wnd, IntPtr param)
            {
                if (filter(wnd, param))
                {
                    // only add the windows that pass the filter
                    windows.Add(wnd);
                }

                // but return true here so that we iterate all windows
                return true;
            }, IntPtr.Zero);

            return windows;
        }

        /// <summary> Find all windows that contain the given title text </summary>
        /// <param name="titleText"> The text that the window title must contain. </param>
        public static IEnumerable<IntPtr> FindWindowsWithText(string titleText)
        {
            return FindWindows(delegate (IntPtr wnd, IntPtr param)
            {
                return GetWindowText(wnd).Contains(titleText);
            });
        }




        private const int SW_HIDE = 0;
        private const int SW_SHOWNORMAL = 1;
        private const int SW_SHOWMINIMIZED = 2;
        private const int SW_SHOWMAXIMIZED = 3;
        private const int SW_SHOWNOACTIVATE = 4;
        private const int SW_RESTORE = 9;
        private const int SW_SHOWDEFAULT = 10;


        private Process process = new Process();
        private ProcessStartInfo startInfo = new ProcessStartInfo();
        private string _tempDir = "";
        public string TempDirectory { get { return _tempDir; } }



        public ShellScripting()
        {
            bckgnd = new BackgroundWorker();
            bckgnd.WorkerReportsProgress = true;
            bckgnd.WorkerSupportsCancellation = true;
            //bckgnd.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
            //bckgnd.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorker1_ProgressChanged);
            //bckgnd.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker1_RunWorkerCompleted);

        }

        public int Execute(string cmd, string arguments = "", int timeoutSeconds = 0)
        {
            ProcessCommand = cmd;
            ProcessStdio = "";
            ProcessStderr = "";
            ProcessArguments = arguments;

            Process process = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.CreateNoWindow = true;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;
            startInfo.UseShellExecute = false;
            startInfo.FileName = cmd;
            startInfo.Arguments = arguments;
            process.StartInfo = startInfo;
            process.Exited += new EventHandler(myProcess_Exited);
            DateTime x = DateTime.Now;
            DateTime y = DateTime.Now;
            ElapsedTime = 0;
            process.Start();

            while (!process.HasExited)
            {
                if (timeoutSeconds > 0)
                {
                    y = DateTime.Now;
                    ElapsedTime = (y - x).TotalSeconds;
                    if (ElapsedTime > (double)timeoutSeconds)
                    {
                        process.Kill();
                        ProcessStderr = process.StandardError.ReadToEnd();
                        ProcessStdio = process.StandardOutput.ReadToEnd();
                        ProcessWasTerminated = true;
                        return process.ExitCode;
                    }
                }
                Thread.Sleep(100);
                if (DisobidientWindow != string.Empty)
                {
                    IEnumerable<IntPtr> dms = FindWindowsWithText(DisobidientWindow);
                    foreach (IntPtr zhWnd in dms)
                    {
                        ShowWindowAsync(zhWnd, SW_HIDE);
                    }
                }
            }
            ProcessStdio = process.StandardOutput.ReadToEnd();
            ProcessStderr = process.StandardError.ReadToEnd();
            return process.ExitCode;
        }

        private void myProcess_Exited(object sender, System.EventArgs e)
        {
            Process myProcess = (Process)sender;
            Debug.WriteLine("");
            try {
                Debug.WriteLine("--------------- Ended process " + myProcess.ProcessName + " ----------------");
                Debug.WriteLine("Command..... " + ProcessCommand);
                Debug.WriteLine("Arguments... " + ProcessArguments);
                Debug.WriteLine("Started..... " + myProcess.StartTime.ToLongDateString());
                Debug.WriteLine("Finished.... " + myProcess.ExitTime.ToLongDateString());
                Debug.WriteLine("Elapsed..... " + ElapsedTime.ToString() + " seconds");
                Debug.WriteLine("Exit code... " + myProcess.ExitCode.ToString());
                Debug.WriteLine("-----------------------------------------------------------------------------");
            }
            catch
            {
                Debug.WriteLine("----------- Ended process <INFO IS NOT AVAILABLE ------------");
            }
            Debug.WriteLine("");
        }


        public bool Start(string cmd, string arguments="")
        {
            //startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.FileName = cmd;
            startInfo.Arguments = arguments;
            process.StartInfo = startInfo;
            process.Start();
            return IsRunning;
        }

        public bool IsRunning
        {
            get
            {
                return !process.HasExited;
            }
        }

        public List<string> FindFiles(string path, string mask)
        {
            return Directory.GetFiles(path, mask, System.IO.SearchOption.AllDirectories).ToList();
        }


        public bool ConnectNetworkDrive(string driveLetter, string networkPath)
        {
            //Process.Start("net.exe", @"USE Z: \\server\share /user:domain\username password").WaitForExit();
            IWshNetwork_Class network = new IWshNetwork_Class();
            try {
                network.RemoveNetworkDrive(driveLetter, true, true);
                network.MapNetworkDrive(driveLetter, networkPath, true);
            }
            catch
            {
                // Nothing
            }
            return true;
        }

        public string CreateTempDirectory()
        {
            string tempPath = Path.GetTempPath();
            string tempName = "".TimeStamp("-") + "__" + Strings.lowercase.Random(8);
            _tempDir = tempPath + tempName;
            Directory.CreateDirectory(_tempDir);
            return _tempDir;
        }

        public void CleanTempDirectory(string FolderName = "")
        {
            if (FolderName == "")
            {
                FolderName = _tempDir;
            }
            
            DirectoryInfo dir = new DirectoryInfo(FolderName);

            foreach (FileInfo fi in dir.GetFiles())
            {
                fi.Delete();
            }

            foreach (DirectoryInfo di in dir.GetDirectories())
            {
                CleanTempDirectory(di.FullName);
                di.Delete();
            }
        }

        public void DeleteAll(string rootPath, string mask)
        {
            // Delete *.mask files
            Array.ForEach(Directory.GetFiles(rootPath, mask), delegate (string path) { System.IO.File.Delete(path); });
        }


    }

    public static class SleepControl
    {
        [FlagsAttribute]
        private enum EXECUTION_STATE : uint
        {
            ES_SYSTEM_REQUIRED = 0x00000001,
            ES_DISPLAY_REQUIRED = 0x00000002,
            // Legacy flag, should not be used. 
            // ES_USER_PRESENT   = 0x00000004, 
            ES_AWAYMODE_REQUIRED = 0x00000040,
            ES_CONTINUOUS = 0x80000000,
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern EXECUTION_STATE SetThreadExecutionState(EXECUTION_STATE esFlags);

        public static void PreventSleep()
        {
            if (SetThreadExecutionState(EXECUTION_STATE.ES_CONTINUOUS
                | EXECUTION_STATE.ES_DISPLAY_REQUIRED
                | EXECUTION_STATE.ES_SYSTEM_REQUIRED
                | EXECUTION_STATE.ES_AWAYMODE_REQUIRED) == 0) //Away mode for Windows >= Vista
                SetThreadExecutionState(EXECUTION_STATE.ES_CONTINUOUS
                    | EXECUTION_STATE.ES_DISPLAY_REQUIRED
                    | EXECUTION_STATE.ES_SYSTEM_REQUIRED); //Windows < Vista, forget away mode
        }

        public static void EnableSleep()
        {
            SetThreadExecutionState(EXECUTION_STATE.ES_CONTINUOUS);
        }

    }

}
