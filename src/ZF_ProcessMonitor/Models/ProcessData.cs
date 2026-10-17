namespace ZF_ProcessMonitor.Models;

public class ProcessData : IEquatable<ProcessData>
{
    public ProcessData(int processId, string processName, long usedRam, string x64, string mainWindowTitle, nint mainWindowHandle, long frame)
    {
        ProcessId = processId;
        ProcessName = processName;
        UsedRam = usedRam;
        X64 = x64;
        MainWindowTitle = mainWindowTitle;
        MainWindowHandle = mainWindowHandle;
        Frame = frame;
    }

    public int ProcessId { get; set; }
    public string ProcessName { get; set; }
    public long UsedRam { get; set; }
    public string X64 { get; set; }
    public string MainWindowTitle { get; set; }
    public nint MainWindowHandle { get; set; }
    public long Frame { get; set; }

    public override int GetHashCode()
    {
        return ProcessId.GetHashCode();
    }

    public bool Equals(ProcessData? other)
    {
        return ReferenceEquals(this, other);
    }
}
