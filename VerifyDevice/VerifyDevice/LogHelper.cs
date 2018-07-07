using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;

namespace VerifyDevice.Helper
{
    public class LogHelper
    {
        string logfilePath = Directory.GetCurrentDirectory() + "\\Log\\";

        public void LogEvent(string Device, string Location)
        {
            DateTime dt = DateTime.Now; 
            string curDate = dt.ToString("dd-MM-yyyy");
            string LogFilePath = logfilePath + "Log_" + curDate + ".txt";
            File.AppendAllText(LogFilePath, DateTime.Now.ToString() + "| " + Device + "|" + Location + Environment.NewLine);
           
        }

        public bool ReadLogEvent(string Device, string Location, DateTime checkDateTime)
        {
            DateTime d = DateTime.Now;
            string dateString = d.ToString("dd-MM-yyyy");
            string LogFilePath = logfilePath + "Log_" + dateString + ".txt";
            TimeSpan timeDiff = new TimeSpan() ;
            if (File.Exists(LogFilePath))
            {
                string[] logs = File.ReadLines(LogFilePath).ToArray();
                if (logs != null)
                {
                    string[] deviceLogs = logs.Where(x => x.Contains(Device)).Select(x => x).ToArray();
                    string latestLog = deviceLogs.Last();
                    string[] curlog = latestLog.Trim() != "" ? latestLog.Split('|') : null;
                    DateTime lastLogDateTime = Convert.ToDateTime(curlog[0]);
                    if (curlog != null)
                        timeDiff = checkDateTime.Subtract(lastLogDateTime);
                    if (timeDiff.Hours > 2)
                        return true;
                    else
                        return false;
                }
                else 
                    return true;

            }
            else
                return true;
        }
    }
}