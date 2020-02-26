using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using MASLAB.ViewModels;
using System;

namespace MASLAB.Views
{
    public class Tank : UserControl
    {
        public Tank()
        {
            this.InitializeComponent();

            AddHandler(PointerPressedEvent, PointerPressedHandler, Avalonia.Interactivity.RoutingStrategies.Tunnel);
        }

        private void PointerPressedHandler(object sender, PointerPressedEventArgs e)
        {
            var p = e.GetPosition(ParentContainer);
            var x = (int)(Math.Round(p.X + 7) / 15) * 15;
            var y = (int)(Math.Round(p.Y + 7) / 15) * 15;


            var point = new Point(x, y);

            ViewModel.CurrentPoint = point;
        }

        public TankViewModel ViewModel { get; set; } = new TankViewModel();

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public static readonly AvaloniaProperty<IControl> ParentContainerProperty =
            AvaloniaProperty.Register<Tank, IControl>("ParentContainer", validate:CoerceParentControl);

        private static IControl CoerceParentControl(Tank tank, IControl t)
        {
            tank.ViewModel.ParentContainer = t;
            return t;
        }

        public IControl ParentContainer
        {
            get => GetValue(ParentContainerProperty);
            set
            {
                SetValue(ParentContainerProperty, value);
            }
        }


        public static readonly AvaloniaProperty<MASL.Controls.DataModel.Tank> TankDataProperty =
            AvaloniaProperty.Register<Tank, MASL.Controls.DataModel.Tank>("Tank", validate: CoerceTank);

        private static MASL.Controls.DataModel.Tank CoerceTank(Tank tank, MASL.Controls.DataModel.Tank t)
        {
            tank.ViewModel.Tank = t;
            return t;
        }

        public MASL.Controls.DataModel.Tank TankData
        {
            get => GetValue(TankDataProperty);
            set
            {
                SetValue(TankDataProperty, value);
            }
        }


        public static readonly AvaloniaProperty<double> LevelProperty =
            AvaloniaProperty.Register<Tank, double>("Level", 50);



        public double Level
        {
            get => GetValue(LevelProperty);
            set
            {
                SetValue(LevelProperty, value);
            }
        }


    }
}
