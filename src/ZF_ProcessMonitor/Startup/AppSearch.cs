using Microsoft.Win32;
using ZF_ProcessMonitor.Models;

namespace ZF_ProcessMonitor.Startup;

internal static class AppSearch
{
    public static StartupEntry[] Find()
    {
        List<StartupEntry> entries = [];

        List<(string, string)> paths = [];
        paths.Add(("All users", "C:\\ProgramData\\Microsoft\\Windows\\Start Menu\\Programs\\Startup"));

        string volume = Environment.GetLogicalDrives()[0];
        string user = Environment.UserName;
        string path = $"{volume}Users\\{user}\\AppData\\Roaming\\Microsoft\\Windows\\Start Menu\\Programs\\Startup";
        paths.Add(("Current user", "C:\\ProgramData\\Microsoft\\Windows\\Start Menu\\Programs\\Startup"));

        foreach (var kv in paths)
        {
            foreach (var file in Directory.GetFiles(kv.Item2))
            {
                entries.Add(new(kv.Item1, file));
            }
        }
        return entries.ToArray();
    }
}
