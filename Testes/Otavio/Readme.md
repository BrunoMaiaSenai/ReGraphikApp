

# Documentação Individual de Testes Unitários

## Módulo Mapa e Pontos de Coleta — Sistema ReGraphik

**Aluno:** Otávio Henrique Barbosa Soares  
**Curso:** Técnico em Desenvolvimento de Sistemas — Noite  
**Instituição:** Serviço Nacional de Aprendizagem Industrial — SENAI  
**Equipe:** ReGraphik  
**Framework de testes:** xUnit  
**Ambiente:** C# / .NET 8 / Visual Studio 2022  
**Local e ano:** Nova Lima — 2026  

> Documentação individual apresentada como parte da atividade de testes unitários aplicada ao Projeto de TCC do curso Técnico em Desenvolvimento de Sistemas — Noite, sob orientação do Instrutor Frederico Martins Aguiar.

## Sumário

- [1 Introdução](#1-introdução)
- [2 Identificação da atividade](#2-identificação-da-atividade)
- [3 Organização e metodologia da suíte](#3-organização-e-metodologia-da-suíte)
- [4 Adaptações necessárias no código original](#4-adaptações-necessárias-no-código-original)
- [5 Plano de testes unitários — WEB](#5-plano-de-testes-unitários--web)
- [6 Plano de testes unitários — API](#6-plano-de-testes-unitários--api)
- [7 Registro dos resultados da suíte](#7-registro-dos-resultados-da-suíte)
- [8 Considerações finais](#8-considerações-finais)
- [Referências](#referências)

---

## 1 Introdução

Este documento apresenta o planejamento dos testes unitários do módulo de Mapa e Pontos de Coleta do sistema ReGraphik. A organização foi preparada como roteiro de execução e de evidências, sem registrar previamente qualquer teste como concluído. Cada caso de teste contém objetivo, procedimento previsto, resultado esperado e um espaço próprio para inserção do print do código correspondente.

A suíte será implementada com xUnit e seguirá a estrutura Arrange, Act e Assert. Os cenários foram definidos a partir dos comportamentos das classes relacionadas ao mapa, considerando caminhos válidos, entradas inesperadas, exceções, respostas de serviços externos e regras de negócio que podem ser verificadas de forma isolada.

Além do plano de testes, o documento registra as adaptações de testabilidade que foram aplicadas ao código original para permitir o isolamento de Google Places e Firebase. Essas adaptações não têm como objetivo alterar regras de negócio; elas permitem que os testes controlem dependências externas e sejam executados de maneira repetível.

| CT | Camada / Pasta | Caso de Teste | Descrição / Cenário | Situação |
| :---: | :--- | :--- | :--- | :---: |
| **CT001** | WEB / ConvertersTeste | Conversão de valor verdadeiro | Entrada válida: o converter recebe o valor booleano true. | ✅ Aprovado |
| **CT002** | WEB / ConvertersTeste | Conversão de valor falso | Entrada válida: o converter recebe o valor booleano false. | ✅ Aprovado |
| **CT003** | WEB / ConvertersTeste | Tratamento de valor não booleano | Entrada inválida: o converter recebe uma string em vez de um valor booleano. | ✅ Aprovado |
| **CT004** | WEB / ConvertersTeste | Tratamento de valor nulo | Entrada nula: o converter recebe null como valor de entrada. | ✅ Aprovado |
| **CT005** | WEB / ConvertersTeste | Conversão inversa não implementada | Exceção esperada: o método ConvertBack é chamado mesmo não possuindo implementação. | ✅ Aprovado |
| **CT006** | WEB / ServicesTeste | Resposta válida da API | Fluxo de sucesso: a API simulada retorna um ponto com identificador, nome, endereço e coordenadas válidas. | ✅ Aprovado |
| **CT007** | WEB / ServicesTeste | Pesquisa sem resultados | Resposta vazia: a API simulada retorna results sem nenhum local. | ✅ Aprovado |
| **CT008** | WEB / ServicesTeste | Falha na comunicação com a API | Falha externa: a requisição HTTP simulada lança HttpRequestException. | ✅ Aprovado |
| **CT009** | WEB / ServicesTeste | Resultado sem place_id | Resposta incompleta: o local retornado não possui place_id. | ✅ Aprovado |
| **CT010** | WEB / ServicesTeste | Resultado sem nome | Resposta incompleta: o local possui identificador e coordenadas, mas não possui name. | ✅ Aprovado |
| **CT011** | WEB / ServicesTeste | Resultado sem coordenadas | Resposta incompleta: o local retornado não possui geometry/location. | ✅ Aprovado |
| **CT012** | WEB / ServicesTeste | JSON inválido | Resposta malformada: o conteúdo retornado não está em formato JSON válido. | ✅ Aprovado |
| **CT013** | WEB / ViewModelsTeste | Inicialização do mapa | Estado inicial: uma nova instância do MapaViewModel é criada sem consultas externas. | ✅ Aprovado |
| **CT014** | WEB / ViewModelsTeste | Estado vazio sem pontos | Coleção vazia e sem carregamento: não existem pontos e IsCarregando está false. | ✅ Aprovado |
| **CT015** | WEB / ViewModelsTeste | Estado vazio com ponto | Coleção com resultado: existe pelo menos um ponto e o carregamento está inativo. | ✅ Aprovado |
| **CT016** | WEB / ViewModelsTeste | Estado durante carregamento | Coleção vazia durante busca: IsCarregando está true. | ✅ Aprovado |
| **CT017** | WEB / ViewModelsTeste | Notificação da propriedade Cidade | Alteração de binding: a propriedade Cidade recebe um novo valor. | ✅ Aprovado |
| **CT018** | WEB / ViewModelsTeste | Notificação da coleção PontosAtuais | Alteração de binding: a coleção PontosAtuais é substituída por uma nova ObservableCollection. | ✅ Aprovado |
| **CT019** | WEB / ViewModelsTeste | Lista nula de marcadores | Entrada nula: GerarJsonMarcadores recebe uma lista null. | ✅ Aprovado |
| **CT020** | WEB / ViewModelsTeste | Lista vazia de marcadores | Entrada vazia: GerarJsonMarcadores recebe uma lista sem pontos. | ✅ Aprovado |
| **CT021** | WEB / ViewModelsTeste | Geração de marcador válido | Fluxo de sucesso: a lista contém um ponto completo com dados e coordenadas válidas. | ✅ Aprovado |
| **CT022** | WEB / ViewModelsTeste | Índices de vários marcadores | Múltiplos registros: a lista contém três pontos em ordem definida. | ✅ Aprovado |
| **CT023** | WEB / ViewModelsTeste | Coordenadas iguais a zero | Dados sem localização válida: o ponto possui Lat = 0 e Lng = 0. | ✅ Aprovado |
| **CT024** | WEB / ViewModelsTeste | Formatação decimal das coordenadas | Formatação regional: o ponto possui coordenadas com várias casas decimais. | ✅ Aprovado |
| **CT025** | WEB / ViewModelsTeste | Tratamento de caracteres especiais | Texto especial: nome, cidade e resíduos contêm aspas, quebra de linha e barra invertida. | ✅ Aprovado |
| **CT026** | WEB / ViewModelsTeste | Foco em ponto do mapa | Mapa carregado: existem dois pontos e o foco é solicitado para o segundo item. | ✅ Aprovado |
| **CT027** | API / ControllerTeste | Listagem de pontos com sucesso | Fluxo de sucesso: o serviço fornece pontos cadastrados para o endpoint GET. | ✅ Aprovado |
| **CT028** | API / ControllerTeste | Falha de comunicação na listagem | Falha externa: o serviço lança HttpRequestException durante a listagem. | ✅ Aprovado |
| **CT029** | API / ControllerTeste | Sincronização sem cidade | Entrada inválida: a sincronização é solicitada com cidade vazia. | ✅ Aprovado |
| **CT030** | API / ControllerTeste | Sincronização sem chave da API | Falha de configuração: GoogleMaps:ApiKey não está configurada. | ✅ Aprovado |
| **CT031** | API / ControllerTeste | Sincronização concluída com sucesso | Fluxo de sucesso: uma cidade válida é sincronizada e o serviço retorna quantidades de salvos e ignorados. | ✅ Aprovado |
| **CT032** | API / ControllerTeste | Busca por ID existente | Registro existente: o serviço encontra o ponto correspondente ao ID informado. | ✅ Aprovado |
| **CT033** | API / ControllerTeste | Busca por ID inexistente | Registro inexistente: o serviço não encontra ponto para o ID informado. | ✅ Aprovado |
| **CT034** | API / ControllerTeste | Cadastro sem dados | Entrada inválida: o endpoint de cadastro recebe um DTO nulo. | ✅ Aprovado |
| **CT035** | API / ControllerTeste | Cadastro válido | Fluxo de sucesso: o endpoint recebe um DTO preenchido com dados válidos. | ✅ Aprovado |
| **CT036** | API / ControllerTeste | Atualização de ID inexistente | Registro inexistente: é solicitada atualização para um ID não cadastrado. | ✅ Aprovado |
| **CT037** | API / ControllerTeste | Atualização válida | Fluxo de sucesso: um ponto existente recebe novos dados válidos. | ✅ Aprovado |
| **CT038** | API / ControllerTeste | Exclusão de ID inexistente | Registro inexistente: é solicitada exclusão para um ID não cadastrado. | ✅ Aprovado |
| **CT039** | API / ControllerTeste | Exclusão válida | Fluxo de sucesso: o ID informado corresponde a um ponto existente. | ✅ Aprovado |
| **CT040** | API / ServicesApiTeste | Configuração do Firebase ausente | Falha de configuração: a URL do Realtime Database não está definida. | ✅ Aprovado |
| **CT041** | API / ServicesApiTeste | Arquivo de credenciais ausente | Falha de configuração: a URL existe, mas o arquivo de credenciais informado não é encontrado. | ✅ Aprovado |
| **CT042** | API / ServicesApiTeste | Status de erro retornado pelo Google | Resposta externa de erro: o Google simulado retorna status REQUEST_DENIED. | ✅ Aprovado |
| **CT043** | API / ServicesApiTeste | Resposta sem resultados | Resposta incompleta: o JSON possui status OK, mas não contém a propriedade results. | ✅ Aprovado |
| **CT044** | API / ServicesApiTeste | Ponto duplicado por coordenadas | Duplicidade: o Google retorna um ponto com latitude e longitude já existentes na base simulada. | ✅ Aprovado |
| **CT045** | API / ServicesApiTeste | Novo ponto contabilizado como salvo | Fluxo de sucesso: o Google retorna um ponto com coordenadas ainda não cadastradas. | ✅ Aprovado |
| **CT046** | API / ServicesApiTeste | JSON inválido na sincronização | Resposta malformada: a sincronização recebe um conteúdo que não é JSON válido. | ✅ Aprovado |

**Resumo da suíte unitária:** 46 testes executados, 46 aprovados, 0 falhas e 0 ignorados.

## 2 Identificação da atividade

| **Aluno**               | Otávio Henrique Barbosa Soares                 |
|-------------------------|------------------------------------------------|
| **Turma**               | Técnico em Desenvolvimento de Sistemas – Noite |
| **Equipe**              | ReGraphik                                      |
| **Data da entrega**     | 25/08/2026                                     |
| **Componente**          | Mapa e Pontos de Coleta                        |
| **Projeto de testes**   | TesteReGraphik                                 |
| **Framework de testes** | xUnit                                          |
| **Ambiente**            | C# / .NET 8 / Visual Studio 2022               |

### 2.1 Escopo individual

O escopo desta documentação será limitado às classes relacionadas ao mapa e aos pontos de coleta. Os testes serão organizados nas pastas WEB/ConvertersTeste, WEB/ServicesTeste, WEB/ViewModelsTeste, APITeste/ControllerTeste e APITeste/ServicesApiTeste. Alterações em componentes de outras funcionalidades da equipe não fazem parte deste trabalho.

## 3 Organização e metodologia da suíte

A suíte será composta por 46 casos de teste, identificados de CT001 a CT046. Cada caso deverá ser implementado em sua classe correspondente e documentado individualmente. Após a execução de cada pasta, será inserido também um print geral do Gerenciador de Testes mostrando o conjunto daquela pasta.

### 3.1 Estrutura planejada

<table>
<colgroup>
<col style="width: 100%" />
</colgroup>
<thead>
<tr class="header">
<th>TesteReGraphik<br />
├── WEB<br />
│ ├── ConvertersTeste<br />
│ │ └── BoolToVisibilityConverterTeste.cs<br />
│ ├── ServicesTeste<br />
│ │ └── GooglePlacesServiceTeste.cs<br />
│ └── ViewModelsTeste<br />
│ └── MapaViewModelTeste.cs<br />
└── APITeste<br />
├── ControllerTeste<br />
│ └── PontosDeColetaControllerTeste.cs<br />
└── ServicesApiTeste<br />
└── PontosColetaServiceTeste.cs</th>
</tr>
</thead>
<tbody>
</tbody>
</table>

### 3.2 Padrão Arrange, Act e Assert

Cada método de teste será organizado em três etapas. Arrange preparará os objetos e dados necessários; Act executará o comportamento que está sendo avaliado; Assert comparará o resultado obtido com o comportamento esperado. Os nomes dos métodos permanecerão em português sempre que possível, preservando apenas termos técnicos do framework ou da própria aplicação.

### 3.3 Padrão de evidências

Para cada CT haverá um espaço reservado para o print do método de teste. O print deverá mostrar, sempre que possível, o atributo \[Fact\], o nome do método e as etapas Arrange, Act e Assert. Ao final de cada pasta haverá um espaço adicional para o print geral do Gerenciador de Testes. O campo “Resultado após execução” deverá ser preenchido somente depois que o teste for realmente executado.

## 4 Adaptações necessárias no código original

Antes da execução completa da suíte, duas classes do código original precisaram receber adaptações de testabilidade. As alterações abaixo deverão ser replicadas na solução oficial que será versionada.

### 4.1 GooglePlacesService

> **Observação de segurança:** qualquer chave real de API foi ocultada das evidências destinadas à publicação no GitHub. Chaves e credenciais não devem ser versionadas no repositório.


**Situação original:** O GooglePlacesService possuía uma instância de HttpClient criada diretamente na declaração do campo e a chave da API também era definida diretamente na classe. Não havia um construtor que permitisse fornecer essas dependências externamente.

**Adaptação realizada:** A inicialização direta dos campos foi transferida para o construtor padrão e foi acrescentado um segundo construtor que recebe HttpClient e apiKey. Dessa forma, a aplicação continua utilizando o mesmo comportamento padrão, enquanto os testes podem fornecer um cliente HTTP controlado.

**Motivo:** Permitir que respostas da API do Google Places sejam simuladas nos testes unitários, evitando chamadas reais à internet e tornando os cenários reproduzíveis.

**Impacto:** Nenhuma regra de negócio do método BuscarPostosNoBrasilAsync foi alterada. A busca, o processamento do JSON, o tratamento das coordenadas, endereço formatado e exceções permanecem com a mesma implementação.

**Testes relacionados:** CT006 a CT012.

#### 4.1.1 Estrutura de referência antes da adaptação

<table>
<colgroup>
<col style="width: 100%" />
</colgroup>
<thead>
<tr class="header">
<th>private readonly HttpClient _httpClient = new HttpClient();<br />
private readonly string _apiKey = "&lt;CHAVE_EXISTENTE&gt;";<br />
<br />
// A classe utiliza diretamente essas dependências na consulta.</th>
</tr>
</thead>
<tbody>
</tbody>
</table>

Figura 1 – Código original do GooglePlacesService antes da adaptação


|---------------------------------------------------------------------------------------------------------------|

Fonte: Elaborado pelo autor (2026).

#### 4.1.2 Estrutura a ser adicionada

<table>
<colgroup>
<col style="width: 100%" />
</colgroup>
<thead>
<tr class="header">
<th>public GooglePlacesService()<br />
{<br />
}<br />
<br />
public GooglePlacesService(HttpClient httpClient, string apiKey)<br />
{<br />
_httpClient = httpClient;<br />
_apiKey = apiKey;<br />
}</th>
</tr>
</thead>
<tbody>
</tbody>
</table>

Figura 2 – GooglePlacesService após inclusão dos construtores para testabilidade


|---------------------------------------------------------------------------------------------------------------|

Fonte: Elaborado pelo autor (2026).

Testes relacionados: CT006 a CT012. A finalidade dessa adaptação será permitir que o arquivo GooglePlacesServiceTeste.cs utilize um HttpClient controlado, sem realizar chamadas reais à internet.

### 4.2 PontosColetaService

**Situação original:** A classe possuía apenas o construtor público responsável pela inicialização normal do Firebase.

**Adaptação realizada:** O construtor público original foi preservado. Foi acrescentado um segundo construtor, com modificador protected, que recebe uma instância de FirebaseClient fornecida externamente.

**Motivo:** Permitir que uma classe derivada utilizada nos testes seja criada com um cliente Firebase controlado, sem executar o processo real de leitura de credenciais e autenticação.

**Impacto:** O funcionamento do construtor utilizado em produção não foi alterado.

#### 4.2.1 Construtor protegido a ser acrescentado

<table>
<colgroup>
<col style="width: 100%" />
</colgroup>
<thead>
<tr class="header">
<th>protected PontosColetaService(<br />
ILogger&lt;PontosColetaService&gt; logger,<br />
IConfiguration configuration,<br />
FirebaseClient firebaseClient)<br />
{<br />
_logger = logger;<br />
_configuration = configuration;<br />
_firebaseClient = firebaseClient;<br />
}</th>
</tr>
</thead>
<tbody>
</tbody>
</table>

Figura 3 – Estrutura original do PontosColetaService antes da inclusão do construtor para testes


|---------------------------------------------------------------------------------------------------------------|

Fonte: Elaborado pelo autor (2026).

Figura 4 – Construtor protegido acrescentado ao PontosColetaService


|---------------------------------------------------------------------------------------------------------------|

Fonte: Elaborado pelo autor (2026).

#### 4.2.2 Métodos que deverão receber o modificador virtual

A lógica interna dos métodos deverá permanecer inalterada. A única mudança será acrescentar virtual às assinaturas abaixo, permitindo override em classes falsas criadas exclusivamente no projeto de testes.

<table>
<colgroup>
<col style="width: 100%" />
</colgroup>
<thead>
<tr class="header">
<th>public virtual async Task&lt;List&lt;PontosColeta&gt;&gt; Listar()<br />
public virtual async Task&lt;PontosColeta&gt; ObterPorId(string id)<br />
public virtual async Task Criar(PontosColeta pontosColeta)<br />
public virtual async Task&lt;(int salvos, int ignorados)&gt; SincronizarComGoogleMapsAsync(<br />
string cidade, string apiKey, HttpClient httpClient)<br />
public virtual async Task Atualizar(string id, PontosColeta pontosColeta)<br />
public virtual async Task Excluir(string id)</th>
</tr>
</thead>
<tbody>
</tbody>
</table>

Figura 5 – Assinaturas do PontosColetaService após aplicação de virtual


|---------------------------------------------------------------------------------------------------------------|

Fonte: Elaborado pelo autor (2026).

Testes relacionados: CT027 a CT046. O construtor público original continuará responsável pelo funcionamento normal da aplicação, enquanto o construtor protegido e os métodos virtuais permitirão substituições apenas no contexto dos testes.

### 4.3 Componentes que não deverão ser alterados

Para os cenários planejados, BoolToVisibilityConverter.cs, MapaViewModel.cs e PontosColetaController.cs não precisarão receber alterações para tornar os testes executáveis. Os testes deverão observar o comportamento já existente nessas classes.

### 4.4 Projeto TesteReGraphik

O arquivo TesteReGraphik.csproj não deverá ser apresentado como uma alteração realizada nesta documentação, pois ele não foi modificado. A documentação deve refletir somente mudanças efetivamente aplicadas no processo.

### 4.5 Quadro-resumo das adaptações

| **Arquivo**                  | **Adaptação planejada**                                | **Finalidade**                                            | **CT relacionados** |
|------------------------------|--------------------------------------------------------|-----------------------------------------------------------|---------------------|
| GooglePlacesService.cs       | Construtor padrão + construtor com HttpClient e apiKey | Simular respostas do Google Places                        | CT006–CT012         |
| PontosColetaService.cs       | Construtor protected + seis métodos virtual            | Isolar Firebase e permitir classes controladas nos testes | CT027–CT046         |
| BoolToVisibilityConverter.cs | Nenhuma                                                | Testar implementação existente                            | CT001–CT005         |
| MapaViewModel.cs             | Nenhuma                                                | Testar implementação existente                            | CT013–CT026         |
| PontosColetaController.cs    | Nenhuma                                                | Testar decisões HTTP da implementação existente           | CT027–CT039         |
| TesteReGraphik.csproj        | Nenhuma alteração registrada                           | Refletir o processo realmente realizado                   | —                   |

## 5 Plano de testes unitários – WEB

Esta seção apresentará os casos planejados para a camada WPF. Cada CT terá um espaço próprio para evidência do método de teste. O resultado deverá ser preenchido apenas após a execução.

### 5.1 WEB / ConvertersTeste

Os casos CT001 a CT005 avaliarão a conversão de valores para estados de visibilidade do WPF.

#### CT001 – Conversão de valor verdadeiro

| **Camada / pasta**  | WEB / ConvertersTeste                         |
|---------------------|-----------------------------------------------|
| **Classe de teste** | BoolToVisibilityConverterTeste                |
| **Nome do método**  | Converter_ValorVerdadeiro_DeveRetornarVisible |
| **Situação**        | Aprovado                                      |

**Objetivo:** Verificar se o converter retorna Visibility.Visible quando recebe o valor booleano true.

**Procedimento previsto:** Será criada uma instância do converter e enviado o valor true ao método Convert. O retorno será comparado com Visibility.Visible.

**Resultado esperado:** O teste deverá confirmar que o resultado é Visibility.Visible.

Figura 6 – Evidência do código do CT001 – BoolToVisibilityConverterTeste

<table>
<colgroup>
<col style="width: 100%" />
</colgroup>
<thead>
<tr class="header">
<th>![Evidência do documento](assets/testes-mapa/image7.png)<strong><br />
</strong></th>
</tr>
</thead>
<tbody>
</tbody>
</table>

Fonte: Elaborado pelo autor (2026).

**Resultado após execução:**



#### CT002 – Conversão de valor falso

| **Camada / pasta**  | WEB / ConvertersTeste                      |
|---------------------|--------------------------------------------|
| **Classe de teste** | BoolToVisibilityConverterTeste             |
| **Nome do método**  | Converter_ValorFalso_DeveRetornarCollapsed |
| **Situação**        | Aprovado                                   |

**Objetivo:** Verificar se o converter retorna Visibility.Collapsed quando recebe o valor booleano false.

**Procedimento previsto:** Será enviado false ao método Convert, mantendo os demais parâmetros apenas como apoio à chamada do conversor.

**Resultado esperado:** O teste deverá confirmar que o resultado é Visibility.Collapsed.

Figura 7 – Evidência do código do CT002 – BoolToVisibilityConverterTeste

<table>
<colgroup>
<col style="width: 100%" />
</colgroup>
<thead>
<tr class="header">
<th>![Evidência do documento](assets/testes-mapa/image9.png)<strong><br />
</strong></th>
</tr>
</thead>
<tbody>
</tbody>
</table>

Fonte: Elaborado pelo autor (2026).

**Resultado após execução:**



#### CT003 – Tratamento de valor não booleano

| **Camada / pasta**  | WEB / ConvertersTeste                            |
|---------------------|--------------------------------------------------|
| **Classe de teste** | BoolToVisibilityConverterTeste                   |
| **Nome do método**  | Converter_ValorNaoBooleano_DeveRetornarCollapsed |
| **Situação**        | Aprovado                                         |

**Objetivo:** Verificar o comportamento do converter quando a entrada não é do tipo booleano.

**Procedimento previsto:** Será utilizada uma string como entrada para representar um valor inesperado.

**Resultado esperado:** O teste deverá confirmar que a entrada é tratada sem exceção e retorna Visibility.Collapsed.

Figura 8 – Evidência do código do CT003 – BoolToVisibilityConverterTeste


|----------------------------------------------------------------------------------------------------------------|

Fonte: Elaborado pelo autor (2026).

**Resultado após execução:**



#### CT004 – Tratamento de valor nulo

| **Camada / pasta**  | WEB / ConvertersTeste                     |
|---------------------|-------------------------------------------|
| **Classe de teste** | BoolToVisibilityConverterTeste            |
| **Nome do método**  | Converter_ValorNulo_DeveRetornarCollapsed |
| **Situação**        | Aprovado                                  |

**Objetivo:** Verificar como o converter trata uma entrada nula.

**Procedimento previsto:** Será enviado um valor nulo ao método Convert.

**Resultado esperado:** O teste deverá confirmar que o método retorna Visibility.Collapsed.

Figura 9 – Evidência do código do CT004 – BoolToVisibilityConverterTeste


|---------------------------------------------------------------------------------------------------------------|

Fonte: Elaborado pelo autor (2026).

**Resultado após execução:**



#### CT005 – Conversão inversa não implementada

| **Camada / pasta**  | WEB / ConvertersTeste                                              |
|---------------------|--------------------------------------------------------------------|
| **Classe de teste** | BoolToVisibilityConverterTeste                                     |
| **Nome do método**  | ConverterDeVolta_QuandoExecutado_DeveLancarNotImplementedException |
| **Situação**        | Aprovado                                                           |

**Objetivo:** Confirmar o comportamento definido para o método ConvertBack.

**Procedimento previsto:** O método ConvertBack será chamado com um valor de Visibility para verificar a exceção prevista na implementação.

**Resultado esperado:** O teste deverá capturar NotImplementedException.

Figura 10 – Evidência do código do CT005 – BoolToVisibilityConverterTeste


|----------------------------------------------------------------------------------------------------------------|

Fonte: Elaborado pelo autor (2026).

**Resultado após execução:**



### EVIDÊNCIA GERAL DA PASTA – WEB / ConvertersTeste

Figura 11 – Resultado geral dos testes – WEB / ConvertersTeste no Gerenciador de Testes


|----------------------------------------------------------------------------------------------------------------|

Fonte: Elaborado pelo autor (2026).

Resumo após execução da pasta: Total: \[ 5 \] Aprovados: \[ 5 \] Falhas: \[ 0 \] Ignorados: \[ 0 \].

### 5.2 WEB / ServicesTeste

Os casos CT006 a CT012 avaliarão o processamento das respostas do Google Places com comunicação HTTP controlada pela suíte.

#### CT006 – Resposta válida da API

| **Camada / pasta**  | WEB / ServicesTeste                                           |
|---------------------|---------------------------------------------------------------|
| **Classe de teste** | GooglePlacesServiceTeste                                      |
| **Nome do método**  | BuscarPostos_RespostaValida_DeveRetornarPontoComDadosCorretos |
| **Situação**        | Aprovado                                                      |

**Objetivo:** Verificar se uma resposta válida do Google Places é transformada corretamente em um ponto de coleta.

**Procedimento previsto:** Será fornecido um JSON controlado contendo place_id, nome, endereço e coordenadas. A chamada HTTP será simulada, sem acesso real à internet.

**Resultado esperado:** O teste deverá encontrar um único ponto e validar ID, nome, endereço, material e coordenadas.

Figura 12 – Evidência do código do CT006 – GooglePlacesServiceTeste


|------------------------------------------------------------------------------------------------------------|

Fonte: Elaborado pelo autor (2026).

**Resultado após execução:**



#### CT007 – Pesquisa sem resultados

| **Camada / pasta**  | WEB / ServicesTeste                               |
|---------------------|---------------------------------------------------|
| **Classe de teste** | GooglePlacesServiceTeste                          |
| **Nome do método**  | BuscarPostos_SemResultados_DeveRetornarListaVazia |
| **Situação**        | Aprovado                                          |

**Objetivo:** Verificar o retorno quando a API informa uma lista vazia de resultados.

**Procedimento previsto:** Será simulado um JSON com a propriedade results contendo um vetor vazio.

**Resultado esperado:** O teste deverá confirmar que a lista retornada está vazia.

Figura 13 – Evidência do código do CT007 – GooglePlacesServiceTeste


|---------------------------------------------------------------------------------------------------------------|

Fonte: Elaborado pelo autor (2026).

**Resultado após execução:**



#### CT008 – Falha na comunicação com a API

| **Camada / pasta**  | WEB / ServicesTeste                                   |
|---------------------|-------------------------------------------------------|
| **Classe de teste** | GooglePlacesServiceTeste                              |
| **Nome do método**  | BuscarPostos_FalhaNaRequisicao_DeveRetornarListaVazia |
| **Situação**        | Aprovado                                              |

**Objetivo:** Verificar o tratamento de uma falha HTTP durante a consulta externa.

**Procedimento previsto:** O manipulador HTTP de teste lançará HttpRequestException no momento da requisição.

**Resultado esperado:** O teste deverá confirmar que a falha é tratada e que o método retorna uma lista vazia.

Figura 14 – Evidência do código do CT008 – GooglePlacesServiceTeste


|----------------------------------------------------------------------------------------------------------------|

Fonte: Elaborado pelo autor (2026).

**Resultado após execução:**



#### CT009 – Resultado sem place_id

| **Camada / pasta**  | WEB / ServicesTeste                           |
|---------------------|-----------------------------------------------|
| **Classe de teste** | GooglePlacesServiceTeste                      |
| **Nome do método**  | BuscarPostos_SemPlaceId_DeveGerarIdSequencial |
| **Situação**        | Aprovado                                      |

**Objetivo:** Verificar a regra usada quando a resposta não possui place_id.

**Procedimento previsto:** Será fornecido um resultado válido sem o campo place_id.

**Resultado esperado:** O teste deverá confirmar que o serviço gera o identificador alternativo previsto.

Figura 15 – Evidência do código do CT009 – GooglePlacesServiceTeste


|----------------------------------------------------------------------------------------------------------------|

Fonte: Elaborado pelo autor (2026).

**Resultado após execução:**



#### CT010 – Resultado sem nome

| **Camada / pasta**  | WEB / ServicesTeste                  |
|---------------------|--------------------------------------|
| **Classe de teste** | GooglePlacesServiceTeste             |
| **Nome do método**  | BuscarPostos_SemNome_DeveUsarSemNome |
| **Situação**        | Aprovado                             |

**Objetivo:** Verificar o valor adotado quando o Google não informa o nome do local.

**Procedimento previsto:** Será simulado um resultado com identificador e coordenadas, mas sem a propriedade name.

**Resultado esperado:** O teste deverá confirmar que NomePonto recebe o texto padrão "Sem Nome".

Figura 16 – Evidência do código do CT010 – GooglePlacesServiceTeste


|----------------------------------------------------------------------------------------------------------------|

Fonte: Elaborado pelo autor (2026).

**Resultado após execução:**



#### CT011 – Resultado sem coordenadas

| **Camada / pasta**  | WEB / ServicesTeste                                          |
|---------------------|--------------------------------------------------------------|
| **Classe de teste** | GooglePlacesServiceTeste                                     |
| **Nome do método**  | BuscarPostos_SemCoordenadas_DeveManterLatitudeELongitudeZero |
| **Situação**        | Aprovado                                                     |

**Objetivo:** Verificar como o serviço trata um resultado sem geometry/location.

**Procedimento previsto:** Será fornecido um resultado sem latitude e longitude.

**Resultado esperado:** O teste deverá confirmar que Lat e Lng permanecem com valor 0.

Figura 17 – Evidência do código do CT011 – GooglePlacesServiceTeste


|----------------------------------------------------------------------------------------------------------------|

Fonte: Elaborado pelo autor (2026).

**Resultado após execução:**



#### CT012 – JSON inválido

| **Camada / pasta**  | WEB / ServicesTeste                              |
|---------------------|--------------------------------------------------|
| **Classe de teste** | GooglePlacesServiceTeste                         |
| **Nome do método**  | BuscarPostos_JsonInvalido_DeveRetornarListaVazia |
| **Situação**        | Aprovado                                         |

**Objetivo:** Verificar o tratamento de conteúdo fora do formato JSON esperado.

**Procedimento previsto:** A resposta HTTP simulada conterá texto inválido em vez de JSON.

**Resultado esperado:** O teste deverá confirmar que a falha de processamento é tratada e retorna uma lista vazia.

Figura 18 – Evidência do código do CT012 – GooglePlacesServiceTeste


|----------------------------------------------------------------------------------------------------------------|

Fonte: Elaborado pelo autor (2026).

**Resultado após execução:**



### EVIDÊNCIA GERAL DA PASTA – WEB / ServicesTeste

Figura 19 – Resultado geral dos testes – WEB / ServicesTeste no Gerenciador de Testes


|----------------------------------------------------------------------------------------------------------------|

Fonte: Elaborado pelo autor (2026).

Resumo após execução da pasta: Total: \[ 7 \] Aprovados: \[ 7 \] Falhas: \[ 0 \] Ignorados: \[ 0 \].

### 5.3 WEB / ViewModelsTeste

Os casos CT013 a CT026 avaliarão estado de tela, notificações de binding, geração de marcadores e foco no mapa.

#### CT013 – Inicialização do mapa

| **Camada / pasta**  | WEB / ViewModelsTeste                                |
|---------------------|------------------------------------------------------|
| **Classe de teste** | MapaViewModelTeste                                   |
| **Nome do método**  | Construtor_AoCriarViewModel_DeveInicializarMapaLivre |
| **Situação**        | Aprovado                                             |

**Objetivo:** Verificar o estado inicial do MapaViewModel.

**Procedimento previsto:** Uma nova instância do ViewModel será criada sem realizar consultas externas.

**Resultado esperado:** O teste deverá confirmar a existência do comando de busca, do HTML básico do mapa e do estado vazio inicial.

Figura 20 – Evidência do código do CT013 – MapaViewModelTeste


|----------------------------------------------------------------------------------------------------------------|

Fonte: Elaborado pelo autor (2026).

**Resultado após execução:**



#### CT014 – Estado vazio sem pontos

| **Camada / pasta**  | WEB / ViewModelsTeste                                             |
|---------------------|-------------------------------------------------------------------|
| **Classe de teste** | MapaViewModelTeste                                                |
| **Nome do método**  | MostrarEstadoVazio_SemPontosENaoCarregando_DeveRetornarVerdadeiro |
| **Situação**        | Aprovado                                                          |

**Objetivo:** Verificar se o estado vazio é exibido quando não existem pontos e não há carregamento.

**Procedimento previsto:** A coleção PontosAtuais será mantida vazia e IsCarregando será definido como false.

**Resultado esperado:** MostrarEstadoVazio deverá retornar verdadeiro.

Figura 21 – Evidência do código do CT014 – MapaViewModelTeste


|----------------------------------------------------------------------------------------------------------------|

Fonte: Elaborado pelo autor (2026).

**Resultado após execução:**



#### CT015 – Estado vazio com ponto

| **Camada / pasta**  | WEB / ViewModelsTeste                         |
|---------------------|-----------------------------------------------|
| **Classe de teste** | MapaViewModelTeste                            |
| **Nome do método**  | MostrarEstadoVazio_ComPonto_DeveRetornarFalso |
| **Situação**        | Aprovado                                      |

**Objetivo:** Verificar se o estado vazio é ocultado quando existe um ponto de coleta.

**Procedimento previsto:** Será adicionada uma instância de PontosColeta à coleção e o carregamento permanecerá inativo.

**Resultado esperado:** MostrarEstadoVazio deverá retornar falso.

Figura 22 – Evidência do código do CT015 – MapaViewModelTeste


|----------------------------------------------------------------------------------------------------------------|

Fonte: Elaborado pelo autor (2026).

**Resultado após execução:**



#### CT016 – Estado durante carregamento

| **Camada / pasta**  | WEB / ViewModelsTeste                                    |
|---------------------|----------------------------------------------------------|
| **Classe de teste** | MapaViewModelTeste                                       |
| **Nome do método**  | MostrarEstadoVazio_DuranteCarregamento_DeveRetornarFalso |
| **Situação**        | Aprovado                                                 |

**Objetivo:** Verificar o estado da interface durante uma operação de carregamento.

**Procedimento previsto:** A coleção permanecerá vazia, porém IsCarregando será definido como true.

**Resultado esperado:** MostrarEstadoVazio deverá retornar falso enquanto o carregamento estiver ativo.

Figura 23 – Evidência do código do CT016 – MapaViewModelTeste


|----------------------------------------------------------------------------------------------------------------|

Fonte: Elaborado pelo autor (2026).

**Resultado após execução:**



#### CT017 – Notificação da propriedade Cidade

| **Camada / pasta**  | WEB / ViewModelsTeste                      |
|---------------------|--------------------------------------------|
| **Classe de teste** | MapaViewModelTeste                         |
| **Nome do método**  | Cidade_AoAlterarValor_DeveNotificarMudanca |
| **Situação**        | Aprovado                                   |

**Objetivo:** Verificar se a alteração da cidade dispara PropertyChanged.

**Procedimento previsto:** O teste assinará o evento PropertyChanged e, em seguida, alterará a propriedade Cidade.

**Resultado esperado:** O nome da propriedade notificada deverá ser Cidade.

Figura 24 – Evidência do código do CT017 – MapaViewModelTeste


|----------------------------------------------------------------------------------------------------------------|

Fonte: Elaborado pelo autor (2026).

**Resultado após execução:**



#### CT018 – Notificação da coleção PontosAtuais

| **Camada / pasta**  | WEB / ViewModelsTeste                               |
|---------------------|-----------------------------------------------------|
| **Classe de teste** | MapaViewModelTeste                                  |
| **Nome do método**  | PontosAtuais_AoAlterarColecao_DeveNotificarMudancas |
| **Situação**        | Aprovado                                            |

**Objetivo:** Verificar as notificações produzidas quando a coleção de pontos é substituída.

**Procedimento previsto:** O evento PropertyChanged será monitorado durante a atribuição de uma nova ObservableCollection.

**Resultado esperado:** Deverão ser notificadas PontosAtuais e MostrarEstadoVazio.

Figura 25 – Evidência do código do CT018 – MapaViewModelTeste


|---------------------------------------------------------------------------------------------------------------|

Fonte: Elaborado pelo autor (2026).

**Resultado após execução:**



#### CT019 – Lista nula de marcadores

| **Camada / pasta**  | WEB / ViewModelsTeste                                |
|---------------------|------------------------------------------------------|
| **Classe de teste** | MapaViewModelTeste                                   |
| **Nome do método**  | GerarJsonMarcadores_ListaNula_DeveRetornarArrayVazio |
| **Situação**        | Aprovado                                             |

**Objetivo:** Verificar se uma referência nula é tratada de forma segura pela geração de marcadores.

**Procedimento previsto:** GerarJsonMarcadores será chamado com lista nula.

**Resultado esperado:** O retorno deverá ser o vetor JSON vazio \[\] sem lançamento de exceção.

Figura 26 – Evidência do código do CT019 – MapaViewModelTeste


|----------------------------------------------------------------------------------------------------------------|

Fonte: Elaborado pelo autor (2026).

**Resultado após execução:**



#### CT020 – Lista vazia de marcadores

| **Camada / pasta**  | WEB / ViewModelsTeste                                 |
|---------------------|-------------------------------------------------------|
| **Classe de teste** | MapaViewModelTeste                                    |
| **Nome do método**  | GerarJsonMarcadores_ListaVazia_DeveRetornarArrayVazio |
| **Situação**        | Aprovado                                              |

**Objetivo:** Verificar o JSON produzido quando não existem pontos de coleta.

**Procedimento previsto:** Será enviada uma lista vazia para GerarJsonMarcadores.

**Resultado esperado:** O retorno deverá ser exatamente \[\].

Figura 27 – Evidência do código do CT020 – MapaViewModelTeste


|----------------------------------------------------------------------------------------------------------------|

Fonte: Elaborado pelo autor (2026).

**Resultado após execução:**



#### CT021 – Geração de marcador válido

| **Camada / pasta**  | WEB / ViewModelsTeste                                     |
|---------------------|-----------------------------------------------------------|
| **Classe de teste** | MapaViewModelTeste                                        |
| **Nome do método**  | GerarJsonMarcadores_ComPontoValido_DeveGerarDadosCorretos |
| **Situação**        | Aprovado                                                  |

**Objetivo:** Verificar a estrutura gerada para um ponto de coleta válido.

**Procedimento previsto:** Será criado um ponto com nome, cidade, resíduos e coordenadas conhecidas e o JSON produzido será inspecionado.

**Resultado esperado:** O JSON deverá conter índice, nome, endereço, tipos de resíduos, latitude e longitude corretos.

Figura 28 – Evidência do código do CT021 – MapaViewModelTeste


|----------------------------------------------------------------------------------------------------------------|

Fonte: Elaborado pelo autor (2026).

**Resultado após execução:**


#### CT022 – Índices de vários marcadores

| **Camada / pasta**  | WEB / ViewModelsTeste                                           |
|---------------------|-----------------------------------------------------------------|
| **Classe de teste** | MapaViewModelTeste                                              |
| **Nome do método**  | GerarJsonMarcadores_ComVariosPontos_DeveGerarIndicesSequenciais |
| **Situação**        | Aprovado                                                        |

**Objetivo:** Verificar a numeração dos marcadores quando existem vários pontos.

**Procedimento previsto:** Serão fornecidos três pontos em uma lista ordenada.

**Resultado esperado:** O JSON deverá conter índices 0, 1 e 2 na mesma ordem dos pontos informados.

Figura 29 – Evidência do código do CT022 – MapaViewModelTeste


|----------------------------------------------------------------------------------------------------------------|

Fonte: Elaborado pelo autor (2026).

**Resultado após execução:**



#### CT023 – Coordenadas iguais a zero

| **Camada / pasta**  | WEB / ViewModelsTeste                                         |
|---------------------|---------------------------------------------------------------|
| **Classe de teste** | MapaViewModelTeste                                            |
| **Nome do método**  | GerarJsonMarcadores_CoordenadasZero_DeveUsarCoordenadasPadrao |
| **Situação**        | Aprovado                                                      |

**Objetivo:** Verificar a regra de coordenadas padrão quando o ponto possui latitude e longitude zero.

**Procedimento previsto:** Será enviado um ponto com Lat = 0 e Lng = 0.

**Resultado esperado:** O JSON deverá utilizar as coordenadas padrão definidas pelo ViewModel.

Figura 30 – Evidência do código do CT023 – MapaViewModelTeste


|----------------------------------------------------------------------------------------------------------------|

Fonte: Elaborado pelo autor (2026).

**Resultado após execução:**



#### CT024 – Formatação decimal das coordenadas

| **Camada / pasta**  | WEB / ViewModelsTeste                                              |
|---------------------|--------------------------------------------------------------------|
| **Classe de teste** | MapaViewModelTeste                                                 |
| **Nome do método**  | GerarJsonMarcadores_CoordenadasDecimais_DeveUsarPontoComoSeparador |
| **Situação**        | Aprovado                                                           |

**Objetivo:** Verificar se a serialização das coordenadas é independente da cultura regional.

**Procedimento previsto:** Será utilizado um ponto com várias casas decimais.

**Resultado esperado:** O JSON deverá usar ponto como separador decimal e não vírgula.

Figura 31 – Evidência do código do CT024 – MapaViewModelTeste


|----------------------------------------------------------------------------------------------------------------|

Fonte: Elaborado pelo autor (2026).

**Resultado após execução:**



#### CT025 – Tratamento de caracteres especiais

| **Camada / pasta**  | WEB / ViewModelsTeste                                               |
|---------------------|---------------------------------------------------------------------|
| **Classe de teste** | MapaViewModelTeste                                                  |
| **Nome do método**  | GerarJsonMarcadores_TextoComCaracteresEspeciais_DeveEscaparConteudo |
| **Situação**        | Aprovado                                                            |

**Objetivo:** Verificar o tratamento de aspas, barras e quebras de linha nos textos enviados ao mapa.

**Procedimento previsto:** Será criado um ponto contendo caracteres especiais em nome, cidade e resíduos.

**Resultado esperado:** O conteúdo deverá ser escapado ou normalizado de forma a não quebrar o JSON/JavaScript.

Figura 32 – Evidência do código do CT025 – MapaViewModelTeste


|----------------------------------------------------------------------------------------------------------------|

Fonte: Elaborado pelo autor (2026).

**Resultado após execução:**



#### CT026 – Foco em ponto do mapa

| **Camada / pasta**  | WEB / ViewModelsTeste                               |
|---------------------|-----------------------------------------------------|
| **Classe de teste** | MapaViewModelTeste                                  |
| **Nome do método**  | FocarNoPonto_MapaCarregado_DeveDefinirIndiceCorreto |
| **Situação**        | Aprovado                                            |

**Objetivo:** Verificar o índice de foco após o carregamento do mapa.

**Procedimento previsto:** Serão adicionados dois pontos à coleção; o mapa será marcado como carregado e o segundo ponto será solicitado para foco.

**Resultado esperado:** IndiceFoco deverá corresponder à posição do segundo ponto na coleção.

Figura 33 – Evidência do código do CT026 – MapaViewModelTeste


|----------------------------------------------------------------------------------------------------------------|

Fonte: Elaborado pelo autor (2026).

**Resultado após execução:**



### EVIDÊNCIA GERAL DA PASTA – WEB / ViewModelsTeste

Figura 34 – Resultado geral dos testes – WEB / ViewModelsTeste no Gerenciador de Testes


|----------------------------------------------------------------------------------------------------------------|

Fonte: Elaborado pelo autor (2026).

Resumo após execução da pasta: Total: \[ 14 \] Aprovados: \[ 14 \] Falhas: \[ 0 \] Ignorados: \[ 0 \].

## 6 Plano de testes unitários – API

Esta seção reunirá os testes das classes da ApiRestReGraphik relacionadas aos pontos de coleta. As dependências de Firebase e Google serão substituídas por respostas controladas quando o objetivo for avaliar apenas a regra da classe sob teste.

### 6.1 APITeste / ControllerTeste

Os casos CT027 a CT039 avaliarão as respostas HTTP da Controller para listagem, sincronização, consulta, cadastro, atualização e exclusão.

#### CT027 – Listagem de pontos com sucesso

| **Camada / pasta**  | API / ControllerTeste          |
|---------------------|--------------------------------|
| **Classe de teste** | PontosColetaControllerTeste    |
| **Nome do método**  | Listar_ComDados_DeveRetornarOk |
| **Situação**        | Aprovado                       |

**Objetivo:** Verificar a resposta do endpoint GET quando o serviço fornece pontos cadastrados.

**Procedimento previsto:** O serviço controlado retornará uma lista com um ponto, e o método Get será executado sem acesso real ao Firebase.

**Resultado esperado:** O Controller deverá retornar 200 OK e uma coleção de PontosColetaDto com os dados esperados.

Figura 35 – Evidência do código do CT027 – PontosColetaControllerTeste


|----------------------------------------------------------------------------------------------------------------|

Fonte: Elaborado pelo autor (2026).

**Resultado após execução:**



#### CT028 – Falha de comunicação na listagem

| **Camada / pasta**  | API / ControllerTeste                          |
|---------------------|------------------------------------------------|
| **Classe de teste** | PontosColetaControllerTeste                    |
| **Nome do método**  | Listar_FalhaComunicacao_DeveRetornarBadGateway |
| **Situação**        | Aprovado                                       |

**Objetivo:** Verificar a resposta HTTP quando a listagem sofre HttpRequestException.

**Procedimento previsto:** O serviço falso lançará uma falha de comunicação durante Listar.

**Resultado esperado:** O Controller deverá retornar 502 Bad Gateway.

Figura 36 – Evidência do código do CT028 – PontosColetaControllerTeste


|----------------------------------------------------------------------------------------------------------------|

Fonte: Elaborado pelo autor (2026).

**Resultado após execução:**



#### CT029 – Sincronização sem cidade

| **Camada / pasta**  | API / ControllerTeste                                |
|---------------------|------------------------------------------------------|
| **Classe de teste** | PontosColetaControllerTeste                          |
| **Nome do método**  | SincronizarCidade_CidadeVazia_DeveRetornarBadRequest |
| **Situação**        | Aprovado                                             |

**Objetivo:** Verificar a validação do parâmetro cidade na sincronização.

**Procedimento previsto:** SincronizarCidade será chamado com texto vazio.

**Resultado esperado:** O Controller deverá retornar 400 Bad Request sem chamar a sincronização externa.

Figura 37 – Evidência do código do CT029 – PontosColetaControllerTeste


|----------------------------------------------------------------------------------------------------------------|

Fonte: Elaborado pelo autor (2026).

**Resultado após execução:**



#### CT030 – Sincronização sem chave da API

| **Camada / pasta**  | API / ControllerTeste                                |
|---------------------|------------------------------------------------------|
| **Classe de teste** | PontosColetaControllerTeste                          |
| **Nome do método**  | SincronizarCidade_SemApiKey_DeveRetornarErroServidor |
| **Situação**        | Aprovado                                             |

**Objetivo:** Verificar a resposta quando GoogleMaps:ApiKey não está configurada.

**Procedimento previsto:** A Controller será criada com uma IConfiguration sem a chave do Google Maps.

**Resultado esperado:** O retorno deverá ser 500 Internal Server Error.

Figura 38 – Evidência do código do CT030 – PontosColetaControllerTeste


|----------------------------------------------------------------------------------------------------------------|

Fonte: Elaborado pelo autor (2026).

**Resultado após execução:**



#### CT031 – Sincronização concluída com sucesso

| **Camada / pasta**  | API / ControllerTeste                   |
|---------------------|-----------------------------------------|
| **Classe de teste** | PontosColetaControllerTeste             |
| **Nome do método**  | SincronizarCidade_Valida_DeveRetornarOk |
| **Situação**        | Aprovado                                |

**Objetivo:** Verificar a resposta da sincronização para uma cidade válida.

**Procedimento previsto:** O serviço falso informará três pontos salvos e um ignorado.

**Resultado esperado:** O Controller deverá retornar 200 OK com Mensagem, PontosSalvos = 3 e PontosIgnoradosPorDuplicidade = 1.

Figura 39 – Evidência do código do CT031 – PontosColetaControllerTeste


|----------------------------------------------------------------------------------------------------------------|

Fonte: Elaborado pelo autor (2026).

**Resultado após execução:**



#### CT032 – Busca por ID existente

| **Camada / pasta**  | API / ControllerTeste                |
|---------------------|--------------------------------------|
| **Classe de teste** | PontosColetaControllerTeste          |
| **Nome do método**  | BuscarPorId_Existente_DeveRetornarOk |
| **Situação**        | Aprovado                             |

**Objetivo:** Verificar a consulta individual de um ponto existente.

**Procedimento previsto:** O serviço controlado retornará um objeto PontosColeta para o ID informado.

**Resultado esperado:** O Controller deverá retornar 200 OK com o objeto esperado.

Figura 40 – Evidência do código do CT032 – PontosColetaControllerTeste


|---------------------------------------------------------------------------------------------------------------|

Fonte: Elaborado pelo autor (2026).

**Resultado após execução:**



#### CT033 – Busca por ID inexistente

| **Camada / pasta**  | API / ControllerTeste                        |
|---------------------|----------------------------------------------|
| **Classe de teste** | PontosColetaControllerTeste                  |
| **Nome do método**  | BuscarPorId_Inexistente_DeveRetornarNotFound |
| **Situação**        | Aprovado                                     |

**Objetivo:** Verificar a resposta para um identificador que não existe.

**Procedimento previsto:** O serviço falso retornará null para ObterPorId.

**Resultado esperado:** O Controller deverá retornar 404 Not Found.

Figura 41 – Evidência do código do CT033 – PontosColetaControllerTeste


|----------------------------------------------------------------------------------------------------------------|

Fonte: Elaborado pelo autor (2026).

**Resultado após execução:**



#### CT034 – Cadastro sem dados

| **Camada / pasta**  | API / ControllerTeste                    |
|---------------------|------------------------------------------|
| **Classe de teste** | PontosColetaControllerTeste              |
| **Nome do método**  | Cadastrar_DtoNulo_DeveRetornarBadRequest |
| **Situação**        | Aprovado                                 |

**Objetivo:** Verificar a validação de entrada no endpoint POST.

**Procedimento previsto:** O método Post será chamado com DTO nulo.

**Resultado esperado:** O Controller deverá retornar 400 Bad Request.

Figura 42 – Evidência do código do CT034 – PontosColetaControllerTeste

|----------------------------------------------------------------------------------------------------------------|

Fonte: Elaborado pelo autor (2026).

**Resultado após execução:**



#### CT035 – Cadastro válido

| **Camada / pasta**  | API / ControllerTeste                      |
|---------------------|--------------------------------------------|
| **Classe de teste** | PontosColetaControllerTeste                |
| **Nome do método**  | Cadastrar_DadosValidos_DeveRetornarCreated |
| **Situação**        | Aprovado                                   |

**Objetivo:** Verificar o fluxo de criação de um ponto de coleta válido.

**Procedimento previsto:** Será fornecido um PontosColetaDto completo e o serviço de criação será controlado pela suíte.

**Resultado esperado:** O Controller deverá retornar CreatedAtAction/201, gerar um GUID e preservar os dados principais do DTO.

Figura 43 – Evidência do código do CT035 – PontosColetaControllerTeste


|---------------------------------------------------------------------------------------------------------------|

Fonte: Elaborado pelo autor (2026).

**Resultado após execução:**



#### CT036 – Atualização de ID inexistente

| **Camada / pasta**  | API / ControllerTeste                        |
|---------------------|----------------------------------------------|
| **Classe de teste** | PontosColetaControllerTeste                  |
| **Nome do método**  | Atualizar_IdInexistente_DeveRetornarNotFound |
| **Situação**        | Aprovado                                     |

**Objetivo:** Verificar a tentativa de atualizar um ponto que não existe.

**Procedimento previsto:** ObterPorId retornará null antes da atualização.

**Resultado esperado:** O Controller deverá retornar 404 Not Found e não concluir a atualização.

Figura 44 – Evidência do código do CT036 – PontosColetaControllerTeste


|---------------------------------------------------------------------------------------------------------------|

Fonte: Elaborado pelo autor (2026).

**Resultado após execução:**



#### CT037 – Atualização válida

| **Camada / pasta**  | API / ControllerTeste                 |
|---------------------|---------------------------------------|
| **Classe de teste** | PontosColetaControllerTeste           |
| **Nome do método**  | Atualizar_DadosValidos_DeveRetornarOk |
| **Situação**        | Aprovado                              |

**Objetivo:** Verificar a atualização de um ponto existente.

**Procedimento previsto:** O serviço falso devolverá um ponto existente; um DTO com novos dados será enviado ao Put.

**Resultado esperado:** O Controller deverá retornar 200 OK e encaminhar ao serviço o ID e os valores atualizados.

Figura 45 – Evidência do código do CT037 – PontosColetaControllerTeste


|----------------------------------------------------------------------------------------------------------------|

Fonte: Elaborado pelo autor (2026).

**Resultado após execução:**



#### CT038 – Exclusão de ID inexistente

| **Camada / pasta**  | API / ControllerTeste                      |
|---------------------|--------------------------------------------|
| **Classe de teste** | PontosColetaControllerTeste                |
| **Nome do método**  | Excluir_IdInexistente_DeveRetornarNotFound |
| **Situação**        | Aprovado                                   |

**Objetivo:** Verificar a tentativa de excluir um ponto não encontrado.

**Procedimento previsto:** ObterPorId retornará null para o identificador informado.

**Resultado esperado:** O Controller deverá retornar 404 Not Found e não executar a exclusão.

Figura 46 – Evidência do código do CT038 – PontosColetaControllerTeste


|----------------------------------------------------------------------------------------------------------------|

Fonte: Elaborado pelo autor (2026).

**Resultado após execução:**



#### CT039 – Exclusão válida

| **Camada / pasta**  | API / ControllerTeste              |
|---------------------|------------------------------------|
| **Classe de teste** | PontosColetaControllerTeste        |
| **Nome do método**  | Excluir_IdExistente_DeveRetornarOk |
| **Situação**        | Aprovado                           |

**Objetivo:** Verificar o fluxo de exclusão de um ponto existente.

**Procedimento previsto:** O serviço falso retornará o ponto solicitado e registrará o ID recebido no método Excluir.

**Resultado esperado:** O Controller deverá retornar 200 OK e o ID encaminhado à exclusão deverá coincidir com o ponto existente.

Figura 47 – Evidência do código do CT039 – PontosColetaControllerTeste


|----------------------------------------------------------------------------------------------------------------|

Fonte: Elaborado pelo autor (2026).

**Resultado após execução:**



### EVIDÊNCIA GERAL DA PASTA – APITeste / ControllerTeste

Figura 48 – Resultado geral dos testes – APITeste / ControllerTeste no Gerenciador de Testes


|----------------------------------------------------------------------------------------------------------------|

Fonte: Elaborado pelo autor (2026).

Resumo após execução da pasta: Total: \[ 13 \] Aprovados: \[ 13 \] Falhas: \[ 0 \] Ignorados: \[ 0 \].

### 6.2 APITeste / ServicesApiTeste

Os casos CT040 a CT046 avaliarão validações de configuração e regras de sincronização do PontosColetaService sem acesso real ao Firebase ou Google.

#### CT040 – Configuração do Firebase ausente

| **Camada / pasta**  | API / ServicesApiTeste                               |
|---------------------|------------------------------------------------------|
| **Classe de teste** | PontosColetaServiceTeste                             |
| **Nome do método**  | ServicoPontosColeta_SemUrlFirebase_DeveLancarExcecao |
| **Situação**        | Aprovado                                             |

**Objetivo:** Verificar a validação da URL do Firebase no construtor de produção.

**Procedimento previsto:** O serviço será instanciado com uma IConfiguration sem Firebase:RealtimeDatabaseUrl.

**Resultado esperado:** A construção deverá lançar exceção com a mensagem prevista para configuração ausente.

Figura 49 – Evidência do código do CT040 – PontosColetaServiceTeste


|----------------------------------------------------------------------------------------------------------------|

Fonte: Elaborado pelo autor (2026).

**Resultado após execução:**



#### CT041 – Arquivo de credenciais ausente

| **Camada / pasta**  | API / ServicesApiTeste                                                   |
|---------------------|--------------------------------------------------------------------------|
| **Classe de teste** | PontosColetaServiceTeste                                                 |
| **Nome do método**  | ServicoPontosColeta_SemArquivoCredencial_DeveLancarFileNotFoundException |
| **Situação**        | Aprovado                                                                 |

**Objetivo:** Verificar a validação do arquivo de credenciais do Firebase.

**Procedimento previsto:** Será configurada uma URL do Firebase e um nome de arquivo inexistente.

**Resultado esperado:** A construção deverá lançar FileNotFoundException e indicar o arquivo não localizado.

Figura 50 – Evidência do código do CT041 – PontosColetaServiceTeste


|----------------------------------------------------------------------------------------------------------------|

Fonte: Elaborado pelo autor (2026).

**Resultado após execução:**



#### CT042 – Status de erro retornado pelo Google

| **Camada / pasta**  | API / ServicesApiTeste                        |
|---------------------|-----------------------------------------------|
| **Classe de teste** | PontosColetaServiceTeste                      |
| **Nome do método**  | Sincronizar_StatusErroGoogle_DeveRetornarZero |
| **Situação**        | Aprovado                                      |

**Objetivo:** Verificar a reação da sincronização a um status do Google diferente de OK e ZERO_RESULTS.

**Procedimento previsto:** A resposta HTTP simulada utilizará REQUEST_DENIED e uma lista vazia.

**Resultado esperado:** O método deverá retornar zero salvos, zero ignorados e não criar pontos.

Figura 51 – Evidência do código do CT042 – PontosColetaServiceTeste


|----------------------------------------------------------------------------------------------------------------|

Fonte: Elaborado pelo autor (2026).

**Resultado após execução:**



#### CT043 – Resposta sem resultados

| **Camada / pasta**  | API / ServicesApiTeste                  |
|---------------------|-----------------------------------------|
| **Classe de teste** | PontosColetaServiceTeste                |
| **Nome do método**  | Sincronizar_SemResults_DeveRetornarZero |
| **Situação**        | Aprovado                                |

**Objetivo:** Verificar a sincronização quando a resposta não possui a propriedade results.

**Procedimento previsto:** Será simulado um JSON com status OK, mas sem results.

**Resultado esperado:** O método deverá encerrar com contadores zerados e sem criar pontos.

Figura 52 – Evidência do código do CT043 – PontosColetaServiceTeste


|----------------------------------------------------------------------------------------------------------------|

Fonte: Elaborado pelo autor (2026).

**Resultado após execução:**



#### CT044 – Ponto duplicado por coordenadas

| **Camada / pasta**  | API / ServicesApiTeste                             |
|---------------------|----------------------------------------------------|
| **Classe de teste** | PontosColetaServiceTeste                           |
| **Nome do método**  | Sincronizar_CoordenadasDuplicadas_DeveIgnorarPonto |
| **Situação**        | Aprovado                                           |

**Objetivo:** Verificar a regra de prevenção de duplicidade pelas coordenadas.

**Procedimento previsto:** Um ponto existente será carregado em memória e o Google simulado retornará outro ponto com a mesma latitude e longitude.

**Resultado esperado:** O método deverá contabilizar um ignorado, zero salvos e não chamar o cadastro do ponto duplicado.

Figura 53 – Evidência do código do CT044 – PontosColetaServiceTeste


|----------------------------------------------------------------------------------------------------------------|

Fonte: Elaborado pelo autor (2026).

**Resultado após execução:**



#### CT045 – Novo ponto contabilizado como salvo

| **Camada / pasta**  | API / ServicesApiTeste                      |
|---------------------|---------------------------------------------|
| **Classe de teste** | PontosColetaServiceTeste                    |
| **Nome do método**  | Sincronizar_NovoPonto_DeveContabilizarSalvo |
| **Situação**        | Aprovado                                    |

**Objetivo:** Verificar o processamento de um ponto ainda inexistente na base.

**Procedimento previsto:** O Google simulado retornará um local com coordenadas novas e o método Criar será substituído por armazenamento em memória.

**Resultado esperado:** O método deverá contabilizar um salvo, zero ignorados e preparar o ponto com os valores de configuração definidos para o teste.

Figura 54 – Evidência do código do CT045 – PontosColetaServiceTeste


|---------------------------------------------------------------------------------------------------------------|

Fonte: Elaborado pelo autor (2026).

**Resultado após execução:**



#### CT046 – JSON inválido na sincronização

| **Camada / pasta**  | API / ServicesApiTeste                                       |
|---------------------|--------------------------------------------------------------|
| **Classe de teste** | PontosColetaServiceTeste                                     |
| **Nome do método**  | Sincronizar_JsonInvalido_DeveLancarInvalidOperationException |
| **Situação**        | Aprovado                                                     |

**Objetivo:** Verificar o tratamento de uma resposta externa corrompida.

**Procedimento previsto:** O HttpClient de teste retornará texto que não pode ser interpretado como JSON.

**Resultado esperado:** A sincronização deverá converter a falha de desserialização para InvalidOperationException, conforme o tratamento previsto no serviço.

Figura 55 – Evidência do código do CT046 – PontosColetaServiceTeste


|-----------------------------------------------------------------------------------------------------------------|

Fonte: Elaborado pelo autor (2026).

**Resultado após execução:**



### EVIDÊNCIA GERAL DA PASTA – APITeste / ServicesApiTeste

Figura 56 – Resultado geral dos testes – APITeste / ServicesApiTeste no Gerenciador de Testes


|-----------------------------------------------------------------------------------------------------------------|

Fonte: Elaborado pelo autor (2026).

Resumo após execução da pasta: Total: \[ 7 \] Aprovados: \[ 7 \] Falhas: \[ 0 \] Ignorados: \[ 0 \].

## 7 Registro dos resultados da suíte

| **Total planejado**    | 46                   |
|------------------------|----------------------|
| **Total executado**    | 46                   |
| **Aprovados**          | 46                   |
| **Falhas encontradas** | 0                    |
| **Falhas corrigidas**  | 0                    |
| **Resultado final**    | 46 Testes aprovados. |

## 8 Considerações finais

A realização dos testes unitários no módulo de Mapa e Pontos de Coleta do sistema ReGraphik permitiu verificar de forma organizada os principais comportamentos das camadas WEB e API relacionadas a essa funcionalidade. Foram elaborados e executados 46 casos de teste, contemplando diferentes cenários, como entradas válidas e inválidas, tratamento de valores nulos, respostas inesperadas, falhas de comunicação, validações, geração de marcadores, sincronização com serviços externos, prevenção de duplicidade, operações de cadastro, consulta, atualização e exclusão.

Ao final da execução da suíte, os 46 testes foram aprovados, sem ocorrência de falhas. Esse resultado demonstra que os comportamentos avaliados apresentaram as respostas esperadas dentro dos cenários definidos para a atividade.

Durante a preparação da suíte, também foi necessário realizar pequenas adaptações de testabilidade em classes relacionadas ao módulo, principalmente para permitir o isolamento de dependências externas como Google Places e Firebase. Essas adaptações não alteraram as regras de negócio nem o funcionamento normal da aplicação, servindo apenas para possibilitar a execução de testes de forma controlada, repetível e independente de conexões externas.

A atividade contribuiu para validar o funcionamento do módulo e também para demonstrar a importância dos testes unitários no desenvolvimento de software. A utilização do xUnit, juntamente com a organização dos testes no padrão Arrange, Act e Assert, facilitou a leitura dos cenários, a identificação dos resultados esperados e a manutenção da suíte.

Dessa forma, a suíte desenvolvida cumpriu o objetivo proposto, finalizando a validação do módulo de Mapa e Pontos de Coleta com **46 testes executados, 46 aprovados e nenhuma falha**, fornecendo maior segurança para a continuidade e manutenção do sistema ReGraphik.

## Referências

SENAI. Aula 02 – Criando o primeiro projeto xUnit.net. Material didático do curso Técnico em Desenvolvimento de Sistemas. Nova Lima, 2026.

SENAI. Aula 03 – Escrevendo os primeiros testes com xUnit.net. Material didático do curso Técnico em Desenvolvimento de Sistemas. Nova Lima, 2026.

SENAI. Situação de Aprendizagem – Suíte Individual de Testes Unitários Aplicada ao Projeto de TCC. Nova Lima, 2026.

REGRAPHIK. Código-fonte do módulo Mapa e Pontos de Coleta. Projeto de TCC, 2026.
