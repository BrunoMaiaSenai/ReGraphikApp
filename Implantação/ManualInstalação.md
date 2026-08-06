# Manual de Instalação e Implantação — ReGraphik

Este documento apresenta as orientações oficiais para a instalação, atualização e execução da plataforma **ReGraphik** em ambientes Microsoft Windows.

---

## 1. Pré-requisitos de Sistema

Antes de iniciar o procedimento, certifique-se de que a estação de trabalho atende às especificações técnicas mínimas exigidas:

| Requisito | Especificação Mínima | Recomendado |
| :--- | :--- | :--- |
| **Sistema Operacional** | Windows 10 (64 bits) | Windows 11 (64 bits) |
| **Processador** | Intel Core i3 (2 GHz ou superior) | Intel Core i5 ou superior |
| **Memória RAM** | 4 GB | 8 GB ou superior |
| **Armazenamento** | 500 MB de espaço livre em disco | 1 GB de espaço livre (SSD) |
| **Privilégios** | Permissões de **Administrador Local** no sistema operacional |

---

## 2. Procedimento de Instalação

Siga os passos abaixo para realizar a implantação correta da aplicação:

### Passo 1: Execução do Assistente
1. Acesse o diretório onde o pacote de instalação `ReGraphik_Setup.exe` foi salvo.
2. Clique com o **botão direito do mouse** sobre o arquivo executável e selecione **"Executar como administrador"**.
3. Caso a tela de Controle de Conta de Usuário (UAC) do Windows seja exibida, confirme clicando em **Sim**.

### Passo 2: Gerenciamento de Versões (Upgrade Automático)
O assistente executará uma varredura preventiva no sistema operacional:
* **Detecção de versão legada:** Se uma versão anterior do ReGraphik for localizada, o assistente exibirá uma notificação solicitando autorização para a remoção prévia.
* **Ação recomendada:** Clique em **Sim**. A versão anterior será desinstalada com segurança para evitar conflitos de DLLs, mantendo o ambiente limpo para a nova versão.

### Passo 3: Finalização do Setup
1. Mantenha selecionada a opção para **"Criar atalho na Área de Trabalho"** para facilitar o acesso.
2. Clique em **Instalar** e aguarde a extração e registro dos componentes.
3. Ao concluir o progresso, clique em **Concluir**.

---

## 3. Inicialização e Acesso

Após a conclusão da instalação, a aplicação poderá ser inicializada através de duas rotas:
* **Atalho na Área de Trabalho:** Dê um duplo clique no ícone **ReGraphik**.
* **Menu Iniciar:** Pesquise por **ReGraphik** na barra de busca nativa do Windows.

---

## 4. Observações de Segurança (Filtro SmartScreen)

Em ambientes com políticas de segurança rigorosas, o Windows Defender SmartScreen poderá exibir um alerta temporário (*"O Windows protegeu o seu PC"*).

**Procedimento de liberação:**
1. Clique no link **"Mais informações"** exibido na janela do alerta.
2. Selecione o botão **"Executar assim mesmo"** para autorizar a inicialização do instalador assinado.

---

## 5. Suporte Técnico

Para dúvidas operacionais, auxílio no processo de implantação ou reporte de divergências, entre em contato com nossa equipe de atendimento:

* **E-mail de Suporte:** reghaphiktcc@gmail.com
* **Desenvolvido por:** Equipe ReGraphik

---
*© ReGraphik. Todos os direitos reservados. Proibida a reprodução ou distribuição não autorizada deste documento.*
