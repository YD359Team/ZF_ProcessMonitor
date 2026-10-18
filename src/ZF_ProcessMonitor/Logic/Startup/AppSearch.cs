using Microsoft.Win32;
using ZF_ProcessMonitor.Models;

namespace ZF_ProcessMonitor.Logic.Startup;

internal static class AppSearch
{
    public static StartupEntry[] Find()
    {
        List<StartupEntry> entries = [];
        string volume = Environment.GetLogicalDrives()[0];
        string user = Environment.UserName;

        List<PathInfo> paths = [];
        paths.Add(new("All users", $"{volume}ProgramData\\Microsoft\\Windows\\Start Menu\\Programs\\Startup"));

        string path = $"{volume}Users\\{user}\\AppData\\Roaming\\Microsoft\\Windows\\Start Menu\\Programs\\Startup";
        paths.Add(new("Current user", path));

        foreach (var kv in paths)
        {
            if (!Directory.Exists(kv.RootPath)) continue;

            foreach (var file in Directory.GetFiles(kv.RootPath))
            {
                entries.Add(new(kv.Category, Path.GetFileName(file), Path.Combine(kv.RootPath, file)));
            }
        }
        return entries.ToArray();
    }

    private record struct PathInfo(string Category, string RootPath);
}
