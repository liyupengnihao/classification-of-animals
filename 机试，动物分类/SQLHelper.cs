using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace 机试_动物分类
{
    public class SQLHelper
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
    }

}
