using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlowCheck.Domain.Entidades;

namespace FlowCheck.ViewModel.TarefaView
{
    public class TarefaPageViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) 
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        
        private string _titulo;
        public string Titulo
        {
            get => _titulo;
            set
            {
                if (_titulo != value)
                {
                    _titulo = value;
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
        #endregion
    }
}
