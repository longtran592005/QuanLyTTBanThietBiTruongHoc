#define MyAppName "School Device Store"
#define MyAppVersion "1.0.0"
#define MyAppPublisher "School Device Store Team"
#define MyAppExeName "SchoolDeviceStore.GUI.exe"
#define MyAppId "{{86C2B4DA-7C9D-4D22-AB50-76E30AAEE42F}"

[Setup]
AppId={#MyAppId}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppPublisher={#MyAppPublisher}
DefaultDirName={localappdata}\Programs\SchoolDeviceStore
DefaultGroupName={#MyAppName}
DisableProgramGroupPage=yes
OutputDir=..\publish\installer
OutputBaseFilename=SchoolDeviceStoreSetup
SetupIconFile=..\GUI.WinForms\app.ico
Compression=lzma
SolidCompression=yes
WizardStyle=modern
PrivilegesRequired=lowest
ArchitecturesAllowed=x86 x64compatible
ArchitecturesInstallIn64BitMode=x64os
UninstallDisplayIcon={app}\{#MyAppExeName}

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "Create a desktop shortcut"; GroupDescription: "Additional shortcuts:"

[Files]
Source: "payload\{#MyAppExeName}"; DestDir: "{app}"; Flags: ignoreversion

[Icons]
Name: "{autoprograms}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "Launch {#MyAppName}"; Flags: nowait postinstall skipifsilent

[Code]
function IsDotNet48Installed: Boolean;
var
  Release: Cardinal;
begin
  Result := False;

  if RegQueryDWordValue(HKLM, 'SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full', 'Release', Release) then
  begin
    if Release >= 528040 then
    begin
      Result := True;
      Exit;
    end;
  end;

  if RegQueryDWordValue(HKLM64, 'SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full', 'Release', Release) then
  begin
    if Release >= 528040 then
    begin
      Result := True;
      Exit;
    end;
  end;
end;

function InitializeSetup: Boolean;
var
  Msg: String;
begin
  Result := True;

  if not IsDotNet48Installed() then
  begin
    Msg := '.NET Framework 4.8 Runtime is required.' + #13#10 + #13#10 +
           'Please install .NET Framework 4.8 Runtime first, then run this setup again.';
    MsgBox(Msg, mbError, MB_OK);
    Result := False;
  end;
end;
