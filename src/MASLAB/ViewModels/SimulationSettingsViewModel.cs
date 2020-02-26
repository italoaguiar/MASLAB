using System;
using System.Collections.Generic;
using System.Text;
using ReactiveUI;

namespace MASLAB.ViewModels
{
    public class SimulationSettingsViewModel:ViewModelBase
    {
        TimeSpan duration = TimeSpan.FromSeconds(2);
        TimeSpan interval = TimeSpan.FromMilliseconds(50);
        string durationString = "00:02:000";
        string intervalString = "00:00:050";
        bool isRealTime = true;

        public TimeSpan Duration 
        { 
            get => duration;
            set => this.RaiseAndSetIfChanged(ref duration, value);
        }

        public TimeSpan Interval 
        { 
            get => interval;
            set => this.RaiseAndSetIfChanged(ref interval, value);
        }

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

        public bool IsRealTime 
        { 
            get => isRealTime;
            set => this.RaiseAndSetIfChanged(ref isRealTime, value);
        }
    }
}
