using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Management;
using System.Net;


namespace WindowsFormsApp1
{

    public partial class Form1 : System.Windows.Forms.Form
    {
  //      public UsbDevice MyUsbDevice;
    //    public UsbDeviceFinder MyUsbFinder = new UsbDeviceFinder(0x1A40, 0x0101);//VID; PID
        //-------------------------
      
        public void Button2_Click(object sender, EventArgs e)
        {
            //Form2 frm = new Form2();
            //frm.Show();
           

        }

        private void Form1_Load(object sender, EventArgs e)
        {
         
        }


        public Form1()
        {
            InitializeComponent();
        }

        private void ОПрограммеToolStripMenuItem_Click(object sender, EventArgs e)
        {
        
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            var usbDevices = GetUSBDevices();

            foreach (var usbDevice in usbDevices)
            {
                //richTextBox1.Text += "\nDevice ID: {0}, PNP Device ID: {1}, Description: {2} \n";
                richTextBox1.Text += "\n" + usbDevice.DeviceID;
                if (usbDevice.DeviceID == "USB\\VID_1A40&PID_0101\\5&ECB7860&0&6") { pictureBox9.Visible = false; } else { pictureBox9.Visible = true; };
                //richTextBox1.Text += usbDevice.PnpDeviceID + "\n";
                //richTextBox1.Text += usbDevice.Description;
            }

            
            Console.Read();

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

            
            //GetDeviceList
            //  MyUsbDevice = UsbDevice.OpenUsbDevice(MyUsbFinder);
           /* if ()//MyUsbDevice != null)
            {
                label5.Text = " подключено !";
            }
            else label5.Text = " не найдено !";
            */

    

        private object GetUSBDevices()
        {
            throw new NotImplementedException();
        }
    }
}
