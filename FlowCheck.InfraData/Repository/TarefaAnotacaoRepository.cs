using FlowCheck.Domain.Entidades;
using FlowCheck.Domain.Interfaces;
using JJ.NET.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowCheck.InfraData.Repository
{
    public class TarefaAnotacaoRepository : Repository<TarefaAnotacao>, ITarefaAnotacaoRepository
    {
        public TarefaAnotacaoRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}
