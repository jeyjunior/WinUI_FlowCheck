using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI; 
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml;
using JJ.Net.Core.Commands;
using FlowCheck.Domain.Entidades;

namespace FlowCheck.ViewModel.CategoriaViewModel
{
    public class CategoriaViewModel : INotifyPropertyChanged
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
        public CategoriaViewModel(Categoria categoria)
        {
            _categoria = categoria;
        }
        #endregion

        #region Categoria
        private readonly Categoria _categoria;
        public Categoria Categoria { get => _categoria; }
        public string Nome
        {
            get => _categoria.Nome;
        }
        public int PK_Categoria
        {
            get => _categoria.PK_Categoria;
        }
        public SolidColorBrush Cor
        {
            get
            {
                return HexadecimalToSolidColorBrush(_categoria.Cor.Hexadecimal);
            }
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

            Color color = Color.FromArgb(a, r, g, b);
            return new SolidColorBrush(color);
        }
        #endregion
    }
}
