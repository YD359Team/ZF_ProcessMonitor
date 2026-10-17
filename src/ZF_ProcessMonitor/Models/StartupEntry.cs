namespace ZF_ProcessMonitor.Models;

public class StartupEntry : IEquatable<StartupEntry>
{
    public StartupEntry(string source, string name, string fullPath)
    {
        Source = source;
        Name = name;
        FullPath = fullPath;
    }

    public string Source { get; set; }
    public string Name { get; set; }
    public string FullPath { get; set; }

    public override int GetHashCode()
    {
        return HashCode.Combine(Source, Name, FullPath);
    }

    public bool Equals(StartupEntry? other)
    {
        return ReferenceEquals(this, other);
    }
}
