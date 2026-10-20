using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Text;
using ZF_ProcessMonitor.Models;

namespace ZF_ProcessMonitor.Logic.Startup;

internal static class RegistrySearch
{
    private static readonly string[] HKCU = 
    [
       "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run",
       "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\RunOnce",
       "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Policies\\Explorer\\Run",
       // a path under the hive, without its name: "HKEY_CURRENT_USER\\..." found nothing
       "SOFTWARE\\Wow6432Node\\Microsoft\\Windows\\CurrentVersion\\Run",
       "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Explorer\\StartupApproved\\Run",
    ];
    private static readonly string[] HKLM =
    [
        "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run",
        "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\RunOnce",
        "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Policies\\Explorer\\Run",
        "SOFTWARE\\Wow6432Node\\Microsoft\\Windows\\CurrentVersion\\Run",
        "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Explorer\\StartupApproved\\Run",
    ];

    public static StartupEntry[] Find()
    {
        List<StartupEntry> entries = [];
        foreach (var entry in HKCU)
        {
            using var subKey = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Default)
                .OpenSubKey(entry);
            if (subKey is null) continue;
            foreach (var val in subKey.GetValueNames())
            {
                entries.Add(new StartupEntry("Registry", val, Path.Combine(subKey.Name, val)));
            }
            
        }
        foreach (var entry in HKLM)
        {
            using var subKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Default)
                .OpenSubKey(entry);
            if (subKey is null) continue;
            foreach (var val in subKey.GetValueNames())
            {
                entries.Add(new StartupEntry("Registry", val, Path.Combine(subKey.Name, val)));
            }

        }
        return entries.ToArray();
    }
}