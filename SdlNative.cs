using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

internal static class SdlNative
{
    private const string SdlLib = "SDL3";

    [DllImport(SdlLib, CallingConvention = CallingConvention.Cdecl)]
    [return: System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.I1)]
    private static extern bool SDL_Init(uint flags);

    [DllImport(SdlLib, CallingConvention = CallingConvention.Cdecl)]
    private static extern void SDL_Quit();

    [DllImport(SdlLib, CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr SDL_GetError();

    [DllImport(SdlLib, CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr SDL_GetGamepads(out int count);

    [DllImport(SdlLib, CallingConvention = CallingConvention.Cdecl)]
    private static extern void SDL_free(IntPtr mem);

    [DllImport(SdlLib, CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr SDL_OpenGamepad(int device_index);

    [DllImport(SdlLib, CallingConvention = CallingConvention.Cdecl)]
    private static extern void SDL_CloseGamepad(IntPtr gamepad);

    [DllImport(SdlLib, CallingConvention = CallingConvention.Cdecl)]
    private static extern ushort SDL_GetGamepadVendor(IntPtr gamepad);

    [DllImport(SdlLib, CallingConvention = CallingConvention.Cdecl)]
    private static extern ushort SDL_GetGamepadProduct(IntPtr gamepad);

    [DllImport(SdlLib, CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr SDL_GetGamepadName(IntPtr gamepad);

    [DllImport(SdlLib, CallingConvention = CallingConvention.Cdecl)]
    private static extern int SDL_GetNumGamepadTouchpads(IntPtr gamepad);

    [DllImport(SdlLib, CallingConvention = CallingConvention.Cdecl)]
    private static extern int SDL_GetGamepadTouchpadFinger(IntPtr gamepad, int touchpad, int finger, out byte down, out float x, out float y, out float pressure);

    [DllImport(SdlLib, CallingConvention = CallingConvention.Cdecl)]
    private static extern byte SDL_GetGamepadButton(IntPtr gamepad, int button);

    [DllImport(SdlLib, CallingConvention = CallingConvention.Cdecl)]
    private static extern short SDL_GetGamepadAxis(IntPtr gamepad, int axis);

    [DllImport(SdlLib, CallingConvention = CallingConvention.Cdecl)]
    private static extern void SDL_UpdateGamepads();

    private const uint SDL_INIT_GAMEPAD = 0x00001000;

    public static bool InitGamepad()
    {
        return SDL_Init(SDL_INIT_GAMEPAD);
    }

    public static void Quit()
    {
        SDL_Quit();
    }

    public static string? GetErrorMessage()
    {
        IntPtr ptr = SDL_GetError();
        return ptr == IntPtr.Zero ? null : Marshal.PtrToStringUTF8(ptr);
    }

    public static IReadOnlyList<int> GetGamepadIds()
    {
        int count = 0;
        IntPtr ptr = SDL_GetGamepads(out count);
        var list = new List<int>();
        if (ptr == IntPtr.Zero || count <= 0)
        {
            return list;
        }

        try
        {
            var ids = new int[count];
            Marshal.Copy(ptr, ids, 0, count);
            list.AddRange(ids);
        }
        finally
        {
            SDL_free(ptr);
        }

        return list;
    }

    public static nint OpenGamepad(int instanceId) => SDL_OpenGamepad(instanceId);
    public static void CloseGamepad(nint gamepad) => SDL_CloseGamepad(gamepad);

    public static ushort GetGamepadVendor(nint gamepad) => SDL_GetGamepadVendor(gamepad);
    public static ushort GetGamepadProduct(nint gamepad) => SDL_GetGamepadProduct(gamepad);
    public static string? GetGamepadName(nint gamepad)
    {
        IntPtr ptr = SDL_GetGamepadName(gamepad);
        return ptr == IntPtr.Zero ? null : Marshal.PtrToStringUTF8(ptr);
    }

    public static int GetNumGamepadTouchpads(nint gamepad) => SDL_GetNumGamepadTouchpads(gamepad);

    public static bool GetGamepadTouchpadFinger(nint gamepad, int touchpadIndex, int fingerIndex, out bool down, out float x, out float y, out float pressure)
    {
        if (SDL_GetGamepadTouchpadFinger(gamepad, touchpadIndex, fingerIndex, out byte d, out x, out y, out pressure) != 0)
        {
            down = d != 0;
            return true;
        }

        down = false;
        x = y = pressure = 0;
        return false;
    }

    public static bool GetGamepadButton(nint gamepad, SdlGamepadButton button) => SDL_GetGamepadButton(gamepad, (int)button) != 0;
    public static short GetGamepadAxis(nint gamepad, SdlGamepadAxis axis) => SDL_GetGamepadAxis(gamepad, (int)axis);

    public static void UpdateGamepads() => SDL_UpdateGamepads();
}

internal enum SdlGamepadButton
{
    South = 0,
    East = 1,
    West = 2,
    North = 3,
    LeftShoulder = 4,
    RightShoulder = 5,
    LeftTrigger = 6,
    RightTrigger = 7,
    Back = 8,
    Start = 9,
    LeftStick = 10,
    RightStick = 11,
    DPadUp = 12,
    DPadDown = 13,
    DPadLeft = 14,
    DPadRight = 15,
    Guide = 16
}

internal enum SdlGamepadAxis
{
    LeftX = 0,
    LeftY = 1,
    RightX = 2,
    RightY = 3,
    LeftTrigger = 4,
    RightTrigger = 5
}
