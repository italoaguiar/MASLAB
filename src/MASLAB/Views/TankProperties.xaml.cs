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
using ICSharpCode.AvalonEdit.AddIn;
using AvaloniaEdit.Utils;
using System.ComponentModel;
using MASLAB.Services;
using System.Windows.Input;
using MASLAB.Models;
using System.IO;
using System.Reflection;

namespace MASLAB.Views
{
    using Pair = KeyValuePair<int, IControl>;


    public class TankProperties : Window
    {
        public TankProperties()
        {
            this.InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif


            _editor = this.FindControl<TextEditor>("Editor");
            _editor.Background = Brushes.Transparent;
            _editor.ShowLineNumbers = true;
            _editor.SyntaxHighlighting = HighlightingManager.Instance.GetDefinition("C#");
            _editor.TextArea.TextEntered += textEditor_TextArea_TextEntered;
            _editor.TextArea.TextEntering += textEditor_TextArea_TextEntering;
            _editor.TextArea.TextInput += TextArea_TextInput;
            _editor.TextArea.Initialized += (s,a) => AnalyzeCodeSyntax();
            _editor.KeyUp += TextArea_KeyUp;
            _editor.TextArea.IndentationStrategy = new AvaloniaEdit.Indentation.CSharp.CSharpIndentationStrategy();

            
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


            

            UndoCommand = new CommandAdapter(true) { Action = (p) => _editor.Undo() };
            RedoCommand = new CommandAdapter(true) { Action = (p) => _editor.Redo() };
        }


        public ICommand UndoCommand { get; set; }
        public ICommand RedoCommand { get; set; }



        private void TextArea_PointerMoved(object sender, PointerEventArgs e)
        {
            var pos = _editor.TextArea.TextView.GetPositionFloor(e.GetPosition(_editor.TextArea.TextView) + _editor.TextArea.TextView.ScrollOffset);
            bool inDocument = pos.HasValue;
            if (inDocument)
            {
                TextLocation logicalPosition = pos.Value.Location;
                int offset = _editor.Document.GetOffset(logicalPosition);

                var markersAtOffset = textMarkerService.GetMarkersAtOffset(offset);
                ITextMarker markerWithToolTip = markersAtOffset.FirstOrDefault(marker => marker.ToolTip != null);

                if (markerWithToolTip != null)
                {
                    if (toolTip == null)
                    {
                        ToolTip.SetTip(_editor.TextArea, markerWithToolTip.ToolTip);
                        ToolTip.SetIsOpen(_editor.TextArea, true);
                    }
                }
            }
            else
            {
                ToolTip.SetTip(_editor.TextArea, null);
                ToolTip.SetIsOpen(_editor.TextArea, false);
            }
            
        }

        private TextEditor _editor;
        private CompletionWindow _completionWindow;
        private FoldingManager foldingManager;
        private BraceFoldingStrategy foldingStretegy;
        private ITextMarkerService textMarkerService;

#pragma warning disable IDE0044 // Adicionar modificador somente leitura
        private OverloadInsightWindow _insightWindow;
        private ToolTip toolTip;
#pragma warning restore IDE0044 // Adicionar modificador somente leitura
        
        


        MASL.Controls.DataModel.Tank tank;

        public TankProperties SetTank(MASL.Controls.DataModel.Tank t)
        {
            tank = t;
            _editor.Document.Text = t.SimulationCode;
            return this;
        }



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
                    m.MarkerColor = item.Severity == DiagnosticSeverity.Error? Colors.Red : Colors.LightGreen;
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
                    e.Handled = true;

                    var ln = _editor.Document.GetLineByOffset(_editor.CaretOffset);                    

                    string tb = "\n";
                    for (int i = ln.Offset; i < ln.EndOffset; i++)
                    {
                        if (char.IsLetterOrDigit(_editor.Document.Text[i])) break;
                        tb += _editor.Document.Text[i];
                    }
                    var offset = _editor.CaretOffset;
                    int oc = tb.Length;

                    if (offset > 1 && offset < ln.EndOffset)
                    {
                        string lineEnd = $"{_editor.Document.Text[offset - 1]}{_editor.Document.Text[offset]}";

                        if (lineEnd == "{}" || lineEnd == "()" || lineEnd == "[]")
                        {
                            tb = $"{tb}\t{tb}";
                            oc++;
                        }
                    }

                    _editor.Document.Insert(offset, tb);

                    _editor.CaretOffset = offset + (tb.Length > 1 ? oc : 1);
                    break;
                //case "(":
                //    e.Handled = true;
                //    _editor.Document.Insert(_editor.CaretOffset, "()");
                //    _editor.CaretOffset--;
                //    break;
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
                    if(_editor.CaretOffset > 1)
                    {
                        if(_editor.Document.Text[_editor.CaretOffset -1] == '/' && _editor.Document.Text[_editor.CaretOffset - 2] == '/')
                        {
                            var line = _editor.Document.GetLineByOffset(_editor.CaretOffset);
                            int offs = _editor.Document.GetText(line.Offset, line.Length).IndexOf('/');
                            string tab = "";
                            for(int i = line.Offset; i< line.Offset + offs; i++)
                            {
                                if (char.IsLetterOrDigit(_editor.Document.Text[i])) return;

                                tab += _editor.Document.Text[i];
                            }

                            e.Handled = true;
                            int lOffset = _editor.CaretOffset;
                            string snippet = $"/<summary>\n{tab}/// \n{tab}///</summary>";
                            _editor.Document.Insert(_editor.CaretOffset, snippet);
                            _editor.CaretOffset = lOffset + snippet.Length/2 + 1;
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

            _insightWindow?.Hide();

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

                var data = await CodeAnalysisService.LoadDocument(_editor.Document.Text).GetCompletitionDataAt(_editor.CaretOffset);

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
                //if(!char.IsLetterOrDigit(e.Text[0]) && !string.IsNullOrWhiteSpace(e.Text))
                if (await CodeAnalysisService.LoadDocument(_editor.Document.Text).ShouldTriggerCompletion(_editor.CaretOffset))
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

            


            //if (e.Text == ".")
            //{

            //    _completionWindow = new CompletionWindow(_editor.TextArea);
            //    _completionWindow.Closed += (o, args) => _completionWindow = null;

            //    var data = _completionWindow.CompletionList.CompletionData;
            //    data.Add(new MyCompletionData("Item1"));
            //    data.Add(new MyCompletionData("Item2"));
            //    data.Add(new MyCompletionData("Item3"));
            //    data.Add(new MyCompletionData("Item4"));
            //    data.Add(new MyCompletionData("Item5"));
            //    data.Add(new MyCompletionData("Item6"));
            //    data.Add(new MyCompletionData("Item7"));
            //    data.Add(new MyCompletionData("Item8"));
            //    data.Add(new MyCompletionData("Item9"));
            //    data.Add(new MyCompletionData("Item10"));
            //    data.Add(new MyCompletionData("Item11"));
            //    data.Add(new MyCompletionData("Item12"));
            //    data.Add(new MyCompletionData("Item13"));


            //    _completionWindow.Show();
            //}
            //else if (e.Text == "(")
            //{
            //    
            //}
        }

        //private class MyOverloadProvider : IOverloadProvider
        //{
        //    private readonly IList<(string header, string content)> _items;
        //    private int _selectedIndex;

        //    public MyOverloadProvider(ImmutableArray<TaggedText> t)
        //    {
        //        _items = new List<(string, string)>();
        //        foreach (var d in t)
        //            _items.Add((d.Text, d.Tag));
        //    }

        //    public MyOverloadProvider(IList<(string header, string content)> items)
        //    {
        //        _items = items;
        //        SelectedIndex = 0;
        //    }

        //    public int SelectedIndex
        //    {
        //        get => _selectedIndex;
        //        set
        //        {
        //            _selectedIndex = value;
        //            OnPropertyChanged();
        //            // ReSharper disable ExplicitCallerInfoArgument
        //            OnPropertyChanged(nameof(CurrentHeader));
        //            OnPropertyChanged(nameof(CurrentContent));
        //            // ReSharper restore ExplicitCallerInfoArgument
        //        }
        //    }

        //    public int Count => _items.Count;
        //    public string CurrentIndexText => null;
        //    public object CurrentHeader => _items[SelectedIndex].header;
        //    public object CurrentContent => _items[SelectedIndex].content;

        //    public event PropertyChangedEventHandler PropertyChanged;

        //    private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        //    {
        //        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        //    }
        //}
        //public class MyCompletionData : ICompletionData
        //{
        //    public MyCompletionData(string text)
        //    {
        //        Text = text;
        //    }

        //    public IBitmap Image => null;

        //    public string Text { get; }

        //    // Use this property if you want to show a fancy UIElement in the list.
        //    public object Content => Text;

        //    public object Description => "Description for " + Text;

        //    public double Priority { get; } = 0;

        //    public void Complete(TextArea textArea, ISegment completionSegment,
        //        EventArgs insertionRequestEventArgs)
        //    {
        //        textArea.Document.Replace(completionSegment, Text);
        //    }
        //}



        //class ElementGenerator : VisualLineElementGenerator, IComparer<Pair>
        //{
        //    public List<Pair> controls = new List<Pair>();

        //    /// <summary>
        //    /// Gets the first interested offset using binary search
        //    /// </summary>
        //    /// <returns>The first interested offset.</returns>
        //    /// <param name="startOffset">Start offset.</param>
        //    public override int GetFirstInterestedOffset(int startOffset)
        //    {
        //        int pos = controls.BinarySearch(new Pair(startOffset, null), this);
        //        if (pos < 0)
        //            pos = ~pos;
        //        if (pos < controls.Count)
        //            return controls[pos].Key;
        //        else
        //            return -1;
        //    }

        //    public override VisualLineElement ConstructElement(int offset)
        //    {
        //        int pos = controls.BinarySearch(new Pair(offset, null), this);
        //        if (pos >= 0)
        //            return new InlineObjectElement(0, controls[pos].Value);
        //        else
        //            return null;
        //    }

        //    int IComparer<Pair>.Compare(Pair x, Pair y)
        //    {
        //        return x.Key.CompareTo(y.Key);
        //    }
        //}

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);

            tank.SimulationCode = _editor.Document.Text;
        }
    }

    
}
