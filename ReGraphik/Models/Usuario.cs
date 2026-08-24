using Newtonsoft.Json;
using System;
using System.Text.Json.Serialization;

namespace ReGraphik.Models
{
    public class Usuario
    {
        [JsonPropertyName("id")]
        [JsonProperty("id")]
        public string Id { get; set; } = string.Empty;

        private string _nome = string.Empty;

        [JsonPropertyName("nome")]
        [JsonProperty("nome")]
        public string Nome
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_nome))
                {
                    return !string.IsNullOrWhiteSpace(Login) ? Login : "Sem Nome";
                }
                return _nome;
            }
            set => _nome = value;
        }

        [JsonPropertyName("cpf")]
        [JsonProperty("cpf")]
        public string CPF { get; set; } = string.Empty;

        [JsonPropertyName("email")]
        [JsonProperty("email")]
        public string Email { get; set; } = string.Empty;

        [JsonPropertyName("login")]
        [JsonProperty("login")]
        public string Login { get; set; } = string.Empty;

        [JsonPropertyName("senha")]
        [JsonProperty("senha")]
        public string Senha { get; set; } = string.Empty;

        [JsonPropertyName("perfil")]
        [JsonProperty("perfil")]
        public string Perfil { get; set; } = string.Empty;

        [JsonPropertyName("cargo")]
        [JsonProperty("cargo")]
        public string? Cargo { get; set; }

        [JsonPropertyName("departamento")]
        [JsonProperty("departamento")]
        public string? Departamento { get; set; }

        [JsonPropertyName("telefone")]
        [JsonProperty("telefone")]
        public string? Telefone { get; set; }

        [JsonPropertyName("data_cadastro")]
        [JsonProperty("data_cadastro")]
        public DateTime DataCadastro { get; set; }

        [JsonPropertyName("foto_perfil")]
        [JsonProperty("foto_perfil")]
        public string? FotoPerfil { get; set; }

        [JsonPropertyName("ativo")]
        [JsonProperty("ativo")]
        public bool Ativo { get; set; }

        /// <summary>
        /// Iniciais do nome para exibição no avatar quando não há foto.
        /// Retorna as iniciais das duas primeiras palavras do nome (ex: "Bruno Maia" → "BM").
        /// </summary>
        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        public string Iniciais
        {
            get
            {
                string nomeParaUsar = Nome;

                if (string.IsNullOrWhiteSpace(nomeParaUsar)) return "?";

                var partes = nomeParaUsar.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (partes.Length == 1) return partes[0][0].ToString().ToUpper();
                return (partes[0][0].ToString() + partes[^1][0].ToString()).ToUpper();
            }
        }
    }
}