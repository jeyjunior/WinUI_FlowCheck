using FlowCheck.Domain.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowCheck.Application.Interfaces
{
    public interface IParametroAppService
    {
        void SalvarParametros(Parametro_AppServiceRequest request);
    }
}
