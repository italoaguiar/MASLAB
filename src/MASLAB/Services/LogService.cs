using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace MASLAB.Services
{
    /// <summary>
    /// Serviço de log da simulação
    /// </summary>
    public class LogService
    {
        /// <summary>
        /// Cria uma nova instância de LogService
        /// </summary>
        private LogService() { }

        private static LogService Service = new LogService();

        /// <summary>
        /// Obtém a instância atual do serviço de Log
        /// </summary>
        public static LogService GetService() => Service;


        /// <summary>
        /// Obtém o cojunto de dados do log da simulação
        /// </summary>
        public static LogServiceData Data { get; private set; } = new LogServiceData();


        /// <summary>
        /// Escreve uma string no log da simulação
        /// </summary>
        public void Log(string value)
        {
            Data.Add(value);
        }

        /// <summary>
        /// Limpa os dados do log da simulação
        /// </summary>
        public void Clear()
        {
            Data.Clear();
        }


    }

    /// <summary>
    /// Representa os dados do log da simulação
    /// </summary>
    public class LogServiceData : INotifyPropertyChanged
    {
        private string log = string.Empty;

        /// <summary>
        /// Obtém a string de log da simulação
        /// </summary>
        public string Log
        {
            get => log;
            private set
            {
                log = value;
                OnPropertyChanged("Log");
            }
        }

        /// <summary>
        /// Inclui uma nova string no log
        /// </summary>
        /// <param name="value">string a ser incluída no log</param>
        public void Add(string value)
        {
            Log += value + Environment.NewLine;
        }

        /// <summary>
        /// Limpa os dados do log
        /// </summary>
        public void Clear()
        {
            Log = string.Empty;
        }


        private void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        /// <summary>
        /// Notifica a alteração de uma propriedade
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
