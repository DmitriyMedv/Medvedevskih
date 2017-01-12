using System;
using System.Collections.Generic;
using System.Management;

namespace WindowsFormsApp1
{

    public partial class Form1 : System.Windows.Forms.Form
    {
       
        private void button2_Click(object sender, EventArgs e)
        {
            {
                //------------------------------------------
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
                //------------------------------------------
            }
            TablePort Portik;
            Portik = new TablePort();
            byte[] buffer;
            buffer = new byte[2048];

            Portik.Open();
            Portik.Write("Open programm test");
            
            Portik.ReadComPort(ref buffer, 11, 2000);
            Portik.Close();

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
