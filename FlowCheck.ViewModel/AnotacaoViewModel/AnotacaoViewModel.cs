using FlowCheck.Domain.Entidades;
using FlowCheck.Domain.Extensoes;
using JJ.Net.Core.Extensoes;
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

        #region Categoria
        public string CategoriaNome
        {
            get => (_anotacao.Categoria != null) ? _anotacao.Categoria.Nome.ObterValorOuPadrao("").Trim() : "";
        }
        public int PK_Categoria
        {
            get => (_anotacao.Categoria != null) ? _anotacao.Categoria.PK_Categoria : 0;
        }
        public SolidColorBrush Cor
        {
            get => ObterCorCategoria();
        }

        private SolidColorBrush ObterCorCategoria()
        {
            if (_anotacao.Categoria == null)
                return ("#FF121212").HexadecimalToSolidColorBrush();

            if (_anotacao.Categoria.Cor == null)
                return ("#FF121212").HexadecimalToSolidColorBrush();

            return _anotacao.Categoria.Cor.Hexadecimal.HexadecimalToSolidColorBrush();
        }
        #endregion
    }
}
