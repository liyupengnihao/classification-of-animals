using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
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
            if(textBox1.Text=="你好"&&textBox2.Text=="123")
            {
                this.Hide();
                PortMessage pm= new PortMessage();
                pm.Show();
                
            }
                
        }
    }
}
