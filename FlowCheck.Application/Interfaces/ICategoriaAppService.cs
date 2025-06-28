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
        bool SalvarCategoria(Categoria categoria);
        IEnumerable<Categoria> Pesquisar(Categoria_Request request);
    }
}
