using JJ.Net.CrossData_WinUI_3.Atributo;
using JJ.NET.Core.Extensoes;
using JJ.NET.Core.Validador;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowCheck.Domain.Entidades
{
    [Entidade("Tarefa")]
    public class Tarefa
    {
        [ChavePrimaria, Obrigatorio]
        public int PK_Tarefa { get; set; }
        [Obrigatorio]
        public string Descricao { get; set; }
        [Obrigatorio]
        public bool Concluido { get; set; }
        [Obrigatorio]
        public int IndiceExibicao { get; set; }

        [Relacionamento("TarefaAnotacao", "PK_TarefaAnotacao")]
        public int? FK_TarefaAnotacao { get; set; }

        [Editavel(false)]
        public ValidarResultado ValidarResultado { get; set; } = new ValidarResultado();

        public bool Validar()
        {
            ValidarResultado = new ValidarResultado();

            if (Descricao.ObterValorOuPadrao("").Trim() == "")
            {
                ValidarResultado.Adicionar("Descrição da tarefa é obrigatório.");
                return false;
            }

            return true;
        }
    }
}
