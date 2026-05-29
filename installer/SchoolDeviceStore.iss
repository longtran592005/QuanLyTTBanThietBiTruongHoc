; Inno Setup script for SchoolDeviceStore
#define MyAppName "SchoolDeviceStore"
#define MyAppVersion "1.0"
#define MyAppExe "SchoolDeviceStore.GUI.exe"  ; Update if your exe has a different name
#define VcRedistName "vc_redist_x64.exe"

[Setup]
AppName={#MyAppName}
AppVersion={#MyAppVersion}
DefaultDirName={pf}\{#MyAppName}
DefaultGroupName={#MyAppName}
DisableProgramGroupPage=yes
OutputBaseFilename={#MyAppName}-Setup
Compression=lzma
SolidCompression=yes
PrivilegesRequired=admin

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Files]
; Copies the entire publish folder into the installation directory. Run the publish script first.
Source: "..\publish\*"; DestDir: "{app}"; Flags: recursesubdirs createallsubdirs preserveallversions
; Optional: include Visual C++ Redistributable (place file in installer\tools\)
Source: "..\installer\tools\{#VcRedistName}"; DestDir: "{tmp}"; Flags: ignoreversion deleteafterinstall uninsneveruninstall

; Install the correct native SQLite interop for the target OS
Source: "..\publish\x64\SQLite.Interop.dll"; DestDir: "{app}"; Flags: ignoreversion; Check: Is64BitInstallMode
Source: "..\publish\x86\SQLite.Interop.dll"; DestDir: "{app}"; Flags: ignoreversion; Check: not Is64BitInstallMode

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExe}"
Name: "{userdesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExe}"; Tasks: desktopicon

[Run]
Filename: "{app}\{#MyAppExe}"; Description: "Launch {#MyAppName}"; Flags: nowait postinstall skipifsilent
; Run VC++ redistributable silently if included
Filename: "{tmp}\{#VcRedistName}"; Parameters: "/install /quiet /norestart"; Flags: runhidden waituntilterminated; Check: FileExists(ExpandConstant('{tmp}\{#VcRedistName}'))

[UninstallDelete]
Type: files; Name: "{app}\*"

; Notes:
; - Before compiling: run the publish script to create the 'publish' folder next to this script.
; - Edit the MyAppExe constant above if your exe filename differs.
; - Compile with Inno Setup (ISCC.exe) to produce the installer .exe.
