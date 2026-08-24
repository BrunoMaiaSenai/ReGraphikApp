using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReGraphik.Services.Interface
{
    /// <summary>
    /// Interface que define os métodos para o serviço de diálogo, incluindo salvar arquivos, exibir confirmações e exibir erros.
    /// </summary>
    public interface IDialogService
    {
        string? SalvarArquivo(string titulo, string filtro, string nomePadrao);
        bool ExibirConfirmacao(string titulo, string mensagem);
        void ExibirErro(string titulo, string mensagem);
    }
}
