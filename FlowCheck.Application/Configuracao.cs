using Dapper;
using FlowCheck.Domain.Entidades;
using FlowCheck.Domain.Interfaces;
using FlowCheck.InfraData.Repository;
using JJ.Net.CrossData_WinUI_3.Atributo;
using JJ.Net.CrossData_WinUI_3.CrossData;
using JJ.Net.CrossData_WinUI_3.Enumerador;
using JJ.Net.CrossData_WinUI_3.Extensao;
using JJ.Net.CrossData_WinUI_3.Interfaces;
using JJ.NET.Data;
using JJ.NET.Data.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace FlowCheck.Application
{
    public static class Configuracao
    {
        public static void Iniciar()
        {
            RegistrarEntidades();
            RegistrarParametros();
        }
        public static void RegistrarEntidades()
        {
            var config = Bootstrap.ServiceProvider.GetRequiredService<IConfiguracaoBancoDados>();

            using (var uow = new UnitOfWork(config.ConexaoAtiva))
            {
                var entidades = ObterEntidadesMapeadas();
                var tabelasExistentes = uow.Connection.VerificarEntidadeExiste(entidades);

                if (tabelasExistentes.Any(i => !i.Existe))
                {
                    try
                    {
                        uow.Begin();

                        foreach (var entidade in tabelasExistentes.Where(e => !e.Existe))
                            uow.Connection.CriarTabela(entidade.TipoEntidade, uow.Transaction);

                        uow.Commit();
                    }
                    catch (SqlException ex)
                    {
                        uow.Rollback();
                        throw new Exception("Erro ao criar as entidades no banco de dados", ex);
                    }
                    catch (Exception ex)
                    {
                        uow.Rollback();
                        throw new Exception("Erro inesperado ao criar as entidades", ex);
                    }
                }
            }
        }
        public static void RegistrarParametros()
        {
            var config = Bootstrap.ServiceProvider.GetRequiredService<IConfiguracaoBancoDados>();

            using (var uow = new UnitOfWork(config.ConexaoAtiva))
            {
                var parametroRepository = new ParametroRepository(uow);

                //try
                //{
                //    string parametro = eParametro.TituloTarefa.ToString();

                //    if (parametroRepository.ObterLista($" Parametro.Nome = '{parametro}' ").FirstOrDefault() == null)
                //    {
                //        uow.Begin();

                //        //parametroRepository.Adicionar(new Domain.Entidades.Parametro { Nome = eParametro.TituloTarefa.ToString(), Valor = "Tarefas" });
                //        //parametroRepository.Adicionar(new Domain.Entidades.Parametro { Nome = eParametro.TituloTarefa.ToString(), Valor = "Anotações" });

                //        uow.Commit();
                //    }
                //}
                //catch (SqlException ex)
                //{
                //    uow.Rollback();
                //    throw new Exception("Erro ao criar as entidades no banco de dados", ex);
                //}
                //catch (Exception ex)
                //{
                //    uow.Rollback();
                //    throw new Exception("Erro inesperado ao criar as entidades", ex);
                //}
            }
        }
        private static IEnumerable<Type> ObterEntidadesMapeadas()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();

            if (!assemblies.Any(a => a.FullName.Contains("FlowCheck.Domain")))
            {
                var domainAssembly = Assembly.Load("FlowCheck.Domain");
                assemblies.Add(domainAssembly);
            }

            return assemblies
                .SelectMany(a => a.GetTypes())
                .Where(t => t.GetCustomAttribute<EntidadeAttribute>() != null && t.IsClass && !t.IsAbstract);
        }
    }
}
