﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using FlowCheck.Application.Interfaces;
using FlowCheck.Domain.Entidades;
using FlowCheck.Domain.Interfaces;
using FlowCheck.InfraData.Repository;
using JJ.Net.Core.Extensoes;
using JJ.Net.Core.Validador;
using JJ.Net.CrossData_WinUI_3.Interfaces;
using JJ.Net.Data;
using JJ.Net.Data.Interfaces;

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
        public bool SalvarAnotacao(Anotacao anotacao)
        {
            if (anotacao == null)
                return false;

            if (anotacao.ValidarResultado == null)
                anotacao.ValidarResultado = new ValidarResultado();

            var config = Bootstrap.ServiceProvider.GetRequiredService<IConfiguracaoBancoDados>();

            using (var uow = new UnitOfWork(config.ConexaoAtiva))
            {
                var anotacaoRepository = new AnotacaoRepository(uow);

                try
                {
                    uow.Begin();

                    if (anotacao.PK_Anotacao > 0)
                    {
                        anotacaoRepository.Atualizar(anotacao);
                    }
                    else
                    {
                        anotacao.PK_Anotacao = anotacaoRepository.Adicionar(anotacao);
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
        public bool RemoverAnotacao(Anotacao anotacao)
        {
            if (anotacao == null)
                return false;

            if (anotacao.PK_Anotacao <= 0)
                return true;

            return RemoverAnotacoes(new List<Anotacao>() { anotacao });
        }
        public bool RemoverAnotacoes(IEnumerable<Anotacao> anotacoes)
        {
            if (anotacoes == null)
                return false;

            if (anotacoes.Count() <= 0)
                return true;

            var config = Bootstrap.ServiceProvider.GetRequiredService<IConfiguracaoBancoDados>();

            using (var uow = new UnitOfWork(config.ConexaoAtiva))
            {
                var anotacaoRepository = new AnotacaoRepository(uow);

                try
                {
                    uow.Begin();

                    foreach (var anotacao in anotacoes)
                    {
                        if (anotacao.PK_Anotacao <= 0)
                            continue;

                        anotacaoRepository.Deletar(anotacao.PK_Anotacao);
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
        public IEnumerable<Anotacao> PesquisarPorDescricao(Anotacao_Request request)
        {
            string condicao = "";
            string descricao = "";

            if (request.Descricao.ObterValorOuPadrao("").Trim() != "")
                descricao = request.Descricao.LimparEntradaSQL();

            if (request.TipoPesquisa == Domain.Enumerador.eTipoPesquisaAnotacao.Tudo)
            {
                condicao = $"Anotacao.Descricao LIKE '%{descricao}%'\n" +
                      $"OR       Categoria.Nome LIKE '%{descricao}%'\n";
            }
            else if (request.TipoPesquisa == Domain.Enumerador.eTipoPesquisaAnotacao.Categoria)
            {
                condicao = $"Categoria.Nome LIKE '%{descricao}%'\n";
            }
            else if (request.TipoPesquisa == Domain.Enumerador.eTipoPesquisaAnotacao.Anotacao)
            {
                condicao = $"Anotacao.Descricao LIKE '%{descricao}%'\n";
            }

            if (request.FK_Categoria == 0)
            {
                condicao = ((condicao.Length > 0) ? $"({condicao})" : condicao) +
                    "\nAND  (Anotacao.FK_Categoria IS NULL OR Anotacao.FK_Categoria = 0)\n";
            }
            else if (request.FK_Categoria > 0)
            {
                condicao = ((condicao.Length > 0) ? $"({condicao})" : condicao) +
                            "\nAND  Anotacao.FK_Categoria = @FK_Categoria\n";
            }

            var parametros = new
            {
                Descricao = descricao,
                FK_Categoria = request.FK_Categoria
            };

            return _anotacaoRepository.ObterLista(condicao, parametros);
        }
        public IEnumerable<Anotacao> PesquisarPorCategoria(int PK_Categoria)
        {
            string condicao = "";

            if (PK_Categoria == 0)
            {
                condicao = $"Anotacao.FK_Categoria IS NULL OR Anotacao.FK_Categoria = 0\n";
            }
            else if (PK_Categoria > 0)
            {
                condicao = $"Anotacao.FK_Categoria = @FK_Categoria\n";
            }

            var parametros = new
            {
                FK_Categoria = PK_Categoria
            };

            return _anotacaoRepository.ObterLista(condicao, parametros);
        }
        public bool AdicionarAnotacaoEmTarefas(Anotacao anotacao)
        {
            if (anotacao == null)
                return false;

            if (anotacao.ValidarResultado == null)
                anotacao.ValidarResultado = new ValidarResultado();

            var config = Bootstrap.ServiceProvider.GetRequiredService<IConfiguracaoBancoDados>();

            using (var uow = new UnitOfWork(config.ConexaoAtiva))
            {
                var tarefaRepository = new TarefaRepository(uow);

                try
                {
                    int ultimoIndice = tarefaRepository.ObterUltimoIndiceExibicao();

                    var tarefa = new Tarefa
                    {
                        Arquivado = false,
                        Concluido = false,
                        Descricao = anotacao.Descricao.Trim(),
                        IndiceExibicao = ultimoIndice,
                        PK_Tarefa = 0
                    };

                    uow.Begin();

                    tarefaRepository.Adicionar(tarefa);

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
        #endregion
    }
}
