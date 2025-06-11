using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI;           
using Microsoft.UI.Windowing;
using WinRT.Interop;
using FlowCheck.Presentation.View;
using FlowCheck.Application;
using FlowCheck.Application.Services;
using FlowCheck.Domain.Entidades;
using JJ.Net.Core.Validador;
using FlowCheck.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace FlowCheck.Presentation
{
    public sealed partial class MainWindow : Window
    {
        #region Propriedades
        private readonly ITarefaAppService tarefaAppService;

        private const int Largura = 600;
        private const int Altura = 600;
        private AppWindow m_AppWindow;
        private Type paginaAtiva;
        #endregion

        #region Construtor
        public MainWindow()
        {
            this.InitializeComponent();

            DefinirPadraoUI();

            Configuracao.Iniciar();

            tarefaAppService = Bootstrap.ServiceProvider.GetRequiredService<ITarefaAppService>();
            this.Closed += MainWindow_Closed;

            CarregarPagina(typeof(TarefaPage), btnTarefa);
        }
        #endregion

        #region Eventos
        private async void MainWindow_Closed(object sender, WindowEventArgs args)
        {
            try
            {
                if (MainFrame.Content is TarefaPage tarefaPage)
                {
                    var viewModel = tarefaPage.ViewModel;

                    var request = new Tarefa_AppServiceRequest
                    {
                        Tarefas = viewModel.Tarefas.Select(i => i.Tarefa).ToList(),
                        ValidarResultado = new ValidarResultado()
                    };

                    tarefaAppService.SalvarTarefas(request); 

                    if (!request.ValidarResultado.EhValido)
                    {
                        
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao salvar tarefas: {ex.Message}");
            }
        }
        private void BtnTarefas_Click(object sender, RoutedEventArgs e)
        {
            CarregarPagina(typeof(TarefaPage), btnTarefa);
        }
        private void BtnAnotacoes_Click(object sender, RoutedEventArgs e)
        {
            CarregarPagina(typeof(AnotacaoPage), btnAnotacao);
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
        #endregion
    }
}
