using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.Extensions.DependencyInjection;
using Windows.ApplicationModel.DataTransfer;
using JJ.Net.Core.Validador;
using FlowCheck.Application;
using FlowCheck.Application.Interfaces;
using FlowCheck.Domain.DTO;
using FlowCheck.Domain.Entidades;
using FlowCheck.Domain.Enumerador;
using FlowCheck.Domain.Helpers;
using FlowCheck.Domain.Interfaces;
using FlowCheck.ViewModel.TarefaViewModel;


namespace FlowCheck.View
{
    public sealed partial class TarefaPage : Page, IPageComandos, IPageItensComandos
    {
        #region Interfaces
        private readonly ITarefaAppService tarefaAppService;
        private readonly IParametroAppService parametroAppService;
        #endregion

        #region Propriedades
        private TarefaPageViewModel ViewModel { get; set; }
        private bool tarefaAdicionada = false;
        #endregion

        #region Construtor
        public TarefaPage()
        {
            this.InitializeComponent();

            ViewModel = new TarefaPageViewModel();
            this.DataContext = ViewModel;

            tarefaAppService = Bootstrap.ServiceProvider.GetRequiredService<ITarefaAppService>();
            parametroAppService = Bootstrap.ServiceProvider.GetRequiredService<IParametroAppService>();
        }
        #endregion

        #region Eventos
        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                CarregarParametros();
                CarregarTarefas();
            }
            catch (Exception ex)
            {
                await Mensagem.ExibirErroAsync(this.Content.XamlRoot, ex.Message);
            }
        }
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            try
            {
                base.OnNavigatedFrom(e);
                SalvarParametros();
                Salvar();
            }
            catch (Exception ex)
            {
                Mensagem.ExibirErroAsync(this.Content.XamlRoot, ex.Message);
            }
        }
        private void txtTituloTarefa_LostFocus(object sender, RoutedEventArgs e)
        {
            txbTituloTarefa.Visibility = Visibility.Visible;
            txtTituloTarefa.Visibility = Visibility.Collapsed;

        }
        private void txbTituloTarefa_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            txbTituloTarefa.Visibility = Visibility.Collapsed;
            txtTituloTarefa.Visibility = Visibility.Visible;
            txtTituloTarefa.Focus(FocusState.Keyboard);
            txtTituloTarefa.SelectAll();
        }
        private void txtTarefa_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is TextBox txt && txt.Tag is string IDGenerico)
                {
                    ViewModel.EditarTarefa(IDGenerico, false);
                }
            }
            catch (Exception ex)
            {
                Mensagem.ExibirErroAsync(this.Content.XamlRoot, ex.Message);
            }
        }
        private async void txbTarefa_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            try
            {
                if (sender is TextBlock txb && txb.Tag is string IDGenerico)
                {
                    ViewModel.EditarTarefa(IDGenerico, true);

                    var parent = VisualTreeHelper.GetParent(txb);
                    while (parent != null && !(parent is StackPanel))
                    {
                        parent = VisualTreeHelper.GetParent(parent);
                    }

                    if (parent is StackPanel container)
                    {
                        var textBox = FindVisualChild<TextBox>(container, "txtTarefa");
                        if (textBox != null)
                        {
                            textBox.Focus(FocusState.Programmatic);
                            textBox.SelectAll();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                await Mensagem.ExibirErroAsync(this.Content.XamlRoot, ex.Message);
            }
        }
        private async void btnRemoverTarefa_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is MenuFlyoutItem btn && btn.Tag is string IDGenerico)
                {
                    var tarefa = ViewModel.ObterTarefa(IDGenerico);
                    if (tarefa == null)
                        return;

                    if (tarefaAppService.RemoverTarefa(tarefa.Tarefa))
                    {
                        ViewModel.RemoverTarefa(IDGenerico);
                    }
                }

            }
            catch (Exception ex)
            {
                await Mensagem.ExibirErroAsync(this.Content.XamlRoot, ex.Message);
            }
        }
        private void TxtTarefaAnotacao_PreviewKeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Tab)
            {
                var textBox = sender as TextBox;
                if (textBox != null)
                {
                    int pos = textBox.SelectionStart;
                    textBox.Text = textBox.Text.Insert(pos, "\t");
                    textBox.SelectionStart = pos + 1;
                    e.Handled = true;
                }
            }
        }
        private void scroll_LayoutUpdated(object sender, object e)
        {
            if (tarefaAdicionada)
            {
                tarefaAdicionada = false;
                scroll.ChangeView(null, scroll.ScrollableHeight, null);
            }
        }
        private void SpTarefa_DragStarting(object sender, DragStartingEventArgs e)
        {
            var stackPanel = sender as StackPanel;
            var tarefaVM = stackPanel.DataContext as TarefaViewModel;

            e.Data.Properties.Add("tarefa", tarefaVM);
            e.Data.Properties.Add("indiceOriginal", ViewModel.Tarefas.IndexOf(tarefaVM));

            e.DragUI.SetContentFromDataPackage();
        }
        private void SpTarefa_DragOver(object sender, DragEventArgs e)
        {
            if (e.DataView.Properties.ContainsKey("tarefa"))
            {
                e.AcceptedOperation = DataPackageOperation.Move;

                var stackPanel = sender as StackPanel;
                stackPanel.Background = new SolidColorBrush(Windows.UI.Color.FromArgb(30, 0, 120, 215));
            }
        }
        private void SpTarefa_Drop(object sender, DragEventArgs e)
        {
            var targetPanel = sender as StackPanel;
            var targetTarefa = targetPanel.DataContext as TarefaViewModel;

            if (e.DataView.Properties.TryGetValue("tarefa", out object draggedItemObj) &&
                e.DataView.Properties.TryGetValue("indiceOriginal", out object originalIndexObj))
            {
                var draggedTarefa = draggedItemObj as TarefaViewModel;
                int originalIndex = (int)originalIndexObj;
                int targetIndex = ViewModel.Tarefas.IndexOf(targetTarefa);

                if (draggedTarefa != null && draggedTarefa != targetTarefa)
                    ViewModel.ReordenarTarefas(originalIndex, targetIndex, draggedTarefa);
            }

            targetPanel.Background = new SolidColorBrush(Colors.Transparent);
        }
        private void SpTarefa_DragLeave(object sender, DragEventArgs e)
        {
            var stackPanel = sender as StackPanel;
            stackPanel.Background = new SolidColorBrush(Colors.Transparent);
        }
        private void chkConcluido_CheckedStatus(object sender, RoutedEventArgs e)
        {
            this.ViewModel.AtualizarStatus();
        }
        #endregion

        #region Métodos
        private void CarregarTarefas()
        {
            var ret = tarefaAppService.Pesquisar(new Tarefa_Request { Arquivado = false });

            foreach (var item in ret)
                ViewModel.AdicionarTarefa(item);
        }
        private void CarregarParametros()
        {
            var parametro = parametroAppService.Pesquisar(new Parametro_Request { Nome = eParametro.TituloTarefa.ToString() });
            if (parametro != null)
                ViewModel.Parametro = parametro.FirstOrDefault();
        }
        private async void SalvarParametros()
        {
            var parametro_AppServiceRequest = new Parametro_AppServiceRequest
            {
                Parametros = new List<Parametro>() { ViewModel.Parametro },
                ValidarResultado = new ValidarResultado()
            };

            parametroAppService.SalvarParametros(parametro_AppServiceRequest);

            if (!parametro_AppServiceRequest.ValidarResultado.EhValido)
            {
                await Mensagem.ExibirErroAsync(this.Content.XamlRoot, parametro_AppServiceRequest.ValidarResultado.ObterPrimeiroErro());
            }
        }
        private void FocarTarefaNovaTextBox(TarefaViewModel tarefaViewModel)
        {
            var container = spPrincipal.ContainerFromItem(tarefaViewModel) as FrameworkElement;
            if (container != null)
            {
                var textBox = FindVisualChild<TextBox>(container, "txtTarefa");
                if (textBox != null)
                {
                    tarefaAdicionada = true;
                    ViewModel.EditarTarefa(tarefaViewModel.IDGenerico, true);
                    textBox.Focus(FocusState.Programmatic);
                    textBox.SelectAll();
                }
            }
        }
        private static T FindVisualChild<T>(DependencyObject parent, string name = null) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is T result && (name == null || (child is FrameworkElement fe && fe.Name == name)))
                    return result;

                var descendant = FindVisualChild<T>(child, name);
                if (descendant != null)
                    return descendant;
            }
            return null;
        }
        #endregion

        #region Métodos Público
        public void Adicionar()
        {
            var novaTarefa = new Tarefa
            {
                Descricao = "",
                Concluido = false,
                IndiceExibicao = ViewModel.Tarefas.Count
            };

            var tarefaViewModel = ViewModel.AdicionarTarefa(novaTarefa);

            FocarTarefaNovaTextBox(tarefaViewModel);
        }
        public void Salvar()
        {
            var tarefa_AppServiceRequest = new Tarefa_AppServiceRequest
            {
                Tarefas = ViewModel.Tarefas.Select(i => i.Tarefa).ToList(),
                ValidarResultado = new ValidarResultado()
            };

            tarefaAppService.SalvarTarefas(tarefa_AppServiceRequest);

            if (!tarefa_AppServiceRequest.ValidarResultado.EhValido)
            {
                Mensagem.ExibirErroAsync(this.Content.XamlRoot, tarefa_AppServiceRequest.ValidarResultado.ObterPrimeiroErro());
            }
        }
        public bool ExisteItensSelecionados()
        {
            return (this.ViewModel.Tarefas.Count(i => i.Concluido) > 0);
        }
        public async void ExcluirItensSelecionados()
        {
            try
            {
                List<TarefaViewModel> tarefaViewModelCollection = ViewModel.Tarefas.Where(i => i.Concluido).ToList();

                if (tarefaAppService.RemoverTarefas(tarefaViewModelCollection.Select(i => i.Tarefa)))
                    ViewModel.RemoverTarefas(tarefaViewModelCollection);
            }
            catch (Exception ex)
            {
                await Mensagem.ExibirErroAsync(this.Content.XamlRoot, ex.Message);
            }
        }
        public void SelecionarTudo(bool value)
        {
            this.ViewModel.TudoConcluido = value;
        }
        #endregion
    }
}
