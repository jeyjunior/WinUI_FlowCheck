using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using JJ.Net.Core.DTO;
using Windows.UI;
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
using FlowCheck.Application;
using FlowCheck.Application.Interfaces;
using FlowCheck.Application.Services;
using FlowCheck.Domain.Entidades;
using FlowCheck.Domain.Helpers;
using FlowCheck.Domain.Interfaces;
using FlowCheck.InfraData.Repository;
using FlowCheck.ViewModel.AnotacaoViewModel;
using FlowCheck.ViewModel.TarefaViewModel;

namespace FlowCheck.View
{
    public sealed partial class AnotacaoPage : Page, IPageComandos, IDialogComandos
    {
        #region Interfaces
        private readonly IAnotacaoAppService anotacaoAppService;
        private readonly ICategoriaRepository categoriaRepository;
        #endregion

        #region Propriedades
        private AnotacaoPageViewModel ViewModel { get; set; }
        private Anotacao anotacao { get; set; }
        private bool fecharDialog = true;
        private CancellationTokenSource _cancellationTokenSource;
        private readonly int _delayPesquisa = 500;
        #endregion

        #region Construtor
        public AnotacaoPage()
        {
            InitializeComponent();

            ViewModel = new AnotacaoPageViewModel();
            this.DataContext = ViewModel;

            anotacaoAppService = Bootstrap.ServiceProvider.GetRequiredService<IAnotacaoAppService>();
            categoriaRepository = Bootstrap.ServiceProvider.GetRequiredService<ICategoriaRepository>();
        }
        #endregion

        #region Eventos
        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                CarregarDropDown();
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
        private void dialogAnotacao_Closing(ContentDialog sender, ContentDialogClosingEventArgs args)
        {
            if (!fecharDialog)
                args.Cancel = true;
        }
        private void dialogAnotacao_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            Salvar();
        }
        private void dialogAnotacao_CloseButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            Fechar();
        }
        private void txtAnotacao_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
        #endregion

        #region Métodos
        private void CarregarAnotacoes()
        {
            ViewModel.LimparAnotacoes();

            var ret = anotacaoAppService.Pesquisar(new Anotacao_Request { Descricao = "" });

            foreach (var item in ret)
                ViewModel.AdicionarAnotacao(item);
        }
        private void CarregarDropDown()
        {
            CarregarTipoDePesquisa();
            CarregarTipoDeOrdenacao();
            CarregarCategorias();
        }
        private void CarregarTipoDePesquisa()
        {
            var tipoDePesqusia = new List<Item>
            {
                new Item { ID = "0", Valor = "Tudo"},
                new Item { ID = "1", Valor = "Categoria"},
                new Item { ID = "2", Valor = "Anotação"},
            };

            this.cboTipoDePesquisa.ItemsSource = tipoDePesqusia;
            this.cboTipoDePesquisa.SelectedValuePath = "ID";
            this.cboTipoDePesquisa.DisplayMemberPath = "Valor";
            this.cboTipoDePesquisa.SelectedValue = "0";
        }
        private void CarregarTipoDeOrdenacao()
        {
            var tipoDePesqusia = new List<Item>
            {
                new Item { ID = "0", Valor = "Data"},
                new Item { ID = "1", Valor = "Categoria"},
                new Item { ID = "2", Valor = "Anotação"},
            };

            this.cboTipoDeOrdenacao.ItemsSource = tipoDePesqusia;
            this.cboTipoDeOrdenacao.SelectedValuePath = "ID";
            this.cboTipoDeOrdenacao.DisplayMemberPath = "Valor";
            this.cboTipoDeOrdenacao.SelectedValue = "2";
        }
        private void CarregarCategorias()
        {
            var categorias = categoriaRepository.ObterLista();

            this.cboCategoria.ItemsSource = categorias.Select(i => new 
            {
                PK_Categoria = i.PK_Categoria,
                Nome = i.Nome,
                Cor_SolidColorBrush = i.Cor.Cor_SolidColorBrush
            }).ToList();

            this.cboCategoria.SelectedValuePath = "PK_Categoria";
            this.cboCategoria.SelectedIndex = 0;
        }
        private void Limpar()
        {
            this.anotacao = null;

            txtAnotacao.Text = "";
            this.ViewModel.MensagemAviso = "";
            this.cboCategoria.SelectedIndex = 0;
        }
        #endregion

        #region Métodos Públicos
        public void Salvar()
        {
            try
            {
                var PK_CategoriaSelecionada = (int)this.cboCategoria.SelectedValue;

                anotacao = new Anotacao()
                {
                    Ativo = true,
                    FK_Categoria = PK_CategoriaSelecionada,
                    Descricao = txtAnotacao.Text.Trim(),
                    DataCriacao = DateTime.Now,
                    PK_Anotacao = 0
                };

                if (!anotacao.Validar())
                {
                    fecharDialog = false;
                    ViewModel.MensagemAviso = anotacao.ValidarResultado.ObterPrimeiroErro();
                    return;
                }

                anotacaoAppService.SalvarAnotacao(anotacao);

                if (!anotacao.ValidarResultado.EhValido)
                {
                    fecharDialog = false;
                    ViewModel.MensagemAviso = anotacao.ValidarResultado.ObterPrimeiroErro();
                    return;
                }

                CarregarAnotacoes();

                fecharDialog = true;
            }
            catch (Exception ex)
            {
                ViewModel.MensagemAviso = ex.Message;
            }
        }
        public async void Adicionar()
        {
            Limpar();

            dialogAnotacao.XamlRoot = this.Content.XamlRoot;
            dialogAnotacao.HorizontalAlignment = HorizontalAlignment.Center;
            dialogAnotacao.VerticalAlignment = VerticalAlignment.Center;

            await dialogAnotacao.ShowAsync();
        }
        public void Fechar()
        {
            fecharDialog = true;
            dialogAnotacao.Hide();
        }
        #endregion
    }
}
