# Plano de Teste - ReGraphik
---

## 1. Identificação do Projeto

| Item | Detalhes |
| :--- | :--- |
| **Nome do Sistema** | **ReGraphik** (Aplicação Desktop WPF/WinForms + API REST .NET 8) |
| **Versão do Sistema** | `1.0.0` |
| **Equipe Responsável** | Equipe de Desenvolvimento e Qualidade ReGraphik (Bruno Maia (documentação técnica e modelagem de BD), Otávio Henrique (cliente WPF/MVVM), Lucas Aquino (cliente WPF/MVVM e GitHub), Luna Beatriz (API REST ASP.NET Core e front-end), Kaio Alves (integração Firebase e Google Maps Places API)) |
| **Data do Planejamento** | 10/08/2026 |

---

## 2. Objetivos dos Testes

O propósito deste Plano de Testes é garantir a qualidade, estabilidade e usabilidade da solução **ReGraphik** antes de sua distribuição aos usuários finais.

* **O que a equipe pretende verificar?**
  * Comunicação e integração contínua entre o aplicativo desktop e a API REST em .NET 8.
  * Validação das regras de negócio de orçamento, precificação e cadastro de pedidos gráficos.
  * Instalação e atualização via instalador Windows (`.msi` / `.exe`).
  * Desempenho e comportamento do sistema sob condições limites (entradas inválidas, perda de conexão com o banco).

* **Quais riscos pretende reduzir?**
  * Falhas críticas de integridade nos cálculos financeiros/orçamentários da gráfica.
  * Indisponibilidade ou falhas não tratadas no consumo dos endpoints da API REST.
  * Problemas de instalação, ausência de dependências runtime no ambiente do cliente e falhas de inicialização.

* **O que será considerado evidência de qualidade?**
  * Execução e aprovação de 100% dos casos de teste de alta severidade/prioridade.
  * Ausência de *crashes* não tratados na aplicação desktop.
  * Registros estruturados de evidências (logs de execução, screenshots e respostas HTTP atestadas).

---

## 3. Escopo

### 3.1 O que será testado (In-Scope)
* **Módulos Funcionais:**
  * Autenticação de usuários e controle de acessos (Login/Logout).
  * Módulo de Gestão de Clientes e Produtos Gráficos.
  * Módulo de Cálculo de Orçamentos e Emissão de Pedidos.
  * Consumo da API REST (`ApiRestReGraphik`) pelo cliente Desktop.
* **Aspectos Não-Funcionais:**
  * Desempenho e tempo de resposta da API REST.
  * Usabilidade das telas desktop em diferentes resoluções.
  * Processo de instalação e desinstalação (`ReGraphik_Setup.msi`).

### 3.2 O que NÃO será testado (Out-of-Scope)
* Testes de carga extrema / estresse massivo (acima de 500 requisições simultâneas por segundo).
* Compatibilidade com sistemas operacionais legados (Windows 7/8 ou edições Linux/macOS).
* Testes de invasão/segurança avançada (PenTest estrito no infra/servidor).

### 3.3 Funcionalidades Prioritárias
1. Autenticação e Autorização via Token.
2. Cálculo Orçamentário e Geração de Pedidos.
3. Comunicação síncrona Aplicação Desktop $\leftrightarrow$ API REST.
4. Processo de Instalação e Empacotamento WiX / Inno Setup.

---

## 4. Base de Teste

Os testes foram projetados com base nos seguintes artefatos e especificações técnicas:
* Documentação de Requisitos Funcionais e Não-Funcionais.
* Diagramas de Casos de Uso e Regras de Negócio de Precificação.
* Contrato da API REST (Swagger/OpenAPI do `.NET 8`).
* Protótipos de Interface de Usuário (Desktop WPF/WinForms).
* Estrutura de Banco de Dados Relacional e Regras de Integridade.

---

## 5. Abordagem de Testes

Para garantir a cobertura adequada, a equipe utilizará uma combinação das seguintes abordagens:

* **Testes Funcionais:** Verificação se cada funcionalidade atende aos requisitos de negócio.
* **Testes de Integração:** Validação da troca de dados via JSON entre o cliente Desktop e os controllers da API REST.
* **Testes de Sistema:** Validação do fluxo ponta a ponta (do login à geração final do orçamento).
* **Testes Não-Funcionais:** Avaliação do tempo de resposta das chamadas e comportamento da interface.
* **Testes Exploratórios:** Sessões livres para identificar comportamentos inesperados não mapeados formalmente.
* **Reteste:** Reexecução pontual do cenário após correção de um defeito reportado.
* **Testes de Regressão:** Execução da suíte de cenários prioritários após qualquer alteração relevante no código-fonte para garantir que funcionalidades existentes não foram quebradas.

---

## 6. Técnicas de Projeto de Testes

| Técnica | Justificativa de Uso no ReGraphik |
| :--- | :--- |
| **Particionamento de Equivalência** | Utilizado na validação de campos (ex: formatos de e-mail, CNPJ/CPF, preços positivos vs. negativos). |
| **Análise de Valor Limite (AVL)** | Aplicada no cálculo de quantidades mínimas/máximas de itens gráficos e descontos percentuais ($0\%$, $100\%$, valores negativos ou $>100\%$). |
| **Tabela de Decisão** | Aplicada no cálculo de descontos e permissões de acesso baseadas em perfis (Admin vs. Operador). |
| **Testes Baseados em Cenários** | Mapeamento de jornadas completas do usuário (ex: Criar cliente $\rightarrow$ Gerar orçamento $\rightarrow$ Aprovar pedido). |

---

## 7. Casos de Teste

| ID | Funcionalidade | Cenário / Condição | Entrada | Resultado Esperado | Resultado Obtido | Status |
| :--- | :--- | :--- | :--- | :--- | :--- | :--- |
| **CT001** | Autenticação | Login com credenciais válidas | Usuário: `admin`, Senha: `123456` | Acesso liberado à tela principal e Token JWT gerado. | — | ⏳ Pendente |
| **CT002** | Autenticação | Login com senha incorreta | Usuário: `admin`, Senha: `errada` | Mensagem de erro: *"Credenciais inválidas"*. Acesso negado. | — | ⏳ Pendente |
| **CT003** | Orçamentos | Cálculo de valor total com quantidade válida | Qtd: `1000`, Preço Un: `R$ 0,50` | Valor Total calculado e exibido como `R$ 500,00`. | — | ⏳ Pendente |
| **CT004** | Orçamentos | Inserção de quantidade zero ou negativa (AVL) | Qtd: `0` ou `-5` | Erro de validação bloqueando o cálculo: *"Quantidade inválida"*. | — | ⏳ Pendente |
| **CT005** | Clientes | Cadastro de cliente sem campos obrigatórios | Nome: `""`, CPF: `123...` | Bloqueio do cadastro realçando o campo obrigatório Nome. | — | ⏳ Pendente |
| **CT006** | API REST | Requisição sem Token JWT | `GET /api/pedidos` sem Header Bearer | Status HTTP `401 Unauthorized`. | — | ⏳ Pendente |
| **CT007** | Instalação | Instalação do app via arquivo `.msi` | Executar `ReGraphik_Setup.msi` | Instalação concluída no `Program Files` e atalho criado. | — | ⏳ Pendente |

---

## 8. Análise de Riscos

| Risco Identificado | Impacto | Probabilidade | Ação de Mitigação / Prevenção |
| :--- | :---: | :---: | :--- |
| **Indisponibilidade do Banco de Dados SQL** | Alto | Média | Criar mecanismo de retry na API REST e exibir mensagem amigável no app Desktop. |
| **Instabilidade da API REST / Perda de Conexão** | Alto | Média | Validar tratamentos de exceção de timeout/HTTP na aplicação desktop. |
| **Entrada de dados inválidos ou malformados** | Médio | Alta | Aplicar Particionamento de Equivalência e Análise de Valor Limite em todas as validações de input. |
| **Incompatibilidade de dependências no Windows (ex: .NET Runtime ausente)** | Alto | Baixa | Incluir precondição no instalador `.msi`/`.exe` para checar dependências necessárias. |

---

## 9. Reteste e Regressão

### 9.1 Conceitos e Aplicação no Projeto
* **Diferenciação Teórica:**
  * **Erro:** Ação humana incorreta realizada pelo desenvolvedor (ex: lógica matemática errada no código).
  * **Defeito (Bug):** A imperfeição presente no código-fonte decorrente do erro (ex: operador `+` no lugar de `*`).
  * **Falha:** A manifestação visível durante a execução do software (ex: orçamento com valor incorreto na tela).

* **Processo de Reteste:** Quando um defeito é corrigido, o testador reexecuta **exatamente o mesmo caso de teste** que falhou para validar se a correção funcionou.
* **Processo de Regressão:** Após a correção, o testador executa uma suíte de testes de outras funcionalidades correlatas para garantir que a alteração não quebrou áreas do sistema que funcionavam.

### 9.2 Cenários Selecionados para Suíte de Regressão
Sempre que houver uma alteração/nova versão, os 5 cenários abaixo deverão ser **obrigatoriamente retestados**:

1. **`CT001` - Autenticação no Sistema:** Garantir que as alterações de código não afetaram a camada de login e token.
2. **`CT003` - Cálculo de Orçamento:** Verificar a integridade dos cálculos financeiros de preços e descontos.
3. **`CT005` - Cadastro e Persistência de Dados:** Garantir o CRUD do banco de dados relacional.
4. **`CT006` - Autenticação da API REST (JWT):** Garantir a segurança dos endpoints expostos.
5. **`CT007` - Execução do Instalador:** Garantir que o build final continua gerando um pacote funcional.

---

## 10. Critérios de Entrada e Saída

### 10.1 Critérios de Entrada (Para iniciar os testes)
* [x] Ambiente de teste configurado e operacional (Banco de dados de teste + API REST em execução).
* [x] Build da Aplicação Desktop e/ou Instalador `.msi` compilado sem erros.
* [x] Casos de teste escritos e revisados.
* [x] Dados de teste cadastrados previamente no banco de dados.

### 10.2 Critérios de Saída (Para aprovar o lançamento/versão)
* [x] Execução de no mínimo **90%** de todos os casos de teste planejados.
* [x] **100% dos testes críticos/de alta prioridade aprovados** (Login, Orçamento, Integração API).
* [x] **Zero defeitos abertos com severidade Alta ou Crítica**.
* [x] Registros e evidências devidamente anexados para avaliação da equipe/orientador.

---

## 11. Evidências e Documentação

### 11.1 Registro de Evidências de Sucesso
Para cada teste executado, as seguintes evidências deverão ser salvas na pasta do projeto `/docs/evidencias/`:
* Capturas de tela (Prints) das interfaces com os resultados.
* Logs do Console/API HTTP em arquivo de texto.

### 11.2 Padrão para Registro de Defeitos (Bug Report)
Caso uma falha seja encontrada, ela deverá ser registrada (via GitHub Issues ou documento de controle) contendo obrigatoriamente a estrutura abaixo:

```text
[BUG] Título do Defeito

- Descrição: Breve resumo da falha observada.
- Passos para Reprodução:
  1. Abrir a tela de Orçamentos.
  2. Digitar a quantidade -5.
  3. Clicar em Calcular.
- Resultado Esperado: Mensagem informando que a quantidade deve ser maior que zero.
- Resultado Obtido: A aplicação trava com a exceção 'Unhandled Exception'.
- Severidade: Alta (Impacta a usabilidade / Crash)
- Prioridade: Alta (Deve ser corrigido no ciclo atual)
- Ambiente: Windows 11, ReGraphik v1.0.0, API Localhost.
- Evidência: print_erro_calculo.png anexado.
