using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System.Threading.Tasks;

namespace MASLAB.Views
{
    /// <summary>
    /// Representa uma janela de diálogo
    /// </summary>
    public class MessageBox : Window
    {
        /// <summary>
        /// Cria uma nova instância de MessageBox
        /// </summary>
        public MessageBox()
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

        /// <summary>
        /// Representa os botões do diálogo MessageBox
        /// </summary>
        public enum MessageBoxButtons
        {
            /// <summary>
            /// Ok
            /// </summary>
            Ok,

            /// <summary>
            /// Ok e Cancelar
            /// </summary>
            OkCancel,

            /// <summary>
            /// Sim e não
            /// </summary>
            YesNo,

            /// <summary>
            /// Sim, não e cancelar
            /// </summary>
            YesNoCancel
        }

        /// <summary>
        /// Resultado do diálogo MessageBox
        /// </summary>
        public enum MessageBoxResult
        {
            /// <summary>
            /// Ok
            /// </summary>
            Ok,

            /// <summary>
            /// Cancelado
            /// </summary>
            Cancel,

            /// <summary>
            /// Sim
            /// </summary>
            Yes,

            /// <summary>
            /// Não
            /// </summary>
            No
        }

        /// <summary>
        /// Exibe a janela de diálogo
        /// </summary>
        /// <param name="text">Texto a ser exibido</param>
        /// <param name="title">Título da janela</param>
        /// <param name="buttons">Botões do diálogo</param>
        /// <returns>Botão clicado pelo usuário</returns>
        public static Task<MessageBoxResult> Show(string text, string title = "", MessageBoxButtons buttons = MessageBoxButtons.Ok)
        {
            var msgbox = new MessageBox()
            {
                Title = title
            };
            msgbox.FindControl<TextBlock>("Text").Text = text;
            var buttonPanel = msgbox.FindControl<StackPanel>("Buttons");

            var res = MessageBoxResult.Ok;

            void AddButton(string caption, MessageBoxResult r, bool def = false)
            {
                var btn = new Button { Content = caption };
                btn.Click += (_, __) => {
                    res = r;
                    msgbox.Close();
                };
                buttonPanel.Children.Add(btn);
                if (def)
                    res = r;
            }

            if (buttons == MessageBoxButtons.Ok || buttons == MessageBoxButtons.OkCancel)
                AddButton("Ok", MessageBoxResult.Ok, true);
            if (buttons == MessageBoxButtons.YesNo || buttons == MessageBoxButtons.YesNoCancel)
            {
                AddButton("Yes", MessageBoxResult.Yes);
                AddButton("No", MessageBoxResult.No, true);
            }

            if (buttons == MessageBoxButtons.OkCancel || buttons == MessageBoxButtons.YesNoCancel)
                AddButton("Cancel", MessageBoxResult.Cancel, true);


            var tcs = new TaskCompletionSource<MessageBoxResult>();
            msgbox.Closed += delegate { tcs.TrySetResult(res); };
            msgbox.ShowDialog(App.MainWindow);
            return tcs.Task;
        }
    }
}
