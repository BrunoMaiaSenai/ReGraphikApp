
# Suíte de Testes Unitários — Sistema ReGraphik

Suíte de testes do **Módulo de Gestão do Estoque Reverso e Resíduos** — Projeto **ReGraphik**

Relatório Técnico apresentado como pré-requisito para conclusão do Curso Superior de Tecnologia em Desenvolvimento de Sistemas, na CFP Escola de Programação e Robótica de Nova Lima, elaborado sob orientação do Instrutor Frederico Martins Aguiar.

* **Aluno:** Kaio Alves Gonzaga Silva
* **Equipe:** ReGraphik
* **Componente escolhido:** Módulo de Gestão do Estoque Reverso e Resíduos
* **Componentes testados:** `Residuo`, `ResiduoService`, `ResiduoViewModel` e `EstoqueReversoViewModel`
* **Tecnologia:** C# / .NET / WPF
* **Framework:** xUnit
* **Local/Ano:** Nova Lima — MG, 2026

## Sumário

1. [Suíte de testes unitários desenvolvida](#1-suíte-de-testes-unitários-desenvolvida)

   * [1.1. Model — Residuo](#11-model--residuo)
   * [1.2. Services — ResiduoService](#12-services--residuoservice)
   * [1.3. ViewModel — Cadastro](#13-viewmodel--cadastro)
   * [1.4. ViewModel — Listagem e Filtros](#14-viewmodel--listagem-e-filtros)
2. [Estrutura dos testes](#2-estrutura-dos-testes)
3. [Documentação da estratégia utilizada](#3-documentação-da-estratégia-utilizada)
4. [Explicação dos casos de teste](#4-explicação-dos-casos-de-teste)
5. [Resultado final dos testes](#5-resultado-final-dos-testes)
6. [Identificação clara dos componentes escolhidos](#6-identificação-clara-dos-componentes-escolhidos)
7. [Contribuição e melhorias realizadas](#7-contribuição-e-melhorias-realizadas)
8. [Considerações finais](#8-considerações-finais)
9. [Checklist de evidências](#9-checklist-de-evidências)

---

## 1. Suíte de testes unitários desenvolvida

A suíte de testes unitários foi desenvolvida para o **Módulo de Gestão do Estoque Reverso e Resíduos** do projeto ReGraphik.

A implementação utiliza **C#**, **.NET**, **WPF** e o framework **xUnit**. Os testes foram estruturados para verificar a formatação de identificadores, validações de regras de negócio, campos obrigatórios, valores numéricos, comunicação com a API e filtragem dos resíduos.

Ao todo, foram desenvolvidos casos de teste para validar diferentes cenários dos componentes escolhidos, incluindo entradas nulas, vazias, inválidas, valores limites, cenários de sucesso, falhas da API e entradas inesperadas.

Cada teste segue a organização **Arrange, Act e Assert**, permitindo separar a preparação dos dados, a execução do método e a verificação do resultado esperado.

### 1.1. Model — Residuo

O componente `Residuo` foi testado para garantir a correta formatação do identificador utilizado nos cards da interface.

#### `IdCard`

| Caso                                    | Descrição                                                              | Objetivo                                                                                   |
| --------------------------------------- | ---------------------------------------------------------------------- | ------------------------------------------------------------------------------------------ |
| **CT006** — Formatação do Identificador | Testa IDs nulos, vazios, curtos, com exatamente 8 caracteres e longos. | Garantir que o identificador seja formatado corretamente para exibição resumida nos cards. |

O comportamento esperado é:

* ID nulo ou vazio: `#00000000`
* ID com até 8 caracteres: exibição completa precedida por `#`
* ID com mais de 8 caracteres: exibição dos primeiros 8 caracteres precedidos por `#`

Exemplo:

```csharp
public string IdCard
{
    get
    {
        if (string.IsNullOrEmpty(Id)) return "#00000000";
        return Id.Length <= 8 ? $"#{Id}" : $"#{Id.Substring(0, 8)}";
    }
}
```

---

### 1.2. Services — ResiduoService

O componente `ResiduoService` foi testado para garantir que os dados sejam validados antes de qualquer comunicação com a API ou Firebase.

#### `AtualizarStatusAsync()`

| Caso                                    | Objetivo                                                                             |
| --------------------------------------- | ------------------------------------------------------------------------------------ |
| **CT007** — Validação ao Alterar Status | Verificar o tratamento de resíduo nulo, ID nulo ou em branco e novo status inválido. |

As seguintes situações são validadas:

* Resíduo nulo.
* ID nulo.
* ID vazio.
* ID contendo apenas espaços.
* Novo status nulo.
* Novo status vazio ou em branco.

Quando os dados são inválidos, o método deve lançar:

```text
ArgumentNullException
ArgumentException
```

A validação ocorre antes de qualquer requisição externa.

---

### 1.3. ViewModel — Cadastro

Os testes da `ResiduoViewModel` verificam as regras relacionadas ao cadastro de resíduos.

#### Validação de campos obrigatórios

| Caso                            | Objetivo                                                                            |
| ------------------------------- | ----------------------------------------------------------------------------------- |
| **CT008** — Campos Obrigatórios | Verificar se o cadastro é bloqueado quando Tipo de Material ou Origem estão vazios. |

Os campos testados são:

* `TipoMaterial`
* `Origem`

Quando algum campo obrigatório está vazio, a respectiva mensagem de erro é preenchida e o método retorna antes de realizar a requisição HTTP.

#### Validação de valores numéricos

| Caso                                 | Objetivo                                                                                  |
| ------------------------------------ | ----------------------------------------------------------------------------------------- |
| **CT009** — Valores Numéricos Limite | Verificar se Quantidade, Largura e Comprimento iguais a zero ou negativos são rejeitados. |

Os valores de limite testados incluem:

```text
0
-1
-50
```

A regra esperada é que todos os valores numéricos sejam maiores que zero.

#### Envio do cadastro à API

| Caso                    | Objetivo                                                                       |
| ----------------------- | ------------------------------------------------------------------------------ |
| **CT010** — Envio à API | Verificar o cadastro com sucesso e o tratamento de falhas retornadas pela API. |

O teste valida dois cenários principais:

* API respondendo com sucesso.
* API respondendo com erro.

Durante a implementação dos testes, foi identificado um problema de testabilidade: o `HttpClient` era criado diretamente dentro do método.

### Código anterior

```csharp
using var client = new HttpClient();
```

Essa abordagem dificultava a utilização de respostas simuladas durante os testes automatizados.

### Solução aplicada

Foi realizada a injeção do `HttpClient`:

```csharp
private readonly HttpClient _httpClient;

public ResiduoViewModel() : this(new HttpClient()) { }

public ResiduoViewModel(HttpClient httpClient)
{
    _httpClient = httpClient;
}
```

Também foi aplicada uma proteção para o caso de `Application.Current` ser nulo durante a execução dos testes:

```csharp
Application.Current?.Dispatcher.Invoke(() =>
{
    // Código de interface
});
```

Com essas alterações, foi possível utilizar um `HttpClient` falso e testar o comportamento da aplicação sem realizar requisições reais.

---

### 1.4. ViewModel — Listagem e Filtros

O componente `EstoqueReversoViewModel` foi testado para validar o comportamento da filtragem dos resíduos cadastrados.

#### Filtragem por Tipo, Origem e Status

| Caso                           | Objetivo                                                                    |
| ------------------------------ | --------------------------------------------------------------------------- |
| **CT011** — Filtros Combinados | Verificar a filtragem por Tipo, Origem e Status, incluindo a opção `Todos`. |

Os testes verificam:

* Correspondência por Tipo.
* Correspondência por Origem.
* Correspondência por Status.
* Combinação de filtros.
* Diferenças entre maiúsculas e minúsculas.
* Opção `Todos`.

O filtro por tipo utiliza comparação sem diferenciação entre letras maiúsculas e minúsculas.

Exemplo:

```text
Filtro: vinil
Resíduo: Adesivo VINIL

Resultado: Incluído
```

#### Filtragem por Período e entrada inesperada

| Caso                                     | Objetivo                                                                                 |
| ---------------------------------------- | ---------------------------------------------------------------------------------------- |
| **CT012** — Período e Entrada Inesperada | Verificar a filtragem por período e o tratamento seguro de objetos que não são resíduos. |

Os cenários incluem:

* Resíduo cadastrado hoje.
* Resíduo cadastrado dentro do período.
* Resíduo cadastrado há 100 dias.
* Filtro `Todos`.
* Entrada de objeto inesperado.

Exemplo:

```text
Filtro: Últimos 7 dias

Resíduo de hoje:
Incluído

Resíduo de 100 dias:
Excluído
```

Quando a entrada recebida não é um objeto `Residuo`, o método retorna:

```text
false
```

Sem lançar exceções.

---

## 2. Estrutura dos testes

Os testes foram organizados de acordo com os componentes do módulo de Estoque Reverso e Resíduos.

```text
ReGraphik
│
├── Models
│   └── Residuo.cs
│
├── Services
│   └── ResiduoService.cs
│
├── ViewModels
│   ├── ResiduoViewModel.cs
│   └── EstoqueReversoViewModel.cs
│
└── Testes
    ├── CT006 - Formatação do Identificador
    ├── CT007 - Atualização de Status
    ├── CT008 - Campos Obrigatórios
    ├── CT009 - Valores Numéricos
    ├── CT010 - Envio à API
    ├── CT011 - Filtros Combinados
    └── CT012 - Filtro por Período
```

Os casos foram organizados por componente e método testado, utilizando a estrutura `Arrange`, `Act` e `Assert`.

---

## 3. Documentação da estratégia utilizada

A estratégia de testes foi baseada na seleção de métodos e componentes importantes para o funcionamento do módulo de gestão de resíduos.

Foram definidos cenários positivos e negativos, incluindo:

* Entradas válidas.
* Entradas nulas.
* Campos vazios.
* Campos contendo apenas espaços.
* Valores iguais a zero.
* Valores negativos.
* Valores limites.
* Entradas inesperadas.
* Respostas de sucesso da API.
* Respostas de falha da API.
* Filtragem combinada de dados.

A execução foi realizada utilizando o **xUnit**, com os testes estruturados de forma isolada sempre que possível.

A estratégia utilizada incluiu:

* Isolamento do componente testado.
* Uso do padrão Arrange, Act e Assert.
* Validação de entradas válidas e inválidas.
* Verificação de valores limites.
* Verificação de mensagens de erro.
* Simulação de respostas da API.
* Teste de comportamentos de sucesso e falha.
* Verificação de entradas inesperadas.
* Análise dos resultados após a execução.

---

## 4. Explicação dos casos de teste

Os casos de teste foram definidos para representar situações importantes para o comportamento do módulo de Estoque Reverso e Resíduos.

### 4.1. Formatação do identificador

O caso **CT006** verifica a propriedade `IdCard`, garantindo o comportamento correto para IDs nulos, vazios, curtos, com oito caracteres e com mais de oito caracteres.

### 4.2. Atualização de status

O caso **CT007** verifica se `AtualizarStatusAsync` valida o resíduo, o identificador e o novo status antes de realizar qualquer comunicação externa.

### 4.3. Campos obrigatórios

O caso **CT008** verifica se a ausência de Tipo de Material ou Origem impede o envio do cadastro e apresenta a mensagem de erro correspondente.

### 4.4. Valores numéricos

O caso **CT009** verifica se Quantidade, Largura e Comprimento iguais a zero ou negativos são rejeitados.

### 4.5. Comunicação com a API

O caso **CT010** verifica o comportamento do cadastro diante de respostas de sucesso e erro da API.

Esse teste também contribuiu para a melhoria do código, permitindo a injeção de um `HttpClient` durante os testes.

### 4.6. Filtragem por critérios

O caso **CT011** verifica a filtragem combinada por Tipo, Origem e Status, incluindo a opção `Todos` e diferenças de capitalização.

### 4.7. Filtragem por período

O caso **CT012** verifica o filtro de período e garante que objetos inesperados sejam tratados com segurança.

---

## 5. Resultado final dos testes

> **30 de 30 testes aprovados.**
>
> *Fonte: Elaborado pelo autor (2026).*

Após a correção dos problemas encontrados durante o desenvolvimento dos testes, a execução final apresentou aprovação completa.

```text
Testes executados: 30
Testes aprovados: 30
Testes reprovados: 0
```

A melhoria realizada no `ResiduoViewModel` permitiu testar corretamente o envio para a API sem utilizar a rede real e também eliminou o problema causado pela ausência de `Application.Current` durante os testes automatizados.

---

## 6. Identificação clara dos componentes escolhidos

Os componentes escolhidos para a atividade foram:

* `Residuo` — Model responsável pelos dados e pela formatação do identificador.
* `ResiduoService` — Service responsável pelas regras relacionadas à atualização do status.
* `ResiduoViewModel` — ViewModel responsável pelo cadastro e validação dos resíduos.
* `EstoqueReversoViewModel` — ViewModel responsável pela listagem e filtragem.

```text
ReGraphik
│
├── Models
│   └── Residuo.cs
│
├── Services
│   └── ResiduoService.cs
│
└── ViewModels
    ├── ResiduoViewModel.cs
    └── EstoqueReversoViewModel.cs
```

Esses componentes abrangem diferentes responsabilidades do módulo, incluindo estrutura de dados, regras de negócio, validações, comunicação com API e filtragem.

---

## 7. Contribuição e melhorias realizadas

A contribuição individual para o projeto foi demonstrada pela implementação dos testes automatizados e pela análise dos problemas identificados durante sua execução.

A principal melhoria identificada foi a necessidade de tornar o `ResiduoViewModel` mais testável.

O código original criava o `HttpClient` diretamente no método, impedindo sua substituição por um componente simulado durante os testes.

Após a alteração, o `HttpClient` passou a ser recebido pelo construtor, permitindo a utilização de um handler falso:

```csharp
var handlerFalso = new HttpMessageHandlerFalso(HttpStatusCode.OK);
var httpClientFalso = new HttpClient(handlerFalso);
var vm = new ResiduoViewModel(httpClientFalso);
```

Também foi necessário adaptar chamadas dependentes da interface WPF para que funcionassem quando `Application.Current` fosse nulo durante os testes.

Essas melhorias permitiram validar o fluxo de cadastro de forma isolada e sem dependência da rede real.

---

## 8. Considerações finais

A atividade permitiu aplicar conceitos de testes unitários ao Módulo de Gestão do Estoque Reverso e Resíduos do projeto ReGraphik.

Os testes realizados cobriram:

* Formatação de dados.
* Validação de parâmetros.
* Campos obrigatórios.
* Valores numéricos.
* Comunicação com a API.
* Tratamento de erros.
* Filtragem combinada.
* Filtragem por período.
* Entradas inesperadas.

Além de validar as funcionalidades, o processo de testes contribuiu para identificar melhorias relacionadas à arquitetura e à testabilidade do código.

O resultado final de **30 testes aprovados** demonstra que os cenários avaliados apresentaram comportamento consistente de acordo com as regras implementadas.

---

## 9. Checklist de evidências

**Tabela 1 — Checklist de entrega da atividade**

| Item                                   | Evidência                                                                                          | Status      |
| -------------------------------------- | -------------------------------------------------------------------------------------------------- | ----------- |
| Suíte de testes unitários desenvolvida | Casos CT006 a CT012                                                                                | ✅ Concluído |
| Código dos testes                      | Testes dos componentes `Residuo`, `ResiduoService`, `ResiduoViewModel` e `EstoqueReversoViewModel` | ✅ Concluído |
| Evidências da execução                 | 30 testes aprovados                                                                                | ✅ Concluído |
| Documentação da estratégia             | Seção 3                                                                                            | ✅ Concluído |
| Explicação dos casos de teste          | Seção 4                                                                                            | ✅ Concluído |
| Resultado final                        | 30 de 30 testes aprovados                                                                          | ✅ Concluído |
| Componentes escolhidos                 | 4 componentes do módulo                                                                            | ✅ Concluído |
| Melhorias identificadas                | Injeção de `HttpClient` e proteção para `Application.Current` nulo                                 | ✅ Concluído |


