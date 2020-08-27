using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.Common;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using System.Configuration;

namespace ExemploSDKInnerREP.DB
{
    public class Conexao
    {
        private string stringConexao;

        public MySqlConnection GetConexao()
        {
            stringConexao = ConfigurationManager.ConnectionStrings["mysqlEDT"].ConnectionString;
            MySqlConnection conexao = new MySqlConnection(stringConexao);
            return conexao;
        }

        public DbCommand GetComando(DbConnection conexao)
        {
            DbCommand comando = conexao.CreateCommand();

            return comando;
        }

        public DbDataReader GetDbDataReader(DbCommand comando)
        {
            return comando.ExecuteReader();
        }


    }
}
