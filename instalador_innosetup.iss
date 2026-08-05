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