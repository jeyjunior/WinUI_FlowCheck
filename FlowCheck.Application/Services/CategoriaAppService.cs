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
        public bool SalvarCategoria(Categoria categoria)
        {
            if (categoria == null)
                return false;

            if (categoria.ValidarResultado == null)
                categoria.ValidarResultado = new ValidarResultado();

            var config = Bootstrap.ServiceProvider.GetRequiredService<IConfiguracaoBancoDados>();

            using (var uow = new UnitOfWork(config.ConexaoAtiva))
            {
                var categoriaRepository = new CategoriaRepository(uow);
                var corRepository = new CorRepository(uow);

                var categoriaExistente = categoriaRepository.ObterLista("Categoria.Nome = @Nome", new { Nome = categoria.Nome }).FirstOrDefault();

                if (categoriaExistente != null)
                {
                    categoria.ValidarResultado.Adicionar("Esse nome de categoria já existe.");
                    return false;
                }

                if (categoria.Cor == null)
                    categoria.Cor = corRepository.GerarCorAleatoria();

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
                    {
                        categoria.ValidarResultado.Adicionar("Não foi possível registrar a cor da categoria.");
                        return false;
                    }

                    categoria.FK_Cor = FK_Cor;
                    categoria.Cor.PK_Cor = FK_Cor;

                    if (categoria.PK_Categoria > 0)
                    {
                        categoriaRepository.Atualizar(categoria);
                    }
                    else
                    {
                        categoria.PK_Categoria = categoriaRepository.Adicionar(categoria);
                    }

                    uow.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    uow.Rollback();
                    
                }
            }
            
            return false;
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
