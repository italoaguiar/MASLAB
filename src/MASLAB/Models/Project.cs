using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MASL.Controls.DataModel
{
    /// <summary>
    /// Representa um diagrama criado no software
    /// </summary>
    public class Project : INotifyPropertyChanged
    {

        private ObservableCollection<Level> _levels;
        private ObservableCollection<Link> _connections;
        private string _projectName;
        private string _author;

        /// <summary>
        /// Representa os níveis físicos (andares) que contêm os tanques
        /// </summary>
        public ObservableCollection<Level> Levels 
        { 
            get => _levels;
            set
            {
                _levels = value;
                OnPropertyChanged(nameof(Levels));
            }
        }

        /// <summary>
        /// Representa as conexões entre tanques
        /// </summary>
        public ObservableCollection<Link> Connections 
        { 
            get => _connections;
            set
            {
                _connections = value;
                OnPropertyChanged(nameof(Connections));
            }
        }

        /// <summary>
        /// Representa o nome do projeto
        /// </summary>
        public string ProjectName 
        { 
            get => _projectName;
            set
            {
                _projectName = value;
                OnPropertyChanged(nameof(ProjectName));
            }
        }

        /// <summary>
        /// Representa o nome do Autor do projeto
        /// </summary>
        public string Author 
        { 
            get => _author;
            set
            {
                _author = value;
                OnPropertyChanged(nameof(Author));
            }
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Notifica a alteração de propriedades da classe.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
