using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using FlowCheck.Domain.Entidades;

namespace FlowCheck.Application.Interfaces
{
    public interface IParametroAppService
    {
        void SalvarParametros(Parametro_AppServiceRequest request);
        IEnumerable<Parametro> Pesquisar(Parametro_Request request);
    }
}
