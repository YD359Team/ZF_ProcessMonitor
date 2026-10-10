using System.Diagnostics;
using ZeppelinForms.Core.Collections;
using ZeppelinForms.Drawing.Primitives;
using ZeppelinForms.Forms;
using ZeppelinForms.Forms.Controls;
using ZeppelinForms.Forms.Controls.Base;
using ZeppelinForms.Forms.Controls.DataGrid;
using ZeppelinForms.Forms.Controls.Text;
using ZeppelinForms.Forms.Enums;

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
        _dataGridView.Items.AddRange(Process.GetProcesses()
            .Select(p => new ProcessData(
                p.Id, 
                p.ProcessName, 
                p.WorkingSet64,
                p.MainWindowTitle, 
                p.MainWindowHandle)));
    }

    private void InitializeComponents()
    {
        this.Title = "ZF Process Monitor";
        this.Size = new(1024, 768);
        this.WindowStartupLocation = WindowStartupLocation.CenterScreen;

        DockPanel rootPanel = new DockPanel().With(x =>
        {
            x.Padding = new(4f, 2f);
        });
        Label topLabel = new Label().With(x => {
            x.Text = "[ZF] ProcessMonitor";
            x.Font = ZeppelinForms.Drawing.Font.Default.WithSize(16f);
            x.Padding = new(2f);
            x.Docking = Dock.Top;
        });
        Label bottomLabel = new Label().With(x =>
        {
            x.Text = "Powered by ZeppelinForms";
            x.HorizontalAlignment = HorizontalAlignment.Center;
            x.VerticalAlignment = VerticalAlignment.Center;
            x.Padding = new(2f);
            x.Docking = Dock.Bottom;
        });
        _dataGridView = new DataGridView().With(x =>
        {
            x.BorderWidth = 2f;
            x.BorderColor = Colors.Gray;
            x.Docking = Dock.Fill;
        });
        _dataGridView.Columns.AddRange(
            new DataGridViewColumn() { Value = (x) => ((ProcessData)x).ProcessId, Header = nameof(ProcessData.ProcessId) },    
            new DataGridViewColumn() { Value = (x) => ((ProcessData)x).ProcessName, Header = nameof(ProcessData.ProcessName) },
            new DataGridViewColumn() { Value = (x) => ((ProcessData)x).UsedRam, Header = nameof(ProcessData.UsedRam) },
            new DataGridViewColumn() { Value = (x) => ((ProcessData)x).MainWindowTitle, Header = nameof(ProcessData.MainWindowTitle) },
            new DataGridViewColumn() { Value = (x) => ((ProcessData)x).MainWindowHandle, Header = nameof(ProcessData.MainWindowHandle) }
        );
        rootPanel.Children.AddRange([topLabel, bottomLabel, _dataGridView]);
        this.Content = rootPanel;
    }
}

public sealed record class ProcessData
{
    public ProcessData(
        int processId, 
        string processName, 
        long usedRam, 
        string mainWindowTitle,
        nint mainWindowHandle)
    {
        ProcessId = processId;
        ProcessName = processName;
        UsedRam = usedRam;
        MainWindowTitle = mainWindowTitle;
        MainWindowHandle = mainWindowHandle;
    }

    public int ProcessId { get; }
    public string ProcessName { get; }
    public long UsedRam { get; }
    public string MainWindowTitle { get; }
    public nint MainWindowHandle { get; }
}
