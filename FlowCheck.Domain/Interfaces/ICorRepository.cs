using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlowCheck.Domain.Entidades;
using JJ.Net.CrossData_WinUI_3.Atributo;
using JJ.Net.CrossData_WinUI_3.Interfaces;

namespace FlowCheck.Domain.Interfaces
{
    public interface ICorRepository : IRepository<Cor>
    {
        Cor GerarCorAleatoria();
    }
}
