using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlowCheck.Domain.Entidades;

namespace FlowCheck.ViewModel.TarefaView
{
    public class TarefaViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private readonly Tarefa _tarefa;
        public Tarefa Tarefa { get => _tarefa; }
        
        public TarefaViewModel(Tarefa tarefa)
        {
            _tarefa = tarefa;
        }

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

        public string AnotacaoTarefa { get => "Teste"; }

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
