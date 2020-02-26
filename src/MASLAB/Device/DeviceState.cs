using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Device
{
    /// <summary>
    /// Representa o estado do dispositivo
    /// </summary>
    public enum DeviceState
    {
        /// <summary>
        /// Representa o dispositivo como LIGADO
        /// </summary>
        Off = 0x0,

        /// <summary>
        /// Representa o dispositivo como DESLIGADO
        /// </summary>
        On = 0x1
    }
}
