using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;

namespace HotelRoomBookingSystem.Models
{
    public class Logger
    {
        private static readonly string appEventFolder = "ApplicationLog";

        public static void WriteLog(string strMsg, string strStack, string strSource, long strUser)
        {
            string LogPath;
            string LogFileName = ConfigurationManager.AppSettings["LogFileName"];
            string LogDateFormat = ConfigurationManager.AppSettings["LogDateFormat"];
            string LogFileExtension = ConfigurationManager.AppSettings["logFileExtension"];
            DateTime eventTime = System.DateTime.Now;

            if (ConfigurationManager.AppSettings["LogPath"] == null)
                LogPath = HttpContext.Current.Server.MapPath(appEventFolder) + "\\" + eventTime.ToString(LogDateFormat) + LogFileName + LogFileExtension;
            else
                LogPath = ConfigurationManager.AppSettings["LogPath"] + "\\" + eventTime.ToString(LogDateFormat) + LogFileName + LogFileExtension;

            if (IsExistFolder(Path.GetDirectoryName(LogPath)))
            {
                // Check if log path exist or not to determine the file operation of either appending
                // text if file is exist, or create new text file if file is not exit.
                StreamWriter sw = null;

                // Check if the defined log file is exist in directory
                if (File.Exists(LogPath))
                {
                    // Set stream writer to append text into text file
                    sw = File.AppendText(LogPath);
                }
                else
                {
                    // Set stream writer to create new text file
                    sw = File.CreateText(LogPath);
                }

                using (sw)
                {
                    // Write logging content
                    sw.WriteLine("".PadLeft(120, '='));
                    sw.WriteLine("");
                    sw.WriteLine("Log Date:" + eventTime.ToShortDateString() + "\t" + eventTime.ToLongTimeString());
                    sw.WriteLine("");
                    sw.WriteLine("Log Source:" + strSource);
                    sw.WriteLine("");
                    sw.WriteLine("Log Page:" + Path.GetFileName(HttpContext.Current.Request.PhysicalPath));
                    sw.WriteLine("");
                    sw.WriteLine("Log User:" + strUser);
                    sw.WriteLine("");
                    sw.WriteLine("Error Message:" + strMsg);
                    sw.WriteLine("");
                    sw.WriteLine("Error Stack:" + strStack);
                    sw.WriteLine("");
                    sw.WriteLine("".PadLeft(120, '='));
                    sw.WriteLine("");
                }
            }
            else
            {
                throw new Exception("Invalid Log Path. Path given: " + LogPath);
            }
        }

        private static bool IsExistFolder(string folder)
        {
            if (Directory.Exists(folder))
            {
                return true;
            }
            Directory.CreateDirectory(folder);
            return true;
        }
    }
}
