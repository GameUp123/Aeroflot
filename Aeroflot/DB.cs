using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;
using MySql.Data.MySqlClient;
using System.Data.SqlClient;

namespace Aeroflot
{
    internal class DB
    {
        SqlConnection conn = new SqlConnection(@"Data Source=(localdb)\MSSQLLocalDB;AttachDbFilename=C:\Users\GameUp\Desktop\MSSQLLocalDB\FlyWay.mdf;Integrated Security=True");

        public void OpenConnection()
        {
            if (conn.State == System.Data.ConnectionState.Closed)
                conn.Open();
        }

        public void CloseConnection()
        {
            if (conn.State == System.Data.ConnectionState.Open)
                conn.Close();
        }

        public SqlConnection getConnection()
        {
            return conn;
        }
    }
}
