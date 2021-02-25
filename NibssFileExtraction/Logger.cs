using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace NibssFileExtraction
{
    public class Logger
    {
        private int LogFileSize = int.Parse("LOG_FILE_SIZE".GetKeyValue());

        public void Error(Exception ex)
        {
            
            if (File.Exists("LOG_PATH".GetKeyValue() + "error" + "_log.txt"))
            {
                FileInfo t = new FileInfo("LOG_PATH".GetKeyValue() + "error" + "_log.txt");
                if (t.Length > LogFileSize * 1024 * 1024)
                {
                    t.MoveTo("LOG_PATH".GetKeyValue() + "error" + "_log_" + DateTime.Now.ToString("ddMMyyyyHHmmss") + ".txt");
                }
            }
            else
            {
                File.Create("LOG_PATH".GetKeyValue() + "error" + "_log.txt");
            }
            var logDetails = $"An error occurred Exception Message : {ex.Message } with stack trace : {ex.StackTrace} and Inner Message : {ex.InnerException}";
            File.AppendAllText("LOG_PATH".GetKeyValue() + "error" + "_log.txt", DateTime.Now.ToString() + " " + logDetails + Environment.NewLine);
        }

        public void Info(string info)
        {
           
            if (File.Exists("LOG_PATH".GetKeyValue() + "info" + "_log.txt"))
            {
                FileInfo t = new FileInfo("LOG_PATH".GetKeyValue() + "error" + "_log.txt");
                if (t.Length > LogFileSize * 1024 * 1024)
                {
                    t.MoveTo("LOG_PATH".GetKeyValue() + "info" + "_log_" + DateTime.Now.ToString("ddMMyyyyHHmmss") + ".txt");
                }
            }
            else
            {
                File.Create("LOG_PATH".GetKeyValue() + "info" + "_log.txt");
            }
            File.AppendAllText("LOG_PATH".GetKeyValue() + "info" + "_log.txt", DateTime.Now.ToString() + " " + info + Environment.NewLine);
            $"{DateTime.Now.ToString()}:::{info}".Dump();
        }
    }
}
