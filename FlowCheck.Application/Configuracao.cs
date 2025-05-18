using Dapper;
using FlowCheck.Domain.Interfaces;
using JJ.NET.CrossData;
using JJ.NET.CrossData.Atributo;
using JJ.NET.CrossData.Enumerador;
using JJ.NET.CrossData.Extensao;
using JJ.NET.Data.Interfaces;
using Microsoft.Data.SqlClient;
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
    internal static class Configuracao
    {
        public static Conexao ConexaoAtiva { get; private set; } = Conexao.SQLite;
        public static IDbConnection ConexaoBaseDados => ObterConexao();
        public static void Iniciar()
        {
            ConfiguracaoBancoDados.IniciarConfiguracao(ConexaoAtiva, ApplicationData.Current.LocalFolder.Path, "flowcheck");
        }
        private static IDbConnection ObterConexao()
        {
            return ConfiguracaoBancoDados.ObterConexao();
        }
        public static void RegistrarEntidades()
        {
            var uow = Bootstrap.Container.GetInstance<IUnitOfWork>();

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
        public static void RegistrarParametros()
        {
            var uow = Bootstrap.Container.GetInstance<IUnitOfWork>();
            var parametroRepository = Bootstrap.Container.GetInstance<IParametroRepository>();

            try
            {
                if (parametroRepository.ObterLista(" Parametro.Nome = 'TituloTarefa' ").FirstOrDefault() == null)
                {
                    uow.Begin();

                    parametroRepository.Adicionar(new Domain.Entidades.Parametro { Nome = "TituloTarefa", Valor = "Defina um título para as tarefas." });

                    uow.Commit();
                }
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
        private static IEnumerable<Type> ObterEntidadesMapeadas()
        {
            // Carrega todos os assemblies, inclusive os que ainda não foram tocados
            var assemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();

            // Aqui você pode carregar o assembly se não estiver na lista
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
