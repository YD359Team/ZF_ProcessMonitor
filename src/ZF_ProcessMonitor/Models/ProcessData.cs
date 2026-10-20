namespace ZF_ProcessMonitor.Models;

public class ProcessData : IEquatable<ProcessData>
{
    public ProcessData(int processId, string processName, long usedRam, string bitness, string mainWindowTitle, nint mainWindowHandle)
    {
        ProcessId = processId;
        ProcessName = processName;
        UsedRam = usedRam;
        Bitness = bitness;
        MainWindowTitle = mainWindowTitle;
        MainWindowHandle = mainWindowHandle;
    }

    public int ProcessId { get; set; }
    public string ProcessName { get; set; }
    public long UsedRam { get; set; }

    /// <summary>"x64", "x32", or "N/A" when the process can't be opened.</summary>
    public string Bitness { get; set; }

    public string MainWindowTitle { get; set; }
    public nint MainWindowHandle { get; set; }

    public override int GetHashCode()
    {
        return ProcessId.GetHashCode();
    }

    public override bool Equals(object? obj) => Equals(obj as ProcessData);

    public bool Equals(ProcessData? other)
    {
        return other is not null && other.ProcessId == this.ProcessId;
    }
}
