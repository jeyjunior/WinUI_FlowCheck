using FlowCheck.Domain.Enumerador;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowCheck.Domain.Helpers
{
    public static class Mensagem
    {
        public static async Task<eTipoMensagemResultado> ExibirAsync(MensagemRequest mensagemRequest)
        {
            if (mensagemRequest == null)
                return eTipoMensagemResultado.Nenhum;

            var dialog = new ContentDialog
            {
                Title = mensagemRequest.Titulo,
                Content = mensagemRequest.Mensagem,
                XamlRoot = mensagemRequest.XamlRoot,
                DefaultButton = ContentDialogButton.Primary
            };

            switch (mensagemRequest.TipoMensagem)
            {
                case eTipoMensagem.Informacao:
                    dialog.PrimaryButtonText = "OK";
                    dialog.RequestedTheme = ElementTheme.Dark;
                    break;

                case eTipoMensagem.Confirmacao:
                    dialog.PrimaryButtonText = "Sim";
                    dialog.CloseButtonText = "Não";
                    dialog.DefaultButton = ContentDialogButton.Primary;
                    break;

                case eTipoMensagem.Aviso:
                    dialog.PrimaryButtonText = "OK";
                    dialog.RequestedTheme = ElementTheme.Dark;
                    break;

                case eTipoMensagem.Erro:
                    dialog.PrimaryButtonText = "OK";
                    dialog.RequestedTheme = ElementTheme.Dark;
                    break;
            }

            var result = await dialog.ShowAsync();

            return mensagemRequest.TipoMensagem switch
            {
                eTipoMensagem.Confirmacao => result == ContentDialogResult.Primary
                    ? eTipoMensagemResultado.Sim
                    : eTipoMensagemResultado.Cancelar,
                _ => eTipoMensagemResultado.OK
            };
        }

        public static async Task<eTipoMensagemResultado> ExibirConfirmacaoAsync(XamlRoot xamlRoot, string mensagem, string titulo = "Confirmação")
        {
            return await ExibirAsync(new MensagemRequest { Titulo = titulo, Mensagem = mensagem, XamlRoot = xamlRoot, TipoMensagem = eTipoMensagem.Confirmacao });
        }
        public static async Task<eTipoMensagemResultado> ExibirErroAsync(XamlRoot xamlRoot, string mensagem, string titulo = "Erro")
        {
            return await ExibirAsync(new MensagemRequest { Titulo = titulo, Mensagem = mensagem, XamlRoot = xamlRoot, TipoMensagem = eTipoMensagem.Erro });
        }
        public static async Task<eTipoMensagemResultado> ExibirAvisoAsync(XamlRoot xamlRoot, string mensagem, string titulo = "Aviso")
        {
            return await ExibirAsync(new MensagemRequest { Titulo = titulo, Mensagem = mensagem, XamlRoot = xamlRoot, TipoMensagem = eTipoMensagem.Aviso });
        }
        public static async Task<eTipoMensagemResultado> ExibirInformacaoAsync(XamlRoot xamlRoot, string mensagem, string titulo = "Informação")
        {
            return await ExibirAsync(new MensagemRequest { Titulo = titulo, Mensagem = mensagem, XamlRoot = xamlRoot, TipoMensagem = eTipoMensagem.Informacao });
        }
    }

    public class MensagemRequest
    {
        public string Titulo { get; set; }
        public string Mensagem { get; set; }
        public XamlRoot XamlRoot { get; set; }
        public eTipoMensagem TipoMensagem { get; set; }
    }
}