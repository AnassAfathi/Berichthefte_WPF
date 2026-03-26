using System.Configuration;
using System.Data;
using System.Windows;
using Berichthefte_WPF.Helpers;

namespace Berichthefte_WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            ThemeManager.Initialize();
        }
    }
}
