﻿using System;
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
using FlowCheck.Domain.DTO;

namespace FlowCheck.ViewModel.TarefaViewModel
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

            AtualizarStatus();
            return tarefaViewModel;
        }
        public void RemoverTarefa(string IDGenerico)
        {
            var tarefa = this.Tarefas.Where(i => i.IDGenerico.Equals(IDGenerico)).FirstOrDefault();
            if (tarefa != null)
                this.Tarefas.Remove(tarefa);

            AtualizarStatus();
        }
        public void RemoverTarefas(List<TarefaViewModel> tarefas)
        {
            foreach (var tarefa in tarefas)
                this.Tarefas.Remove(tarefa);

            AtualizarStatus();
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
        public void LimparTarefas()
        {
            Tarefas.Clear();
            AtualizarStatus();
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
                Tarefas[i].Tarefa.IndiceExibicao = i;
            }

            OnPropertyChanged(nameof(Tarefas));
        }
        public bool TudoConcluido
        {
            set
            {
                if (Tarefas == null)
                    return;

                if (Tarefas.Count <= 0)
                    return;

                foreach (var item in Tarefas)
                    item.Concluido = value;

                OnPropertyChanged(nameof(TarefaStatus));
            }
        }
        public string TarefaStatus
        {
            get
            {
                string ret = "";

                if (Tarefas.Count > 0)
                    ret = $"{Tarefas.Count(i => i.Concluido)}/{Tarefas.Count}";

                return ret;
            }
        }
        public void AtualizarStatus()
        {
            OnPropertyChanged(nameof(TarefaStatus));
        }
    }
}