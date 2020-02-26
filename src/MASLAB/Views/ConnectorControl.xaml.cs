using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace MASLAB.Views
{
    public class ConnectorControl : UserControl
    {
        public ConnectorControl()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        protected override void OnTemplateApplied(TemplateAppliedEventArgs e)
        {
            base.OnTemplateApplied(e);

            if(ParentWindow != null)
            {
                ParentWindow.PointerPressed += (s, a) => _PointerPressed(a);
                ParentWindow.PointerMoved += (s, a) => _PointerMoved(a);
            }
        }

        public static readonly AvaloniaProperty<IList<Point>> PointsProperty =
            AvaloniaProperty.Register<ConnectorControl, IList<Point>>("Points", new List<Point>());

        public IList<Point> Points
        {
            get { return this.GetValue(PointsProperty); }
            set { this.SetValue(PointsProperty, value); }
        }


        public static readonly AvaloniaProperty<IControl> ParentWindowProperty =
            AvaloniaProperty.Register<ConnectorControl, IControl>("ParentWindow");

        public IControl ParentWindow
        {
            get => GetValue(ParentWindowProperty);
            set => SetValue(ParentWindowProperty, value);
        }

        public static readonly AvaloniaProperty<bool> IsLinkEnabledProperty =
            AvaloniaProperty.Register<ConnectorControl, bool>("IsLinkEnabled", false);

        public bool IsLinkEnabled
        {
            get => GetValue(IsLinkEnabledProperty);
            set => SetValue(IsLinkEnabledProperty, value);
        }

        private void _PointerPressed(PointerPressedEventArgs e)
        {
            base.OnPointerPressed(e);

            if (IsLinkEnabled)
            {
                var p = e.GetPosition(this);

                var x = (int)(Math.Round(p.X + 7) / 15) * 15;
                var y = (int)(Math.Round(p.Y + 7) / 15) * 15;


                Points.Add(new Point(x, y));

                Points = new List<Point>(Points);
            }           
        }

        

        private void _PointerMoved(PointerEventArgs e)
        {
            base.OnPointerMoved(e);

            if (IsLinkEnabled && Points.Count > 1)
            {
                var point = Points[Points.Count -1];
                var lp = Points[Points.Count - 2];

                var p = e.GetPosition(this);

                var x = (int)(Math.Round(p.X + 7) / 15) * 15;
                var y = (int)(Math.Round(p.Y + 7) / 15) * 15;


                if (Math.Abs(x - lp.X) >= Math.Abs(y - lp.Y))
                {
                    point = new Point(x, lp.Y);
                }
                else
                {
                    point = new Point(lp.X, y);
                }

                Points[Points.Count - 1] = point;

                Points = new List<Point>(Points);
            }
        }
    }
}
