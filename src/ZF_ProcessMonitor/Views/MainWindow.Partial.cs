using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using ZeppelinForms;
using ZeppelinForms.Core.Collections;
using ZeppelinForms.Drawing;
using ZeppelinForms.Drawing.Primitives;
using ZeppelinForms.Forms.Controls;
using ZeppelinForms.Forms.Controls.Base;
using ZeppelinForms.Forms.Controls.DataGrid;
using ZeppelinForms.Forms.Controls.Text;
using ZeppelinForms.Forms.Enums;
using ZeppelinForms.Forms.Styling;
using ZF_ProcessMonitor.Logic;
using ZF_ProcessMonitor.Models;

namespace ZF_ProcessMonitor;

public partial class MainWindow
{
    private System.Timers.Timer _timer;
    private DataGridView _gridProcesses;
    private DataGridView _gridStartup;

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
            x.HorizontalAlignment = HorizontalAlignment.Left;
            x.Font = ZeppelinForms.Drawing.Font.Default.WithSize(16f);
            x.Padding = new(2f);
            x.Docking = Dock.Top;
        });
        LinkLabel bottomLabel = new LinkLabel().With(x =>
        {
            x.Text = "Powered by ZeppelinForms";
            x.Url = "https://github.com/YD359Team/ZeppelinForms-solution";
            x.Click += (_, _) => Process.Start(new ProcessStartInfo(x.Url) { UseShellExecute = true });
            x.HorizontalAlignment = HorizontalAlignment.Center;
            x.VerticalAlignment = VerticalAlignment.Center;
            x.Padding = new(2f);
            x.Docking = Dock.Bottom;
        });
        SplitView splitView = new SplitView().With(x =>
        {
            x.DisplayMode = SplitViewDisplayMode.Inline;
            x.IsPaneOpen = true;
            x.Docking = Dock.Fill;
        });

        SetupGridProcesses();
        SetupGridStartup();
        SplitViewItem processView = new SplitViewItem("Processes",
            icon: IconSource.FromPath("M3 4h18v16H3z M7 15l3-4 3 3 4-6", strokeWidth: 1.5f),
            content: () => _gridProcesses);
        SplitViewItem startupView = new SplitViewItem("Startup",
            icon: IconSource.FromPath("M6 4l13 8-13 8z", strokeWidth: 1.5f),
            content: () => _gridStartup);
        splitView.Items.AddRange([processView, startupView]);
        rootPanel.Children.AddRange([topLabel, bottomLabel, splitView]);
        this.Content = rootPanel;
        App.Styles.Load("Styles/GridStyles.zss");
    }

    private void SetupGridProcesses()
    {
        _gridProcesses = new DataGridView();
        _gridProcesses.Columns.AddRange(
            new DataGridViewColumn() { Value = (x) => ((ProcessData)x).ProcessId, Header = nameof(ProcessData.ProcessId) },
            new DataGridViewColumn() { Value = (x) => ((ProcessData)x).ProcessName, Header = nameof(ProcessData.ProcessName) },
            new DataGridViewColumn() { Value = (x) => StringHelpers.RamToStr(((ProcessData)x).UsedRam), Header = nameof(ProcessData.UsedRam) },
            new DataGridViewColumn() { Value = (x) => ((ProcessData)x).X64, Header = nameof(ProcessData.X64) },
            new DataGridViewColumn() { Value = (x) => ((ProcessData)x).MainWindowTitle, Header = nameof(ProcessData.MainWindowTitle) },
            new DataGridViewColumn() { Value = (x) => ((ProcessData)x).MainWindowHandle, Header = nameof(ProcessData.MainWindowHandle) }
        );
    }

    private void SetupGridStartup()
    {
        _gridStartup = new DataGridView();
        _gridStartup.Columns.AddRange(
            new DataGridViewColumn() { Value = (x) => ((StartupEntry)x).Source, Header = nameof(StartupEntry.Source), },
            new DataGridViewColumn() { Value = (x) => ((StartupEntry)x).Name, Header = nameof(StartupEntry.Name) },
            new DataGridViewColumn() { Value = (x) => ((StartupEntry)x).FullPath, Header = nameof(StartupEntry.FullPath) }
        );
    }
}
