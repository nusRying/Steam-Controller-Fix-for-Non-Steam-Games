[Setup]
AppName=Steam Controller Bridge
AppVersion=0.1
DefaultDirName={pf32}\Steam Controller Bridge
DefaultGroupName=Steam Controller Bridge
OutputDir=installer_output
OutputBaseFilename=SteamControllerBridgeInstaller
Compression=lzma2/ultra
SolidCompression=yes

[Files]
; Copy all published files
Source: "{#PUBLISH_DIR}\*"; DestDir: "{app}"; Flags: recursesubdirs createallsubdirs

[Icons]
Name: "{group}\Steam Controller Bridge"; Filename: "{app}\SteamControllerBridge.exe"

[Run]
Filename: "{app}\SteamControllerBridge.exe"; Description: "Launch Steam Controller Bridge"; Flags: nowait postinstall skipifsilent

; NOTE: Replace {#PUBLISH_DIR} with the actual publish folder when invoking ISCC.
