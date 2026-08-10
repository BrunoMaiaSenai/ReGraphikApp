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
- **Detecção de Versão Anterior:** O código em Pascal Script identifica instalações antigas no Registro e oferece a remoção automática silenciosa antes de gravar os novos arquivos.

### 3.2 Script de Compilação (instalador_innosetup.iss)

```Delphi
[Setup]
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

; --- CONFIGURAÇÕES DE ÍCONES E IMAGENS ---
UsePreviousAppDir=yes
DirExistsWarning=yes
SetupIconFile=logoRegraphik.ico
UninstallDisplayIcon={app}\App\logoRegraphik.ico

; Permite ao usuário escolher o idioma no início
ShowLanguageDialog=yes

; Imagens do assistente
WizardImageFile=wizardLogo.png
WizardSmallImageFile=logoRegraphik.png

LicenseFile=TermoDeUso.txt

; --- SUPORTE A IDIOMAS ---
[Languages]
Name: "brazilianportuguese"; MessagesFile: "compiler:Languages\BrazilianPortuguese.isl"
Name: "english"; MessagesFile: "compiler:Default.isl"

; --- TRADUÇÕES COMPLETAS DE TODAS AS MENSAGENS ---
[CustomMessages]
; Português
brazilianportuguese.AppRunningWarning=O ReGraphik está em execução no momento.%n%nSe você prosseguir, a aplicação será fechada automaticamente para concluir a operação.%n%nDeseja continuar?
brazilianportuguese.MaintenanceTitle=Opções de Manutenção
brazilianportuguese.MaintenanceSubTitle=Uma versão anterior do ReGraphik já está instalada no sistema.
brazilianportuguese.SelectAction=Selecione a ação que deseja realizar:
brazilianportuguese.OptionUpdate=Atualizar (Instalar versão mais recente)
brazilianportuguese.OptionReinstall=Restaurar / Reparar (Remove a versão atual e instala do zero)
brazilianportuguese.OptionUninstall=Desinstalar o ReGraphik do computador
brazilianportuguese.NewVersionNotice=Uma nova versão (%1) está disponível!%nVersão atual instalada: %2.%n%nDeseja atualizar agora?
brazilianportuguese.CompApp=Aplicação Principal ReGraphik
brazilianportuguese.CompAPI=Pacote de Serviços e API Rest
brazilianportuguese.AlreadyInstalled=A versão %1 do ReGraphik já está instalada no seu computador.%n%nNão há uma versão mais recente disponível para atualização.%nCaso queira corrigir arquivos corrompidos, marque a caixa "Restaurar / Reparar".

; Inglês
english.AppRunningWarning=ReGraphik is currently running.%n%nIf you proceed, the application will be closed automatically to complete the operation.%n%nDo you wish to continue?
english.MaintenanceTitle=Maintenance Options
english.MaintenanceSubTitle=A previous version of ReGraphik is already installed on your system.
english.SelectAction=Select the action you want to perform:
english.OptionUpdate=Update (Install the latest version)
english.OptionReinstall=Repair / Restore (Remove current version and perform a clean install)
english.OptionUninstall=Uninstall ReGraphik from this computer
english.NewVersionNotice=A new version (%1) is available!%nCurrent version installed: %2.%n%nDo you wish to update now?
english.CompApp=ReGraphik Main Application
english.CompAPI=REST API & Services Package
english.AlreadyInstalled=Version %1 of ReGraphik is already installed on your computer.%n%nThere is no newer version available for update.%nIf you want to repair corrupted files, check the "Repair / Restore" option.

[Components]
Name: "main"; Description: "{cm:CompApp}"; Types: full compact custom; Flags: fixed
Name: "api"; Description: "{cm:CompAPI}"; Types: full compact custom; Flags: fixed

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"

[Files]
Source: "ReGraphik\bin\Release\net8.0-windows\win-x64\publish\*"; DestDir: "{app}\App"; Components: main; Flags: ignoreversion recursesubdirs
Source: "ApiRestReGraphik\bin\Release\net8.0\win-x64\publish\*"; DestDir: "{app}\API"; Components: api; Flags: ignoreversion recursesubdirs
Source: "logoRegraphik.ico"; DestDir: "{app}\App"; Flags: ignoreversion

[Icons]
Name: "{group}\ReGraphik"; Filename: "{app}\App\ReGraphik.exe"; WorkingDir: "{app}\App"; IconFilename: "{app}\App\logoRegraphik.ico"
Name: "{autodesktop}\ReGraphik"; Filename: "{app}\App\ReGraphik.exe"; WorkingDir: "{app}\App"; IconFilename: "{app}\App\logoRegraphik.ico"; Tasks: desktopicon

[Code]
var
  MaintenancePage: TWizardPage;
  ChkUpdate, ChkReinstall, ChkUninstall: TCheckBox;
  IsOldVersionInstalled: Boolean;
  UninstPath: String;
  InstalledVersion: String;

// --- FUNÇÃO PARA COMPARAR VERSÕES ---
function CompareVersion(V1, V2: String): Integer;
var
  P1, P2, N1, N2: Integer;
  S1, S2: String;
begin
  Result := 0;
  S1 := V1;
  S2 := V2;

  while (Length(S1) > 0) or (Length(S2) > 0) do
  begin
    P1 := Pos('.', S1);
    if P1 > 0 then
    begin
      N1 := StrToIntDef(Copy(S1, 1, P1 - 1), 0);
      Delete(S1, 1, P1);
    end
    else
    begin
      N1 := StrToIntDef(S1, 0);
      S1 := '';
    end;

    P2 := Pos('.', S2);
    if P2 > 0 then
    begin
      N2 := StrToIntDef(Copy(S2, 1, P2 - 1), 0);
      Delete(S2, 1, P2);
    end
    else
    begin
      N2 := StrToIntDef(S2, 0);
      S2 := '';
    end;

    if N1 > N2 then
    begin
      Result := 1;
      Exit;
    end;
    if N1 < N2 then
    begin
      Result := -1;
      Exit;
    end;
  end;
end;

// --- VERIFICA E FECHA A APLICAÇÃO CASO ESTEJA ABERTA ---
function CheckAndCloseApp(): Boolean;
var
  ResultCode: Integer;
  UserChoice: Integer;
begin
  Result := True;
  Exec('cmd.exe', '/c tasklist /FI "IMAGENAME eq ReGraphik.exe" | find /I "ReGraphik.exe"', '', SW_HIDE, ewWaitUntilTerminated, ResultCode);

  if ResultCode = 0 then
  begin
    UserChoice := MsgBox(CustomMessage('AppRunningWarning'), mbConfirmation, MB_YESNO);
    if UserChoice = IDYES then
    begin
      Exec('taskkill.exe', '/F /IM ReGraphik.exe', '', SW_HIDE, ewWaitUntilTerminated, ResultCode);
      Result := True;
    end
    else
    begin
      Result := False;
    end;
  end;
end;

// --- INICIALIZAÇÃO DO ASSISTENTE ---
procedure InitializeWizard();
var
  lblSelect: TLabel;
begin
  // --- BUSCA DA VERSÃO INSTALADA ---
  if not RegQueryStringValue(HKLM, 'SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\{C918494A-DA28-4F94-A9B5-246200DBF17E}_is1', 'DisplayVersion', InstalledVersion) then
    RegQueryStringValue(HKCU, 'SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\{C918494A-DA28-4F94-A9B5-246200DBF17E}_is1', 'DisplayVersion', InstalledVersion);

  IsOldVersionInstalled := RegQueryStringValue(HKLM, 'SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\{C918494A-DA28-4F94-A9B5-246200DBF17E}_is1', 'UninstallString', UninstPath) or
                          RegQueryStringValue(HKCU, 'SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\{C918494A-DA28-4F94-A9B5-246200DBF17E}_is1', 'UninstallString', UninstPath);

  // --- CRIAÇÃO DA PÁGINA CUSTOMIZADA DE MANUTENÇÃO (TEXTOS MULTI-IDIOMA) ---
  if IsOldVersionInstalled then
  begin
    MaintenancePage := CreateCustomPage(wpWelcome, CustomMessage('MaintenanceTitle'), CustomMessage('MaintenanceSubTitle'));

    lblSelect := TLabel.Create(MaintenancePage);
    lblSelect.Caption := CustomMessage('SelectAction');
    lblSelect.Parent := MaintenancePage.Surface;
    lblSelect.Left := 0;
    lblSelect.Top := 10;
    lblSelect.Font.Style := [fsBold];

    ChkUpdate := TCheckBox.Create(MaintenancePage);
    ChkUpdate.Caption := CustomMessage('OptionUpdate');
    ChkUpdate.Parent := MaintenancePage.Surface;
    ChkUpdate.Left := 15;
    ChkUpdate.Top := 45;
    ChkUpdate.Width := 400;
    ChkUpdate.Checked := False;

    ChkReinstall := TCheckBox.Create(MaintenancePage);
    ChkReinstall.Caption := CustomMessage('OptionReinstall');
    ChkReinstall.Parent := MaintenancePage.Surface;
    ChkReinstall.Left := 15;
    ChkReinstall.Top := 80;
    ChkReinstall.Width := 400;
    ChkReinstall.Checked := False;

    ChkUninstall := TCheckBox.Create(MaintenancePage);
    ChkUninstall.Caption := CustomMessage('OptionUninstall');
    ChkUninstall.Parent := MaintenancePage.Surface;
    ChkUninstall.Left := 15;
    ChkUninstall.Top := 115;
    ChkUninstall.Width := 400;
    ChkUninstall.Checked := False;
  end;
end;

// --- AÇÕES AO CLICAR EM AVANÇAR ---
function NextButtonClick(CurPageID: Integer): Boolean;
var
  ResultCode: Integer;
  UserChoice: Integer;
  MsgText: String;
  CompResult: Integer;
begin
  Result := True;

  if (IsOldVersionInstalled) and (CurPageID = MaintenancePage.ID) then
  begin
    if (not ChkUpdate.Checked) and (not ChkReinstall.Checked) and (not ChkUninstall.Checked) then
    begin
      Exit;
    end;

    if ChkUpdate.Checked then
    begin
      if InstalledVersion <> '' then
      begin
        CompResult := CompareVersion('{#SetupSetting("AppVersion")}', InstalledVersion);

        if CompResult <= 0 then
        begin
          MsgBox(FmtMessage(CustomMessage('AlreadyInstalled'), [InstalledVersion]), mbInformation, MB_OK);
          Result := False;
          Exit;
        end;

        if CompResult > 0 then
        begin
          MsgText := FmtMessage(CustomMessage('NewVersionNotice'), ['{#SetupSetting("AppVersion")}', InstalledVersion]);
          UserChoice := MsgBox(MsgText, mbInformation, MB_YESNO);
          if UserChoice = IDNO then
          begin
            Result := False;
            Exit;
          end;
        end;
      end;
    end;

    if not CheckAndCloseApp() then
    begin
      Result := False;
      Exit;
    end;

    if ChkUninstall.Checked then
    begin
      UninstPath := RemoveQuotes(UninstPath);
      Exec(UninstPath, '/SILENT /NORESTART', '', SW_SHOW, ewWaitUntilTerminated, ResultCode);
      WizardForm.Close;
      Result := False;
      Exit;
    end;

    if ChkReinstall.Checked then
    begin
      UninstPath := RemoveQuotes(UninstPath);
      Exec(UninstPath, '/SILENT /NORESTART /SUPPRESSMSGBOXES', '', SW_HIDE, ewWaitUntilTerminated, ResultCode);
    end;
  end;

  if CurPageID = wpSelectDir then
  begin
    Result := CheckAndCloseApp();
  end;
end;
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

   <img width="1412" height="951" alt="image" src="https://github.com/user-attachments/assets/0462917c-0953-4183-b469-7af76d5c5159" />


---

5. **Vincular e Salvar na Solução**  
   Acesse o menu **File > Save As...** e salve o arquivo do script (`.iss`) diretamente no diretório raiz da solução do seu projeto.

   <img width="291" height="298" alt="image" src="https://github.com/user-attachments/assets/e3896dcb-a926-4fb7-91fa-50dd008ada85" />

   <img width="1225" height="682" alt="Seleção da pasta raiz do projeto no Windows Explorer" src="https://github.com/user-attachments/assets/c054f1ad-8e25-4b30-ae69-9900018301ae" />

---

#### Compilação e Localização do Instalador (`.exe`)

6. **Compilação do Script e Acesso ao Arquivo Gerado**  
   * No Inno Setup Compiler, pressione **F9** ou clique no menu superior **Build > Compile** para gerar o instalador.
   * Após a compilação, acesse a pasta de saída clicando no menu **Build > Open Output Folder** (ou pressione `Ctrl + F9`).
   * O executável final estará salvo no diretório especificado pelo script:
     ```text
     ReGraphik\Installer_InnoSetup\ReGraphik_Setup.exe
     ```
     <img width="775" height="63" alt="image" src="https://github.com/user-attachments/assets/c28b5376-e022-402d-a9be-2b1cb10f034c" />

   * **Este arquivo `ReGraphik_Setup.exe` é o único executável necessário para enviar e instalar a aplicação no cliente.**

---

## 4. Etapa 3: Criação do Instalador do Windows Installer (.msi) com WiX Toolset
### 4.1 Decisões de Implementação
O WiX Toolset (v3.11) foi utilizado para gerar o pacote corporativo de instalação no formato nativo da Microsoft (.msi).

- **Interface de Instalação Comercial (WixUI_InstallDir):** Utiliza o fluxo estendido da interface do WiX que permite ao usuário escolher o diretório de destino (C:\Program Files\ReGraphik), aceitar o contrato de licença (RTF) e visualizar a barra de progresso de forma limpa.

- **Fechamento Automático de Processos (util:CloseApplication):** Integrado via extensão WixUtilExtension. Durante a instalação/upgrade, o WiX notifica o sistema para encerrar o processo ReGraphik.exe caso ele esteja em execução, prevenindo erros de bloqueio de arquivo.

- **Geração Automática das Imagens do Wizard:** Utilizou-se automação via PowerShell para desenhar as artes Bitmap (wizardLogo.bmp e logoRegraphik.bmp). As imagens mantêm o fundo branco no painel direito, garantindo que os textos nativos do Windows Installer permaneçam em preto e com legibilidade ideal.

- **Codificação e Arquitetura x64:** Configurado com Platform="x64", InstallScope="perMachine" e codificação ANSI (Codepage="1252" / Language="1046" para Português-BR).

- **Estrutura de Referências no .wixproj:** O arquivo de projeto inclui explicitamente as DLLs WixUIExtension.dll e WixUtilExtension.dll dentro do MSBuild, permitindo compilar diretamente no Visual Studio sem erros de elementos não tratados.

### 4.2 Arquivo do Projeto Visual Studio (ReGraphikSetup.wixproj)
```xml
<?xml version="1.0" encoding="utf-8"?>
<Project ToolsVersion="4.0" DefaultTargets="Build" InitialTargets="EnsureWixToolsetInstalled" xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
  <PropertyGroup>
    <Configuration Condition=" '$(Configuration)' == '' ">Debug</Configuration>
    <Platform Condition=" '$(Platform)' == '' ">x86</Platform>
    <ProductVersion>3.10</ProductVersion>
    <ProjectGuid>e01c995a-90f4-4e25-8a3a-b5767f6dbf6f</ProjectGuid>
    <SchemaVersion>2.0</SchemaVersion>
    <OutputName>ReGraphikSetup</OutputName>
    <OutputType>Package</OutputType>
  </PropertyGroup>
  <PropertyGroup Condition=" '$(Configuration)|$(Platform)' == 'Debug|x86' ">
    <OutputPath>bin\$(Configuration)\</OutputPath>
    <IntermediateOutputPath>obj\$(Configuration)\</IntermediateOutputPath>
    <DefineConstants>Debug</DefineConstants>
  </PropertyGroup>
  <PropertyGroup Condition=" '$(Configuration)|$(Platform)' == 'Release|x86' ">
    <OutputPath>bin\$(Configuration)\</OutputPath>
    <IntermediateOutputPath>obj\$(Configuration)\</IntermediateOutputPath>
  </PropertyGroup>

  <ItemGroup>
    <Compile Include="Product.wxs" />
  </ItemGroup>
  <ItemGroup>
    <Content Include="logoRegraphik.ico" />
    <Content Include="wizardLogo.bmp" />
    <Content Include="logoRegraphik.bmp" />
    <Content Include="TermoDeUso.rtf" />
  </ItemGroup>

  <!-- REFERÊNCIAS DE EXTENSÕES WIX -->
  <ItemGroup>
    <WixExtension Include="WixUIExtension">
      <HintPath>$(WixExtDir)\WixUIExtension.dll</HintPath>
      <Name>WixUIExtension</Name>
    </WixExtension>
    <WixExtension Include="WixUtilExtension">
      <HintPath>$(WixExtDir)\WixUtilExtension.dll</HintPath>
      <Name>WixUtilExtension</Name>
    </WixExtension>
  </ItemGroup>

  <Import Project="$(WixTargetsPath)" Condition=" '$(WixTargetsPath)' != '' " />
  <Import Project="$(MSBuildExtensionsPath32)\Microsoft\WiX\v3.x\Wix.targets" Condition=" '$(WixTargetsPath)' == '' AND Exists('$(MSBuildExtensionsPath32)\Microsoft\WiX\v3.x\Wix.targets') " />
  <Target Name="EnsureWixToolsetInstalled" Condition=" '$(WixTargetsImported)' != 'true' ">
    <Error Text="The WiX Toolset v3.11 build tools must be installed to build this project." />
  </Target>
</Project>
```
### 4.3 Arquivo de Configuração XML (Product.wxs)
```xml
<?xml version="1.0" encoding="utf-8"?>
<Wix xmlns="http://schemas.microsoft.com/wix/2006/wi"
     xmlns:util="http://schemas.microsoft.com/wix/UtilExtension">

	<Product Id="*" 
			 Name="ReGraphik" 
			 Language="1046" 
			 Version="1.0.0.0" 
			 Manufacturer="Equipe ReGraphik" 
			 UpgradeCode="C918494A-DA28-4F94-A9B5-246200DBF17E">

		<Package InstallerVersion="200" 
				 Compressed="yes" 
				 InstallScope="perMachine" 
				 Platform="x64" 
				 SummaryCodepage="1252" />

		<!-- GERENCIAMENTO DE UPGRADES -->
		<MajorUpgrade
		  Schedule="afterInstallInitialize"
		  AllowSameVersionUpgrades="yes"
		  DowngradeErrorMessage="Uma versão mais recente do ReGraphik já está instalada no sistema." />

		<MediaTemplate EmbedCab="yes" />

		<!-- FECHAMENTO AUTOMÁTICO DO PROCESSO SE TIVER EM EXECUÇÃO -->
		<util:CloseApplication Id="CloseReGraphikApp" 
							   Target="ReGraphik.exe" 
							   CloseMessage="yes" 
							   RebootPrompt="no" />

		<!-- ÍCONE DO PAINEL DE CONTROLE -->
		<Icon Id="AppIcon.ico" SourceFile="logoRegraphik.ico" />
		<Property Id="ARPPRODUCTICON" Value="AppIcon.ico" />

		<!-- PERSONALIZAÇÃO VISUAL E LICENÇA -->
		<WixVariable Id="WixUILicenseRtf" Value="TermoDeUso.rtf" />
		<WixVariable Id="WixUIBannerBmp" Value="logoRegraphik.bmp" />
		<WixVariable Id="WixUIDialogBmp" Value="wizardLogo.bmp" />

		<!-- INTERFACE COM SELEÇÃO DE PASTA -->
		<UIRef Id="WixUI_InstallDir" />
		<UIRef Id="WixUI_ErrorProgressText" />
		<Property Id="WIXUI_INSTALLDIR" Value="INSTALLFOLDER" />

		<!-- RECURSOS DO SISTEMA -->
		<Feature Id="MainApplication" Title="Aplicação Principal ReGraphik" Level="1" Absent="disallow">
			<ComponentGroupRef Id="AppPublishComponents" />
			<ComponentGroupRef Id="ApiPublishComponents" />
			<ComponentRef Id="ApplicationShortcut" />
			<ComponentRef Id="DesktopShortcut" />
		</Feature>
	</Product>

	<!-- ESTRUTURA DE DIRETÓRIOS -->
	<Fragment>
		<Directory Id="TARGETDIR" Name="SourceDir">
			<Directory Id="ProgramFiles64Folder">
				<Directory Id="INSTALLFOLDER" Name="ReGraphik">
					<Directory Id="APPFOLDER" Name="App" />
					<Directory Id="APIFOLDER" Name="API" />
				</Directory>
			</Directory>
			<Directory Id="ProgramMenuFolder">
				<Directory Id="ApplicationProgramsFolder" Name="ReGraphik" />
			</Directory>
			<Directory Id="DesktopFolder" Name="Desktop" />
		</Directory>
	</Fragment>

	<!-- ATALHOS -->
	<Fragment>
		<DirectoryRef Id="ApplicationProgramsFolder">
			<Component Id="ApplicationShortcut" Guid="D4E5F6A7-B890-1234-CDEF-5678901234DE">
				<Shortcut Id="ApplicationStartMenuShortcut"
						  Name="ReGraphik"
						  Description="Sistema ReGraphik"
						  Target="[APPFOLDER]ReGraphik.exe"
						  WorkingDirectory="APPFOLDER"
						  Icon="AppIcon.ico" />
				<RemoveFolder Id="RemoveApplicationProgramsFolder" On="uninstall" />
				<RegistryValue Root="HKCU" Key="Software\ReGraphik" Name="installed" Type="integer" Value="1" KeyPath="yes" />
			</Component>
		</DirectoryRef>

		<DirectoryRef Id="DesktopFolder">
			<Component Id="DesktopShortcut" Guid="E5F6A7B8-9012-3456-CDEF-6789012345EF">
				<Shortcut Id="ApplicationDesktopShortcut"
						  Name="ReGraphik"
						  Description="Sistema ReGraphik"
						  Target="[APPFOLDER]ReGraphik.exe"
						  WorkingDirectory="APPFOLDER"
						  Icon="AppIcon.ico" />
				<RegistryValue Root="HKCU" Key="Software\ReGraphik" Name="desktopShortcut" Type="integer" Value="1" KeyPath="yes" />
			</Component>
		</DirectoryRef>
	</Fragment>
</Wix>
```

### 4.4 Guia de Configuração e Criação do Projeto WiX Toolset

Siga o passo a passo abaixo para instalar a extensão do WiX Toolset no Visual Studio, preparar o compilador e criar o pacote de instalação `.msi`:

#### Instalação da Extensão no Visual Studio

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

#### Download e Instalação do Compilador WiX

4. **Instalação do Engine do WiX Toolset v3.11**  
   Faça o download do executável oficial do compilador no GitHub através do link [wix311.exe](https://github.com/wixtoolset/wix3/releases/download/wix3112rtm/wix311.exe) e conclua a instalação em sua máquina.

   <img width="250" height="67" alt="Arquivo wix311.exe baixado" src="https://github.com/user-attachments/assets/b38fedf5-844e-4cea-9fe5-832bf276d5c0" />

   <img width="492" height="498" alt="Tela de instalação do WiX Toolset Build Tools" src="https://github.com/user-attachments/assets/e05a3d20-06c3-40b5-93c1-00036e7a37f4" />

---

#### Criação e Configuração do Projeto Setup

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
  	<img width="967" height="987" alt="image" src="https://github.com/user-attachments/assets/c036ab9b-d22c-4b3e-9af1-2fa1f1ad0c69" />

   
---


#### Compilação e Localização do Instalador (`.msi`)

7. **Compilação do Pacote e Acesso ao Arquivo Gerado**  
   * No Visual Studio, clique com o botão direito sobre o projeto WiX (`ReGraphikSetup`) no **Solution Explorer** e selecione **Build** (ou **Rebuild**).
   * Após a compilação bem-sucedida, clique novamente com o botão direito sobre o projeto WiX e selecione **Open Folder in File Explorer**.
   * O instalador gerado estará localizado no seguinte diretório:
     ```text
     ReGraphik\ReGraphikSetup\bin\Release\ReGraphikSetup.msi
     ou
     ReGraphik\ReGraphikSetup\ReGraphikSetup.msi
     ```
     
     <img width="795" height="97" alt="image" src="https://github.com/user-attachments/assets/a7123ebc-1dcb-4fbf-8edf-5ba2bcdc78ef" />

   * **Este arquivo `.msi` é o pacote final de instalação que deve ser disponibilizado ao cliente.**

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
