using ApiSap.DB;
using ExemploSDKInnerREP.DB;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EDTBrasilScheduler
{
    public partial class ApiSap
    {
        //Delegates para ser chamados nos metodos executados pelas Thread
        private delegate void AtualizaListBoxColeta(int rep, string v);
        private delegate void AtualizaUltimaEmpresaColeta(int rep, string v);
        private delegate void AtualizaNumRegistrosColeta(int rep, string v);
        private delegate void AtualizaNumSerieColeta(int rep, string v);
        private delegate void AtualizaMaiorDataColeta(int rep, string v);
        private delegate void AtualizaMenorDataColeta(int rep, string v);
        //private delegate void AtualizaLabels(Label lbl, string v);
        //private delegate void AtualizaTextBox(TextBox txt, string v);
        private delegate void AtualizaLabelsStatusPapel(int rep, int impressora, string v);
        //private delegate void AtualizaListBoxUsuariosBio(ListBox list, string v);
        //private delegate void AtualizaGridTemplates(DataGridView dgv, DataGridViewRow row, string Acao);

        private delegate void AtualizaGridEmpregadosRecebidos(int rep, string[] row);
        private delegate void LimpaGridEmpregadosRecebidos(int REP);

        private delegate void AtualizaGridEmpregadosRecebidosRB1(string[] row);
        private delegate void LimpaGridEmpregadosRecebidosRB1();

        private delegate void AtualizaGridTemplatesRB1(string[] row);
        private delegate void LimpaGridTemplatesRB1();

        private delegate void AtualizaGridEmpregadosExcluidosRecebidos(int rep, string[] row);
        private delegate void LimpaGridEmpregadosExcluidosRecebidos(int REP);

        private delegate void AtualizaGridEmpregadosAlteracoesRecebidos(int rep, string[] row);
        private delegate void LimpaGridEmpregadosAlteracoesRecebidos(int REP);

        private delegate int BuscaListBoxUsuariosBioSel(int rep);

        private delegate void LimpaListBoxColeta(int rep);
        //private delegate void LimpaListBoxUsuariosBio(ListBox list);

        private delegate void AtualizarTela();

        private String _UltimoRegistroBD = String.Empty;
        private String _UltimoRegistroProcessado = String.Empty;
        private String _IdEquipamento = String.Empty;
        private Int32 _NumeroTotal = 0;
        private String stringConexao = ConfigurationManager.ConnectionStrings["mysqlEDT"].ConnectionString;

        private bool _pararColeta = false;

        //Parametros para comunicacao com os REPS.
        class ParametrosComunicacao
        {
            public bool _RepIniciaComunicacao = false;
            public string _IP = "";
            public string _CpfResponsavel = "";
            public string _IPcomputador = "";
            public int _PortaComputador = 60000;
            public int _TempoEspera = 20;
            public string _Nomecomputador = "";
            public string _Mascara = "";
            public string _Gateway = "";
            public int _Intervaloconexao = 5;
            public int _ID = 0;
            public bool _HabilitaDNS = false;
            public string _HostDNS = "";
            public string _DNS = "";
            public string _NomeRep = "";

            public string[] _Selecionados = null;
            public string[,] _SelecionadosGrid = null;

            public int _QtdSelecionados = 0;
            public int _QtdGrid = 0;

            //Parametros para comunicacao com os REPS.
            public ParametrosComunicacao(int REP, string cpf, string ip, string[] arSelecionados)
                : this(REP, cpf, ip)
            {
                if (REP == 1)
                {
                    _Selecionados = arSelecionados;
                }
                else
                {
                    _RepIniciaComunicacao = false;
                    _Selecionados = arSelecionados;
                }

            }

            public ParametrosComunicacao(int REP, string cpf, string ip, string[,] arSelecionados, int qtdSelecionados)
                : this(REP, cpf, ip)
            {
                if (REP == 1)
                {
                    _SelecionadosGrid = arSelecionados;
                }

                _QtdSelecionados = qtdSelecionados;

            }

            public ParametrosComunicacao(int REP, string cpf, string ip)
            {
                _ID = REP;
                _CpfResponsavel = cpf;

                if (REP == 1)
                {
                    _RepIniciaComunicacao = false;
                    _IP = ip;
                }
            }
        }

        public void IniciarColeta()
        {
            //busca ip na base de equipamentos
            using (MySqlConnection conexaoLocal = new MySqlConnection(stringConexao))
            {
                MySqlCommand cmd = new MySqlCommand();
                cmd.CommandText = "select id, ip, cpf, coletaFinal " +
                                  "from equipamentos ";

                cmd.Connection = conexaoLocal;
                conexaoLocal.Open();

                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    string ip = reader["ip"].ToString();
                    _UltimoRegistroBD = reader["coletaFinal"].ToString();
                    _IdEquipamento = reader["id"].ToString();
                    string numNsr = reader["coletaFinal"].ToString();
                    string cpf = reader["cpf"].ToString();
                    //MessageBox.Show(reader[0].ToString());

                    // dispara uma nova thread para executar 
                    ParametrosComunicacao comunicacao = new ParametrosComunicacao(1, cpf, ip);
                    ParametrosComunicacao comunicacao2 = new ParametrosComunicacao(2, cpf, ip);
                    Boolean multiThread = true;

                    if (multiThread)
                    {
                        ThreadStart starter = delegate { Coleta1(comunicacao, numNsr); };
                        Thread Rep1 = new Thread(starter);
                        Rep1.Start();
                    }
                    else
                    {
                        //MessageBox.Show("Passou!");

                        Coleta1(comunicacao, numNsr);
                    }
                }
            }
        }

        private void Coleta1(ParametrosComunicacao pComunicacao, string numNsr)
        {
            _pararColeta = false;
            int ret;
            int nsr = 0;
            int statusColeta = 0;
            int resultadoColeta = 0;
            string numeroSerie;
            string dadosRegistro;
            bool recebeuUltimoBilhete = false;
            int numeroRegistrosLidos = 0;
            int tentativas = 0;

            //Limpa o listboxcoleta pela Thread
            //Invoke(new LimpaListBoxColeta(LimparListBoxColeta), new object[] { pComunicacao._ID });

            /* Cria uma instancia da classe InnerRepSdk */
            Sdk_Inner_Rep.InnerRepPlusSDK innerRep = new Sdk_Inner_Rep.InnerRepPlusSDK();

            /* Define os parâmetros de comunicação com o REP é possível inibir o uso do ping na comunicação 1 não envia ping e qualquer outro valor envia o ping */
            ret = innerRep.DefinirParametrosComunicacao(pComunicacao._CpfResponsavel, pComunicacao._IP, "**AUTENTICACAO**", 1);

            if (ret == (int)Sdk_Inner_Rep.InnerRepPlusSDK.ParametrosComunicacao.ERRO_CPF_RESPONSAVEL)
            {
                //Invoke(new AtualizaListBoxColeta(AtualizarListBoxColeta), new object[] { pComunicacao._ID, "Informe o CPF!" });
                return;
            }

            /* Define que o REP ira iniciar a comunicacao passando os parametros Porta do Pc onde o rep ira comunicar e o tempo em segundos para cada conexao */
            if (pComunicacao._RepIniciaComunicacao)
            {
                ret = innerRep.DefinirRepIniciaConexao(pComunicacao._PortaComputador, pComunicacao._TempoEspera);
            }

            nsr = int.Parse(numNsr) - 1;

            /* Loop até receber o último NSR */
            while (!recebeuUltimoBilhete && !_pararColeta)
            {
                try
                {
                    String UltimoRegistro = String.Empty;
                    /* Solicita os Registros de forma incremental... */
                    nsr++;
                    ret = innerRep.SolicitarRegistroNsr(nsr);

                    /* Verifica a conexão com o REP */

                    if (ret == 0)
                    {
                        /* Solicita o Status da Leitura.. */
                        statusColeta = innerRep.LerStatusColeta();
                        tentativas = 0;
                        /* Enquanto Status da leitura estiver em Andamento (1) verifica o status novamente.. */
                        while (statusColeta < 2 && tentativas < 1000)
                        {
                            Thread.Sleep(10);
                            //Application.DoEvents();
                            Thread.Sleep(10);
                            statusColeta = innerRep.LerStatusColeta();
                            tentativas++;
                        }

                        /* Status da coleta recebido, verifica se recebeu o registro com sucesso */
                        if ((statusColeta == (int)Sdk_Inner_Rep.InnerRepPlusSDK.StatusLeitura.FINALIZADA_COM_ULTIMO_REGISTRO) ||
                            (statusColeta == (int)Sdk_Inner_Rep.InnerRepPlusSDK.StatusLeitura.FINALIZADA_COM_REGISTRO))
                        {
                            /* Recupera o número de Série do REP */
                            numeroSerie = innerRep.LerNumSerieRep();

                            //Atualiza o label NumSeriecoleta pela Thread
                            //Invoke(new AtualizaNumSerieColeta(AtualizarNumSerieColeta), new object[] { pComunicacao._ID, numeroSerie });

                            /* Recebe o resultado da leitura */
                            resultadoColeta = innerRep.LerResultadoColeta();

                            /* Faz a leitura do registro e exibe no listbox */
                            dadosRegistro = innerRep.LerRegistroLinha();

                            if (dadosRegistro != "" && dadosRegistro != null)
                            {
                                Int32 regAfetado = 0;
                                
                                using (MySqlConnection conexaoLocal = new MySqlConnection(this.stringConexao))
                                {
                                    MySqlCommand cmd = new MySqlCommand();
                                    cmd.CommandText = "INSERT INTO nsr (nsr) VALUES (@nsr) ";

                                    cmd.Connection = conexaoLocal;
                                    conexaoLocal.Open();
                                    cmd.Parameters.Add(new MySqlParameter("@nsr", dadosRegistro));
                                    regAfetado = cmd.ExecuteNonQuery();

                                    if (regAfetado == 0)
                                    {
                                        //MessageBox.Show("DadosRegistro não incluido:" + dadosRegistro);
                                    }

                                    //Insere no listboxcoleta pela Thread
                                    //Invoke(new AtualizaListBoxColeta(AtualizarListBoxColeta), new object[] { pComunicacao._ID, dadosRegistro });
                                }

                                //MessageBox.Show("inclui:" + dadosRegistro);
                                //MessageBox.Show("tamanho:" + dadosRegistro.Length.ToString());

                                /* PARTE DA TABELA NOVA */
                                Int32 iRegAfetado = 0;
                                String sCodigo = dadosRegistro.Substring(0, 9);
                                String sRegistro = dadosRegistro.Substring(9, 1);
                                String sDatas = dadosRegistro.Substring(10, 8);

                                //MessageBox.Show(sDatas);

                                sDatas = sDatas.Substring(4, 4) + "-" + sDatas.Substring(2, 2) + "-" + sDatas.Substring(0, 2);
                                String sHora = dadosRegistro.Substring(18, 4);
                                sHora = sHora.Substring(0, 2) + ":" + sHora.Substring(2, 2);
                                String sPis = dadosRegistro.Substring(22);

                                if (sPis.Length > 12)
                                {
                                    sPis = sPis.Substring(0, 12);
                                }

                                using (MySqlConnection conexaoLocal = new MySqlConnection(this.stringConexao))
                                {
                                    MySqlCommand cmd = new MySqlCommand();
                                    cmd.CommandText = "INSERT INTO registros(idEquipamento, codigo, registro, data, hora, pis, nsrcompleto) VALUES(@idEquipamento, @codigo, @registro, @data, @hora, @pis, @nsrcompleto)";

                                    cmd.Connection = conexaoLocal;
                                    conexaoLocal.Open();
                                    cmd.Parameters.Add(new MySqlParameter("@idEquipamento", _IdEquipamento));
                                    cmd.Parameters.Add(new MySqlParameter("@codigo", sCodigo));
                                    cmd.Parameters.Add(new MySqlParameter("@registro", sRegistro));
                                    cmd.Parameters.Add(new MySqlParameter("@data", sDatas));
                                    cmd.Parameters.Add(new MySqlParameter("@hora", sHora));
                                    cmd.Parameters.Add(new MySqlParameter("@pis", sPis));
                                    cmd.Parameters.Add(new MySqlParameter("@nsrcompleto", dadosRegistro));

                                    iRegAfetado = cmd.ExecuteNonQuery();

                                    if (iRegAfetado == 0)
                                    {
                                        //MessageBox.Show("DadosRegistro não incluido:" + dadosRegistro);
                                    }

                                    _UltimoRegistroProcessado = sCodigo;
                                }
                            }

                            /* Incrementa a quantidade de registros lidos e atualiza o label..*/
                            numeroRegistrosLidos++;
                            //lblNumeroRegistrosLidos.Text = numeroRegistrosLidos.ToString();
                            _NumeroTotal = _NumeroTotal + numeroRegistrosLidos;
                            //Atualiza o label NumRegistrosLidos pela Th.read
                            //Invoke(new AtualizaNumRegistrosColeta(AtualizarNumRegLidosColeta), new object[] { pComunicacao._ID, numeroRegistrosLidos.ToString() });
                        }

                        /* Exibe o maior registro lido e o menor registro Lido */
                        //Invoke(new AtualizaMaiorDataColeta(AtualizarMaiorDataColeta), new object[] { pComunicacao._ID, innerRep.LerMaiorDataEntreRegistroLidos() });
                        //Invoke(new AtualizaMenorDataColeta(AtualizarMenorDataColeta), new object[] { pComunicacao._ID, innerRep.LerMenorDataEntreRegistroLidos() });

                        /* Exibe o maior registro de empresa Lido */
                        //Invoke(new AtualizaUltimaEmpresaColeta(AtualizarEmpresaLidaColeta), new object[] { pComunicacao._ID, innerRep.LerUltimaEmpresa() });

                        /* Se o status da coleta foi finalizado com ultimo registro, ou se a coleta não possui registros, sai do laço.. */
                        if ((statusColeta == (int)Sdk_Inner_Rep.InnerRepPlusSDK.StatusLeitura.FINALIZADA_SEM_REGISTRO) ||
                            (statusColeta == (int)Sdk_Inner_Rep.InnerRepPlusSDK.StatusLeitura.FINALIZADA_COM_ULTIMO_REGISTRO))
                        {
                            recebeuUltimoBilhete = true;
                        }
                        else // Falha de Conexão 
                            if (statusColeta == (int)Sdk_Inner_Rep.InnerRepPlusSDK.StatusLeitura.FALHA_DE_CONEXAO)
                        {
                            //Invoke(new AtualizaListBoxColeta(AtualizarListBoxColeta), new object[] { pComunicacao._ID, "Não foi possível realizar a conexão com o Inner Rep Plus de IP:  " + pComunicacao._IP });
                            //MessageBox.Show("Não foi possível realizar a conexão com o Inner Rep Plus de IP: " + pComunicacao._IP);
                            break;
                        }
                    }
                    else
                    {
                        //Atualiza o listboxColeta pela Thread
                        //Invoke(new AtualizaListBoxColeta(AtualizarListBoxColeta), new object[] { pComunicacao._ID, "erro" });

                        break;
                    }
                }
                catch (Exception ex)
                {
                    //MessageBox.Show("Erro: " + ex.ToString());
                }
            }

            using (MySqlConnection conexaoLocal = new MySqlConnection(this.stringConexao))
            {
                MySqlCommand cmd = new MySqlCommand();
                cmd.CommandText = "UPDATE EQUIPAMENTOS SET coletaInicial=@coletaInicial, coletaFinal=@coletaFinal WHERE id=@id ";

                cmd.Connection = conexaoLocal;
                conexaoLocal.Open();
                cmd.Parameters.Add(new MySqlParameter("@id", _IdEquipamento));
                cmd.Parameters.Add(new MySqlParameter("@coletaInicial", _UltimoRegistroBD));
                cmd.Parameters.Add(new MySqlParameter("@coletaFinal", _UltimoRegistroProcessado));
                try
                {
                    int i = cmd.ExecuteNonQuery();

                    if (i > 0)
                    {
                        //MessageBox.Show("Registro atualizado com sucesso!");
                    }
                }
                catch (Exception ex)
                {
                    //MessageBox.Show("Erro: " + ex.ToString());
                }
            }

            if (recebeuUltimoBilhete)
            {
                /* Insere no listBox o cabeçalho do AFD montado com os registros coletados */
                //Invoke(new AtualizaListBoxColeta(AtualizarListBoxColeta), new object[] { pComunicacao._ID, innerRep.LerCabecalhoRegistrosColetados() });
                /* Insere no listBox o trailer do AFD montado com os registros coletados */
                //Invoke(new AtualizaListBoxColeta(AtualizarListBoxColeta), new object[] { pComunicacao._ID, innerRep.LerTrailerRegistrosColetados() });

            }

            innerRep.FinalizarLeitura();

            _pararColeta = false;
        }

        private void AtualizarListBoxColeta(int REP, string valor)
        {
            if (REP == 1)
            {
                //listboxColeta.Items.Insert(0, valor);
                //listboxColeta.Refresh();
            }
        }

        private void AtualizarNumRegLidosColeta(int REP, string valor)
        {
            if (REP == 1)
            {
                //lblNumeroRegistrosLidos.Text = valor;
            }
        }

        private void AtualizarMaiorDataColeta(int REP, string valor)
        {
            if (REP == 1)
            {

            }
        }

        private void AtualizarNumSerieColeta(int REP, string valor)
        {
            if (REP == 1)
            {

            }
            else
            {

            }
        }

        private void LimparListBoxColeta(int REP)
        {
            if (REP == 1)
            {
                //listboxColeta.Items.Clear();
            }
        }

        private void txbIp_TextChanged(object sender, EventArgs e)
        {

        }

        private void pararColeta_Click(object sender, EventArgs e)
        {
            _pararColeta = true;
            //Close();
        }

        private void frmExemploSdkInnerRep_Load(object sender, EventArgs e)
        {
            /*  Relogios app = new Relogios();
              DataTable dataTable = app.GetRelogios();
              dataGridIp.DataSource = dataTable;
              dataGridIp.Refresh();
              */
        }

        public void atualizarColetas()
        {

        }
    }
}