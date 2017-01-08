using System;
using System.IO;
using System.IO.Ports;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Management;

namespace WindowsFormsApp1
{

    static class Program
    {

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());

            //System.IO.File.AppendAllText(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), DateTime.Now.ToString("dd:MM:yyyy hh:mm:ss Run Programm\n"));
            //string currentPath = Directory.GetCurrentDirectory();

            string currentPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            string currentDate = DateTime.Now.ToString("ddMMyyyy");
            string currentTime = DateTime.Now.ToString("hh:mm:ss");
            string currentPathData = currentPath + "\\" + currentDate;
            string LogFile = currentPathData + "\\logs.txt";
            if (!Directory.Exists(Path.Combine(currentPath, currentDate)))
                 Directory.CreateDirectory(Path.Combine(currentPath, currentDate));

            //if (!File.Exists(LogFile))
            



            FileStream Logs = new FileStream(LogFile, FileMode.Append);
            StreamWriter writer = new StreamWriter(Logs); //создаем «потоковый писатель» и связываем его с файловым потоком 
            writer.Write("\nSmartHome streeeeeeeeeeeaaaaam\n------\n"+currentTime+"Start application"); //записываем в файл
            writer.Close(); //закрываем поток. Не закрыв поток, в файл ничего не запишется 


            // if(!File.Exists(LogFile))
            //File.CreateText(LogFile);
            //File.AppendAllText(LogFile, "\nSmartHome\n-----\n"+currentTime+"Start application");   

        }
    }
}