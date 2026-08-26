using ApiRestReGraphik.Models;
using Firebase.Database;
using Firebase.Database.Query;
using Google.Apis.Auth.OAuth2;
using System.Text.Json;

namespace ApiRestReGraphik.Services
{
    public class PontosColetaService
    {
        private readonly FirebaseClient _firebaseClient;
        private readonly ILogger<PontosColetaService> _logger;
        private readonly IConfiguration _configuration;

        private const string NodeName = "pontos_coleta";

        /// <summary>
        /// Construtor da classe PontosColetaService, responsável por inicializar
        /// o cliente do Firebase e o logger da classe.
        /// </summary>
        /// <param name="logger">Logger utilizado para registrar informações e erros.</param>
        /// <param name="configuration">Configurações utilizadas pela aplicação.</param>
        public PontosColetaService(
            ILogger<PontosColetaService> logger,
            IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;

            var dbUrl =
                configuration["Firebase:RealtimeDatabaseUrl"];

            var credentialsFileName =
                configuration["Firebase:CredentialFilePath"]
                ?? "ReGraphikFirebaseKey.json";

            if (string.IsNullOrEmpty(dbUrl))
            {
                _logger.LogError(
                    "Erro crítico: URL do Realtime Database não encontrada no appsettings.json");

                throw new Exception(
                    "Configurações do Firebase ausentes.");
            }

            try
            {
                // Obtém o caminho físico onde a API está sendo executada.
                var caminhoBase =
                    AppContext.BaseDirectory;

                var caminhoCompletoChave =
                    Path.Combine(
                        caminhoBase,
                        credentialsFileName);

                if (!File.Exists(caminhoCompletoChave))
                {
                    _logger.LogError(
                        $"Arquivo de credenciais não encontrado em: {caminhoCompletoChave}");

                    throw new FileNotFoundException(
                        $"O arquivo {credentialsFileName} precisa estar na raiz da API.");
                }

                // Carrega as credenciais do Firebase.
                GoogleCredential credenciais;

                using (var stream = new FileStream(
                    caminhoCompletoChave,
                    FileMode.Open,
                    FileAccess.Read))
                {
                    credenciais =
                        GoogleCredential
                            .FromStream(stream)
                            .CreateScoped(new[]
                            {
                                "https://www.googleapis.com/auth/userinfo.email",
                                "https://www.googleapis.com/auth/firebase.database"
                            });
                }

                _firebaseClient = new FirebaseClient(
                    dbUrl,
                    new FirebaseOptions
                    {
                        AuthTokenAsyncFactory = async () =>
                        {
                            var token =
                                await credenciais
                                    .UnderlyingCredential
                                    .GetAccessTokenForRequestAsync();

                            return token;
                        }
                    });
            }
            catch (Exception ex)
            {
                _logger.LogCritical(
                    $"Falha fatal ao inicializar o FirebaseService: {ex.Message}");

                throw;
            }
        }

        /// <summary>
        /// Construtor alternativo utilizado para permitir o isolamento
        /// do Firebase durante a execução dos testes automatizados.
        /// </summary>
        /// <param name="logger">Logger utilizado pelo serviço.</param>
        /// <param name="configuration">Configurações utilizadas pelo serviço.</param>
        /// <param name="firebaseClient">Cliente Firebase controlado externamente.</param>
        protected PontosColetaService(
            ILogger<PontosColetaService> logger,
            IConfiguration configuration,
            FirebaseClient firebaseClient)
        {
            _logger = logger;
            _configuration = configuration;
            _firebaseClient = firebaseClient;
        }

        /// <summary>
        /// Lista todos os pontos de coleta cadastrados no ReGraphik.
        /// </summary>
        public virtual async Task<List<PontosColeta>> Listar()
        {
            try
            {
                var pontos =
                    await _firebaseClient
                        .Child(NodeName)
                        .OnceAsync<PontosColeta>();

                return pontos
                    .Select(p => p.Object)
                    .ToList();
            }
            catch (FirebaseException ex)
            {
                _logger.LogError(
                    ex,
                    "Erro de comunicação ou permissão no Firebase ao listar pontos de coleta.");

                throw;
            }
            catch (JsonException ex)
            {
                _logger.LogError(
                    ex,
                    "Erro de desserialização. Os dados no Firebase estão em formato inválido.");

                throw new InvalidOperationException(
                    "Os dados recuperados do banco estão corrompidos.",
                    ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Erro inesperado na camada de serviço ao listar pontos de coleta.");

                throw;
            }
        }

        /// <summary>
        /// Obtém um ponto de coleta específico através do ID.
        /// </summary>
        /// <param name="id">ID do ponto de coleta.</param>
        public virtual async Task<PontosColeta> ObterPorId(
            string id)
        {
            try
            {
                var ponto =
                    await _firebaseClient
                        .Child(NodeName)
                        .Child(id)
                        .OnceSingleAsync<PontosColeta>();

                return ponto;
            }
            catch (FirebaseException ex)
            {
                _logger.LogError(
                    ex,
                    $"Erro de infraestrutura no Firebase ao obter o ponto de coleta por ID: {id}");

                throw;
            }
            catch (JsonException ex)
            {
                _logger.LogError(
                    ex,
                    $"Erro de desserialização. Os nós relacionados ao ID {id} possuem dados inválidos.");

                throw new InvalidOperationException(
                    "Os dados obtidos do Firebase estão corrompidos ou em formato inválido.",
                    ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    $"Erro inesperado ao obter o ponto de coleta por ID: {id}");

                throw;
            }
        }

        /// <summary>
        /// Adiciona um novo ponto de coleta ao Firebase.
        /// </summary>
        /// <param name="pontosColeta">Ponto de coleta que será cadastrado.</param>
        public virtual async Task Criar(
            PontosColeta pontosColeta)
        {
            try
            {
                pontosColeta.Id = null;

                var resultado =
                    await _firebaseClient
                        .Child(NodeName)
                        .PostAsync(pontosColeta);

                pontosColeta.Id =
                    resultado.Key;

                await _firebaseClient
                    .Child(NodeName)
                    .Child(resultado.Key)
                    .PutAsync(pontosColeta);
            }
            catch (FirebaseException ex)
            {
                _logger.LogError(
                    ex,
                    "Erro no Firebase ao tentar criar novo ponto de coleta.");

                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Erro inesperado ao adicionar o ponto de coleta.");

                throw;
            }
        }

        /// <summary>
        /// Sincroniza os pontos de coleta utilizando os resultados
        /// encontrados através da API do Google Maps.
        /// </summary>
        /// <param name="cidade">Cidade utilizada na pesquisa.</param>
        /// <param name="apiKey">Chave da API do Google Maps.</param>
        /// <param name="httpClient">Cliente HTTP utilizado na comunicação.</param>
        public virtual async Task<(int salvos, int ignorados)>
            SincronizarComGoogleMapsAsync(
                string cidade,
                string apiKey,
                HttpClient httpClient)
        {
            try
            {
                var pontosNoBanco =
                    (await Listar())?.ToList()
                    ?? new List<PontosColeta>();

                var coordenadasExistentes =
                    new HashSet<(double, double)>(
                        pontosNoBanco.Select(
                            p => (p.Lat, p.Lng)));

                var query =
                    Uri.EscapeDataString(
                        $"ponto de coleta reciclagem {cidade}");

                var url =
                    $"https://maps.googleapis.com/maps/api/place/textsearch/json?query={query}&key={apiKey}";

                var json =
                    await httpClient.GetStringAsync(url);

                using var doc =
                    JsonDocument.Parse(json);

                if (doc.RootElement.TryGetProperty(
                    "status",
                    out var statusProp))
                {
                    string statusApi =
                        statusProp.GetString() ?? "";

                    if (statusApi != "OK" &&
                        statusApi != "ZERO_RESULTS")
                    {
                        _logger.LogWarning(
                            $"API do Google Maps retornou um status de erro: {statusApi}");

                        return (0, 0);
                    }
                }

                if (!doc.RootElement.TryGetProperty(
                    "results",
                    out var results))
                {
                    return (0, 0);
                }

                int totalSalvo = 0;
                int totalIgnorado = 0;

                foreach (var item in results.EnumerateArray())
                {
                    var nome =
                        item.TryGetProperty(
                            "name",
                            out var n)
                        ? n.GetString()
                        : "Sem nome";

                    double lat = 0;
                    double lng = 0;

                    if (item.TryGetProperty(
                            "geometry",
                            out var geo)
                        &&
                        geo.TryGetProperty(
                            "location",
                            out var loc))
                    {
                        lat =
                            loc.TryGetProperty(
                                "lat",
                                out var la)
                            ? la.GetDouble()
                            : 0;

                        lng =
                            loc.TryGetProperty(
                                "lng",
                                out var ln)
                            ? ln.GetDouble()
                            : 0;
                    }

                    if (coordenadasExistentes.Contains(
                        (lat, lng)))
                    {
                        totalIgnorado++;

                        continue;
                    }

                    var novoPonto =
                        new PontosColeta
                        {
                            NomePonto = nome,
                            Cidade = cidade,

                            Estado =
                                _configuration[
                                    "Sincronizacao:EstadoPadrao"]
                                ?? "BR",

                            CEP =
                                _configuration[
                                    "Sincronizacao:CepPadrao"]
                                ?? "—",

                            ResiduosAceitos =
                                _configuration[
                                    "Sincronizacao:ResiduosAceitos"]
                                ?? "Reciclável",

                            Lat = lat,
                            Lng = lng
                        };

                    await Criar(novoPonto);

                    coordenadasExistentes.Add(
                        (lat, lng));

                    totalSalvo++;
                }

                return (
                    totalSalvo,
                    totalIgnorado);
            }
            catch (FirebaseException ex)
            {
                _logger.LogError(
                    ex,
                    "Erro de comunicação ou permissão no Firebase ao buscar pontos de coleta.");

                throw;
            }
            catch (JsonException ex)
            {
                _logger.LogError(
                    ex,
                    "Erro de desserialização. Os dados no Firebase estão em formato inválido.");

                throw new InvalidOperationException(
                    "Os dados recuperados do banco estão corrompidos.",
                    ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Erro inesperado na camada de serviço ao buscar pontos de coleta.");

                throw;
            }
        }

        /// <summary>
        /// Atualiza um ponto de coleta existente.
        /// </summary>
        /// <param name="id">ID do ponto.</param>
        /// <param name="pontosColeta">Dados atualizados.</param>
        public virtual async Task Atualizar(
            string id,
            PontosColeta pontosColeta)
        {
            try
            {
                pontosColeta.Id = id;

                await _firebaseClient
                    .Child(NodeName)
                    .Child(id)
                    .PutAsync(pontosColeta);
            }
            catch (FirebaseException ex)
            {
                _logger.LogError(
                    ex,
                    $"Erro no Firebase ao tentar atualizar o ponto de coleta ID: {id}");

                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    $"Erro inesperado ao atualizar o ponto de coleta ID: {id}");

                throw;
            }
        }

        /// <summary>
        /// Exclui um ponto de coleta através do ID.
        /// </summary>
        /// <param name="id">ID do ponto que será excluído.</param>
        public virtual async Task Excluir(
            string id)
        {
            try
            {
                await _firebaseClient
                    .Child(NodeName)
                    .Child(id)
                    .DeleteAsync();
            }
            catch (FirebaseException ex)
            {
                _logger.LogError(
                    ex,
                    $"Erro no Firebase ao tentar excluir o ponto de coleta ID: {id}");

                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    $"Erro inesperado ao excluir o ponto de coleta ID: {id}");

                throw;
            }
        }
    }
}