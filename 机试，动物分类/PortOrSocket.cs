using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace 机试_动物分类
{
    internal class PortOrSocket:log_on
    {
        private static bool b;
        public static bool B
        {
            get
            {
                return b;
            }
            set
            {

                b = value;
            }
        }
        private static IPAddress ip;
        public static IPAddress IP { get; set; }
        private static int port;
        public static int Port { get; set; }
    }
}
