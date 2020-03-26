using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MASL.Controls.DataModel
{
    /// <summary>
    /// Representa um nível da simulação
    /// </summary>
    public class Level: INotifyPropertyChanged
    {
        private ObservableCollection<Tank> _items;
        private string _levelName;
        private ICommand _addCommand;

        /// <summary>
        /// Cria uma nova instância de Level
        /// </summary>
        /// <param name="parent">Projeto pai do Level</param>
        public Level(Project parent)
        {
            Project = parent;
        }

        /// <summary>
        /// Representa a coleção de tanques individuais de cada nível
        /// </summary>
        public ObservableCollection<Tank> Items 
        { 
            get => _items;
            set
            {
                _items = value;
                OnPropertyChanged(nameof(Items));
            }
        }

        /// <summary>
        /// Representa o nome do nível
        /// </summary>
        public string Name 
        { 
            get => _levelName;
            set
            {
                _levelName = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        /// <summary>
        /// Representa um comando de adição
        /// </summary>
        [JsonIgnore]
        public ICommand AddCommand 
        { 
            get => _addCommand;
            set
            {
                _addCommand = value;
                OnPropertyChanged(nameof(AddCommand));
            }
        }

        /// <summary>
        /// Projeto pai do Level
        /// </summary>
        public Project Project { get; private set; }

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
