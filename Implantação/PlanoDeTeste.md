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
* **Especificação da API REST / Swagger (OpenAPI):** Disponível em `https://webregraphik.runasp.net`.
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

---

## 7. Casos de Teste

| ID | Funcionalidade | Cenário / Condição | Entrada | Resultado Esperado | Resultado Obtido | Status |
| :--- | :--- | :--- | :--- | :--- | :--- | :--- |
| **CT001** | Login | Autenticação com credenciais válidas | Login: `operador`, Senha: `123456` | Acesso liberado, Token JWT/Firebase gerado e redirecionamento para a Dashboard. | — | ⏳ Pendente |
| **CT002** | Login | Tentativa de login com senha incorreta | Login: `operador`, Senha: `errada` | Mensagem de erro *"Usuário ou senha inválidos"* e acesso bloqueado. | — | ⏳ Pendente |
| **CT003** | Cadastro de Resíduo | Inclusão de aparas de papel com dados válidos | Tipo: `Papel A4`, Qtd: `50`, Dimensões: `21x29.7cm` | Resíduo cadastrado com sucesso, status `Disponível` e visível no Estoque Reverso. | — | ⏳ Pendente |
| **CT004** | Cadastro de Resíduo | Tentativa de cadastro com quantidade zerada/negativa (AVL) | Tipo: `Vinil`, Qtd: `-5` | Bloqueio do formulário exibindo mensagem *"Quantidade deve ser maior que zero"*. | — | ⏳ Pendente |
| **CT005** | Estoque Reverso | Filtragem dinâmica por tipo de resíduo | Filtro: `Vinil` | A listagem atualiza via `ICollectionView` mostrando apenas resíduos do tipo Vinil. | — | ⏳ Pendente |
| **CT006** | Pontos de Coleta | Busca de pontos de reciclagem por cidade | Cidade: `Nova Lima` | Chamada à Google Places API e exibição dos pins dos pontos no mapa Leaflet/WebView2. | — | ⏳ Pendente |
| **CT007** | Chat Tempo Real | Envio de mensagem entre dois usuários ativos | Texto: *"Resíduo reservado para coleta"* | Mensagem é salva no Firebase e aparece instantaneamente na janela do destinatário. | — | ⏳ Pendente |
| **CT008** | Relatórios / ESG | Exportação de relatório consolidado em PDF | Clicar em *"Exportar PDF"* | Arquivo PDF gerado via QuestPDF e salvo no computador com os dados e gráficos compilados. | — | ⏳ Pendente |
| **CT009** | Instalação | Execução do instalador Windows | Executar `ReGraphik_Setup.msi` | Instalação concluída no pasta `Program Files`, com atalhos criados no Menu Iniciar/Área de Trabalho. | — | ⏳ Pendente |

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
