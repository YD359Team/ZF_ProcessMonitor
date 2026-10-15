namespace ZF_ProcessMonitor.Models;

public class StartupEntry
{
    public StartupEntry(string source, string name)
    {
        Source = source;
        Name = name;
    }

    public string Source { get; set; }
    public string Name { get; set; }
}
