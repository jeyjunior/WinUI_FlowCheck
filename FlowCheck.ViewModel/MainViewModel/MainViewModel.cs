using FlowCheck.Domain.Entidades;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowCheck.ViewModel.MainViewModel
{
    public class MainViewModel : INotifyPropertyChanged
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

        private Visibility _exibirBotoesAcao;
        public Visibility ExibirBotoesAcao
        {
            get => _exibirBotoesAcao;
            set
            {
                _exibirBotoesAcao = value;
                OnPropertyChanged(nameof(ExibirBotoesAcao));
            }
        }
    }
}
