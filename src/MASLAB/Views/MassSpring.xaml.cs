using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace MASLAB.Views
{
    /// <summary>
    /// Controle do modelo massa-mola
    /// </summary>
    public class MassSpring : UserControl
    {
        /// <summary>
        /// Cria uma nova instância de MassSpring
        /// </summary>
        public MassSpring()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
