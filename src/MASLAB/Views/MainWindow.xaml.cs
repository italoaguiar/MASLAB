using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Markup.Xaml;
using MASLAB.Services;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace MASLAB.Views
{
    /// <summary>
    /// Janela principal do software
    /// </summary>
    public class MainWindow : Window
    {
        /// <summary>
        /// Cria uma nova instância de MainWindow
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
            bool show = false;
            this.Activated += async (s, a) =>
            {
                await Task.Delay(500);
                this.FindControl<Popup>("popup").IsOpen = !show;
                show = true;
            };

            this.KeyDown += (s, a) =>
            {
                if (a.Key == Avalonia.Input.Key.Escape)
                    ConnectionHelper.Cancel();
            };

            this.PointerPressed += (s, a) =>
            {
#pragma warning disable CS0618 // O tipo ou membro é obsoleto
                if (a.MouseButton == Avalonia.Input.MouseButton.Right)
#pragma warning restore CS0618 // O tipo ou membro é obsoleto
                    ConnectionHelper.Cancel();
            };
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this); 
        }
    }
}
