using FlowCheck.Domain.Entidades;
using JJ.Net.Core.Commands;
using JJ.Net.Core.Extensoes;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowCheck.ViewModel.TarefaView
{
    public class TarefaViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public RelayCommand ToggleAnotacaoCommand { get; }
        public TarefaViewModel(Tarefa tarefa)
        {
            _tarefa = tarefa;
            ToggleAnotacaoCommand = new RelayCommand(ToggleAnotacao);

            _idGenerico = Guid.NewGuid();

            DefinirPadraoInicial();
        }
        
        #region MetodosPadronizado
        private void DefinirPadraoInicial()
        {
            ExibirAnotacao = Visibility.Collapsed;
            IconeBotaoExibirTarefaAnotacao = "\xE70D";
        }
        #endregion

        #region Tarefa
        private Guid _idGenerico;
        public string IDGenerico => _idGenerico.ToString();

        private readonly Tarefa _tarefa;
        public Tarefa Tarefa { get => _tarefa; }
        public bool Concluido
        {
            get => _tarefa.Concluido;
            set
            {
                _tarefa.Concluido = value;
                OnPropertyChanged(nameof(Concluido));
            }
        }
        public string Descricao
        {
            get => _tarefa.Descricao;
            set
            {
                _tarefa.Descricao = value;
                OnPropertyChanged(nameof(Descricao));
            }
        }

        private bool _editarTarefa;
        public bool EditarTarefa
        {
            get => _editarTarefa;
            set
            {
                _editarTarefa = value;
                OnPropertyChanged(nameof(ExibirTextoEditavel));
                OnPropertyChanged(nameof(ExibirTextoVisual));
            }
        }
        public Visibility ExibirTextoEditavel
        {
            get
            {
                return ((_editarTarefa) ? Visibility.Visible : Visibility.Collapsed);
            }
        }
        public Visibility ExibirTextoVisual
        {
            get
            {
                return ((_editarTarefa) ? Visibility.Collapsed : Visibility.Visible);
            }
        }
        #endregion

        #region TarefaAnotacao
        private Visibility _exibirAnotacao = Visibility.Collapsed;
        public Visibility ExibirAnotacao
        {
            get => _exibirAnotacao;
            set
            {
                if (value != _exibirAnotacao)
                {
                    _exibirAnotacao = value;
                    OnPropertyChanged(nameof(ExibirAnotacao));
                }
            }
        }

        private string _iconeBotaoExibirTarefaAnotacao;
        public string IconeBotaoExibirTarefaAnotacao
        {
            get => _iconeBotaoExibirTarefaAnotacao;
            set
            {
                _iconeBotaoExibirTarefaAnotacao = value;
                OnPropertyChanged(nameof(IconeBotaoExibirTarefaAnotacao));
            }
        }
        private void ToggleAnotacao()
        {
            if (ExibirAnotacao == Visibility.Visible)
            {
                ExibirAnotacao = Visibility.Collapsed;
                IconeBotaoExibirTarefaAnotacao = "\xE70D";
            }
            else
            {
                ExibirAnotacao = Visibility.Visible;
                IconeBotaoExibirTarefaAnotacao = "\xE70E";
            }
        }
        public string AnotacaoTarefa 
        {
            get 
            {
                string valor = "";

                if (Tarefa.TarefaAnotacao != null)
                    valor = Tarefa.TarefaAnotacao.Anotacao.ObterValorOuPadrao("");

                return valor;
            }
            set
            {
                if (Tarefa.TarefaAnotacao != null)
                {
                    Tarefa.TarefaAnotacao.Anotacao = value;
                }
                else
                {
                    Tarefa.TarefaAnotacao = new TarefaAnotacao();
                    Tarefa.TarefaAnotacao.Anotacao = value;
                }

                OnPropertyChanged(nameof(AnotacaoTarefa));
            }
        }
        #endregion

        #region TextTarefa

        #endregion
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
