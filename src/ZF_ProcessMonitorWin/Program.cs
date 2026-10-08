using ZeppelinForms;
using ZeppelinForms.Windows;
using ZF_ProcessMonitor;

App app = new(new WindowsPlatform())
{
    MainForm = new MainWindow()
};
app.Run();
