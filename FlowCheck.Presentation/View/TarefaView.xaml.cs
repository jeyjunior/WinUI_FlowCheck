using FlowCheck.ViewModel.TarefaView;
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

namespace FlowCheck.Presentation.View
{
    public sealed partial class TarefaView : Page
    {
        public TarefaViewModel ViewModel { get; set; }

        public TarefaView()
        {
            this.InitializeComponent();

            ViewModel = new TarefaViewModel();
            this.DataContext = ViewModel;
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

        private void btnExcluirTarefa_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
