using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowCheck.Domain.Interfaces
{
    public interface IPageItensComandos
    {
        void SelecionarTudo(bool selecionar);
        void ExcluirItensSelecionados();
        bool ExisteItensSelecionados();
    }
}
