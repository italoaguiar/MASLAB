using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.VisualTree;
using MASLAB.ViewModels;

namespace MASLAB.Views
{
    /// <summary>
    /// Representa um controle mola do modelo massa-mola
    /// </summary>
    public class Spring : UserControl
    {
        /// <summary>
        /// Cria uma nova instância de Spring
        /// </summary>
        public Spring()
        {
            ViewModel = new SpringViewModel(this);

            this.InitializeComponent();

            //canvas = this.FindControl<Canvas>("canvas");
        }

        /// <summary>
        /// ViewModel do controle
        /// </summary>
        public SpringViewModel ViewModel { get; set; }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private Point origin = new Point(0,0);
        private bool isPressed = false;

        /// <summary>
        /// Captura os eventos do Ponteiro ao se mover sobre a mola
        /// </summary>
        private void OnPointerMoved(object sender, Avalonia.Input.PointerEventArgs e)
        {
            if (isPressed)
            {
                var p = e.GetPosition(sender as IVisual);
                p = new Point(p.X, p.Y);

                ViewModel.X = p.X + origin.X;
                //ViewModel.Y = p.Y + origin.Y;

                System.Diagnostics.Debug.WriteLine($"{ViewModel.X}; {ViewModel.Y}");
            }
        }

        /// <summary>
        /// Captura os eventos do Ponteiro ao pressionar
        /// </summary>
        public void OnPointerPressed(object sender, Avalonia.Input.PointerPressedEventArgs e)
        {
            //var s = sender as IInputElement;
            //e.Pointer.Capture(s);
            //isPressed = true;
            //var o  = e.GetPosition(s);
            //origin = new Point(o.X, o.Y);

            //MassSpringIncludeWindow msiw = new MassSpringIncludeWindow();
            //msiw.Show();
        }

        /// <summary>
        /// Captura os eventos do Pointeiro ao liberar o cursor 
        /// </summary>
        private void OnPointerReleased(object sender, Avalonia.Input.PointerReleasedEventArgs e)
        {
            //isPressed = false;
        }

        /// <summary>
        /// Captura os eventos do Ponteiro ao entrar na região da mola
        /// </summary>
        public void OnPointerLeave(object sender, Avalonia.Input.PointerEventArgs e)
        {

        }

        /// <summary>
        /// Captura os eventos do Ponteiro ao deixar a região da Mola
        /// </summary>
        public void OnPointerEnter(object sender, Avalonia.Input.PointerEventArgs e)
        {

        }
    }
}
