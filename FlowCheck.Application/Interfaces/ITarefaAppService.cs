using FlowCheck.Domain.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowCheck.Application.Interfaces
{
    public interface ITarefaAppService
    {
        void SalvarTarefas(Tarefa_AppServiceRequest request);
        bool RemoverTarefa(Tarefa tarefa);
        bool RemoverTarefas(IEnumerable<Tarefa> tarefas);
    }
}
