using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using System.Security.Cryptography;

namespace chat_application_with_crypto_stream
{
    public partial class Form1 : Form
    {
        static StreamReader str = null;
        static StreamWriter stw = null;
        TcpListener t = new TcpListener(new IPEndPoint(IPAddress.Loopback, 80));
        static NetworkStream ns;
        public Form1()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;

        }

        DESCryptoServiceProvider provider = new DESCryptoServiceProvider() { Key = Encoding.ASCII.GetBytes("HGFEDBCA"), IV = Encoding.ASCII.GetBytes("HGFEDBCA") };
        public void button1_Click(object sender, EventArgs e)
        {


            t.Start();
            TcpClient p = t.AcceptTcpClient();

             ns = p.GetStream();
            byte[] buffer = new byte[1024];
            new Thread(() =>
            {
                while (true)
                {
                    int size = ns.Read(buffer, 0, buffer.Length);
                    MemoryStream tw = new MemoryStream();
                    CryptoStream we = new CryptoStream(tw, provider.CreateDecryptor(), CryptoStreamMode.Write);
                    we.Write(buffer, 0, size);
                    we.FlushFinalBlock();
                    Invoke(new Action(() =>
                    {
                        listBox1.Items.Add(Encoding.ASCII.GetString(tw.ToArray()));
                    }));
                }
            }).Start();



        }

        public void button2_Click(object sender, EventArgs e)
        {
            MemoryStream tw = new MemoryStream();
            CryptoStream we = new CryptoStream(tw, provider.CreateEncryptor(), CryptoStreamMode.Write);
            we.Write(Encoding.ASCII.GetBytes(textBox1.Text), 0, textBox1.Text.Length);
            we.FlushFinalBlock();
            byte[] encrypted = tw.ToArray();
            ns.Write(encrypted,0,encrypted.Length);
            ns.Flush();
        }



        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
