using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using MASLAB.ViewModels;
using System;

namespace MASLAB.Views
{
    /// <summary>
    /// Representa um tanque do Diagrama
    /// </summary>
    public class Tank : UserControl
    {
        /// <summary>
        /// Cria uma nova instância de Tank
        /// </summary>
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

        /// <summary>
        /// Determina a ViewModel do Tank
        /// </summary>
        protected TankViewModel ViewModel { get; set; } = new TankViewModel();

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        /// <summary>
        /// Determina do controle Pai de Tank
        /// </summary>
        public static readonly AvaloniaProperty<IControl> ParentContainerProperty =
            AvaloniaProperty.Register<Tank, IControl>("ParentContainer", validate:CoerceParentControl);

        private static IControl CoerceParentControl(Tank tank, IControl t)
        {
            tank.ViewModel.ParentContainer = t;
            return t;
        }


        /// <summary>
        /// Determina do controle Pai de Tank
        /// </summary>
        public IControl ParentContainer
        {
            get => GetValue(ParentContainerProperty);
            set
            {
                SetValue(ParentContainerProperty, value);
            }
        }


        /// <summary>
        /// Representa o modelo de dados de Tank
        /// </summary>
        public static readonly AvaloniaProperty<MASL.Controls.DataModel.Tank> TankDataProperty =
            AvaloniaProperty.Register<Tank, MASL.Controls.DataModel.Tank>("Tank", validate: CoerceTank);

        private static MASL.Controls.DataModel.Tank CoerceTank(Tank tank, MASL.Controls.DataModel.Tank t)
        {
            tank.ViewModel.Tank = t;
            return t;
        }

        /// <summary>
        /// Representa o modelo de dados de Tank
        /// </summary>
        public MASL.Controls.DataModel.Tank TankData
        {
            get => GetValue(TankDataProperty);
            set
            {
                SetValue(TankDataProperty, value);
            }
        }

        /// <summary>
        /// Representa o nível do tanque
        /// </summary>
        public static readonly AvaloniaProperty<double> LevelProperty =
            AvaloniaProperty.Register<Tank, double>("Level", 50);


        /// <summary>
        /// Representa o nível do tanque
        /// </summary>
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
