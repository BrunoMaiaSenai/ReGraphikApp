# Plano de Teste - ReGraphik

---

## 1. Identificação do Projeto

| Item | Detalhes |
| :--- | :--- |
| **Nome do Sistema** | **ReGraphik** — Plataforma de Gestão de Estoque Reverso |
| **Versão** | `1.0.0`|
| **Equipe Responsável** | Bruno Maia, Otávio Henrique, Lucas Aquino, Luna Beatriz, Kaio Alves |
| **Unidade SENAI** | SENAI Afonso Greco – Nova Lima |
| **Instrutor Orientador**| Frederico Martins Aguiar |
| **Data do Planejamento**| 10/08/2026 |
| **Ambientes** | **API REST:** `webregraphik.runasp.net` / **Desktop:** WPF .NET 8 / **BD:** Firebase Realtime DB |

---

## 2. Objetivos dos Testes

O objetivo deste Plano de Testes é validar a estabilidade, integridade, usabilidade e conformidade técnica do ecossistema **ReGraphik** (Cliente Desktop WPF + API REST ASP.NET Core) antes da implantação final nas indústrias do setor gráfico.

* **O que a equipe pretende verificar?**
  * Sincronização e persistência em tempo real dos dados de resíduos e conversas no **Firebase Realtime Database**.
  * Consumo seguro e síncrono da **API REST** (`webregraphik.runasp.net`) pelo cliente **Desktop WPF**.
  * Precisão dos algoritmos de recomendação da **Economia Circular** e dos cálculos do **Módulo ESG**.
  * Comportamento e fluidez de integrações externas (**Google Maps Places API**, **Imgur API** para fotos de perfil).
  * Geração e formatação correta dos relatórios exportados em **PDF via QuestPDF**.
  * Instalação e execução através dos instaladores gerados (**WiX Toolset `.msi`** e **Inno Setup `.exe`**).

* **Quais riscos pretende reduzir?**
  * Perda ou corrupção de registros de estoque reverso no banco de dados em nuvem.
  * Incompatibilidade/Falha de carregamento no componente `WebView2` (Mapa Leaflet.js).
  * Exceções não tratadas (*crashes*) na interface desktop ao lidar com indisponibilidade de conexão/API.
  * Lentidão no carregamento dos indicadores da **Dashboard** (gráficos OxyPlot) ou estouro de cota do Firebase.

* **O que será considerado evidência de qualidade?**
  * Aprovação de 100% dos Casos de Teste críticos e de alta prioridade.
  * Ausência de bugs de severidade Alta ou Crítica abertos na release.
  * Logs de execução sem exceções não capturadas (`Unhandled Exceptions`).
  * Relatórios de auditoria e certidões ESG gerados com dados 100% consistentes.

---

## 3. Escopo

### 3.1 O que será testado (In-Scope)
* **Módulos Funcionais do Cliente WPF:**
  * **Autenticação em 2 Etapas:** Login, Recuperação de Senha e validação de Token de Convite.
  * **Dashboard:** Carregamento de métricas financeiras, histórico e gráficos com OxyPlot.
  * **Cadastrar Resíduos:** Validação de entradas (dimensões, tipo, quantidade, origem) e upload de imagem.
  * **Estoque Reverso:** Filtragem com `ICollectionView` por tipo, status e período.
  * **Mapa / Pontos de Coleta:** Integração Google Maps Places API e exibição no Leaflet via WebView2.
  * **Sugestão de Resíduos:** Algoritmo de cruzamento e associação de reaproveitamento.
  * **Chat em Tempo Real:** Envio/recebimento de mensagens direto via Firebase Realtime DB.
  * **Relatórios e ESG:** Geração de estatísticas e exportação em PDF via QuestPDF.
  * **Minha Conta / Perfil:** Alteração de dados e upload de foto via Imgur API.
* **API REST (`ApiRestReGraphik`):**
  * Endpoints de CRUD, validação de requisições JSON e respostas HTTP adequadas (`200`, `201`, `400`, `401`, `404`, `500`).
* **Instalação e Empacotamento:**
  * Instalação limpa, criação de atalhos e desinstalação via pacote `.msi` / `.exe`.

### 3.2 O que NÃO será testado (Out-of-Scope)
* Testes de carga extrema / estresse superiores a 1.000 requisições simultâneas por segundo na API.
* Compatibilidade com sistemas operacionais antigos (Windows 7/8) ou plataformas não-Windows (macOS/Linux).
* Validação interna da infraestrutura dos servidores do Firebase ou Google.

### 3.3 Funcionalidades Prioritárias
1. Autenticação e Controle de Acesso (Token e Perfis).
2. Cadastro, Alteração e Atualização de Status do Resíduo.
3. Comunicação síncrona Cliente WPF $\leftrightarrow$ API REST $\leftrightarrow$ Firebase.
4. Geração do Relatório ESG em PDF.
5. Funcionamento do Mapa de Pontos de Coleta.

---

## 4. Base de Teste

Artefatos e especificações técnicas utilizados para desenhar e executar os cenários de teste:
* **Especificação da API REST / Swagger (OpenAPI):** Disponível em `https://webregraphik.runasp.net.`.
* **Documentação Mintlify do Projeto:** Publicada em `https://brunomaia.mintlify.app/`.
* **Modelagem de Dados:** Diagramas Conceitual, Lógico e Físico do sistema (Firebase / API / WPF).
* **Diagramas da UML:** Casos de Uso, Diagramas de Fluxo, Diagramas de Sequência e Mapa de Bounded Contexts.
* **Regras de Negócio do Setor Gráfico:** Tolerâncias de dimensões (cm/m), status do resíduo (`Disponível`, `Reservado`, `Descartado`) e equivalências de descarte ecológico.

---

## 5. Abordagem de Testes

* **Testes Funcionais:** Validação ponta a ponta se as regras do negócio gráfico estão sendo atendidas.
* **Testes de Integração:** 
  * Integração WPF $\leftrightarrow$ API REST (via HTTP/JSON).
  * Integração WPF $\leftrightarrow$ Firebase Realtime Database (Chat em tempo real).
  * Integração API REST $\leftrightarrow$ Google Maps Places API.
  * Integração WPF $\leftrightarrow$ Imgur API (Upload de imagens).
* **Testes de Sistema:** Verificação de fluxos completos (ex: Cadastrar resíduo $\rightarrow$ Consultar no Estoque Reverso $\rightarrow$ Aplicar Sugestão $\rightarrow$ Alterar Status $\rightarrow$ Gerar Relatório ESG).
* **Testes Não-Funcionais:** 
  * Desempenho e resposta da interface gráfica em WPF durante o carregamento de dados.
  * Usabilidade em diferentes resoluções de tela desktop (HD e Full HD).
* **Testes Exploratórios:** Sessões livres focadas em cenários de uso não previstos ou ações inesperadas do usuário.
* **Reteste e Regressão:** Reexecução de cenários após correções de código para garantir estabilidade contínua.

---

## 6. Técnicas de Projeto de Testes

| Técnica | Aplicação no ReGraphik |
| :--- | :--- |
| **Particionamento de Equivalência** | Validação de formatos em campos como E-mail, CPF, CEP e URLs de imagem. |
| **Análise de Valor Limite (AVL)** | Aplicada em dimensões em cm/m ($0$, valores negativos, casas decimais), quantidades de resíduo ($0$, $1$, $999999$) e limites de caracteres de texto no Chat. |
| **Tabela de Decisão** | Regras de alteração de Status do Resíduo (`Disponível` $\rightarrow$ `Reservado` $\rightarrow$ `Descartado`) baseadas no perfil do usuário (Administrador vs. Operador). |
| **Testes Baseados em Cenários** | Simulação do fluxo de trabalho diário de um operador de gráfica registrando aparas de papel e buscando reciclagem. |
| **Testes de Aceitação:** | Aplicável nas entregas para o instrutor orientador e stakeholders do SENAI, validando se o sistema atende aos critérios definidos nos Casos de Uso e histórias de usuário do TCC (ex: operador consegue cadastrar, buscar e reciclar um resíduo do início ao fim). |
| **Testes Baseados em Cenários:** | Simulação de fluxos reais de uso do dia a dia de uma gráfica (ex: operador cadastra apara de papel → sistema sugere reaproveitamento → chat combina retirada → status muda para Reservado → relatório ESG reflete a movimentação).|
| **Testes Exploratórios** | Sessões livres guiadas por checklist mínimo, focadas em interações inesperadas na tela de Cadastro de Resíduos e no comportamento do WebView2 sob perda de conexão. |

---

## 7. Casos de Teste

### 7.1. Autenticação, Cadastro de Usuários e Segurança

| ID | Funcionalidade | Cenário / Condição | Entrada | Resultado Esperado | Resultado Obtido | Evidência do Teste | Status |
| :--- | :--- | :--- | :--- | :--- | :--- | :--- | :---|
| **CT001** | Login | Autenticação com credenciais válidas | Login: `LunnaB@gmail.com`, Senha: `Luna123` | Acesso liberado, Token de sessão salvo no Firebase Auth e redirecionamento para a Dashboard. | Login foi realizado para adentrar no sistema ReGraphik e tela principal de DashBoard carregada. |   | ⏳ Pendente |
| **CT002** | Login | Tentativa de login com senha incorreta | Login: `operador@gmail.com`, Senha: `errada` | Mensagem de erro *"Usuário ou senha inválidos"* e acesso bloqueado. | — |  |⏳ Pendente |
| **CT003** | Cadastro / Convite | Registro de novo usuário com Token de Convite Válido | E-mail: `novo@gmail.com`, Token: `849201` | Conta ativada com sucesso no Firebase e permissão concedida. | — |  |⏳ Pendente |
| **CT004** | Cadastro / Convite | Registro de novo usuário com Token Invalidador/Expirado | E-mail: `novo@gmail.com`, Token: `000000` | Mensagem de erro *"Token de convite inválido ou já utilizado"* e bloqueio do cadastro. | — | | ⏳ Pendente |
| **CT005** | Perfil do Usuário | Upload e alteração de foto de perfil via Imgur API | Selecionar imagem `avatar.png` (PNG < 2MB) | Upload concluído, URL gerada no Imgur v5 e atualizada na interface WPF. | — |  |⏳ Pendente |

## CT001 
1.  Funcionalidade
CT001  - Login

3.  Cenário / Condição
Autenticação com credenciais válidas.

4. Entrada
Entrada: Login: LunnaB

5. Resultado Esperado
Acesso liberado, Token JWT/Firebase gerado e redirecionamento para a Dashboard

6. Resultado Obtido
 
Login foi realizado para adentrar no sistema ReGraphik e tela principal de DashBoard carregada.
<img width="484" height="283" alt="01" src="https://github.com/user-attachments/assets/e6b30079-1a42-4994-b180-55bfbf934a3d" />
<img width="489" height="282" alt="02" src="https://github.com/user-attachments/assets/d8656c46-c9ee-43cf-b909-5e13fdbeaf95" />

6. Evidência do Teste

Foi avaliado e homologado conforme evidenciado no resultado obtido, o cliente realiza o login, adicionando login e senha, e é direcionado para a tela da Dashboard.
7.             Status
Concluído. 

## CT002

## CT003 

## CT004

## CT005
1. Funcionalidade
CT005 - Cadastro / Convite 
2.             Cenário / Condição
Registro de novo usuário com Token Invalidador/Expirado 
3.             Entrada
E-mail: novo@gmail.com, Token: 000000 
4.             Resultado Esperado
Mensagem de erro "Token de convite inválido ou já utilizado" e bloqueio do cadastro. 
5.             Resultado Obtido

Cadastro do usuário realizado e token válido enviado com sucesso!

<img width="479" height="277" alt="03" src="https://github.com/user-attachments/assets/422bcec1-8787-469a-9301-7319986f5b1a" />


	Ao inserir um token inválido no credenciamento do usuário, o sistema não retornou uma mensagem de “Token de convite inválido ou já utilizado" .
 

6.  Evidência do Teste

Foi identificado a causa raiz do problema, que era a falta de uma mensagem de erro do token, para identificar que o token está invalido.

<img width="491" height="230" alt="05" src="https://github.com/user-attachments/assets/ff2f524a-7fe1-4e25-bd40-2ba2a9f84d67" />

<img width="491" height="184" alt="04" src="https://github.com/user-attachments/assets/68ef529e-29d6-4c6c-851d-9b7c23779182" />
	
Após a correção, a rotina foi replicada novamente e a mensagem foi exibida corretamente.

<img width="491" height="241" alt="07" src="https://github.com/user-attachments/assets/1c050662-581b-465b-bf11-3cc1741b48a8" />

<img width="493" height="157" alt="06" src="https://github.com/user-attachments/assets/18b0669c-d87c-4bf1-8ced-3fc8a7974883" />


7.  Status
Concluído.

---

### 7.2. Gestão do Estoque Reverso e Resíduos

| ID | Funcionalidade | Cenário / Condição | Entrada | Resultado Esperado | Resultado Obtido | Evidência do Teste | Status |
| :--- | :--- | :--- | :--- | :--- | :--- | :--- | :--- |
| **CT006** | Cadastro de Resíduo | Inclusão de aparas de papel com dados válidos | Tipo: `Papel A4`, Qtd: `50`, Dimensões: `21x29.7cm` | Resíduo salvo no Firebase com status `Disponível` e atualização imediata do estoque. | — |  | ⏳ Pendente |
| **CT007** | Cadastro de Resíduo | Tentativa de cadastro com quantidade zerada ou negativa | Tipo: `Vinil`, Qtd: `-5` | Validação de formulário impedindo envio: *"A quantidade deve ser maior que zero"*. | — |  | ⏳ Pendente |
| **CT008** | Cadastro de Resíduo | Omissão de campos obrigatórios no cadastro | Tipo: `Em Branco`, Qtd: `10` | Indicação em vermelho nos campos nulos com mensagem *"Preencha os campos obrigatórios"*. | — |  | ⏳ Pendente |
| **CT009** | Consulta de Estoque | Filtragem dinâmica por tipo de resíduo | Filtro em memória: `Vinil` | A listagem atualiza via `ICollectionView` exibindo apenas os resíduos do tipo Vinil sem travamentos. | — |   | ⏳ Pendente |
| **CT010** | Alteração de Status | Transição do estado do resíduo de "Disponível" para "Reservado" | Clicar em *"Reservar Resíduo"* ID `#104` | Status no Firebase alterado para `Reservado`, mudando a cor do indicador no card do app. | — |  | ⏳ Pendente |
| **CT011** | Alteração de Status | Finalização do ciclo de vida para "Descartado/Reaproveitado" | Clicar em *"Dar Baixa / Concluir"* | Resíduo movido para a lista de concluídos e contabilizado na métrica da Dashboard ESG. | — |  | ⏳ Pendente |
| **CT012** | Exclusão de Resíduo | Remoção de item do estoque por perfil Usuário vs Administrador | Perfil: `Usuário` clicando em Excluir | Opção desabilitada ou com mensagem *"Ação restrita a Administradores"*. | — |  | ⏳ Pendente |

## CT006

## CT007 

## CT008

## CT009 

## CT010 

## CT011

## CT012
  
---

### 7.3. Módulo de Geolocalização e Pontos de Coleta

| ID | Funcionalidade | Cenário / Condição | Entrada | Resultado Esperado | Resultado Obtido | Status |
| :--- | :--- | :--- | :--- | :--- | :--- | :--- |
| **CT013** | Pontos de Coleta | Busca de pontos de reciclagem por cidade válida | Cidade: `Nova Lima` | Consulta enviada à Google Places API e marcadores (pins) desenhados no Leaflet/WebView2. | — | ⏳ Pendente |
| **CT014** | Pontos de Coleta | Pesquisa por cidade sem retorno ou inexistente | Cidade: `CityX99` | Retorno limpo e exibição da mensagem *"Nenhum ponto de coleta localizado nesta região"*. | — | ⏳ Pendente |
| **CT015** | Interatividade do Mapa | Clique no Pin do Ponto de Coleta | Clicar no marcador no mapa | Janela (popup) é aberta contendo endereço, telefone de contato e tipos de materiais aceitos. | — | Concluído |
| **CT016** | Fallback do Mapa | Execução sem conexão com a internet ou API fora do ar | Desconectar internet / Buscar mapa | Tratamento de exceção exibindo *"Serviço de mapa indisponível. Verifique sua conexão"*. | — | ⏳ Pendente |

## CT013 

## CT014

## CT015

## CT016

---

### 7.4. Comunicação Corporativa (Chat em Tempo Real)

| ID | Funcionalidade | Cenário / Condição | Entrada | Resultado Esperado | Resultado Obtido | Status |
| :--- | :--- | :--- | :--- | :--- | :--- | :--- |
| **CT017** | Chat Tempo Real | Envio de mensagem de texto simples entre dois usuários | Texto: *"Resíduo #104 liberado para coleta."* | Mensagem enviada e renderizada instantaneamente via Firebase Realtime na tela do destinatário. | — | ⏳ Pendente |
| **CT018** | Chat Tempo Real | Tentativa de envio de mensagem vazia | Texto: `""` (vazio/espaços) | O botão de envio permanece inativo ou ignora a ação. | — | ⏳ Pendente |
| **CT019** | Notificação de Chat | Recebimento de mensagem com a janela do chat minitimizada | Envio de mensagem externa | Indicador visual de alerta de "Nova Mensagem" exibido na barra superior da aplicação. | — | ⏳ Pendente |

## CT017 

## CT018

## CT019

---

### 7.5. Indicadores, Dashboard e Relatórios ESG

| ID | Funcionalidade | Cenário / Condição | Entrada | Resultado Esperado | Resultado Obtido | Status |
| :--- | :--- | :--- | :--- | :--- | :--- | :--- |
| **CT020** | Dashboard | Atualização automática dos cards de métricas (Kg Reaproveitados) | Inserir baixa de 100Kg de Papel | Total acumulado de kg reaproveitados atualiza automaticamente sem necessidade de reiniciar o app. | — | ⏳ Pendente |
| **CT021** | Relatórios ESG | Exportação do Relatório Consolidado em PDF | Clicar no botão *"Exportar PDF"* | O QuestPDF compila a estrutura, gera o arquivo `.pdf` com formatação e salva no diretório escolhido. | — | ⏳ Pendente |
| **CT022** | Relatórios ESG | Exportação de relatório em período sem dados registrados | Filtrar Período: `Data sem registros` | Arquivo PDF gerado exibindo o cabeçalho oficial e a mensagem *"Nenhum resíduo movimentado neste período"*. | — | ⏳ Pendente |

## CT020

## CT021 

## CT022

---

### 7.6. Instalação, Manutenção e Atualização (Inno Setup)

| ID | Funcionalidade | Cenário / Condição | Entrada | Resultado Esperado | Resultado Obtido | Status |
| :--- | :--- | :--- | :--- | :--- | :--- | :--- |
| **CT023** | Instalador | Instalação limpa em um computador sem o ReGraphik | Executar `ReGraphik_Setup.exe` | Arquivos copiados para `Program Files`, registro criado e atalhos adicionados à Área de Trabalho. | A instalação ocorreu tranquila e tudo foi instalado com sucesso | Concluído |
| **CT024** | Instalador (Manutenção) | Reexecutar instalador com a mesma versão já instalada no sistema | Selecionar nenhuma opção e clicar em *"Avançar"* | **Bloqueio ativado:** Exibe alerta exigindo a seleção de uma das opções de manutenção (Atualizar, Reparar ou Desinstalar). | O bloqueio é efetuado corretamente e o usuário não consegui progredir sem escolher uma opção | Concluído |
| **CT025** | Instalador (Seleção) | Teste de exclusividade das CheckBoxes de Manutenção | Marcar *"Atualizar"* e depois *"Desinstalar"* | Comportamento de radio-button: A marcação de "Atualizar" é removida automaticamente ao clicar na outra. | O executável não permite a marcação de duas ou mais CheckBoxes, caso uma seja marcada a outro e desmarcada automaticamente | Concluído |
| **CT026** | Instalador (Atualização) | Execução do setup em sistema que já possui versão antiga instalada | Selecionar *"Atualizar (Versão mais recente)"* | O instalador sobrepõe os binários mantendo as configurações do usuário e atualiza a versão no Registro. | Como o sistema não possui ainda uma versão atualizada ele apenas alerta o usuário com uma mensagem | Concluído |
| **CT027** | Instalador (Execução) | Tentar instalar/atualizar com a aplicação ReGraphik aberta | Clicar em *"Avançar"* com app aberto | Alerta exibido pedindo permissão; ao aceitar, fecha o processo `ReGraphik.exe` via `taskkill` e prossegue. | Uma mensagem avisando que o sistema está aberto aparece na tela para que o usuário possa fechar o aplicativo antes de prosseguir | Concluído |
| **CT028** | Desinstalação | Removendo a aplicação via Opção de Manutenção ou Painel do Windows | Selecionar *"Desinstalar"* | Processo `/SILENT` executado, removendo atalhos e pasta da aplicação da máquina. | O aplicativo e desinstalado com sucesso da máquina | Concluído |

## CT023

- Primeiro teste de instalação em um notebook que não possuia o Visaul Studio e sem o aplicativo da Regraphik baixado.

<img width="1455" height="740" alt="InnoSetup_Imagem14" src="https://github.com/user-attachments/assets/68598604-5c27-49aa-9b10-34a7a97e9099" />
  

- Foi instalado o executável na pasta do sistema.

<img width="965" height="76" alt="InnoSetup_Imagem15" src="https://github.com/user-attachments/assets/f9ce1147-43d9-4681-8141-eb311a15afe7" />
  

- A linguagem foi escolhida e os termos de uso foram lidos e aceitos.

<img width="532" height="278" alt="InnoSetup_Imagem16" src="https://github.com/user-attachments/assets/a135b25d-d2cf-4c76-8b7f-b1567a9862d0" />

<img width="842" height="652" alt="InnoSetup_Imagem17" src="https://github.com/user-attachments/assets/cd3c70fb-bb7b-4d5f-abc5-106ed9079500" />
  

- Assim como o esperado os arquivos foram copiados e enviados para a pasta `Program Files` e foi criado um atalho na tela do usuário.

<img width="851" height="660" alt="InnoSetup_Imagem18" src="https://github.com/user-attachments/assets/b7906657-51a5-4929-abfe-3936f32ad9a4" />

<img width="832" height="652" alt="InnpSetup_Imagem1" src="https://github.com/user-attachments/assets/a34b5666-5eea-4182-8f17-be85183077fa" />

<img width="827" height="647" alt="InnoSetup_Imagem3" src="https://github.com/user-attachments/assets/42a5bd28-4fed-43f3-8112-0ff052efd110" />

<img width="840" height="653" alt="InnoSetup_Imagem4" src="https://github.com/user-attachments/assets/1052c2b2-5b97-48de-bbbe-fe7640deeb9c" />

<img width="851" height="656" alt="InnoSetup_Imagem19" src="https://github.com/user-attachments/assets/932284f5-0311-46b4-a31a-18e55d533be9" />

<img width="1366" height="988" alt="InnoSetup_Imagem20" src="https://github.com/user-attachments/assets/fe09baef-d1f4-4703-bbb2-5c0862b9169c" />
  

## CT024

- Caso uma versão já exista na máquina, o executável fornecerá opção para o aplicativo já baixado

<img width="1391" height="978" alt="InnoSetup_Imagem21" src="https://github.com/user-attachments/assets/d754023a-a5f2-4ee9-aafc-da50a99e56e3" />


- Se o usuário tenta avançar sem escolher uma opção, uma mensagem de alerta aparece e ele não consegue prosseguir se uma opção não for escolhida.

<img width="822" height="647" alt="InnoSetup_Imagem5" src="https://github.com/user-attachments/assets/13b433e9-e9ad-49a5-96a6-867ac217a59d" />


## CT025

- O usuário marca a opção de atualização e logo tenta marcar a opção de desinstalar sem desmarcar a outra antes, o sistema não permite que mais de uma seja marcada então a outra e automaticamente desmarcada.

<img width="831" height="646" alt="InnoSetup_Imagem6" src="https://github.com/user-attachments/assets/aac11c3f-6a5d-4e0b-8709-35434bf0c786" />

<img width="842" height="647" alt="InnoSetup_Imagem7" src="https://github.com/user-attachments/assets/65b66c53-1026-4434-ab33-4b3e90f937cf" />


## CT026 

- O usuário escolhe a opção de atualizar, mas como o sistema não possui uma versão superior, ele apenas alerta o usuário sobre isso.

<img width="840" height="651" alt="InnoSetup_Imagem8" src="https://github.com/user-attachments/assets/1c0c40a8-1c8c-4e6f-8e11-bb6d33f01c5f" />


## CT027 

- O usuário tenta usar a opção de restaurar o aplicativo quando ele ainda está aberto, e uma mensagem avisando que o sistema está aberto é enviada para que ele aceite fechar o aplicativo aberto antes de restaurar ele.

<img width="1600" height="844" alt="InnoSetup_Imagem10" src="https://github.com/user-attachments/assets/2d5e411d-a69f-46fc-b767-6e6213178107" />


## CT028

- O usuário escolher desinstalar o aplicativo e o instalador desinstala ele com sucesso.

<img width="842" height="647" alt="InnoSetup_Imagem7" src="https://github.com/user-attachments/assets/f085cf0a-8fde-4c67-a8a1-c632e9786aa7" />

<img width="1390" height="983" alt="InnoSetup_Imagem11" src="https://github.com/user-attachments/assets/84c36153-5104-4030-957e-0b7c40061b78" />


---

### 7.7. Resiliência e Conectividade

| ID | Funcionalidade | Cenário / Condição | Entrada | Resultado Esperado | Resultado Obtido | Status |
| :--- | :--- | :--- | :--- | :--- | :--- | :--- |
| **CT029** | Tratamento de Rede | Perda de conexão com a Internet durante a navegação no App | Desconectar cabo de rede/Wi-Fi | A interface exibe o status *"Modo Offline / Sem Conexão"* no rodapé e desabilita requisições pendentes sem travar a UI (evita Crash). | — | ⏳ Pendente |
| **CT030** | Usabilidade em diferentes resoluções | Verifica se a interface WPF não quebra layout em HD (1366x768) e Full HD (1920x1080) | Rodar o mesmo fluxo (ex: Cadastrar Resíduo) nas duas resoluções | Checar se botões, campos e o WebView2 do mapa continuam visíveis e clicáveis | — | ⏳ Pendente |

## CT029

## CT030
1.             Funcionalidade
Usabilidade em diferentes resoluções

2.             Cenário / Condição
Verifica se a interface WPF não quebra layout em HD (1366x768) e Full HD (1920x1080)

3.             Entrada
Rodar o mesmo fluxo (ex: Cadastrar Resíduo) nas duas resoluções

4.             Resultado Esperado
Checar se botões, campos e o WebView2 do mapa continuam visíveis e clicáveis

5.             Resultado Obtido
<img width="1232" height="613" alt="11" src="https://github.com/user-attachments/assets/98b62033-a75e-4d23-86d8-9da3bad6716a" />
<img width="1734" height="623" alt="10" src="https://github.com/user-attachments/assets/2ee35390-a05c-4853-b1a2-6b5785e1092c" />
<img width="1813" height="607" alt="12" src="https://github.com/user-attachments/assets/aec6a05c-0df9-4a3f-9f39-7232a45acaa9" />


6.             Evidência do Teste

Teste em conformidade, conforme evidenciado acima no item 5
7.             Status
Concluído. 


---

## 8. Análise de Riscos

| Risco Identificado | Impacto | Probabilidade | Ação de Mitigação / Prevenção |
| :--- | :---: | :---: | :--- |
| **Inatividade/Warm-up da API (Runasp.net no plano gratuito)** | Médio | Alta | Implementar indicador de carregamento (*Spinner/Loading*) no app WPF e *retry policy* nas chamadas HTTP. |
| **Perda de conexão com o Firebase durante o Chat** | Médio | Média | Tratamento de exceção com notificação na tela (*Toast*) e reconexão automática ao reestabelecer rede. |
| **Falha de renderização do WebView2 (Leaflet.js)** | Alto | Baixa | Adicionar verificação de presença do Runtime do WebView2 no instalador `.msi`/`.exe`. |
| **Estouro de cota de chamadas da Google Places API** | Médio | Baixa | Armazenar em cache no Firebase os resultados de pontos de coleta já pesquisados por cidade. |
| **Entradas de dimensões malformadas no cadastro** | Médio | Alta | Aplicar mascaramento de entrada nos inputs e validação estrita via regex e Análise de Valor Limite. |

---

## 9. Reteste e Regressão

### 9.1 Conceitos e Aplicação no ReGraphik
* **Erro:** Uma falha humana cometida pelo desenvolvedor (ex: esquecer de passar o token JWT no cabeçalho HTTP).
* **Defeito (Bug):** A imperfeição presente na base de código (ex: método de consulta retornando `401 Unauthorized` por falta de header).
* **Falha:** A manifestação visível ao usuário durante o uso (ex: a tela de Estoque Reverso fica em branco e exibe alerta de erro).

* **Processo de Reteste:** Após a correção do defeito pelo desenvolvedor, o testador reexecuta **exatamente o caso de teste que falhou** para confirmar que o problema foi sanado.
* **Processo de Regressão:** Suíte de cenários executados para assegurar que a correção de um módulo (ex: alteração na API) não causou efeitos colaterais indesejados em outros módulos funcionais (ex: Chat ou Relatórios).

### 9.2 Suíte Obrigatória de Testes de Regressão (Top 5 Cenários)
Sempre que houver um novo deploy na API ou nova versão do cliente WPF, as 5 funcionalidades abaixo deverão ser **obrigatoriamente retestadas**:

1. **`CT001` - Autenticação e Renovação de Sessão:** Garantir que o fluxo de login e validação do token permanecem operacionais.
2. **`CT003` - Fluxo de Entrada e Persistência no Estoque Reverso:** Confirmar que o cadastro de resíduos continua gravando corretamente no Firebase.
3. **`CT006` - Integração com Mapa e Geolocalização:** Assegurar que a busca de pontos de coleta via Google Places API / WebView2 continua ativa.
4. **`CT007` - Sincronização do Chat em Tempo Real:** Validar que as mensagens continuam trafegando com baixa latência.
5. **`CT008` - Compilação de Relatórios em PDF:** Garantir que o QuestPDF continua gerando o documento sem erros de layout ou dados zerados.

### 9.3 Critérios de Classificação de Severidade e Prioridade

Para padronizar a triagem de defeitos, a equipe adotará os seguintes critérios objetivos:

**Severidade (impacto técnico no sistema)**

| Nível | Critério no ReGraphik |
| :--- | :--- |
| **Crítica** | Sistema trava, dados são perdidos/corrompidos no Firebase, ou login/API ficam totalmente inacessíveis. |
| **Alta** | Funcionalidade principal (Cadastro, Estoque, ESG) não funciona, mas o sistema não trava. |
| **Média** | Funcionalidade secundária falha (ex: Chat com atraso, Mapa não carrega um pin) mas existe alternativa. |
| **Baixa** | Problemas visuais, de layout ou usabilidade que não impedem o uso (ex: ícone desalinhado). |

**Prioridade (urgência de correção para o negócio)**

| Nível | Critério no ReGraphik |
| :--- | :--- |
| **Alta** | Bloqueia o fluxo principal do operador ou compromete a apresentação/entrega do TCC. |
| **Média** | Afeta a experiência, mas há contorno possível até a próxima versão. |
| **Baixa** | Pode ser corrigido em ciclos futuros sem impacto imediato. |

> Importante: Severidade e Prioridade são avaliadas separadamente. Um defeito de baixa severidade técnica (ex: erro visual na tela de login) pode receber prioridade alta se ocorrer durante a apresentação da banca.

---

## 10. Critérios de Entrada e Saída

### 10.1 Critérios de Entrada (Para iniciar os testes)
* [x] API REST implantada e acessível em `https://webregraphik.runasp.net`.
* [x] Instância do Firebase Realtime Database configurada e online.
* [x] Build da aplicação Desktop WPF e/ou pacote instalador (`.msi` / `.exe`) gerados sem erros.
* [x] Chaves de API (Google Places, Imgur) válidas e configuradas nas variáveis do sistema.

### 10.2 Critérios de Saída (Para aprovar a entrega)
* [x] Execução de no mínimo **95%** dos casos de teste planejados neste documento.
* [x] **100% de aprovação nos Casos de Teste do Escopo Crítico** (Login, Cadastro de Resíduos, API e ESG).
* [x] **Zero defeitos com severidade Alta ou Crítica pendentes de correção**.
* [x] Registro e arquivamento de todas as evidências (prints, logs e PDFs) no repositório.

---

## 11. Evidências e Documentação

### 11.1 Armazenamento de Evidências
Todas as evidências geradas durante os testes deverão ser salvas na pasta do repositório:
`docs/evidencias-testes/`

Formatação esperada das evidências:
* **Capturas de Tela (Prints):** NomeDoTeste_Status.png (Ex: `CT003_CadastroResiduo_SUCESSO.png`).
* **Relatórios Impressos:** Exemplo de PDF exportado com dados de teste.
* **Logs da API/HTTP:** Arquivos `.txt` contendo os retornos do Swagger/Postman.

### 11.2 Padrão de Reporte de Defeito (Bug Report)
Ao encontrar qualquer falha, o testador deverá abrir uma **Issue no GitHub** utilizando o padrão abaixo:

```text
[BUG] - Título claro e objetivo do problema

- Descrição: Explicação sucinta sobre o comportamento incorreto observador.
- Módulo Afetado: [Ex: Cadastrar Resíduos / Chat / API REST / Mapa]
- Passos para Reproduzir:
  1. Abrir o ReGraphik Desktop.
  2. Fazer login com perfil Operador.
  3. Navegar até a aba 'Cadastrar Resíduos'.
  4. Preencher o campo 'Quantidade' com -10 e clicar em 'Salvar'.
- Resultado Esperado: O sistema deve exibir um alerta de validação impedindo o envio.
- Resultado Obtido: O sistema trava (Crash) e lança uma exceção 'FormatException'.
- Severidade: Alta (Trava a aplicação)
- Prioridade: Alta (Bloqueia o fluxo principal do operador)
- Ambiente: Windows 11 64-bits / ReGraphik v1.0.0 / .NET 8.0 Runtime
- Evidência: print_erro_crash.png (em anexo)

````

---

## 12. Recursos Necessários

| Tipo | Recurso |
| :--- | :--- |
| **Equipe** | 5 integrantes atuando em revezamento entre desenvolvimento e testes |
| **Ambiente** | 1 máquina Windows 10/11 64-bits para testes de instalação e execução do WPF |
| **Ferramentas** | Postman/Swagger (API), GitHub Issues (defeitos), WebView2 Runtime |
| **Dados de Teste** | Massa de dados fictícia de resíduos, usuários e conversas no Firebase (ambiente de homologação) |
| **Acessos** | Chaves de API válidas (Google Places, Imgur) configuradas em ambiente de teste |

---

## 13. Cronograma

| Atividade | Período Previsto |
| :--- | :--- |
| Preparação do ambiente e dados de teste | Semana 1 |
| Execução dos Casos de Teste funcionais | Semana 2 |
| Testes de integração (API, Firebase, Google Maps, Imgur) | Semana 2–3 |
| Testes não funcionais e exploratórios | Semana 3 |
| Correção de defeitos, reteste e regressão | Semana 4 |
| Fechamento e relatório final | Semana 5 |

> Este cronograma deverá ser atualizado a cada ciclo de desenvolvimento (sprint) do TCC.


