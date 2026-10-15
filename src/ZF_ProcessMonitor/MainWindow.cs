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
using ZF_ProcessMonitor.Win32API;

namespace ZF_ProcessMonitor;

public partial class MainWindow : Form
{
    private DataGridView _dataGridView;

    private int _frame = 1;
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
        _timer = new(TimeSpan.FromSeconds(1));
        _timer.Elapsed += (_, _) => UpdateProcesses();
        _timer.Start();
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
        _dataGridView.Items.Add(procData);
        _processData.Add(proc.Id, procData);
    }

    private void UpdateProcesses()
    {
        _frame++;
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

        foreach (var procData in _processData.Values.Where(x => x.Frame < _frame))
        {
            _dataGridView.Items.Remove(procData);
            _processData.Remove(procData.ProcessId);
        }
    }

    private string RamToStr(long ramBytes)
    {
        const int KB = 1024;
        const int MB = KB * 1024;
        const int GB = MB * 1024;

        if (ramBytes < KB) return $"{ramBytes} B";
        if (ramBytes < MB)
        {
            long kb = ramBytes / KB;
            return $"{kb}.{ramBytes % KB} KB";
        }
        if (ramBytes < GB)
        {
            long mb = ramBytes / MB;
            return $"{mb}.{(ramBytes / KB) % MB} MB";
        }

        long gb = ramBytes / GB;
        return $"{gb}.{(ramBytes / MB) % GB} GB";
    }

    protected override void OnClosed(EventArgs e)
    {
        _timer?.Dispose();
        base.OnClosed(e);
    }
}