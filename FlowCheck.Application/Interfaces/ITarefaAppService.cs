using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using FlowCheck.Domain.Entidades;

namespace FlowCheck.Application.Interfaces
{
    public interface ITarefaAppService
    {
        void SalvarTarefas(Tarefa_AppServiceRequest request);
        bool RemoverTarefa(Tarefa tarefa);
        bool RemoverTarefas(IEnumerable<Tarefa> tarefas);
        IEnumerable<Tarefa> Pesquisar(Tarefa_Request request);
    }
}
