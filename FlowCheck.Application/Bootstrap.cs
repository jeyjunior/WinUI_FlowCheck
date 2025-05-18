using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JJ.NET.Data.Interfaces;
using JJ.NET.Data;
using SimpleInjector;
using FlowCheck.Domain.Interfaces;
using FlowCheck.InfraData.Repository;

namespace FlowCheck.Application
{
    public static class Bootstrap
    {
        public static Container Container { get; private set; }

        public static async Task IniciarAsync()
        {
            Configuracao.Iniciar();

            Container = new Container();
            Container.Options.DefaultLifestyle = Lifestyle.Scoped;

            Container.Register<IUnitOfWork>(() => new UnitOfWork(Configuracao.ConexaoBaseDados), Lifestyle.Singleton);

            Container.Register<IAnotacaoRepository, AnotacaoRepository>(Lifestyle.Singleton);
            Container.Register<ICategoriaRepository, CategoriaRepository>(Lifestyle.Singleton);
            Container.Register<ICorRepository, CorRepository>(Lifestyle.Singleton);
            Container.Register<IParametroRepository, ParametroRepository>(Lifestyle.Singleton);
            Container.Register<ITarefaAnotacaoRepository, TarefaAnotacaoRepository>(Lifestyle.Singleton);
            Container.Register<ITarefaRepository, TarefaRepository>(Lifestyle.Singleton);

            Container.Options.EnableAutoVerification = false;

            Configuracao.RegistrarEntidades();
            Configuracao.RegistrarParametros();
        }
    }
}
