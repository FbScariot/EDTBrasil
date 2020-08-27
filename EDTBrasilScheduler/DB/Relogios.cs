using ExemploSDKInnerREP.DB;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiSap.DB
{
    class Relogios
    {
        private string stringConexao = ConfigurationManager.ConnectionStrings["mysqlEDT"].ConnectionString;

        public DataTable GetRelogios()
        {
            DataTable dataTable = new DataTable();

            using (MySqlConnection conexaoLocal = new MySqlConnection(this.stringConexao))
            {
                MySqlCommand cmd = new MySqlCommand();
                cmd.CommandText = "SELECT a.id as reg, a.codigoEquipamento as codigo, a.chaveAutenticacao as chave, a.ip as endereco, b.marca as m, " +
                                   "a.dataCadastro as dtc," +
                                   "a.statusColeta as coleta," +
                                   "a.ultimaColeta as dtcoleta" +
                                   " FROM equipamentos a " +
                                   "INNER JOIN marcasequipamentos b ON b.id = a.codRelogio";

                cmd.Connection = conexaoLocal;
                conexaoLocal.Open();
                MySqlDataReader reader = cmd.ExecuteReader();

                dataTable.Load(reader);
            }

            return dataTable;
        }
    }
}
