using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
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
using JJ.Net.Core.Validador;
using FlowCheck.Application;
using FlowCheck.Application.Interfaces;
using FlowCheck.Domain.Entidades;
using FlowCheck.Domain.Enumerador;
using FlowCheck.Domain.Interfaces;
using FlowCheck.InfraData.Repository;
using FlowCheck.ViewModel.TarefaView;
using System.Threading.Tasks;

namespace FlowCheck.Presentation.View
{
    public sealed partial class TarefaPage : Page
    {
        #region Interfaces
        private readonly IParametroRepository parametroRepository;
        private readonly ITarefaRepository tarefaRepository;
        private readonly ITarefaAppService tarefaAppService;
        private readonly IParametroAppService parametroAppService;
        #endregion
        
        #region Propriedades Públicas
        public TarefaPageViewModel ViewModel { get; set; }
        #endregion

        #region Construtor
        public TarefaPage()
        {
            this.InitializeComponent();

            ViewModel = new TarefaPageViewModel();
            this.DataContext = ViewModel;

            parametroRepository = Bootstrap.ServiceProvider.GetRequiredService<IParametroRepository>();
            tarefaRepository = Bootstrap.ServiceProvider.GetRequiredService<ITarefaRepository>();
            tarefaAppService = Bootstrap.ServiceProvider.GetRequiredService<ITarefaAppService>();
            parametroAppService = Bootstrap.ServiceProvider.GetRequiredService<IParametroAppService>();
        }
        #endregion
        
        #region Eventos
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                CarregarParametros();
                CarregarTarefas();
            }
            catch (Exception ex)
            {

            }
        }
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            try
            {
                base.OnNavigatedFrom(e);
                SalvarParametros();
                SalvarTarefas();
            }
            catch (Exception ex)
            {

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
            txtTituloTarefa.Focus( FocusState.Keyboard);
            txtTituloTarefa.SelectAll();
        }
        private void btnAdicionarTarefa_Click(object sender, RoutedEventArgs e)
        {
            txtNovaTarefa.Text =  "";
            var result = AdicionarTarefaDialog.ShowAsync();

            if (result.GetResults() == ContentDialogResult.Primary && !string.IsNullOrWhiteSpace(txtNovaTarefa.Text))
            {
                var novaTarefa = new Tarefa
                {
                    Descricao = txtNovaTarefa.Text.Trim(),
                    Concluido = false,
                    IndiceExibicao = ViewModel.Tarefas.Count
                };

                ViewModel.AdicionarTarefa(novaTarefa);
            }
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

            }
        }
        private void txbTarefa_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            try
            {
                if (sender is TextBlock txb && txb.Tag is string IDGenerico)
                {
                    ViewModel.EditarTarefa(IDGenerico, true);
                }
            }
            catch (Exception ex)
            {

            }
        }
        private void btnRemoverTarefa_Click(object sender, RoutedEventArgs e)
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

            }
        }
        private void AdicionarTarefaDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            if (string.IsNullOrWhiteSpace(txtNovaTarefa.Text))
            {
                args.Cancel = true; 
            }
        }
        #endregion

        #region Métodos
        private void CarregarTarefas()
        {
            var ret = tarefaRepository.ObterLista();

            foreach (var item in ret)
                ViewModel.AdicionarTarefa(item);
        }
        private void CarregarParametros()
        {
            var parametro = parametroRepository.ObterLista("Nome = @Nome", new { Nome = eParametro.TituloTarefa.ToString() }).FirstOrDefault();

            if (parametro != null)
                ViewModel.Parametro = parametro;
        }

        private void SalvarParametros()
        {
            var parametro_AppServiceRequest = new Parametro_AppServiceRequest
            {
                Parametros = new List<Parametro>() { ViewModel.Parametro },
                ValidarResultado = new ValidarResultado()
            };

            parametroAppService.SalvarParametros(parametro_AppServiceRequest);

            if (!parametro_AppServiceRequest.ValidarResultado.EhValido)
            {
                // Mensagem com erros?
            }
        }
        private void SalvarTarefas()
        {
            var tarefa_AppServiceRequest = new Tarefa_AppServiceRequest
            {
                Tarefas = ViewModel.Tarefas.Select(i => i.Tarefa).ToList(),
                ValidarResultado = new ValidarResultado()
            };

            tarefaAppService.SalvarTarefas(tarefa_AppServiceRequest);

            if (!tarefa_AppServiceRequest.ValidarResultado.EhValido)
            {
                // Mensagem com erros?
            }
        }
        #endregion
    }
}
