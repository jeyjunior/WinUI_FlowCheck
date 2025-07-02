using System;
using System.Collections.Generic;
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
}
