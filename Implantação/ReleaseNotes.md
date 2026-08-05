# Notas da Versão — ReGraphik 1.0.0

**Sistema:** ReGraphik — Sistema de Gestão de Estoque Reverso  
**Versão:** 1.0.0
**Atualização:** agosto de 2026

Registro das funcionalidades previstas, melhorias, limitações conhecidas e pendências da primeira versão oficial do projeto acadêmico.

## Versão 1.0 

| Campo | Informação |
| --- | --- |
| Versão | 1.0.0 |
| Data prevista | Agosto de 2026 |
| Canal | Entrega acadêmica / primeira versão oficial do projeto |
| Compatibilidade | Windows 10/11 64 bits; .NET 8+; WebView2; internet. |
| Status | Candidata — depende da conclusão das validações listadas neste documento. |

## Funcionalidades previstas para a versão

- Autenticação e controle de acesso.

- Pré-cadastro e ativação por token, quando habilitados.

- Cadastro e consulta de resíduos.

- Gestão de status do estoque reverso.

- Sugestões de reaproveitamento por tipo de material.

- Busca de pontos de coleta por cidade.

- Dashboard e relatórios, quando aprovados no build final.

- Configuração local de foto de perfil.

- Operações administrativas compatíveis com o perfil.

## Correções e melhorias incluídas

- Estruturação em camadas e adoção do padrão MVVM no cliente WPF.

- Integração entre cliente WPF, API REST e Firebase.

- Uso de programação assíncrona para evitar bloqueios na interface.

- Mecanismo para evitar consultas externas repetidas de pontos já armazenados.

- Validação local de CPF e restrição de formatos de imagem.

- Documentação integrada de instalação, uso, treinamento e entrega.

## Limitações conhecidas

| Limitação | Consequência | Tratamento antes da entrega |
| --- | --- | --- |
| Endereço definitivo da API não informado | O cliente pode não se comunicar com o backend. | Definir, configurar e testar. |
| Contato oficial de suporte não informado | Usuário fica sem canal formal. | Definir e inserir no manual/licença. |
| Status dos módulos é divergente no TCC | Risco de anunciar função incompleta. | Validar módulo a módulo e ajustar estas notas. |
| Instalação em segundo computador ainda não comprovada neste documento | Critério de avaliação pendente. | Executar teste e anexar evidências. |
| Dependência de internet e serviços externos | Mapa, login ou dados podem ficar indisponíveis. | Exibir mensagens claras e aplicar contingência. |
| Chat com status documental inconsistente | Pode não estar disponível na versão 1.0. | Excluir da apresentação ou validar integralmente. |

## Próximas versões

- Concluir e estabilizar todos os módulos ainda não aprovados.

- Adicionar autenticação JWT na API REST.

- Ampliar testes unitários, integração e interface.

- Definir rotina formal de backup e restauração para ambiente de produção.

- Publicar ambiente definitivo de API e estabelecer monitoramento.

- Criar canal oficial de suporte e política de atualização.

- Avaliar assinatura digital do instalador.

---

