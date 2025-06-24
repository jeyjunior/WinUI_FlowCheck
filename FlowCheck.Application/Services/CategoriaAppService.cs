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
        public void SalvarCategoria(Categoria categoria)
        {
            if (categoria == null)
                return;

            if (categoria.ValidarResultado == null)
                categoria.ValidarResultado = new ValidarResultado();

            if (categoria.Cor == null || !categoria.Cor.Validar())
                return;

            var config = Bootstrap.ServiceProvider.GetRequiredService<IConfiguracaoBancoDados>();

            using (var uow = new UnitOfWork(config.ConexaoAtiva))
            {
                var categoriaRepository = new CategoriaRepository(uow);
                var corRepository = new CorRepository(uow);

                try
                {
                    uow.Begin();

                    int FK_Cor = 0;

                    if (categoria.Cor.PK_Cor > 0)
                    {
                        corRepository.Atualizar(categoria.Cor);
                        FK_Cor = categoria.Cor.PK_Cor;
                    }
                    else
                    {
                        FK_Cor = corRepository.Adicionar(categoria.Cor);
                    }

                    if (FK_Cor <= 0)
                        throw new Exception("Não foi possível registrar a cor da categoria.");

                    categoria.FK_Cor = FK_Cor;

                    if (categoria.PK_Categoria > 0)
                    {
                        categoriaRepository.Atualizar(categoria);
                    }
                    else
                    {
                        categoria.PK_Categoria = categoriaRepository.Adicionar(categoria);
                    }

                    uow.Commit();
                }
                catch (Exception ex)
                {
                    uow.Rollback();
                    categoria.ValidarResultado.Adicionar("Não foi possível adicionar informações na base.\n " + ex.Message);
                }
            }
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
