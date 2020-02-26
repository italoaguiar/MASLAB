using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Device
{
    /// <summary>
    /// Representa o modo de leitura do dispositivo
    /// </summary>
    public enum ReadMode
    {
        /// <summary>
        /// Modo controlado. O dispositivo envia leitura somente quando solicitado
        /// </summary>
        Controlled,

        /// <summary>
        /// Modo contínuo. O dispositivo envia leituras continuamente.
        /// </summary>
        Continuous
    }
}
