using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using INIDLL;

namespace 机试_动物分类
{
    public delegate void serial();
    public partial class Form1 : Form
    {
        int l = 0;//    陆地INI序列
        int a = 0;//    水中INI序列
        int s = 0;//    天空INI序列
        bool b = true;//    判断接收的信息是否正确
        int ID = 0;
        int sqlCount = 0;
        private SerialPort _serialPort;
        /// <summary>
        /// 构造函数 初始化传入的数据
        /// </summary>
        /// <param name="serialPort1"></param>
        public Form1(SerialPort serialPort1)
        {
            InitializeComponent();
            _serialPort = serialPort1;//    构造函数接收值后赋予字段    两个界面用的是同一个实例
        }
        /// <summary>
        /// 确定Csv的行数
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <returns></returns>
        static int CountCsvRows(string filePath)
        {
            int rowCount = 0;

            try
            {
                using (StreamReader reader = new StreamReader(filePath))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        rowCount++;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"读取文件时发生错误: {ex.Message}");
            }

            return rowCount;
        }
        /// <summary>
        /// 时间控件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer1_Tick(object sender, EventArgs e)
        {
            textBox1.Text=DateTime.Now.ToString();
        }
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Load(object sender, EventArgs e)
        {
            Control.CheckForIllegalCrossThreadCalls=false;
            if (checkBox1.Checked)
                timer1.Start();
            else
                timer1.Stop();
            _serialPort.Open();
            //  this为当前类（对象）下的一个实例
            if (this._serialPort != null)// 检测是否实例化
            {
                this._serialPort.DataReceived += new SerialDataReceivedEventHandler(_serialPort_DataReceived);//    订阅事件与接收关联
                                                                                                              //  方法_serialPort_DataReceived订阅事件_serialPort.DataReceived
                                                                                                              //  事件的拥有者_serialPort，事件的成员DataReceived，事件的响应者new SerialDataReceivedEventHandler()这个实例，事件处理器_serialPort_DataReceived，订阅事件this._serialPort.DataReceived += new SerialDataReceivedEventHandler(_serialPort_DataReceived);
            }

            #region 确定主键的序列
            string sql = "select ID from animalTable";
            DataTable dt = SQLHelper.SelectSQL(sql);
            ID+=dt.Rows.Count;//    确定数据库中动物数量
            #endregion
            #region 与CSV文件同步
            string pathOne = Application.StartupPath+"\\CSV.csv";
            if (File.Exists(pathOne))
            {
                using (StreamReader sr = new StreamReader(pathOne, Encoding.Default))// 会自动释放，不用close
                {
                    if (ID<CountCsvRows(pathOne)-1)//   有标题要减一
                    {
                        string line = sr.ReadLine();//CSV第一行
                        string lines;
                        int counts=0;
                        int IDOne = ID;
                        while ((lines = sr.ReadLine())!=null)//  第二行开始
                        {
                            if (counts<IDOne)//   去除CSV中的旧内容
                            {
                                counts++;
                            }
                            else
                            {
                                string[] columns = lines.Split(',');// 分割每一行  

                                if (columns.Length > 1) // 行内有数据
                                {
                                    if (!string.IsNullOrEmpty(columns[1].ToString()))//空或null返回true
                                    {
                                        string secondColumnValue = columns[1].ToString();//  第二列的数据
                                        string sqlOne = $"insert into animalTable(ID,lanimal,aanimal,sanimal)values({ID+1},'{columns[1].ToString()}','','');";
                                        sqlCount+=SQLHelper.EditSQL(sqlOne);
                                       
                                    }
                                    else if (!string.IsNullOrEmpty(columns[2].ToString()))
                                    {
                                        string threeColumnValue = columns[2].ToString();
                                        string sqlOne = $"insert into animalTable(ID,lanimal,aanimal,sanimal)values({ID+1},'','{columns[2].ToString()}','');";
                                        sqlCount+=SQLHelper.EditSQL(sqlOne);
                                       
                                    }
                                    else if (!string.IsNullOrEmpty(columns[3].ToString()))
                                    {
                                        string fourColumnValue = columns[3].ToString();
                                        string sqlOne = $"insert into animalTable(ID,lanimal,aanimal,sanimal)values({ID+1},'','','{columns[3].ToString()}');";
                                        sqlCount +=SQLHelper.EditSQL(sqlOne);
                                   
                                    }
                                }
                                ID++;
                            }
                        }          
                    }
                }
            }
            #endregion
            #region 确定INI键的内容l、a、s
            string path = Application.StartupPath+"\\CSV.csv";
            using (StreamReader reader = new StreamReader(path, Encoding.Default))
            {
                string line = reader.ReadLine();//第一行
                string lineOne;
                while ((lineOne = reader.ReadLine())!=null)//  第二行开始
                {

                    string[] columns = lineOne.Split(',');// 分割每一行  

                    if (columns.Length > 1) // 从第二行开始
                    {
                        if (comboBox1.Text=="陆地动物")
                        {
                            if (columns[1].ToString()!="")
                            {
                                l++;
                            }
                        }
                        else if (comboBox1.Text=="水中动物")
                        {
                            if (columns[2].ToString()!="")
                            {
                                a++;
                            }
                        }
                        else
                        {
                            if (columns[3].ToString()!="")
                            {
                                s++;
                            }
                        }
                    }
                }
            }
            #endregion



        }
        /// <summary>
        /// 实时时间
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)// 选中状态
                timer1.Start();
            else
                timer1.Stop();

        }

        /// <summary>
        /// 同时退出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }
        /// <summary>
        /// 接收信息事件 写入到分类格中 写入INI中，写入数据库中
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _serialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            //定义字节数组
            byte[] bt = new byte[_serialPort.BytesToRead];
            _serialPort.Read(bt, 0, bt.Length);
            string str = Encoding.Default.GetString(bt);

            textBox5.Text += "[接收]"+DateTime.Now.ToString()+ ":"+str+"\r\n";

            string[] sort = { "A：", "B：", "C：" };
            for (int i = 0; i<sort.Length; i++)//   确定是否有分类的字母加冒号
            {
                if (str.Contains(sort[i]))
                {
                    b=false;
                    break;
                }
                else
                    continue;
            }
            if (b)
            {
                _serialPort.WriteLine("please input right's format :    animal's sort：animal's name");//  format(格式)  sort(分类)
                _serialPort.WriteLine(" ：   is chinese symbol       animal's sort：A    Land Animals 、B    Aquatic Animals 、C   Sky Animals");

            }
            else
            {
                _serialPort.WriteLine("OK");//  回传OK
                string path = Application.StartupPath+"\\CSV.csv";
                string[] title = { "时间戳", "陆地动物", "水中动物", "天空动物" };
                string[] parts = str.Split('：');//  分割信息
                string[] animal = { DateTime.Now.ToString(), parts[0], parts[1] };//    时间、动物种类、动物名称
                WriteCSV(path, title, animal);//    写入CSV

                #region 写入数据库
                if (parts[0]=="A")
                {
                    string sql = $" insert into animalTable(ID,lanimal) values({ID},'{parts[1]}');";
                    ID++;
                    SQLHelper.EditSQL(sql);
                }
                else if (parts[0]=="B")
                {
                    string sql = $" insert into animalTable(ID,aanimal) values({ID},'{parts[1]}');";
                    ID++;
                    SQLHelper.EditSQL(sql);
                }
                else
                {
                    string sql = $" insert into animalTable(ID,sanimal) values({ID},'{parts[1]}');";
                    ID++;
                    SQLHelper.EditSQL(sql);
                }
                #endregion


                #region 写入界面分类文本中
                if (parts[0]=="A")//    陆地
                {
                    textBox2.Text=parts[1]+"\r\n";
                }
                else if (parts[0]=="B")//   水中
                {
                    textBox3.Text=parts[1]+"\r\n";
                }
                else//  天空
                {
                    textBox4.Text=parts[1]+"\r\n";
                }
                #endregion


                #region 写入INI
                string jointOne = "陆地动物";// A
                string jointTwo = "水中动物";// B
                string jointThree = "天空动物";//   C
                string land = string.Format($"名称{l}=");
                string aquatic = string.Format($"名称{a}=");
                string sky = string.Format($"名称{s}=");
                if (parts[0]=="A")
                {
                    IniAPI.INIWriteValue(Application.StartupPath+"\\INI.ini", jointOne, land, parts[1]);
                    l++;
                }
                else if (parts[0]=="B")
                {
                    IniAPI.INIWriteValue(Application.StartupPath+"\\INI.ini", jointTwo, aquatic, parts[1]);
                    a++;
                }
                else
                {
                    IniAPI.INIWriteValue(Application.StartupPath+"\\INI.ini", jointThree, sky, parts[1]);
                    s++;
                }
                #endregion
            }
        }

        /// <summary>
        /// 写入CSV
        /// </summary>
        public void WriteCSV(string path, string[] title, string[] animal)
        {
            if (!File.Exists(path))
            {
                FileStream fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                StreamWriter sw = new StreamWriter(fs, Encoding.Default);
                StringBuilder sb = new StringBuilder();
                foreach (string item in title)
                {
                    sb.Append(item).Append(",");
                }
                sw.WriteLine(sb);
                sw.Close();
                fs.Close();
            }
            StreamWriter swOne = new StreamWriter(path, true, Encoding.Default);//    true为追加
            StringBuilder sbOne = new StringBuilder();
            if (animal[1]=="A")//   陆地动物
            {
                sbOne.Append(animal[0]).Append(",").Append(animal[2]).Append(",").Append(",");
            }
            else if (animal[1]=="B")//   水中动物
            {
                sbOne.Append(animal[0]).Append(",").Append(",").Append(animal[2]).Append(",");
            }
            else//  天空动物
            {
                sbOne.Append(animal[0]).Append(",").Append(",").Append(",").Append(animal[2]);
            }
            swOne.WriteLine(sbOne);
            swOne.Close();
        }

        /// <summary>
        /// 读取CSV文件，读取数据库内容
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            string path = Application.StartupPath+"\\CSV.csv";
            if (File.Exists(path))
            {

                using (StreamReader reader = new StreamReader(path, Encoding.Default))
                {

                    #region 读取数据库
                    string sql = "select*from animalTable";
                    DataTable dt = SQLHelper.SelectSQL(sql);
                    for (int i = 0; i<dt.Rows.Count; i++)
                    {
                        if (dt.Rows[i]["lanimal"].ToString()!="")//  陆地
                        {
                            textBox7.AppendText(dt.Rows[i]["lanimal"].ToString()+"\r\n");
                        }
                        else if (dt.Rows[i]["aanimal"].ToString()!="")// 水中
                        {
                            textBox7.AppendText(dt.Rows[i]["aanimal"].ToString()+"\r\n");
                        }
                        else if (dt.Rows[i]["sanimal"].ToString()!="")// 天空
                        {
                            textBox7.AppendText(dt.Rows[i]["sanimal"].ToString()+"\r\n");
                        }
                    }


                    #endregion
                    #region 读取CSV文件并打印在textbox6中
                    string line = reader.ReadLine();//第一行
                    string lines;
                    while ((lines = reader.ReadLine())!=null)//  第二行开始
                    {

                        string[] columns = lines.Split(',');// 分割每一行  

                        if (columns.Length > 1) // 从第二行开始
                        {
                            if (comboBox1.Text=="陆地动物")
                            {
                                if (!string.IsNullOrEmpty(columns[1].ToString()))//空或null返回true
                                {
                                    string secondColumnValue = columns[1].ToString();//  第二列的数据  
                                    textBox6.AppendText(secondColumnValue+"\r\n");
                                }
                            }
                            else if (comboBox1.Text=="水中动物")
                            {
                                if (!string.IsNullOrEmpty(columns[2].ToString()))
                                {
                                    string threeColumnValue = columns[2].ToString();
                                    textBox6.AppendText(threeColumnValue+"\r\n");
                                }
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(columns[3].ToString()))
                                {
                                    string fourColumnValue = columns[3].ToString();
                                    textBox6.AppendText(fourColumnValue+"\r\n");
                                }
                            }
                        }
                    }
                    #endregion
                }
            }

        }
    }
}
