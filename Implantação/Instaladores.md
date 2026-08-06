# Documentação de Criação dos Instaladores – Sistema ReGraphik 

**Projeto:** Sistema ReGraphik <br>
**Tecnologias:** .NET 8 (WPF e Web API REST), Inno Setup, WiX Toolset v3

Um instalador (ou setup) é um programa de computador criado especialmente para automatizar o processo de transferência, configuração e preparação de um software para que ele possa ser executado no sistema operacional do usuário.

## 1. Visão Geral da Arquitetura do Sistema
O sistema ReGraphik foi projetado seguindo uma arquitetura desacoplada, composta por dois componentes principais desenvolvidos na plataforma .NET 8:

- **ReGraphik (Frontend Desktop):** Aplicação de interface gráfica desenvolvida em WPF (Windows Presentation Foundation).

- **ApiRestReGraphik (Backend RESTful):** API responsável pelo processamento de dados, acesso ao banco de dados e regras de negócio.

---

## 2. Etapa 1: Publicação em Modo Release (dotnet publish)
### 2.1 Decisões de Engenharia
Para garantir que a aplicação rode em qualquer máquina de usuário final (mesmo naquelas que não possuem o ambiente de desenvolvimento .NET instalado), foram adotadas as seguintes diretivas de compilação:

1. **Configuração de Release (-c Release):** Otimiza o código-fonte, remove símbolos de depuração desnecessários e reduz o tamanho final do binário.

2. **Runtime Target (-r win-x64):** Define explicitamente a arquitetura de execução para Windows de 64 bits.

3. **Modo Self-Contained (--self-contained true):** Empacota todas as DLLs e o Runtime do .NET 8 junto com a aplicação. Isso elimina a dependência externa do .NET Runtime no computador do cliente.

4. **Single File (-p:PublishSingleFile=true):** Compacta múltiplos assemblies em um único executável principal para a interface WPF, simplificando a estrutura de arquivos e prevenindo exclusões acidentais de DLLs por parte do usuário.

### 2.2 Comandos Executados no Terminal
Os binários otimizados para produção foram gerados executando os seguintes comandos na raiz do projeto:

```bash
# Publicação do Aplicativo WPF (Frontend)
dotnet publish ReGraphik/ReGraphik.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true

# Publicação da API REST (Backend)
dotnet publish ApiRestReGraphik/ApiRestReGraphik.csproj -c Release -r win-x64 --self-contained true
```

Os artefatos compilados foram gerados nos seguintes diretórios relativos:

- **ReGraphik\bin\Release\net8.0-windows\win-x64\publish**

- **ApiRestReGraphik\bin\Release\net8.0\win-x64\publish**

---

## 3. Etapa 2: Criação do Instalador Executável (.exe) com Inno Setup
### 3.1 Decisões de Implementação
O Inno Setup foi selecionado para gerar o instalador executável padrão pela sua simplicidade, leveza e alta taxa de compressão.

- **Algoritmo de Compressão (LZMA2 / Solid):** Garante que o instalador final ocupe o menor espaço em disco possível, empacotando os runtimes do .NET de forma eficiente.

- **Isolamento de Diretórios:** O instalador organiza a aplicação em subpastas no diretório de destino **{autopf}\ReGraphik:**

    - **{app}\App:** Contém a aplicação desktop WPF.

    - **{app}\API:** Contém o serviço Web API.

- **Experiência do Usuário (UX):** Criação automática de atalhos no Menu Iniciar e caixa de seleção opcional para criação de atalho na Área de Trabalho.

### 3.2 Script de Compilação (instalador_innosetup.iss)

```Delphi
[Setup]
; ID Único (Uma chave externa para escapar o GUID)
AppId={{C918494A-DA28-4F94-A9B5-246200DBF17E}
AppName=ReGraphik
AppVersion=1.0.0
DefaultDirName={autopf}\ReGraphik
DefaultGroupName=ReGraphik
OutputDir=.\Installer_InnoSetup
OutputBaseFilename=ReGraphik_Setup
Compression=lzma2
SolidCompression=yes

; --- PERMISSÕES E ARQUITETURA ---
PrivilegesRequired=admin
ArchitecturesInstallIn64BitMode=x64

; --- CONFIGURAÇÕES DE ÍCONES (INSTALADOR E PAINEL DE CONTROLE) ---
UsePreviousAppDir=yes
DirExistsWarning=yes
SetupIconFile=logoRegraphik.ico

; --- DEFINE O ÍCONE NO "ADICIONAR OU REMOVER PROGRAMAS"
UninstallDisplayIcon={app}\App\logoRegraphik.ico

[Code]
// Função executada logo ao iniciar o instalador
function InitializeSetup(): Boolean;
var
  UninstPath: String;
  ResultCode: Integer;
  UserChoice: Integer;
begin
  Result := True;
  
  // Procura pelo desinstalador antigo no Registro do Windows
  if RegQueryStringValue(HKLM, 'SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\{C918494A-DA28-4F94-A9B5-246200DBF17E}_is1', 'UninstallString', UninstPath) or
     RegQueryStringValue(HKCU, 'SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\{C918494A-DA28-4F94-A9B5-246200DBF17E}_is1', 'UninstallString', UninstPath) then
  begin
    // Exibe a mensagem avisando o usuário antes de prosseguir
    UserChoice := MsgBox(
      'Uma versão anterior do ReGraphik foi detectada no sistema.' + #13#10 + #13#10 +
      'Deseja desinstalá-la automaticamente para prosseguir com a nova instalação?',
      mbConfirmation, MB_YESNO
    );
    
    if UserChoice = IDYES then
    begin
      // Limpa as aspas do caminho do desinstalador
      UninstPath := RemoveQuotes(UninstPath);
      
      // Executa a remoção silenciosa
      Exec(UninstPath, '/SILENT /NORESTART /SUPPRESSMSGBOXES', '', SW_HIDE, ewWaitUntilTerminated, ResultCode);
    end
    else
    begin
      // Cancela o instalador caso o usuário escolha NÃO
      Result := False;
    end;
  end;
end;

[Files]
; Publicação do App WPF (Arquivos compilados)
Source: "ReGraphik\bin\Release\net8.0-windows\win-x64\publish\*"; DestDir: "{app}\App"; Flags: ignoreversion recursesubdirs

; Publicação da API REST (Arquivos compilados)
Source: "ApiRestReGraphik\bin\Release\net8.0\win-x64\publish\*"; DestDir: "{app}\API"; Flags: ignoreversion recursesubdirs

; Copia o arquivo de ícone para dentro da pasta do App
Source: "logoRegraphik.ico"; DestDir: "{app}\App"; Flags: ignoreversion

[Icons]
Name: "{group}\ReGraphik"; Filename: "{app}\App\ReGraphik.exe"; WorkingDir: "{app}\App"; IconFilename: "{app}\App\logoRegraphik.ico"
Name: "{autodesktop}\ReGraphik"; Filename: "{app}\App\ReGraphik.exe"; WorkingDir: "{app}\App"; IconFilename: "{app}\App\logoRegraphik.ico"; Tasks: desktopicon

[Tasks]
Name: "desktopicon"; Description: "Criar atalho na Área de Trabalho"; GroupDescription: "Atalhos adicionais:"
```

### 3.3 Guia de Instalação e Configuração do Inno Setup

Siga os passos abaixo para preparar o ambiente e vincular o script de instalação à solução do projeto:

#### Download e Instalação da Ferramenta

1. **Acesso ao Portal Oficial**  
   Acesse o site oficial do Inno Setup em [jrsoftware.org/isinfo.php](https://jrsoftware.org/isinfo.php) e clique no link **Inno Setup Downloads**.

   <img width="1412" height="496" alt="Acesso ao site oficial do Inno Setup" src="https://github.com/user-attachments/assets/f1afb04a-8bbf-44a7-95f2-194b68ae3aa7" />

---

2. **Obtenção do Executável**  
   Navegue até a seção de downloads e faça o download da versão estável mais recente do instalador (`innosetup-x.x.x.exe`).

   <img width="1320" height="122" alt="Seção de download da versão estável" src="https://github.com/user-attachments/assets/73a7637a-943d-495e-8681-1a92e21182df" />

---

3. **Instalação e Execução**  
   Execute o arquivo baixado, siga o assistente padrão de instalação do Windows e abra o **Inno Setup Compiler**.

   <img width="1417" height="587" alt="Tela inicial do Inno Setup Compiler" src="https://github.com/user-attachments/assets/97a8aef5-9e2d-4122-aa09-9567aad49cdb" />

---

#### Configuração do Script no Projeto

4. **Inclusão do Script de Compilação**  
   Cole o script `.iss` configurado para o ReGraphik no editor do Inno Setup.

   <img width="1417" height="707" alt="Script de compilação inserido no editor" src="https://github.com/user-attachments/assets/19b117b3-65d6-47b0-96d4-f553973f4e06" />

---

5. **Vincular e Salvar na Solução**  
   Acesse o menu **File > Save As...** e salve o arquivo do script (`.iss`) diretamente no diretório raiz da solução do seu projeto.

   <img width="603" height="347" alt="Menu File Save As no Inno Setup" src="https://github.com/user-attachments/assets/818bbfa3-5019-4396-ae69-69e017091740" />

   <img width="1225" height="682" alt="Seleção da pasta raiz do projeto no Windows Explorer" src="https://github.com/user-attachments/assets/c054f1ad-8e25-4b30-ae69-9900018301ae" />

---

## 4. Etapa 3: Criação do Instalador do Windows Installer (.msi) com WiX Toolset
### 4.1 Decisões de Implementação
O WiX Toolset (v3.11) foi utilizado para gerar o pacote corporativo de instalação no formato nativo da Microsoft (.msi).

- **Integração Nativa na Solution:** Utilizou-se o projeto do tipo Setup Project (WiX) direto no Visual Studio, permitindo que a construção do instalador faça parte do fluxo de build da solução.

- **Compatibilidade de Codificação ANSI (Codepage="1252"):** O motor do Windows Installer exige estritamente a tabela ANSI para a tabela de resumo (Summary Information). Foi configurado Codepage="1252" para evitar erros de compilação sem afetar a localização em Português-BR.

- **Gerenciamento de GUIDs:** Utilização de identificadores únicos universais (GUIDs) para os componentes WpfExecutable e ApiExecutable, assegurando a rastreabilidade pelo registro do Windows e facilitando upgrades ou desinstalações automáticas.

### 4.2 Arquivo de Configuração XML (Product.wxs)
```xml
<?xml version="1.0" encoding="Windows-1252"?>
<Wix xmlns="http://schemas.microsoft.com/wix/2006/wi">
	<Product Id="*" Name="ReGraphik" Language="1046" Version="1.0.0.0" Manufacturer="Equipe ReGraphik" UpgradeCode="c918494a-da28-4f94-a9b5-246200dbf17e" Codepage="1252">
		<Package InstallerVersion="200" Compressed="yes" InstallScope="perMachine" SummaryCodepage="1252" />

		<!-- Impede reinstalação/downgrade de versões mais antigas -->
		<MajorUpgrade
			Schedule="afterInstallInitialize"
			AllowSameVersionUpgrades="yes"
			DowngradeErrorMessage="Uma versão mais recente do [ProductName] já está instalada." />
		<MediaTemplate EmbedCab="yes" />

		<!-- ÍCONE DO PAINEL DE CONTROLE ("Adicionar ou Remover Programas") -->
		<Icon Id="AppIcon.ico" SourceFile="logoRegraphik.ico" />
		<Property Id="ARPPRODUCTICON" Value="AppIcon.ico" />

		<Feature Id="ProductFeature" Title="ReGraphik" Level="1">
			<ComponentGroupRef Id="ProductComponents" />
			<ComponentRef Id="ApplicationShortcut" />
			<ComponentRef Id="DesktopShortcut" />
		</Feature>
	</Product>

	<Fragment>
		<Directory Id="TARGETDIR" Name="SourceDir">
			<!-- Pasta Arquivos de Programas -->
			<Directory Id="ProgramFilesFolder">
				<Directory Id="INSTALLFOLDER" Name="ReGraphik">
					<Directory Id="APPFOLDER" Name="App" />
					<Directory Id="APIFOLDER" Name="API" />
				</Directory>
			</Directory>

			<!-- Pasta do Menu Iniciar -->
			<Directory Id="ProgramMenuFolder">
				<Directory Id="ApplicationProgramsFolder" Name="ReGraphik" />
			</Directory>

			<!-- Área de Trabalho -->
			<Directory Id="DesktopFolder" Name="Desktop" />
		</Directory>
	</Fragment>

	<Fragment>
		<ComponentGroup Id="ProductComponents">
			<!-- Executável da Aplicação WPF -->
			<Component Id="WpfExecutable" Directory="APPFOLDER" Guid="b2c3d4e5-f6a7-8901-bcde-2345678901bc">
				<File Id="ReGraphikEXE" Source="..\ReGraphik\bin\Release\net8.0-windows\win-x64\publish\ReGraphik.exe" KeyPath="yes" />
			</Component>

			<!-- Executável da API REST -->
			<Component Id="ApiExecutable" Directory="APIFOLDER" Guid="c3d4e5f6-a7b8-9012-cdef-3456789012cd">
				<File Id="ApiRestReGraphikEXE" Source="..\ApiRestReGraphik\bin\Release\net8.0\win-x64\publish\ApiRestReGraphik.exe" KeyPath="yes" />
			</Component>
		</ComponentGroup>

		<!-- ATALHO NO MENU INICIAR -->
		<DirectoryRef Id="ApplicationProgramsFolder">
			<Component Id="ApplicationShortcut" Guid="d4e5f6a7-b890-1234-cdef-5678901234de">
				<!-- Usar [#ReGraphikEXE] garante que o Windows ache o arquivo e o ícone sem erros -->
				<Shortcut Id="ApplicationStartMenuShortcut"
						  Name="ReGraphik"
						  Description="Sistema ReGraphik"
						  Target="[#ReGraphikEXE]"
						  WorkingDirectory="APPFOLDER"
						  Icon="AppIcon.ico" />
				<RemoveFolder Id="RemoveApplicationProgramsFolder" On="uninstall" />
				<RegistryValue Root="HKCU" Key="Software\ReGraphik" Name="installed" Type="integer" Value="1" KeyPath="yes" />
			</Component>
		</DirectoryRef>

		<!-- ATALHO NA ÁREA DE TRABALHO -->
		<DirectoryRef Id="DesktopFolder">
			<Component Id="DesktopShortcut" Guid="e5f6a7b8-9012-3456-cdef-6789012345ef">
				<Shortcut Id="ApplicationDesktopShortcut"
						  Name="ReGraphik"
						  Description="Sistema ReGraphik"
						  Target="[#ReGraphikEXE]"
						  WorkingDirectory="APPFOLDER"
						  Icon="AppIcon.ico" />
				<RegistryValue Root="HKCU" Key="Software\ReGraphik" Name="desktopShortcut" Type="integer" Value="1" KeyPath="yes" />
			</Component>
		</DirectoryRef>
	</Fragment>
</Wix>
```

### 4.3 Guia de Configuração e Criação do Projeto WiX Toolset

Siga o passo a passo abaixo para instalar a extensão do WiX Toolset no Visual Studio, preparar o compilador e criar o pacote de instalação `.msi`:

#### 1. Instalação da Extensão no Visual Studio

1. **Acesso ao Gerenciador de Extensões**  
   Abra o Visual Studio, acesse o menu superior em **Extensions** (Extensões) e clique em **Manage Extensions** (Gerenciar Extensões).

   <img width="951" height="76" alt="Acesso ao menu Extensions no Visual Studio" src="https://github.com/user-attachments/assets/67d2ef63-c8b5-4778-af9c-34ee4faaaa65" />

---

2. **Instalação da Extensão WiX**  
   Na barra de pesquisa, busque por **WiX Toolset Extension**, selecione a extensão correspondente e clique em **Download/Install**.

   <img width="1432" height="781" alt="Busca e instalação da extensão WiX Toolset Extension" src="https://github.com/user-attachments/assets/ae7fbb63-b000-4a17-bbbe-d115c6d15f98" />

---

3. **Conclusão do VSIX Installer**  
   Feche o Visual Studio para permitir que o instalador **VSIX** aplique as modificações necessárias e instale os recursos da extensão.

   <img width="545" height="408" alt="Assistente de instalação do VSIX em andamento" src="https://github.com/user-attachments/assets/a571c39e-fc9b-4f41-af56-8f3ca80141d4" />

   <img width="542" height="408" alt="Conclusão da instalação da extensão no VSIX Installer" src="https://github.com/user-attachments/assets/652b0eee-81d6-4a81-8cdf-7289fa5f0698" />

---

#### 2. Download e Instalação do Compilador WiX

4. **Instalação do Engine do WiX Toolset v3.11**  
   Faça o download do executável oficial do compilador no GitHub através do link [wix311.exe](https://github.com/wixtoolset/wix3/releases/download/wix3112rtm/wix311.exe) e conclua a instalação em sua máquina.

   <img width="250" height="67" alt="Arquivo wix311.exe baixado" src="https://github.com/user-attachments/assets/b38fedf5-844e-4cea-9fe5-832bf276d5c0" />

   <img width="492" height="498" alt="Tela de instalação do WiX Toolset Build Tools" src="https://github.com/user-attachments/assets/e05a3d20-06c3-40b5-93c1-00036e7a37f4" />

---

#### 3. Criação e Configuração do Projeto Setup

5. **Inclusão do Projeto na Solução**  
   No **Solution Explorer**, clique com o botão direito sobre a **Solução**, navegue até **Add > New Project...** e selecione o modelo **WPF / Windows Setup Project** (WiX).

   <img width="440" height="211" alt="Menu de contexto Add New Project na Solução" src="https://github.com/user-attachments/assets/ae546b08-ceb5-469d-8076-44583fccd8c2" />

   <img width="752" height="747" alt="Janela de seleção do modelo de projeto WiX" src="https://github.com/user-attachments/assets/119cdfe4-9e66-40e0-ac83-95a56ae03f08" />

   <img width="1298" height="238" alt="Projeto WiX adicionado à arvore de arquivos da Solução" src="https://github.com/user-attachments/assets/c13b6b31-e7e8-4db8-b820-6acd99db0c50" />

---

6. **Edição do Arquivo `Product.wxs`**  
   Abra o arquivo `Product.wxs` gerado automaticamente pelo template e substitua o código padrão pelas configurações XML atualizadas do ReGraphik.

   * **Modelo Padrão (Gerado pelo Template):**
   <img width="1128" height="487" alt="Código padrão do arquivo Product.wxs" src="https://github.com/user-attachments/assets/c70b7b8e-5ea2-4da5-bb75-483d95c27a5b" />

   * **Modelo Atualizado (Customizado para o ReGraphik):**
   <img width="1246" height="955" alt="Código XML atualizado com as regras do ReGraphik" src="https://github.com/user-attachments/assets/d3ab1107-b088-4523-8d43-a552f9fed84f" />
   
---

## 5. Matriz Comparativa e Resumo dos Artefatos

| Parâmetro / Requisito | Instalador Inno Setup | Instalador WiX Toolset |
| :--- | :--- | :--- |
| **Formato de Saída** | Executável (`.exe`) | Pacote do Windows Installer (`.msi`) |
| **Tipo de Instalação** | Script standalone | Nativa do sistema operacional (MSI) |
| **Público-Alvo** | Usuários Finais (B2C) | Ambientes Corporativos / TI (GPO / Active Directory) |
| **Taxa de Compressão** | Alta (LZMA2) | Média (Padrão CAB/MSI) |
| **Local do Artefato** | `\Installer_InnoSetup\ReGraphik_Setup.exe` | `\ReGraphikSetup\bin\Debug\ReGraphikSetup.msi` |

---

## 6. Adocão do Inno Setup no Sistema ReGraphik
A equipe ReGraphik selecionou o **Inno Setup** como o motor oficial de implantação e empacotamento da solução. Essa decisão foi pautada na busca por um instalador autônomo, resiliente, de alta performance e com baixa complexidade de manutenção contínua, garantindo que o ciclo de vida de atualização no ambiente do cliente final ocorra de forma transparente e sem fricção. 
A escolha do Inno Setup para o ecossistema ReGraphik equilibra robustez técnica e facilidade operacional. A ferramenta nos proporciona controle absoluto sobre a sequência de instalação e desinstalação via script, reduz potenciais chamados de suporte técnico relacionados a ambientes corrompidos e garante uma experiência profissional, segura e rápida para o usuário final.

### 6.1. Arquitetura de Scripts Resiliente e Baixa Complexidade de Manutenção
Sintaxe Declarativa e Limpa: Enquanto alternativas como o WiX Toolset demandam esquemas XML extensos e suscetíveis a erros de validação estrutural, o Inno Setup utiliza sintaxe de configuração baseada em seções estilo INI/Pascal.

Redução de Debito Técnico: A simplicidade do código reduz drasticamente o tempo de integração de novas versões, facilitando a onboarding de novos desenvolvedores na equipe de empacotamento.

### 6.2. Orquestração Avançada com Pascal Script ([Code])
- **Gestão de Ciclo de Vida do Aplicativo:** Através de ganchos (event hooks) nativos como o InitializeSetup(), o instalador consulta dinamicamente o Registro do Windows para detectar qualquer versão preexistente do ReGraphik.

- **Experiência de Atualização Transparente (Zero-Touch):** O instalador notifica o usuário, solicita a confirmação e aciona a desinstalação prévia de forma totalmente silenciosa antes da aplicação do novo binário, eliminando arquivos órfãos ou misturas de DLLs legadas.

### 6.3. Autonomia de Execução e Alta Compressão Binária
- **Empacotamento Autônomo (Single .EXE):** Diferente dos pacotes .msi, que são estritamente dependentes do serviço do Windows Installer (sujeito a bloqueios de política de grupo ou corrupção de banco de dados do sistema), o Inno Setup gera um binário autossuficiente (ReGraphik_Setup.exe).

- **Algoritmo LZMA2 Solid:** Oferece taxas de compressão superiores, reduzindo o volume do pacote de distribuição. Isso agiliza o download para os clientes e reduz os custos de tráfego de rede/servidor da empresa.

### 6.4. Integração Profunda com o Ecossistema Windows
- **Prevenção de Erros em Runtime:** Configuração direta de diretórios de execução (WorkingDir), garantindo que o executável WPF e a API REST embutida localizem suas dependências sem falhas de inicialização.

- **Conformidade de Interface:** Registro completo das chaves UninstallDisplayIcon e UninstallString, garantindo que a marca e o ícone do ReGraphik fiquem integrados ao painel nativo do Windows (Adicionar ou Remover Programas).

### 6.5. Eficiência Operacional e Custo Total de Propriedade (TCO)
- **Licenciamento Comercial Gratuito:** Solução open-source consolidada no mercado há décadas, eliminando custos de licenciamento de ferramentas de deploy proprietárias.

- **Compatibilidade Extensa:** Roda de forma consistente em sistemas operacionais x64 sem exigência de pré-requisitos complexos do sistema operacional hospedeiro.
