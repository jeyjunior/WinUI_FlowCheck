using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlowCheck.Domain.Entidades;
using FlowCheck.Domain.Interfaces;
using JJ.Net.Data.Interfaces;

namespace FlowCheck.InfraData.Repository
{
    public class CorRepository : Repository<Cor>, ICorRepository
    {
        public CorRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}
