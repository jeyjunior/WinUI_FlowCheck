using FlowCheck.Domain.Entidades;
using JJ.Net.Core.Commands;
using JJ.Net.Core.Extensoes;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowCheck.ViewModel.TarefaView
{
    public class TarefaPageViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) 
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private Parametro _parametro { get; set; }
        public Parametro Parametro 
        { 
            get => _parametro;
            set 
            {
                _parametro = value;
                OnPropertyChanged(nameof(Titulo));
            } 
        }

        private string _titulo;
        public string Titulo 
        {
            get 
            {
                if (_titulo.ObterValorOuPadrao("").Trim() != "")
                    return _titulo;

                if (_parametro != null)
                    return _parametro.Valor.ObterValorOuPadrao("").Trim();

                return "Título";
            }
            set
            {
                if (_titulo != value)
                {
                    _titulo = value;
                    _parametro.Valor = value;
                    OnPropertyChanged(nameof(Titulo));
                }
            }
        }

        #region Tarefa Itens
        public ObservableCollection<TarefaViewModel> Tarefas { get; } = new();
        public void AdicionarTarefa(Tarefa tarefa)
        {
            Tarefas.Add(new TarefaViewModel(tarefa));
        }
        public void RemoverTarefa(string IDGenerico)
        {
            var tarefa = this.Tarefas.Where(i => i.IDGenerico.Equals(IDGenerico)).FirstOrDefault();
            if (tarefa != null)
                this.Tarefas.Remove(tarefa);
        }
        public void EditarTarefa(string IDGenerico, bool editar)
        {
            var tarefa = this.Tarefas.Where(i => i.IDGenerico.Equals(IDGenerico)).FirstOrDefault();
            if (tarefa != null)
                tarefa.EditarTarefa = editar;
        }
        public TarefaViewModel ObterTarefa(string IDGenerico)
        {
            return this.Tarefas.Where(i => i.IDGenerico.Equals(IDGenerico)).FirstOrDefault();
        }
        #endregion
    }
}
