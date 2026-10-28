using System.Diagnostics;
using ZeppelinForms;
using ZeppelinForms.Core.Collections;
using ZeppelinForms.Data;
using ZeppelinForms.Dispatchers;
using ZeppelinForms.Drawing;
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
    private DispatcherTimer? _timer;
    private DataGridView _gridProcesses = null!;
    private DataGridView _gridStartup = null!;

    /// <summary>The processes as the grid shows them: filtered by the search box.</summary>
    private DataSource<ProcessData> _processView = null!;

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

        UIElement processesPage = SetupGridProcesses();
        SetupGridStartup();
        SplitViewItem processView = new SplitViewItem("Processes",
            icon: IconSource.FromPath("M3 4h18v16H3z M7 15l3-4 3 3 4-6", strokeWidth: 1.5f),
            content: () => processesPage);
        SplitViewItem startupView = new SplitViewItem("Startup",
            icon: IconSource.FromPath("M6 4l13 8-13 8z", strokeWidth: 1.5f),
            content: () => _gridStartup);
        splitView.Items.AddRange([processView, startupView]);
        rootPanel.Children.AddRange([topLabel, bottomLabel, splitView]);
        this.Content = rootPanel;

        // the sheet is copied to Assets/ next to the executable: LoadAsset looks for it there.
        // Styles.Load takes a relative path from the current directory — the sheet wasn't
        // found, and was skipped with only a line in the debugger's output
        App.Styles.Add(StyleSheet.LoadAsset("Styles/GridStyles.zss"));
    }

    /// <summary>The processes grid with a search box over it.</summary>
    private UIElement SetupGridProcesses()
    {
        // typed columns: no casts, and the numbers sort as numbers — the memory column
        // used to be text, and "9.50 MB" sorted after "10.20 GB". The header is the
        // member's name unless one is given
        _gridProcesses = new DataGridView
        {
            SelectionMode = SelectionMode.Extended,   // Ctrl/Shift + click, Ctrl+A, Ctrl+C
            Columns =
            {
                new DataGridViewColumn<ProcessData, int>(x => x.ProcessId) { Width = GridLength.Fixed(90) },
                new DataGridViewColumn<ProcessData, string>(x => x.ProcessName) { Width = GridLength.Star(2) },
                new DataGridViewColumn<ProcessData, long>(x => x.UsedRam)
                {
                    Width = GridLength.Fixed(110),
                    Align = HorizontalContentAlignment.Right,
                    Format = StringHelpers.RamToStr,
                },
                new DataGridViewColumn<ProcessData, string>(x => x.Bitness) { Width = GridLength.Fixed(70) },
                new DataGridViewColumn<ProcessData, string>(x => x.MainWindowTitle) { Width = GridLength.Star(3) },
                new DataGridViewColumn<ProcessData, nint>(x => x.MainWindowHandle) { Width = GridLength.Fixed(130) },
            },
        };

        // the grid shows a view of the collection; the search box filters the view
        _processView = new DataSource<ProcessData>(_processes);
        _gridProcesses.DataSource = _processView;

        TextBox search = new TextBox().With(x =>
        {
            x.Docking = Dock.Top;
            x.Margin = new(0, 0, 0, 4);
            x.Watermark = "Search...";
        });
        search.TextChanged += (_, _) =>
        {
            string text = search.Text ?? string.Empty;

            _processView.Filter = text.Length == 0
                ? null
                : process => process.ProcessName.Contains(text, StringComparison.OrdinalIgnoreCase) ||
                             process.MainWindowTitle.Contains(text, StringComparison.OrdinalIgnoreCase) ||
                             process.ProcessId.ToString().StartsWith(text, StringComparison.Ordinal);
        };

        DockPanel page = new DockPanel();
        page.Children.AddRange([search, _gridProcesses]);

        return page;
    }

    private void SetupGridStartup()
    {
        _gridStartup = new DataGridView
        {
            SelectionMode = SelectionMode.Extended,
            Columns =
            {
                new DataGridViewColumn<StartupEntry, string>(x => x.Source),
                new DataGridViewColumn<StartupEntry, string>(x => x.Name),
                new DataGridViewColumn<StartupEntry, string>(x => x.FullPath) { Width = GridLength.Star(3) },
            },
        };
    }
}
