using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace 机试_动物分类
{
    public partial class log_on : Form
    {
        public log_on()
        {
            InitializeComponent();         
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (radioButton2.Checked)//串口
            {
                PortOrSocket.B=true;
                if (textBox1.Text=="你好"&&textBox2.Text=="123")
                {
                    this.Hide();
                    PortMessage pm = new PortMessage();
                    pm.Show();

                }
                else               
                    MessageBox.Show("账号或密码错误");               
            }   
            else//网口
            {
                if(textBox1.Text=="你好"&&textBox2.Text=="123")
                {
                    PortOrSocket.B=false;
                    this.Hide();
                    SocketMessage pm = new SocketMessage();
                    pm.Show();
                }
                else
                    MessageBox.Show("账号或密码错误");
            }
        }
    }
}
