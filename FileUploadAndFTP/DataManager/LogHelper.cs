using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;

namespace FileUploadAndFTP
{
    public class LogHelper
    {
        string filePath = HostingEnvironment.MapPath("~/Log/");

        public void LogEvent(string Device, string Location)
        {
            DateTime dt = DateTime.Now;
            string curDate = dt.ToString("dd-MM-yyyy");
            string LogFilePath = filePath + "Log_" + curDate + ".txt";
            File.AppendAllText(LogFilePath, DateTime.Now.ToString() + "| " + Device + "|" + Location + Environment.NewLine);

        }
    }
}