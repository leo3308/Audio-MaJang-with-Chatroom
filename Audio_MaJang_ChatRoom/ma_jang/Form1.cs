using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Chatclass;

namespace ma_jang
{
    public partial class Form1 : Form
    {
        TcpClient clientsocket = new TcpClient();
        Packet Info = new Packet();

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form2 f2 = new Form2(textBox1.Text);
            f2.Show();
            this.Hide();
        }
    }
}
