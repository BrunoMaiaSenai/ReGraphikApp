using Firebase.Database.Query;
using ReGraphik.Models;
using ReGraphik.Services.Interface;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ReGraphik.Services
{
    /// <summary>
    /// Esta classe é responsável por lidar com a lógica relacionada aos resíduos, como obter a lista de resíduos do banco de dados.
    /// </summary>
    public class ResiduoService : IResiduoService
    {
        /// Usamos um HttpClient estático para reutilizar a mesma instância em toda a aplicação, evitando problemas de esgotamento de conexões.
        private static readonly HttpClient _httpClient = new HttpClient { BaseAddress = new Uri("https://webregraphik.runasp.net/api/") };

        /// Nó do Realtime Database espelhado pela API, usado pelo Estoque Reverso em tempo real.
        private const string NodeResiduos = "residuos";

        /// <summary>
        /// Método para obter todos os resíduos do banco de dados, fazendo uma requisição GET para a API.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="HttpRequestException"></exception>
        public async Task<List<Residuo>> ObterTodosResiduosAsync()
        {
            HttpResponseMessage response = await _httpClient.GetAsync("Residuo");

            if (!response.IsSuccessStatusCode)
                throw new HttpRequestException($"Erro na API: {response.StatusCode}");

            string jsonResult = await response.Content.ReadAsStringAsync();
            var opcoes = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            return JsonSerializer.Deserialize<List<Residuo>>(jsonResult, opcoes) ?? new List<Residuo>();
        }

        /// <summary>
        /// Altera apenas o status de um resíduo já cadastrado (ex: "Disponível", "Descartado", "Reaproveitado").
        /// Como o endpoint PUT da API espera o objeto completo, os demais campos são reenviados sem alteração.
        /// Depois de gravar na API, o nó do Firebase é sincronizado para que os cards do Estoque Reverso
        /// reflitam a mudança imediatamente, sem esperar um novo carregamento.
        /// </summary>
        /// <param name="residuo">Resíduo que será atualizado.</param>
        /// <param name="novoStatus">Novo status a ser gravado.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="HttpRequestException"></exception>
        public async Task AtualizarStatusAsync(Residuo residuo, string novoStatus)
        {
            if (residuo == null)
                throw new ArgumentNullException(nameof(residuo));

            if (string.IsNullOrWhiteSpace(residuo.Id))
                throw new ArgumentException("O resíduo não possui um Id válido para ser atualizado.", nameof(residuo));

            if (string.IsNullOrWhiteSpace(novoStatus))
                throw new ArgumentException("O novo status não pode ser vazio.", nameof(novoStatus));

            /// Reenvia o cadastro inteiro trocando somente o Status, pois o PUT da API substitui o registro.
            /// Os números vão em cultura invariável (ponto decimal) para não depender da cultura do servidor.
            using var form = new MultipartFormDataContent
            {
                { new StringContent(residuo.TipoResiduo ?? string.Empty), "TipoResiduo" },
                { new StringContent(residuo.Especificacao ?? string.Empty), "Especificacao" },
                { new StringContent(residuo.Origem ?? string.Empty), "Origem" },
                { new StringContent(residuo.Projeto ?? string.Empty), "Projeto" },
                { new StringContent(residuo.Quantidade.ToString(CultureInfo.InvariantCulture)), "Quantidade" },
                { new StringContent(residuo.UnidadeMedida ?? "kg"), "UnidadeMedida" },
                { new StringContent(residuo.UnidadeDimensao ?? "cm"), "UnidadeDimensao" },
                { new StringContent(residuo.DataCadastro.ToString("o", CultureInfo.InvariantCulture)), "DataCadastro" },
                { new StringContent(residuo.Condicao ?? string.Empty), "Condicao" },
                { new StringContent((residuo.DimensoesCm ?? 0).ToString(CultureInfo.InvariantCulture)), "DimensoesCm" },
                { new StringContent((residuo.DimensoesLm ?? 0).ToString(CultureInfo.InvariantCulture)), "DimensoesLm" },
                { new StringContent(residuo.Observacao ?? string.Empty), "Observacao" },
                { new StringContent(residuo.Anexo ?? string.Empty), "Anexo" },
                { new StringContent(novoStatus), "Status" }
            };

            var response = await _httpClient.PutAsync($"Residuo/{residuo.Id}", form);

            if (!response.IsSuccessStatusCode)
            {
                string erro = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Erro na API ao alterar o status ({(int)response.StatusCode}): {erro}");
            }

            /// Espelha o novo status no Realtime Database (fonte dos cards do Estoque Reverso).
            /// Uma falha aqui não invalida a gravação na API, então apenas registramos no log de depuração.
            try
            {
                await FirebaseConfigService.Client
                    .Child(NodeResiduos)
                    .Child(residuo.Id)
                    .PatchAsync(new { Status = novoStatus });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Aviso] Status salvo na API, mas não foi possível espelhar no Firebase: {ex.Message}");
            }
        }
    }
}
