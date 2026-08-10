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