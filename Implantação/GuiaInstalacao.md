# Guia de Instalação — ReGraphik

**Sistema:** ReGraphik — Sistema de Gestão de Estoque Reverso  
**Versão:** 1.0.0 
**Atualização:** agosto de 2026

Este documento orienta a equipe técnica e a pessoa responsável pela instalação do ReGraphik em computadores Windows.

> Documento correspondente ao arquivo GuiaInstalacao.md do pacote Versao_1.0. Destina-se à equipe técnica ou à pessoa responsável pela instalação.

## Informações gerais

| Item | Valor |
| --- | --- |
| Nome do sistema | ReGraphik |
| Versão | 1.0 |
| Plataforma | Windows Desktop — WPF/.NET |
| Arquitetura | Cliente WPF → API REST ASP.NET Core → Firebase / Google Maps Places |
| Instalador previsto | Setup.exe, produzido preferencialmente com Inno Setup ou WiX Toolset. |
| Diretório recomendado | C:\Program Files\ReGraphik\ |
| Dados locais | Configuração de perfil em %AppData%\ReGraphik\config.txt, conforme documentação da solução. |

## Requisitos mínimos e recomendados

| Recurso | Mínimo definido | Recomendado para melhor experiência |
| --- | --- | --- |
| Sistema operacional | Windows 10 ou Windows 11, 64 bits. | Windows 11, 64 bits, atualizado. |
| Processador | Intel Core i3. | Intel Core i5 ou equivalente. |
| Memória RAM | 4 GB. | 8 GB ou mais. |
| Armazenamento | SSD de 512 GB no equipamento. | SSD de 512 GB ou superior, com espaço livre para instalação e atualizações. |
| Conectividade | Acesso à internet. | Conexão estável, sem bloqueio aos serviços utilizados. |
| Resolução | Não definida nos documentos-base. | 1366 × 768 ou superior, a validar pela equipe. |

## Dependências de software

| Dependência | Finalidade | Tratamento recomendado no instalador |
| --- | --- | --- |
| .NET 8 Runtime ou superior | Executar o cliente WPF e componentes .NET. | Detectar a versão; instalar automaticamente ou orientar o usuário. |
| Microsoft Edge WebView2 Runtime | Renderizar o mapa com Leaflet.js no aplicativo. | Verificar presença; instalar o Evergreen Runtime quando ausente. |
| Microsoft Visual C++ Redistributable | Suportar componentes nativos utilizados por dependências. | Incluir ou encadear a instalação da versão compatível. |
| Internet e HTTPS liberado | Comunicação com API, Firebase e Google Maps. | Validar firewall, proxy e resolução DNS antes do teste funcional. |

## Preparação antes da instalação

1. Confirmar que o computador atende aos requisitos de hardware e sistema operacional.

2. Entrar com uma conta que possua permissão administrativa ou ter credenciais de administrador disponíveis.

3. Fechar versões anteriores do ReGraphik e aplicações que possam bloquear arquivos.

4. Confirmar conexão com a internet e acesso aos serviços externos.

5. Verificar se o endereço definitivo da API já foi configurado no build de distribuição.

6. Realizar cópia de segurança dos dados locais/configurações existentes quando se tratar de atualização.

7. Desativar bloqueios somente quando estritamente necessário e reativá-los após a instalação; nunca desabilitar permanentemente o antivírus.

## Processo de instalação

1. Localize o arquivo Setup.exe dentro da pasta Versao_1.0.

2. Clique com o botão direito e selecione “Executar como administrador”, quando solicitado.

3. Confirme a janela de Controle de Conta de Usuário do Windows.

4. Leia as informações iniciais e o Termo de Licença Acadêmica.

5. Mantenha o diretório padrão C:\Program Files\ReGraphik\, salvo orientação técnica diferente.

6. Autorize a criação de atalho no Menu Iniciar e, opcionalmente, na Área de Trabalho.

7. Aguarde a verificação/instalação das dependências.

8. Conclua o assistente e mantenha marcada a opção de iniciar o ReGraphik, se disponível.

9. Registre no checklist a data, o computador, a versão e o responsável pela instalação.

| Instalação limpa<br>A versão acadêmica deve ser testada em um computador que não possua Visual Studio nem o código-fonte. O objetivo é comprovar que o pacote contém tudo o que o usuário precisa para executar o sistema. |
| --- |

## Configuração inicial

1. Abra o ReGraphik pelo atalho criado.

2. Verifique se a tela inicial ou tela de login é exibida sem mensagens de dependência ausente.

3. Confirme que o aplicativo consegue alcançar a API. Como o endereço definitivo ainda não foi informado, essa etapa permanece pendente até a configuração final.

4. Realize o primeiro acesso com credenciais de teste autorizadas ou execute o fluxo de pré-cadastro.

5. Acesse o mapa e confirme que o componente WebView2 renderiza o conteúdo.

6. Verifique se a foto de perfil/configuração local pode ser gravada no diretório de dados do usuário.

## Verificação pós-instalação

| Teste | Resultado esperado | Status |
| --- | --- | --- |
| Inicialização | Aplicativo abre sem erro e exibe a tela de acesso. | [ ] Aprovado  [ ] Reprovado |
| Autenticação | Usuário autorizado entra no sistema. | [ ] Aprovado  [ ] Reprovado |
| Comunicação com API | Dados são consultados sem erro de conexão. | [ ] Aprovado  [ ] Reprovado |
| Mapa | Mapa é exibido e busca por cidade retorna resultados. | [ ] Aprovado  [ ] Reprovado |
| Persistência | Cadastro de teste permanece disponível após reiniciar. | [ ] Aprovado  [ ] Reprovado |
| Relatório | Quando habilitado, arquivo/visualização é gerado corretamente. | [ ] Aprovado  [ ] Reprovado |
| Desinstalação | Sistema é removido pelo Windows sem deixar atalhos inválidos. | [ ] Aprovado  [ ] Reprovado |

## Atualização do sistema

1. Identificar a versão atual em “Sobre”, no arquivo ReleaseNotes ou no Painel de Aplicativos do Windows.

2. Ler as notas da nova versão e verificar requisitos adicionais.

3. Exportar ou proteger os dados/configurações locais antes da atualização.

4. Encerrar o ReGraphik em todos os usuários do computador.

5. Executar o novo Setup.exe como administrador.

6. Manter o diretório de instalação e concluir a atualização por substituição controlada.

7. Executar o checklist de testes rápidos e registrar a nova versão instalada.

8. Manter o instalador anterior disponível até a aprovação da atualização.

## Desinstalação

1. Fechar o ReGraphik.

2. Abrir Configurações do Windows → Aplicativos → Aplicativos instalados.

3. Localizar ReGraphik e selecionar Desinstalar.

4. Confirmar a remoção e aguardar a conclusão.

5. Validar se os atalhos e arquivos do programa foram removidos.

6. Preservar ou remover o conteúdo de %AppData%\ReGraphik somente conforme política de dados aprovada.

## Solução de problemas de instalação

| Sintoma | Causa provável | Ação recomendada |
| --- | --- | --- |
| O Setup.exe não inicia | Bloqueio do Windows/antivírus ou arquivo incompleto. | Confirmar origem, integridade do pacote e executar como administrador. |
| Mensagem sobre .NET | Runtime ausente ou incompatível. | Instalar .NET 8 Runtime x64 ou gerar publicação self-contained. |
| Mapa em branco | WebView2 ausente, internet bloqueada ou API indisponível. | Instalar WebView2, testar internet/firewall e validar endpoint. |
| Falha de login/conexão | Endereço da API incorreto ou serviço offline. | Confirmar configuração, DNS, HTTPS e disponibilidade do backend. |
| Acesso negado ao diretório | Permissão insuficiente. | Reinstalar com credenciais administrativas e evitar pastas protegidas personalizadas. |
| Aplicativo abre no computador do desenvolvedor, mas não no cliente | Dependência não incluída ou configuração específica do ambiente. | Revisar publicação, incluir dependências e repetir teste em máquina limpa. |

---

