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

; --- PERSONALIZAÇÃO VISUAL DO ASSISTENTE (SUBSTITUI A CAIXA E O CD) ---
WizardImageFile=wizardLogo.bmp
WizardSmallImageFile=logoRegraphik.bmp

; --- CONFIGURAÇÃO DE TERMOS DE USO (LICENÇA) ---
LicenseFile=TermoDeUso.txt

; --- SUPORTE A IDIOMAS ---
[Languages]
Name: "brazilianportuguese"; MessagesFile: "compiler:Languages\BrazilianPortuguese.isl"
Name: "english"; MessagesFile: "compiler:Default.isl"

; --- TEXTOS CUSTOMIZADOS MULTI-IDIOMA ---
[CustomMessages]
brazilianportuguese.AppRunningWarning=O ReGraphik está em execução no momento.%n%nSe você prosseguir, a aplicação será fechada automaticamente para concluir a operação.%n%nDeseja continuar?
english.AppRunningWarning=ReGraphik is currently running.%n%nIf you proceed, the application will be closed automatically to complete the operation.%n%nDo you wish to continue?

brazilianportuguese.OldVersionDetected=Uma versão anterior do ReGraphik foi detectada no sistema.%n%nDeseja desinstalá-la automaticamente para prosseguir com a nova instalação?
english.OldVersionDetected=A previous version of ReGraphik was detected on your system.%n%nDo you want to automatically uninstall it to proceed with the new installation?

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"

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

[Code]
// --- FUNÇÃO PARA VERIFICAR E FECHAR O PROCESSO ABERTO ---
// --- FUNÇÃO PARA VERIFICAR E FECHAR O PROCESSO ABERTO ---
function CheckAndCloseApp(): Boolean;
var
  ResultCode: Integer;
  UserChoice: Integer;
begin
  Result := True;

  // Verifica se o ReGraphik.exe está rodando usando o tasklist do Windows
  Exec('cmd.exe', '/c tasklist /FI "IMAGENAME eq ReGraphik.exe" | find /I "ReGraphik.exe"', '', SW_HIDE, ewWaitUntilTerminated, ResultCode);

  // Se o ResultCode for 0, significa que o processo foi encontrado aberto
  if ResultCode = 0 then
  begin
    // Alerta o usuário informando que o programa será fechado (Uso de mbConfirmation)
    UserChoice := MsgBox(CustomMessage('AppRunningWarning'), mbConfirmation, MB_YESNO);

    if UserChoice = IDYES then
    begin
      // Força o fechamento do ReGraphik.exe
      Exec('taskkill.exe', '/F /IM ReGraphik.exe', '', SW_HIDE, ewWaitUntilTerminated, ResultCode);
      Result := True;
    end
    else
    begin
      // Usuário optou por cancelar
      Result := False;
    end;
  end;
end;

// --- FUNÇÃO EXECUTADA AO INICIAR A INSTALAÇÃO ---
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
    // Pergunta se deseja remover a versão antiga
    UserChoice := MsgBox(CustomMessage('OldVersionDetected'), mbConfirmation, MB_YESNO);

    if UserChoice = IDYES then
    begin
      // 1. Verifica se a aplicação está aberta antes de desinstalar
      if not CheckAndCloseApp() then
      begin
        Result := False;
        Exit;
      end;

      // 2. Limpa as aspas do caminho do desinstalador
      UninstPath := RemoveQuotes(UninstPath);

      // 3. Executa a remoção silenciosa
      Exec(UninstPath, '/SILENT /NORESTART /SUPPRESSMSGBOXES', '', SW_HIDE, ewWaitUntilTerminated, ResultCode);
    end
    else
    begin
      // Cancela o instalador caso o usuário escolha NÃO
      Result := False;
    end;
  end;
end;

// --- VALIDAÇÃO ADICIONAL ANTES DE COMEÇAR A COPIAR OS ARQUIVOS ---
function NextButtonClick(CurPageID: Integer): Boolean;
begin
  Result := True;
  
  // Na página de seleção de pasta, faz uma última checagem se o App foi aberto
  if CurPageID = wpSelectDir then
  begin
    Result := CheckAndCloseApp();
  end;
end;