using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace MASLAB.Models
{
    /// <summary>
    /// Cria um adaptador para a interface ICommand
    /// </summary>
    public class CommandAdapter : ICommand
    {
        /// <summary>
        /// Cria uma nova instância de CommandAdapter
        /// </summary>
        public CommandAdapter()
        {

        }

        /// <summary>
        /// Cria uma nova instância de CommandAdapter
        /// </summary>
        /// <param name="canExecute">Determina se o comando pode ser executado</param>
        public CommandAdapter(bool canExecute)
        {
            SetCanExecute(canExecute);
        }

        /// <summary>
        /// Cria uma nova instância de CommandAdapter
        /// </summary>
        /// <param name="action">Determina a ação a ser executada pelo comando</param>
        public CommandAdapter(Action<object> action)
        {
            Action = action;
        }

        /// <summary>
        /// Cria uma nova instância de CommandAdapter
        /// </summary>
        /// <param name="canExecute">Determina se o comando pode ser executado</param>
        /// <param name="action">Determina a ação a ser executada pelo comando</param>
        public CommandAdapter(bool canExecute, Action<object> action) : this(canExecute)
        {
            Action = action;
        }

        /// <summary>
        /// Notifica se o comando pode ser executado ou não
        /// </summary>
        public event EventHandler CanExecuteChanged;

        bool _canExecute = true;

        /// <summary>
        /// Obtém a possibilidade de execução do comando.
        /// </summary>
        /// <param name="parameter">Parâmetro do comando</param>
        /// <returns>Valor indicando se o comando pode ser executado.</returns>
        public bool CanExecute(object parameter)
        {
            return _canExecute;
        }

        /// <summary>
        /// Ação a ser executada pelo comando.
        /// </summary>
        public Action<object> Action { get; set; }

        /// <summary>
        /// Atribui a possibilidade de execução do comando
        /// </summary>
        /// <param name="canExecute">Valor indicando se o comando pode ser executado.</param>
        public void SetCanExecute(bool canExecute)
        {
            _canExecute = canExecute;
            CanExecuteChanged?.Invoke(this, new EventArgs());
        }

        /// <summary>
        /// Executa o comando
        /// </summary>
        /// <param name="parameter">Parâmetro do comando</param>
        public void Execute(object parameter)
        {
            Action?.Invoke(parameter);
        }
    }
}
