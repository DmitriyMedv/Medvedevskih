using System;
using System.Collections.Generic;
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

    public partial class Form1 : System.Windows.Forms.Form
    {
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
        }
        void USBL(string UsbID, bool PB, bool debug)
        {
            string currentPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            string currentDate = DateTime.Now.ToString("ddMMyyyy");
            string currentTime = DateTime.Now.ToString("hh:mm:ss");
            string currentPathData = currentPath + "\\logs\\" + currentDate;
            string LogFile = currentPathData + "\\logs.txt";
            if (!Directory.Exists(Path.Combine(currentPath, currentDate)))
                Directory.CreateDirectory(Path.Combine(currentPath, currentDate));

            FileStream Log = new FileStream(LogFile, FileMode.Append);
            StreamWriter writer = new StreamWriter(Log);
            //----------------------dopilit'
            if ((!PB) && (debug) && (UsbID == "USB\\VID_1A40&PID_0101\\5&ECB7860&0&6")) { writer.Write("\n" + currentTime + " " + UsbID + "usb on"); }
            PB = true;//kvm-switch
            if ((PB) && (debug) && (UsbID != "USB\\VID_1A40&PID_0101\\5&ECB7860&0&6")) { writer.Write("\n" + currentTime + " " + UsbID + "usb off"); }
            PB = false;
            writer.Dispose();

        }

        private void Button2_Click(object sender, EventArgs e)
        {
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

            //TablePort Portik;
            //Portik = new TablePort();
            //------------------------------------------
            bool PB9, PB10, PB11;
			var usbDevices = GetUSBDevices();
                PB9 = false;
				PB10 = false;//--------------------
				PB11 = false;
                foreach (var usbDevice in usbDevices)
                {
                USBL(usbDevice.DeviceID, PB9, true);
                USBL(usbDevice.DeviceID, PB10, true);
                USBL(usbDevice.DeviceID, PB11, true);

                // Log(usbDevice.DeviceID, bool PB, bool debug)

                if (usbDevice.DeviceID == "USB\\VID_1A40&PID_0201\\6&1F0E0D5E&0&2") PB11 = false;else PB11 = true;//usb-hub

                if(!PB9)pictureBox9.Visible = true;
                pictureBox10.Visible = false;//-------------
                if (PB11) pictureBox11.Visible = true;

            }
            //------------------------------------------


        }
        private void Button4_Click(object sender, EventArgs e)
        {
            TablePort Portik;
            Portik = new TablePort();
            byte[] buffer;
            buffer = new byte[2048];
            bool Sent;
            byte[] got = new byte[2048];
            

            Portik.Open();

            Sent = Portik.Write("Open programm test");
            richTextBox1.Text += "Open programm test";
            if (Sent)   richTextBox1.Text += "\nsent";
            else        richTextBox1.Text += "\nnot";
            //Portik.ReadComPort(ref buffer, 1, 2000);
            got = Portik.ReadValue(40);
            Portik.Close();
            richTextBox1.Text += "\n\n";
            for (int i = 0; i<20; i++)
            {
                richTextBox1.Text += +got[i]+"  ";
            }
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
        private void PictureBox1_Click(object sender, EventArgs e)
        {
            Form2 frm = new Form2();
            frm.label1.Text = "PC1";
            frm.Show();
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
        private object GetUSBDevices()
        {
            throw new NotImplementedException();
        }

    }

}
