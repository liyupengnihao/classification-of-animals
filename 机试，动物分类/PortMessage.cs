using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace 机试_动物分类
{
    public partial class PortMessage : Form
    {
        public PortMessage()
        {
            InitializeComponent();
        }
        bool _b = true;//   确定是否要打开界面
        private void PortMessage_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        private void PortMessage_Load(object sender, EventArgs e)
        {
            foreach (var item in SerialPort.GetPortNames())
            {
                comboBox1.Items.Add(item);
            }
        }
        /// <summary>
        /// 连接串口通讯与跳转界面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {

            while (_b)
            {
                this.Hide();
                Form1 form = new Form1();
                form.ShowDialog();
                _b=false;
            }

            #region 构造函数传参的版本
            //serialPort1.PortName = comboBox1.Text;
            //serialPort1.BaudRate=Convert.ToInt32(comboBox2.Text);
            //switch (comboBox3.Text)
            //{
            //    case "NONE":
            //        serialPort1.Parity=Parity.None;
            //        break;
            //    case "ODD":
            //        serialPort1.Parity=Parity.Odd;
            //        break;
            //    case "EVEN":
            //        serialPort1.Parity=Parity.Even;
            //        break;
            //}
            //serialPort1.DataBits=int.Parse(comboBox4.Text);
            //switch (int.Parse(comboBox5.Text))// 文本
            //{
            //    case 1:
            //        serialPort1.StopBits=StopBits.One;
            //        break;
            //    case 2:
            //        serialPort1.StopBits=StopBits.Two;
            //        break;
            //}


            ////打开窗口并传参
            //while (_b)
            //{
            //    this.Hide();
            //    Form1 form = new Form1(serialPort1);//  通过构造函数传值
            //    form.ShowDialog();
            //    _b=false;
            //}
            #endregion
        }


    }
}
