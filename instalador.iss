[Setup]
AppName=PDV Pro
AppVersion=1.0
DefaultDirName={autopf}\PDV Pro
DefaultGroupName=PDV Pro
OutputDir=.\InstaladorFinal
OutputBaseFilename=InstalarPDV
Compression=lzma
SolidCompression=yes
SetupIconFile=icone.ico

[Tasks]
Name: "desktopicon"; Description: "Criar icone no ambiente de trabalho"

[Files]
Source: "PDVPro.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "dist\agente_fiscal.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "gestor.db"; DestDir: "{app}"; Flags: ignoreversion
Source: "SQLite.Interop.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "System.Data.SQLite.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "fundo.jpg"; DestDir: "{app}"; Flags: ignoreversion
Source: "icone.ico"; DestDir: "{app}"; Flags: ignoreversion

[Icons]
Name: "{autoprograms}\PDV Pro"; Filename: "{app}\PDVPro.exe"; IconFilename: "{app}\icone.ico"
Name: "{autodesktop}\PDV Pro"; Filename: "{app}\PDVPro.exe"; Tasks: desktopicon; IconFilename: "{app}\icone.ico"

[Run]
Filename: "{app}\PDVPro.exe"; Description: "Iniciar PDV Pro"; Flags: nowait postinstall skipifsilent