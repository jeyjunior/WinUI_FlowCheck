using FlowCheck.Application;
using FlowCheck.Application.Interfaces;
using FlowCheck.Application.Services;
using FlowCheck.Domain.Entidades;
using FlowCheck.Domain.Helpers;
using FlowCheck.ViewModel.AnotacaoViewModel;
using FlowCheck.ViewModel.TarefaViewModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;


namespace FlowCheck.View
{
    public sealed partial class AnotacaoPage : Page
    {
        #region Interfaces
        private readonly IAnotacaoAppService anotacaoAppService;
        #endregion

        #region Propriedades
        private AnotacaoPageViewModel ViewModel { get; set; }
        #endregion

        #region Construtor
        public AnotacaoPage()
        {
            InitializeComponent();

            ViewModel = new AnotacaoPageViewModel();
            this.DataContext = ViewModel;

            anotacaoAppService = Bootstrap.ServiceProvider.GetRequiredService<IAnotacaoAppService>();
        }
        #endregion

        #region Eventos
        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                CarregarAnotacoes();
            }
            catch (Exception ex)
            {
                await Mensagem.ExibirErroAsync(this.Content.XamlRoot, ex.Message);
            }
        }
        private void scroll_LayoutUpdated(object sender, object e)
        {

        }
        private void txtPesquisaCategoria_KeyUp(object sender, KeyRoutedEventArgs e)
        {

        }
        #endregion
        #region Métodos
        private void CarregarAnotacoes()
        {
            var ret = anotacaoAppService.Pesquisar(new Anotacao_Request { Descricao = "" });

            foreach (var item in ret)
                ViewModel.AdicionarAnotacao(item);
        }
        #endregion
    }
}
