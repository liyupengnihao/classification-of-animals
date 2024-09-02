using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace 机试_动物分类
{
    //饿汉式（类加载慢，对象加载快）
    internal class InstanceClass : PortMessage
    {
        private static SerialPort serialPort;
        #region 饿汉式
        private InstanceClass() { }
        private static InstanceClass _instance = new InstanceClass();
        public static InstanceClass GetInstance()
        {
            return _instance;
        }
        #endregion

        public SerialPort GetParams()
        {
            serialPort= new SerialPort();
            serialPort.PortName = comboBox1.Text;
            serialPort.BaudRate=Convert.ToInt32(comboBox2.Text);
            switch (comboBox3.Text)
            {
                case "NONE":
                    serialPort.Parity=Parity.None;
                    break;
                case "ODD":
                    serialPort.Parity=Parity.Odd;
                    break;
                case "EVEN":
                    serialPort.Parity=Parity.Even;
                    break;
            }
            serialPort.DataBits=int.Parse(comboBox4.Text);
            switch (int.Parse(comboBox5.Text))// 文本
            {
                case 1:
                    serialPort.StopBits=StopBits.One;
                    break;
                case 2:
                    serialPort.StopBits=StopBits.Two;
                    break;
            }
            return serialPort;
        }


    }
}
