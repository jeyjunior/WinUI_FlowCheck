using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using JJ.Net.Core.Validador;
using FlowCheck.Application;
using FlowCheck.Application.Interfaces;
using FlowCheck.Application.Services;
using FlowCheck.Domain.Entidades;
using FlowCheck.Domain.Enumerador;
using FlowCheck.Domain.Helpers;
using FlowCheck.Domain.Interfaces;
using FlowCheck.ViewModel.CategoriaViewModel;
using FlowCheck.ViewModel.TarefaViewModel;

namespace FlowCheck.View
{
    public sealed partial class CategoriaPage : Page, IPageComandos
    {
        #region Interfaces
        private readonly ICategoriaAppService categoriaAppService;
        #endregion

        #region Propriedades
        private CategoriaPageViewModel ViewModel { get; set; }
        #endregion

        #region Construtor
        public CategoriaPage()
        {
            this.InitializeComponent();

            ViewModel = new CategoriaPageViewModel();
            this.DataContext = ViewModel;

            categoriaAppService = Bootstrap.ServiceProvider.GetRequiredService<ICategoriaAppService>();
        }
        #endregion

        #region Eventos
        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                CarregarCategorias();
            }
            catch (Exception ex)
            {
                await Mensagem.ExibirErroAsync(ex.Message, this.Content.XamlRoot);
            }
        }
        private void scroll_LayoutUpdated(object sender, object e)
        {

        }
        private void txtCategoria_LostFocus(object sender, RoutedEventArgs e)
        {

        }
        private void txbCategoria_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {

        }
        private void btnRemoverCategoria_Click(object sender, RoutedEventArgs e)
        {

        }
        private void MenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {

        }
        private void btnEditarCategoria_Click(object sender, RoutedEventArgs e)
        {

        }
        #endregion

        #region Métodos
        private void CarregarCategorias()
        {
            var ret = categoriaAppService.Pesquisar(new Categoria_Request { Nome = "" });

            foreach (var item in ret)
                ViewModel.AdicionarCategoria(item);
        }
        #endregion

        #region Métodos Público
        public void Adicionar()
        {

        }
        public void Salvar()
        {
            
        }
        #endregion
    }
}
