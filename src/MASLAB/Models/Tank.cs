using Avalonia;
using Avalonia.Platform;
using MASLAB;
using MASLAB.Models;
using MASLAB.Services;
using Newtonsoft.Json;
using Simulation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MASL.Controls.DataModel
{
    /// <summary>
    /// Representa um tanque no modelo
    /// </summary>
    public class Tank: INotifyPropertyChanged
    {
        static Tank()
        {
            if(DefaultCode == null)
            {
                var assets = AvaloniaLocator.Current.GetService<IAssetLoader>();
                var st = new StreamReader(assets.Open(new Uri("avares://MASLAB/ScriptTemplate.txt")));

                DefaultCode = st.ReadToEnd();
            }
        }

        private static string DefaultCode = null;
        private long tickCount = 0;

        /// <summary>
        /// Cria uma nova instância de Tank
        /// </summary>
        /// <param name="parent">Nível pai do Tank</param>
        public Tank(Level parent)
        {
            Parent = parent;

            RemoveCommand = new CommandAdapter(true)
            {
                Action = (p) => 
                {
                    //remove o tanque
                    Parent.Items.Remove((Tank)p);

                    //procura pelas conexões associadas ao tanque
                    var t = Parent.Project.Connections.Where(x => x.Origin.Tank == p || x.Target.Tank == p).ToList();

                    //remove as conexões associadas ao tanque
                    foreach(var tank in t) { Parent.Project.Connections.Remove(tank); }
                }
            };

            SimulationCode = DefaultCode;
        }

        private Level parent;
        private double tankLevel;
        private double leftOutput;
        private double rightOutput;

        /// <summary>
        /// Representa o Nível que o tanque ocupa
        /// </summary>
        public Level Parent 
        { 
            get => parent;
            set
            {
                parent = value;
                OnPropertyChanged(nameof(Parent));
            }
        }

        /// <summary>
        /// Representa o nível do tanque atual
        /// </summary>
        public double TankLevel
        {
            get => tankLevel;
            set
            {
                tankLevel = value;
                OnPropertyChanged(nameof(TankLevel));
            }
        }

        /// <summary>
        /// Saída calculada na simulação
        /// </summary>
        public double LeftOutput
        {
            get => leftOutput;
            set
            {
                leftOutput = value;
                OnPropertyChanged(nameof(LeftOutput));
            }
        }

        /// <summary>
        /// Saída calculada na simulação
        /// </summary>
        public double RightOutput
        {
            get => rightOutput;
            set
            {
                rightOutput = value;
                OnPropertyChanged(nameof(RightOutput));
            }
        }

        /// <summary>
        /// Representa o código da simulação escrito no editor de texto
        /// </summary>
        public string SimulationCode { get; set; }


        /// <summary>
        /// Representa o comando de remoção do tanque
        /// </summary>
        [JsonIgnore]
        public CommandAdapter RemoveCommand { get; private set; }

        /// <summary>
        /// Representa a instância compilada do código da simulação
        /// </summary>
        [JsonIgnore]
        public SimulationTank SimulationTank { get; set; }
        


        /// <summary>
        /// Método de compilação do código da simulação
        /// </summary>
        /// <returns>Instância da classe de simulação escrita no editor de texto</returns>
        public async Task<SimulationTank> Compile()
        {
            SimulationTank = await CompilationService.LoadCode(SimulationCode).Compile<SimulationTank>("Tank");
            return SimulationTank;
        }

        /// <summary>
        /// Atualiza o estado do tanque para o instante atual de tempo
        /// </summary>
        /// <param name="currentTime">Instante atual de tempo</param>
        /// <param name="interval">Intervalo entre chamadas de função</param>
        /// <param name="input1">Entrada 1 do tanque</param>
        /// <param name="input2">Entrada 2 do tanque</param>
        /// <param name="output1">Saída 1 do tanque</param>
        /// <param name="output2">Saída 2 do tanque</param>
        public void UpdateTank(TimeSpan currentTime, TimeSpan interval, double input1, double input2, double output1, double output2)
        {
            if (SimulationTank != null)
            {
                var s = SimulationTank.OnUpdate(currentTime.TotalSeconds, interval.TotalSeconds, input1, input2, output1, output2);

                //não atualizar o nível durante a inicialização
                if(tickCount > Simulator.INITIALIZATION_TICKS)
                    SimulationTank.Level = TankLevel = s.Level > 0 ? s.Level : 0;

                LeftOutput = s.LeftOutput;
                RightOutput = s.RightOutput;

                tickCount++;
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
