using System;
using System.IO;
using Windows.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using JJ.Net.CrossData_WinUI_3.Extensao;
using JJ.Net.CrossData_WinUI_3.Enumerador;
using JJ.Net.CrossData_WinUI_3.Interfaces;
using JJ.Net.Data.Interfaces;
using JJ.Net.Data;
using FlowCheck.Domain.Interfaces;
using FlowCheck.InfraData.Repository;
using FlowCheck.Application.Interfaces;
using FlowCheck.Application.Services;

namespace FlowCheck.Application
{
    public static class Bootstrap
    {
        public static IServiceProvider ServiceProvider { get; private set; }

        public static void Iniciar()
        {
            try
            {
                var host = Host.CreateDefaultBuilder()
                    .ConfigureServices((context, services) =>
                    {
                        services.AddSingletonConfiguracao(config =>
                        {
                            config.TipoBanco = TipoBancoDados.SQLite;
                            config.NomeAplicacao = "FlowCheck";
                        });

                        RegistrarServicos(services);
                    })
                    .Build();

                ServiceProvider = host.Services;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro na inicialização: {ex}");
            }
        }

        private static void RegistrarServicos(IServiceCollection services)
        {
            services.AddSingleton<IUnitOfWork>(provider =>
            {
                var config = provider.GetRequiredService<IConfiguracaoBancoDados>();
                return new UnitOfWork(config.ConexaoAtiva);
            });

            services.AddSingleton<IAnotacaoRepository, AnotacaoRepository>();
            services.AddSingleton<ICategoriaRepository, CategoriaRepository>();
            services.AddSingleton<ICorRepository, CorRepository>();
            services.AddSingleton<IParametroRepository, ParametroRepository>();
            services.AddSingleton<ITarefaAnotacaoRepository, TarefaAnotacaoRepository>();
            services.AddSingleton<ITarefaRepository, TarefaRepository>();

            services.AddSingleton<ITarefaAppService, TarefaAppService>();
        }
    }
}