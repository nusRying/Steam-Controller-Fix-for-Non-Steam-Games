# Steam Controller Bridge

Steam Controller Bridge exposes a Steam Controller as a virtual Xbox 360 controller on Windows so non-Steam games can use it via XInput. It reads the controller using SDL3 and sends input to a ViGEm virtual Xbox 360 device.

Key features
- Read Steam Controller input using SDL3
- Present a ViGEm virtual Xbox 360 controller (XInput)
- Small Windows GUI and headless CLI mode

Quick Start

Prerequisites
- Windows 10 or Windows 11
- ViGEm Bus driver: https://github.com/ViGEm/ViGEmBus/releases (install and reboot if required)
- `SDL3.dll` available next to the app binary or on the system PATH

Run from source

Open a PowerShell prompt and run:

```powershell
dotnet run --project . -- run
```

Run headless (no GUI):

```powershell
dotnet run --project . -- run --no-gui
```

Run with GUI (Windows):

```powershell
dotnet run --project SteamControllerBridge.UI
```

Publish a portable build (example for win-x64):

```powershell
.\publish.ps1 -Configuration Release -Runtime win-x64 -Output .\publish
```

Installer

If you build the installer, the author used Inno Setup. Example to build (adjust path to your ISCC.exe):

```powershell
& 'C:\Users\umair\AppData\Local\Programs\Inno Setup 6\ISCC.exe' .\installer.iss
```

Project layout

- `Program.cs` — command-line entry and main bridge loop
- `SdlNative.cs` — SDL3 interop for the Steam Controller
- `ViGEmXbox360Controller.cs` — ViGEm wrapper for the virtual controller
- `SteamControllerBridge.UI/` — Windows Forms UI and settings
- `publish.ps1` — helper script to publish binaries
- `installer.iss` — Inno Setup script for creating the installer

Troubleshooting

- ViGEm errors: ensure ViGEm Bus driver is installed and running; reinstall if needed.
- SDL3 load failures: place `SDL3.dll` alongside the executable or in your PATH.
- If controls behave oddly in a game, try recalibrating mappings in the GUI or testing with an XInput tester.

Contributing

Contributions welcome. Open issues for bugs or feature requests and send PRs for fixes or improvements.

License

See the project repository for license information (no license file included here).

For UI-specific settings and documentation, see the `SteamControllerBridge.UI` folder.
