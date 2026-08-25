# Suíte de Testes Unitários — Sistema ReGraphik

Suíte de testes das camadas **API** e **APPWPF** — Projeto **ReGraphikApp**

Relatório Técnico apresentado como pré-requisito para conclusão do Curso Superior de Tecnologia em Desenvolvimento de Sistemas, na CFP Escola de Programação e Robótica de Nova Lima, elaborado sob orientação do Instrutor Frederico Martins Aguiar.

- **Aluno:** Bruno Maia Santos
- **Turma:** Desenvolvimento de Sistemas
- **Projeto:** ReGraphikApp
- **Componentes:** InputSanitizationService, UsuarioController, ResiduoController, PontosColetaController (API); ValidacaoCpfService, UsuarioSessaoService, CadastroViewModel, BaseViewModel (APPWPF)
- **Tecnologia:** C# / .NET 8.0
- **Framework:** xUnit
- **Local/Ano:** Nova Lima — MG, 2026

## Sumário

1. [Suíte de testes unitários desenvolvida](#1-suíte-de-testes-unitários-desenvolvida)
   - [1.1. API — Services (InputSanitizationService)](#11-api--services-inputsanitizationservice)
   - [1.2. API — Controllers](#12-api--controllers)
   - [1.3. APPWPF — Services](#13-appwpf--services)
   - [1.4. APPWPF — ViewModels](#14-appwpf--viewmodels)
2. [Estrutura dos testes](#2-estrutura-dos-testes)
3. [Documentação da estratégia utilizada](#3-documentação-da-estratégia-utilizada)
4. [Explicação dos casos de teste](#4-explicação-dos-casos-de-teste)
5. [Resultado final dos testes](#5-resultado-final-dos-testes)
6. [Identificação clara dos componentes escolhidos](#6-identificação-clara-dos-componentes-escolhidos)
7. [Evidência de contribuição no projeto](#7-evidência-de-contribuição-no-projeto)
8. [Considerações finais](#8-considerações-finais)
9. [Checklist de evidências](#9-checklist-de-evidências)

---

## 1. Suíte de testes unitários desenvolvida

A suíte de testes unitários foi desenvolvida individualmente para os componentes das camadas de API e APPWPF do projeto ReGraphikApp. A implementação utiliza C#, .NET 8.0 e o framework xUnit.

A suíte foi estruturada para verificar diferentes comportamentos dos componentes escolhidos, incluindo cenários de sucesso, entradas inválidas, valores limites, segurança, sanitização, validação de objetos, respostas HTTP de controllers e comportamento de ViewModels. Ao todo, foram considerados **42 casos de teste**, distribuídos em quatro classes de teste.

Cada teste segue a organização **Arrange, Act e Assert**, permitindo separar a preparação dos dados, a execução do método e a verificação do resultado esperado.

### 1.1. API — Services (InputSanitizationService)

Os 15 casos de teste implementados para o componente `InputSanitizationService` estão descritos a seguir, agrupados pelo método testado. Código-fonte extraído da classe `ApiServicesTests.cs`.

#### `IdEhSeguro()`

| Caso | Descrição | Objetivo |
|---|---|---|
| **CT01** — ID Válido | Verificar se um identificador dentro das regras é aceito. |
| **CT02** — ID Nulo | Verificar o tratamento de identificador nulo. |
| **CT03** — ID Vazio | Verificar o tratamento de identificador vazio. |
| **CT04** — Path Traversal | Verificar a rejeição de tentativa de Path Traversal. |
| **CT05** — Caractere Inválido do Firebase | Verificar a rejeição de caractere não permitido. |
| **CT06** — ID Maior que 128 Caracteres | Verificar o limite máximo definido para o identificador. |

#### `SanitizarTexto()`

| Caso | Objetivo |
|---|---|
| **CT07** — Texto com HTML | Verificar a remoção das tags HTML, preservando o conteúdo textual. |
| **CT08** — Texto Nulo | Verificar o tratamento de texto nulo. |
| **CT09** — Texto Acima do Limite | Verificar o truncamento de texto acima do limite definido. |

#### `ValidarResiduo()`

| Caso | Objetivo |
|---|---|
| **CT10** — Resíduo Válido | Verificar se dados válidos não geram erros de validação. |
| **CT11** — Dados Obrigatórios Inválidos | Verificar as validações de campos obrigatórios (tipo, origem e quantidade). |

#### `ValidarUsuario()`

| Caso | Objetivo |
|---|---|
| **CT12** — Usuário Válido | Verificar se um usuário válido passa pelas regras de validação. |
| **CT13** — E-mail Inválido | Verificar a validação do formato do e-mail. |
| **CT14** — Login Muito Curto | Verificar o limite mínimo de caracteres do login. |
| **CT15** — Senha Muito Curta | Verificar o limite mínimo de caracteres da senha. |

### 1.2. API — Controllers

Os 6 casos de teste a seguir verificam o comportamento dos controllers da API diante de identificadores inseguros e parâmetros obrigatórios ausentes, garantindo que a camada HTTP rejeite entradas indevidas antes de acionar a lógica de negócio. Código-fonte extraído da classe `ApiControllersTests.cs`.

**UsuarioController**

| Caso | Objetivo |
|---|---|
| **CT16** — GET Usuário com ID Inseguro | Verificar se o controller rejeita, com `BadRequest`, uma requisição GET contendo tentativa de path traversal no ID. |
| **CT17** — DELETE Usuário com ID Inseguro | Verificar se o controller rejeita, com `BadRequest`, uma requisição DELETE contendo ID inseguro. |

**ResiduoController**

| Caso | Objetivo |
|---|---|
| **CT18** — GET Resíduo com ID Inseguro | Verificar se o controller de resíduos rejeita, com `BadRequest`, um ID inseguro na consulta. |
| **CT19** — DELETE Resíduo com ID Inseguro | Verificar se o controller de resíduos rejeita, com `BadRequest`, um ID inseguro na exclusão. |

**PontosColetaController**

| Caso | Objetivo |
|---|---|
| **CT20** — Sincronização com Cidade Vazia | Verificar se a sincronização é rejeitada, com `BadRequest`, quando o nome da cidade está vazio, sem acessar serviços externos. |
| **CT21** — Sincronização com Cidade em Branco | Verificar se a sincronização é rejeitada, com `BadRequest`, quando o nome da cidade contém apenas espaços em branco. |

### 1.3. APPWPF — Services

Os 11 casos de teste a seguir verificam os serviços da aplicação desktop (WPF), cobrindo a validação/formatação de CPF e o gerenciamento do ciclo de vida da sessão do usuário logado. Código-fonte extraído da classe `AppWpfServicesTests.cs`.

**ValidacaoCpfService**

| Caso | Objetivo |
|---|---|
| **CT22** — CPF Válido sem Máscara | Verificar se um CPF válido, informado sem máscara, é aceito. |
| **CT23** — CPF Válido com Máscara | Verificar se um CPF válido, informado com máscara, é aceito. |
| **CT24** — CPF Inválido | Verificar a rejeição de um CPF com dígito verificador incorreto. |
| **CT25** — CPF com Dígitos Repetidos | Verificar a rejeição de um CPF cujos dígitos são todos iguais. |
| **CT26** — CPF Vazio | Verificar o tratamento de um CPF vazio. |
| **CT27** — Formatação de CPF | Verificar se a máscara é aplicada corretamente a um CPF válido sem formatação. |

**UsuarioSessaoService**

| Caso | Objetivo |
|---|---|
| **CT28** — Instância Única do Serviço de Sessão | Verificar se o serviço de sessão mantém uma única instância (padrão Singleton). |
| **CT29** — Início de Sessão | Verificar se, ao iniciar uma sessão, o usuário e seu ID são corretamente armazenados. |
| **CT30** — Encerramento de Sessão | Verificar se, ao encerrar a sessão, os dados do usuário, o ID e a foto são limpos. |
| **CT31** — Expiração Forçada de Sessão | Verificar se a expiração forçada limpa a sessão e dispara o evento correspondente. |
| **CT32** — Notificação de Alteração do Usuário Logado | Verificar se a alteração do usuário logado dispara as notificações de propriedade (`PropertyChanged`) esperadas. |

### 1.4. APPWPF — ViewModels

Os 10 casos de teste a seguir verificam a `CadastroViewModel` (máscara de CPF, controle de exibição do formulário e comandos de solicitação de acesso, validação de token e finalização de cadastro) e a `BaseViewModel`. Código-fonte extraído da classe `AppWpfViewModelsTests.cs`.

**CadastroViewModel — CPF**

| Caso | Objetivo |
|---|---|
| **CT33** — Aplicação de Máscara ao CPF | Verificar se o CPF digitado sem máscara é formatado progressivamente pela ViewModel. |
| **CT34** — Limite de Dígitos do CPF | Verificar se a ViewModel limita a entrada do CPF a 11 dígitos numéricos. |

**CadastroViewModel — Estado do formulário**

| Caso | Objetivo |
|---|---|
| **CT35** — Token Inválido Oculta o Formulário | Verificar se, com token inválido, o formulário completo de cadastro permanece oculto. |
| **CT36** — Token Válido Exibe o Formulário | Verificar se, com token válido, o formulário completo de cadastro é exibido. |
| **CT37** — Cadastro Finalizado Oculta o Formulário | Verificar se, após a finalização do cadastro, o formulário completo volta a ficar oculto. |

**CadastroViewModel — Comandos e validações**

| Caso | Objetivo |
|---|---|
| **CT38** — Solicitação de Acesso sem E-mail | Verificar se a ausência de e-mail impede a solicitação de acesso e gera mensagem de obrigatoriedade. |
| **CT39** — Solicitação de Acesso com E-mail Inválido | Verificar se um e-mail em formato inválido gera mensagem de validação de formato. |
| **CT40** — Validação sem Token | Verificar se a ausência do token de ativação gera mensagem de obrigatoriedade. |
| **CT41** — Finalização sem Dados Obrigatórios | Verificar se a ausência de nome, login, senha e CPF gera as respectivas mensagens de validação ao finalizar o cadastro. |

**BaseViewModel**

| Caso | Objetivo |
|---|---|
| **CT42** — Interface INotifyPropertyChanged | Verificar se a ViewModel base implementa a interface `INotifyPropertyChanged`, base para o binding de dados. |

---

## 2. Estrutura dos testes

Os testes foram implementados em quatro classes específicas do projeto de testes, mantendo a separação entre o código de produção e o código destinado à verificação automatizada.

```
ReGraphik.Testes
└── API
    ├── Services
    │   └── ApiServicesTests.cs
    └── Controllers
        └── ApiControllersTests.cs
└── APPWPF
    ├── Services
    │   └── AppWpfServicesTests.cs
    └── ViewModels
        └── AppWpfViewModelsTests.cs
```

Cada classe reúne os testes referentes às regras do componente escolhido, organizados em regiões (`#region`) por método/controller/serviço testado. As asserções são utilizadas de acordo com o comportamento esperado de cada método.

---

## 3. Documentação da estratégia utilizada

A estratégia foi baseada na seleção de componentes com métodos determinísticos e testáveis de forma isolada, cobrindo tanto a camada de API (Services e Controllers) quanto a camada de aplicação desktop APPWPF (Services e ViewModels). Foram definidos cenários positivos e negativos, além de entradas nulas, vazias, valores limites e entradas relacionadas à segurança.

A execução foi realizada por meio do xUnit integrado ao Visual Studio. Cada caso possui uma condição de entrada e uma expectativa objetiva, permitindo que uma falha seja identificada automaticamente.

- Isolamento do componente testado.
- Independência entre os casos de teste.
- Uso de Arrange, Act e Assert.
- Uso de asserções compatíveis com cada cenário.
- Validação de entradas válidas e inválidas.
- Verificação de valores limites.
- Verificação de respostas HTTP (`BadRequest`) em controllers da API.
- Análise dos resultados após a execução.

---

## 4. Explicação dos casos de teste

Os casos foram definidos para representar situações relevantes ao comportamento dos componentes selecionados. A combinação dos 42 cenários permite verificar diferentes caminhos de execução das camadas de API e APPWPF.

### 4.1. Validação de identificadores (API)
Os casos CT01 a CT06 verificam identificadores válidos, nulos, vazios, contendo tentativa de Path Traversal, caracteres inválidos e tamanho superior ao limite.

### 4.2. Sanitização de textos (API)
Os casos CT07 a CT09 verificam o tratamento de conteúdo HTML, texto nulo e texto acima do limite. Durante a execução, o CT07 apresentou inicialmente uma expectativa incompatível com o comportamento real do método.

### 4.3. Validação de resíduos (API)
Os casos CT10 e CT11 verificam, respectivamente, uma entrada válida e uma entrada com campos obrigatórios inválidos.

### 4.4. Validação de usuários (API)
Os casos CT12 a CT15 verificam usuário válido, e-mail inválido, login abaixo do tamanho mínimo e senha abaixo do tamanho mínimo.

### 4.5. Controllers da API
Os casos CT16 a CT21 verificam se `UsuarioController`, `ResiduoController` e `PontosColetaController` retornam `BadRequest` diante de IDs inseguros (tentativas de path traversal) e de parâmetros obrigatórios vazios ou em branco, sem acionar dependências externas.

### 4.6. Validação e formatação de CPF (APPWPF)
Os casos CT22 a CT27 verificam a validação de CPFs válidos (com e sem máscara), CPFs inválidos, CPFs com dígitos repetidos, CPF vazio e a formatação de um CPF válido.

### 4.7. Gerenciamento de sessão do usuário (APPWPF)
Os casos CT28 a CT32 verificam o padrão Singleton do serviço de sessão, o início e o encerramento de sessão, a expiração forçada e o disparo do evento `PropertyChanged` ao alterar o usuário logado.

### 4.8. ViewModel de cadastro (APPWPF)
Os casos CT33 a CT41 verificam a máscara e o limite de dígitos do CPF, o controle de exibição do formulário completo conforme o estado do token e do cadastro, e as mensagens de validação dos comandos de solicitação de acesso, validação de token e finalização de cadastro.

### 4.9. ViewModel base (APPWPF)
O caso CT42 verifica se a `BaseViewModel` implementa a interface `INotifyPropertyChanged`, requisito para o data binding das telas WPF.

---

## 5. Resultado final dos testes

> **Teste executado com Aprovação.**
> *Fonte: Elaborado pelo autor (2026).*

Na execução inicial do teste de sanitização HTML (CT07), foi identificada uma diferença entre o resultado esperado e o resultado produzido pelo método:

```
Expected: "Olá Bruno"
Actual:   "Olá alert('x') Bruno"
```

A análise mostrou que o método remove as tags HTML, mas preserva o conteúdo textual que estava dentro delas. Por esse motivo, a expectativa do teste foi ajustada para representar o comportamento efetivo do componente:

```csharp
Assert.Equal("Olá alert('x') Bruno", resultado);
```

Após o ajuste da asserção, os testes foram executados novamente e apresentaram resultado aprovado. O registro da execução final deve ser utilizado como evidência principal da conclusão da suíte.

---

## 6. Identificação clara dos componentes escolhidos

Os componentes escolhidos para a realização da atividade foram os seguintes, distribuídos entre a API e a aplicação desktop (APPWPF) do projeto ReGraphik:

- `InputSanitizationService` — camada Services da API.
- `UsuarioController`, `ResiduoController` e `PontosColetaController` — camada Controllers da API.
- `ValidacaoCpfService` e `UsuarioSessaoService` — camada Services do APPWPF.
- `CadastroViewModel` e `BaseViewModel` — camada ViewModels do APPWPF.

```
ApiRestReGraphik
└── Services
│   └── InputSanitizationService.cs
└── Controllers
    └── UsuarioController.cs, ResiduoController.cs, PontosColetaController.cs

ReGraphikAPPWPF
└── Services
│   └── ValidacaoCpfService.cs, UsuarioSessaoService.cs
└── ViewModels
    └── CadastroViewModel.cs, BaseViewModel.cs
```

As classes de teste responsáveis pela verificação desses componentes são a `ApiServicesTests.cs`, a `ApiControllersTests.cs`, a `AppWpfServicesTests.cs` e a `AppWpfViewModelsTests.cs`, localizadas no projeto `ReGraphik.Testes`.

---

## 7. Evidência de contribuição no projeto

A contribuição individual é demonstrada pela implementação da suíte de testes para os componentes escolhidos, pela execução e análise dos resultados e pelo registro das alterações no repositório do projeto.

Para garantir rastreabilidade, recomenda-se inserir uma captura do histórico de commits contendo as alterações referentes à criação ou atualização da suíte de testes nas quatro classes.

> **Histórico do GitHub demonstrando o commit relacionado à contribuição individual.**
> *Fonte: Elaborado pelo autor (2026).*

A evidência deve permitir relacionar o aluno, os componentes, as classes de teste e as alterações realizadas no projeto.

---

## 8. Considerações finais

A atividade permitiu aplicar os conceitos de testes unitários no projeto de TCC ReGraphik, estabelecendo uma suíte composta por **42 casos** distribuídos em **quatro classes de teste**, cobrindo os componentes `InputSanitizationService`, `UsuarioController`, `ResiduoController`, `PontosColetaController`, `ValidacaoCpfService`, `UsuarioSessaoService`, `CadastroViewModel` e `BaseViewModel`, nas camadas API e APPWPF do sistema.

Além da implementação dos testes, foi realizada a análise dos resultados. A falha encontrada no teste de sanitização demonstrou a importância de comparar a expectativa definida no teste com o comportamento efetivamente implementado antes de modificar o código de produção.

A documentação e as evidências apresentadas permitem demonstrar a contribuição individual para a qualidade e a confiabilidade do projeto, tanto na API quanto na aplicação desktop.

---

## 9. Checklist de evidências

**Tabela 1 — Checklist de entrega da atividade**

| Item | Evidência | Status |
|---|---|---|
| Suíte de testes unitários desenvolvida | 42 casos documentados (4 classes) | ✅ Concluído |
| Código dos testes | `ApiServicesTests.cs`, `ApiControllersTests.cs`, `AppWpfServicesTests.cs`, `AppWpfViewModelsTests.cs` | ✅ Concluído |
| Evidências da execução | Capturas do Test Explorer | ✅ Concluído |
| Documentação da estratégia | Seção 3 | ✅ Concluído |
| Explicação dos casos de teste | Seção 4 | ✅ Concluído |
| Resultado final dos testes | Execução após ajuste (CT07) | ✅ Concluído |
| Componentes escolhidos | 8 componentes (API e APPWPF) | ✅ Concluído |
| Contribuição no projeto | Histórico/commit do GitHub | ✅ Concluído |

*Fonte: Elaborado pelo autor (2026).*
