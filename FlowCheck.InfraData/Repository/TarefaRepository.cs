using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using FlowCheck.Domain.Entidades;
using FlowCheck.Domain.Interfaces;
using JJ.Net.Core.Extensoes;
using JJ.Net.Data.Interfaces;

namespace FlowCheck.InfraData.Repository
{
    public class TarefaRepository : Repository<Tarefa>, ITarefaRepository
    {
        private string QUERY = "";
        private string ORDERBY = "";

        public TarefaRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            QUERY = " SELECT  Tarefa.*, \n" +
                    "         TarefaAnotacao.* \n" +
                    " FROM    Tarefa \n" +
                    " LEFT    JOIN  TarefaAnotacao \n" +
                    "         ON    TarefaAnotacao.PK_TarefaAnotacao = Tarefa.FK_TarefaAnotacao \n";

            ORDERBY = " ORDER   BY \n" +
                      "         Tarefa.IndiceExibicao \n";
        }

        public override IEnumerable<Tarefa> ObterLista(string condition = "", object parameters = null)
        {
            string query = QUERY;

            if (condition.ObterValorOuPadrao("").Trim() != "")
                query += " WHERE " + condition + "\n";

            query += ORDERBY;

            var resultado = unitOfWork.Connection.Query<Tarefa, TarefaAnotacao, Tarefa>(
                sql: query.ToSQL(),
                map: (tarefa, tarefaAnotacao) =>
                {
                    tarefa.TarefaAnotacao = tarefaAnotacao;
                    return tarefa;
                },
                param: parameters,
                splitOn: "PK_Tarefa, PK_TarefaAnotacao")
                .ToList();

            return resultado;
        }
    }
}
