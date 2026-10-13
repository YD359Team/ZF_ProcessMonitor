namespace ZF_ProcessMonitor.Models;

public class ProcessData
{
    public ProcessData(int processId, string processName, long usedRam, string x64, string mainWindowTitle, nint mainWindowHandle)
    {
        ProcessId = processId;
        ProcessName = processName;
        UsedRam = usedRam;
        X64 = x64;
        MainWindowTitle = mainWindowTitle;
        MainWindowHandle = mainWindowHandle;
    }

    public int ProcessId { get; set; }
    public string ProcessName { get; set; }
    public long UsedRam { get; set; }
    public string X64 { get; set; }
    public string MainWindowTitle { get; set; }
    public nint MainWindowHandle { get; set; }
}
