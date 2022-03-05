using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using AvaloniaEdit;
using AvaloniaEdit.CodeCompletion;
using AvaloniaEdit.Document;
using AvaloniaEdit.Highlighting;
using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Interactivity;
using Microsoft.CodeAnalysis;
using Avalonia.Threading;
using AvaloniaEdit.Folding;
using ICSharpCode.AvalonEdit.Folding;
using ICSharpCode.SharpDevelop.Editor;
using MASLAB.Services;
using AvaloniaEdit.Utils;
using System.ComponentModel;
using System.Windows.Input;
using MASLAB.Models;
using System.Runtime.InteropServices;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace MASLAB.Views
{
    using Pair = KeyValuePair<int, IControl>;

    /// <summary>
    /// Representa a janela de propriedades de um tanque
    /// </summary>
    public class TankProperties : Window
    {
        /// <summary>
        /// Cria uma nova instância de TankProperties
        /// </summary>
        public TankProperties()
        {
            this.InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif



            _editor = this.FindControl<TextEditor>("Editor");
            _editor.Background = Brushes.Transparent;
            _editor.Options.ConvertTabsToSpaces = true;
            _editor.Options.IndentationSize = 4;
            _editor.ShowLineNumbers = true;
            _editor.SyntaxHighlighting = HighlightingManager.Instance.GetDefinition("C#");
            _editor.TextArea.TextEntered += textEditor_TextArea_TextEntered;
            _editor.TextArea.TextEntering += textEditor_TextArea_TextEntering;
            _editor.TextArea.TextInput += TextArea_TextInput;
            _editor.TextArea.Initialized += (s, a) => AnalyzeCodeSyntax();
            _editor.KeyUp += TextArea_KeyUp;
            _editor.TextArea.IndentationStrategy = new AvaloniaEdit.Indentation.CSharp.CSharpIndentationStrategy();

            _insightWindow = new OverloadInsightWindow(_editor.TextArea);

            _editor.FontFamily = GetPlatformFontFamily();

            _editor.TextArea.PointerMoved += TextArea_PointerMoved;

            foldingManager = FoldingManager.Install(_editor.TextArea);
            foldingStretegy = new BraceFoldingStrategy();

            var textMarkerService = new TextMarkerService(_editor.Document);
            _editor.TextArea.TextView.BackgroundRenderers.Add(textMarkerService);
            _editor.TextArea.TextView.LineTransformers.Add(textMarkerService);
            IServiceContainer services = _editor.Document.GetService<IServiceContainer>();
            if (services != null)
                services.AddService(typeof(ITextMarkerService), textMarkerService);
            this.textMarkerService = textMarkerService;

            

            this.AddHandler(PointerWheelChangedEvent, (o, i) =>
            {
                if (i.KeyModifiers != KeyModifiers.Control) return;
                if (i.Delta.Y > 0) _editor.FontSize++;
                else _editor.FontSize = _editor.FontSize > 1 ? _editor.FontSize - 1 : 1;
            }, RoutingStrategies.Bubble, true);

            codeService = CodeAnalysisService.LoadDocument(_editor.Document.Text);

            UndoCommand = new CommandAdapter(true) { Action = (p) => _editor.Undo() };
            RedoCommand = new CommandAdapter(true) { Action = (p) => _editor.Redo() };
        }

        private static string GetPlatformFontFamily()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return "Consolas";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return "Menlo";
            }
            else
            {
                return "Monospace";
            }
        }


        /// <summary>
        /// Comando de Desfazer
        /// </summary>
        public ICommand UndoCommand { get; set; }

        /// <summary>
        /// Comando de Refazer
        /// </summary>
        public ICommand RedoCommand { get; set; }



        private async void TextArea_PointerMoved(object sender, PointerEventArgs e)
        {
            var position = e.GetPosition(_editor.TextArea.TextView) + _editor.TextArea.TextView.ScrollOffset;
            var pos = _editor.TextArea.TextView.GetPositionFloor(position);
            bool inDocument = pos.HasValue;
            if (inDocument && !pos.Value.IsAtEndOfLine)
            {
                TextLocation logicalPosition = pos.Value.Location;
                int offset = _editor.Document.GetOffset(logicalPosition);

                var markersAtOffset = textMarkerService.GetMarkersAtOffset(offset);
                ITextMarker markerWithToolTip = markersAtOffset.FirstOrDefault(marker => marker.ToolTip != null);

                if (markerWithToolTip != null)
                {
                    if (toolTip == null)
                    {                        
                        ToolTip.SetTip(_editor, markerWithToolTip.ToolTip);
                        ToolTip.SetIsOpen(_editor, true);
                        return;
                    }
                }

                var info = await codeService.GetInfoAt(offset);
                if(info != null)
                {                    
                    var tip = ToolTip.GetTip(_editor);
                    if (tip != null && tip.ToString() != info)
                    {
                        ToolTip.SetIsOpen(_editor, false);
                    }

                    ToolTip.SetTip(_editor, info);
                    ToolTip.SetIsOpen(_editor, true);
                    return;
                }
            }
            else
            {
                ToolTip.SetTip(_editor, null);
                ToolTip.SetIsOpen(_editor, false);
            }

        }

        private TextEditor _editor;
        private CompletionWindow _completionWindow;
        private FoldingManager foldingManager;
        private BraceFoldingStrategy foldingStretegy;
        private ITextMarkerService textMarkerService;
        private CodeAnalysisService codeService;

#pragma warning disable IDE0044 // Adicionar modificador somente leitura
        private OverloadInsightWindow _insightWindow;
        private ToolTip toolTip = null;
#pragma warning restore IDE0044 // Adicionar modificador somente leitura








        private DispatcherTimer dispatcherTimer;

        private void InitializeComponent()
        {
            Directory.SetCurrentDirectory(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location));
            AvaloniaXamlLoader.Load(this);

            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Interval = TimeSpan.FromSeconds(2);
            dispatcherTimer.Tick += (s, a) => AnalyzeCodeSyntax();
        }


        private async void AnalyzeCodeSyntax()
        {
            dispatcherTimer.Stop();

            try
            {
                foldingStretegy.UpdateFoldings(foldingManager, _editor.Document);

                var errorService = ErrorService.GetService();
                errorService.Clear();


                var d = await CodeAnalysisService.LoadDocument(_editor.Document.Text).GetDiagnosticsAsync();

                var s = d.Select(x =>
                {
                    var cd = new CompilationDiagnostic(x);
                    var line = _editor.Document.GetLineByOffset(x.Location.SourceSpan.Start);
                    cd.Line = line.LineNumber;
                    cd.Column = line.Length;
                    return cd;
                });

                errorService.AddRange(s);

                textMarkerService.RemoveAll(m => true);

                foreach (var item in d)
                {
                    var span = item.Location.SourceSpan;
                    ITextMarker m = textMarkerService.Create(span.Start, span.Length);
                    m.MarkerTypes = TextMarkerTypes.SquigglyUnderline;
                    m.MarkerColor = item.Severity == DiagnosticSeverity.Error ? Colors.Red : Colors.LightGreen;
                    m.ToolTip = item.ToString();
                }
            }
            catch { }
        }

        void textEditor_TextArea_TextEntering(object sender, TextInputEventArgs e)
        {
            switch (e.Text)
            {
                case "\n":                    

                    var ln = _editor.Document.GetLineByOffset(_editor.CaretOffset);
                    var bline = _editor.Document.GetText(ln.Offset, ln.EndOffset - ln.Offset);

                    if (bline.EndsWith("{}"))
                    {
                        e.Handled = true;

                        string tb = string.Empty;
                        for (int i = ln.Offset; i < ln.EndOffset; i++)
                        {
                            if (_editor.Document.Text[i] == 9)
                                tb += _editor.Document.Text[i];
                        }
                        var offset = _editor.CaretOffset;
                        int oc = tb.Length;
                        
                        _editor.Document.Insert(offset, $"{Environment.NewLine}{tb}\t{Environment.NewLine}{tb}");

                        _editor.CaretOffset = offset + (tb.Length > 1 ? oc : 1) + 3;
                    }
                    break;

                case ")":
                    _insightWindow?.Close();
                    break;
                case "{":
                    e.Handled = true;
                    _editor.Document.Insert(_editor.CaretOffset, "{}");
                    _editor.CaretOffset--;
                    break;
                case "[":
                    e.Handled = true;
                    _editor.Document.Insert(_editor.CaretOffset, "[]");
                    _editor.CaretOffset--;
                    break;
                case "'":
                    e.Handled = true;
                    _editor.Document.Insert(_editor.CaretOffset, "''");
                    _editor.CaretOffset--;
                    break;
                case "\"":
                    e.Handled = true;
                    _editor.Document.Insert(_editor.CaretOffset, "\"\"");
                    _editor.CaretOffset--;
                    break;
                case "/":
                    if (_editor.CaretOffset > 1)
                    {
                        if (_editor.Document.Text[_editor.CaretOffset - 1] == '/' && _editor.Document.Text[_editor.CaretOffset - 2] == '/')
                        {
                            var line = _editor.Document.GetLineByOffset(_editor.CaretOffset);
                            int offs = _editor.Document.GetText(line.Offset, line.Length).IndexOf('/');
                            string tab = "";
                            for (int i = line.Offset; i < line.Offset + offs; i++)
                            {
                                if (char.IsLetterOrDigit(_editor.Document.Text[i])) return;

                                tab += _editor.Document.Text[i];
                            }

                            e.Handled = true;
                            int lOffset = _editor.CaretOffset;
                            string snippet = $"/<summary>\n{tab}/// \n{tab}///</summary>";
                            _editor.Document.Insert(_editor.CaretOffset, snippet);
                            _editor.CaretOffset = lOffset + snippet.Length / 2 + 1;
                        }
                    }
                    break;
            }

            if (e.Text.Length > 0 && _completionWindow != null)
            {
                if (!char.IsLetterOrDigit(e.Text[0]))
                {
                    // Whenever a non-letter is typed while the completion window is open,
                    // insert the currently selected element.
                    _completionWindow.CompletionList.RequestInsertion(e);
                }
            }

            //_insightWindow?.Hide();

            // Do not set e.Handled=true.
            // We still want to insert the character that was typed. 
        }

        private void TextArea_TextInput(object sender, TextInputEventArgs e)
        {


        }

        private async void TextArea_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyModifiers == KeyModifiers.Control && e.Key == Key.Space)
            {
                e.Handled = true;

                _editor.Undo();

                _completionWindow = new CompletionWindow(_editor.TextArea);
                _completionWindow.Closed += (o, args) => _completionWindow = null;

                var data = await codeService.GetCompletitionDataAt(_editor.CaretOffset);

                if (data.Count == 0 || _completionWindow == null) return;

                foreach (var d in data) _completionWindow.CompletionList.CompletionData.Add(d);

                _completionWindow.Show();
            }

            dispatcherTimer.Stop();
            dispatcherTimer.Start();
        }

        async void textEditor_TextArea_TextEntered(object sender, TextInputEventArgs e)
        {
            try
            {
                codeService = CodeAnalysisService.LoadDocument(_editor.Document.Text);

                if (e.Text == "(")
                {
                    var p = await codeService.GetMethodSignature(_editor.CaretOffset -2);
                    if (p != null)
                    {

                        _insightWindow = new OverloadInsightWindow(_editor.TextArea);
                        _insightWindow.Closed += (s, a) => _insightWindow = null;

                        _insightWindow.Provider = new OverloadProvider(p);
                        _insightWindow.Show();
                        return;
                    }
                }
                if (await codeService.ShouldTriggerCompletion(_editor.CaretOffset))
                {
                    _completionWindow = new CompletionWindow(_editor.TextArea);
                    _completionWindow.Closed += (o, args) => _completionWindow = null;

                    var data = await CodeAnalysisService.LoadDocument(_editor.Document.Text).GetCompletitionDataAt(_editor.CaretOffset);

                    if (data.Count == 0 || _completionWindow == null) return;

                    foreach (var d in data) _completionWindow.CompletionList.CompletionData.Add(d);

                    _completionWindow.StartOffset -= 1;

                    _completionWindow.Show();

                }
            }
            catch { }

        }



        MASL.Controls.DataModel.Tank tank;

        /// <summary>
        /// Atribui o tanque associado ao controle
        /// </summary>
        /// <param name="t">Modelo do tanque da simulação</param>
        /// <returns>Instância de TankProperties</returns>
        public TankProperties SetTank(MASL.Controls.DataModel.Tank t)
        {
            tank = t;
            _editor.Document.Text = t.SimulationCode;
            return this;
        }

        /// <summary>
        /// Método chamado quando a janela está se fechando
        /// </summary>
        /// <param name="e">Parâmetros do evento</param>
        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);

            tank.SimulationCode = _editor.Document.Text;
        }
    }

    internal class OverloadProvider : IOverloadProvider
    {
        private readonly IList<(string header, string content)> _items;
        private int _selectedIndex;

        public OverloadProvider(IList<(string header, string content)> items)
        {
            _items = items;
            SelectedIndex = 0;
        }

        public int SelectedIndex
        {
            get => _selectedIndex;
            set
            {
                _selectedIndex = value;
                OnPropertyChanged();
                // ReSharper disable ExplicitCallerInfoArgument
                OnPropertyChanged(nameof(CurrentHeader));
                OnPropertyChanged(nameof(CurrentContent));
                // ReSharper restore ExplicitCallerInfoArgument
            }
        }

        public int Count => _items.Count;
        public string CurrentIndexText => null;
        public object CurrentHeader => _items[SelectedIndex].header;
        public object CurrentContent => _items[SelectedIndex].content;

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    
}
