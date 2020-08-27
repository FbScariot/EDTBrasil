using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data.SqlClient;
using System.Xml;

namespace EDTBrasilScheduler
{
    public class Geral
    {
        private SqlConnection sqlConnection;

        public Geral()
        {
            //string exportDataSQL = ConfigurationManager.ConnectionStrings["ExportDataSQL"].ToString();
            //sqlConnection = new SqlConnection(exportDataSQL);
            //sqlConnection.Open();
        }

        public void LerRegistros()
        {
            try
            {
                ServiceLog.WriteErrorLog("Iniciou a Execução");
                DateTime agora = DateTime.Now.AddMinutes(-1);
                ApiSap apiSap = new ApiSap();

                apiSap.IniciarColeta();
                //TODO: Buscar relógios no MySQL
                //TODO: identificar qual o tipo de relógio
                //TODO: Chamar o relógio com os dados -IP, CPF, Login
                //TODO: Ler os registros do relógio
                //TODO: Inserir os registros no MySQL

                ServiceLog.WriteErrorLog("Concluiu a Execução com êxito");
            }
            catch (Exception ex)
            {
                ServiceLog.WriteErrorLog("Exception: " + ex);
                ServiceLog.WriteErrorLog("Concluiu a Execução com erro");
            }
        }
    }
}
