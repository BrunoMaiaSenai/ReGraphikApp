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
// Função para desinstalar versão anterior automaticamente antes de instalar a nova
function InitializeSetup(): Boolean;
var
  UninstPath: String;
  ResultCode: Integer;
begin
  Result := True;
  
  // Procura pelo desinstalador antigo nas chaves de registro do Windows
  if RegQueryStringValue(HKLM, 'SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\{C918494A-DA28-4F94-A9B5-246200DBF17E}_is1', 'UninstallString', UninstPath) or
     RegQueryStringValue(HKCU, 'SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\{C918494A-DA28-4F94-A9B5-246200DBF17E}_is1', 'UninstallString', UninstPath) then
  begin
    // Limpa as aspas se existirem
    UninstPath := RemoveQuotes(UninstPath);
    
    // Executa a desinstalação antiga em modo silencioso sem fechar o instalador novo
    Exec(UninstPath, '/SILENT /NORESTART /SUPPRESSMSGBOXES', '', SW_HIDE, ewWaitUntilTerminated, ResultCode);
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

### 3.3 Passos para Criação:

#### Como baixar e instalar
- **1. Acesse o site oficial:** ***jrsoftware.org/isinfo.php*** e aperte em Downloads Inno Setup

<img width="1412" height="496" alt="image" src="https://github.com/user-attachments/assets/f1afb04a-8bbf-44a7-95f2-194b68ae3aa7" />

---

- **2. Vá até a seção de Downloads e baixe a versão mais recente.**

<img width="1320" height="122" alt="image" src="https://github.com/user-attachments/assets/73a7637a-943d-495e-8681-1a92e21182df" />

---

- **3. Instale e abra o Inno Setup.**
  
<img width="1417" height="587" alt="image" src="https://github.com/user-attachments/assets/97a8aef5-9e2d-4122-aa09-9567aad49cdb" />

---

- **4. Adicione o script de compilação.**
  
<img width="1417" height="707" alt="image" src="https://github.com/user-attachments/assets/19b117b3-65d6-47b0-96d4-f553973f4e06" />

---

- **5. Salve na solução do seu projeto, apertando em file e save as**
  
  <img width="603" height="347" alt="image" src="https://github.com/user-attachments/assets/818bbfa3-5019-4396-ae69-69e017091740" />

  <img width="1225" height="682" alt="image" src="https://github.com/user-attachments/assets/c054f1ad-8e25-4b30-ae69-9900018301ae" />
  
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

### 4.3 Passos para Criação:

- **1. Abra o Visual Studio e entre em extensão (Extensions) e selecione manager extensions.**

<img width="951" height="76" alt="image" src="https://github.com/user-attachments/assets/67d2ef63-c8b5-4778-af9c-34ee4faaaa65" />


---

- **2. Instalação do compilador WiX Toolset v3.11 no Windows e da extensão WiX Toolset Extension no Visual Studio.**

<img width="1432" height="781" alt="Captura de tela 2026-08-04 155320" src="https://github.com/user-attachments/assets/ae7fbb63-b000-4a17-bbbe-d115c6d15f98" />

---

- **3. Abra o Wix e deixe instalar os recursos.**

<img width="545" height="408" alt="Captura de tela 2026-08-04 155348" src="https://github.com/user-attachments/assets/a571c39e-fc9b-4f41-af56-8f3ca80141d4" />

<img width="542" height="408" alt="Captura de tela 2026-08-04 155454" src="https://github.com/user-attachments/assets/652b0eee-81d6-4a81-8cdf-7289fa5f0698" />

---

- **4. Selecione a solução com o botão direito, vá em Add e crie um projeto wix.**

<img width="440" height="211" alt="Captura de tela 2026-08-04 155808" src="https://github.com/user-attachments/assets/ae546b08-ceb5-469d-8076-44583fccd8c2" />

<img width="752" height="747" alt="Captura de tela 2026-08-04 155827" src="https://github.com/user-attachments/assets/119cdfe4-9e66-40e0-ac83-95a56ae03f08" />

<img width="1298" height="238" alt="Captura de tela 2026-08-04 155851" src="https://github.com/user-attachments/assets/c13b6b31-e7e8-4db8-b820-6acd99db0c50" />

---

- **5. Baixe o aplicativo wix, utilizando o link https://github.com/wixtoolset/wix3/releases/download/wix3112rtm/wix311.exe e instale.**

<img width="250" height="67" alt="image" src="https://github.com/user-attachments/assets/b38fedf5-844e-4cea-9fe5-832bf276d5c0" />

<img width="492" height="498" alt="image" src="https://github.com/user-attachments/assets/e05a3d20-06c3-40b5-93c1-00036e7a37f4" />

---

- **6. Entre no Product.wxs do seu novo Setup e coloque os arquivos atualizados das configurações XML.**

**Modelo Padrão criado:**
<img width="1128" height="487" alt="Captura de tela 2026-08-04 160827" src="https://github.com/user-attachments/assets/c70b7b8e-5ea2-4da5-bb75-483d95c27a5b" />

**Modelo novo atualizado:**
<img width="1246" height="955" alt="image" src="https://github.com/user-attachments/assets/d3ab1107-b088-4523-8d43-a552f9fed84f" />

---

## 5. Matriz Comparativa e Resumo dos Artefatos

| Parâmetro / Requisito | Instalador Inno Setup | Instalador WiX Toolset |
| :--- | :--- | :--- |
| **Formato de Saída** | Executável (`.exe`) | Pacote do Windows Installer (`.msi`) |
| **Tipo de Instalação** | Script standalone | Nativa do sistema operacional (MSI) |
| **Público-Alvo** | Usuários Finais (B2C) | Ambientes Corporativos / TI (GPO / Active Directory) |
| **Taxa de Compressão** | Alta (LZMA2) | Média (Padrão CAB/MSI) |
| **Local do Artefato** | `\Installer_InnoSetup\ReGraphik_Setup.exe` | `\ReGraphikSetup\bin\Debug\ReGraphikSetup.msi` |
