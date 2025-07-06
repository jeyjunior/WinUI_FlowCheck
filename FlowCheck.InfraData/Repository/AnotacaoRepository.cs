using Dapper;
using FlowCheck.Domain.Entidades;
using FlowCheck.Domain.Interfaces;
using JJ.Net.Core.Extensoes;
using JJ.Net.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowCheck.InfraData.Repository
{
    public class AnotacaoRepository : Repository<Anotacao>, IAnotacaoRepository
    {
        private string QUERY = "";
        private string ORDERBY = "";

        public AnotacaoRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            QUERY = " SELECT  Anotacao.*, \n" +
                    "         Categoria.*, \n" +
                    "         Cor.* \n" +
                    " FROM    Anotacao \n" +
                    " LEFT    JOIN  Categoria \n" +
                    "         ON    Categoria.PK_Categoria = Anotacao.FK_Categoria \n" +
                    " LEFT    JOIN  Cor \n" +
                    "         ON    Cor.PK_Cor = Categoria.FK_Cor \n";

            ORDERBY = " ORDER   BY \n" +
                      "         Anotacao.DataCriacao \n";
        }

        public override IEnumerable<Anotacao> ObterLista(string condition = "", object parameters = null)
        {
            string query = QUERY;

            if (condition.ObterValorOuPadrao("").Trim() != "")
                query += " WHERE " + condition + "\n";

            query += ORDERBY;

            var resultado = unitOfWork.Connection.Query<Anotacao, Categoria, Cor, Anotacao>(
                sql: query.ToSQL(),
                map: (anotacao, categoria, cor) =>
                {
                    anotacao.Categoria = categoria;

                    if (anotacao.Categoria != null)
                        anotacao.Categoria.Cor = cor;

                    return anotacao;
                },
                param: parameters,
                splitOn: "PK_Anotacao, PK_Categoria, PK_Cor")
                .ToList();

            return resultado;
        }
    }
}
