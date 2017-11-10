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

namespace Client
{
    public partial class Form1 : Form
    {
        static StreamReader str = null;
        static StreamWriter stw = null;
        TcpClient client = new TcpClient();
        
        static NetworkStream ns;
        public Form1()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
        }

        byte[] buff = new byte[2048];
        DESCryptoServiceProvider provider = new DESCryptoServiceProvider() { Key = Encoding.ASCII.GetBytes("HGFEDBCA"), IV = Encoding.ASCII.GetBytes("HGFEDBCA") };
        private void button1_Click(object sender, EventArgs e)
        {




            client.Connect(new IPEndPoint(IPAddress.Parse(textBox1.Text), 80));
            MessageBox.Show("connected");

            ns = client.GetStream();
            byte[] buffer = new byte[1024];
            new Thread(() =>
            {
                while (true)
                {

                    int size = ns.Read(buffer, 0, buffer.Length);
                    MemoryStream m = new MemoryStream();
                    CryptoStream p = new CryptoStream(m, provider.CreateDecryptor(), CryptoStreamMode.Write);
                    p.Write(buffer, 0, size);
                    p.FlushFinalBlock();
                    Invoke(new Action(() =>
                    {
                        listBox1.Items.Add(Encoding.ASCII.GetString(m.ToArray()));
                    }));

                }
            }).Start();



        }

        private void button2_Click(object sender, EventArgs e)
        {
            MemoryStream tw = new MemoryStream();
            CryptoStream we = new CryptoStream(tw, provider.CreateEncryptor(), CryptoStreamMode.Write);
            we.Write(Encoding.ASCII.GetBytes(textBox2.Text), 0, textBox2.Text.Length);
            we.FlushFinalBlock();
            byte[] encrypted = tw.ToArray();
            ns.Write(encrypted, 0, encrypted.Length);
            ns.Flush();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
