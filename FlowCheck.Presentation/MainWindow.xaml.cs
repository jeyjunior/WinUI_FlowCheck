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
using FlowCheck.Presentation.View;
using FlowCheck.Domain.DTO;

namespace FlowCheck.Presentation
{
    public sealed partial class MainWindow : Window
    {
        #region Propriedades
        private const int Largura = 600;
        private const int Altura = 600;
        private AppWindow m_AppWindow;
        private Type paginaAtiva;

        private eTelaEmExecucao telaEmExecucao = eTelaEmExecucao.Nenhuma;
        #endregion

        #region Construtor
        public MainWindow()
        {
            this.InitializeComponent();

            DefinirPadraoUI();

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
                if (MainFrame.Content is TarefaPage tarefaPage)
                {
                    if (tarefaPage == null)
                        return;

                    tarefaPage.SalvarTarefas();
                }
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

        private async void btnAdicionar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                switch (telaEmExecucao)
                {
                    case eTelaEmExecucao.Nenhuma:
                        break;
                    case eTelaEmExecucao.Tarefa:

                        if (MainFrame.Content is TarefaPage tarefaPage)
                        {
                            if (tarefaPage != null)
                                tarefaPage.AdicionarTarefa();
                        }

                        break;
                    case eTelaEmExecucao.Anotacao:
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                await Mensagem.ExibirErroAsync($"Erro ao tentar adicionar a tarefa: \n{ex.Message}", this.Content.XamlRoot);
            }
        }
        private void btnArquivarTudo_Click(object sender, RoutedEventArgs e)
        {

        }
        private async void btnExcluirTudo_Click(object sender, RoutedEventArgs e)
        {
            switch (telaEmExecucao)
            {
                case eTelaEmExecucao.Nenhuma:
                    break;
                case eTelaEmExecucao.Tarefa:
                    if (MainFrame.Content is TarefaPage tarefaPage)
                    {
                        if (tarefaPage == null)
                            return;

                        if (!tarefaPage.ExisteTarefasSelecionadas())
                        {
                            await Mensagem.ExibirInformacaoAsync("Selecione as tarefas a serem excluídas.", this.Content.XamlRoot);
                            return;
                        }

                        var resultado = await Mensagem.ExibirConfirmacaoAsync("Tem certeza que deseja excluir as tarefas selecionadas?\nEsta ação não poderá ser desfeita.", this.Content.XamlRoot);
                        if (resultado == eTipoMensagemResultado.Sim)
                            tarefaPage.ExcluirTarefasSelecionadas();
                    }
                    break;
                case eTelaEmExecucao.Anotacao:
                    break;
                default:
                    break;
            }
        }
        private async void chkTodos_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                switch (telaEmExecucao)
                {
                    case eTelaEmExecucao.Nenhuma:
                        break;
                    case eTelaEmExecucao.Tarefa:

                        if (MainFrame.Content is TarefaPage tarefaPage)
                        {
                            if (tarefaPage != null)
                                tarefaPage.SelecionarTarefas(chkTodos.IsChecked.ObterValorOuPadrao(false));
                        }

                        break;
                    case eTelaEmExecucao.Anotacao:
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                await Mensagem.ExibirErroAsync($"Erro ao tentar marcar/desmarcar as tarefas: \n{ex.Message}", this.Content.XamlRoot);
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
            CarregarPagina(typeof(TarefaPage), btnTarefa);
            telaEmExecucao = eTelaEmExecucao.Tarefa;
        }
        private void CarregarTelaAnotacao()
        {
            CarregarPagina(typeof(AnotacaoPage), btnAnotacao);
            telaEmExecucao = eTelaEmExecucao.Anotacao;
        }
        #endregion
    }
}
