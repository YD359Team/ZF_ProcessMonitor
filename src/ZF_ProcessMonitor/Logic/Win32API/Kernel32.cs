using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace ZF_ProcessMonitor.Logic.Win32API;

internal static class Kernel32
{
    [DllImport("kernel32.dll", SetLastError = true, CallingConvention = CallingConvention.Winapi)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool IsWow64Process([In] IntPtr hProcess, [Out] out bool lpSystemInfo);
}
