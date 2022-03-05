﻿/*------------------------------------------------------
 * 3rd Party class developed by wieslawsoltes
 * Project Name: Core2d
 * https://github.com/wieslawsoltes/Core2D
 * -----------------------------------------------------
 */

using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Xaml.Interactivity;
using System;
using System.Collections.Generic;
using System.Text;

namespace MASLAB.Behaviors
{
    /// <summary>
    /// Comportamento de Arrastar
    /// </summary>
    public sealed class DragControlBehavior : Behavior<Control>
    {
        /// <summary>
        /// Controle alvo
        /// </summary>
        public static readonly StyledProperty<Control> TargetControlProperty =
            AvaloniaProperty.Register<DragControlBehavior, Control>(nameof(TargetControl), null);

        private IControl _parent;
        private Point _previous;

        /// <summary>
        /// Controle alvo
        /// </summary>
        public Control TargetControl
        {
            get => GetValue(TargetControlProperty);
            set => SetValue(TargetControlProperty, value);
        }

        /// <summary>
        /// Método invocado ao anexar um comportamento
        /// </summary>
        protected override void OnAttached()
        {
            base.OnAttached();

            var source = AssociatedObject;
            if (AssociatedObject is { })
            {
                source.PointerPressed += Source_PointerPressed;
            }
        }

        /// <summary>
        /// Método invocado ao desanexar um comportamento
        /// </summary>
        protected override void OnDetaching()
        {
            base.OnDetaching();

            var source = AssociatedObject;
            if (source is { })
            {
                source.PointerPressed -= Source_PointerPressed;
            }

            _parent = null;
        }

        private void Source_PointerPressed(object sender, PointerPressedEventArgs e)
        {
            var target = TargetControl ?? AssociatedObject;
            if (target is { })
            {
                _parent = target.Parent;

                if (!(target.RenderTransform is TranslateTransform))
                {
                    target.RenderTransform = new TranslateTransform();
                }

                _previous = e.GetPosition(_parent);
                _parent.PointerMoved += Parent_PointerMoved;
                _parent.PointerReleased += Parent_PointerReleased;
            }
        }

        private void Parent_PointerMoved(object sender, PointerEventArgs args)
        {
            var target = TargetControl ?? AssociatedObject;
            if (target is { })
            {
                var pos = args.GetPosition(_parent);
                if (target.RenderTransform is TranslateTransform tr)
                {
                    tr.X += pos.X - _previous.X;
                    tr.Y += pos.Y - _previous.Y;
                }
                _previous = pos;
            }
        }

        private void Parent_PointerReleased(object sender, PointerReleasedEventArgs e)
        {
            if (_parent is { })
            {
                _parent.PointerMoved -= Parent_PointerMoved;
                _parent.PointerReleased -= Parent_PointerReleased;
                _parent = null;
            }
        }
    }
}
