using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI;
using Windows.UI.Text;
using JJ.Net.Core.Commands;
using JJ.Net.Core.Extensoes;
using FlowCheck.Domain.Entidades;

namespace FlowCheck.ViewModel.TarefaViewModel
{
    public class TarefaViewModel : INotifyPropertyChanged
    {
        #region Propriedades
        public event PropertyChangedEventHandler PropertyChanged;
        public RelayCommand ToggleAnotacaoCommand { get; }
        #endregion

        #region Construtor
        public TarefaViewModel(Tarefa tarefa)
        {
            _tarefa = tarefa;
            ToggleAnotacaoCommand = new RelayCommand(ToggleAnotacao);

            _idGenerico = Guid.NewGuid();

            DefinirPadraoInicial();
        }
        #endregion

        #region Metodos
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
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
                OnPropertyChanged(nameof(DecoracaoTexto));
                OnPropertyChanged(nameof(CorTextoTextBlock));
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
        public TextDecorations DecoracaoTexto
        {
            get => ((Concluido) ? TextDecorations.Strikethrough : TextDecorations.None);
        }
        public SolidColorBrush CorTextoTextBlock
        {
            get
            {
                return Concluido
                    ? (SolidColorBrush)Microsoft.UI.Xaml.Application.Current.Resources["Cinza3"] 
                    : (SolidColorBrush)Microsoft.UI.Xaml.Application.Current.Resources["Branco"];
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
                    OnPropertyChanged(nameof(ExisteAnotacao));
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

            OnPropertyChanged(nameof(ExisteAnotacao));
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
                OnPropertyChanged(nameof(ExisteAnotacao));
            }
        }
        private SolidColorBrush _existeAnotacao = (SolidColorBrush)Microsoft.UI.Xaml.Application.Current.Resources["Nenhuma"];
        public SolidColorBrush ExisteAnotacao
        {
            get
            {
                return AnotacaoTarefa.ObterValorOuPadrao("").Trim() != ""
                    ? (SolidColorBrush)Microsoft.UI.Xaml.Application.Current.Resources["Roxo"] : (SolidColorBrush)Microsoft.UI.Xaml.Application.Current.Resources["Nenhuma"];
            }
            set
            {
                if (_existeAnotacao != value)
                {
                    _existeAnotacao = value;
                    OnPropertyChanged(nameof(ExisteAnotacao));
                }
            }
        }
        #endregion

        #region Indice
        private int _indiceOrdem;
        public int IndiceOrdem
        {
            get => _indiceOrdem;
            set
            {
                if (_indiceOrdem != value)
                {
                    _indiceOrdem = value;
                    OnPropertyChanged(nameof(IndiceOrdem));

                    // Atualiza também no modelo Tarefa se necessário
                    if (_tarefa != null)
                        _tarefa.IndiceExibicao = value;
                }
            }
        }
        #endregion
    }
}
