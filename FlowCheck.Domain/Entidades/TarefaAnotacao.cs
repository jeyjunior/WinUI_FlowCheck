
using JJ.Net.CrossData_WinUI_3.Atributo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowCheck.Domain.Entidades
{
    [Entidade("TarefaAnotacao")]
    public class TarefaAnotacao
    {
        [ChavePrimaria, Obrigatorio]
        public int PK_TarefaAnotacao { get; set; }
        public string Anotacao { get; set; }
    }
}
