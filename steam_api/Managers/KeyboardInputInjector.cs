using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;

namespace SKYNET.Managers
{
    /// <summary>
    /// Restores focus to the game and emits Unicode keyboard input for the
    /// floating Steam keyboard contract. Work is always performed off-thread.
    /// </summary>
    internal static class KeyboardInputInjector
    {
        private const uint InputKeyboard = 1;
        private const uint KeyEventUnicode = 0x0004;
        private const uint KeyEventKeyUp = 0x0002;

        public static void QueueText(IntPtr targetWindow, string text)
        {
            if (targetWindow == IntPtr.Zero || string.IsNullOrEmpty(text))
            {
                return;
            }

            ThreadPool.QueueUserWorkItem(_ =>
            {
                try
                {
                    SetForegroundWindow(targetWindow);
                    Thread.Sleep(40);

                    var inputs = new List<NativeInput>(text.Length * 2);
                    foreach (var character in text)
                    {
                        inputs.Add(CreateKeyboardInput(character, keyUp: false));
                        inputs.Add(CreateKeyboardInput(character, keyUp: true));
                    }

                    if (inputs.Count > 0)
                    {
                        var sent = SendInput(
                            checked((uint)inputs.Count),
                            inputs.ToArray(),
                            Marshal.SizeOf(typeof(NativeInput)));
                        if (sent != (uint)inputs.Count)
                        {
                            SteamEmulator.Write(
                                "KeyboardInputInjector",
                                $"SendInput wrote {sent}/{inputs.Count} events (error {Marshal.GetLastWin32Error()})");
                        }
                    }
                }
                catch (Exception ex)
                {
                    SteamEmulator.Write("KeyboardInputInjector", ex);
                }
            });
        }

        private static NativeInput CreateKeyboardInput(char character, bool keyUp)
        {
            return new NativeInput
            {
                Type = InputKeyboard,
                Union = new InputUnion
                {
                    Keyboard = new KeyboardInput
                    {
                        Scan = character,
                        Flags = KeyEventUnicode | (keyUp ? KeyEventKeyUp : 0)
                    }
                }
            };
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetForegroundWindow(IntPtr window);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern uint SendInput(uint inputCount, NativeInput[] inputs, int inputSize);

        [StructLayout(LayoutKind.Sequential)]
        private struct NativeInput
        {
            public uint Type;
            public InputUnion Union;
        }

        [StructLayout(LayoutKind.Explicit)]
        private struct InputUnion
        {
            [FieldOffset(0)]
            public KeyboardInput Keyboard;

            [FieldOffset(0)]
            public MouseInput Mouse;

            [FieldOffset(0)]
            public HardwareInput Hardware;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct KeyboardInput
        {
            public ushort VirtualKey;
            public ushort Scan;
            public uint Flags;
            public uint Time;
            public UIntPtr ExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MouseInput
        {
            public int X;
            public int Y;
            public uint MouseData;
            public uint Flags;
            public uint Time;
            public UIntPtr ExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct HardwareInput
        {
            public uint Message;
            public ushort ParameterLow;
            public ushort ParameterHigh;
        }
    }
}
