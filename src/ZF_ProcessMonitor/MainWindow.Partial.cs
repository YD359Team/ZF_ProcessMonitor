using System;
using System.Collections.Generic;
using System.Text;
using ZeppelinForms.Core.Collections;
using ZeppelinForms.Drawing.Primitives;
using ZeppelinForms.Forms.Controls;
using ZeppelinForms.Forms.Controls.Base;
using ZeppelinForms.Forms.Controls.DataGrid;
using ZeppelinForms.Forms.Controls.Text;
using ZeppelinForms.Forms.Enums;
using ZF_ProcessMonitor.Models;

namespace ZF_ProcessMonitor;

public partial class MainWindow
{
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
        SplitView splitView = new SplitView().With(x =>
        {
            x.DisplayMode = SplitViewDisplayMode.Inline;
            x.Docking = Dock.Fill;
        });
        _dataGridView = new DataGridView().With(x =>
        {
            x.BorderWidth = 2f;
            x.BorderColor = Colors.Gray;
            x.Docking = Dock.Fill;
        });
        SplitViewItem processView = new SplitViewItem("Processes", content: () => _dataGridView);
        SplitViewItem startupView = new SplitViewItem("Startup", content: () => _dataGridView);
        splitView.Items.AddRange([processView, startupView]);
        _dataGridView.Columns.AddRange(
            new DataGridViewColumn() { Value = (x) => ((ProcessData)x).ProcessId, Header = nameof(ProcessData.ProcessId) },
            new DataGridViewColumn() { Value = (x) => ((ProcessData)x).ProcessName, Header = nameof(ProcessData.ProcessName) },
            new DataGridViewColumn() { Value = (x) => RamToStr(((ProcessData)x).UsedRam), Header = nameof(ProcessData.UsedRam) },
            new DataGridViewColumn() { Value = (x) => ((ProcessData)x).X64, Header = nameof(ProcessData.X64) },
            new DataGridViewColumn() { Value = (x) => ((ProcessData)x).MainWindowTitle, Header = nameof(ProcessData.MainWindowTitle) },
            new DataGridViewColumn() { Value = (x) => ((ProcessData)x).MainWindowHandle, Header = nameof(ProcessData.MainWindowHandle) }
        );
        rootPanel.Children.AddRange([topLabel, bottomLabel, splitView]);
        this.Content = rootPanel;
    }
}
