using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace MASLAB.Services
{
    /// <summary>
    /// Serviço de erros de compilação e análise de código
    /// </summary>
    internal class ErrorService
    {
        /// <summary>
        /// Cria uma nova instância de ErrorService
        /// </summary>
        private ErrorService() { }

        private static ErrorService Service = new ErrorService();

        public static ObservableCollection<CompilationDiagnostic> Errors { get; private set; } = new ObservableCollection<CompilationDiagnostic>();

        /// <summary>
        /// Obtém a instância atual do serviço
        /// </summary>
        public static ErrorService GetService() => Service;

        /// <summary>
        /// Adiciona uma coleção de erros
        /// </summary>
        /// <param name="errors">Coleção a ser adicionada</param>
        public void AddRange(IEnumerable<CompilationDiagnostic> errors)
        {
            foreach (var e in errors)
                Add(e);
        }

        /// <summary>
        /// Adiciona uma coleção de erros
        /// </summary>
        /// <param name="errors">Coleção a ser adicionada</param>
        public void AddRange(IEnumerable<Diagnostic> errors)
        {
            foreach (var e in errors)
                Add(new CompilationDiagnostic(e));
        }

        /// <summary>
        /// Adiciona um erro 
        /// </summary>
        /// <param name="error">Erro a ser adicionado</param>
        public void Add(CompilationDiagnostic error)
        {
            Errors.Add(error);
        }


        /// <summary>
        /// Limpa a coleção de erros
        /// </summary>
        public void Clear()
        {
            Errors.Clear();
        }
    }
}
