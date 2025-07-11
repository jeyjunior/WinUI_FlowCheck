using FlowCheck.Application;
using FlowCheck.Application.Interfaces;
using FlowCheck.Application.Services;
using FlowCheck.Domain.Entidades;
using FlowCheck.Domain.Enumerador;
using FlowCheck.Domain.Helpers;
using FlowCheck.Domain.Interfaces;
using FlowCheck.InfraData.Repository;
using FlowCheck.ViewModel.AnotacaoViewModel;
using FlowCheck.ViewModel.TarefaViewModel;
using JJ.Net.Core.DTO;
using JJ.Net.Core.Extensoes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI;
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
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;

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
        private eDirecaoOrdenacao direcaoOrdenacao = eDirecaoOrdenacao.Descendente;
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
        private async void txtPesquisaAnotacao_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            await PesquisarAnotacoesAsync();
        }
        private async void cboTipoDePesquisa_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            await PesquisarAnotacoesAsync();
        }
        private async void cboCategoriaExibicao_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                int PK_CategoriaSelecionada = (int)this.cboCategoriaExibicao.SelectedValue;

                txtPesquisaAnotacao.Text = "";
                ViewModel.LimparAnotacoes();

                var ret = anotacaoAppService.PesquisarPorCategoria(PK_CategoriaSelecionada).ToList();
                foreach (var item in ret)
                    ViewModel.AdicionarAnotacao(item);

                await OrdenarAnotacoesAsync();
            }
            catch (Exception ex)
            {
                await Mensagem.ExibirErroAsync(this.Content.XamlRoot, ex.Message);
            }
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
        private async void btnEditarAnotacao_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is MenuFlyoutItem btn && btn.Tag is int PK_Anotacao)
                {
                    anotacao = ViewModel.ObterAnotacao(PK_Anotacao);
                    if (anotacao == null)
                        return;

                    if (anotacao.FK_Categoria != null)
                        this.cboCategoria.SelectedValue = anotacao.FK_Categoria;

                    txtAnotacao.Text = anotacao.Descricao;
                    txtAnotacao.SelectAll();

                    await this.dialogAnotacao.ShowAsync();
                }
            }
            catch (Exception ex)
            {
                await Mensagem.ExibirErroAsync(this.Content.XamlRoot, ex.Message);
            }
        }
        private async void btnRemoverAnotacao_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var ret = await Mensagem.ExibirConfirmacaoAsync(this.Content.XamlRoot, "Tem certeza que deseja remover essa anotação?\nA operação não poderá ser desfeita.");
                if (ret != Domain.Enumerador.eTipoMensagemResultado.Sim)
                    return;

                if (sender is MenuFlyoutItem btn && btn.Tag is int PK_Anotacao)
                {
                    anotacao = ViewModel.ObterAnotacao(PK_Anotacao);
                    if (anotacao == null)
                        return;

                    if (anotacaoAppService.RemoverAnotacao(anotacao))
                    {
                        ViewModel.RemoverAnotacao(PK_Anotacao);
                    }

                    anotacao = null;
                }
            }
            catch (Exception ex)
            {
                await Mensagem.ExibirErroAsync(this.Content.XamlRoot, ex.Message);
            }
        }
        private async void btnAdicionarComoTarefa_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is MenuFlyoutItem btn && btn.Tag is int PK_Anotacao)
                {
                    anotacao = ViewModel.ObterAnotacao(PK_Anotacao);
                    if (anotacao == null)
                        return;

                    anotacaoAppService.AdicionarAnotacaoEmTarefas(anotacao);
                    anotacao = null;
                }
            }
            catch (Exception ex)
            {
                await Mensagem.ExibirErroAsync(this.Content.XamlRoot, ex.Message);
            }
        }
        private async void btnOrdenarAnotacoes_Click(object sender, RoutedEventArgs e)
        {
            await OrdenarAnotacoesAsync();
        }
        #endregion

        #region Métodos
        private void CarregarAnotacoes()
        {
            ViewModel.LimparAnotacoes();

            var request = new Anotacao_Request
            {
                Descricao = "",
                TipoPesquisa = ObterTipoPesquisaSelecionada(),
                FK_Categoria = -1
            };

            if (cboCategoriaExibicao.SelectedValue != null)
                request.FK_Categoria = (int)cboCategoriaExibicao.SelectedValue;

            IEnumerable<Anotacao> anotacoes = new List<Anotacao>();
            eTipoOrdenacaoAnotacao tipoOrdenacao = ObterTipoOrdenacaoSelecionada();

            if (tipoOrdenacao == eTipoOrdenacaoAnotacao.Data)
            {
                anotacoes = anotacaoAppService.PesquisarPorDescricao(request)
                    .OrderBy(i => i.DataCriacao)
                    .ToList();
            }
            else if (tipoOrdenacao == eTipoOrdenacaoAnotacao.Categoria)
            {
                anotacoes = anotacaoAppService.PesquisarPorDescricao(request)
                    .OrderBy(i => i.Categoria?.Nome)
                    .ToList();
            }
            else if (tipoOrdenacao == eTipoOrdenacaoAnotacao.Anotacao)
            {
                anotacoes = anotacaoAppService.PesquisarPorDescricao(request)
                    .OrderBy(i => i.Descricao)
                    .ToList();
            }

            foreach (var item in anotacoes)
                ViewModel.AdicionarAnotacao(item);
        }
        private void CarregarDropDown()
        {
            CarregarTipoDePesquisa();
            CarregarTipoDeOrdenacao();
            CarregarCategorias();
            CarregarCategoriasExibicao();
        }
        private void CarregarTipoDePesquisa()
        {
            var tipoDePesqusia = new List<Item>();

            foreach (eTipoPesquisaAnotacao item in Enum.GetValues(typeof(eTipoPesquisaAnotacao)))
                tipoDePesqusia.Add(new Item { ID = ((int)item).ToString(), Valor = item.ObterDescricao() });

            this.cboTipoDePesquisa.ItemsSource = tipoDePesqusia;
            this.cboTipoDePesquisa.SelectedValuePath = "ID";
            this.cboTipoDePesquisa.DisplayMemberPath = "Valor";
            this.cboTipoDePesquisa.SelectedValue = ((int)eTipoPesquisaAnotacao.Tudo).ToString();
        }
        private void CarregarTipoDeOrdenacao()
        {
            var tipoDePesqusia = new List<Item>();

            foreach (eTipoOrdenacaoAnotacao item in Enum.GetValues(typeof(eTipoOrdenacaoAnotacao)))
                tipoDePesqusia.Add(new Item { ID = ((int)item).ToString(), Valor = item.ObterDescricao() });

            this.cboTipoDeOrdenacao.ItemsSource = tipoDePesqusia;
            this.cboTipoDeOrdenacao.SelectedValuePath = "ID";
            this.cboTipoDeOrdenacao.DisplayMemberPath = "Valor";
            this.cboTipoDeOrdenacao.SelectedValue = ((int)eTipoOrdenacaoAnotacao.Categoria).ToString();
        }
        private void CarregarCategorias()
        {
            List<Categoria> categorias = new List<Categoria>()
            {
                new Categoria { PK_Categoria = 0, Nome = "Nenhuma" }
            };

            var ret = categoriaRepository.ObterLista().OrderBy(i => i.Nome).ToList();
            categorias.AddRange(ret);

            this.cboCategoria.ItemsSource = categorias.Select(i => new
            {
                PK_Categoria = i.PK_Categoria,
                Nome = i.Nome,
                Cor_SolidColorBrush = ObterCorCategorias(i.Cor)
            })
                .ToList();

            this.cboCategoria.SelectedValuePath = "PK_Categoria";
            this.cboCategoria.SelectedIndex = 0;
        }

        private void CarregarCategoriasExibicao()
        {
            List<Categoria> categorias = new List<Categoria>()
            {
                new Categoria { PK_Categoria = -1, Nome = "Tudo" },
                new Categoria { PK_Categoria = 0, Nome = "Sem Categorias" }
            };

            var ret = categoriaRepository.ObterLista().OrderBy(i => i.Nome).ToList();

            categorias.AddRange(ret);

            this.cboCategoriaExibicao.ItemsSource = categorias.Select(i => new
            {
                PK_Categoria = i.PK_Categoria,
                Nome = i.Nome,
                Cor_SolidColorBrush = ObterCorCategorias(i.Cor)
            })
                .ToList();

            this.cboCategoriaExibicao.SelectedValuePath = "PK_Categoria";
            this.cboCategoriaExibicao.SelectedValue = -1;
        }
        private SolidColorBrush ObterCorCategorias(Cor cor)
        {
            if (cor == null)
                return new SolidColorBrush(Colors.Transparent);

            return cor.Cor_SolidColorBrush;
        }
        private void Limpar()
        {
            this.anotacao = null;

            txtPesquisaAnotacao.Text = "";
            txtAnotacao.Text = "";
            this.ViewModel.MensagemAviso = "";
            this.cboCategoria.SelectedIndex = 0;
            this.cboCategoriaExibicao.SelectedValue = -1;
            this.cboTipoDeOrdenacao.SelectedValue = ((int)eTipoOrdenacaoAnotacao.Categoria).ToString();
            this.cboTipoDePesquisa.SelectedValue = ((int)eTipoPesquisaAnotacao.Tudo).ToString();
        }
        private async Task PesquisarAnotacoesAsync()
        {
            try
            {
                _cancellationTokenSource?.Cancel();
                _cancellationTokenSource = new CancellationTokenSource();

                if (txtPesquisaAnotacao.Text.ObterValorOuPadrao("").Trim() == "")
                {
                    CarregarAnotacoes();
                    return;
                }

                await Task.Delay(_delayPesquisa, _cancellationTokenSource.Token);

                if (!_cancellationTokenSource.IsCancellationRequested)
                {
                    ViewModel.LimparAnotacoes();

                    var request = new Anotacao_Request
                    {
                        Descricao = txtPesquisaAnotacao.Text,
                        TipoPesquisa = ObterTipoPesquisaSelecionada(),
                        FK_Categoria = (int)cboCategoriaExibicao.SelectedValue
                    };

                    var ret = anotacaoAppService.PesquisarPorDescricao(request).ToList();
                    foreach (var item in ret)
                        ViewModel.AdicionarAnotacao(item);

                    direcaoOrdenacao = eDirecaoOrdenacao.Ascendente;
                    await OrdenarAnotacoesAsync();
                }
            }
            catch (TaskCanceledException tc)
            {
            }
            catch (Exception ex)
            {
                await Mensagem.ExibirErroAsync(this.Content.XamlRoot, ex.Message);
            }
        }
        private async Task OrdenarAnotacoesAsync()
        {
            try
            {
                if (direcaoOrdenacao == eDirecaoOrdenacao.Ascendente)
                {
                    OrdenarAsc();
                    direcaoOrdenacao = eDirecaoOrdenacao.Descendente;
                }
                else if (direcaoOrdenacao == eDirecaoOrdenacao.Descendente)
                {
                    OrdenarDesc();
                    direcaoOrdenacao = eDirecaoOrdenacao.Ascendente;
                }
            }
            catch (Exception ex)
            {
                await Mensagem.ExibirErroAsync(this.Content.XamlRoot, ex.Message);
            }
        }
        private void OrdenarAsc()
        {
            IEnumerable<AnotacaoViewModel> anotacoesOrdenadas = new List<AnotacaoViewModel>();
            eTipoOrdenacaoAnotacao tipoOrdenacao = ObterTipoOrdenacaoSelecionada();

            if (tipoOrdenacao == eTipoOrdenacaoAnotacao.Data)
            {
                anotacoesOrdenadas = ViewModel.Anotacoes
                    .OrderBy(i => i.Anotacao.DataCriacao)
                    .ThenBy(i => i.Anotacao.Categoria?.Nome)
                    .ThenBy(i => i.Anotacao.Descricao)
                    .ToList();
            }
            else if (tipoOrdenacao == eTipoOrdenacaoAnotacao.Categoria)
            {
                anotacoesOrdenadas = ViewModel.Anotacoes
                    .OrderBy(i => i.Anotacao.Categoria?.Nome)
                    .ThenBy(i => i.Anotacao.Descricao)
                    .ToList();
            }
            else if (tipoOrdenacao == eTipoOrdenacaoAnotacao.Anotacao)
            {
                anotacoesOrdenadas = ViewModel.Anotacoes
                    .OrderBy(i => i.Anotacao.Descricao)
                    .ThenBy(i => i.Anotacao.Categoria?.Nome)
                    .ToList();
            }

            ViewModel.LimparAnotacoes();

            foreach (var item in anotacoesOrdenadas)
                ViewModel.AdicionarAnotacao(item);
        }
        private void OrdenarDesc()
        {
            IEnumerable<AnotacaoViewModel> anotacoesOrdenadas = new List<AnotacaoViewModel>();
            eTipoOrdenacaoAnotacao tipoOrdenacao = ObterTipoOrdenacaoSelecionada();

            if (tipoOrdenacao == eTipoOrdenacaoAnotacao.Data)
            {
                anotacoesOrdenadas = ViewModel.Anotacoes
                    .OrderByDescending(i => i.Anotacao.DataCriacao)
                    .ThenByDescending(i => i.Anotacao.Categoria?.Nome)
                    .ThenByDescending(i => i.Anotacao.Descricao)
                    .ToList();
            }
            else if (tipoOrdenacao == eTipoOrdenacaoAnotacao.Categoria)
            {
                anotacoesOrdenadas = ViewModel.Anotacoes
                    .OrderByDescending(i => i.Anotacao.Categoria?.Nome)
                    .ThenByDescending(i => i.Anotacao.Descricao)
                    .ToList();
            }
            else if (tipoOrdenacao == eTipoOrdenacaoAnotacao.Anotacao)
            {
                anotacoesOrdenadas = ViewModel.Anotacoes
                    .OrderByDescending(i => i.Anotacao.Descricao)
                    .ThenByDescending(i => i.Anotacao.Categoria?.Nome)
                    .ToList();
            }

            ViewModel.LimparAnotacoes();

            foreach (var item in anotacoesOrdenadas)
                ViewModel.AdicionarAnotacao(item);
        }

        private eTipoOrdenacaoAnotacao ObterTipoOrdenacaoSelecionada()
        {
            eTipoOrdenacaoAnotacao tipoOrdenacao = eTipoOrdenacaoAnotacao.Anotacao;

            if (cboTipoDeOrdenacao.SelectedValue != null)
            {
                var item = cboTipoDeOrdenacao.SelectedItem as Item;

                if (item != null)
                    tipoOrdenacao = (eTipoOrdenacaoAnotacao)item.ID.ConverterParaInt32();
            }

            return tipoOrdenacao;
        }
        private eTipoPesquisaAnotacao ObterTipoPesquisaSelecionada()
        {
            eTipoPesquisaAnotacao tipoPesquisa = eTipoPesquisaAnotacao.Anotacao;

            if (cboTipoDePesquisa.SelectedValue != null)
            {
                var item = cboTipoDePesquisa.SelectedItem as Item;

                if (item != null)
                    tipoPesquisa = (eTipoPesquisaAnotacao)item.ID.ConverterParaInt32();
            }

            return tipoPesquisa;
        }
        #endregion

        #region Métodos Públicos
        public void Salvar()
        {
            try
            {
                var PK_CategoriaSelecionada = (int)this.cboCategoria.SelectedValue;

                if (anotacao != null && anotacao.PK_Anotacao > 0)
                {
                    anotacao.Ativo = true;
                    anotacao.FK_Categoria = PK_CategoriaSelecionada;
                    anotacao.Descricao = txtAnotacao.Text.Trim();
                }
                else
                {
                    anotacao = new Anotacao()
                    {
                        Ativo = true,
                        FK_Categoria = PK_CategoriaSelecionada > 0 ? PK_CategoriaSelecionada : null,
                        Descricao = txtAnotacao.Text.Trim(),
                        DataCriacao = DateTime.Now,
                        PK_Anotacao = 0
                    };
                }

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

