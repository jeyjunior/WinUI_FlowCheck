﻿using FlowCheck.Domain.Entidades;
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

namespace FlowCheck.ViewModel.CategoriaViewModel
{
    public class CategoriaPageViewModel : INotifyPropertyChanged
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

        #region Categoria Itens
        public ObservableCollection<CategoriaViewModel> Categorias { get; } = new ObservableCollection<CategoriaViewModel>();
        public CategoriaViewModel AdicionarCategoria(Categoria categoria)
        {
            var categoriaViewModel = new CategoriaViewModel(categoria);
            this.Categorias.Add(categoriaViewModel);

            AtualizarStatus();
            return categoriaViewModel;
        }
        public void RemoverCategoria(int PK_Categoria)
        {
            var categoria = this.Categorias.Where(i => i.Categoria.PK_Categoria.Equals(PK_Categoria)).FirstOrDefault();
            if (categoria != null)
                this.Categorias.Remove(categoria);

            AtualizarStatus();
        }
        public Categoria ObterCategoria(int PK_Categoria)
        {
            return this.Categorias.Where(i => i.Categoria.PK_Categoria.Equals(PK_Categoria)).FirstOrDefault().Categoria;
        }

        public void LimparCategorias()
        {
            Categorias.Clear();
            AtualizarStatus();
        }
        #endregion

        #region Status 
        public string CategoriaStatus
        {
            get
            {
                string ret = "";

                if (Categorias.Count > 0)
                    ret = $"{Categorias.Count}";

                return ret;
            }
        }
        public void AtualizarStatus()
        {
            OnPropertyChanged(nameof(CategoriaStatus));
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
