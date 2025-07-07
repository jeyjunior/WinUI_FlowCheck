using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage;
using Dapper;
using FlowCheck.Domain.Entidades;
using FlowCheck.Domain.Enumerador;
using FlowCheck.Domain.Interfaces;
using FlowCheck.InfraData.Repository;
using JJ.Net.CrossData_WinUI_3.Atributo;
using JJ.Net.CrossData_WinUI_3.CrossData;
using JJ.Net.CrossData_WinUI_3.Enumerador;
using JJ.Net.CrossData_WinUI_3.Extensao;
using JJ.Net.CrossData_WinUI_3.Interfaces;
using JJ.Net.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;

namespace FlowCheck.Application
{
    public static class Configuracao
    {
        public static void Iniciar()
        {
            RegistrarEntidades();
            RegistrarParametros();
            RegistrarCoresIniciais();
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

                try 
                {
                    string parametro =  eParametro.TituloTarefa.ToString();

                    if (parametroRepository.ObterLista($" Parametro.Nome = '{parametro}' ").FirstOrDefault() == null)
                    {
                        uow.Begin();

                        parametroRepository.Adicionar(new Domain.Entidades.Parametro { Nome = eParametro.TituloTarefa.ToString(), Valor = "Tarefas" });

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
        }

        public static void RegistrarCoresIniciais()
        {
            var config = Bootstrap.ServiceProvider.GetRequiredService<IConfiguracaoBancoDados>();

            using (var uow = new UnitOfWork(config.ConexaoAtiva))
            {
                var corRepository = new CorRepository(uow);

                try
                {
                    if (corRepository.ObterLista("Cor.Nome = 'Vermelho 1'").FirstOrDefault() == null)
                    {
                        uow.Begin();

                        corRepository.Adicionar(new Cor { Nome = "Preto", RGB = "0,0,0", Hexadecimal = "#FF000000" });
                        corRepository.Adicionar(new Cor { Nome = "Branco", RGB = "255,255,255", Hexadecimal = "#FFFFFFFF" });

                        #region Vermelho1
                        corRepository.Adicionar(new Cor { Nome = "Vermelho 1", RGB = "251,72,72", Hexadecimal = "#FFFB4848" });
                        corRepository.Adicionar(new Cor { Nome = "Vermelho 2", RGB = "255,54,54", Hexadecimal = "#FFFF3636" });
                        corRepository.Adicionar(new Cor { Nome = "Vermelho 3", RGB = "223,50,50", Hexadecimal = "#FFDF3232" });
                        corRepository.Adicionar(new Cor { Nome = "Vermelho 4", RGB = "215,26,26", Hexadecimal = "#FFD71A1A" });
                        corRepository.Adicionar(new Cor { Nome = "Vermelho 5", RGB = "190,10,10", Hexadecimal = "#FFBE0A0A" });
                        corRepository.Adicionar(new Cor { Nome = "Vermelho 6", RGB = "150,0,0", Hexadecimal = "#FF9F0000" });
                        #endregion

                        #region AzulSuave1
                        corRepository.Adicionar(new Cor { Nome = "Azul Suave 1", RGB = "90,180,230", Hexadecimal = "#FF5AB4E6" });
                        corRepository.Adicionar(new Cor { Nome = "Azul Suave 2", RGB = "57,155,210", Hexadecimal = "#FF399BD2" });
                        corRepository.Adicionar(new Cor { Nome = "Azul Suave 3", RGB = "33,131,186", Hexadecimal = "#FF2183BA" });
                        corRepository.Adicionar(new Cor { Nome = "Azul Suave 4", RGB = "6,101,154", Hexadecimal = "#FF06659A" });
                        corRepository.Adicionar(new Cor { Nome = "Azul Suave 5", RGB = "0,69,108", Hexadecimal = "#FF00456C" });
                        corRepository.Adicionar(new Cor { Nome = "Azul Suave 6", RGB = "0,50,78", Hexadecimal = "#FF00324E" });
                        #endregion

                        #region Azul
                        corRepository.Adicionar(new Cor { Nome = "Azul 1", RGB = "0,132,255", Hexadecimal = "#FF0084FF" });
                        corRepository.Adicionar(new Cor { Nome = "Azul 2", RGB = "0,114,221", Hexadecimal = "#FF0072DD" });
                        corRepository.Adicionar(new Cor { Nome = "Azul 3", RGB = "0,98,189", Hexadecimal = "#FF0062BD" });
                        corRepository.Adicionar(new Cor { Nome = "Azul 4", RGB = "0,81,157", Hexadecimal = "#FF00519D" });
                        corRepository.Adicionar(new Cor { Nome = "Azul 5", RGB = "0,63,121", Hexadecimal = "#FF003F79" });
                        corRepository.Adicionar(new Cor { Nome = "Azul 6", RGB = "0,44,84", Hexadecimal = "#FF002C54" });
                        #endregion

                        #region Verde
                        corRepository.Adicionar(new Cor { Nome = "Verde 1", RGB = "50,188,42", Hexadecimal = "#FF32BC2A" });
                        corRepository.Adicionar(new Cor { Nome = "Verde 2", RGB = "51,171,44", Hexadecimal = "#FF33AB2C" });
                        corRepository.Adicionar(new Cor { Nome = "Verde 3", RGB = "12,152,4", Hexadecimal = "#FF0C9804" });
                        corRepository.Adicionar(new Cor { Nome = "Verde 4", RGB = "8,142,0", Hexadecimal = "#FF088E00" });
                        corRepository.Adicionar(new Cor { Nome = "Verde 5", RGB = "7,124,0", Hexadecimal = "#FF077C00" });
                        corRepository.Adicionar(new Cor { Nome = "Verde 6", RGB = "6,103,0", Hexadecimal = "#FF066700" });
                        #endregion

                        #region Laranja
                        corRepository.Adicionar(new Cor { Nome = "Laranja 1", RGB = "255,157,0", Hexadecimal = "#FFFF9D00" });
                        corRepository.Adicionar(new Cor { Nome = "Laranja 2", RGB = "255,136,0", Hexadecimal = "#FFFF8800" });
                        corRepository.Adicionar(new Cor { Nome = "Laranja 3", RGB = "251,124,13", Hexadecimal = "#FFFB7C0D" });
                        corRepository.Adicionar(new Cor { Nome = "Laranja 4", RGB = "238,111,0", Hexadecimal = "#FFEE6F00" });
                        corRepository.Adicionar(new Cor { Nome = "Laranja 5", RGB = "230,101,8", Hexadecimal = "#FFE66508" });
                        corRepository.Adicionar(new Cor { Nome = "Laranja 6", RGB = "216,90,0", Hexadecimal = "#FFD85A00" });
                        #endregion

                        #region Roxo
                        corRepository.Adicionar(new Cor { Nome = "Roxo 1", RGB = "191,0,255", Hexadecimal = "#FFBF00FF" });
                        corRepository.Adicionar(new Cor { Nome = "Roxo 2", RGB = "172,0,229", Hexadecimal = "#FFAC00E5" });
                        corRepository.Adicionar(new Cor { Nome = "Roxo 3", RGB = "154,0,205", Hexadecimal = "#FF9A00CD" });
                        corRepository.Adicionar(new Cor { Nome = "Roxo 4", RGB = "137,0,183", Hexadecimal = "#FF8900B7" });
                        corRepository.Adicionar(new Cor { Nome = "Roxo 5", RGB = "115,0,153", Hexadecimal = "#FF730099" });
                        corRepository.Adicionar(new Cor { Nome = "Roxo 6", RGB = "97,0,129", Hexadecimal = "#FF610081" });
                        #endregion

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
