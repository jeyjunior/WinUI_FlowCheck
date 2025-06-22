using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using JJ.Net.Core.Extensoes;
using JJ.Net.Data.Interfaces;
using FlowCheck.Domain.Entidades;
using FlowCheck.Domain.Interfaces;


namespace FlowCheck.InfraData.Repository
{
    public class CategoriaRepository : Repository<Categoria>, ICategoriaRepository
    {
        private string QUERY = "";
        private string ORDERBY = "";

        public CategoriaRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            QUERY = " SELECT  Categoria.*, \n" +
                    "         Cor.* \n" +
                    " FROM    Categoria \n" +
                    " INNER   JOIN  Cor \n" +
                    "         ON    Cor.PK_Cor = Categoria.FK_Cor \n";

            ORDERBY = " ORDER   BY \n" +
                      "         Categoria.Nome \n";
        }
        public override IEnumerable<Categoria> ObterLista(string condition = "", object parameters = null)
        {
            string query = QUERY;

            if (condition.ObterValorOuPadrao("").Trim() != "")
                query += " WHERE " + condition + "\n";

            query += ORDERBY;

            var resultado = unitOfWork.Connection.Query<Categoria, Cor, Categoria>(
                sql: query.ToSQL(),
                map: (categoria, cor) =>
                {
                    categoria.Cor = cor;
                    return categoria;
                },
                param: parameters,
                splitOn: "PK_Categoria, PK_Cor")
                .ToList();

            return resultado;
        }
    }
}
