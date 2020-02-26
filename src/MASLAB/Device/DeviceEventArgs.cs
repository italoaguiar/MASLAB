using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Device
{
    /// <summary>
    /// Representa um evento relacionado a um dispositivo
    /// </summary>
    public class DeviceEventArgs:EventArgs
    {
        /// <summary>
        /// Representa um relé do dispositivo
        /// </summary>
        public Relays Relay { get; set; }

        /// <summary>
        /// Representa o estado do dispositivo
        /// </summary>
        public DeviceState State { get; set; }
    }
}
