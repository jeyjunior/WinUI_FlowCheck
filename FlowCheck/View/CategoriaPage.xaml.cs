using FlowCheck.Application;
using FlowCheck.Application.Interfaces;
using FlowCheck.Application.Services;
using FlowCheck.Domain.Entidades;
using FlowCheck.Domain.Helpers;
using FlowCheck.Domain.Interfaces;
using FlowCheck.ViewModel.CategoriaViewModel;
using JJ.Net.Core.Extensoes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Client;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using System;
using System.Runtime.ConstrainedExecution;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI;

namespace FlowCheck.View
{
    public sealed partial class CategoriaPage : Page, IPageComandos, IDialogComandos
    {
        #region Interfaces
        private readonly ICategoriaAppService categoriaAppService;
        #endregion

        #region Propriedades
        private CategoriaPageViewModel ViewModel { get; set; }
        private Categoria categoria;
        private bool fecharDialog = true;
        private CancellationTokenSource _cancellationTokenSource;
        private readonly int _delayPesquisa = 500;
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
                await Mensagem.ExibirErroAsync(this.Content.XamlRoot, ex.Message);
            }
        }
        private void scroll_LayoutUpdated(object sender, object e)
        {

        }
        private async void btnRemoverCategoria_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var ret = await Mensagem.ExibirConfirmacaoAsync(this.Content.XamlRoot, "Tem certeza que deseja remover essa categoria? A operação não poderá ser desfeita.");
                if (ret != Domain.Enumerador.eTipoMensagemResultado.Sim)
                    return;

                if (sender is MenuFlyoutItem btn && btn.Tag is int PK_Categoria)
                {
                    categoria = ViewModel.ObterCategoria(PK_Categoria);
                    if (categoria == null)
                        return;

                    if (categoriaAppService.RemoverCategoria(categoria))
                    {
                        ViewModel.RemoverCategoria(PK_Categoria);
                    }

                    categoria = null;
                }
            }
            catch (Exception ex)
            {
                await Mensagem.ExibirErroAsync(this.Content.XamlRoot, ex.Message);
            }
        }
        private async void btnEditarCategoria_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Limpar();

                if (sender is MenuFlyoutItem btn && btn.Tag is int PK_Categoria)
                {
                    categoria = ViewModel.ObterCategoria(PK_Categoria);
                    if (categoria == null)
                        return;

                    if (categoria.Cor != null)
                    {
                        var rgb = categoria.Cor.RGB.Split(",");

                        if (rgb.Length == 3)
                        {
                            rSlider.Value = Convert.ToDouble(rgb[0]);
                            gSlider.Value = Convert.ToDouble(rgb[1]);
                            bSlider.Value = Convert.ToDouble(rgb[2]);
                        }
                    }

                    txtCategoria.Text = categoria.Nome;
                    txtCategoria.SelectAll();

                    await this.dialogCategoria.ShowAsync();
                }
            }
            catch (Exception ex)
            {
                await Mensagem.ExibirErroAsync(this.Content.XamlRoot, ex.Message);
            }
        }
        private void Slider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            byte r = (byte)rSlider.Value;
            byte g = (byte)gSlider.Value;
            byte b = (byte)bSlider.Value;
            frameColorPreview.Background = new SolidColorBrush(Color.FromArgb(255, r, g, b));
        }
        private void txtCategoria_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (ViewModel.MensagemAviso.ObterValorOuPadrao("").Trim() == "")
                return;

            ViewModel.MensagemAviso = "";
        }
        private void dialogCategoria_Closing(ContentDialog sender, ContentDialogClosingEventArgs args)
        {
            if (!fecharDialog)
                args.Cancel = true;
        }
        private void dialogCategoria_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            Salvar();
        }
        private void dialogCategoria_CloseButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            Fechar();
        }
        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Fechar();
        }
        private async void txtPesquisaCategoria_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            try
            {
                _cancellationTokenSource?.Cancel();
                _cancellationTokenSource = new CancellationTokenSource();

                if (txtPesquisaCategoria.Text.ObterValorOuPadrao("").Trim() == "")
                {
                    CarregarCategorias();
                    return;
                }

                await Task.Delay(_delayPesquisa, _cancellationTokenSource.Token);

                if (!_cancellationTokenSource.IsCancellationRequested)
                {
                    ViewModel.LimparCategorias();

                    var ret = categoriaAppService.Pesquisar(new Categoria_Request { Nome = txtPesquisaCategoria.Text });
                    foreach (var item in ret)
                        ViewModel.AdicionarCategoria(item);
                }
            }
            catch (TaskCanceledException)
            {

            }
        }
        #endregion

        #region Métodos
        private void CarregarCategorias()
        {
            ViewModel.Categorias.Clear();

            var ret = categoriaAppService.Pesquisar(new Categoria_Request { Nome = "", PesquisaPorIgualdade = true });
            foreach (var item in ret)
                ViewModel.AdicionarCategoria(item);
        }
        private void Limpar()
        {
            this.categoria = null;

            this.txtCategoria.Text = "";
            this.rSlider.Value = 0;
            this.gSlider.Value = 0;
            this.bSlider.Value = 0;

            this.ViewModel.MensagemAviso = "";
        }
        private string RgbToHex(byte r, byte g, byte b)
        {
            return $"#{r:X2}{g:X2}{b:X2}";
        }
        #endregion

        #region Métodos Públicos
        public void Salvar()
        {
            try
            {
                byte r = (byte)rSlider.Value;
                byte g = (byte)gSlider.Value;
                byte b = (byte)bSlider.Value;

                if (categoria != null && categoria.PK_Categoria > 0)
                {
                    categoria.Cor.Hexadecimal = RgbToHex(r, g, b);
                    categoria.Cor.RGB = $"{r},{g},{b}";
                    categoria.Cor.ValidarResultado = new JJ.Net.Core.Validador.ValidarResultado();

                    categoria.Nome = txtCategoria.Text.Trim();
                    categoria.ValidarResultado = new JJ.Net.Core.Validador.ValidarResultado();

                    if (!categoria.Validar())
                    {
                        fecharDialog = false;
                        ViewModel.MensagemAviso = categoria.ValidarResultado.ObterPrimeiroErro();
                        return;
                    }
                }
                else
                {
                    var cor = new Cor
                    {
                        Hexadecimal = RgbToHex(r, g, b),
                        Nome = "CorGenerica",
                        RGB = $"{r},{g},{b}",
                        PK_Cor = 0,
                        ValidarResultado = new JJ.Net.Core.Validador.ValidarResultado()
                    };

                    categoria = new Categoria
                    {
                        Nome = txtCategoria.Text.Trim(),
                        Cor = cor,
                        PK_Categoria = 0,
                        FK_Cor = 0,
                        ValidarResultado = new JJ.Net.Core.Validador.ValidarResultado()
                    };

                    if (!categoria.Validar())
                    {
                        fecharDialog = false;
                        ViewModel.MensagemAviso = categoria.ValidarResultado.ObterPrimeiroErro();
                        return;
                    }
                }

                var ret = categoriaAppService.SalvarCategoria(categoria);

                if (!categoria.ValidarResultado.EhValido)
                {
                    fecharDialog = false;
                    ViewModel.MensagemAviso = categoria.ValidarResultado.ObterPrimeiroErro();
                    return;
                }

                CarregarCategorias();

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

            dialogCategoria.XamlRoot = this.Content.XamlRoot; 
            dialogCategoria.HorizontalAlignment = HorizontalAlignment.Center;
            dialogCategoria.VerticalAlignment = VerticalAlignment.Center;

            await dialogCategoria.ShowAsync();
        }
        public void Fechar()
        {
            fecharDialog = true;
            dialogCategoria.Hide();
        }
        #endregion
    }
}