using System;
using System.IO;

namespace Utils
{
    public static class Logger
    {
        public static void WriteLog(string text, string filePath)
        {
            text = DateTime.Now + " | " + text;
            if (!File.Exists(filePath))
            {
                File.Create(filePath).Close();
                var writer = new StreamWriter(filePath, true);
                writer.WriteLine(text);
                writer.Close();
            }
            else if (File.Exists(filePath))
            {
                using (var writer2 = new StreamWriter(filePath, true))
                {
                    writer2.WriteLine(text);
                    writer2.Close();
                }
            }
        }



    }
}
