using FlowCheck.Domain.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowCheck.Application.Interfaces
{
    public interface IAnotacaoAppService
    {
        IEnumerable<Anotacao> PesquisarPorDescricao(Anotacao_Request request);
        IEnumerable<Anotacao> PesquisarPorCategoria(int PK_Categoria);
        bool SalvarAnotacao(Anotacao anotacao);
        bool RemoverAnotacao(Anotacao anotacao);
        bool RemoverAnotacoes(IEnumerable<Anotacao> anotacoes);
        bool AdicionarAnotacaoEmTarefas(Anotacao anotacao);
    }
}
