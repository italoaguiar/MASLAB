using Avalonia;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MASL.Controls.DataModel
{
    /// <summary>
    /// Representa uma das conexões de um tanque
    /// </summary>
    public class Connection: INotifyPropertyChanged
    {
        private Point _position;
        private ConnectionType _connectionType;
        private Tank tank;

        /// <summary>
        /// Representa a coordenada da conexão do tanque
        /// </summary>
        public Point Position 
        { 
            get => _position;
            set
            {
                _position = value;
                OnPropertyChanged(nameof(Position));
            }
        }

        /// <summary>
        /// Representa o tipo de conexão do tanque: Entrada ou saída.
        /// </summary>
        public ConnectionType ConnectionType 
        { 
            get => _connectionType;
            set
            {
                _connectionType = value;
                OnPropertyChanged(nameof(ConnectionType));
            }
        }

        /// <summary>
        /// Representa o tanque de origem da conexão
        /// </summary>
        public Tank Tank 
        { 
            get => tank;
            set
            {
                tank = value;
                OnPropertyChanged(nameof(Tank));
            }
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Notifica uma alteração ocorrida em uma propriedade da classe
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
