namespace ZF_ProcessMonitor.Models;

public sealed record class ProcessData(
    int ProcessId,
    string ProcessName,
    long UsedRam,
    string MainWindowTitle,
    nint MainWindowHandle
);
