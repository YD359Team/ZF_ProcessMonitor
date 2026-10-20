using System.Collections.ObjectModel;
using System.Diagnostics;
using ZeppelinForms.Dispatchers;
using ZeppelinForms.Forms;
using ZF_ProcessMonitor.Logic.Startup;
using ZF_ProcessMonitor.Logic.Win32API;
using ZF_ProcessMonitor.Models;

namespace ZF_ProcessMonitor;

public partial class MainWindow : Form
{
    /// <summary>The processes the grid shows: a collection it follows through its
    /// DataSource. Changed on the UI thread only — the snapshot is taken on a worker
    /// and applied here.</summary>
    private readonly ObservableCollection<ProcessData> _processes = [];

    private readonly Dictionary<int, ProcessData> _processData = new(100);

    /// <summary>A snapshot is being taken: a slow one is not overlapped by the next tick.</summary>
    private bool _updating;

    public MainWindow()
    {
        InitializeComponents();
        this.Shown += MainWindow_Shown;
    }

    private async void MainWindow_Shown(object? sender, EventArgs e)
    {
        UpdateStartups();

        await UpdateProcessesAsync();

        // ticks on the UI thread: no Invoke, and no tick after the window closed
        _timer = new DispatcherTimer(this) { Interval = TimeSpan.FromSeconds(1) };
        _timer.Tick += async (_, _) => await UpdateProcessesAsync();
        _timer.Start();
    }

    private void UpdateStartups()
    {
        _gridStartup.DataSource = (StartupEntry[])[.. RegistrySearch.Find(), .. AppSearch.Find()];
    }

    private async Task UpdateProcessesAsync()
    {
        if (_updating) return;

        _updating = true;

        try
        {
            // enumerating processes takes milliseconds: off the UI thread
            HashSet<int> known = [.. _processData.Keys];
            List<ProcessSnapshot> snapshot = await Task.Run(() => TakeSnapshot(known));

            Apply(snapshot);
        }
        finally
        {
            _updating = false;
        }
    }

    /// <summary>What a process is now. Read on a worker: Process objects are disposed
    /// here — each holds a handle, and one per process per second added up.</summary>
    private readonly record struct ProcessSnapshot(int Id, string Name, long UsedRam, string? Bitness, string Title, nint Handle);

    private static List<ProcessSnapshot> TakeSnapshot(HashSet<int> known)
    {
        Process[] processes = Process.GetProcesses();
        List<ProcessSnapshot> snapshot = new(processes.Length);

        foreach (Process proc in processes)
        {
            using (proc)
            {
                try
                {
                    // opening a process for its bitness is the costly part: only for new ones
                    string? bitness = known.Contains(proc.Id) ? null : BitnessOf(proc);

                    snapshot.Add(new ProcessSnapshot(
                        proc.Id, proc.ProcessName, proc.WorkingSet64, bitness, proc.MainWindowTitle, proc.MainWindowHandle));
                }
                catch (InvalidOperationException)
                {
                    // exited between the list and the read
                }
            }
        }

        return snapshot;
    }

    private static string BitnessOf(Process proc)
    {
        try
        {
            // IsWow64Process is true for a 32-bit process on a 64-bit Windows:
            // it runs under WOW64. It used to be shown the other way round
            _ = Kernel32.IsWow64Process(proc.Handle, out bool wow64);
            return wow64 || !Environment.Is64BitOperatingSystem ? "x32" : "x64";
        }
        catch
        {
            return "N/A";
        }
    }

    /// <summary>The snapshot into the collection: new processes added, gone ones
    /// removed, the others updated in place.</summary>
    private void Apply(List<ProcessSnapshot> snapshot)
    {
        HashSet<int> alive = new(snapshot.Count);

        foreach (ProcessSnapshot s in snapshot)
        {
            alive.Add(s.Id);

            if (_processData.TryGetValue(s.Id, out ProcessData? stored))
            {
                stored.UsedRam = s.UsedRam;
                stored.MainWindowTitle = s.Title;
                stored.MainWindowHandle = s.Handle;
                continue;
            }

            var data = new ProcessData(s.Id, s.Name, s.UsedRam, s.Bitness ?? "N/A", s.Title, s.Handle);

            _processData.Add(s.Id, data);
            _processes.Add(data);
        }

        for (int i = _processes.Count - 1; i >= 0; i--)
        {
            ProcessData data = _processes[i];

            if (alive.Contains(data.ProcessId)) continue;

            _processes.RemoveAt(i);
            _processData.Remove(data.ProcessId);
        }

        // the records changed in place and don't tell about it: the grid reads them
        // again, and sorts again if it is sorted by memory
        _gridProcesses.Refresh();
    }

    protected override void OnClosed(EventArgs e)
    {
        _timer?.Dispose();
        base.OnClosed(e);
    }
}
