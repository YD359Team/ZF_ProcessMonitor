using System;
using System.Collections.Generic;
using System.Text;

namespace ZF_ProcessMonitor.Logic;

internal static class StringHelpers
{
    public static string RamToStr(long ramBytes)
    {
        const double KB = 1024;
        const double MB = KB * 1024;
        const double GB = MB * 1024;

        if (ramBytes < KB) return $"{ramBytes} B";
        if (ramBytes < MB)
        {
            return $"{ramBytes / KB:F2} KB";
        }
        if (ramBytes < GB)
        {
            return $"{ramBytes / MB:F2} MB";
        }

        return $"{ramBytes / GB:F2} GB";
    }
}
