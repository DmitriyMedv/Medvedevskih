using System;
using System.IO;
using System.IO.Ports;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Management;
using System.Net;
using Microsoft.Win32;

namespace WindowsFormsApp1
{

    public partial class Form1 : System.Windows.Forms.Form
    {
       
        private void button2_Click(object sender, EventArgs e)
        {
            var usbDevices = GetUSBDevices();
            pictureBox9.Visible = true;
            pictureBox10.Visible = false;//-------------
            pictureBox11.Visible = true;

            foreach (var usbDevice in usbDevices)
            {
                //---------------------------------
                //kvm-switch
                if (usbDevice.DeviceID == "USB\\VID_1A40&PID_0101\\5&ECB7860&0&6") pictureBox9.Visible = false;
                //usb-hub
                if (usbDevice.DeviceID == "USB\\VID_1A40&PID_0201\\6&1F0E0D5E&0&2") pictureBox11.Visible = false;
                /*  write log by each usb device
                 */
                //---------------------------------
            }
            List<USBDeviceInfo> GetUSBDevices()
            {
                List<USBDeviceInfo> devices = new List<USBDeviceInfo>();

                ManagementObjectCollection collection;
                using (var searcher = new ManagementObjectSearcher(@"Select * From Win32_USBHub"))
                    collection = searcher.Get();

                foreach (var device in collection)
                {
                    devices.Add(new USBDeviceInfo(
                    (string)device.GetPropertyValue("DeviceID"),
                    (string)device.GetPropertyValue("PNPDeviceID"),
                    (string)device.GetPropertyValue("Description")
                    ));
                }

                collection.Dispose();
                return devices;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }
        private void Form1_Close(object sender, EventArgs e)
        {
            //writer.Write("\nSmartHome streeeeeeeeeeeaaaaam\n------\n" + currentTime + "Start application"); //записываем в файл
            // writer.Close(); //закрываем поток. Не закрыв поток, в файл ничего не запишется 

        }
        public Form1()
        {
            InitializeComponent();
        }

        private void ОПрограммеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox1 Abfrm = new AboutBox1();
            Abfrm.Show();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            //Form2 frm = new Form2();
            //frm.Show();



            TablePort Portik;
            Portik = new TablePort();
            Portik.Open();
            Portik.Write("hello world");
            Portik.ReadComPort(, 11, 2000);
            Portik.Close();


        }
        private void pictureBox9_Click(object sender, EventArgs e)
        {

            if (pictureBox9.Visible)
            {
                Form2 frm = new Form2();
                frm.label1.Text = "KVM-switch \n находится в обрыве";
                frm.Show();

            }
        }
        private void pictureBox11_Click(object sender, EventArgs e)
        {
            if (pictureBox11.Visible)
            {
                Form2 frm = new Form2();
                frm.label1.Text = "USB-hub \n находится в обрыве";
                frm.Show();
              
            }
        }
        private void tabPage2_Click(object sender, EventArgs e)
        {

        }

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
                        buff[0] = (byte)(nR == 0 ? 0x86 : nR == 1 ? 0x87 : 0x88);
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

        private object GetUSBDevices()
        {
            throw new NotImplementedException();
        }

    }
}
