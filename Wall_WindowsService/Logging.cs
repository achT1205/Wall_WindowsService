using System.IO;
using System;

namespace Wall_WindowsService
{
    public static class Logging
    {
        public static void Log(string message)
        {
            try
            {
                string path = AppDomain.CurrentDomain.BaseDirectory + "\\Logs";
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                string filePah = AppDomain.CurrentDomain.BaseDirectory + "\\Logs\\EmailServiceLog_" + DateTime.Now.Date.ToShortDateString().Replace('/', '_') + ".txt";
                if (!File.Exists(filePah))
                {
                    using (StreamWriter sw = new StreamWriter(filePah))
                    {
                        sw.WriteLine($"{DateTime.Now}: {message}");
                    }
                }
                else
                {
                    using (StreamWriter sw = File.AppendText(filePah))
                    {
                        sw.WriteLine($"{DateTime.Now}: {message}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to log message: {ex.Message}");
            }
        }
    }
}
