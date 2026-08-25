# Relatório Técnico de Testes Unitários – Sistema ReGraphik

| Parâmetro | Detalhamento |
| :--- | :--- |
| **Instituição** | SENAI – CFP Escola de Programação e Robótica de Nova Lima |
| **Curso** | Superior de Tecnologia em Desenvolvimento de Sistemas |
| **Autor(a)** | Luna Beatriz Alves |
| **Equipe** | ReGraphik |
| **Orientador** | Prof. Frederico Martins Aguiar |
| **Data / Ano** | 2026 |
| **Tecnologia** | C# / .NET 8.0 |
| **Framework de Teste** | xUnit |
| **Status da Suíte** | 100% Aprovado (28 / 28) |

---

## 1. Visão Geral

Este documento formaliza a estratégia, a execução, os resultados e as evidências da suíte de testes unitários desenvolvida para o sistema **ReGraphik**. A cobertura abrange a validação de regras de negócio essenciais, serviços de comunicação em tempo real, gestão de estado de apresentação via ViewModels e integração simulada com serviços em nuvem (Firebase).

### 1.1 Escopo dos Componentes Testados
* **ChatService / ChatViewModel:** Rotinas de mensageria em tempo real (`EnviarMensagemAsync`, `ValidarEnvio`, `ReceberMensagem`, `ContarNaoLidasAsync`, `ConversaId`, `MarcarComoLidaAsync` e `ListarUsuariosAsync`).
* **DashboardViewModel:** Processamento e formatação de dados analíticos (`CalcularIniciais`, `ObterFotoPerfil`, `FormatterTipos`, `CarregarIndicadores`, `FiltrarUltimosResiduos` e `MapearCoresPizza`).
* **EsgViewModel:** Fluxos de geração de relatórios corporativos (`ExportarPdfCommand`, `IrParaRelatoriosCommand`, `SalvarArquivo` e `ExibirConfirmacao`).
* **PerfilViewModel (ContaViewModel):** Validação de cadastro e segurança de dados (`AplicarMascaras`, `AlternarMascaraEmail`, `ValidarEmail`, `SalvarPerfilCommand`, `AtualizarAsync`, `ValidarConfirmacaoSenha` e `AtualizarInformacoesProfissionais`).

---

## 2. Análise Metodológica

### 2.1 Justificativa da Seleção dos Componentes
A seleção dos módulos prioritários fundamentou-se no nível de criticidade operacional e nos riscos de negócio associados:
* **ChatService / ChatViewModel:** Módulo crítico para a comunicação operacional dos usuários. Falhas impactam diretamente a disponibilidade da plataforma.
* **DashboardViewModel:** Núcleo de inteligência analítica. Erros no processamento comprometem a exibição dos indicadores financeiros e das métricas de reciclagem.
* **EsgViewModel:** Componente responsável pela consolidação e emissão de relatórios oficiais em formato PDF via QuestPDF.
* **PerfilViewModel (ContaViewModel):** Responsável pelo controle de acesso, integridade de credenciais e conformidade com as diretrizes de privacidade de dados (LGPD).

### 2.2 Mapeamento de Comportamentos
Foram isolados e avaliados os seguintes comportamentos funcionais:
* **Gerenciamento de Estado e Mensageria:** Envio síncrono/assíncrono de mensagens, ordenação determinística de identificadores de sala (`ConversaId`), atualização de sinalizadores de leitura e contadores de pendências.
* **Transformação de Dados e Interface:** Algoritmos para extração de monogramas de identificação, validação de recursos de mídia (URLs e caminhos locais), ordenação de coleções via LINQ e estruturação de séries temporais para o OxyPlot.
* **Validação e Proteção de Dados:** Alternância dinâmica de máscaras de exibição em campos sensíveis (CPF e E-mail), verificação sintática de entradas de usuário e validação cruzada de senhas.

### 2.3 Mapeamento de Cenários de Teste
Os cenários mapeados originaram os casos de teste **CT017 a CT044**, cobrindo fluxos principais e exceções:
* **Comunicação:** Validação de payloads válidos/inválidos, resiliência contra falhas de conexão e tratamento de desserialização JSON heterogênea.
* **Painel Analítico:** Padronização de cargos, agregações financeiras e volumétricas, ordenação de registros recentes e mapeamento cromático de séries.
* **Relatórios:** Controle de fluxo em caixas de diálogo e geração física de documentos em disco.
* **Gestão de Conta:** Validação sintática de e-mails, campos obrigatórios, regras de alteração de senha e persistência de dados cadastrais.

### 2.4 Relevância e Impacto dos Testes
* **Integridade Operacional e Financeira:** O teste **CT029** garante a precisão nos cálculos de volumes e projeções financeiras, evitando divergências em relatórios.
* **Conformidade Legal (LGPD):** Os testes **CT036** e **CT038** asseguram a mitigação do risco de exposição indevida de dados sensíveis na camada de apresentação.
* **Tolerância a Falhas (Fault Tolerance):** Os testes **CT023** e **CT024** garantem a estabilidade da aplicação em cenários de indisponibilidade de serviços externos.
* **Qualidade da Experiência (UX):** A suíte garante que a interface responda de forma previsível e sem inconsistências visuais aos dados fornecidos.

### 2.5 Limitações do Escopo
* **Renderização Visível (UI):** Aspectos puramente visuais da interface XAML (layouts, margens e renderização gráfica) foram desconsiderados nesta suíte, cabendo a testes automatizados de UI (Appium/FlaUI).
* **Simulação de Rede Concorrente:** Testes de alta latência e cenários de estresse de infraestrutura foram delegados para a fase de testes de carga.
* **Isolamento de Serviços Internos:** A interação com serviços externos (Firebase) foi simulada através de objetos *Mock/Stub*, garantindo a execução determinística dos testes em ambientes de Integração Contínua (CI).

---

## 3. Arquitetura da Suíte de Testes

### 3.1 Padrão AAA (Arrange, Act, Assert)
A estruturação do código dos testes segue rigorosamente a convenção AAA:
1. **Arrange (Preparação):** Configuração de estado inicial, mocks de dependências e instanciação dos componentes sob teste.
2. **Act (Ação):** Execução do método ou comando específico sob avaliação.
3. **Assert (Verificação):** Avaliação dos resultados obtidos em relação aos critérios de aceitação estabelecidos.

### 3.2 Desacoplamento e Injeção de Dependências
O isolamento dos componentes foi garantido pelo uso de abstrações para interfaces como `IDialogService` e `IAutorizarService`. A utilização de mocks eliminou efeitos colaterais e dependências de serviços externos durante a execução da suíte.

---

## 4. Detalhamento dos Casos de Teste

### 4.1 Módulo de Mensageria (`ChatService` / `ChatViewModel`)
* **CT017 & CT018:** Validação do envio de mensagens válidas e bloqueio de entradas vazias com lançamento de `ArgumentException`.
* **CT019 & CT020:** Verificação do incremento de notificações em segundo plano e geração determinística das chaves de conversação.
* **CT021 a CT024:** Garantia de persistência de status de leitura, tolerância a variações de schema JSON e tratamento de exceções de rede.

### 4.2 Módulo Analítico (`DashboardViewModel`)
* **CT025 a CT028:** Padronização de nomenclaturas de cargos, cálculo de monogramas de usuário, validação de arquivos de imagem e formatação numérica de eixos.
* **CT029 a CT032:** Agregação de métricas via LINQ, seleção de registros mais recentes e estruturação de dados para representação gráfica.

### 4.3 Módulo de Relatórios ESG (`EsgViewModel`)
* **CT033 a CT035:** Validação do ciclo de vida da geração de relatórios, incluindo interceptação de cancelamentos pelo usuário e compilação do arquivo PDF.

### 4.4 Módulo de Gestão de Conta (`ContaViewModel`)
* **CT036 a CT038:** Mascaramento e desmascaramento dinâmico de dados pessoais de acordo com o estado do componente.
* **CT039 a CT044:** Validação de formato de e-mail, verificação de obrigatoriedade de campos, alteração segura de senhas e sincronização com a camada de serviço.

---

## 5. Matriz de Rastreabilidade e Resultados

### 5.1 Comunicação Corporativa (Chat em Tempo Real)

| ID | Funcionalidade | Cenário / Condição | Entrada | Resultado Esperado | Resultado Obtido | Evidência | Status |
| :--- | :--- | :--- | :--- | :--- | :--- | :--- | :---: |
| **CT017** | **Chat Tempo Real** | Envio de mensagem de texto simples entre dois usuários | Texto: `"Resíduo #104 liberado para coleta."` | Mensagem enviada e renderizada instantaneamente via Firebase Realtime na tela do destinatário. | Objeto `Mensagem` instanciado com remetente, destinatário, texto do resíduo e status `Lida = false` validados com sucesso. | EV-CT017 | Aprovado |
| **CT018** | **Chat Tempo Real** | Tentativa de envio de mensagem vazia | Texto: `""` (vazio/espaços) | O botão de envio permanece inativo ou ignora a ação. | O método `EnviarMensagemAsync` disparou exceção `ArgumentException` ao receber textos vazios ou espaços em branco. | EV-CT018 | Aprovado |
| **CT019** | **Notificação de Chat** | Recebimento de mensagem com a janela do chat minimizada | Envio de mensagem externa | Indicador visual de alerta de "Nova Mensagem" exibido na barra superior da aplicação. | A propriedade `MensagensNaoLidas` foi incrementada de 0 para 1 ao receber nova mensagem com a tela desautorizada/minimizada. | EV-CT019 | Aprovado |
| **CT020** | **Identificador de Sala** | Geração determinística de ID da conversa entre dois usuários | IDs: `"userA"` e `"userB"` | O ID da conversa deve ser sempre idêntico independentemente da ordem em que os usuários iniciam o chat. | A chamada estática ordenou alfabeticamente os IDs gerando `"userA_userB"` tanto para (A, B) quanto para (B, A). | EV-CT020 | Aprovado |
| **CT021** | **Marcação de Leitura** | Atualização do status das mensagens recebidas para lido | Lista de mensagens pendentes | Apenas as mensagens enviadas pelo remetente e com status `Lida = false` devem ser filtradas para atualização. | O filtro LINQ isolou com precisão a mensagem pendente ignorando mensagens já lidas e mensagens enviadas pelo destinatário. | EV-CT021 | Aprovado |
| **CT022** | **Leitura de Usuários** | Desserialização de JSON flexível para lista de usuários | JSON com chaves `name` / `foto_perfil` | O modelo `Usuario` deve ser preenchido corretamente mesmo com variações de nomenclatura no nó do Firebase. | Fallback de propriedades mapeou com êxito `name` para `Nome` e `foto_perfil` para `FotoPerfil`, além de capturar a chave do Firebase. | EV-CT022 | Aprovado |
| **CT023** | **Resiliência de Conexão** | Ocorrência de falha ou perda de conexão no Firebase | Consulta a nó indisponível | O serviço deve tratar a exceção via `try/catch` retornando lista vazia ou valor padrão `0` sem estourar exceção na UI. | Todos os métodos assíncronos de busca (`ObterMensagensAsync`, `ContarNaoLidasAsync`, `ListarUsuariosAsync`) trataram o erro retornando coleções vazias. | EV-CT023 | Aprovado |
| **CT024** | **Resiliência de Conexão** | Contagem de mensagens não lidas com erro de rede ou Firebase | Parâmetros: `"destinatario"`, `"remetente"` | O serviço deve tratar a exceção e retornar o valor padrão `0` sem interromper a execução do app. | O método `ContarNaoLidasAsync` capturou a falha no bloco `catch` e retornou `0` como valor de fallback seguro. | EV-CT024 | Aprovado |

---

### 5.2 Indicadores, Dashboard e Relatórios ESG

| ID | Funcionalidade | Cenário / Condição | Entrada | Resultado Esperado | Resultado Obtido | Evidência | Status |
| :--- | :--- | :--- | :--- | :--- | :--- | :--- | :---: |
| **CT025** | **Dashboard / Perfil** | Formatação de perfil e geração de iniciais do usuário | Nome: `"Carlos Eduardo"`, Perfil: `"Admin"` | Exibir nome completo, iniciais `"CE"` e o perfil expandido como `"Administrador"`. | A View Model atribuiu corretamente o nome, calculou as iniciais `"CE"` e converteu `"Admin"` para `"Administrador"`. | EV-CT025 | Aprovado |
| **CT026** | **Dashboard / Usuário** | Cálculo do fallback de iniciais para variações de nome | Nomes variados (`"João Silva"`, `"Maria"`, `""`, `null`) | Gerar iniciais com base nas palavras do nome ou retornar `"?"` em caso de valor inválido. | O cálculo retornou `"JS"`, `"M"` e fallback `"?"` em todos os casos de teste estipulados. | EV-CT026 | Aprovado |
| **CT027** | **Dashboard / Avatar** | Validação do caminho do arquivo local da foto de perfil | Caminho: `@"C:\caminho_inexistente_foto.png"` | Retornar `null` caso a foto de perfil não seja uma URL e o arquivo local não exista no disco. | A propriedade `FotoPerfil` validou a inexistência do arquivo no disco via `File.Exists` e retornou `null` com segurança. | EV-CT027 | Aprovado |
| **CT028** | **Dashboard / Gráficos** | Formatação de valores do eixo Y no gráfico de barras | Entrada numérica: `150.5` | Formatar o peso exibindo duas casas decimais seguidas do sufixo `"kg"`. | A função `FormatterTipos` formatou o valor numérico para o padrão textual `"150,50 kg"`. | EV-CT028 | Aprovado |
| **CT029** | **Dashboard / Indicadores** | Agregação de contadores e valor total estimado de resíduos | Lista de resíduos com quantidades e status variados | Calcular o total de resíduos, contagens por status (`"Reaproveitado"`, `"Em Estoque"`) e multiplicar a quantidade por R$ 5,50. | Os contadores totais foram consolidados e a expressão LINQ `Sum` calculou com precisão o valor monetário acumulado. | EV-CT029 | Aprovado |
| **CT030** | **Dashboard / Listagem** | Filtragem dos 5 últimos resíduos com reindexação de ID | Coleção com 10 resíduos cadastrados em datas distintas | Selecionar os 5 mais recentes por data e reindexar seus IDs visualmente de `1` a `5`. | A ordenação decrescente por `DataCadastro` isolou os 5 registros mais recentes e aplicou IDs sequenciais de `1` a `5`. | EV-CT030 | Aprovado |
| **CT031** | **Dashboard / Gráfico Pizza** | Mapeamento dinâmico de cores por status do resíduo | Status do resíduo: `"Disponível"`, `"Reservado"`, etc. | Atribuir a cor RGB correspondente ao status da fatia do gráfico de pizza. | A instrução `switch` mapeou com exatidão as cores de cada status, aplicando a paleta padrão para valores não cadastrados. | EV-CT031 | Aprovado |
| **CT032** | **Dashboard / Gráfico Barras** | Agrupamento de peso por tipo de resíduo | Resíduos duplicados por tipo (ex: 2x `"Plástico"`) | Somar a quantidade total agrupando por tipo e ordenar o resultado de forma crescente. | A consulta LINQ agrupou o tipo `"Plástico"`, somando suas massas e ordenando os tipos em ordem crescente no gráfico. | EV-CT032 | Aprovado |
| **CT033** | **Proposta ESG / Inicialização** | Instanciação da ViewModel com injeção de dependências | Instância de `Usuario`, mocks de `ICommand` e `IDialogService` | As propriedades de comando `ExportarPdfCommand` e `IrParaRelatoriosCommand` não devem ser nulas. | Os comandos foram instanciados corretamente e mantidos disponíveis para a View. | EV-CT033 | Aprovado |
| **CT034** | **Proposta ESG / PDF Cancelado** | Cancelamento do salvamento de arquivo na caixa de diálogo | Retorno `null` no método `SalvarArquivo` do `IDialogService` | O fluxo deve ser interrompido sem acionar a mensagem de confirmação ou gerar o arquivo. | O serviço confirmou o retorno nulo e `ExibirConfirmacao` nunca foi executado (`Times.Never`). | EV-CT034 | Aprovado |
| **CT035** | **Proposta ESG / Geração PDF** | Exportação bem-sucedida do documento PDF em caminho válido | Caminho temporário gerado por `Path.GetTempPath()` | Gerar o arquivo PDF físico no disco e acionar a caixa de diálogo de confirmação ao usuário. | O arquivo foi criado no diretório temporário (`File.Exists` = `true`) e a confirmação foi exibida uma vez (`Times.Once`). | EV-CT035 | Aprovado |

---

### 5.3 Perfil do Usuário

| ID | Funcionalidade | Cenário / Condição | Entrada | Resultado Esperado | Resultado Obtido | Evidência | Status |
| :--- | :--- | :--- | :--- | :--- | :--- | :---: | :---: |
| **CT036** | **Conta / Perfil** | Inicialização do perfil e aplicação de máscaras | Dados cadastrais de `Ana Souza` (CPF, E-mail, Cargo, Dpto, Tel) | Mapear dados para a ViewModel, mascarar CPF/E-mail, calcular iniciais e validar estado sem foto. | A ViewModel carregou todos os dados, exibiu `123.***.***-**`, `an*******@empresa.com`, iniciais `"AS"` e `SemFoto` = `true`. | EV-CT036 | Aprovado |
| **CT037** | **Conta / Avatar** | Cálculo de iniciais para diferentes formatos de nome | Variações de entrada (`"Carlos Eduardo Silva"`, `"Beatriz"`, `""`, `null`) | Gerar as iniciais baseadas no primeiro e último nome ou retornar `"?"` em caso de valor inválido. | A propriedade `Iniciais` calculou corretamente `"CS"`, `"B"` e o fallback `"?"` para entradas vazias. | EV-CT037 | Aprovado |
| **CT038** | **Conta / E-mail** | Alternância de máscara no campo de e-mail ao focar/desfocar | Ganho de foco (`GotFocus`) seguido de perda de foco (`LostFocus`) | Exibir o e-mail em texto puro durante a edição e reaplicar a máscara ao perder o foco. | O comando exibiu `"desenvolvedor@teste.com"` no foco e remascarou para `"de***********@teste.com"` ao perder o foco. | EV-CT038 | Aprovado |
| **CT039** | **Conta / Validação** | Validação de formato de e-mail ao perder o foco | E-mail inválido sem caractere `@` (`"emailsemarrobainvalido.com"`) | Definir a mensagem de erro apropriada na propriedade `MensagemErroEmail`. | A propriedade `MensagemErroEmail` foi preenchida com `"E-mail inválido. Verifique o endereço informado."`. | EV-CT039 | Aprovado |
| **CT040** | **Conta / Validação** | Salvar perfil com campos obrigatórios em branco | Nome ou Login vazios/nulos | Exibir mensagem de erro geral e impedir a chamada ao serviço de atualização. | A mensagem `"Nome e Login são obrigatórios."` foi exibida e `AtualizarAsync` não foi executado (`Times.Never`). | EV-CT040 | Aprovado |
| **CT041** | **Conta / Atualização** | Salvar alterações do perfil com dados válidos | Novo Nome (`"Novo Nome"`) e Novo Login (`"novo.login"`) | Atualizar os dados do objeto do usuário e invocar o método de atualização do serviço. | O modelo `Usuario` foi atualizado e `AtualizarAsync` foi chamado exatamente uma vez (`Times.Once`). | EV-CT041 | Aprovado |
| **CT042** | **Conta / Segurança** | Validação de confirmação de senha com valores divergentes | Array de parâmetros contendo senhas distintas (`"senha123"`, `"senhaDiferente"`) | Exibir erro de incompatibilidade de senhas e cancelar a persistência. | A mensagem `"As senhas digitadas não coincidem."` foi atribuída e o serviço de atualização não foi acionado. | EV-CT042 | Aprovado |
| **CT043** | **Conta / Segurança** | Alteração de senha quando as entradas são idênticas | Array de parâmetros com senhas iguais (`"NovaSenha123!"`, `"NovaSenha123!"`) | Atualizar a propriedade `Senha` do usuário e persistir as alterações com sucesso. | A propriedade `usuario.Senha` foi atualizada e o serviço `AtualizarAsync` foi invocado com sucesso. | EV-CT043 | Aprovado |
| **CT044** | **Conta / Profissional** | Atualização dos campos de informações profissionais | Alteração de Cargo, Departamento e Telefone via ViewModel | Refletir as alterações profissionais diretamente na entidade do usuário e persistir via API. | As propriedades `Cargo`, `Departamento` e `Telefone` do modelo foram atualizadas e salvas via `AtualizarAsync`. | EV-CT044 | Aprovado |

---

## 6. Lista de Verificação de Evidências

### 6.1 Módulo: Chat em Tempo Real (`ChatService` / `ChatViewModel`)
- [x] **EV-CT017:** Captura de tela/log confirmando a gravação do payload no nó `mensagens/{convId}/{id}` do Firebase em tempo de execução.
- [x] **EV-CT018:** Log de exceção confirmando o disparo de `ArgumentException` ao submeter mensagem com string vazia ou espaço em branco.
- [x] **EV-CT019:** Registro de alteração de estado do contador visual de mensagens não lidas com o chat em segundo plano.
- [x] **EV-CT020:** Log de validação da ordenação ordinal determinística dos IDs de usuário para a propriedade `ConversaId`.
- [x] **EV-CT021:** Registro de atualização do atributo `lida = true` exclusivo para mensagens enviadas pelo remetente oposto.
- [x] **EV-CT022:** Log de desserialização confirmando o *fallback* de propriedades heterogêneas no JSON (`name`/`Nome`, `foto_perfil`/`FotoPerfil`).
- [x] **EV-CT023:** Relatório de captura de exceção em `ObterMensagensAsync` retornando coleção vazia sem indisponibilidade da UI.
- [x] **EV-CT024:** Relatório de tratamento de falha de rede em `ContarNaoLidasAsync` retornando o valor padrão seguro (`0`).

### 6.2 Módulo: Dashboard Analítico (`DashboardViewModel`)
- [x] **EV-CT025:** Log de execução do construtor validando a normalização de strings de cargos (ex.: mapeamento de `"Admin"` para `"Administrador"`).
- [x] **EV-CT026:** Validação do algoritmo de extração de monogramas (iniciais) e *fallback* com símbolo `"?"` para entradas nulas.
- [x] **EV-CT027:** Registro de checagem do método `File.Exists` retornando `null` para caminhos de imagem inválidos ou inexistentes.
- [x] **EV-CT028:** Captura da formatação da string do eixo visual contendo duas casas decimais e o sufixo `"kg"` (`"N2" + "kg"`).
- [x] **EV-CT029:** Mapeamento do resultado da consulta LINQ confirmando o somatório de massas e cálculo do valor financeiro estimado ($Quantity \times R\$ 5,50$).
- [x] **EV-CT030:** Log de filtragem via `Take(5)` validando a reindexação sequencial dos 5 registros mais recentes (1 a 5).
- [x] **EV-CT031:** Mapeamento de atribuição de códigos Hex/RGB de cores OxyPlot por categoria de resíduo, incluindo o tom padrão `#CBD5E1`.
- [x] **EV-CT032:** Estrutura de dados das séries de barras do OxyPlot confirmando o agrupamento correto por `TipoResiduo`.

### 6.3 Módulo: Relatórios ESG (`EsgViewModel`)
- [x] **EV-CT033:** Instanciação da *ViewModel* confirmando a injeção do mock `IDialogService` e licenciamento da biblioteca QuestPDF.
- [x] **EV-CT034:** Log do fluxo de controle confirmando a interrupção graciosa do comando ao simular o cancelamento pelo usuário no diálogo.
- [x] **EV-CT035:** Confirmação da compilação e escrita do arquivo PDF gerado no diretório temporário do sistema.

### 6.4 Módulo: Perfil e Conta (`ContaViewModel`)
- [x] **EV-CT036:** Captura de tela/log da carga inicial do perfil confirmando o mascaramento de dados sensíveis (CPF `123.***.***-**` e E-mail).
- [x] **EV-CT037:** Matriz de saídas do cálculo de monogramas para diferentes estruturas de nomes próprios.
- [x] **EV-CT038:** Registro de transição de estado da propriedade de e-mail alternando entre texto legível (`GotFocus`) e mascarado (`LostFocus`).
- [x] **EV-CT039:** Registro da validação sintática no evento `LostFocus` exibindo a mensagem `"E-mail inválido. Verifique o endereço informado."`.
- [x] **EV-CT040:** Log de bloqueio de comando de salvamento ao identificar propriedades `Nome` ou `Login` nulas/vazias.
- [x] **EV-CT041:** Registro de chamada bem-sucedida ao serviço `IAutorizarService` contendo o modelo atualizado.
- [x] **EV-CT042:** Disparo de mensagem de erro `"As senhas digitadas não coincidem."` em caso de divergência nos campos de senha.
- [x] **EV-CT043:** Confirmação de alteração de senha e posterior redefinição dos campos de entrada para string vazia.
- [x] **EV-CT044:** Log de propagação e persistência dos campos `Cargo`, `Departamento` e `Telefone` na camada de serviço.

---

## 7. Conclusão

A execução da suíte composta por **28 casos de teste unitário** (CT017 a CT044) obteve **100% de taxa de aprovação**. Os resultados e evidências associadas confirmam que as regras de negócio, transformações de dados e tratamentos de exceções foram implementados de acordo com os requisitos especificados, proporcionando estabilidade para a aplicação **ReGraphik** e reduzindo o risco de regressões em manutenções futuras.
