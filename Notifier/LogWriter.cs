using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notifier
{
  public  class LogWriter
    {
        public static void WriteErrorLog(string message)
        {
            StreamWriter sw = null;
            try
            {

                sw = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "\\LogFile.txt", true);
                sw.WriteLine(DateTime.Now.ToString() + " : " + message);
                sw.Flush();
           
           
                sw.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public static void CleanLog()
        {
            StreamWriter sw = null;
            try
            {

                sw = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "\\LogFile.txt", true);
                sw.Flush();
                long fileSize = sw.BaseStream.Length / (1024 * 1024);
                sw.Close();
                if (fileSize>=100) {
                    sw = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "\\LogFile.txt", false);
                    sw.Flush();
                    sw.Close();
                }
           
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

    }
    }

