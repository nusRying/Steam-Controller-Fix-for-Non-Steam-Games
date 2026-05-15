using System;
using System.Collections.Concurrent;
using System.Reflection;

internal sealed class ViGEmXbox360Controller : IDisposable
{
    private Assembly? clientAssembly;
    private object? clientInstance;
    private object? controllerInstance;
    private Type? xboxButtonType;
    private Type? xboxAxisType;

    public ViGEmXbox360Controller()
    {
        // Lazy initialization in Connect to avoid early native calls that may crash
    }

    private static Assembly? LoadViGEmAssembly()
    {
        foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
        {
            if (asm.GetName().Name?.StartsWith("Nefarius.ViGEm.Client", StringComparison.OrdinalIgnoreCase) == true)
                return asm;
        }

        try
        {
            return Assembly.Load("Nefarius.ViGEm.Client");
        }
        catch
        {
            return null;
        }
    }

    public void Connect()
    {
        try
        {
            if (controllerInstance is null)
            {
                clientAssembly = LoadViGEmAssembly();
                if (clientAssembly is null) throw new InvalidOperationException("ViGEm.Client assembly not found. Ensure Nefarius.ViGEm.Client is referenced and restored.");

                Type? clientType = clientAssembly.GetType("Nefarius.ViGEm.Client.ViGEmClient");
                Type? xboxType = clientAssembly.GetType("Nefarius.ViGEm.Client.Targets.Xbox360Controller");
                xboxButtonType = clientAssembly.GetType("Nefarius.ViGEm.Client.Targets.Xbox360Button");
                xboxAxisType = clientAssembly.GetType("Nefarius.ViGEm.Client.Targets.Xbox360Axis");

                if (clientType is null || xboxType is null) throw new InvalidOperationException("Could not find ViGEm client types in the assembly.");

                clientInstance = Activator.CreateInstance(clientType)!;
                controllerInstance = Activator.CreateInstance(xboxType, new object[] { clientInstance })!;
            }

            InvokeIfExists(controllerInstance, "Connect");
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine("ViGEm connect failed: " + ex.Message);
            // Leave controllerInstance as null to avoid disposing an invalid native object
            controllerInstance = null;
            clientInstance = null;
        }
    }

    public void Reset()
    {
        InvokeIfExists(controllerInstance, "Reset");
    }

    public void SetButton(string name, bool pressed)
    {
        if (controllerInstance is null) return;

        object? enumVal = TryGetEnumValue(xboxButtonType, name);
        if (enumVal is null)
        {
            return;
        }

        InvokeIfExists(controllerInstance, "SetButtonState", enumVal, pressed);
    }

    public void SetAxis(string name, short value)
    {
        if (controllerInstance is null) return;

        object? enumVal = TryGetEnumValue(xboxAxisType, name);
        if (enumVal is null)
        {
            return;
        }

        InvokeIfExists(controllerInstance, "SetAxisValue", enumVal, value);
    }

    private static object? TryGetEnumValue(Type? enumType, string name)
    {
        if (enumType is null) return null;
        try
        {
            // Try direct parse
            return Enum.Parse(enumType, name, ignoreCase: true);
        }
        catch
        {
            // Try common renames
            var map = new ConcurrentDictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            map["LeftThumbX"] = "LeftThumbX";
            map["LeftThumbY"] = "LeftThumbY";
            map["RightThumbX"] = "RightThumbX";
            map["RightThumbY"] = "RightThumbY";
            map["LeftTrigger"] = "LeftTrigger";
            map["RightTrigger"] = "RightTrigger";
            map["A"] = "A";
            map["B"] = "B";
            map["X"] = "X";
            map["Y"] = "Y";
            map["DPadUp"] = "Up";
            map["DPadDown"] = "Down";
            map["DPadLeft"] = "Left";
            map["DPadRight"] = "Right";

            if (map.TryGetValue(name, out var alt))
            {
                try
                {
                    return Enum.Parse(enumType, alt, ignoreCase: true);
                }
                catch
                {
                    return null;
                }
            }

            return null;
        }
    }

    private static object? InvokeIfExists(object? target, string methodName, params object?[] args)
    {
        if (target is null) return null;
        MethodInfo? method = target.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);
        if (method is null) return null;
        return method.Invoke(target, args);
    }

    public void Dispose()
    {
        if (controllerInstance is not null)
        {
            InvokeIfExists(controllerInstance, "Disconnect");
        }

        if (clientInstance is IDisposable disposable)
        {
            disposable.Dispose();
        }
    }
}
