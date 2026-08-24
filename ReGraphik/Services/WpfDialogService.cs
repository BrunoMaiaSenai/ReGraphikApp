using Microsoft.Win32;
using ReGraphik.Services.Interface;
using ReGraphik.Views;
using System.Windows;

namespace ReGraphik.Services
{
    /// <summary>
    ///  Serviço de diálogo para aplicações WPF, implementando a interface IDialogService.
    /// </summary>
    public class WpfDialogService : IDialogService
    {
        public string? SalvarArquivo(string titulo, string filtro, string nomePadrao)
        {
            var dialog = new SaveFileDialog
            {
                Title = titulo,
                Filter = filtro,
                FileName = nomePadrao,
                DefaultExt = ".pdf"
            };

            return dialog.ShowDialog() == true ? dialog.FileName : null;
        }

        public bool ExibirConfirmacao(string titulo, string mensagem)
        {
            return Application.Current?.Dispatcher.Invoke(() =>
            {
                bool? resultado = MensagemPdfWindow.Exibir(titulo, mensagem, MensagemPdfWindow.TipoMensagem.Confirmacao);
                return resultado == true;
            }) ?? false;
        }

        public void ExibirErro(string titulo, string mensagem)
        {
            Application.Current?.Dispatcher.Invoke(() =>
            {
                MensagemPdfWindow.Exibir(titulo, mensagem, MensagemPdfWindow.TipoMensagem.Erro);
            });
        }
    }
}