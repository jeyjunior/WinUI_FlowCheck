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
    public class CategoriaAppService : ICategoriaAppService, IDisposable
    {
        #region Interfaces
        private readonly IUnitOfWork _uow;
        private readonly ICategoriaRepository _categoriaRepository;
        #endregion

        #region Construtor
        public CategoriaAppService(IUnitOfWork uow, ICategoriaRepository categoriaRepository)
        {
            this._uow = uow;
            this._categoriaRepository = categoriaRepository;
        }
        #endregion

        #region Metodos
        public void Dispose()
        {
            _uow.Dispose();
            _categoriaRepository.Dispose();
        }
        public IEnumerable<Categoria> Pesquisar(Categoria_Request request)
        {
            string condicao = "";

            if (request.Nome.ObterValorOuPadrao("").Trim() != "")
                condicao += "Nome = @Nome\n ";

            var parametros = new
            {
                Nome = request.Nome
            };

            return _categoriaRepository.ObterLista(condicao, parametros);
        }
        #endregion
    }
}
