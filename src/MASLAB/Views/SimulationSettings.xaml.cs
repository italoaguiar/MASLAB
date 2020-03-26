using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using MASLAB.ViewModels;

namespace MASLAB.Views
{
    /// <summary>
    /// Representa a janela de configurações da simulação
    /// </summary>
    public class SimulationSettings : Window
    {
        /// <summary>
        /// Cria uma nova instância de SimulationSettings
        /// </summary>
        public SimulationSettings()
        {
            this.InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
        }

        /// <summary>
        /// ViewModel de SimulationSettings
        /// </summary>
        public SimulationSettingsViewModel ViewModel { get; set; } = new SimulationSettingsViewModel();

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
