using System;
using System.Collections.Generic;
using System.Text;
using ReactiveUI;

namespace MASLAB.ViewModels
{
    /// <summary>
    /// Representa a ViewModel da janela de configurações da simulação
    /// </summary>
    public class SimulationSettingsViewModel:ViewModelBase
    {
        TimeSpan duration = TimeSpan.FromSeconds(2);
        TimeSpan interval = TimeSpan.FromMilliseconds(50);
        string durationString = "00:02:000";
        string intervalString = "00:00:050";
        bool isRealTime = true;

        /// <summary>
        /// Duração da simulação
        /// </summary>
        public TimeSpan Duration 
        { 
            get => duration;
            set => this.RaiseAndSetIfChanged(ref duration, value);
        }

        /// <summary>
        /// Intervalo entre chamadas de método
        /// </summary>
        public TimeSpan Interval 
        { 
            get => interval;
            set => this.RaiseAndSetIfChanged(ref interval, value);
        }

        /// <summary>
        /// String formatada de tempo de duração
        /// </summary>
        public string DurationString 
        { 
            get => durationString;
            set
            {
                this.RaiseAndSetIfChanged(ref durationString, value);
                try
                {
                    Duration = TimeSpan.ParseExact(value, @"m\:s\:fff", System.Globalization.CultureInfo.InvariantCulture);
                }
                catch { }
            }
        }

        /// <summary>
        /// String formatada de tempo de intervalo
        /// </summary>
        public string IntervalString 
        { 
            get => intervalString;
            set
            {
                this.RaiseAndSetIfChanged(ref intervalString, value);
                try
                {
                    Interval = TimeSpan.ParseExact(value, @"m\:s\:fff", System.Globalization.CultureInfo.InvariantCulture);
                }
                catch { }
            }
        }

        /// <summary>
        /// Determina o tipo de simulação
        /// </summary>
        public bool IsRealTime 
        { 
            get => isRealTime;
            set => this.RaiseAndSetIfChanged(ref isRealTime, value);
        }
    }
}
