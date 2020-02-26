using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using MASLAB.ViewModels;
using MASLAB.Views;
using OxyPlot.Avalonia;

namespace MASLAB
{
    public class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
            OxyPlotModule.EnsureLoaded();
            OxyPlotModule.Initialize();
        }

        public static Window MainWindow { get; set; }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow
                {
                    DataContext = new MainWindowViewModel(),
                };
                MainWindow = desktop.MainWindow;
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}
