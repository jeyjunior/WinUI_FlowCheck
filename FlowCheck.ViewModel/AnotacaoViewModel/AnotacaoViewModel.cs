using FlowCheck.Domain.Entidades;
using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowCheck.ViewModel.AnotacaoViewModel
{
    public class AnotacaoViewModel : INotifyPropertyChanged
    {
        #region Propriedades
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Metodos
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        #endregion

        #region Construtor
        public AnotacaoViewModel(Anotacao anotacao)
        {
            _anotacao = anotacao;
        }
        #endregion

        #region Anotacao
        private readonly Anotacao _anotacao;
        public Anotacao Anotacao { get => _anotacao; }
        public string Descricao
        {
            get => _anotacao.Descricao;
        }
        public int PK_Anotacao
        {
            get => _anotacao.PK_Anotacao;
        }
        #endregion
    }
}
