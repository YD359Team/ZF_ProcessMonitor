using System.Diagnostics;
using ZeppelinForms.Core.Collections;
using ZeppelinForms.Drawing.Primitives;
using ZeppelinForms.Forms;
using ZeppelinForms.Forms.Controls;
using ZeppelinForms.Forms.Controls.Base;
using ZeppelinForms.Forms.Controls.DataGrid;
using ZeppelinForms.Forms.Controls.Text;
using ZeppelinForms.Forms.Enums;
using ZF_ProcessMonitor.Models;
using ZF_ProcessMonitor.Startup;
using ZF_ProcessMonitor.Win32API;

namespace ZF_ProcessMonitor;

public partial class MainWindow : Form
{
    private long _frame = DateTime.UtcNow.Ticks;
    private System.Timers.Timer _timer;
    private readonly Dictionary<int, ProcessData> _processData = new(100);

    public MainWindow()
    {
        InitializeComponents();
        this.Shown += MainWindow_Shown;
    }

    private void MainWindow_Shown(object? sender, EventArgs e)
    {
        FirstProcessFind();
        UpdateStartups();
        _timer = new(TimeSpan.FromSeconds(1));
        _timer.Elapsed += (_, _) => UpdateProcesses();
        _timer.Start();
    }

    private void UpdateStartups()
    {
        this.Invoke(() =>
        {
            _gridStartup.Items.AddRange(RegistrySearch.Find());
            _gridStartup.Items.AddRange(AppSearch.Find());
        });
    }

    private void FirstProcessFind()
    {
        foreach (var proc in Process.GetProcesses())
        {
            RegisterProcess(proc);
        }
    }

    private void RegisterProcess(Process proc)
    {
        string x64str;
        try
        {
            _ = Kernel32.IsWow64Process(proc.Handle, out bool x64);
            x64str = x64 ? "x64" : "x32";
        }
        catch
        {
            x64str = "N/A";
        }
        ProcessData procData = new ProcessData(
            proc.Id,
            proc.ProcessName,
            proc.WorkingSet64,
            x64str,
            proc.MainWindowTitle,
            proc.MainWindowHandle,
            _frame);
        this.Invoke(() =>
        {
            _gridProcesses.Items.Add(procData);
            _processData.Add(proc.Id, procData);
        });
    }

    private void UpdateProcesses()
    {
        _frame = DateTime.UtcNow.Ticks;
        foreach (var proc in Process.GetProcesses())
        {
            if (_processData.TryGetValue(proc.Id, out ProcessData? stored))
            {
                stored.UsedRam = proc.WorkingSet64;
                stored.Frame = _frame;
                continue;
            }

            RegisterProcess(proc);
        }

        var tempList = _processData.Values.Where(x => x.Frame < _frame).ToList();
        foreach (var procData in tempList)
        {
            this.Invoke(() =>
            {
                _gridProcesses.Items.Remove(procData);
                _processData.Remove(procData.ProcessId);
            });
        }
    }

    private string RamToStr(long ramBytes)
    {
        const double KB = 1024;
        const double MB = KB * 1024;
        const double GB = MB * 1024;

        if (ramBytes < KB) return $"{ramBytes} B";
        if (ramBytes < MB)
        {
            return $"{ramBytes / KB:F2} KB";
        }
        if (ramBytes < GB)
        {
            return $"{ramBytes / MB:F2} MB";
        }

        return $"{ramBytes / GB:F2} GB";
    }

    protected override void OnClosed(EventArgs e)
    {
        _timer?.Dispose();
        base.OnClosed(e);
    }
}