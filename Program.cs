using System.Diagnostics;
using System.Globalization;

internal static class Program
{
	private const ushort ValveVendorId = 0x28DE;
	private const ushort SteamControllerUsbProductId = 0x1102;

	private static int Main(string[] args)
	{
		string command = args.Length == 0 ? "run" : args[0].ToLowerInvariant();

		try
		{
			return command switch
			{
				"run" => RunMapper(),
				"run-ui" => RunMapperUi(),
				"status" => PrintStatus(),
				"install-startup" => InstallStartup(),
				"uninstall-startup" => UninstallStartup(),
				"help" or "--help" or "-h" => PrintUsage(),
				_ => Fail($"Unknown command: {args[0]}")
			};
		}
		catch (Exception exception)
		{
			Console.Error.WriteLine(exception.ToString());
			return 1;
		}
	}

	private static int RunMapper()
	{
		using var mapper = new SteamControllerMapper();
		Console.WriteLine("Steam controller bridge running.");
		Console.WriteLine("Press Ctrl+C to stop.");
		mapper.Run();
		return 0;
	}

	private static int RunMapperUi()
	{
		using var mapper = new SteamControllerMapper();
		Console.WriteLine("Steam controller bridge running (UI mode).");
		Console.WriteLine("Press Ctrl+C to stop.");
		mapper.RunWithUiOutput();
		return 0;
	}

	private static int PrintStatus()
	{
		using var mapper = new SteamControllerMapper();
		IReadOnlyList<SteamControllerDeviceInfo> devices = mapper.DiscoverSteamControllers();

		if (devices.Count == 0)
		{
			Console.WriteLine("No Steam Controller detected through SDL.");
			return 0;
		}

		foreach (SteamControllerDeviceInfo device in devices)
		{
			Console.WriteLine($"{device.InstanceId}: {device.Name} [vendor=0x{device.VendorId:X4}, product=0x{device.ProductId:X4}, touchpads={device.Touchpads}]");
		}

		return 0;
	}

	private static int InstallStartup()
	{
		string exePath = Environment.ProcessPath ?? throw new InvalidOperationException("Could not resolve the current executable path.");
		string arguments = "run";
		StartupInstaller.Install(exePath, arguments);
		Console.WriteLine("Installed startup entry for the current user.");
		return 0;
	}

	private static int UninstallStartup()
	{
		StartupInstaller.Uninstall();
		Console.WriteLine("Removed startup entry for the current user.");
		return 0;
	}

	private static int PrintUsage()
	{
		Console.WriteLine("SteamControllerBridge");
		Console.WriteLine();
		Console.WriteLine("Usage:");
		Console.WriteLine("  SteamControllerBridge run               Start the controller bridge");
		Console.WriteLine("  SteamControllerBridge status            Detect Steam Controller devices");
		Console.WriteLine("  SteamControllerBridge install-startup   Start on login");
		Console.WriteLine("  SteamControllerBridge uninstall-startup Remove startup entry");
		return 0;
	}

	private static int Fail(string message)
	{
		Console.Error.WriteLine(message);
		PrintUsage();
		return 1;
	}
}

internal sealed class SteamControllerMapper : IDisposable
{
	private readonly Dictionary<int, SteamControllerSession> sessions = new();
	private readonly CancellationTokenSource cancellation = new();
	private readonly ViGEmXbox360Controller? virtualController = null;

	public SteamControllerMapper()
	{
		if (!SdlNative.InitGamepad())
		{
			throw new InvalidOperationException($"SDL initialization failed: {SdlNative.GetErrorMessage()}");
		}

		if (!IsViGEmServiceRunning())
		{
			throw new InvalidOperationException("ViGEm Bus driver not detected. Please install the ViGEm Bus driver from https://github.com/ViGEm/ViGEmBus/releases and ensure the service is running.");
		}

		// ViGEm client initialization is optional — skip here to avoid crashes if the driver
		// or client is not properly installed. Install ViGEm Bus and re-enable in future.

		if (IsViGEmServiceRunning())
		{
			try
			{
				virtualController = new ViGEmXbox360Controller();
				virtualController.Connect();
				Console.WriteLine("ViGEm client initialized.");
			}
			catch (Exception ex)
			{
				Console.Error.WriteLine("ViGEm initialization failed: " + ex.Message);
				virtualController = null;
			}
		}

		Console.CancelKeyPress += HandleCancelKeyPress;
	}

	private static bool IsViGEmServiceRunning()
	{
		try
		{
			string driverPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "System32", "drivers", "ViGEmBus.sys");
			return File.Exists(driverPath);
		}
		catch
		{
			return false;
		}
	}

	public void Run()
	{
		try
		{
			while (!cancellation.IsCancellationRequested)
			{
				SdlNative.UpdateGamepads();
				RefreshSessions();
				UpdateSessions();
				Thread.Sleep(8);
			}
		}
		finally
		{
			Dispose();
		}
	}

		public void RunWithUiOutput()
		{
			try
			{
				while (!cancellation.IsCancellationRequested)
				{
					SdlNative.UpdateGamepads();
					RefreshSessions();
					UpdateSessions();
					// emit a status line per session for UI parsing
					foreach (var kv in sessions)
					{
						int instanceId = kv.Key;
						try
						{
							string status = kv.Value.GetStatusText();
							Console.WriteLine($"STATE {instanceId} {status}");
						}
						catch { }
					}
					Thread.Sleep(50);
				}
			}
			finally
			{
				Dispose();
			}
		}

	public IReadOnlyList<SteamControllerDeviceInfo> DiscoverSteamControllers()
	{
		var devices = new List<SteamControllerDeviceInfo>();
		foreach (int instanceId in SdlNative.GetGamepadIds())
		{
			nint gamepad = SdlNative.OpenGamepad(instanceId);
			if (gamepad == nint.Zero)
			{
				continue;
			}

			try
			{
				if (SteamControllerDeviceInfo.TryCreate(instanceId, gamepad, out SteamControllerDeviceInfo? device) && device is not null)
				{
					devices.Add(device);
				}
			}
			finally
			{
				SdlNative.CloseGamepad(gamepad);
			}
		}

		return devices;
	}

	private void RefreshSessions()
	{
		HashSet<int> connectedIds = new(SdlNative.GetGamepadIds());

		foreach (int instanceId in connectedIds)
		{
			if (sessions.ContainsKey(instanceId))
			{
				continue;
			}

			nint gamepad = SdlNative.OpenGamepad(instanceId);
			if (gamepad == nint.Zero)
			{
				continue;
			}

			if (!SteamControllerDeviceInfo.TryCreate(instanceId, gamepad, out SteamControllerDeviceInfo? deviceInfo))
			{
				SdlNative.CloseGamepad(gamepad);
				continue;
			}

			sessions.Add(instanceId, new SteamControllerSession(gamepad, deviceInfo));
			Console.WriteLine($"Connected: {deviceInfo.Name}");
		}

		var disconnected = sessions.Keys.Where(id => !connectedIds.Contains(id)).ToArray();
		foreach (int instanceId in disconnected)
		{
			sessions[instanceId].Dispose();
			sessions.Remove(instanceId);
			Console.WriteLine($"Disconnected: {instanceId}");
			virtualController?.Reset();
		}
	}

	private void UpdateSessions()
	{
		foreach (SteamControllerSession session in sessions.Values)
		{
			session.Update(virtualController);
		}
	}

	private void HandleCancelKeyPress(object? sender, ConsoleCancelEventArgs e)
	{
		e.Cancel = true;
		cancellation.Cancel();
	}

	public IReadOnlyList<SteamControllerDeviceInfo> DiscoverSteamControllersCached()
	{
		return DiscoverSteamControllers();
	}

	public void Dispose()
	{
		Console.CancelKeyPress -= HandleCancelKeyPress;

		foreach (SteamControllerSession session in sessions.Values)
		{
			session.Dispose();
		}

		sessions.Clear();
		virtualController?.Dispose();
		SdlNative.Quit();
		cancellation.Dispose();
	}
}

internal sealed class SteamControllerSession : IDisposable
{
	private readonly nint gamepad;
	private readonly SteamControllerDeviceInfo deviceInfo;

	public SteamControllerSession(nint gamepad, SteamControllerDeviceInfo deviceInfo)
	{
		this.gamepad = gamepad;
		this.deviceInfo = deviceInfo;
	}

	public void Update(ViGEmXbox360Controller? virtualController)
	{
		if (virtualController is null)
		{
			// No virtual controller available; nothing to send
			return;
		}

		public string GetStatusText()
		{
			try
			{
				short lx = SdlNative.GetGamepadAxis(gamepad, SdlGamepadAxis.LeftX);
				short ly = SdlNative.GetGamepadAxis(gamepad, SdlGamepadAxis.LeftY);
				short rx = SdlNative.GetGamepadAxis(gamepad, SdlGamepadAxis.RightX);
				short ry = SdlNative.GetGamepadAxis(gamepad, SdlGamepadAxis.RightY);
				short lt = SdlNative.GetGamepadAxis(gamepad, SdlGamepadAxis.LeftTrigger);
				short rt = SdlNative.GetGamepadAxis(gamepad, SdlGamepadAxis.RightTrigger);

				string tp0 = "";
				string tp1 = "";
				if (SdlNative.GetNumGamepadTouchpads(gamepad) > 0)
				{
					if (TryReadTouchpad(0, out bool d0, out float x0, out float y0))
						tp0 = $"tp0={(d0?1:0)},{x0:F3},{y0:F3}";
				}
				if (SdlNative.GetNumGamepadTouchpads(gamepad) > 1)
				{
					if (TryReadTouchpad(1, out bool d1, out float x1, out float y1))
						tp1 = $"tp1={(d1?1:0)},{x1:F3},{y1:F3}";
				}

				return $"{deviceInfo.Name.Replace(' ', '_')} lx={lx} ly={ly} rx={rx} ry={ry} lt={lt} rt={rt} {tp0} {tp1}";
			}
			catch
			{
				return deviceInfo.Name.Replace(' ', '_');
			}
		}
		virtualController.SetButton("A", SdlNative.GetGamepadButton(gamepad, SdlGamepadButton.South));
		virtualController.SetButton("B", SdlNative.GetGamepadButton(gamepad, SdlGamepadButton.East));
		virtualController.SetButton("X", SdlNative.GetGamepadButton(gamepad, SdlGamepadButton.West));
		virtualController.SetButton("Y", SdlNative.GetGamepadButton(gamepad, SdlGamepadButton.North));
		virtualController.SetButton("Back", SdlNative.GetGamepadButton(gamepad, SdlGamepadButton.Back));
		virtualController.SetButton("Start", SdlNative.GetGamepadButton(gamepad, SdlGamepadButton.Start));
		virtualController.SetButton("Guide", SdlNative.GetGamepadButton(gamepad, SdlGamepadButton.Guide));
		virtualController.SetButton("LeftShoulder", SdlNative.GetGamepadButton(gamepad, SdlGamepadButton.LeftShoulder));
		virtualController.SetButton("RightShoulder", SdlNative.GetGamepadButton(gamepad, SdlGamepadButton.RightShoulder));
		virtualController.SetButton("LeftStick", SdlNative.GetGamepadButton(gamepad, SdlGamepadButton.LeftStick));
		virtualController.SetButton("RightStick", SdlNative.GetGamepadButton(gamepad, SdlGamepadButton.RightStick));
		virtualController.SetButton("DPadUp", SdlNative.GetGamepadButton(gamepad, SdlGamepadButton.DPadUp));
		virtualController.SetButton("DPadDown", SdlNative.GetGamepadButton(gamepad, SdlGamepadButton.DPadDown));
		virtualController.SetButton("DPadLeft", SdlNative.GetGamepadButton(gamepad, SdlGamepadButton.DPadLeft));
		virtualController.SetButton("DPadRight", SdlNative.GetGamepadButton(gamepad, SdlGamepadButton.DPadRight));

		virtualController.SetAxis("LeftThumbX", SdlNative.GetGamepadAxis(gamepad, SdlGamepadAxis.LeftX));
		virtualController.SetAxis("LeftThumbY", SdlNative.GetGamepadAxis(gamepad, SdlGamepadAxis.LeftY));

		if (deviceInfo.Touchpads > 1)
		{
			MapRightTouchpadToRightStick(virtualController);
		}
		else
		{
			virtualController.SetAxis("RightThumbX", SdlNative.GetGamepadAxis(gamepad, SdlGamepadAxis.RightX));
			virtualController.SetAxis("RightThumbY", SdlNative.GetGamepadAxis(gamepad, SdlGamepadAxis.RightY));
		}

		MapLeftTouchpadToDPad(virtualController);
		virtualController.SetAxis("LeftTrigger", ToTriggerValue(SdlNative.GetGamepadAxis(gamepad, SdlGamepadAxis.LeftTrigger)));
		virtualController.SetAxis("RightTrigger", ToTriggerValue(SdlNative.GetGamepadAxis(gamepad, SdlGamepadAxis.RightTrigger)));
	}

	private void MapLeftTouchpadToDPad(ViGEmXbox360Controller virtualController)
	{
		if (!TryReadTouchpad(0, out bool down, out float x, out float y))
		{
			virtualController.SetButton("DPadUp", false);
			virtualController.SetButton("DPadDown", false);
			virtualController.SetButton("DPadLeft", false);
			virtualController.SetButton("DPadRight", false);
			return;
		}

		if (!down)
		{
			virtualController.SetButton("DPadUp", false);
			virtualController.SetButton("DPadDown", false);
			virtualController.SetButton("DPadLeft", false);
			virtualController.SetButton("DPadRight", false);
			return;
		}

		const float leftThreshold = 0.35f;
		const float rightThreshold = 0.65f;
		const float topThreshold = 0.35f;
		const float bottomThreshold = 0.65f;

		virtualController.SetButton("DPadLeft", x <= leftThreshold);
		virtualController.SetButton("DPadRight", x >= rightThreshold);
		virtualController.SetButton("DPadUp", y <= topThreshold);
		virtualController.SetButton("DPadDown", y >= bottomThreshold);
	}

	private void MapRightTouchpadToRightStick(ViGEmXbox360Controller virtualController)
	{
		if (!TryReadTouchpad(1, out bool down, out float x, out float y) || !down)
		{
			virtualController.SetAxis("RightThumbX", 0);
			virtualController.SetAxis("RightThumbY", 0);
			return;
		}

		short rightX = ToThumbValue((x - 0.5f) * 2.0f);
		short rightY = ToThumbValue((0.5f - y) * 2.0f);
		virtualController.SetAxis("RightThumbX", rightX);
		virtualController.SetAxis("RightThumbY", rightY);
	}

	private bool TryReadTouchpad(int touchpadIndex, out bool down, out float x, out float y)
	{
		down = false;
		x = 0;
		y = 0;

		return SdlNative.GetGamepadTouchpadFinger(gamepad, touchpadIndex, 0, out down, out x, out y, out _);
	}

	private static short ToThumbValue(float normalized)
	{
		float clamped = Math.Clamp(normalized, -1.0f, 1.0f);
		return (short)Math.Round(clamped * short.MaxValue, MidpointRounding.AwayFromZero);
	}

	private static short ToTriggerValue(short sdlTriggerValue)
	{
		float normalized = sdlTriggerValue / 32767f;
		return (short)Math.Round(Math.Clamp(normalized, 0.0f, 1.0f) * short.MaxValue, MidpointRounding.AwayFromZero);
	}

	public void Dispose()
	{
		SdlNative.CloseGamepad(gamepad);
	}
}

internal sealed class SteamControllerDeviceInfo
{
	public SteamControllerDeviceInfo(int instanceId, string name, ushort vendorId, ushort productId, int touchpads)
	{
		InstanceId = instanceId;
		Name = name;
		VendorId = vendorId;
		ProductId = productId;
		Touchpads = touchpads;
	}

	public int InstanceId { get; }
	public string Name { get; }
	public ushort VendorId { get; }
	public ushort ProductId { get; }
	public int Touchpads { get; }

	public static bool TryCreate(int instanceId, nint gamepad) => TryCreate(instanceId, gamepad, out _);

	public static bool TryCreate(int instanceId, nint gamepad, out SteamControllerDeviceInfo? deviceInfo)
	{
		ushort vendorId = SdlNative.GetGamepadVendor(gamepad);
		ushort productId = SdlNative.GetGamepadProduct(gamepad);
		string name = SdlNative.GetGamepadName(gamepad) ?? string.Empty;

		bool looksLikeSteamController = vendorId == 0x28DE && (productId == 0x1102 || name.Contains("Steam Controller", StringComparison.OrdinalIgnoreCase));
		if (!looksLikeSteamController)
		{
			deviceInfo = null;
			return false;
		}

		int touchpads = SdlNative.GetNumGamepadTouchpads(gamepad);
		deviceInfo = new SteamControllerDeviceInfo(instanceId, name, vendorId, productId, touchpads);
		return true;
	}
}

internal static class StartupInstaller
{
	private const string RunKeyPath = @"Software\Microsoft\Windows\CurrentVersion\Run";
	private const string ValueName = "SteamControllerBridge";

	public static void Install(string executablePath, string arguments)
	{
		using var key = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(RunKeyPath, writable: true)
			?? throw new InvalidOperationException("Could not create the startup registry key.");

		key.SetValue(ValueName, Quote(executablePath) + " " + arguments, Microsoft.Win32.RegistryValueKind.String);
	}

	public static void Uninstall()
	{
		using var key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(RunKeyPath, writable: true);
		key?.DeleteValue(ValueName, throwOnMissingValue: false);
	}

	private static string Quote(string value) => value.Contains(' ') ? $"\"{value}\"" : value;
}

internal static class StringExtensions
{
	public static string? ToNullableString(this nint pointer)
	{
		return pointer == nint.Zero ? null : System.Runtime.InteropServices.Marshal.PtrToStringUTF8(pointer);
	}
}
