using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using MASLAB.ViewModels;

namespace MASLAB.Views
{
    public class SimulationSettings : Window
    {
        public SimulationSettings()
        {
            this.InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
        }

        public SimulationSettingsViewModel ViewModel { get; set; } = new SimulationSettingsViewModel();

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
