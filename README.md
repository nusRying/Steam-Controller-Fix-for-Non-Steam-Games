# Steam Controller Bridge

Steam Controller Bridge is a Windows app that reads a Steam Controller through SDL3 and exposes it as a virtual Xbox 360 controller through ViGEm. The goal is to make the controller usable in non-Steam games without requiring Steam.

## Requirements

- Windows 10 or Windows 11.
- ViGEm Bus driver installed: https://github.com/ViGEm/ViGEmBus/releases
- `SDL3.dll` available next to the app or in the runtime path.

## Run It

### From source

```powershell
dotnet run --project . run
```

### From the installer

Run `installer_output\SteamControllerBridgeInstaller.exe` and launch the app from the Start menu or installed shortcut.

## Build

Publish a distributable build:

```powershell
.\publish.ps1 -Configuration Release -Runtime win-x64 -Output .\publish
```

Build the Windows installer:

```powershell
& 'C:\Users\umair\AppData\Local\Programs\Inno Setup 6\ISCC.exe' .\installer.iss
```

## Files

- `Program.cs` contains the command-line entry point and controller bridge loop.
- `SdlNative.cs` wraps the SDL3 controller APIs.
- `ViGEmXbox360Controller.cs` wraps the virtual Xbox 360 controller.
- `publish.ps1` creates the distributable publish folder.
- `installer.iss` builds the installer.

## Notes

- If the app exits with a ViGEm error, confirm the ViGEm Bus driver is installed and running.
- If SDL3 cannot be loaded, make sure `SDL3.dll` is present in the app folder or publish output.
