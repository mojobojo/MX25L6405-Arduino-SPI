using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;
using System.Threading;

namespace LexmarkPrinterDump
{
    public partial class Form1 : Form
    {
        SerialPort serialPort;
        private delegate void UpdatePBDel(int value);

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                serialPort = new SerialPort(textBox1.Text);

                serialPort.BaudRate = 115200;
                serialPort.Parity = Parity.None;
                serialPort.StopBits = StopBits.One;
                serialPort.DataBits = 8;
                serialPort.Handshake = Handshake.None;

                serialPort.Open();

                serialPort.WriteLine("i");

                //string s1 = serialPort.ReadLine().Replace("\n", "").Replace("\r", "");
                //string s2 = serialPort.ReadLine().Replace("\n", "").Replace("\r", "");
                //string s3 = serialPort.ReadLine().Replace("\n", "").Replace("\r", "");


                byte[] data = new byte[3];
                serialPort.Read(data, 0, 1);
                serialPort.Read(data, 1, 1);
                serialPort.Read(data, 2, 1);


                MessageBox.Show("Connected to " + textBox1.Text + "\nRDID: " + data[0].ToString("X2") + data[1].ToString("X2") + data[2].ToString("X2"));

                //mySerialPort.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);

                //mySerialPort.Open();

                //Console.WriteLine("Press any key to continue...");
                //Console.WriteLine();
                //Console.ReadKey();
                //mySerialPort.Close();
            }
            catch (Exception x)
            {
                MessageBox.Show(x.Message);
            }
        }

        private void UpdateProgressBar(int value)
        {
            if (progressBar1.InvokeRequired)
            {
                progressBar1.Invoke(new UpdatePBDel(UpdateProgressBar), new object[] { value });
            }

            progressBar1.Value = value;
        }

        private void DumpThread()
        {
            serialPort.WriteLine("d");

            byte[] data = new byte[0x800000];

            for (int i = 0; i < data.Length; i++)
            {
                UpdateProgressBar((int)Math.Round(((double)i / (double)data.Length) * 100));
                serialPort.Read(data, i, 1);
            }

            //SaveFileDialog sfd = new SaveFileDialog();

            //if (sfd.ShowDialog() == DialogResult.OK)
            //{
            System.IO.File.WriteAllBytes("C:\\dump.bin", data);
            //}

            UpdateProgressBar(0);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Thread t = new Thread(DumpThread);
            t.Start();
        }

        //private static void DataReceivedHandler(
        //                object sender,
        //                SerialDataReceivedEventArgs e)
        //{
        //    SerialPort sp = (SerialPort)sender;
        //    string indata = sp.ReadExisting();
        //    Console.WriteLine("Data Received:");
        //    Console.Write(indata);
        //}
    }
}
