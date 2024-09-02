using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace 机试_动物分类
{
    public class SQLCSVHelper
    {

        /// <summary>
        /// 存数据进数据库,返回受影响的行数
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public static int EditSQL(string sql)
        {
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString="Server=localhost;Database=animalDatabase;Trusted_Connection=true";
            conn.Open();
            SqlCommand cmd = new SqlCommand(sql, conn);

            int count = 0;
            try
            {
                count=cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                count--;//  有异常count为-1
                MessageBox.Show(ex.ToString());
            }
            conn.Close();
            return count;
        }
        /// <summary>
        /// 取数据进数据库,返回DataTable(一个表)
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public static DataTable SelectSQL(string sql)
        {
            SqlConnection conn = new SqlConnection("Server=localhost;Database=animalDatabase;Trusted_Connection=true;");
            conn.Open();
            SqlCommand cmd = new SqlCommand(sql, conn);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);
            conn.Close();

            DataTable dt = new DataTable();
            dt=ds.Tables[0];
            return dt;
        }
        /// <summary>
        /// 写入CSV（四值写入）
        /// </summary>
        /// <param name="path">路劲</param>
        /// <param name="title">标题数组</param>
        /// <param name="animal">写入值数组</param>
        public static void WriteCSV(string path, string[] title, string[] animal)
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
    }

}
