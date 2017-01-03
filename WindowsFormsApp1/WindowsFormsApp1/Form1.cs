using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using LibUsbDotNet;
using LibUsbDotNet.Info;
using LibUsbDotNet.Main;

namespace WindowsFormsApp1
{

    public partial class Form1 : System.Windows.Forms.Form
    {
        public static UsbDevice MyUsbDevice0;
        public static UsbDevice MyUsbDevice1;
        public static UsbDevice MyUsbDevice2;
        public static UsbDevice MyUsbDevice3;
        public static UsbDevice MyUsbDevice4;
        public static UsbDevice MyUsbDevice5;

        public static UsbDeviceFinder MyUsbFinder = new UsbDeviceFinder(0x16c0, 0x05df);//VID; PID
        //-------------------------

        public void Button_Click(object sender, EventArgs e)
        {
            // Label l = sender as Label;
            //this.form2.show();
            Form2 frm = new Form2();
            frm.Show();

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
           
        }
    }
}
