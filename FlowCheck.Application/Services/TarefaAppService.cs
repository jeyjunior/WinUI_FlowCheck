using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlowCheck.Application.Interfaces;
using FlowCheck.Domain.Entidades;
using FlowCheck.Domain.Interfaces;
using FlowCheck.InfraData.Repository;
using Microsoft.Extensions.DependencyInjection;
using JJ.Net.Core.Validador;
using JJ.Net.CrossData_WinUI_3.Interfaces;
using JJ.Net.Data;
using JJ.Net.Data.Interfaces;

namespace FlowCheck.Application.Services
{
    public class TarefaAppService : ITarefaAppService, IDisposable
    {
        #region Interfaces
        private readonly IUnitOfWork _uow;
        private readonly ITarefaRepository _tarefaRepository;
        private readonly ITarefaAnotacaoRepository _tarefaAnotacaoRepository;
        #endregion

        #region Construtor
        public TarefaAppService(IUnitOfWork uow, ITarefaRepository tarefaRepository, ITarefaAnotacaoRepository tarefaAnotacaoRepository)
        {
            this._uow = uow;
            this._tarefaRepository = tarefaRepository;
            this._tarefaAnotacaoRepository = tarefaAnotacaoRepository;
        }
        #endregion

        #region Metodos
        public void SalvarTarefas(Tarefa_AppServiceRequest request)
        {
            if (request == null)
                return;

            if (request.ValidarResultado == null)
                request.ValidarResultado = new ValidarResultado();

            if (request.Tarefas == null || request.Tarefas.Count <= 0)
                return;

            var config = Bootstrap.ServiceProvider.GetRequiredService<IConfiguracaoBancoDados>();

            using (var uow = new UnitOfWork(config.ConexaoAtiva))
            {
                var tarefaRepository = new TarefaRepository(uow);
                var tarefaAnotacaoRepository = new TarefaAnotacaoRepository(uow);

                foreach (var tarefa in request.Tarefas)
                {
                    try
                    {
                        uow.Begin();

                        int FK_TarefaAnotacao = 0;

                        if (tarefa.TarefaAnotacao != null)
                        {
                            if (tarefa.TarefaAnotacao.PK_TarefaAnotacao > 0)
                            {
                                tarefaAnotacaoRepository.Atualizar(tarefa.TarefaAnotacao);
                                FK_TarefaAnotacao = tarefa.TarefaAnotacao.PK_TarefaAnotacao;
                            }
                            else
                            {
                                FK_TarefaAnotacao = tarefaAnotacaoRepository.Adicionar(tarefa.TarefaAnotacao);
                            }
                        }

                        tarefa.FK_TarefaAnotacao = ((FK_TarefaAnotacao > 0) ? FK_TarefaAnotacao : null);

                        if (tarefa.PK_Tarefa > 0)
                        {
                            tarefaRepository.Atualizar(tarefa);
                        }
                        else
                        {
                            tarefa.PK_Tarefa = tarefaRepository.Adicionar(tarefa);
                        }

                        uow.Commit();
                    }
                    catch (Exception ex)
                    {
                        uow.Rollback();
                        request.ValidarResultado.Adicionar("Não foi possível adicionar informações na base.\n " + ex.Message);
                    }
                }
            }
        }
        public bool RemoverTarefa(Tarefa tarefa)
        {
            if (tarefa == null)
                return false;

            if (tarefa.PK_Tarefa <= 0)
                return true;

            if (tarefa.ValidarResultado == null)
                tarefa.ValidarResultado = new ValidarResultado();

            var config = Bootstrap.ServiceProvider.GetRequiredService<IConfiguracaoBancoDados>();

            using (var uow = new UnitOfWork(config.ConexaoAtiva))
            {
                var tarefaRepository = new TarefaRepository(uow);
                var tarefaAnotacaoRepository = new TarefaAnotacaoRepository(uow);

                try
                {
                    uow.Begin();

                    if (tarefa.TarefaAnotacao != null)
                        tarefaAnotacaoRepository.Deletar(tarefa.TarefaAnotacao.PK_TarefaAnotacao);

                    tarefaRepository.Deletar(tarefa.PK_Tarefa);

                    uow.Commit();

                    return true;
                }
                catch (Exception ex)
                {
                    uow.Rollback();
                    tarefa.ValidarResultado.Adicionar("Não foi possível deletar informações na base.\n " + ex.Message);
                }
            }

            return false;
        }
        public void Dispose()
        {
            _uow.Dispose();
            _tarefaRepository.Dispose();
            _tarefaAnotacaoRepository.Dispose();
        }
        #endregion
    }
}
