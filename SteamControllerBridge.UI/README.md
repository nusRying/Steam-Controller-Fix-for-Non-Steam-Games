# SteamControllerBridge.UI

Tray UI for SteamControllerBridge. Provides Start/Stop controls and a simple settings editor.

Run from the UI project folder:

```powershell
dotnet run --project .
```

Notes:
- The UI starts the `SteamControllerBridge.exe` (expects it to be in the parent folder of the UI project output).
- Settings are saved to `bridge-settings.json` next to the bridge executable.
