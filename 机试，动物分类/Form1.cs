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
using System.Net;//IPAddress类
using System.Net.Sockets;//其他网口要的类
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
        #region 服务端要的内容，客户端不用倾听
        TcpClient _tc;//客户连接
        NetworkStream _ns;//网络流传输
        TcpListener _tl;//tcp连接倾听

        Thread _th;
        IPAddress _ip;//ip连接类
        int _port;//端口号
        #endregion
        /// <summary>
        /// 构造函数 初始化传入的数据
        /// </summary>
        /// <param name="serialPort1">构造函数传参时的参数</param>
        public Form1()
        {
            InitializeComponent();
            //_serialPort = serialPort1;//    构造函数接收值后赋予字段    两个界面用的是同一个实例
            ////改为单例使用
          

            if (PortOrSocket.B)
            {
                InstanceClass instance = InstanceClass.GetInstance();
                _serialPort=instance.GetParams();//instance.GetParams()返回为一个SerialPort的实例
            }
            else
            {
                SingletonClass_Socket_ instance = SingletonClass_Socket_.GetInstance();
                instance.GetParams(out _ip, out _port);
            }
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

            
            #region 确定主键的序列
            string sql = "select ID from animalTable";
            DataTable dt = SQLCSVHelper.SelectSQL(sql);
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
                                        sqlCount+=SQLCSVHelper.EditSQL(sqlOne);
                                       
                                    }
                                    else if (!string.IsNullOrEmpty(columns[2].ToString()))
                                    {
                                        string threeColumnValue = columns[2].ToString();
                                        string sqlOne = $"insert into animalTable(ID,lanimal,aanimal,sanimal)values({ID+1},'','{columns[2].ToString()}','');";
                                        sqlCount+=SQLCSVHelper.EditSQL(sqlOne);
                                       
                                    }
                                    else if (!string.IsNullOrEmpty(columns[3].ToString()))
                                    {
                                        string fourColumnValue = columns[3].ToString();
                                        string sqlOne = $"insert into animalTable(ID,lanimal,aanimal,sanimal)values({ID+1},'','','{columns[3].ToString()}');";
                                        sqlCount +=SQLCSVHelper.EditSQL(sqlOne);
                                   
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

            #region 串口或网口连接
            if (PortOrSocket.B)//串口
            {
                _serialPort.Open();//打开串口
                                   //  this为当前类（对象）下的一个实例
                if (this._serialPort != null)// 检测是否实例化
                {
                    this._serialPort.DataReceived += new SerialDataReceivedEventHandler(_serialPort_DataReceived);//    订阅事件与接收关联
                                                                                                                  //  方法_serialPort_DataReceived订阅事件_serialPort.DataReceived                                                                                                            //  事件的拥有者_serialPort，事件的成员DataReceived，事件的响应者new SerialDataReceivedEventHandler()这个实例，事件处理器_serialPort_DataReceived，订阅事件this._serialPort.DataReceived += new SerialDataReceivedEventHandler(_serialPort_DataReceived);
                }
            }
            else//网口
            {
                WaitClient();
            }
            #endregion
            textBox2.Enabled = false;
            textBox3.Enabled = false;
            textBox4.Enabled = false;
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

        #region 在Form1中实现网口连接方法，写入到分类格中 写入INI中，写入数据库中
        /// <summary>
        /// 等待用户连接
        /// </summary>
        public void WaitClient()
        {
            #region 服务端要的内容，客户端不用倾听
            //TcpClient _tc;//客户连接
            //NetworkStream _ns;//网络流传输
            TcpListener _tl;//tcp连接倾听
            IPAddress _ip;//ip连接类
            int _port;//端口号
            #endregion
            _ip = PortOrSocket.IP;
            _port=Convert.ToInt32(PortOrSocket.Port);
            //倾听
            _tl=new TcpListener(_ip, _port);
            _tl.Start();
            List<Task> tasks = new List<Task>();
            tasks.Add(Task.Run(() =>
            {
                this.textBox8.AppendText("等待网口用户连接\r\n");
                _tc=_tl.AcceptTcpClient();
                this.textBox8.AppendText("网口用户连接成功\r\n");
            }));
            Task.WhenAll(tasks).ContinueWith(t =>
            {
                _ns=_tc.GetStream();
                _th=new Thread(ReadMessage);
                _th.IsBackground=true;
                _th.Start();
            });
        }
        /// <summary>
        /// 读取网口接收的信息 
        /// </summary>
        private void ReadMessage()
        {
            while (true)
            {
                byte[] bt = new byte[1024*1024];
                int length = _ns.Read(bt, 0, bt.Length);//读的内容，偏移量，读的长度
                if (length>0)//有信息，只要在连接就有
                {
                    string str = Encoding.UTF8.GetString(bt);//接收了信息
                    textBox5.Text+="[接收]"+DateTime.Now.ToString()+":"+str+"\r\n";
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
                        byte[] btOne=Encoding.UTF8.GetBytes("please input right's format     animal's sort  colon  animal's name\r\n\" colon   is chinese symbol       animal's sort  A    Land Animals  B    Aquatic Animals  C   Sky Animals\"");//  format(格式)  sort(分类)
                        _ns.Write(btOne,0,btOne.Length);//写入_ns网络流中                      
                    }
                    else
                    {
                        byte[] btOne = Encoding.UTF8.GetBytes("OK");
                        _ns.Write(btOne, 0, btOne.Length);

                        #region 写入CSV
                        string path = Application.StartupPath+"\\CSV.csv";
                        string[] title = { "时间戳", "陆地动物", "水中动物", "天空动物" };
                        string[] parts = str.Split('：');//  分割信息
                        string[] animal = { DateTime.Now.ToString(), parts[0], parts[1] };//    时间、动物种类、动物名称
                        SQLCSVHelper.WriteCSV(path, title, animal);//    写入CSV
                        #endregion

                        #region 写入数据库
                        if (parts[0]=="A")
                        {
                            string sql = $" insert into animalTable(ID,lanimal) values({ID},'{parts[1]}');";
                            ID++;
                            SQLCSVHelper.EditSQL(sql);
                        }
                        else if (parts[0]=="B")
                        {
                            string sql = $" insert into animalTable(ID,aanimal) values({ID},'{parts[1]}');";
                            ID++;
                            SQLCSVHelper.EditSQL(sql);
                        }
                        else
                        {
                            string sql = $" insert into animalTable(ID,sanimal) values({ID},'{parts[1]}');";
                            ID++;
                            SQLCSVHelper.EditSQL(sql);
                        }
                        #endregion

                        #region 写入界面分类文本
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
                else
                {
                    textBox8.AppendText(Environment.NewLine+"网口连接断开,等待用户连接");//Environment提供有关当前环境和平台的信息以及操作它们的方法
                    //NewLine获取为此环境定义的换行字符串
                    break;
                }
            }
        }
        #endregion
        #region 串口接收信息事件方法
        /// <summary>
        /// 串口接收信息事件方法 写入到分类格中 写入INI中，写入数据库中
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
                _serialPort.WriteLine("please input right's format     animal's sort  colon  animal's name");//  format(格式)  sort(分类)
                _serialPort.WriteLine(" colon   is chinese symbol       animal's sort  A    Land Animals  B    Aquatic Animals  C   Sky Animals");

            }
            else
            {
                _serialPort.WriteLine("OK");//  回传OK
                string path = Application.StartupPath+"\\CSV.csv";

                #region 写入CSV
                string[] title = { "时间戳", "陆地动物", "水中动物", "天空动物" };
                string[] parts = str.Split('：');//  分割信息
                string[] animal = { DateTime.Now.ToString(), parts[0], parts[1] };//    时间、动物种类、动物名称
                SQLCSVHelper.WriteCSV(path, title, animal);//    写入CSV
                #endregion

                #region 写入数据库
                if (parts[0]=="A")
                {
                    string sql = $" insert into animalTable(ID,lanimal) values({ID},'{parts[1]}');";
                    ID++;
                    SQLCSVHelper.EditSQL(sql);
                }
                else if (parts[0]=="B")
                {
                    string sql = $" insert into animalTable(ID,aanimal) values({ID},'{parts[1]}');";
                    ID++;
                    SQLCSVHelper.EditSQL(sql);
                }
                else
                {
                    string sql = $" insert into animalTable(ID,sanimal) values({ID},'{parts[1]}');";
                    ID++;
                    SQLCSVHelper.EditSQL(sql);
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
        #endregion
        

        /// <summary>
        /// 读取CSV文件，读取数据库内容，并打印在文本中
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
                    DataTable dt = SQLCSVHelper.SelectSQL(sql);
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
