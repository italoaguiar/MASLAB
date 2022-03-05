using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Text;

namespace MASLAB.ViewModels
{
    /// <summary>
    /// ViewModel responsável por controlar o comportamento de uma mola
    /// </summary>
    public class SpringViewModel : ViewModelBase
    {
        /// <summary>
        /// Cria uma nova instância de SpringViewModel
        /// </summary>
        public SpringViewModel(UserControl uc)
        {
            userControl = uc;
        }
        
        /// <summary>
        /// Coordenada X
        /// </summary>
        public double X
        {
            get => x;
            set
            {
                this.RaiseAndSetIfChanged(ref x, value);
            }
        }

        /// <summary>
        /// Coordenada Y
        /// </summary>
        public double Y
        {
            get => y;
            set
            {
                this.RaiseAndSetIfChanged(ref y, value);
            }
        }




        
        private UserControl userControl;
        private double x = 0;
        private double y = 0;
        
    }
}
