using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ApiRestReGraphik.Models
{
    /// <summary>
    /// Classe que representa a estrutura de dados para um usuário na API REST.
    /// </summary>
    public class Usuario
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("nome")]
        public string Nome { get; set; } = string.Empty;

        [JsonPropertyName("cpf")]
        public string CPF { get; set; } = string.Empty;

        [JsonPropertyName("email")]
        public string Email { get; set; } = string.Empty;

        [JsonPropertyName("login")]
        public string Login { get; set; } = string.Empty;

        [JsonPropertyName("senha")]
        public string Senha { get; set; } = string.Empty;

        [JsonPropertyName("perfil")]
        public string Perfil { get; set; } = "User";


        [JsonPropertyName("cargo")]
        public string? Cargo { get; set; }

        [JsonPropertyName("departamento")]
        public string? Departamento { get; set; }

        [JsonPropertyName("telefone")]
        public string? Telefone { get; set; }

        [JsonPropertyName("data_cadastro")]
        public DateTime DataCadastro { get; set; }

        [JsonPropertyName("foto_perfil")]
        public string? FotoPerfil { get; set; }

        /// <summary>
        /// Arquivo enviado pelo formulário (FormUpload).
        /// </summary>
        [JsonIgnore]
        [NotMapped]
        public IFormFile? ImagemPerfil { get; set; }

        [JsonPropertyName("ativo")]
        public bool Ativo { get; set; } = true;
    }
}