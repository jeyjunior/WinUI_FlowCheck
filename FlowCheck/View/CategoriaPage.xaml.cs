using FlowCheck.Application;
using FlowCheck.Application.Interfaces;
using FlowCheck.Application.Services;
using FlowCheck.Domain.Entidades;
using FlowCheck.Domain.Enumerador;
using FlowCheck.Domain.Helpers;
using FlowCheck.Domain.Interfaces;
using FlowCheck.ViewModel.CategoriaViewModel;
using FlowCheck.ViewModel.TarefaViewModel;
using JJ.Net.Core.Extensoes;
using JJ.Net.Core.Validador;
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
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;

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
        public void Salvar()
        {
            
        }

        public async void Adicionar()
        {
            var _categoria = new Categoria();

            ColorTextBox.Text = "#FF874CFC";
            btnCor.Background = (SolidColorBrush)Microsoft.UI.Xaml.Application.Current.Resources["Roxo"];

            var result = await AddCategoryDialog.ShowAsync();

            if (result != ContentDialogResult.Primary)
                return;

            _categoria.Nome = CategoryTextBox.Text.ObterValorOuPadrao("").Trim();
            string cor = ColorTextBox.Text;

            var _cor = new Cor()
            {
                Nome = "Cor",
                Hexadecimal = cor,
                RGB = ColorFromHex(cor).ToString()
            };

            _categoria.Cor = _cor;

            categoriaAppService.SalvarCategoria(_categoria);

            if (!_categoria.ValidarResultado.EhValido)
                return;

            this.ViewModel.AdicionarCategoria(_categoria);
        }

        private Windows.UI.Color ColorFromHex(string hex)
        {
            hex = hex.Replace("#", "");

            if (hex.Length == 6)
                hex = "FF" + hex; // Adiciona alpha se não existir

            var a = (byte)Convert.ToUInt32(hex.Substring(0, 2), 16);
            var r = (byte)Convert.ToUInt32(hex.Substring(2, 2), 16);
            var g = (byte)Convert.ToUInt32(hex.Substring(4, 2), 16);
            var b = (byte)Convert.ToUInt32(hex.Substring(6, 2), 16);

            return Windows.UI.Color.FromArgb(a, r, g, b);
        }
        #endregion
    }
}
