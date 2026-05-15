SteamControllerBridge
=====================

Quick start
-----------

1. Install the ViGEm Bus driver (required to expose a virtual Xbox 360 controller).
   - Official installer: https://github.com/ViGEm/ViGEmBus/releases
   - Or install via Chocolatey (Admin PowerShell): `choco install vigem`.

2. Place `SDL3.dll` next to the built executable (`bin\Debug\net10.0-windows`) or install the SDL3 runtime.
   - Download the Windows redistributable from https://www.libsdl.org/ (pick the Win32/Win64 SDL3 build).
   - Alternatively, use the included `install-deps.ps1` to download and extract a supplied SDL3 zip (you must provide a valid download URL).

3. Run the bridge:

```powershell
dotnet run --project . run
```

Notes
-----
- The project uses SDL3 via P/Invoke and Nefarius.ViGEm.Client for the virtual controller. The ViGEm Bus driver must be installed for the virtual controller to appear.
- If you get "Unable to load DLL 'SDL3'" then `SDL3.dll` is missing from the process PATH or application folder.

If you want, allow me to download a specific SDL3 redistributable into the build output and retry running the bridge. Provide the SDL3 Windows zip URL or say "auto" to let me attempt a known SDL3 download URL.
# Steam Controller Bridge

This tool automates the Steam Library step that makes Steam Controller support work for non-Steam games.

Steam stores non-Steam entries in `shortcuts.vdf`. This app writes those entries for you so you do not need to add games manually in the Steam UI every time.

## Commands

```powershell
SteamControllerBridge init
SteamControllerBridge add "C:\Games\Hades\Hades.exe" Hades
SteamControllerBridge sync steam-shortcuts.json
```

## Workflow

1. Run `SteamControllerBridge init` to create a starter `steam-shortcuts.json` file.
2. Edit the file with the games you want Steam to manage.
3. Run `SteamControllerBridge sync` whenever your library changes.

## Notes

- Steam must be installed and you need to have launched it at least once.
- The tool searches for `userdata/*/config/shortcuts.vdf` under your Steam folder.
- If Steam is already open, restart it after syncing so it reloads the shortcuts file.

Building a redistributable EXE and installer
-------------------------------------------

1. Create a self-contained single-file EXE (publish):

```powershell
.\publish.ps1 -Configuration Release -Runtime win-x64 -Output .\publish
```

2. Build an installer (Inno Setup):
 - Install Inno Setup: https://jrsoftware.org/isinfo.php
 - Open `installer.iss` and replace the `{#PUBLISH_DIR}` token with the absolute path to the `publish` folder (or run `ISCC` with a preprocessor define).
 - Compile with Inno Setup Compiler (`ISCC.exe installer.iss`). The resulting installer will be in `installer_output`.

Notes
-----
- The installer script is a minimal template — adjust license, icons, and shortcuts as needed.
- The published folder already includes `SDL3.dll` (if present) and `Nefarius.ViGEm.Client.dll` to simplify distribution.
