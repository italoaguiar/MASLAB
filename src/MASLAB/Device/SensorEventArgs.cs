using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Device
{
    /// <summary>
    /// Representa um evento relacionado a um sensor do dispositivo
    /// </summary>
    public class SensorEventArgs: EventArgs
    {
        /// <summary>
        /// Representa o sensor de origem do evento
        /// </summary>
        public Sensors Sensor { get; set; }

        /// <summary>
        /// Representa o dado lido pelo sensor
        /// </summary>
        public double Data { get; set; }
    }
}
