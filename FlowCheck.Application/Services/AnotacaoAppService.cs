using FlowCheck.Application.Interfaces;
using FlowCheck.Domain.Entidades;
using FlowCheck.Domain.Interfaces;
using FlowCheck.InfraData.Repository;
using JJ.Net.Core.Extensoes;
using JJ.Net.Core.Validador;
using JJ.Net.CrossData_WinUI_3.Interfaces;
using JJ.Net.Data;
using JJ.Net.Data.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowCheck.Application.Services
{
    public class AnotacaoAppService : IAnotacaoAppService, IDisposable
    {
        #region Interfaces
        private readonly IUnitOfWork _uow;
        private readonly IAnotacaoRepository _anotacaoRepository;
        #endregion

        #region Construtor
        public AnotacaoAppService(IUnitOfWork uow, IAnotacaoRepository anotacaoRepository)
        {
            this._uow = uow;
            this._anotacaoRepository = anotacaoRepository;
        }
        #endregion


        #region Metodos
        public void Dispose()
        {
            _uow.Dispose();
        }
        public IEnumerable<Anotacao> Pesquisar(Anotacao_Request request)
        {
            string condicao = "";

            string descricao = request.Descricao.ObterValorOuPadrao("").LimparEntradaSQL().Trim();
            if (descricao != "")
                condicao += "Descricao = @Descricao\n ";

            var parametros = new
            {
                Descricao = descricao
            };

            return _anotacaoRepository.ObterLista(condicao, parametros);
        }
        #endregion
    }
}
