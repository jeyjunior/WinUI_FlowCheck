using FlowCheck.Domain.Entidades;
using FlowCheck.Domain.Enumerador;
using JJ.Net.Core.Extensoes;
using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FlowCheck.ViewModel.AnotacaoViewModel
{
    public class AnotacaoPageViewModel : INotifyPropertyChanged
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

        #region Anotacao Itens
        public ObservableCollection<AnotacaoViewModel> Anotacoes { get; } = new ObservableCollection<AnotacaoViewModel>();

        public AnotacaoViewModel AdicionarAnotacao(Anotacao anotacao)
        {
            var anotacaoViewModel = new AnotacaoViewModel(anotacao);
            this.Anotacoes.Add(anotacaoViewModel);

            AtualizarStatus();
            return anotacaoViewModel;
        }
        public AnotacaoViewModel AdicionarAnotacao(AnotacaoViewModel anotacao)
        {
            this.Anotacoes.Add(anotacao);

            AtualizarStatus();
            return anotacao;
        }
        public void RemoverAnotacao(int PK_Anotacao)
        {
            var anotacao = this.Anotacoes.Where(i => i.Anotacao.PK_Anotacao.Equals(PK_Anotacao)).FirstOrDefault();
            if (anotacao != null)
                this.Anotacoes.Remove(anotacao);

            AtualizarStatus();
        }
        public Anotacao ObterAnotacao(int PK_Anotacao)
        {
            return this.Anotacoes.Where(i => i.Anotacao.PK_Anotacao.Equals(PK_Anotacao)).FirstOrDefault().Anotacao;
        }
        public void LimparAnotacoes()
        {
            Anotacoes.Clear();
            AtualizarStatus();
        }
        #endregion

        #region Status 
        public string AnotacaoStatus
        {
            get
            {
                string ret = "";

                if (Anotacoes.Count > 0)
                    ret = $"{Anotacoes.Count}";

                return ret;
            }
        }
        public void AtualizarStatus()
        {
            OnPropertyChanged(nameof(AnotacaoStatus));
        }
        #endregion

        #region Aviso
        private string _mensagemAviso { get; set; }
        public Visibility ExibirAviso
        {
            get => (_mensagemAviso.ObterValorOuPadrao("").Trim() == "" ? Visibility.Collapsed : Visibility.Visible);
        }

        public string MensagemAviso
        {
            get => _mensagemAviso;
            set
            {
                _mensagemAviso = value;
                OnPropertyChanged(nameof(MensagemAviso));
                OnPropertyChanged(nameof(ExibirAviso));
            }
        }
        #endregion
    }
}
