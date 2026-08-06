# Migracao.md — ReGraphik

**Unidade Curricular:** Implantação de Sistemas
**Projeto:** ReGraphik — Sistema de Gestão de Estoque Reverso
**Repositório:** https://github.com/BrunoMaiaSenai/ReGraphikApp

---

## Material de uso exclusivo

Este documento é de uso exclusivo da equipe do projeto ReGraphik e do SENAI
Afonso Greco – Nova Lima, elaborado como artefato obrigatório da Unidade
Curricular *Implantação de Sistemas*, sob orientação do instrutor **Frederico
Martins Aguiar**. Sua reprodução ou redistribuição fora do contexto acadêmico
não é autorizada. O conteúdo deve ser mantido versionado no repositório
GitHub do projeto, refletindo sempre o estado mais recente da estratégia de
migração e backup adotada pela equipe.

---

## 1. Dados do Sistema

| Campo | Descrição |
|---|---|
| Nome do sistema | ReGraphik — Sistema de Gestão de Estoque Reverso |
| Versão atual | v0.3 (Fase 3 — API REST em progresso) |
| Unidade SENAI | Senai Afonso Greco – Nova Lima |
| Instrutor orientador | Frederico Martins Aguiar |
| Equipe | Bruno Maia (documentação técnica e modelagem de BD), Otávio Henrique (cliente WPF/MVVM), Lucas Aquino (cliente WPF/MVVM e GitHub), Luna Beatriz (API REST ASP.NET Core e front-end), Kaio Alves (integração Firebase e Google Maps Places API) |
| Domínio da API | https://webregraphik.runasp.net |
| Documentação interativa | `/swagger` (Swashbuckle / OpenAPI) |
| Repositório de código | https://github.com/BrunoMaiaSenai/ReGraphikApp |
| Ambiente de desenvolvimento | Visual Studio 2022 Community Edition, SDK .NET 8/.NET 9, Windows 11 |
| Empresa parceira (Mini Mundo) | AML (setor gráfico) |

---

## 2. Banco Utilizado

### 2.1 Banco de dados atual
- **Firebase Realtime Database** (NoSQL, hospedado em nuvem pela Google, plano Spark/Blaze).
- Persistência via **API REST em ASP.NET Core**, que concentra regras de negócio, validações e autenticação antes de gravar/consultar no Firebase.
- SLA declarado do provedor: 99,95% de disponibilidade.

### 2.2 Banco anterior (legado do cliente desktop)
- **SQLite** (`sistema.db`), acessado diretamente pelo cliente WPF, sem camada de API.
- Cada instalação do WPF possuía seu próprio arquivo local, sem sincronização entre usuários da mesma empresa.

### 2.3 Motivo da migração já realizada (histórico)
| Problema no SQLite | Solução no Firebase + API |
|---|---|
| Dados isolados por máquina | Dados centralizados em nuvem, compartilhados entre todos os usuários da empresa |
| Login incompatível com o novo modelo de autenticação | Telas de login/cadastro recriadas com token de e-mail e Firebase |
| Sem sincronização em tempo real | Firebase Realtime Database sincroniza automaticamente |
| Regras de negócio espalhadas nos clientes | API REST concentra validações e regras de negócio |

---

## 3. Estrutura Atual

### 3.1 Modelo relacional de origem (referência lógica)
Mesmo persistindo em NoSQL, o domínio segue um modelo relacional de referência com cinco entidades principais:

| Entidade | Papel |
|---|---|
| `CadastroUsuarios` (Usuario) | Usuários do sistema, vinculados a uma empresa |
| `TipoMaterial` | Classificação dos resíduos (pré-configurada pelo Administrador) |
| `CadastroResiduos` (Residuo) | Registro de cada resíduo gerado no processo produtivo |
| `Sugestoes` | Sugestões de reaproveitamento por tipo de material |
| `SugestoesResiduos` | Entidade associativa N:N entre `Residuo` e `Sugestoes`, com data de aplicação |

Relacionamentos principais:
- `USUARIO` 1:N `CADASTRO_RESIDUOS`
- `TIPO_MATERIAL` 1:N `CADASTRO_RESIDUOS` e 1:N `SUGESTOES`
- `CADASTRO_RESIDUOS` N:N `SUGESTOES`, resolvido por `SUGESTOES_RESIDUOS`
- Demais entidades de apoio: `PontosColeta`, `Mensagens`/`Conversas` (chat interno)

### 3.2 Arquitetura em camadas
| Camada | Componentes | Responsabilidade |
|---|---|---|
| VIEW | MainWindow, DashboardPage, ResiduosPage, EstoqueReversoPage, MapaPage, RelatoriosPage | Apresentação (XAML), sem lógica de negócio |
| VIEWMODEL | BaseViewModel, ResiduoViewModel, LoginViewModel, MapViewModel | Lógica de apresentação, `INotifyPropertyChanged`, `RelayCommand` |
| MODEL | Usuario, Residuo, Sugestao, SugestaoResiduo, PontosColeta | Entidades de domínio (POCOs) |
| SERVICE LAYER | GooglePlacesService, ApiService | Integração HTTP com API REST e Google Maps |
| BACKEND | API REST (ASP.NET Core, 5 controllers CRUD) | Regras de negócio e persistência no Firebase |
| PERSISTÊNCIA | Firebase Realtime Database | Armazenamento NoSQL em nuvem |

### 3.3 Status atual dos módulos
| Módulo | Status |
|---|---|
| Mapa / Pontos de Coleta | Disponível |
| Dashboard | Em construção |
| Cadastro de Resíduo | Em construção |
| Sugestões | Em construção |
| Relatórios | Em construção |
| Chat Interno | Implementação futura |
| Central de Notificações | Implementação futura |
| Log de Consulta | Implementação futura |

### 3.4 Volume de dados atual
Projeto em fase acadêmica/piloto: volume ainda reduzido (dados de teste da equipe e da empresa AML). Não há histórico de produção em larga escala até o momento.

---

## 4. Alterações Previstas

- Conclusão dos controllers **Sugestao** e **SugestaoResiduos** no padrão CRUD completo.
- Implementação de **autenticação JWT** na API REST (hoje ausente — risco técnico de probabilidade **Alta** e impacto **Alto**, conforme análise de viabilidade técnica).
- Finalização dos módulos **Dashboard**, **Cadastro de Resíduo** e **Relatórios** (KPIs, exportação em CSV/PDF).
- Possível migração do plano **Firebase Spark (gratuito)** para **Blaze (pay-as-you-go)** caso a cota gratuita seja excedida.
- Avaliação de **cache local (SQLite)** para funcionamento offline parcial em caso de perda de conectividade.
- Possível criação de versão web (Blazor) ou mobile (.NET MAUI) para reduzir a dependência exclusiva do WPF/Windows.
- Novas tabelas/campos podem ser adicionados aos módulos em construção (ex.: campos de auditoria para exclusão de resíduos, já previstos nas regras de negócio, mas ainda não totalmente implementados).
- **Substituição do banco de dados: Firebase Realtime Database (NoSQL) e SQLite (legado) → Microsoft SQL Server (relacional).** Trata-se da alteração mais crítica prevista, pois envolve troca completa do banco de dados em produção. Principais pontos de atenção:
  - **Motivação:** ganhar integridade referencial nativa (chaves estrangeiras reais entre `Usuario`, `TipoMaterial`, `Residuo`, `Sugestao` e `SugestoesResiduos`), consultas relacionais/JOINs mais eficientes para Dashboard e Relatórios, e eliminar a dependência de dois bancos distintos (Firebase em produção e SQLite como cache/legado).
  - **Impacto na arquitetura:** a camada de persistência da API REST deixa de usar o Firebase SDK e passa a usar um ORM (ex.: Entity Framework Core) sobre SQL Server; os Controllers e Services precisam ser adaptados, mas a camada de Views/ViewModels do cliente WPF tende a permanecer inalterada, pois consome apenas a API.
  - **Modelagem:** os modelos Conceitual, Lógico e Físico já elaborados (BRModelo, cinco entidades principais) servem de base direta para a criação do schema relacional no SQL Server, com tipos de dados definidos (VARCHAR, DATETIME, DECIMAL, ENUM/CHECK constraints).
  - **Dados a migrar:** todos os nós atualmente no Firebase (`Usuarios`, `TipoMaterial`, `Residuos`, `Sugestoes`, `SugestoesResiduos`, `PontosColeta`) e eventuais registros ainda existentes no SQLite legado.
  - **Risco:** por ser uma migração de um banco não relacional para um banco relacional (e não apenas uma atualização simples de estrutura), o risco de perda ou inconsistência de dados é maior que nas demais alterações previstas, exigindo planejamento cuidadoso e testes de validação reforçados, incluindo uma etapa extra de conversão dos documentos do Firebase para linhas de tabelas relacionais.
  - **Convivência temporária:** durante a transição, recomenda-se manter o Firebase em modo somente leitura (read-only) como fonte de consulta de contingência até a validação completa do SQL Server em produção.

Cada uma dessas alterações representa um evento de migração — seja mudança na estrutura dos dados, nova versão do sistema ou reorganização — e por isso deve ser sempre planejada com backup validado, testada primeiro em homologação e só depois aplicada em produção, com um plano de retorno pronto caso algo dê errado. A substituição de banco (Firebase/SQLite → SQL Server), em especial, deve ser tratada como uma migração completa, já que envolve a totalidade dos dados do sistema, e não apenas parte deles.

---

## 5. Estratégia de Backup

### 5.1 Regra de ouro
Nenhuma alteração estrutural, migração ou deploy em produção deve ocorrer sem um backup completo e validado, seguindo a regra de ouro da implantação: *"Nunca execute uma migração diretamente em produção sem planejamento, backup validado e um plano de retorno (rollback)."*

### 5.2 O que é copiado no backup
| Item | Método |
|---|---|
| Firebase Realtime Database (produção) | Exportação do JSON completo via Firebase Console / Admin SDK (`.export()` ou `firebase database:get /`) |
| Configurações da API (`appsettings.json`, chaves do Google Service Account) | Cópia segura fora do repositório público (secret manager / variável de ambiente) |
| Código-fonte (API + Cliente WPF) | Versionado no Git/GitHub — tags de release a cada backup relevante |
| Banco legado SQLite (`sistema.db`), quando aplicável | Cópia do arquivo `.db` para local seguro antes de qualquer alteração |
| Documentação técnica (Modelos Conceitual, Lógico, Físico) | Versionada junto ao código, na pasta de Modelagem do repositório |

### 5.3 Periodicidade
- **Antes de qualquer migração ou deploy:** backup completo obrigatório.
- **Diário (automático):** exportação incremental do Firebase enquanto o sistema estiver em uso ativo pela equipe/empresa AML.
- **Semanal:** backup completo consolidado, armazenado em local externo ao Firebase (ex.: repositório privado ou storage em nuvem separado).

### 5.4 Local de armazenamento
- Cópias fora do ambiente de produção (Firebase de homologação, storage externo ou repositório privado), nunca apenas no mesmo projeto Firebase de produção.

---

## 6. Estratégia de Migração

### 6.1 Fluxo geral (baseado no fluxo de 6 etapas da Aula 03)
1. **Planejamento** — definir cronograma, responsável, janela de manutenção, recursos necessários, riscos e plano de rollback para cada alteração prevista.
2. **Backup** — executar backup completo do Firebase, das configurações e do código-fonte, seguindo a estratégia de backup adotada pela equipe.
3. **Validação do Backup** — restaurar o backup em ambiente de homologação e confirmar integridade antes de qualquer alteração em produção.
4. **Migração** — aplicar a alteração (novo controller, novo campo, JWT, etc.) primeiro em homologação, depois em produção.
5. **Testes** — validar funcionalidades, integração com o banco, API, mapa/pontos de coleta e desempenho.
6. **Produção (Go Live)** — disponibilizar a alteração aos usuários e iniciar monitoramento intensivo nas primeiras horas.

### 6.2 Tipo de migração aplicável ao ReGraphik
- **Migração completa:** já ocorreu uma vez, quando o sistema saiu do SQLite local para o Firebase junto com a API, migrando usuários, resíduos e demais entidades por completo.
- **Migração incremental:** aplicável às próximas evoluções (novos campos, novos módulos), pois o sistema já está em uso e não pode ser reconstruído do zero a cada alteração.
- **Migração parcial:** poderá ser usada em cenários específicos, como migrar apenas registros de uma empresa/cliente para um ambiente isolado de testes.

### 6.3 Etapas técnicas de uma migração de dados no Firebase
| Etapa | Objetivo | Resultado Esperado |
|---|---|---|
| Exportação | Extrair o JSON atual do nó afetado (Usuarios, Residuos, etc.) | Arquivo de exportação íntegro |
| Transformação | Ajustar estrutura (novos campos, tipos, limpeza de inconsistências) | Dados compatíveis com o novo modelo |
| Importação | Gravar os dados transformados no Firebase (produção ou homologação) | Banco atualizado corretamente |
| Validação | Conferir quantidade de registros antes/depois e integridade referencial (IDs de usuário, tipo de material, sugestões aplicadas) | Dados íntegros e sistema operante |

### 6.4 Ambiente de homologação
Toda migração deve ser executada primeiro em um projeto Firebase de homologação/teste, isolado do projeto de produção, replicando a mesma estrutura de nós e regras de segurança, antes de ser aplicada ao ambiente real usado pela empresa AML.

### 6.5 Cenário: Migração de Cliente com Sistema Próprio em SQL Server

Cenário aplicável quando uma empresa cliente (ex.: uma nova gráfica além da AML) já possui
um sistema legado próprio, com banco de dados **SQL Server** estruturado de forma diferente
do modelo do ReGraphik, e deseja migrar seus dados históricos (usuários, materiais, resíduos,
pontos de coleta etc.) para o ReGraphik. Este cenário é mais complexo que uma atualização
interna de schema, pois envolve **dois modelos de dados distintos** e uma base já em produção
no cliente.

#### 6.5.1 Etapas específicas

1. **Levantamento (Discovery)**
   - Mapear o schema do banco SQL Server do cliente: tabelas, colunas, tipos de dados, chaves
     primárias/estrangeiras, constraints, triggers e stored procedures existentes.
   - Identificar volume de dados (linhas por tabela) e qualidade dos dados (nulos, duplicados,
     inconsistências, campos livres sem padronização).

2. **Mapeamento De-Para (Data Mapping)**
   - Construir uma planilha/tabela de mapeamento campo a campo entre o schema do cliente e as
     cinco entidades do ReGraphik (`Usuario`, `TipoMaterial`, `Residuo`, `Sugestao`,
     `SugestaoResiduo`).
   - Exemplo de mapeamento:

     | Campo no sistema do cliente | Campo no ReGraphik | Observação |
     |---|---|---|
     | `TB_CLIENTE.NOME_COMPLETO` | `Usuario.Nome` | Sem transformação |
     | `TB_CLIENTE.EMAIL_CORP` | `Usuario.Email` | Validar formato de e-mail |
     | `TB_RESIDUO.COD_TIPO` (int) | `TipoMaterial.Id` (código único gerado pelo sistema) | Requer tabela de conversão de chave (De-Para de IDs) |
     | `TB_RESIDUO.QTD` (float) | `CadastroResiduos.Quantidade` (decimal) | Ajuste de precisão numérica |
     | *(sem correspondência)* | `Residuo.Status` | Definir valor padrão (`Em Estoque`) para registros migrados sem status |

   - Registrar explicitamente campos do cliente **sem correspondência** no ReGraphik (serão
     descartados ou armazenados em um campo de observações) e campos obrigatórios do
     ReGraphik **sem origem** no sistema do cliente (definir valores padrão ou pendência manual).

3. **Scripts de Migração**
   - **Extração:** scripts em SQL (`SELECT ... INTO`) ou ferramentas de importação/exportação
     do próprio SQL Server (como o Assistente de Importação e Exportação de Dados) para tirar
     os dados do banco do cliente e colocá-los em uma **área temporária de trabalho** (uma
     cópia isolada, separada do banco final, usada só para organizar e conferir os dados antes
     da carga definitiva), seja no mesmo banco ou em um arquivo intermediário CSV/JSON.
   - **Transformação:** scripts que aplicam o mapeamento de-para (conversão de tipos de dados,
     geração de novos códigos únicos de identificação, padronização das opções de status,
     limpeza de duplicados e campos em branco).
   - **Carga:** scripts de `INSERT` (ou chamadas em lote aos endpoints da API REST do
     ReGraphik, ex.: `POST /api/residuos`) para gravar os dados já transformados no banco de
     destino do ReGraphik.
   - Todo script deve poder ser **executado mais de uma vez sem duplicar dados** (checando
     antes se o registro já existe: se existir, atualiza; se não existir, insere), permitindo
     reprocessar a carga em caso de falha parcial sem gerar registros repetidos.

4. **Testes da Migração**
   - **Teste simulado (ensaio da migração):** executar o processo completo em ambiente de
     homologação, com uma cópia dos dados reais do cliente, como um "treino" antes da migração
     de verdade — sem afetar o ambiente de produção.
   - **Teste com amostra:** migrar um subconjunto (ex.: 1 mês de dados) e validar manualmente
     registro a registro antes de migrar a base completa.
   - **Teste de carga completa:** medir tempo total do processo (extração + transformação +
     carga) para dimensionar a janela de manutenção necessária no Go Live.
   - **Teste de reconciliação:** comparar contagem de registros e somatórios (ex.: quantidade
     total de resíduos em kg) entre origem (SQL Server do cliente) e destino (ReGraphik) após
     a carga.

5. **Validações**
   - Integridade referencial: todo `IdUsuario`/`IdTipoMaterial` referenciado em `Residuo`
     existe na tabela correspondente após a carga.
   - Ausência de duplicidade: chaves de negócio (ex.: e-mail do usuário, combinação
     material+origem+data) não se repetem indevidamente.
   - Conformidade de domínio: valores de status, tipo de material e demais campos com
     domínio fechado (enum) estão dentro dos valores aceitos pelo ReGraphik.
   - Validação funcional pós-carga: login dos usuários migrados, listagem de resíduos migrados
     no cliente WPF, geração de relatório com os dados migrados.

6. **Rollback específico do cenário**
   - Manter o backup do banco do cliente (origem) intacto e o backup do ReGraphik anterior à
     carga (destino), permitindo reverter a carga sem afetar o sistema legado do cliente, que
     deve continuar operacional até a validação completa da migração.

#### 6.5.2 Checklist da migração de cliente com SQL Server próprio
- [ ] Schema do cliente mapeado e documentado
- [ ] Planilha de-para (campo a campo) revisada e aprovada pela equipe e, se possível, pelo cliente
- [ ] Scripts de extração, transformação e carga versionados no repositório
- [ ] Scripts testados em um ensaio simulado com dados reais (ambiente de homologação)
- [ ] Amostra migrada e validada manualmente
- [ ] Reconciliação de contagens/somatórios entre origem e destino sem divergência
- [ ] Integridade referencial validada no destino
- [ ] Sistema legado do cliente mantido intacto até validação completa
- [ ] Plano de rollback testado
- [ ] Janela de manutenção definida e comunicada ao cliente

### 6.6 Cenário: Importação de Dados a partir de Arquivos PDF e Excel

Cenário aplicável quando uma empresa cliente (especialmente gráficas pequenas e médias, que
ainda não possuem nenhum sistema informatizado de controle de resíduos) controla seus dados
apenas em **planilhas Excel** e **relatórios em PDF**, e deseja trazer esse histórico para
dentro do ReGraphik. É exatamente o cenário descrito no problema que deu origem ao projeto:
empresas que recorrem a planilhas manuais por falta de uma ferramenta digital própria. Esse
tipo de origem é bem diferente de migrar de um banco de dados, porque **não existe uma
estrutura fixa e confiável de tabelas** — os dados foram digitados por pessoas, em formatos
livres, e podem conter erros de digitação, células mescladas, colunas fora de ordem e até
páginas de PDF que são apenas imagens escaneadas, sem texto que possa ser copiado.

#### 6.6.1 Características e desafios das fontes de dados

| Fonte | Formato típico | Principais desafios |
|---|---|---|
| Planilha Excel | Uma linha por resíduo, com colunas como tipo de material, data, quantidade e origem | Colunas fora de padrão, células mescladas, fórmulas em vez de valores prontos, uso de vírgula ou ponto para separar decimais, linhas em branco ou de totalização misturadas com os dados |
| Relatório em PDF gerado por computador | Texto selecionável, geralmente em formato de tabela | Texto que "quebra" ao ser copiado (colunas viram uma linha só de texto corrido), cabeçalhos repetidos em cada página |
| Relatório em PDF escaneado (papel digitalizado) | Apenas uma imagem da página, sem texto selecionável | Não é possível copiar o texto diretamente; é necessário reconhecimento de caracteres a partir da imagem (uma tecnologia chamada OCR), que pode errar letras e números parecidos (ex.: confundir "0" com "O", ou "1" com "l") |

#### 6.6.2 Etapas específicas

1. **Levantamento das fontes**
   - Reunir todas as planilhas e PDFs que a empresa possui, identificando quais realmente têm
     dados de resíduos, materiais e pontos de coleta, e quais são apenas anotações soltas ou
     relatórios administrativos sem relação com o estoque reverso.
   - Verificar se os PDFs têm texto que pode ser selecionado/copiado ou se são apenas imagens
     escaneadas, pois isso muda totalmente a forma de extrair os dados.

2. **Extração dos dados**
   - **Planilhas Excel:** ler o arquivo diretamente com uma ferramenta de programação
     (por exemplo, um script simples em Python usando bibliotecas de leitura de planilhas), ou,
     em último caso, exportar a planilha para o formato CSV (texto separado por vírgulas) antes
     de processar.
   - **PDFs com texto selecionável:** usar uma ferramenta de extração de texto/tabelas de PDF,
     que localiza as colunas e linhas da tabela dentro do documento e as transforma em dados
     organizados.
   - **PDFs escaneados (apenas imagem):** usar uma ferramenta de reconhecimento de caracteres a
     partir de imagem (OCR) para transformar a imagem da página em texto, e só depois aplicar a
     extração de tabela sobre esse texto reconhecido.

3. **Conferência manual dos dados extraídos**
   - Toda extração automática de PDF/Excel deve passar por uma conferência humana, comparando
     uma amostra dos dados extraídos com o arquivo original, especialmente nos casos que
     passaram por OCR, já que o reconhecimento de caracteres pode gerar erros silenciosos
     (números trocados, por exemplo).

4. **Padronização e transformação**
   - Definir um mapeamento de-para entre as colunas encontradas nas planilhas/PDFs e os campos
     do ReGraphik (`TipoMaterial`, `Quantidade`, `Origem`, `Data`, `Status`, etc.), da mesma
     forma feita para uma migração de banco de dados.
   - Padronizar formatos que variam bastante em arquivos preenchidos manualmente: datas escritas
     de formas diferentes (ex.: "05/08/2026", "5 de agosto de 2026", "2026-08-05"), quantidades
     com vírgula ou ponto decimal, nomes de materiais escritos de formas diferentes para o mesmo
     tipo (ex.: "papelão", "papel/cartão", "PAPELAO").
   - Descartar ou sinalizar linhas de totalização, cabeçalhos repetidos e linhas em branco que
     não representam um resíduo de verdade.

5. **Carga dos dados**
   - Após a padronização, os dados seguem o mesmo caminho de uma carga normal: scripts de
     inserção ou chamadas em lote aos endpoints da API REST do ReGraphik (ex.:
     `POST /api/residuos`), sempre verificando antes se aquele registro já foi inserido, para
     não duplicar.

6. **Testes da importação**
   - **Teste com um arquivo pequeno primeiro:** extrair e importar uma única planilha ou um
     único PDF antes de processar o lote completo, revisando manualmente todos os registros
     gerados.
   - **Teste de arquivo "difícil":** testar deliberadamente com uma planilha com células
     mescladas, um PDF com tabela quebrada entre páginas e, se houver, um PDF escaneado, para
     garantir que o processo lida bem com os piores casos, e não só com os arquivos "perfeitos".
   - **Teste de reconciliação:** comparar a quantidade total de resíduos (em kg, por exemplo)
     somada nos arquivos originais com o total que entrou no ReGraphik após a importação.

7. **Validações**
   - Nenhum registro deve entrar no ReGraphik sem um tipo de material reconhecido pelo
     sistema — registros com material não identificado devem ficar pendentes de revisão manual,
     nunca ser descartados silenciosamente.
   - Datas fora de um intervalo plausível (ex.: datas futuras ou muito antigas, geradas por erro
     de digitação ou de leitura do OCR) devem ser sinalizadas para conferência antes de entrar
     em produção.
   - Quantidades zeradas, negativas ou absurdamente altas (indicando erro de digitação ou de
     leitura) devem ser sinalizadas, não apenas aceitas automaticamente.
   - Ao final, um relatório de importação deve listar quantos registros foram importados com
     sucesso, quantos ficaram pendentes de revisão e quantos foram descartados, com o motivo de
     cada descarte.

#### 6.6.3 Checklist da importação de PDF e Excel
- [ ] Todas as planilhas e PDFs relevantes foram reunidos e identificados
- [ ] Verificado quais PDFs têm texto selecionável e quais são apenas imagem escaneada
- [ ] Ferramenta de extração definida para cada tipo de arquivo (planilha, PDF com texto, PDF escaneado com OCR)
- [ ] Amostra dos dados extraídos conferida manualmente contra o arquivo original
- [ ] Mapeamento de-para das colunas/campos definido e documentado
- [ ] Formatos de data, decimal e nomes de material padronizados
- [ ] Linhas de totalização, cabeçalhos repetidos e linhas em branco tratadas
- [ ] Teste realizado com arquivo pequeno antes do lote completo
- [ ] Teste realizado com arquivo "difícil" (células mescladas, tabela quebrada, PDF escaneado)
- [ ] Reconciliação de totais entre arquivo original e sistema realizada
- [ ] Registros com material não identificado, data ou quantidade suspeita sinalizados para revisão manual
- [ ] Relatório final de importação gerado (sucesso, pendente, descartado)

### 6.7 Plano de rollback
- Manter o backup validado, gerado conforme a estratégia de backup adotada pela equipe, pronto para restauração imediata.
- Manter a versão anterior da API publicada (ou facilmente reimplantável) em caso de falha após o Go Live.
- Documentar, a cada migração, os passos exatos de reversão (ex.: reimportar o JSON anterior, reverter deploy da API para a tag de release anterior no GitHub).

---

## 7. Testes de Validação de Backup

Um backup só tem valor quando sua restauração é testada. A validação deve seguir o fluxo abaixo:

1. **Selecionar o backup** — escolher a exportação mais recente e íntegra do Firebase.
2. **Restaurar em ambiente de testes** — importar o JSON no projeto Firebase de homologação, sem impactar a produção.
3. **Confirmar integridade** — verificar se todos os nós (Usuarios, TipoMaterial, Residuos, Sugestoes, SugestoesResiduos, PontosColeta) foram recuperados corretamente, com as mesmas contagens de registros do momento do backup.
4. **Validar funcionalmente** — executar consultas via Swagger (`/swagger`) e operações básicas no cliente WPF (login, listagem de resíduos, busca de pontos de coleta) para confirmar que os dados restaurados funcionam normalmente.

### 7.1 Checklist de validação (básico)
- [ ] Backup realizado
- [ ] Backup restaurado em ambiente de homologação
- [ ] Contagem de registros confere com a origem
- [ ] Relacionamentos (Usuario → Residuo → Sugestoes) íntegros após restauração
- [ ] Login e autenticação funcionando com os dados restaurados
- [ ] Endpoints principais testados via Swagger (`/api/residuos`, `/api/usuarios`, `/api/sugestoes/aplicar`, `/api/coleta/proximos`)
- [ ] Cliente WPF consegue consumir a API restaurada sem erros
- [ ] Resultado documentado (data, responsável, sucesso/falha)

### 7.2 Cenários Reais de Falha e Validações Detalhadas

Além do fluxo básico (7.1), a equipe deve projetar e testar deliberadamente os cenários de
falha abaixo, pois um backup "aparentemente correto" pode falhar silenciosamente em produção.

#### 7.2.1 Backup corrompido ou incompleto
| Situação simulada | Como testar | Validação esperada |
|---|---|---|
| Arquivo de backup truncado (falha de rede/energia durante a exportação) | Interromper deliberadamente uma exportação do Firebase/SQL Server em homologação e tentar restaurar o arquivo parcial | O processo de restauração deve **falhar de forma explícita** (erro de parsing/JSON inválido ou arquivo `.bak` inconsistente), nunca "restaurar parcialmente" sem avisar |
| Código de verificação do arquivo não confere | Gerar um **código de verificação** do arquivo de backup (uma espécie de "impressão digital" do arquivo, calculada automaticamente) no momento em que ele é criado, e comparar esse mesmo código no momento da restauração | Os códigos devem ser idênticos; se forem diferentes, o arquivo foi alterado ou corrompido em algum momento e deve ser descartado, usando-se o backup do dia anterior |
| Backup vazio (0 registros) por falha silenciosa na rotina automática de exportação | Simular uma falha na rotina automática de backup (agendada para rodar sozinha) e verificar o tamanho do arquivo gerado | O processo de backup deve emitir um alerta automático quando o volume de dados exportado for muito menor que o esperado (ex.: uma variação maior que 20% em relação ao backup anterior) |
| Backup de banco relacional (SQL Server) feito no meio de uma operação ainda não finalizada | Realizar o backup durante uma carga de dados simulada em andamento | Antes de restaurar de fato, deve-se rodar primeiro um **comando de verificação do backup** (que confere se o arquivo está íntegro e consistente, sem restaurá-lo ainda), só então prosseguindo para a restauração completa |

#### 7.2.2 Incompatibilidade de campos e mudanças na estrutura do banco
| Situação simulada | Como testar | Validação esperada |
|---|---|---|
| Campo novo adicionado ao schema atual, ausente no backup antigo | Restaurar um backup gerado antes da inclusão de um novo campo obrigatório (ex.: campo de auditoria) | Script de restauração deve aplicar valor padrão ou marcar o registro como pendente, sem quebrar a carga inteira |
| Mudança de tipo de dado (ex.: `Quantidade` de número com casas decimais soltas para um formato com casas decimais fixas) | Restaurar um backup com o tipo antigo sobre a estrutura nova do banco | Verificar se os valores foram arredondados corretamente e se não ocorreu **erro de valor muito grande para o campo** (quando um número não cabe no novo tipo de dado, ou um texto é cortado por ser maior que o novo limite de caracteres) |
| Excesso de caracteres em campo `VARCHAR` de tamanho reduzido | Restaurar registros com strings mais longas que o novo limite de coluna | Sistema deve rejeitar ou truncar de forma controlada (nunca truncar silenciosamente sem log) |
| Constraint `NOT NULL` adicionada após o backup ter sido gerado | Restaurar dados antigos com campos nulos em uma coluna hoje obrigatória | Processo deve identificar e reportar todos os registros que violam a nova constraint antes de aceitar a restauração como concluída |
| Enum/domínio de `Status` alterado (ex.: novo status incluído) | Restaurar backup com valores de status do domínio antigo | Validar que todos os valores restaurados ainda são reconhecidos pela aplicação; caso contrário, mapear para um valor de transição |
| Chave estrangeira apontando para um ID que não existe mais no destino | Restaurar `Residuo` referenciando um `TipoMaterial` removido após o backup | Restauração deve reportar órfãos (registros sem FK válida) em vez de inserir uma referência quebrada |

#### 7.2.3 Falhas de ambiente e infraestrutura
| Situação simulada | Como testar | Validação esperada |
|---|---|---|
| Credenciais/permissões expiradas (Google Service Account, login SQL Server) | Tentar restaurar com uma credencial revogada/expirada propositalmente em homologação | Falha deve ser clara e imediata (erro de autenticação), documentando o procedimento de renovação de credenciais |
| Restauração em servidor com recursos insuficientes (disco/memória) | Restaurar um backup grande em uma instância de homologação com pouco espaço em disco | Processo deve falhar de forma segura, sem corromper o ambiente de destino, e alertar sobre a limitação de recursos |
| Diferença de versão do banco de dados (ex.: backup gerado em uma versão do SQL Server mais nova sendo restaurado em uma instalação mais antiga) | Testar a restauração entre versões diferentes do SQL Server disponíveis para a equipe | Verificar a compatibilidade de versão antes de aceitar o backup como restaurável, usando um comando que lê apenas as informações do backup (versão e origem) sem restaurá-lo de fato |
| Restauração muito demorada em um backup grande | Restaurar o maior backup disponível e medir o tempo total gasto | O tempo de restauração deve ser compatível com o tempo máximo de indisponibilidade aceitável, definido previamente pela equipe para o projeto |

#### 7.2.4 Validações de consistência de negócio pós-restauração
- **Reconciliação numérica:** soma total de `Quantidade` em `Residuo` no backup restaurado deve bater com o valor registrado no momento do backup (auditoria de totais, não só de contagem de linhas).
- **Integridade de relacionamento N:N:** cada registro em `SugestoesResiduos` deve referenciar um `Residuo` e uma `Sugestao` existentes; registros órfãos indicam falha na restauração.
- **Duplicidade pós-restauração:** verificar se a restauração não gerou registros duplicados (ex.: o mesmo `Residuo` inserido duas vezes por um script que foi executado mais de uma vez sem checar se o dado já existia).
- **Auditoria e status:** validar se resíduos marcados como `Reaproveitado`/`Descartado` mantêm o histórico de aplicação de sugestão (`DataAplicacao`) após a restauração, e não retornam para `Em Estoque` indevidamente.
- **Validação cruzada com log da aplicação:** comparar os logs de auditoria (exclusões restritas ao perfil Administrador) gerados antes do backup com o estado restaurado, garantindo que nenhuma exclusão legítima foi revertida indevidamente.

### 7.3 Checklist estendido de validação (cenários de falha)
- [ ] Código de verificação (checagem de integridade) do backup conferido antes da restauração
- [ ] Comando de verificação do backup (SQL Server) ou checagem do arquivo JSON (Firebase) executado com sucesso, antes da restauração completa
- [ ] Teste de backup corrompido/incompleto executado de propósito ao menos uma vez por ciclo de homologação
- [ ] Registros com incompatibilidade de estrutura (campo novo, tipo alterado, regra de campo obrigatório nova) identificados e tratados
- [ ] Registros órfãos (FK inválida) verificados e reportados
- [ ] Teste de credenciais expiradas/revogadas realizado em homologação
- [ ] Tempo de restauração medido e comparado ao tempo máximo de indisponibilidade aceitável, definido para o projeto
- [ ] Reconciliação numérica (somatórios) validada, não apenas contagem de linhas
- [ ] Ausência de duplicidade pós-restauração confirmada
- [ ] Resultado de cada cenário documentado (data, cenário testado, sucesso/falha, ação corretiva)

### 7.4 Periodicidade dos testes de restauração
O fluxo básico de restauração deve ser repetido a cada backup semanal consolidado, garantindo que toda cópia guardada por mais tempo tenha passado por, pelo menos, uma checagem completa. Já os cenários de falha — backup corrompido, incompatibilidade de campos, falhas de ambiente e validações de consistência de negócio — são obrigatórios sempre que houver uma migração planejada, com prioridade máxima antes da substituição do Firebase/SQLite pelo SQL Server e antes de qualquer migração de dados de um cliente que já possua sistema próprio. Além disso, mesmo sem nenhuma migração agendada, esses cenários devem ser testados no mínimo uma vez por ciclo ou sprint de desenvolvimento, como forma de garantir que os procedimentos de contingência continuam funcionando quando forem realmente necessários.

---

## 8. Resumo

Os dados representam o principal ativo do ReGraphik. Toda alteração estrutural — novos campos, autenticação JWT, novos módulos ou mudança de provedor de banco — deve ser precedida de planejamento, backup validado e plano de rollback, testada primeiro em homologação e só então aplicada em produção. O código pode ser reconstruído a partir do repositório GitHub, mas os dados de resíduos, sugestões e usuários da empresa AML, uma vez perdidos, podem não ter recuperação possível. Proteger essas informações é a prioridade número um em qualquer evolução do sistema.
