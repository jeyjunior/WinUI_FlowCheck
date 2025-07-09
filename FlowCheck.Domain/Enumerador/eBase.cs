using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowCheck.Domain.Enumerador
{
    public enum eParametro
    {
        TituloTarefa = 1,
    }

    public enum eTelaEmExecucao
    {
        Nenhuma = 0,
        Tarefa = 1,
        Anotacao = 2,
    }

    public enum eTipoMensagem
    {
        Informacao = 0,
        Confirmacao = 1,
        Aviso = 2,
        Erro = 3
    }

    public enum eTipoMensagemResultado
    {
        Nenhum = 0,
        OK = 1,
        Sim = 2,
        Cancelar = 3
    }

    public enum eTipoPesquisaAnotacao
    {
        [Description("Tudo")]
        Tudo = 0,
        [Description("Categoria")]
        Categoria = 1,
        [Description("Anotação")]
        Anotacao = 2
    }

    public enum eTipoOrdenacaoAnotacao
    {
        [Description("Data Criação")]
        Data = 0,
        [Description("Categoria")]
        Categoria = 1,
        [Description("Anotação")]
        Anotacao = 2
    }

    public enum eDirecaoOrdenacao
    {
        Ascendente,
        Descendente
    }
}
