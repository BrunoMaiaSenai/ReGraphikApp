[Setup]
AppName=ReGraphik
AppVersion=1.0.0
DefaultDirName={autopf}\ReGraphik
DefaultGroupName=ReGraphik
OutputDir=.\Installer_InnoSetup
OutputBaseFilename=ReGraphik_Setup
Compression=lzma2
SolidCompression=yes

[Files]
; Publicação do App WPF
Source: "ReGraphik\bin\Release\net8.0-windows\win-x64\publish\*"; DestDir: "{app}\App"; Flags: ignoreversion recursesubdirs

; Publicação da API
Source: "ApiRestReGraphik\bin\Release\net8.0-windows\win-x64\publish\*"; DestDir: "{app}\API"; Flags: ignoreversion recursesubdirs

[Icons]
; Criar atalho no Menu Iniciar e na Área de Trabalho
Name: "{group}\ReGraphik"; Filename: "{app}\App\ReGraphik.exe"
Name: "{autodesktop}\ReGraphik"; Filename: "{app}\App\ReGraphik.exe"; Tasks: desktopicon

[Tasks]
Name: "desktopicon"; Description: "Criar atalho na Área de Trabalho"; GroupDescription: "Atalhos adicionais:"