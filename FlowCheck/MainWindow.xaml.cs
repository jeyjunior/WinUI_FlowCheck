using System;
using WinRT.Interop;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using JJ.Net.Core.Extensoes;
using FlowCheck.Application;
using FlowCheck.Domain.Enumerador;
using FlowCheck.Domain.Helpers;
using FlowCheck.Domain.Interfaces;
using FlowCheck.View;
using FlowCheck.ViewModel.CategoriaViewModel;
using FlowCheck.ViewModel.MainViewModel;

namespace FlowCheck
{
    public sealed partial class MainWindow : Window
    {
        #region Propriedades
        private const int Largura = 600;
        private const int Altura = 700;
        private AppWindow m_AppWindow;
        private Type paginaAtiva;
        private MainViewModel ViewModel;
        #endregion

        #region Construtor
        public MainWindow()
        {
            this.InitializeComponent();

            DefinirPadraoUI();
            
            ViewModel = new MainViewModel();
            this.RootGrid.DataContext = ViewModel;
            
            Configuracao.Iniciar();

            this.Closed += MainWindow_Closed;
            CarregarTelaTarefa();
        }
        #endregion

        #region Eventos
        private void MainWindow_Closed(object sender, WindowEventArgs args)
        {
            try
            {
                if (MainFrame.Content is IPageComandos pagina)
                    pagina.Salvar();
            }
            catch
            {

            }
        }
        private void BtnTarefas_Click(object sender, RoutedEventArgs e)
        {
            CarregarTelaTarefa();
        }
        private void BtnAnotacoes_Click(object sender, RoutedEventArgs e)
        {
            CarregarTelaAnotacao();
        }
        private void btnCategoria_Click(object sender, RoutedEventArgs e)
        {
            CarregarTelaCategoria();
        }
        private async void btnAdicionar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (MainFrame.Content is IPageComandos pagina)
                    pagina.Adicionar();
            }
            catch (Exception ex)
            {
                await Mensagem.ExibirErroAsync(this.Content.XamlRoot, $"Erro ao tentar adicionar a tarefa: \n{ex.Message}");
            }
        }
        private async void btnArquivarTudo_Click(object sender, RoutedEventArgs e)
        {

        }
        private async void btnExcluirTudo_Click(object sender, RoutedEventArgs e)
        {
            if (MainFrame.Content is IPageItensComandos pagina)
            {
                if (!pagina.ExisteItensSelecionados())
                {
                    await Mensagem.ExibirInformacaoAsync(this.Content.XamlRoot, "Nenhum item selecionado.");
                    return;
                }

                var resultado = await Mensagem.ExibirConfirmacaoAsync(this.Content.XamlRoot, "Tem certeza que deseja excluir as tarefas selecionadas?\nEsta ação não poderá ser desfeita.");

                if (resultado == eTipoMensagemResultado.Sim)
                    pagina.ExcluirItensSelecionados();
            }
        }
        private async void chkTodos_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                if (MainFrame.Content is IPageItensComandos pagina)
                    pagina.SelecionarTudo(chkTodos.IsChecked.ObterValorOuPadrao(false));
            }
            catch (Exception ex)
            {
                await Mensagem.ExibirErroAsync(this.Content.XamlRoot, $"Erro ao tentar marcar/desmarcar os itens: \n{ex.Message}");
            }
        }
        #endregion

        #region Metodos
        private void DefinirPadraoUI()
        {
            m_AppWindow = ObterAppWindowAtual();
            m_AppWindow.Title = "FlowCheck";
            m_AppWindow.SetIcon("Assets/flowcheck_icone_24.ico");

            DefinirTamanhoUI();
            CentralizarUI();
        }
        private void DefinirTamanhoUI()
        {
            m_AppWindow.Resize(new Windows.Graphics.SizeInt32(Largura, Altura));
        }
        private void CentralizarUI()
        {
            var displayArea = DisplayArea.GetFromWindowId(m_AppWindow.Id, DisplayAreaFallback.Primary);
            var centralizacao = new Windows.Graphics.PointInt32
            {
                X = displayArea.WorkArea.X + (displayArea.WorkArea.Width - Largura) / 2,
                Y = displayArea.WorkArea.Y + (displayArea.WorkArea.Height - Altura) / 2
            };
            m_AppWindow.Move(centralizacao);
        }
        private AppWindow ObterAppWindowAtual()
        {
            IntPtr hWnd = WindowNative.GetWindowHandle(this);
            WindowId wndId = Win32Interop.GetWindowIdFromWindow(hWnd);
            return AppWindow.GetFromWindowId(wndId);
        }
        private void CarregarPagina(Type pagina, Button botao)
        {
            if (paginaAtiva?.Name == pagina.Name)
                return;

            MainFrame.Navigate(pagina);
            paginaAtiva = pagina;

            SetActiveButton(botao);
        }
        private void SetActiveButton(Button activeButton)
        {
            btnTarefa.Opacity = 1;
            btnAnotacao.Opacity = 1;

            activeButton.Opacity = 0.6;
        }
        private void CarregarTelaTarefa()
        {
            this.ViewModel.ExibirBotoesAcao = Visibility.Visible;
            CarregarPagina(typeof(TarefaPage), btnTarefa);
        }
        private void CarregarTelaAnotacao()
        {
            this.ViewModel.ExibirBotoesAcao = Visibility.Collapsed;
            CarregarPagina(typeof(AnotacaoPage), btnAnotacao);
        }
        private void CarregarTelaCategoria()
        {
            this.ViewModel.ExibirBotoesAcao = Visibility.Collapsed;
            CarregarPagina(typeof(CategoriaPage), btnCategoria);
        }
        #endregion
    }
}
