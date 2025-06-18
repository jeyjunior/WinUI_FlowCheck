using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlowCheck.Domain.Entidades;
using Microsoft.UI.Xaml.Controls;
using JJ.Net.Core.Commands;
using JJ.Net.Core.Extensoes;

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
        public ObservableCollection<TarefaViewModel> Tarefas { get; } = new ObservableCollection<TarefaViewModel>();
        public TarefaViewModel AdicionarTarefa(Tarefa tarefa)
        {
            var tarefaViewModel = new TarefaViewModel(tarefa);
            Tarefas.Add(tarefaViewModel);

            return tarefaViewModel;
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


        public void ReordenarTarefas(int indiceOriginal, int indiceAlvo, TarefaViewModel tarefaMovida)
        {
            Tarefas.RemoveAt(indiceOriginal);

            if (indiceAlvo >= Tarefas.Count)
                Tarefas.Add(tarefaMovida);
            else
                Tarefas.Insert(indiceAlvo, tarefaMovida);

            AtualizarIndicesOrdem();
        }

        private void AtualizarIndicesOrdem()
        {
            for (int i = 0; i < Tarefas.Count; i++)
            {
                Tarefas[i].IndiceOrdem = i;
            }

            OnPropertyChanged(nameof(Tarefas));
        }
    }
}