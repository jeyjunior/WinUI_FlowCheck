using FlowCheck.Domain.Entidades;
using FlowCheck.Domain.Interfaces;
using JJ.Net.CrossData_WinUI_3.Enumerador;
using JJ.Net.CrossData_WinUI_3.Interfaces;
using JJ.NET.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowCheck.InfraData.Repository
{
    public class AnotacaoRepository : Repository<Anotacao>, IAnotacaoRepository
    {
        TipoBancoDados IRepository<Anotacao>.Conexao { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public AnotacaoRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

    }
}
