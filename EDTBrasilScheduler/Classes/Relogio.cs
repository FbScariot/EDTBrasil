using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiSap.Classes
{
    class Relogio
    {
        public string CPF { get; set; }
        public string Usuario { get; set; }
        public string Senha { get; set; }
        public string CodigoAcesso { get; set; }
        public string UltimaSnr { get; set; }
        public string Endereco { get; set; }
        public string Porta { get; set; }


        public Relogio(string cpf, string usuario, string senha, string codigoAcesso, string ulmasnr, string endereco, string porta)
        {
            this.CPF = cpf;
            this.Usuario = usuario;
            this.Senha = senha;
            this.CodigoAcesso = codigoAcesso;
            this.UltimaSnr = ulmasnr;
            this.Endereco = endereco;
            this.Porta = porta;

        }

    }
}
