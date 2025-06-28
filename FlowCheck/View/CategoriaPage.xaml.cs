using FlowCheck.Application;
using FlowCheck.Application.Interfaces;
using FlowCheck.Domain.Entidades;
using FlowCheck.Domain.Helpers;
using FlowCheck.Domain.Interfaces;
using FlowCheck.ViewModel.CategoriaViewModel;
using JJ.Net.Core.Extensoes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using System;
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
            try
            {
                byte r = (byte)rSlider.Value;
                byte g = (byte)gSlider.Value;
                byte b = (byte)bSlider.Value;

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

                var ret = categoriaAppService.SalvarCategoria(categoria);

                if (!categoria.ValidarResultado.EhValido)
                {
                    fecharDialog = false;
                    ViewModel.MensagemAviso = categoria.ValidarResultado.ObterPrimeiroErro();
                    return;
                }

                ViewModel.AdicionarCategoria(categoria);
                fecharDialog = true;
            }
            catch (Exception ex)
            {
                ViewModel.MensagemAviso = ex.Message;
            }
        }
        private void dialogCategoria_CloseButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            fecharDialog = true;
        }
        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Fechar();
        }
        #endregion

        #region Métodos
        private void CarregarCategorias()
        {
            var ret = categoriaAppService.Pesquisar(new Categoria_Request { Nome = "" });
            foreach (var item in ret)
                ViewModel.AdicionarCategoria(item);
        }
        private void Limpar()
        {
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
            // Implementação existente
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