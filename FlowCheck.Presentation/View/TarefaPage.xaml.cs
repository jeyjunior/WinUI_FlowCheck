using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using FlowCheck.ViewModel.TarefaView;
using System.Text;
using FlowCheck.Domain.Interfaces;
using FlowCheck.Application;
using Microsoft.Extensions.DependencyInjection;

namespace FlowCheck.Presentation.View
{
    public sealed partial class TarefaPage : Page
    {
        private readonly ITarefaRepository tarefaRepository;
        public TarefaPageViewModel ViewModel { get; set; }

        public TarefaPage()
        {
            this.InitializeComponent();

            ViewModel = new TarefaPageViewModel();
            this.DataContext = ViewModel;
            tarefaRepository = Bootstrap.ServiceProvider.GetRequiredService<ITarefaRepository>();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            foreach (var item in ViewModel.Tarefas)
                tarefaRepository.Atualizar(item.Tarefa);
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

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            txtTituloTarefa.Text = (Windows.Storage.ApplicationData.Current.LocalFolder).Path;

            var ret = tarefaRepository.ObterLista();

            foreach (var item in ret)
                ViewModel.AdicionarTarefa(item);

            // CarregarTeste();
        }

        private void CarregarTeste()
        {
            Random random = new Random();
            string[] palavrasLorem = new string[] { "Lorem", "ipsum", "dolor", "sit", "amet", "consectetur",
                "adipiscing", "elit", "sed", "do", "eiusmod", "tempor", "incididunt", "ut", "labore",
                "et", "dolore", "magna", "aliqua", "Ut", "enim", "ad", "minim", "veniam" };

            for (int i = 0; i < 10; i++)
            {
                int tamanhoDescricao = random.Next(20, 201);
                string descricao = GerarTextoAleatorio(palavrasLorem, tamanhoDescricao);
                bool concluido = random.Next(0, 2) == 0;

                ViewModel.AdicionarTarefa(new Domain.Entidades.Tarefa()
                {
                    Concluido = concluido,
                    Descricao = $"Tarefa {i + 1}: {descricao}",
                    IndiceExibicao = i
                });
            }
        }

        private void btnAdicionarTarefa_Click(object sender, RoutedEventArgs e)
        {

        }

        private void txtTarefa_LostFocus(object sender, RoutedEventArgs e)
        {

        }

        private void txbTarefa_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {

        }

        private void btnExibirTarefaAnotacao_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnOpcoes_Click(object sender, RoutedEventArgs e)
        {

        }

        private string GerarTextoAleatorio(string[] palavras, int tamanhoMaximo)
        {
            Random random = new Random();
            StringBuilder texto = new StringBuilder();

            while (texto.Length < tamanhoMaximo)
            {
                string palavra = palavras[random.Next(palavras.Length)];

                if (texto.Length + palavra.Length + 1 > tamanhoMaximo)
                    break;

                if (texto.Length > 0)
                    texto.Append(" ");

                texto.Append(palavra);

                // Adiciona pontuação ocasionalmente
                if (random.Next(0, 5) == 0 && texto.Length + 1 <= tamanhoMaximo) // ~20% de chance
                {
                    char[] pontuacao = new char[] { '.', ',', '!', '?' };
                    texto.Append(pontuacao[random.Next(pontuacao.Length)]);
                }
            }

            // Garante que a primeira letra seja maiúscula
            if (texto.Length > 0)
            {
                texto[0] = char.ToUpper(texto[0]);
            }

            // Adiciona ponto final se não terminar com pontuação
            if (texto.Length > 0 && !char.IsPunctuation(texto[texto.Length - 1]))
            {
                if (texto.Length + 1 <= tamanhoMaximo)
                {
                    texto.Append('.');
                }
            }

            return texto.ToString();
        }
    }
}
