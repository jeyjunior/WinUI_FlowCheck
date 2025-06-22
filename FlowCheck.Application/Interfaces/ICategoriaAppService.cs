using FlowCheck.Domain.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowCheck.Application.Interfaces
{
    public interface ICategoriaAppService
    {
        IEnumerable<Categoria> Pesquisar(Categoria_Request request);
    }
}
