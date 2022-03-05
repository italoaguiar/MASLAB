using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MASLAB.Services
{
    internal class CompilationService
    {
        private CompilationService() { }

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


        private static string documentCode;
        private static SourceText sourceText;

        //singleton
        private static CompilationService service = new CompilationService();



        /// <summary>
        /// Efetua o carregamento do código para a análise no mecanismo Roslyn
        /// </summary>
        /// <param name="code">Código C# a ser analisado</param>
        /// <returns>Instância do serviço de análise</returns>
        public static CompilationService LoadCode(string code)
        {
            documentCode = code;
            sourceText = SourceText.From(documentCode);

            return service;
        }


        public async Task<T> Compile<T>(string className)
        {
            return await Task.Run(() =>
            {
                var tree = SyntaxFactory.ParseSyntaxTree(sourceText, CSharpParseOptions.Default);

                var compilation = CSharpCompilation.Create("MASLAB.Script",
                    new SyntaxTree[] { tree },
                    DefaultReferences,
                    options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

                using(MemoryStream ms = new MemoryStream())
                {
                    EmitResult emit = compilation.Emit(ms);

                    if (!emit.Success)
                    {
                        throw new CompilationException(emit.Diagnostics);
                    }

                    Assembly assembly = Assembly.Load(ms.ToArray());

                    return (T)assembly.CreateInstance(className);

                }
                               
            });
        }
    }

    internal class CompilationException : Exception
    {
        public CompilationException(ImmutableArray<Diagnostic> Errors) : base("Compilation Failed")
        {
            this.Errors = Errors;
        }

        public ImmutableArray<Diagnostic> Errors { get; private set; }
    }

    internal class CompilationDiagnostic
    {
        public CompilationDiagnostic(Diagnostic diagnostic)
        {
            Diagnostic = diagnostic;
        }

        public Diagnostic Diagnostic { get; private set; }

        public int Line { get; set; }
        public int Column { get; set; }
    }
}
