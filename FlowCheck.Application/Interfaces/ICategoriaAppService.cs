using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlowCheck.Domain.Entidades;

namespace FlowCheck.Application.Interfaces
{
    public interface ICategoriaAppService
    {
        bool SalvarCategoria(Categoria categoria);
        bool RemoverCategoria(Categoria categoria);
        bool RemoverCategorias(IEnumerable<Categoria> categorias);
        IEnumerable<Categoria> Pesquisar(Categoria_Request request);
    }
}
