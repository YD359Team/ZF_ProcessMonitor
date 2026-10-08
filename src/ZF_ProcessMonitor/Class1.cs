using System.Diagnostics;
using ZeppelinForms.Core.Collections;
using ZeppelinForms.Forms;
using ZeppelinForms.Forms.Controls;
using ZeppelinForms.Forms.Controls.DataGrid;

namespace ZF_ProcessMonitor;

public class MainWindow : Form
{
    private DataGridView _dataGridView;

    public MainWindow()
    {
        InitializeComponents();
        this.Shown += MainWindow_Shown;
    }

    private void MainWindow_Shown(object? sender, EventArgs e)
    {
        _dataGridView.Items.AddRange(Process.GetProcesses().Select(p => new ProcessData(p.Id, p.ProcessName, p.MainWindowTitle, p.MainWindowHandle)));
    }

    private void InitializeComponents()
    {
        this.Title = "ZF Process Monitor";
        this.Size = new(1024, 768);

        _dataGridView = new();
        _dataGridView.Columns.AddRange(
            new DataGridViewColumn() { Value = (x) => ((ProcessData)x).ProcessId, Header = nameof(ProcessData.ProcessId) },    
            new DataGridViewColumn() { Value = (x) => ((ProcessData)x).ProcessName, Header = nameof(ProcessData.ProcessName) },
            new DataGridViewColumn() { Value = (x) => ((ProcessData)x).MainWindowTitle, Header = nameof(ProcessData.MainWindowTitle) },
            new DataGridViewColumn() { Value = (x) => ((ProcessData)x).MainWindowHandle, Header = nameof(ProcessData.MainWindowHandle) }
        );
        this.Content = _dataGridView;
    }
}

public sealed record class ProcessData
{
    public ProcessData(int processId, string processName, string mainWindowTitle, nint mainWindowHandle)
    {
        ProcessId = processId;
        ProcessName = processName;
        MainWindowTitle = mainWindowTitle;
        MainWindowHandle = mainWindowHandle;
    }

    public int ProcessId { get; }
    public string ProcessName { get; }
    public string MainWindowTitle { get; }
    public nint MainWindowHandle { get; }
}
