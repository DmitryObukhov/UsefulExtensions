using System;
using System.IO;
using System.Diagnostics;
using IWshRuntimeLibrary; // Requires COM reference "Windows Script Host Object Model"
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UsefulExtensions
{
    public class ShellScripting
    {
        private Process process = new Process();
        private ProcessStartInfo startInfo = new ProcessStartInfo();
        private string _tempDir = "";
        public string TempDirectory { get { return _tempDir; } }

        public bool Start(string cmd, string arguments="")
        {
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
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
            } catch
            {
                // Nothing
            }
            network.MapNetworkDrive(driveLetter, networkPath, true);
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

    }
}
