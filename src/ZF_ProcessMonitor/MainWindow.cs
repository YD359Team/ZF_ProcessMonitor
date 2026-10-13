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

    private Dictionary<int, ProcessData> _processData = new(100);

    public MainWindow()
    {
        InitializeComponents();
        this.Shown += MainWindow_Shown;
    }

    private void MainWindow_Shown(object? sender, EventArgs e)
    {
        FirstProcessFind();
        System.Timers.Timer timer = new(TimeSpan.FromSeconds(1));
        timer.Elapsed += (_, _) => UpdateProcesses();
        timer.Start();
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
            proc.MainWindowHandle);
        _dataGridView.Items.Add(procData);
        _processData.Add(proc.Id, procData);
    }

    private void UpdateProcesses()
    {
        foreach (var proc in Process.GetProcesses())
        {
            if (_processData.TryGetValue(proc.Id, out ProcessData? stored))
            {
                stored.UsedRam = proc.WorkingSet64;
                continue;
            }

            RegisterProcess(proc);
        }
    }

    private string RamToStr(long ramBytes)
    {
        const int KB = 1024;
        const int MB = KB * 1024;
        const int GB = MB * 1024;

        if (ramBytes < KB) return $"{ramBytes} b";
        if (ramBytes < MB)
        {
            long kb = ramBytes / KB;
            return $"{kb}.{ramBytes % KB} kb";
        }
        if (ramBytes < GB)
        {
            long mb = ramBytes / MB;
            return $"{mb}.{(ramBytes / KB) % MB} mb";
        }

        long gb = ramBytes / GB;
        return $"{gb}.{(ramBytes / MB) % GB} gb";
    } 
}