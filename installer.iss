[Setup]
AppName=Steam Controller Bridge
AppVersion=0.1
DefaultDirName={commonpf32}\Steam Controller Bridge
DefaultGroupName=Steam Controller Bridge
OutputDir=installer_output
OutputBaseFilename=SteamControllerBridgeInstaller
Compression=lzma2/ultra
SolidCompression=yes

#ifndef PUBLISH_DIR
	#define PUBLISH_DIR "publish"
#endif

[Files]
; Copy all published files
Source: "{#PUBLISH_DIR}\*"; DestDir: "{app}"; Flags: recursesubdirs createallsubdirs

[Icons]
Name: "{group}\Steam Controller Bridge"; Filename: "{app}\SteamControllerBridge.exe"

[Run]
Filename: "{app}\SteamControllerBridge.exe"; Description: "Launch Steam Controller Bridge"; Flags: nowait postinstall skipifsilent
