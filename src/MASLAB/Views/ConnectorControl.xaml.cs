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
    /// <summary>
    /// Controle de conexão entre tanques
    /// </summary>
    public class ConnectorControl : UserControl
    {
        /// <summary>
        /// Cria uma nova instância de ConnectorControl
        /// </summary>
        public ConnectorControl()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        /// <summary>
        /// Método invocado quando o template XAML é aplicado
        /// </summary>
        /// <param name="e">Argumentos do template</param>
        protected override void OnTemplateApplied(TemplateAppliedEventArgs e)
        {
            base.OnTemplateApplied(e);

            if(ParentWindow != null)
            {
                ParentWindow.PointerPressed += (s, a) => _PointerPressed(a);
                ParentWindow.PointerMoved += (s, a) => _PointerMoved(a);
            }
        }

        /// <summary>
        /// Conjuntos de pontos desenhado
        /// </summary>
        public static readonly AvaloniaProperty<IList<Point>> PointsProperty =
            AvaloniaProperty.Register<ConnectorControl, IList<Point>>("Points", new List<Point>());

        /// <summary>
        /// Conjunto de pontos desenhado
        /// </summary>
        public IList<Point> Points
        {
            get { return this.GetValue(PointsProperty); }
            set { this.SetValue(PointsProperty, value); }
        }

        /// <summary>
        /// Representa a janela pai do controle
        /// </summary>
        public static readonly AvaloniaProperty<IControl> ParentWindowProperty =
            AvaloniaProperty.Register<ConnectorControl, IControl>("ParentWindow");

        /// <summary>
        /// Representa a janela pai do controle
        /// </summary>
        public IControl ParentWindow
        {
            get => GetValue(ParentWindowProperty);
            set => SetValue(ParentWindowProperty, value);
        }

        /// <summary>
        /// Determina se a interação está habilitada
        /// </summary>
        public static readonly AvaloniaProperty<bool> IsLinkEnabledProperty =
            AvaloniaProperty.Register<ConnectorControl, bool>("IsLinkEnabled", false);

        /// <summary>
        /// Determina se a interação está habilitada
        /// </summary>
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

                var point = new Point(x, y);
                var lp = Points[Points.Count - 3];

                Points.Add(point);

                Points = new List<Point>(Points);
            }           
        }

        

        private void _PointerMoved(PointerEventArgs e)
        {
            base.OnPointerMoved(e);

            if (IsLinkEnabled && Points.Count > 1)
            {
                var point = Points[Points.Count -2];
                var lp = Points[Points.Count - 3];

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

                Points[Points.Count - 1] = new Point(p.X > lp.X ? Math.Max(point.X, lp.X + 10) : Math.Min(point.X, lp.X - 10), point.Y);
                Points[Points.Count - 2] = point;

                Points = new List<Point>(Points);
            }
        }
    }
}
