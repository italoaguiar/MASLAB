using Avalonia.Controls;
using Avalonia.Media.Imaging;
using AvaloniaEdit.CodeCompletion;
using AvaloniaEdit.Document;
using AvaloniaEdit.Editing;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Completion;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Host.Mef;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.Emit;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition.Hosting;
using System.IO;
using System.Runtime;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;

namespace MASLAB.Services
{
    /// <summary>
    /// Serviço de análise C# para o editor de código
    /// </summary>
    internal class CodeAnalysisService
    {
        private CodeAnalysisService() { }

        static CodeAnalysisService()
        {
            

            host = MefHostServices.Create(MefHostServices.DefaultAssemblies);
            workspace = new AdhocWorkspace(host); 

            projectInfo = ProjectInfo.Create(
                ProjectId.CreateNewId(),
                VersionStamp.Create(),
                "codeAnalysis",
                "CodeAnalysis",
                LanguageNames.CSharp
                )
                .WithMetadataReferences(DefaultReferences);   
        }

        

        private static string assemblyDir = Path.GetDirectoryName(typeof(object).Assembly.Location);
        private static string appDir = Path.GetDirectoryName(typeof(App).Assembly.Location);
        private static readonly IEnumerable<MetadataReference> DefaultReferences =
            new[]
            {
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location, documentation: XmlDocumentationProvider.CreateFromFile($"{appDir}/XML/System.Runtime.Extensions.xml")),
                MetadataReference.CreateFromFile(typeof(App).Assembly.Location, documentation: XmlDocumentationProvider.CreateFromFile($"{appDir}/MASLAB.xml")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyDir, "System.Runtime.dll"), documentation: XmlDocumentationProvider.CreateFromFile($"{appDir}/XML/System.Runtime.xml")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyDir, "System.Private.Uri.dll")),
                MetadataReference.CreateFromFile(Path.Combine(appDir, "System.IO.Ports.dll"), documentation: XmlDocumentationProvider.CreateFromFile($"{appDir}/XML/System.IO.Ports.xml")),
                MetadataReference.CreateFromFile(Path.Combine(appDir, "System.Text.Json.dll"), documentation: XmlDocumentationProvider.CreateFromFile($"{appDir}/XML/System.Text.Json.xml")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyDir, "System.Net.Http.dll"), documentation: XmlDocumentationProvider.CreateFromFile($"{appDir}/XML/System.Net.Http.xml")),
                MetadataReference.CreateFromFile(Path.Combine(appDir, "MathNet.Numerics.dll"), documentation: XmlDocumentationProvider.CreateFromFile($"{appDir}/XML/MathNet.Numerics.xml")),
                MetadataReference.CreateFromFile(Path.Combine(appDir, "FSharp.Core.dll"), documentation: XmlDocumentationProvider.CreateFromFile($"{appDir}/XML/FSharp.Core.xml")),
                MetadataReference.CreateFromFile(Path.Combine(appDir, "FParsec.dll"), documentation: XmlDocumentationProvider.CreateFromFile($"{appDir}/XML/FParsec.xml")),
                MetadataReference.CreateFromFile(Path.Combine(appDir, "FParsecCS.dll"), documentation: XmlDocumentationProvider.CreateFromFile($"{appDir}/XML/FParsecCS.xml")),                
            };

        //singleton
        private static CodeAnalysisService service = new CodeAnalysisService();

        private static MefHostServices host;
        private static AdhocWorkspace workspace;
        private static Document document;
        private static string documentCode;
        private static SourceText sourceText;
        private static ProjectInfo projectInfo;




        /// <summary>
        /// Efetua o carregamento do código para a análise no mecanismo Roslyn
        /// </summary>
        /// <param name="code">Código C# a ser analisado</param>
        /// <returns>Instância do serviço de análise</returns>
        public static CodeAnalysisService LoadDocument(string code)
        {
            documentCode = code;
            sourceText = SourceText.From(documentCode);

            workspace.ClearSolution();
            var project = workspace.AddProject(projectInfo);
            document = workspace.AddDocument(project.Id, "CodeAnalysis.cs", sourceText);

            return service;
        }





        /// <summary>
        /// Método utilizado para obter as sugestões do recurso
        /// auto completar do editor de código.
        /// </summary>
        /// <param name="position">Posição atual do cursor</param>
        /// <returns>Conjunto de sugestões para o editor</returns>
        public async Task<IList<ICompletionData>> GetCompletitionDataAt(int position)
        {
            return await Task.Run<IList<ICompletionData>>( async () =>
            {
                var completionService = CompletionService.GetService(document);
                var results = await completionService.GetCompletionsAsync(document, position);

                List<ICompletionData> data = new List<ICompletionData>();

                if (results != null)
                {
                    foreach (var item in results.Items)
                    {
                        data.Add(new CompletionData(item.DisplayText, item.Tags)
                        {
                           ContentDescription = new Lazy<object>(()=> completionService.GetDescriptionAsync(document, item).Result.Text)
                        });
                    }
                }
                
                return data;
            });
        }



        



        /// <summary>
        /// Método utilizado para determinar se o caractere
        /// digitado pelo usuário pode ser utilizado para
        /// abrir a janela de sugestões do editor
        /// </summary>
        /// <param name="position">Posição atual do cursor</param>
        /// <returns>True se o caractere digitado é um gatilho, False do contrário</returns>
        public async Task<bool> ShouldTriggerCompletion(int position)
        {
            return await Task.Run(() =>
            {
                if (position < 1)
                    return false;

                var completionService = CompletionService.GetService(document);
                return completionService.ShouldTriggerCompletion(sourceText, position, CompletionTrigger.CreateInsertionTrigger(documentCode[position - 1]));
            });
        }



        /// <summary>
        /// Métudo utilizado para obter erros de sintaxe no código
        /// </summary>
        /// <returns>Conjunto de erros e avisos detectados</returns>
        public Task<ImmutableArray<Diagnostic>> GetDiagnosticsAsync()
        {
            return Task.Run(() =>
            {
                var tree = SyntaxFactory.ParseSyntaxTree(sourceText, CSharpParseOptions.Default);

                var compilation = CSharpCompilation.Create("MASLAB.Script", 
                    new SyntaxTree[] { tree }, 
                    DefaultReferences,
                    options:new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

                return compilation.GetDiagnostics();
            });            
        }




        /// <summary>
        /// Obtém a descrição para um determinado token em uma posição
        /// </summary>
        /// <param name="position">Posição do token</param>
        /// <returns>Descrição do token</returns>
        public async Task<string> GetInfoAt(int position)
        {
            return await Task.Run(async () =>
            {
                var service = Microsoft.CodeAnalysis.QuickInfo.QuickInfoService.GetService(document);

                var info = await service.GetQuickInfoAsync(document, position);

                return info != null ? string.Join(Environment.NewLine, info.Sections.Select(x=> x.Text)) : null;
            });
        }


        public async Task<List<(string,string)>> GetMethodSignature(int position)
        {
            return await Task.Run(async () =>
            {
                try
                {
                    var root = await document.GetSyntaxRootAsync();
                    var model = await document.GetSemanticModelAsync();
                    var token = root.FindToken(position);
                    var node = root.FindNode(token.Span);

                    var group = model.GetMemberGroup(node);
                    
                    if (group != null && group.Length > 0)
                    {
                        List<(string, string)> r = new List<(string, string)>();
                        foreach (var i in group)
                        {
                            r.Add((i.ToDisplayString(), i.ToMinimalDisplayString(model,position)));
                        }
                        return r;
                    }
                    return null;
                    
                }
                catch { return null; }
            });
        }


        public class CompletionData : ICompletionData
        {
            public CompletionData(string text, IList<string> tags)
            {
                Text = text;
                Image = Icons.Find(tags);
            }

            public IBitmap Image { get; }

            public string Text { get; }

            // Use this property if you want to show a fancy UIElement in the list.
            public object Content => new TextBlock() { Text = Text };

            internal Lazy<object> ContentDescription { get; set; }

            public object Description {
                get
                {
                    var r = ContentDescription.Value;
                    System.Diagnostics.Debug.WriteLine(r);
                    return r;
                }
            }

            public double Priority { get; } = 0;

            public void Complete(TextArea textArea, ISegment completionSegment,
                EventArgs insertionRequestEventArgs)
            {

                textArea.Document.Replace(completionSegment, Text);
            }
        }
    }
}
