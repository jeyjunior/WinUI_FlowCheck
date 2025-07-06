using FlowCheck.Domain.Entidades;
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
                return HexadecimalToSolidColorBrush("#FF121212");

            if (_anotacao.Categoria.Cor == null)
                return HexadecimalToSolidColorBrush("#FF121212");

            return HexadecimalToSolidColorBrush(_anotacao.Categoria.Cor.Hexadecimal);
        }

        private SolidColorBrush HexadecimalToSolidColorBrush(string hexColor)
        {
            hexColor = hexColor.Replace("#", "");
            if (hexColor.Length == 6)
                hexColor = "FF" + hexColor;

            byte a = Convert.ToByte(hexColor.Substring(0, 2), 16);
            byte r = Convert.ToByte(hexColor.Substring(2, 2), 16);
            byte g = Convert.ToByte(hexColor.Substring(4, 2), 16);
            byte b = Convert.ToByte(hexColor.Substring(6, 2), 16);

            Windows.UI.Color color = Windows.UI.Color.FromArgb(a, r, g, b);
            return new SolidColorBrush(color);
        }
        #endregion
    }
}
