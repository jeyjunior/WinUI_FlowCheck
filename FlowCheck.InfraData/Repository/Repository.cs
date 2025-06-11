using JJ.Net.CrossData_WinUI_3.Atributo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JJ.Net.CrossData_WinUI_3.Enumerador;
using JJ.Net.CrossData_WinUI_3.Interfaces;
using JJ.Net.CrossData_WinUI_3.Extensao;
using JJ.Net.Data.Interfaces;

namespace FlowCheck.InfraData.Repository
{
    public abstract class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        public TipoBancoDados Conexao { get; set; } = TipoBancoDados.SQLite;

        protected IUnitOfWork unitOfWork = null;

        public Repository(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public virtual int Adicionar(TEntity entity)
        {
            return unitOfWork.Connection.Adicionar(entity, unitOfWork.Transaction);
        }

        public virtual int Atualizar(TEntity entity)
        {
            return unitOfWork.Connection.Atualizar(entity, unitOfWork.Transaction);
        }
        public bool CriarTabela(string query)
        {
            return unitOfWork.Connection.CriarTabelas(query, unitOfWork.Transaction);
        }

        public virtual int Deletar(object id)
        {
            return unitOfWork.Connection.Deletar<TEntity>(id, unitOfWork.Transaction);
        }

        public virtual void Dispose()
        {
            unitOfWork.Dispose();
            GC.SuppressFinalize(this);
        }

        public virtual int ExecutarQuery(string query)
        {
            return unitOfWork.Connection.ExecutarQuery(query, unitOfWork.Transaction);
        }

        public virtual TEntity Obter(int id)
        {
            return unitOfWork.Connection.Obter<TEntity>(id);
        }

        public virtual IEnumerable<TEntity> ObterLista(string condition = "", object parameters = null)
        {
            return unitOfWork.Connection.ObterLista<TEntity>(condition, parameters);
        }
    }
}
