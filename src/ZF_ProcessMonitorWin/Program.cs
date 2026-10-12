using ZeppelinForms;
using ZeppelinForms.Theming;
using ZeppelinForms.Windows;
using ZF_ProcessMonitor;

App app = new(new WindowsPlatform())
{
    MainForm = new MainWindow(),
    StartupTheme = Themes.FluentLight
};
app.Run();
