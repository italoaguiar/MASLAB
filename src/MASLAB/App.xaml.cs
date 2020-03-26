using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using MASLAB.Services;
using MASLAB.ViewModels;
using MASLAB.Views;
using OxyPlot.Avalonia;

namespace MASLAB
{
    /// <summary>
    /// Representa a classe App do software
    /// </summary>
    public class App : Application
    {
        /// <summary>
        /// Inicializa as dependências do software
        /// </summary>
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
            OxyPlotModule.EnsureLoaded();
            OxyPlotModule.Initialize();

            //inicializa os serviços de código
            CodeAnalysisService.LoadDocument("");
        }

        /// <summary>
        /// Janela principal do software
        /// </summary>
        public static Window MainWindow { get; set; }


        /// <summary>
        /// Método invocado na finalização da inicialização do framework
        /// </summary>
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
