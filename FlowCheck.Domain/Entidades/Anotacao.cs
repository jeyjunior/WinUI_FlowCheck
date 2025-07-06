using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JJ.Net.Core.Extensoes;
using JJ.Net.Core.Validador;
using JJ.Net.CrossData_WinUI_3.Atributo;

namespace FlowCheck.Domain.Entidades
{
    [Entidade("Anotacao")]
    public class Anotacao
    {
        [ChavePrimaria, Obrigatorio]
        public int PK_Anotacao { get; set; }
        
        [Obrigatorio]
        public string Descricao { get; set; }
        
        [Obrigatorio]
        public DateTime DataCriacao { get; set; }
        
        [Obrigatorio]
        public bool Ativo { get; set; }

        [Obrigatorio, Relacionamento("Categoria", "PK_Categoria")]
        public int? FK_Categoria { get; set; }

        /* Relacionamento */
        [Editavel(false)]
        public Categoria Categoria { get; set; }

        [Editavel(false)]
        public ValidarResultado ValidarResultado { get; set; } = new ValidarResultado();

        public bool Validar()
        {
            ValidarResultado = new ValidarResultado();

            if (Descricao.ObterValorOuPadrao("").Trim() == "")
            {
                ValidarResultado.Adicionar("Descrição da anotação é obrigatório.");
                return false;
            }
            else if (FK_Categoria <= 0)
            {
                ValidarResultado.Adicionar("Propriedade Categoria é obrigatória.");
                return false;
            }

            return true;
        }
    }

    public class Anotacao_Request
    {
        public string Descricao { get; set; }
        public ValidarResultado ValidarResultado { get; set; } = new ValidarResultado();
    }
}
