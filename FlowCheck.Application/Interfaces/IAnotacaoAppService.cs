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
        IEnumerable<Anotacao> Pesquisar(Anotacao_Request request);
    }
}
