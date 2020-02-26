using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Markup.Xaml;
using System;
using System.Reflection;

namespace MASLAB.Views
{
    public class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
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
