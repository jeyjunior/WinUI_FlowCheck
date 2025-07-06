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
    [Entidade("Categoria")]
    public class Categoria
    {
        [ChavePrimaria, Obrigatorio]
        public int PK_Categoria { get; set; }

        [Obrigatorio]
        public string Nome { get; set; }

        [Relacionamento("Cor", "PK_Cor")]
        public int? FK_Cor { get; set; }

        /* Relacionamento */
        [Editavel(false)]
        public Cor Cor { get; set; }

        [Editavel(false)]
        public ValidarResultado ValidarResultado { get; set; } = new ValidarResultado();

        public bool Validar()
        {
            ValidarResultado = new ValidarResultado();

            if (Nome.ObterValorOuPadrao("").Trim() == "")
            {
                ValidarResultado.Adicionar("Nome da categoria é obrigatório.");
                return false;
            }

            return true;
        }
    }

    public class Categoria_Request
    {
        public string Nome { get; set; }
        public bool PesquisaPorIgualdade { get; set; }
        public ValidarResultado ValidarResultado { get; set; } = new ValidarResultado();
    }
}
