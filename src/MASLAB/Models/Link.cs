using Avalonia;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MASL.Controls.DataModel
{
    /// <summary>
    /// Representa a conexão entre dois tanques
    /// </summary>
    public class Link: INotifyPropertyChanged
    {
        private Connection _origin;
        private Connection _target;
        private IList<Point> _points = new List<Point>();

        /// <summary>
        /// Representa a conexão de origem entre dois tanques
        /// </summary>
        public Connection Target 
        { 
            get => _target;
            set
            {
                _target = value;
                OnPropertyChanged(nameof(Target));
            }
        }

        /// <summary>
        /// Representa a conexão de destino entre dois tanques
        /// </summary>
        public Connection Origin 
        { 
            get => _origin;
            set
            {
                _origin = value;
                OnPropertyChanged(nameof(Origin));
            }
        }

        /// <summary>
        /// Representa o conjunto de pontos da ligação
        /// </summary>
        public IList<Point> Points 
        { 
            get => _points;
            set
            {
                _points = value;
                OnPropertyChanged(nameof(Points));
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
