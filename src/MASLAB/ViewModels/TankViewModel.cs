using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using MASL.Controls.DataModel;
using MASLAB.Models;
using MASLAB.Services;
using MASLAB.Views;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace MASLAB.ViewModels
{
    public class TankViewModel:ViewModelBase
    {
        public TankViewModel()
        {
            InputConnectionCommand = new CommandAdapter(true, InputConnectionRequested);
            OutputConnectionCommand = new CommandAdapter(true, OutputConnectionRequested);

            
        }


        /// <summary>
        /// Especifica o comando para conexões de entrada do tanque
        /// </summary>
        public ICommand InputConnectionCommand { get; set; }

        /// <summary>
        /// Especifica o comando para conexões de saída do tanque
        /// </summary>
        public ICommand OutputConnectionCommand { get; set; }


        /// <summary>
        /// Especifica o controle pai que contém o diagrama
        /// </summary>
        public IControl ParentContainer { get; set; }
        
        /// <summary>
        /// Representa a posição atual do mouse em relação ao controle pai
        /// </summary>
        public Point CurrentPoint { get; set; }


        /// <summary>
        /// Representa o objeto de dados do tanque atual
        /// </summary>
        public MASL.Controls.DataModel.Tank Tank { get; set; }


        


        /// <summary>
        /// Trata as requisições de conexões de entrada
        /// </summary>
        /// <param name="p">Parâmetro especificado no DataBinding</param>
        private async void InputConnectionRequested(object p)
        {
            try
            {
                ConnectionHelper.RequestConnection(new Connection()
                {
                    ConnectionType = ConnectionType.Input,
                    Position = CurrentPoint,
                    Tank = Tank
                });
            }
            catch(Exception e)
            {
                await MessageBox.Show(e.Message);
            }
        }

        

        /// <summary>
        /// Trata as Requisições de conexões de saída
        /// </summary>
        /// <param name="p">Parâmetro especificado no DataBinding</param>
        private async void OutputConnectionRequested(object p)
        {
            try
            {
                ConnectionHelper.RequestConnection(new Connection()
                {
                    ConnectionType = ConnectionType.Output,
                    Position = CurrentPoint,
                    Tank = Tank
                });
            }
            catch(Exception e)
            {
                await MessageBox.Show(e.Message);
            }
        }
    }
}
