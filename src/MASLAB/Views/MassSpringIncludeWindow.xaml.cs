using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace MASLAB.Views
{
    /// <summary>
    /// Janela de inclusão de componentes do modelo massa-mola
    /// </summary>
    public class MassSpringIncludeWindow : Window
    {
        /// <summary>
        /// Cria uma nova instância de MassSpringIncludeWindow
        /// </summary>
        public MassSpringIncludeWindow()
        {
            this.InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
