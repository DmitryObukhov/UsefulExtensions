using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using UsefulExtensions;
using System.Data;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace UsefulExtensions
{
    public static class APPLOG
    {
        public static List<string> LogText = new List<string>();
        public static DataTable Records = new DataTable();

        public static string DEBUG = "DEBUG";
        public static string FAIL = "FAIL";
        public static string ERROR = "ERROR";
        public static string WARNING = "WARNING";
        public static string INFO = "INFO";

        private static DateTime lastWrite = DateTime.MinValue;
        private static DateTime startTime = DateTime.MinValue;
        private static string logFile;
        private static DataSet DS = new DataSet();


        static APPLOG()
        {
            Reset();
        }


        public static void Reset()
        {
            LogText.Clear();

            startTime = DateTime.Now;
            string timeStamp = "".TimeStamp();
            string logPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string baseName = AppDomain.CurrentDomain.FriendlyName+"__"+ timeStamp;
            logFile = Path.Combine(logPath, baseName + ".log");

            DS.Clear();
            DS.Tables.Clear();

            Records.Clear();

            Records.Columns.Add("Time", typeof(DateTime));
            Records.Columns.Add("TimeFromLast", typeof(double));
            Records.Columns.Add("TimeFromStart", typeof(int));
            Records.Columns.Add("Class", typeof(string));
            Records.Columns.Add("MessageCode", typeof(int));
            Records.Columns.Add("CallerMethod", typeof(string));
            Records.Columns.Add("CallerFile", typeof(string));
            Records.Columns.Add("CallerLineNumber", typeof(int));
            Records.Columns.Add("Message", typeof(string));

            Records.TableName = "Records";
            DS.Tables.Add(Records);
            DS.DataSetName = baseName;

        }

        public static void Log( string message, 
                                string logType = "",
                                int logCode = 0,
                                [CallerMemberName] string callerName = "", 
                                [CallerFilePath] string callerFile = "", 
                                [CallerLineNumber] int callerLineNum = -1)
        {
            DateTime nowTime = DateTime.Now;
            string timeStamp = nowTime.ToLongTimeString();
            double timeFromLastWrite = 0;
            if (lastWrite != DateTime.MinValue)
            {
                timeFromLastWrite = (nowTime - lastWrite).TotalMilliseconds;
            }

            StackTrace stackTrace = new StackTrace();
            string caller = stackTrace.GetFrame(1).GetMethod().Name;

            string recType = logType;
            if (recType == "")
            {
                recType = "CLI";
            }

            DataRow newRec = Records.NewRow();
            newRec["Time"] = nowTime;
            newRec["TimeFromLast"] = timeFromLastWrite;
            newRec["TimeFromStart"] = (nowTime - startTime).TotalSeconds;
            newRec["Class"] = recType;
            newRec["MessageCode"] = logCode;

            newRec["CallerMethod"] = callerName;
            newRec["CallerFile"] = callerFile;
            newRec["CallerLineNumber"] = callerLineNum;

            newRec["Message"] = message;
            Records.Rows.Add(newRec);

            if (logType == "")
            {
                Console.WriteLine(message);
            }
            Debug.WriteLine(recType.FixLen(8) +": "+ message);
            LogText.Add(recType.FixLen(8) + ": " + message);

            using (StreamWriter sw = File.AppendText(logFile))
            {
                sw.WriteLine(logType.FixLen(8) + ": " + message);
            }

            lastWrite = nowTime;
        }

        public static void Save(string fileName = "")
        {
            if (fileName == string.Empty)
            {
                string logPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                fileName = Path.Combine(logPath, DS.DataSetName+".xml");
            }
            DS.WriteXml(fileName);
        }
    }
}
