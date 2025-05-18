using JJ.NET.Core.Extensoes;
using JJ.NET.Core.Validador;
using JJ.NET.CrossData.Atributo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowCheck.Domain.Entidades
{
    [Entidade("Parametro")]
    public class Parametro
    {
        [ChavePrimaria, Obrigatorio]
        public int PK_Parametro { get; set; }

        [Obrigatorio]
        public string Nome { get; set; }

        public string Valor { get; set; }

        [Editavel(false)]
        public ValidarResultado ValidarResultado { get; set; } = new ValidarResultado();

        public bool Validar()
        {
            ValidarResultado = new ValidarResultado();

            if (Nome.ObterValorOuPadrao("").Trim() == "")
            {
                ValidarResultado.Adicionar("Nome do parâmetro é obrigatório.");
                return false;
            }

            return true;
        }
    }
}
