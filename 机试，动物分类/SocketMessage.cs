using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace 机试_动物分类
{
    public partial class SocketMessage : Form
    {
        

        public SocketMessage()
        {
            InitializeComponent();
        }
        /// <summary>
        /// 打开前
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SocketMessage_Load(object sender, EventArgs e)
        {
            
        }
        /// <summary>
        /// 关闭后
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SocketMessage_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();//关闭后全部退出
        }
        /// <summary>
        /// 跳转
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            PortOrSocket.IP=IPAddress.Parse(textBox1.Text);
            PortOrSocket.Port=Convert.ToInt32(textBox2.Text);
            Form1 form = new Form1();
            this.Hide();
            form.Show();          
        }
    }
}
