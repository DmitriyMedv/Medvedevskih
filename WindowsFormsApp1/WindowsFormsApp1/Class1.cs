using System;
using System.Collections.Generic;
//using System.Data.SQLite;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Management;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    /*class SQLite//dopilit'
    { 
        public void Create()
        {
            string databaseName = @"C:\cyber.db";
            SQLiteConnection.CreateFile(databaseName);
        }
        public void CreateTable()
        {
            {
                /*
                 * command.CommandText = @"CREATE TABLE [workers] (
                    [id] integer PRIMARY KEY AUTOINCREMENT NOT NULL,
                    [name] char(100) NOT NULL,
                    [family] char(100) NOT NULL,
                    [age] int NOT NULL,
                    [profession] char(100) NOT NULL
                    );";
      command.CommandType = CommandType.Text;
      command.ExecuteNonQuery(); 
                 
                const string databaseName = @"C:\cyber.db";
                SQLiteConnection connection = new SQLiteConnection(string.Format("Data Source={0};", databaseName));
                SQLiteCommand command = 
                    new SQLiteCommand("CREATE TABLE example (" +
                    "[id] INTEGER PRIMARY KEY," +
                    "[value] TEXT" +
                    ");", connection);
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }

        }
        public void AskListOfTables()
        {
            const string databaseName = @"C:\cyber.db";
            SQLiteConnection connection = new SQLiteConnection(string.Format("Data Source={0};", databaseName));
            connection.Open();
            SQLiteCommand command = new SQLiteCommand("SELECT name FROM sqlite_master WHERE type='table' ORDER BY name;", connection);
            SQLiteDataReader reader = command.ExecuteReader();
            foreach (DbDataRecord record in reader)
            connection.Close();
        }
        public void Add()
        {
            const string databaseName = @"C:\cyber.db";
            SQLiteConnection connection = new SQLiteConnection(string.Format("Data Source={0};", databaseName));
            connection.Open();
            SQLiteCommand command = new SQLiteCommand("INSERT INTO 'example' ('id', 'value') VALUES (1, 'Вася');", connection);
            command.ExecuteNonQuery();
            connection.Close();
        }
        public void Select()
        {
            const string databaseName = @"C:\cyber.db";
            SQLiteConnection connection = new SQLiteConnection(string.Format("Data Source={0};", databaseName));
            connection.Open();
            SQLiteCommand command = new SQLiteCommand("SELECT * FROM 'example';", connection);
            SQLiteDataReader reader = command.ExecuteReader();
            Console.Write("\u250C" + new string('\u2500', 5) + "\u252C" + new string('\u2500', 60) + "\u2510");
            Console.WriteLine("\n\u2502" + "  id \u2502" + new string(' ', 30) + "value" + new string(' ', 25) + "\u2502");
            Console.Write("\u251C" + new string('\u2500', 5) + "\u253C" + new string('\u2500', 60) + "\u2524\n");
            foreach (DbDataRecord record in reader)
            {
                string id = record["id"].ToString();
                id = id.PadLeft(5 - id.Length, ' ');
                string value = record["value"].ToString();
                string result = "\u2502" + id + " \u2502";
                value = value.PadLeft(60, ' ');
                result += value + "\u2502";
                Console.WriteLine(result);
            }
            Console.Write("\u2514" + new string('\u2500', 5) + "\u2534" + new string('\u2500', 60) + "\u2518");
            connection.Close();
            Console.ReadKey(true);
        } 
    }*/
    class USBDeviceInfo
    {
        public USBDeviceInfo(string deviceID, string pnpDeviceID, string description)
        {
            this.DeviceID = deviceID;
            this.PnpDeviceID = pnpDeviceID;
            this.Description = description;
        }
        public string DeviceID { get; private set; }
        public string PnpDeviceID { get; private set; }
        public string Description { get; private set; }
        //-------------------------------------------------

    }
    class TablePort
    {
        const string PathRegistry = "Software\\armmmt";
        private SerialPort port;
        private string lastname;
        private bool stop;

        public TablePort()
        {
            port = new SerialPort
            {
                BaudRate = 57600,
                DataBits = 8,
                StopBits = StopBits.One,
                RtsEnable = false,
                WriteTimeout = 1000,
                ReadTimeout = 1000
            };
            try
            {
                RegistryKey rk = Registry.CurrentUser.CreateSubKey(PathRegistry);
                lastname = rk.GetValue("comlast", "").ToString();
            }
            catch { }
        }
        public bool ReadComPort(ref byte[] buff, int len, int timeout)
        {
            const int waiter = 200; // ms
            int att = timeout / waiter + 1;
            try
            {
                while (att-- > 0 && port.BytesToRead < len)
                {
                    Thread.Sleep(waiter);
                }
                if (port.BytesToRead < len) return false;
                int count = port.Read(buff, 0, len);
                return count == len;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public void Close()
        {
            if (port.IsOpen) port.Close();
        }
        public bool Open()
        {
            stop = false;
            string portName = "";
            Application.DoEvents();
            byte[] buff = new byte[10];
            if (lastname.Length > 0)
            {
                if (port.IsOpen && port.PortName.Equals(lastname)) return true;
                Close();
                port.PortName = lastname;
                buff[0] = 0x85;
                try
                {
                    port.Open();
                    port.DiscardInBuffer();
                    port.DiscardOutBuffer();
                    port.Write(buff, 0, 1);
                    debug(buff, 1, false);
                }
                catch (Exception e1)
                {
                    debug(lastname + ":" + e1.Message);
                }
                if (ReadComPort(ref buff, 9, 1000)) portName = lastname;
            }
            if (portName.Length == 0)
            {
                foreach (string name in SerialPort.GetPortNames())
                {
                    if (stop) return false;
                    Application.DoEvents();
                    Close();
                    port.PortName = name;
                    try
                    {
                        port.Open();
                    }
                    catch (Exception e1)
                    {
                        debug(name + ":" + e1.Message);
                        continue;
                    }
                    buff[0] = 0x85;
                    try
                    {
                        port.DiscardInBuffer();
                        port.DiscardOutBuffer();
                        port.Write(buff, 0, 1);
                        debug(buff, 1, false);
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                    if (!ReadComPort(ref buff, 9, 1000)) continue;
                    portName = name;
                    debug(buff, 9, true);
                    break;
                }
            }
            if (portName.Length == 0) return false;
            lastname = portName;
            try
            {
                RegistryKey rk = Registry.CurrentUser.CreateSubKey(PathRegistry);
                rk.SetValue("comlast", lastname);
            }
            catch { }
            return true;
        }
        public void Write(string s)
        {
            port.Write(s);
        }
        public void Stop()
        {
            stop = true;
            port.Close();
        }
        public int ReadValue(int i, int nR)
        {
            byte[] buff = new byte[6];
            int att = 10;
            while (true)
            {
                try
                {
                    port.DiscardOutBuffer();
                    port.DiscardInBuffer();
                    buff[0] = (byte)(nR == 0 ? 0x86 : nR == 1 ? 0x87 : 0x88);//???
                    buff[1] = (byte)i;
                    port.Write(buff, 0, 2);
                    debug(buff, 2, false);
                }
                catch (Exception)
                {
                    return -1;
                }
                Thread.Sleep(150);
                if (!ReadComPort(ref buff, 6, 3000))
                {
                    if (att-- > 0) continue;
                    return -1;
                }
                // данные пришли
                debug(buff, 6, true);
                int pos = buff[0] * 10 + buff[1];
                if (pos != i)
                {
                    if (att-- > 0) continue;
                    return -1;
                }
                if (buff[2] != buff[4] || buff[3] != buff[5])
                {
                    if (att-- > 0) continue;
                    return -1;
                }
                break;
            }
            return buff[2] + (buff[3] << 8);
        }
        private void debug(byte[] buff, int len, bool input)
        {
            try
            {
                StreamWriter sw = new StreamWriter("c:\\work\\log.txt", true);
                sw.Write(port.PortName + ": " + DateTime.Now);
                sw.Write(input ? "> " : "< ");
                for (int i = 0; i < len; i++)
                {
                    sw.Write("0x{0:X2} ", buff[i]);
                }
                sw.Write("   ");
                for (int i = 0; i < len; i++)
                {
                    sw.Write(buff[i] < 32 ? ". " : ((char)buff[i]).ToString());
                }
                sw.WriteLine("");
                sw.Close();
            }
            catch (Exception)
            {
            }
        }
        private void debug(string s)
        {
            try
            {
                StreamWriter sw = new StreamWriter("c:\\work\\log.txt", true);
                sw.Write("......: " + DateTime.Now + "  ");
                sw.WriteLine(s);
                sw.Close();
            }
            catch (Exception)
            {
            }
        }
    }

}
