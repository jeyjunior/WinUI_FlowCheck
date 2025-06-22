using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using JJ.Net.Core.Extensoes;
using JJ.Net.Core.Validador;
using JJ.Net.CrossData_WinUI_3.Interfaces;
using JJ.Net.Data;
using JJ.Net.Data.Interfaces;
using FlowCheck.Application.Interfaces;
using FlowCheck.Domain.Entidades;
using FlowCheck.Domain.Interfaces;
using FlowCheck.InfraData.Repository;

namespace FlowCheck.Application.Services
{
    public class ParametroAppService : IParametroAppService
    {
        #region Interfaces
        private readonly IUnitOfWork _uow;
        private readonly IParametroRepository _parametroRepository;
        #endregion

        #region Metodos
        public void Dispose()
        {
            _uow.Dispose();
            _parametroRepository.Dispose();
        }
        public ParametroAppService(IUnitOfWork uow, IParametroRepository parametroRepository)
        {
            this._uow = uow;
            this._parametroRepository = parametroRepository;
        }
        public void SalvarParametros(Parametro_AppServiceRequest request)
        {
            if (request == null)
                return;

            if (request.ValidarResultado == null)
                request.ValidarResultado = new ValidarResultado();

            if (request.Parametros == null || request.Parametros.Count <= 0)
                return;

            var config = Bootstrap.ServiceProvider.GetRequiredService<IConfiguracaoBancoDados>();

            using (var uow = new UnitOfWork(config.ConexaoAtiva))
            {
                var parametroRepository = new ParametroRepository(uow);

                foreach (var parametro in request.Parametros)
                {
                    try
                    {
                        uow.Begin();

                        if (parametro.PK_Parametro > 0)
                        {
                            parametroRepository.Atualizar(parametro);
                        }
                        else
                        {
                            parametro.PK_Parametro = parametroRepository.Adicionar(parametro);
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
        public IEnumerable<Parametro> Pesquisar(Parametro_Request request)
        {
            string condicao = "";

            if (request.Nome.ObterValorOuPadrao("").Trim() != "")
                condicao += "Nome = @Nome\n";

            var parametros = new
            {
                Nome = request.Nome
            };

            return _parametroRepository.ObterLista(condicao, parametros);
        }
        #endregion
    }
}
