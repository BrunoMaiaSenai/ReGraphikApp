# Infraestrutura do Sistema — ReGraphik

## Informações do Projeto

| Campo | Descrição |
|---|---|
| **Nome do Projeto** | ReGraphik — Plataforma de Gestão de Estoque Reverso |
| **Instituição** | SENAI Afonso Greco – Nova Lima  |
| **Versão** | `1.0.0` |
| **Repositorio** | https://github.com/BrunoMaiaSenai/ReGraphikApp/tree/master|
| **Domínio da Aplicação** | Gestão de resíduos reaproveitáveis (papel, cartão, vinil) no setor gráfico, com sugestões de economia circular e localização de pontos de coleta |
| **Componentes do Sistema** | Cliente Desktop (WPF), API REST (ASP.NET Core), Banco de Dados em nuvem (Firebase Realtime Database) |
| **Integrantes** | Lucas Aquino Guedes, Bruno Maia Santos, Otavio Henrique Barbosa Soares, Luna Beatriz Alves, Kaio Alves Gonzaga Silva |

---

## Requisitos de Hardware

O sistema é composto por um cliente desktop (WPF) e uma API REST. Os requisitos abaixo seguem a lógica de mínimo/recomendado apresentada em aula, adaptados à natureza da aplicação (não há uso de IA local, mas há renderização de mapa via WebView2, geração de PDF e gráficos no cliente).

### Cliente Desktop (ReGraphik – WPF)

| Recurso | Mínimo | Recomendado |
|---|---|---|
| Processador | Intel Core i3-8100 / AMD Ryzen 3 2200G (ou equivalente) | Intel Core i5-10400 / AMD Ryzen 5 5500 (ou equivalente) |
| Memória RAM | 4 GB | 8 GB |
| Armazenamento | 2 GB livres | 5 GB livres (SSD) |
| Internet | 5 Mbps | 20 Mbps |
| Vídeo | Intel UHD Graphics 620 (integrada, DirectX 11) | Intel Iris Xe Graphics / AMD Radeon Vega 8 (integrada) |

### Servidor da API (ApiRestReGraphik)

| Recurso | Mínimo | Recomendado |
|---|---|---|
| Processador | Intel Xeon E-2224 / AMD EPYC 3251 (ou equivalente, 4 núcleos) | Intel Xeon E-2378 / AMD EPYC 7302P (ou equivalente, 8 núcleos) |
| Memória RAM | 2 GB | 4 GB |
| Armazenamento | 1 GB livre | 3 GB livres (SSD) |
| Internet | 10 Mbps (upload estável) | 50 Mbps (upload estável) |

> Observação: a API está publicada em produção no plano gratuito do runasp.net (`webregraphik.runasp.net`), o que implica um período de "aquecimento" (cold start) após inatividade — item relevante para o plano de contingência.

---

## Requisitos de Software

### Cliente Desktop
- Sistema Operacional: **Windows 10 ou Windows 11**
- **.NET 8 Desktop Runtime** (ou SDK completo em ambiente de desenvolvimento)
- **Microsoft Edge WebView2 Runtime** (necessário para o mapa interativo com Leaflet.js)
- Permissões de usuário padrão (não requer administrador para execução, apenas para instalação)

### API REST
- Sistema Operacional: **Windows Server**, **Windows 10/11** ou **Linux** (ASP.NET Core é multiplataforma)
- **.NET 8 Runtime (ASP.NET Core Runtime)**
- Acesso de saída à internet (para comunicação com Firebase e Google Maps API)

### Ferramentas de desenvolvimento (ambiente do desenvolvedor)
- Visual Studio 2022 ou JetBrains Rider
- Git

---

## Dependências

### Cliente Desktop — ReGraphik (WPF)

| Dependência | Função |
|---|---|
| .NET 8 Runtime (net8.0-windows) | Executa a aplicação WPF |
| CommunityToolkit.Mvvm (8.4.2) | Suporte ao padrão MVVM |
| FirebaseDatabase.Net (4.2.0) | Acesso direto ao Firebase (chat em tempo real) |
| Imgur.API (5.0.0) | Upload de foto de perfil |
| MahApps.Metro.IconPacks.Material (6.2.1) | Ícones de interface |
| MahApps.Metro.SimpleChildWindow (2.2.1) | Modais/janelas filhas |
| Microsoft.Web.WebView2 (1.0.2903.40) | Renderização do mapa interativo (Leaflet.js) — requer o **WebView2 Runtime** instalado no computador do cliente |
| OxyPlot.Wpf (2.2.0) | Gráficos do módulo ESG/Dashboard |
| QuestPDF (2026.5.0) | Geração de relatórios em PDF |
| System.Net.Http.Json (incluso no .NET 8) | Suporte a chamadas HTTP para a API |

### API REST — ApiRestReGraphik (ASP.NET Core)

| Dependência | Função |
|---|---|
| .NET 8 Runtime (net8.0) | Executa a API |
| FirebaseAdmin (3.5.0) | Autenticação e acesso administrativo ao Firebase |
| FirebaseDatabase.net (5.0.0) | Acesso ao Firebase Realtime Database |
| Swashbuckle.AspNetCore (6.6.2) | Geração da documentação Swagger/OpenAPI |
| Arquivo de credencial `ReGraphikFirebaseKey.json` (Service Account) | Autenticação da API com o Firebase — **arquivo sensível, não deve ser versionado publicamente** |
| Chave de API do Google Maps (Places API) | Busca de pontos de coleta |
| Chave de API do ImgBB | Upload/hospedagem de imagens de resíduos |

### Serviços externos (nuvem)
- **Firebase Realtime Database** — banco de dados principal do sistema
- **Google Maps Places API** — localização de pontos de coleta
- **Imgur API / ImgBB API** — hospedagem de imagens (foto de perfil e fotos de resíduos)

Uma das principais causas de falha em implantações é justamente a ausência dessas dependências — por isso todo o checklist abaixo deve ser validado antes do Go Live.

---

## Arquitetura do Sistema

```
┌─────────────────────────────────────────────────────┐
│              Cliente Desktop (WPF)                   │
│  Views (XAML) ↔ ViewModels (C#) ↔ Services (C#)      │
└───────────────────┬───────────────────────────────────┘
                     │  HTTP/REST (JSON)
                     ▼
┌─────────────────────────────────────────────────────┐
│            API REST (ASP.NET Core .NET 8)             │
│   Controllers → Services → Firebase Realtime DB       │
└───────┬──────────────────────────────┬────────────────┘
        │                              │
        ▼                              ▼
┌───────────────┐            ┌──────────────────────┐
│ Firebase       │            │  Google Maps          │
│ Realtime DB    │            │  Places API            │
└───────────────┘            └──────────────────────┘
```

**Fluxo da informação:** Usuário → Cliente WPF → API REST → Firebase Realtime Database (e serviços externos, quando aplicável) → retorno ao Cliente WPF.

| Camada | Responsabilidade |
|---|---|
| Usuário | Interage com o sistema através da interface gráfica (WPF) |
| Cliente WPF | Interface gráfica, validação de dados e consumo da API (padrão MVVM) |
| API REST | Regras de negócio, autenticação, validação e acesso ao Firebase |
| Firebase Realtime DB | Armazena permanentemente os dados do sistema (usuários, resíduos, sugestões, mensagens de chat, pontos de coleta) |
| Google Maps / Imgur / ImgBB | Serviços externos consumidos pela API |

**Por que usar uma API entre o cliente e o banco?**
- Centraliza as regras de negócio em um único local.
- Evita que o Firebase seja acessado diretamente pelo cliente, aumentando a segurança.
- Facilita manutenção e futura integração com outros clientes (web, mobile).

---

## Riscos Identificados

| Risco | Causa | Impacto |
|---|---|---|
| Ambiente do cliente sem .NET 8 Desktop Runtime | Máquina do cliente sem os pré-requisitos instalados | Aplicação não inicia |
| Ausência do WebView2 Runtime | Componente não vem instalado por padrão em todas as versões do Windows | Falha no módulo de mapa interativo |
| "Cold start" da API em produção (runasp.net) | Plano gratuito de hospedagem hiberna a API após inatividade | Lentidão/timeout na primeira requisição |
| Exposição da credencial do Firebase (`ReGraphikFirebaseKey.json`) | Arquivo sensível versionado ou distribuído incorretamente | Acesso não autorizado ao banco de dados |
| CORS liberado para qualquer origem (`AllowAnyOrigin`) | Configuração atual da API (`PermitirTudo`) | Superfície de ataque maior; recomenda-se restringir em produção |
| Indisponibilidade de internet no ambiente do cliente | Firewall restritivo ou conexão instável | Sistema totalmente dependente de rede (Firebase e API são em nuvem); sem internet a aplicação não funciona |
| Chave de API do Google Maps exposta em `appsettings.json` | Chave versionada no repositório | Uso indevido/consumo de cota por terceiros |
| Falha ou indisponibilidade do Firebase | Serviço de terceiros fora do controle da equipe | Sistema inteiro fica indisponível (não há banco local de contingência) |
| Antivírus bloqueando o instalador do cliente WPF | Aplicação recém-desenvolvida, sem assinatura digital | Bloqueio da instalação/execução no ambiente do cliente |
| Falha em implantação de nova versão da API | Ausência de processo de rollback documentado para a API | Indisponibilidade prolongada até correção manual |
| Perda de dados por falta de rotina de backup | Backup do Firebase realizado sem periodicidade ou processo definido | Impossibilidade de restaurar dados em caso de corrupção/exclusão acidental |

---

## Plano de Contingência

| Problema | Solução |
|---|---|
| API em cold start / indisponível | Aguardar aquecimento automático (runasp.net); mitigar com *health-check* periódico (ping automatizado a cada poucos minutos) para manter a instância ativa; migrar para hospedagem paga com always-on caso o problema persista |
| Firebase indisponível | Monitorar status oficial do Firebase; comunicar usuários; não há fallback local nesta versão — ação futura recomendada |
| Falha na atualização do cliente WPF | Manter versão anterior do instalador disponível para rollback manual |
| Falha na implantação de nova versão da API | Manter o build anterior disponível para redeploy imediato; validar em ambiente de homologação antes do Go Live |
| Perda de credenciais do Firebase | Revogar a chave comprometida no Console do Firebase e gerar novo Service Account |
| Antivírus bloqueando a aplicação | Testar previamente em ambiente semelhante ao do cliente; assinar digitalmente o executável quando possível |
| Queda de internet no ambiente do cliente | Orientar o cliente sobre a dependência de conectividade; não há modo offline nesta versão |
| Google Maps API indisponível/cota excedida | Módulo de mapa degrada graciosamente (demais módulos do sistema continuam funcionando) |
| Perda ou corrupção de dados no Firebase | Realizar exportação periódica do Realtime Database (ex.: semanal) e armazenar em local seguro fora do Firebase, com processo documentado de restauração |

---

## Checklist de Implantação

### Hardware
- [ ] Processador compatível (mínimo Dual Core no cliente / 2 vCPUs na API)
- [ ] Memória RAM suficiente (mínimo 4 GB no cliente / 2 GB na API)
- [ ] Espaço em disco disponível

### Sistema
- [ ] Windows 10/11 atualizado no cliente
- [ ] .NET 8 Runtime instalado (Desktop Runtime no cliente / ASP.NET Core Runtime no servidor)
- [ ] Microsoft Edge WebView2 Runtime instalado no cliente

### Banco de Dados / Serviços em Nuvem
- [ ] Projeto Firebase criado e Realtime Database configurado
- [ ] Arquivo de credencial (`ReGraphikFirebaseKey.json`) disponível de forma segura na API
- [ ] Regras de acesso (Rules) do Firebase revisadas
- [ ] Backup/exportação do Firebase Realtime Database realizado, com periodicidade definida e processo de restauração testado

### Segurança
- [ ] Firewall liberando as portas usadas pela API e por HTTPS
- [ ] Política de CORS revisada para produção (hoje liberada para qualquer origem)
- [ ] Chaves de API (Google Maps, Imgur, ImgBB) protegidas fora do controle de versão
- [ ] Antivírus do ambiente do cliente validado com o instalador da aplicação

### Rede
- [ ] Internet funcionando no ambiente do cliente e do servidor
- [ ] API acessível via HTTPS (`webregraphik.runasp.net` ou ambiente equivalente)
- [ ] Conectividade com Firebase, Google Maps API, Imgur/ImgBB validada
- [ ] DNS configurado (caso hospedagem própria seja utilizada)

### Aplicação
- [ ] `appsettings.json` da API configurado com URLs e chaves corretas do ambiente de destino
- [ ] Swagger acessível para validação dos endpoints (`/`)
- [ ] Cliente WPF testado em máquina semelhante à do usuário final antes do Go Live
- [ ] Build anterior da API mantido disponível para rollback rápido em caso de falha na nova versão
