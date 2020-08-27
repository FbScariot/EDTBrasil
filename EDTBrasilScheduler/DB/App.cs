using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace ExemploSDKInnerREP.DB
{
   public class App
    {
        public DataTable GetApp()
        {
            DataTable dataTable = new DataTable();

            String stringConexao = ConfigurationManager.ConnectionStrings["mysqlEDT"].ConnectionString;

            using (MySqlConnection conexaoLocal = new MySqlConnection(stringConexao))
            {
                MySqlCommand cmd = new MySqlCommand();
                cmd.CommandText = "select * " +
                                  "from nsr ";

                cmd.Connection = conexaoLocal;
                conexaoLocal.Open();

                MySqlDataReader reader = cmd.ExecuteReader();
                dataTable.Load(reader);
            }

            return dataTable;
        }
    }
}
