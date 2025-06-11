using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlowCheck.Domain.Entidades;
using FlowCheck.Domain.Interfaces;
using JJ.Net.CrossData_WinUI_3.Enumerador;
using JJ.Net.CrossData_WinUI_3.Interfaces;
using JJ.Net.Data.Interfaces;

namespace FlowCheck.InfraData.Repository
{
    public class AnotacaoRepository : Repository<Anotacao>, IAnotacaoRepository
    {
        public AnotacaoRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}
