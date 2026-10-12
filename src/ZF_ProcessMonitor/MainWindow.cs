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

namespace ZF_ProcessMonitor;

public partial class MainWindow : Form
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
}