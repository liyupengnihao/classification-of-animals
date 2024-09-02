using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace 机试_动物分类
{
    internal class SingletonClass_Socket_ : SocketMessage
    {
        #region 服务端要的内容，客户端不用倾听
        //TcpClient _tc;//客户连接
        //NetworkStream _ns;//网络流传输
        //TcpListener _tl;//tcp连接倾听
        //IPAddress _ip;//ip连接类
        //int _port;//端口号
        #endregion

        TcpClient _tc;//客户连接(在方法外部就赋值了？)
        NetworkStream _ns;//网络流传输
        Thread _th;
        #region 饿汉式单例
        private static SingletonClass_Socket_ _instance = new SingletonClass_Socket_();
        private SingletonClass_Socket_() { }
        public static SingletonClass_Socket_ GetInstance()
        { return _instance; }
        #endregion

        public void GetParams(out IPAddress _ip,out int _port)
        {
            _ip=IPAddress.Parse(textBox1.Text);
            _port=Convert.ToInt32(textBox2.Text);
        }     
    }
}
